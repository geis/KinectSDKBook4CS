using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using System.Windows.Shapes;

namespace SkeletonTracking
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
        Close();
      }
    }

    /// <summary>
    /// Kinectの動作を開始する
    /// </summary>
    /// <param name="kinect"></param>
    private void StartKinect( KinectSensor kinect )
    {
      // RGBカメラを有効にして、フレーム更新イベントを登録する
      kinect.ColorStream.Enable();
      kinect.ColorFrameReady +=
        new EventHandler<ColorImageFrameReadyEventArgs>( kinect_ColorFrameReady );

      // 距離カメラを有効にして、フレーム更新イベントを登録する
      kinect.DepthStream.Enable();
      kinect.DepthFrameReady +=
        new EventHandler<DepthImageFrameReadyEventArgs>( kinect_DepthFrameReady );

      // スケルトンを有効にして、フレーム更新イベントを登録する
      kinect.SkeletonFrameReady +=
        new EventHandler<SkeletonFrameReadyEventArgs>( kinect_SkeletonFrameReady );
      kinect.SkeletonStream.Enable();

      // Kinectの動作を開始する
      kinect.Start();

      // defaultモードとnearモードの切り替え
      comboBoxRange.Items.Clear();
      foreach ( var range in Enum.GetValues( typeof( DepthRange ) ) ) {
        comboBoxRange.Items.Add( range.ToString() );
      }

      comboBoxRange.SelectedIndex = 0;
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
          kinect.ColorFrameReady -= kinect_ColorFrameReady;
          kinect.DepthFrameReady -= kinect_DepthFrameReady;
          kinect.SkeletonFrameReady -= kinect_SkeletonFrameReady;

          // Kinectの停止と、ネイティブリソースを解放する
          kinect.Stop();
          kinect.Dispose();

          imageRgb.Source = null;
          imageDepth.Source = null;
        }
      }
    }

    /// <summary>
    /// RGBカメラのフレーム更新イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void kinect_ColorFrameReady( object sender, ColorImageFrameReadyEventArgs e )
    {
      try {
        // RGBカメラのフレームデータを取得する
        using ( ColorImageFrame colorFrame = e.OpenColorImageFrame() ) {
          if ( colorFrame != null ) {
            // RGBカメラのピクセルデータを取得する
            byte[] colorPixel = new byte[colorFrame.PixelDataLength];
            colorFrame.CopyPixelDataTo( colorPixel );

            // ピクセルデータをビットマップに変換する
            imageRgb.Source = BitmapSource.Create( colorFrame.Width, colorFrame.Height, 96, 96,
                PixelFormats.Bgr32, null, colorPixel, colorFrame.Width * colorFrame.BytesPerPixel );
          }
        }
      }
      catch ( Exception ex ) {
        MessageBox.Show( ex.Message );
      }
    }

    /// <summary>
    /// 距離カメラのフレーム更新イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void kinect_DepthFrameReady( object sender, DepthImageFrameReadyEventArgs e )
    {
      try {
        // センサーのインスタンスを取得する
        KinectSensor kinect = sender as KinectSensor;
        if ( kinect == null ) {
          return;
        }

        // 距離カメラのフレームデータを取得する
        using ( DepthImageFrame depthFrame = e.OpenDepthImageFrame() ) {
          if ( depthFrame != null ) {
            // 距離データを画像化して表示
            imageDepth.Source = BitmapSource.Create( depthFrame.Width, depthFrame.Height, 96, 96,
                PixelFormats.Bgr32, null, ConvertDepthColor( kinect, depthFrame ),
                depthFrame.Width * Bgr32BytesPerPixel );
          }
        }
      }
      catch ( Exception ex ) {
        MessageBox.Show( ex.Message );
      }
    }

    /// <summary>
    /// スケルトンのフレーム更新イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void kinect_SkeletonFrameReady( object sender, SkeletonFrameReadyEventArgs e )
    {
      try {
        // Kinectのインスタンスを取得する
        KinectSensor kinect = sender as KinectSensor;
        if ( kinect == null ) {
          return;
        }

        // スケルトンのフレームを取得する
        using ( SkeletonFrame skeletonFrame = e.OpenSkeletonFrame() ) {
          if ( skeletonFrame != null ) {
            DrawSkeleton( kinect, skeletonFrame );
          }
        }
      }
      catch ( Exception ex ) {
        MessageBox.Show( ex.Message );
      }
    }

    /// <summary>
    /// スケルトンを描画する
    /// </summary>
    /// <param name="kinect"></param>
    /// <param name="skeletonFrame"></param>
    private void DrawSkeleton( KinectSensor kinect, SkeletonFrame skeletonFrame )
    {
      // スケルトンのデータを取得する
      Skeleton[] skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
      skeletonFrame.CopySkeletonDataTo( skeletons );

      canvasSkeleton.Children.Clear();

      // トラッキングされているスケルトンのジョイントを描画する
      foreach ( Skeleton skeleton in skeletons ) {
        // スケルトンがトラッキング状態(デフォルトモード)の場合は、ジョイントを描画する
        if ( skeleton.TrackingState == SkeletonTrackingState.Tracked ) {
          // ジョイントを描画する
          foreach ( Joint joint in skeleton.Joints ) {
            // ジョイントがトラッキングされていなければ次へ
            if ( joint.TrackingState == JointTrackingState.NotTracked ) {
              continue;
            }

            // ジョイントの座標を描く
            DrawEllipse( kinect, joint.Position );
          }
        }
        // スケルトンが位置追跡(ニアモードの)の場合は、スケルトン位置(Center hip)を描画する
        else if ( skeleton.TrackingState == SkeletonTrackingState.PositionOnly ) {
          // スケルトンの座標を描く
          DrawEllipse( kinect, skeleton.Position );
        }
      }
    }

    /// <summary>
    /// ジョイントの円を描く
    /// </summary>
    /// <param name="kinect"></param>
    /// <param name="position"></param>
    private void DrawEllipse( KinectSensor kinect, SkeletonPoint position )
    {
      const int R = 5;

      // スケルトンの座標を、RGBカメラの座標に変換する
      ColorImagePoint point = kinect.MapSkeletonPointToColor( position, kinect.ColorStream.Format );

      // 座標を画面のサイズに変換する
      point.X = (int)ScaleTo( point.X, kinect.ColorStream.FrameWidth, canvasSkeleton.Width );
      point.Y = (int)ScaleTo( point.Y, kinect.ColorStream.FrameHeight, canvasSkeleton.Height );

      // 円を描く
      canvasSkeleton.Children.Add( new Ellipse()
      {
        Fill = new SolidColorBrush( Colors.Red ),
        Margin = new Thickness( point.X - R, point.Y - R, 0, 0 ),
        Width = R * 2,
        Height = R * 2,
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
    /// 距離データをカラー画像に変換する
    /// </summary>
    /// <param name="kinect"></param>
    /// <param name="depthFrame"></param>
    /// <returns></returns>
    private byte[] ConvertDepthColor( KinectSensor kinect, DepthImageFrame depthFrame )
    {
      ColorImageStream colorStream = kinect.ColorStream;
      DepthImageStream depthStream = kinect.DepthStream;

      // 距離カメラのピクセルごとのデータを取得する
      short[] depthPixel = new short[depthFrame.PixelDataLength];
      depthFrame.CopyPixelDataTo( depthPixel );

      // 距離カメラの座標に対応するRGBカメラの座標を取得する(座標合わせ)
      ColorImagePoint[] colorPoint = new ColorImagePoint[depthFrame.PixelDataLength];
      kinect.MapDepthFrameToColorFrame( depthStream.Format, depthPixel,
        colorStream.Format, colorPoint );

      byte[] depthColor = new byte[depthFrame.PixelDataLength * Bgr32BytesPerPixel];
      for ( int index = 0; index < depthPixel.Length; index++ ) {
        // 距離カメラのデータから、プレイヤーIDと距離を取得する
        int player = depthPixel[index] & DepthImageFrame.PlayerIndexBitmask;
        int distance = depthPixel[index] >> DepthImageFrame.PlayerIndexBitmaskWidth;

        // 変換した結果が、フレームサイズを超えることがあるため、小さいほうを使う
        int x = Math.Min( colorPoint[index].X, colorStream.FrameWidth - 1 );
        int y = Math.Min( colorPoint[index].Y, colorStream.FrameHeight - 1 );
        int colorIndex = ((y * depthFrame.Width) + x) * Bgr32BytesPerPixel;

        // プレイヤーがいるピクセルの場合
        if ( player != 0 ) {
          depthColor[colorIndex] = 255;
          depthColor[colorIndex + 1] = 255;
          depthColor[colorIndex + 2] = 255;
        }
        // プレイヤーではないピクセルの場合
        else {
          // サポート外 0-40cm
          if ( distance == depthStream.UnknownDepth ) {
            depthColor[colorIndex] = 0;
            depthColor[colorIndex + 1] = 0;
            depthColor[colorIndex + 2] = 255;
          }
          // 近すぎ 40cm-80cm(default mode)
          else if ( distance == depthStream.TooNearDepth ) {
            depthColor[colorIndex] = 0;
            depthColor[colorIndex + 1] = 255;
            depthColor[colorIndex + 2] = 0;
          }
          // 遠すぎ 3m(Near),4m(Default)-8m
          else if ( distance == depthStream.TooFarDepth ) {
            depthColor[colorIndex] = 255;
            depthColor[colorIndex + 1] = 0;
            depthColor[colorIndex + 2] = 0;
          }
          // 有効な距離データ
          else {
            depthColor[colorIndex] = 0;
            depthColor[colorIndex + 1] = 255;
            depthColor[colorIndex + 2] = 255;
          }
        }
      }

      return depthColor;
    }

    /// <summary>
    /// 距離カメラの通常/近接モード変更イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void comboBoxRange_SelectionChanged( object sender,
      System.Windows.Controls.SelectionChangedEventArgs e )
    {
      try {
        KinectSensor.KinectSensors[0].DepthStream.Range = (DepthRange)comboBoxRange.SelectedIndex;
      }
      catch ( Exception ) {
        comboBoxRange.SelectedIndex = 0;
      }
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