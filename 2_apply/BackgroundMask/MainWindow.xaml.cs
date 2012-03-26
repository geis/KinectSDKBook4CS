using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace BackgroundMask
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window
  {
    readonly int Bgr32BytesPerPixel = PixelFormats.Bgr32.BitsPerPixel / 8;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public MainWindow()
    {
      try {
        InitializeComponent();

        // Kinectが接続されているかどうかを確認する
        if ( KinectSensor.KinectSensors.Count == 0 ) {
          throw new Exception( "Kinectを接続してください" );
        }

        // Kinectの動作を開始する
        StartKinect( KinectSensor.KinectSensors[0] );
      }
      catch ( Exception ex ) {
        MessageBox.Show( ex.Message );
        Close();
      }
    }

    /// <summary>
    /// Kinectの動作を開始する
    /// </summary>
    /// <param name="kinect"></param>
    private void StartKinect( KinectSensor kinect )
    {
      // RGBカメラ、距離カメラ、スケルトン・トラッキング(プレイヤーの認識)を有効にする
      kinect.ColorStream.Enable();
      kinect.DepthStream.Enable();
      kinect.SkeletonStream.Enable();

      // RGBカメラ、距離カメラ、スケルトンのフレーム更新イベントを登録する
      kinect.AllFramesReady +=
        new EventHandler<AllFramesReadyEventArgs>( kinect_AllFramesReady );

      // Kinectの動作を開始する
      kinect.Start();
    }

    /// <summary>
    /// Kinectの動作を停止する
    /// </summary>
    /// <param name="kinect"></param>
    private void StopKinect( KinectSensor kinect )
    {
      if ( kinect != null ) {
        if ( kinect.IsRunning ) {
          // フレーム更新イベントを削除する
          kinect.AllFramesReady -= kinect_AllFramesReady;

          // Kinectの停止と、ネイティブリソースを解放する
          kinect.Stop();
          kinect.Dispose();

          imageRgb.Source = null;
        }
      }
    }

    /// <summary>
    /// RGBカメラ、距離カメラ、骨格のフレーム更新イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void kinect_AllFramesReady( object sender, AllFramesReadyEventArgs e )
    {
      try {
        // Kinectのインスタンスを取得する
        KinectSensor kinect = sender as KinectSensor;
        if ( kinect == null ) {
          return;
        }

        // 背景をマスクした画像を描画する
        using ( ColorImageFrame colorFrame = e.OpenColorImageFrame() ) {
          using ( DepthImageFrame depthFrame = e.OpenDepthImageFrame() ) {
            if ( (colorFrame != null) && (depthFrame != null) ) {
              imageRgb.Source = BitmapSource.Create( colorFrame.Width, colorFrame.Height, 96, 96,
                  PixelFormats.Bgr32, null, BackgroundMask( kinect, colorFrame, depthFrame ),
                  colorFrame.Width * colorFrame.BytesPerPixel );
            }
          }
        }
      }
      catch ( Exception ex ) {
        MessageBox.Show( ex.Message );
      }
    }

    /// <summary>
    ///  プレーヤーだけ表示する
    /// </summary>
    /// <param name="colorFrame"></param>
    /// <param name="depthFrame"></param>
    /// <returns></returns>
    private byte[] BackgroundMask( KinectSensor kinect,
      ColorImageFrame colorFrame, DepthImageFrame depthFrame )
    {
      ColorImageStream colorStream = kinect.ColorStream;
      DepthImageStream depthStream = kinect.DepthStream;

      // RGBカメラのピクセルごとのデータを取得する
      byte[] colorPixel = new byte[colorFrame.PixelDataLength];
      colorFrame.CopyPixelDataTo( colorPixel );

      // 距離カメラのピクセルごとのデータを取得する
      short[] depthPixel = new short[depthFrame.PixelDataLength];
      depthFrame.CopyPixelDataTo( depthPixel );

      // 距離カメラの座標に対応するRGBカメラの座標を取得する(座標合わせ)
      ColorImagePoint[] colorPoint = new ColorImagePoint[depthFrame.PixelDataLength];
      kinect.MapDepthFrameToColorFrame( depthStream.Format, depthPixel,
        colorStream.Format, colorPoint );

      // 出力バッファ(初期値は白(255,255,255))
      byte[] outputColor = new byte[colorPixel.Length];
      for ( int i = 0; i < outputColor.Length; i += Bgr32BytesPerPixel ) {
        outputColor[i] = 255;
        outputColor[i + 1] = 255;
        outputColor[i + 2] = 255;
      }

      for ( int index = 0; index < depthPixel.Length; index++ ) {
        // プレイヤーを取得する
        int player = depthPixel[index] & DepthImageFrame.PlayerIndexBitmask;

        // 変換した結果が、フレームサイズを超えることがあるため、小さいほうを使う
        int x = Math.Min( colorPoint[index].X, colorStream.FrameWidth - 1 );
        int y = Math.Min( colorPoint[index].Y, colorStream.FrameHeight - 1 );
        int colorIndex = ((y * depthFrame.Width) + x) * Bgr32BytesPerPixel;

        // プレーヤーを検出した座標だけ、RGBカメラの画像を使う
        if ( player != 0 ) {
          outputColor[colorIndex] = colorPixel[colorIndex];
          outputColor[colorIndex + 1] = colorPixel[colorIndex + 1];
          outputColor[colorIndex + 2] = colorPixel[colorIndex + 2];
        }
      }

      return outputColor;
    }

    /// <summary>
    /// Windowが閉じられるときのイベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_Closing( object sender, System.ComponentModel.CancelEventArgs e )
    {
      StopKinect( KinectSensor.KinectSensors[0] );
    }
  }
}
