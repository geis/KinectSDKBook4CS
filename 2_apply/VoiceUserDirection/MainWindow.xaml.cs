using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using System.Windows.Shapes;

namespace VoiceUserDirection
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window
  {
    // 音源方向
    int soundSourceAngle = 0;

    const int PlayerCount = 7;
    bool[] enablePlayer = new bool[PlayerCount];
    int[] playerAngles = new int[PlayerCount];
    Color[] playerColor = new Color[PlayerCount] {
      Colors.White, Colors.Red, Colors.Blue, Colors.Green,
      Colors.Yellow, Colors.Pink, Colors.Black
    };

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

      // 音源方向通知を設定して、音声処理を開始する
      kinect.AudioSource.SoundSourceAngleChanged +=
          new EventHandler<SoundSourceAngleChangedEventArgs>( AudioSource_SoundSourceAngleChanged );

      kinect.AudioSource.Start();
    }

    /// <summary>
    /// 音源方向が通知される
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void AudioSource_SoundSourceAngleChanged( object sender, SoundSourceAngleChangedEventArgs e )
    {
      if ( e.ConfidenceLevel > 0.5 ) {
        const int Range = 5;  // 誤差範囲
        soundSourceAngle = (int)e.Angle;

        for ( int i = 1; i < playerAngles.Length; i++ ) {
          // 無効なプレイヤー
          if ( playerAngles[i] == -1 ) {
            continue;
          }

          // 音源と頭の角度が一定範囲内にあれば、その人の音とみなす
          if ( ((soundSourceAngle - Range) <= playerAngles[i]) &&
                (playerAngles[i] <= (soundSourceAngle + Range)) ) {
            enablePlayer[i] = true;
          }

          // ユーザーの位置を表示する
          labelSoundSource.Content = string.Format( "音源方向:{0}, プレイヤー方向:{1}",
            soundSourceAngle.ToString(), playerAngles[i] );
        }
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
      for ( int i = 0; i < skeletons.Length; i++ ) {
        // プレーヤーインデックスは、skeleton配列のインデックス + 1
        int playerIndex = i + 1;

        // プレイヤー位置を初期化する
        playerAngles[playerIndex] = -1;

        // スケルトンがトラッキング状態でなければ次へ
        if ( skeletons[i].TrackingState != SkeletonTrackingState.Tracked ) {
          continue;
        }

        Skeleton skeleton = skeletons[i];

        // 頭の位置を取得する
        Joint joint = skeleton.Joints[JointType.Head];
        if ( joint.TrackingState == JointTrackingState.NotTracked ) {
          continue;
        }

        // ジョイントの座標を描く
        DrawEllipse( kinect, joint.Position );

        // プレイヤーの角度を取得する
        playerAngles[playerIndex] = GetPlayerAngle( joint );

      }
    }

    /// <summary>
    /// プレイヤーの角度を取得する
    /// </summary>
    /// <param name="joint"></param>
    /// <returns></returns>
    private int GetPlayerAngle( Joint joint )
    {
      // 頭の角度(Kinect 中心)
      var a = Math.Abs( joint.Position.X * 100 );
      var b = Math.Abs( joint.Position.Z * 100 );
      var c = Math.Sqrt( Math.Pow( a, 2 ) + Math.Pow( b, 2 ) );
      int theta = (int)(Math.Acos( a / c ) * 180 / Math.PI);

      theta = (int)Math.Abs( theta - 90 );
      if ( joint.Position.X < 0 ) {
        theta = -theta;
      }

      return theta;
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
          // 有効なプレーヤーに色付けする
          if ( enablePlayer[player] ) {
            depthColor[colorIndex] = playerColor[player].B;
            depthColor[colorIndex + 1] = playerColor[player].G;
            depthColor[colorIndex + 2] = playerColor[player].R;
          }
        }
      }

      return depthColor;
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
