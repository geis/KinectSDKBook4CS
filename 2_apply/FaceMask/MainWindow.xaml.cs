using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Kinect;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace FaceMask
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window
  {
    private KinectSensor kinect;
    private WriteableBitmap outputImage;

    // 画像データ配列
    private byte[] BGR32PixelData;
    private byte[] RGB24PixelData;

    // OpenCV用
    private IplImage openCVImage;
    private IplImage openCVGrayImage;
    private CvMemStorage storage;
    private CvHaarClassifierCascade cascade;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public MainWindow()
    {
      InitializeComponent();

      StartKinect(KinectSensor.KinectSensors[0]);
      InitOpenCV();
    }

    /// <summary>
    /// Kinectの動作を開始する
    /// </summary>
    /// <param name="kin"></param>
    private void StartKinect(KinectSensor kin)
    {
      try {
        if ( KinectSensor.KinectSensors.Count == 0 ) {
          throw new Exception( "Kinectが接続されていません" );
        }

        kinect = kin;
        kinect.ColorStream.Enable();
        kinect.SkeletonStream.Enable();
        kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>( kinect_AllFramesReady );

        kinect.Start();
      }
      catch ( Exception ex ) {
        MessageBox.Show( ex.Message );
        Close();
      }
    }

    /// <summary>
    /// Kinectの動作を停止する
    /// </summary>
    /// <param name="kinect"></param>
    private void StopKinect( KinectSensor kinect )
    {
      if ( kinect != null ) {
        if ( kinect.IsRunning ) {
          kinect.AllFramesReady -= kinect_AllFramesReady;

          kinect.Stop();
          kinect.Dispose();
        }
      }
    }

    /// <summary>
    /// OpenCV関連の変数初期化
    /// </summary>
    private void InitOpenCV()
    {
      openCVImage = new IplImage(
          kinect.ColorStream.FrameWidth,
          kinect.ColorStream.FrameHeight,
          BitDepth.U8, 3 );

      openCVGrayImage = new IplImage( kinect.ColorStream.FrameWidth, kinect.ColorStream.FrameHeight, BitDepth.U8, 1 );

      storage = new CvMemStorage();
      cascade = CvHaarClassifierCascade.FromFile( "../../haarcascade_frontalface_alt2.xml" );
    }

    /// <summary>
    /// RGBカメラ、距離カメラのフレーム更新イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void kinect_AllFramesReady( object sender, AllFramesReadyEventArgs e )
    {
      // RGBカメラのフレームデータを取得する
      using ( ColorImageFrame colorFrame = e.OpenColorImageFrame() ) {
        if ( colorFrame != null ) {
          // ColorImageFrame -> ImageSource (WriteableBitmap)　変換
          image1.Source = ConvertToBitmap( colorFrame );

          // スケルトンフレームデータを取得する
          using ( SkeletonFrame skeletonFrame = e.OpenSkeletonFrame() ) {
            if ( skeletonFrame != null ) {
              // スケルトンデータを取得する
              Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
              skeletonFrame.CopySkeletonDataTo( skeletonData );

              var point = new ColorImagePoint();

              // プレーヤーごとのスケルトンから頭の位置を取得
              foreach ( var skeleton in skeletonData ) {
                var head = skeleton.Joints[JointType.Head];
                if ( head.TrackingState == JointTrackingState.Tracked ) {
                  point = kinect.MapSkeletonPointToColor( head.Position, kinect.ColorStream.Format );
                }
              }

              // 鼻眼鏡の描画
              // ・スケルトンが認識出来なかった場合は処理をしない
              // ・6フレーム毎に顔検出を行う
              if ( colorFrame != null && 
                  (point.X != 0 || point.Y != 0) && 
                   colorFrame.FrameNumber % 6 == 0 ) {
                Rect rect = CheckFacePosition( point );

                image2.Margin = new Thickness( rect.X, rect.Y, 0, 0 );
                image2.Width = rect.Width;
                image2.Height = rect.Height;
                image2.Visibility = System.Windows.Visibility.Visible;
              }
            }
          }
        }
      }
    }

    /// <summary>
    /// WriteableBitmapに変換する
    /// </summary>
    /// <param name="cif"></param>
    /// <returns></returns>
    private ImageSource ConvertToBitmap( ColorImageFrame cif )
    {
      // 変数のインスタンス作成
      if ( outputImage == null || BGR32PixelData == null ) {
        this.BGR32PixelData = new byte[cif.PixelDataLength];

        this.outputImage = new WriteableBitmap(
            cif.Width, cif.Height,
            96, 96,
            PixelFormats.Rgb24, null );

      }

      cif.CopyPixelDataTo( this.BGR32PixelData );

      // Kinectから得る画像データをOpenCVで処理できるように変換する
      RGB24PixelData = ConvertBGR32toRGB24( this.BGR32PixelData );

      // 画像情報の書き込み
      this.outputImage.WritePixels(
          new Int32Rect( 0, 0, cif.Width, cif.Height ),
          this.RGB24PixelData,
          outputImage.BackBufferStride,
          0 );

      return outputImage;
    }

    /// <summary>
    /// 32bitBGR構成の画像バイト配列を24bitRGB構成に変換する
    /// </summary>
    /// <param name="pixels"></param>
    /// <returns></returns>
    private byte[] ConvertBGR32toRGB24( byte[] pixels )
    {
      int BGR32Bits = 4;
      int RGB24Bits = 3;

      var bytes = new byte[pixels.Length / BGR32Bits * RGB24Bits];

      for ( int i = 0; i < pixels.Length / BGR32Bits; i++ ) {
        // BGR32bitからRGB24に変換
        bytes[i * RGB24Bits] = pixels[i * BGR32Bits + 2];       // R
        bytes[i * RGB24Bits + 1] = pixels[i * BGR32Bits + 1];   // G
        bytes[i * RGB24Bits + 2] = pixels[i * BGR32Bits];       // B
      }

      return bytes;
    }

    /// <summary>
    /// 顔の位置を取得
    /// </summary>
    /// <param name="headPosition">スケルトンの頭の位置座標</param>
    /// <returns>顔座標</returns>
    private Rect CheckFacePosition( ColorImagePoint headPosition )
    {
      //切り取る領域の範囲
      int snipWidth = 200;
      int snipHeight = 200;

      // 返却用Rect (初期値はスケルトンの頭の座標とimage2画像の幅)
      Rect reRect = new Rect(headPosition.X, headPosition.Y,
                             image2.Width, image2.Height);

      storage.Clear();
      openCVGrayImage.ResetROI();           // たまにROIがセットされた状態で呼ばれるためROIをリセット

      openCVImage.CopyFrom( outputImage );                                        // WriteableBitmap -> IplImage
      Cv.CvtColor( openCVImage, openCVGrayImage, ColorConversion.BgrToGray );     // 画像をグレイスケール化
      Cv.EqualizeHist( openCVGrayImage, openCVGrayImage );                        // 画像の平滑化

      // 顔認識
      try {
        // 画像の切り取り
        var snipImage = SnipFaceImage( openCVGrayImage, headPosition, snipWidth, snipHeight );

        if ( snipImage != null ) {
          CvSeq<CvAvgComp> faces = Cv.HaarDetectObjects( snipImage, cascade, storage );

          // 顔を検出した場合
          if ( faces.Total > 0 ) {
            reRect.X = faces[0].Value.Rect.X + (headPosition.X - snipWidth / 2);
            reRect.Y = faces[0].Value.Rect.Y + (headPosition.Y - snipHeight / 2);
            reRect.Width = faces[0].Value.Rect.Width;
            reRect.Height = faces[0].Value.Rect.Height;
          }
        }
      }
      catch ( Exception ) { }

      return reRect;
    }

    /// <summary>
    /// 画像を指定した領域で切り取る
    /// </summary>
    /// <param name="src">切り取る元画像</param>
    /// <param name="centerPosition">切り取る領域の中心座標</param>
    /// <param name="snipWidth">切り取る横幅</param>
    /// <param name="snipHeight">切り取る縦幅</param>
    /// <returns>切り取った画像</returns>
    private IplImage SnipFaceImage( IplImage src, ColorImagePoint centerPosition, int snipWidth, int snipHeight )
    {
      int faceX, faceY;

      // 画面からはみ出している場合は切り取り処理しない
      if ( centerPosition.X - snipWidth / 2 < 0 ||
           centerPosition.Y - snipHeight / 2 < 0 ) {
             return null;
      }
      else {
        faceX = centerPosition.X - snipWidth / 2;
        faceY = centerPosition.Y - snipHeight / 2;
      }

      // 切り取り領域の設定
      var faceRect = new CvRect( faceX, faceY, snipWidth, snipHeight );
      var part = new IplImage( faceRect.Size, BitDepth.U8, 1 );

      src.SetROI( faceRect );       // 切り取り範囲を設定
      Cv.Copy( src, part );         // データをコピー
      src.ResetROI();               // 指定した範囲のリセット

      return part;
    }

    /// <summary>
    /// ウィンドウを閉じている
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_Closing( object sender, System.ComponentModel.CancelEventArgs e )
    {
      StopKinect( KinectSensor.KinectSensors[0] );
    }
  }
}
