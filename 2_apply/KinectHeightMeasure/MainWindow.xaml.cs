using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using System.Diagnostics;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace KinectHeightMeasure
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window
  {
    readonly int Bgr32BytesPerPixel = PixelFormats.Bgr32.BitsPerPixel / 8;

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

        // RGBカメラのフレームデータを取得する
        using ( ColorImageFrame colorFrame = e.OpenColorImageFrame() ) {
          if ( colorFrame != null ) {
            // RGBカメラの画像を表示する
            byte[] colorPixel = new byte[colorFrame.PixelDataLength];
            colorFrame.CopyPixelDataTo( colorPixel );
            imageRgb.Source = BitmapSource.Create( colorFrame.Width, colorFrame.Height, 96, 96,
                PixelFormats.Bgr32, null, colorPixel, colorFrame.Width * colorFrame.BytesPerPixel );
          }
        }

        // 距離カメラのフレームデータを取得する
        using ( DepthImageFrame depthFrame = e.OpenDepthImageFrame() ) {
          // スケルトンのフレームを取得する
          using ( SkeletonFrame skeletonFrame = e.OpenSkeletonFrame() ) {
            if ( (depthFrame != null) && (skeletonFrame != null) ) {
              // 身長を表示する
              HeightMeasure( kinect, depthFrame, skeletonFrame );
            }
          }
        }
      }
      catch ( Exception ex ) {
        MessageBox.Show( ex.Message );
      }
    }

    /// <summary>
    /// 距離データをカラー画像に変換する
    /// </summary>
    /// <param name="kinect"></param>
    /// <param name="depthFrame"></param>
    /// <returns></returns>
    private void HeightMeasure( KinectSensor kinect, DepthImageFrame depthFrame, SkeletonFrame skeletonFrame )
    {
      ColorImageStream colorStream = kinect.ColorStream;
      DepthImageStream depthStream = kinect.DepthStream;

      // トラッキングされている最初のスケルトンを取得する
      // インデックスはプレイヤーIDに対応しているのでとっておく
      Skeleton[] skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
      skeletonFrame.CopySkeletonDataTo( skeletons );

      int playerIndex = 0;
      for ( playerIndex = 0; playerIndex < skeletons.Length; playerIndex++ ) {
        if ( skeletons[playerIndex].TrackingState == SkeletonTrackingState.Tracked ) {
          break;
        }
      }
      if ( playerIndex == skeletons.Length ) {
        return;
      }

      // トラッキングされている最初のスケルトン
      Skeleton skeleton = skeletons[playerIndex];

      // 実際のプレイヤーIDは、スケルトンのインデックス+1
      playerIndex++;

      // 頭、足先がトラッキングされていない場合は、そのままRGBカメラのデータを返す
      Joint head = skeleton.Joints[JointType.Head];
      Joint leftFoot = skeleton.Joints[JointType.FootLeft];
      Joint rightFoot = skeleton.Joints[JointType.FootRight];
      if ( (head.TrackingState != JointTrackingState.Tracked) || 
           (leftFoot.TrackingState != JointTrackingState.Tracked) ||
           (rightFoot.TrackingState != JointTrackingState.Tracked) ) {
             return;
      }

      // 距離カメラのピクセルごとのデータを取得する
      short[] depthPixel = new short[depthFrame.PixelDataLength];
      depthFrame.CopyPixelDataTo( depthPixel );

      // 距離カメラの座標に対応するRGBカメラの座標を取得する(座標合わせ)
      ColorImagePoint[] colorPoint = new ColorImagePoint[depthFrame.PixelDataLength];
      kinect.MapDepthFrameToColorFrame( depthStream.Format, depthPixel,
        colorStream.Format, colorPoint );

      // 頭のてっぺんを探す
      DepthImagePoint headDepth = depthFrame.MapFromSkeletonPoint( head.Position );
      int top = 0;
      for ( int i = 0; (headDepth.Y - i) > 0; i++ ) {
        // 一つ上のY座標を取得し、プレイヤーがいなければ、現在の座標が最上位
        int index = ((headDepth.Y - i) * depthFrame.Width) + headDepth.X;
        int player = depthPixel[index] & DepthImageFrame.PlayerIndexBitmask;
        if ( player == playerIndex ) {
          top = i;
        }
      }

      // 頭のてっぺんを3次元座標に戻し、足の座標(下にあるほう)を取得する
      // この差分で身長を測る
      head.Position = depthFrame.MapToSkeletonPoint( headDepth.X, headDepth.Y - top );
      Joint foot = (leftFoot.Position.Y < rightFoot.Position.Y) ? leftFoot : rightFoot;

      // 背丈を表示する
      DrawMeasure( kinect, colorStream, head, foot );
    }

    /// <summary>
    /// 背丈を表示する
    /// </summary>
    /// <param name="kinect"></param>
    /// <param name="colorStream"></param>
    /// <param name="head"></param>
    /// <param name="foot"></param>
    private void DrawMeasure( KinectSensor kinect, ColorImageStream colorStream, Joint head, Joint foot )
    {
      // 頭と足の座標の差分を身長とする(メートルからセンチメートルに変換する)
      int height = (int)(Math.Abs( head.Position.Y - foot.Position.Y ) * 100);

      // 頭と足のスケルトン座標を、RGBカメラの座標に変換する
      ColorImagePoint headColor = kinect.MapSkeletonPointToColor( head.Position, colorStream.Format );
      ColorImagePoint footColor = kinect.MapSkeletonPointToColor( foot.Position, colorStream.Format );

      // RGBカメラの座標を、表示する画面の座標に変換する
      Point headScalePoint = new Point(
        ScaleTo( headColor.X, colorStream.FrameWidth, canvasMeasure.Width ),
        ScaleTo( headColor.Y, colorStream.FrameHeight, canvasMeasure.Height )
        );

      Point footScalePoint = new Point(
        ScaleTo( footColor.X, colorStream.FrameWidth, canvasMeasure.Width ),
        ScaleTo( footColor.Y, colorStream.FrameHeight, canvasMeasure.Height )
        );

      const int lineLength = 50;
      const int thickness = 10;
      canvasMeasure.Children.Clear();
      // 頭の位置
      canvasMeasure.Children.Add( new Line()
      {
        Stroke = new SolidColorBrush( Colors.Red ),
        X1 = headScalePoint.X,
        Y1 = headScalePoint.Y,
        X2 = headScalePoint.X + lineLength,
        Y2 = headScalePoint.Y,
        StrokeThickness = thickness,
      } );

      // 足の位置
      canvasMeasure.Children.Add( new Line()
      {
        Stroke = new SolidColorBrush( Colors.Red ),
        X1 = footScalePoint.X,
        Y1 = footScalePoint.Y,
        X2 = headScalePoint.X + lineLength,
        Y2 = footScalePoint.Y,
        StrokeThickness = thickness,
      } );

      // 頭と足を結ぶ線
      canvasMeasure.Children.Add( new Line()
      {
        Stroke = new SolidColorBrush( Colors.Red ),
        X1 = headScalePoint.X + lineLength,
        Y1 = headScalePoint.Y,
        X2 = headScalePoint.X + lineLength,
        Y2 = footScalePoint.Y,
        StrokeThickness = thickness,
      } );

      // 身長の表示Y位置
      double Y = Math.Abs( headScalePoint.Y + footScalePoint.Y ) / 2;
      canvasMeasure.Children.Add( new TextBlock()
      {
        Margin = new Thickness( headScalePoint.X + lineLength, Y, 0, 0 ),
        Text = height.ToString(),
        Height = 36,
        Width = 60,
        FontSize = 24,
        FontWeight = FontWeights.Bold,
        Background = new SolidColorBrush( Colors.White ),
      } );
    }

    /// <summary>
    /// スケールを変換する
    /// </summary>
    /// <param name="value"></param>
    /// <param name="source"></param>
    /// <param name="dest"></param>
    /// <returns></returns>
    double ScaleTo( double value, double source, double dest )
    {
      return (value * dest) / source;
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