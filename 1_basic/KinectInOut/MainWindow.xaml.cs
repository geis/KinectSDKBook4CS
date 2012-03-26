using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Geis.Win32;
using Microsoft.Kinect;

namespace KinectInOut
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window
  {
    readonly int Bgr32BytesPerPixel = PixelFormats.Bgr32.BitsPerPixel / 8;

    bool isContinue = true;

    public MainWindow()
    {
      try {
        InitializeComponent();

        // Kinectの状態変更通知を受け取る
        KinectSensor.KinectSensors.StatusChanged +=
          new EventHandler<StatusChangedEventArgs>
            ( KinectSensors_StatusChanged );

        // Kinectが1台以上接続されていれば、最初から開始する
        if ( (KinectSensor.KinectSensors.Count >= 1) &&
             (KinectSensor.KinectSensors[0].Status == KinectStatus.Connected) ) {
          StartKinect( KinectSensor.KinectSensors[0] );
        }
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
        new EventHandler<SkeletonFrameReadyEventArgs>
          ( kinect_SkeletonFrameReady );
      kinect.SkeletonStream.Enable();

      // Kinectの動作を開始する
      kinect.Start();

      // 音源推定のビームと、音源方向が変更した際に通知されるイベントを登録する
      kinect.AudioSource.BeamAngleChanged +=
        new EventHandler<BeamAngleChangedEventArgs>
          ( AudioSource_BeamAngleChanged );

      kinect.AudioSource.SoundSourceAngleChanged +=
        new EventHandler<SoundSourceAngleChangedEventArgs>
          ( AudioSource_SoundSourceAngleChanged );

      // 音声入出力スレッド
      Thread thread = new Thread( new ThreadStart( () =>
      {
        // ストリーミングプレイヤー
        StreamingWavePlayer player = new StreamingWavePlayer( 16000, 16, 1, 100 );
        // 音声入力用のバッファ
        byte[] buffer = new byte[1024];

        // エコーのキャンセルと抑制を有効にする
        kinect.AudioSource.EchoCancellationMode =
          EchoCancellationMode.CancellationAndSuppression;

        // 音声の入力を開始する
        using ( Stream stream = kinect.AudioSource.Start() ) {
          while ( isContinue ) {
            // 音声を入力し、スピーカーに出力する
            stream.Read( buffer, 0, buffer.Length );
            player.Output( buffer );
          }
        }
      } ) );

      // スレッドの動作を開始する
      thread.Start();

      // defaultモードとnearモードの切り替え
      comboBoxRange.Items.Clear();
      foreach ( var range in Enum.GetValues( typeof( DepthRange ) ) ) {
        comboBoxRange.Items.Add( range.ToString() );
      }

      comboBoxRange.SelectedIndex = 0;

      // チルトモーターを動作させるスライダーを設定する
      sliderTiltAngle.Maximum = kinect.MaxElevationAngle;
      sliderTiltAngle.Minimum = kinect.MinElevationAngle;
      sliderTiltAngle.Value = kinect.ElevationAngle;
    }

    /// <summary>
    /// Kinectの動作を停止する
    /// </summary>
    /// <param name="kinect"></param>
    private void StopKinect( KinectSensor kinect )
    {
      if ( kinect != null ) {
        if ( kinect.IsRunning ) {
          // 音声のスレッドを停止する
          isContinue = false;
          kinect.AudioSource.Stop();

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
    /// Kinectの接続状態が変わった時に呼び出される
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void KinectSensors_StatusChanged( object sender, StatusChangedEventArgs e )
    {
      // デバイスが接続された
      if ( e.Status == KinectStatus.Connected ) {
        StartKinect( e.Sensor );
      }
      // デバイスが切断された
      else if ( e.Status == KinectStatus.Disconnected ) {
        StopKinect( e.Sensor );
      }
      // ACが抜けてる
      else if ( e.Status == KinectStatus.NotPowered ) {
        StopKinect( e.Sensor );
        MessageBox.Show( "電源ケーブルを接続してください" );
      }
      // Kinect for Xbox 360
      else if ( e.Status == KinectStatus.DeviceNotSupported ) {
        MessageBox.Show( "Kinect for Xbox 360 はサポートされません" );
      }
      // USBの帯域が足りない
      else if ( e.Status == KinectStatus.InsufficientBandwidth ) {
        MessageBox.Show( "USBの帯域が足りません" );
      }
    }

    /// <summary>
    /// 音源方向が変化した
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void AudioSource_SoundSourceAngleChanged( object sender,
      SoundSourceAngleChangedEventArgs e )
    {
      soundSource.Angle = -e.Angle;
    }

    /// <summary>
    /// ビーム方向が変化した
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void AudioSource_BeamAngleChanged( object sender,
      BeamAngleChangedEventArgs e )
    {
      beam.Angle = -e.Angle;
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
            imageRgb.Source = BitmapSource.Create( colorFrame.Width,
              colorFrame.Height, 96, 96, PixelFormats.Bgr32, null,
              colorPixel, colorFrame.Width * colorFrame.BytesPerPixel );
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
            imageDepth.Source = BitmapSource.Create( depthFrame.Width,
              depthFrame.Height, 96, 96, PixelFormats.Bgr32, null,
              ConvertDepthColor( kinect, depthFrame ),
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
        // スケルトンがトラッキング状態(デフォルトモード)の場合は、
        // ジョイントを描画する
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
        // スケルトンが位置追跡(ニアモードの)の場合は、スケルトン位置(Center hip)
        // を描画する
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
      ColorImagePoint point = kinect.MapSkeletonPointToColor( position,
        kinect.ColorStream.Format );

      // 座標を画面のサイズに変換する
      point.X = (int)ScaleTo( point.X, kinect.ColorStream.FrameWidth,
        canvasSkeleton.Width );
      point.Y = (int)ScaleTo( point.Y, kinect.ColorStream.FrameHeight,
        canvasSkeleton.Height );

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
    private byte[] ConvertDepthColor( KinectSensor kinect,
      DepthImageFrame depthFrame )
    {
      ColorImageStream colorStream = kinect.ColorStream;
      DepthImageStream depthStream = kinect.DepthStream;

      // 距離カメラのピクセルごとのデータを取得する
      short[] depthPixel = new short[depthFrame.PixelDataLength];
      depthFrame.CopyPixelDataTo( depthPixel );

      // 距離カメラの座標に対応するRGBカメラの座標を取得する(座標合わせ)
      ColorImagePoint[] colorPoint =
        new ColorImagePoint[depthFrame.PixelDataLength];
      kinect.MapDepthFrameToColorFrame( depthStream.Format, depthPixel,
        colorStream.Format, colorPoint );

      byte[] depthColor =new byte[depthFrame.PixelDataLength * Bgr32BytesPerPixel];
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
        KinectSensor.KinectSensors[0].DepthStream.Range =
          (DepthRange)comboBoxRange.SelectedIndex;
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
    private void Window_Closing( object sender,
      System.ComponentModel.CancelEventArgs e )
    {
      StopKinect( KinectSensor.KinectSensors[0] );
    }

    /// <summary>
    /// スライダーの位置が変更された
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void sliderTiltAngle_ValueChanged( object sender,
      RoutedPropertyChangedEventArgs<double> e )
    {
      try {
        KinectSensor.KinectSensors[0].ElevationAngle = (int)e.NewValue;
      }
      catch ( Exception ex ) {
        Trace.WriteLine( ex.Message );
      }
    }
  }
}
