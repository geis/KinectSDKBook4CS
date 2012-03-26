using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;


namespace MultiKinect
{
  /// <summary>
  /// KinectWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class KinectWindow : UserControl
  {
    readonly int Bgr32BytesPerPixel = PixelFormats.Bgr32.BitsPerPixel / 8;

    //bool isContinue = true;

    public KinectSensor Kinect
    {
      get;
      private set;
    }

    public KinectWindow()
    {
      try {
        InitializeComponent();
      }
      catch ( Exception ex ) {
        MessageBox.Show( ex.Message );
      }
    }

    /// <summary>
    /// Kinectの動作を開始する
    /// </summary>
    /// <param name="kinect"></param>
    public void StartKinect( KinectSensor kinect )
    {
      Kinect = kinect;

      // RGBカメラ、距離カメラ、スケルトン・トラッキング(プレイヤーの認識)を有効にする
      Kinect.ColorStream.Enable();
      //Kinect.DepthStream.Enable();
      Kinect.SkeletonStream.Enable();

      // RGBカメラ、距離カメラ、スケルトンのフレーム更新イベントを登録する
      Kinect.AllFramesReady +=
        new EventHandler<AllFramesReadyEventArgs>( kinect_AllFramesReady );

      // Kinectの動作を開始する
      Kinect.Start();

      // 音源推定のビームと、音源方向が変更した際に通知されるイベントを登録する
      Kinect.AudioSource.BeamAngleChanged +=
        new EventHandler<BeamAngleChangedEventArgs>( AudioSource_BeamAngleChanged );
      Kinect.AudioSource.SoundSourceAngleChanged +=
        new EventHandler<SoundSourceAngleChangedEventArgs>( AudioSource_SoundSourceAngleChanged );

      //// 音声入出力スレッド
      //Thread thread = new Thread( new ThreadStart( () =>
      //{
      //  // ストリーミングプレイヤー
      //  StreamingWavePlayer player = new StreamingWavePlayer( 16000, 16, 1, 100 );
      //  // 音声入力用のバッファ
      //  byte[] buffer = new byte[1024];

      //  // エコーのキャンセルと抑制を有効にする
      //  kinect.AudioSource.EchoCancellationMode = EchoCancellationMode.CancellationAndSuppression;

      //  // 音声の入力を開始する
      //  using ( Stream stream = kinect.AudioSource.Start() ) {
      //    while ( isContinue ) {
      //      // 音声を入力し、スピーカーに出力する
      //      stream.Read( buffer, 0, buffer.Length );
      //      player.Output( buffer );
      //    }
      //  }
      //} ) );

      //// スレッドの動作を開始する
      //thread.Start();

      // defaultモードとnearモードの切り替え
      comboBoxRange.Items.Clear();
      foreach ( var range in Enum.GetValues( typeof( DepthRange ) ) ) {
        comboBoxRange.Items.Add( range.ToString() );
      }

      comboBoxRange.SelectedIndex = 0;

      // チルトモーターを動作させるスライダーを設定する
      sliderTiltAngle.Maximum = Kinect.MaxElevationAngle;
      sliderTiltAngle.Minimum = Kinect.MinElevationAngle;
      sliderTiltAngle.Value = Kinect.ElevationAngle;
    }

    /// <summary>
    /// Kinectの動作を停止する
    /// </summary>
    /// <param name="kinect"></param>
    public void StopKinect()
    {
      if ( Kinect != null ) {
        if ( Kinect.IsRunning ) {
          // 音声のスレッドを停止する
          //isContinue = false;
          //Kinect.AudioSource.Stop();

          // フレーム更新イベントを削除する
          Kinect.AllFramesReady -= kinect_AllFramesReady;

          // Kinectの停止と、ネイティブリソースを解放する
          Kinect.Stop();
          Kinect.Dispose();
          Kinect = null;

          imageRgb.Source = null;
          imageDepth.Source = null;
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
            // RGBカメラのピクセルデータを取得する
            byte[] colorPixel = new byte[colorFrame.PixelDataLength];
            colorFrame.CopyPixelDataTo( colorPixel );

            // ピクセルデータをビットマップに変換する
            imageRgb.Source = BitmapSource.Create( colorFrame.Width, colorFrame.Height, 96, 96,
                PixelFormats.Bgr32, null, colorPixel, colorFrame.Width * colorFrame.BytesPerPixel );
          }
        }

        //// 距離カメラのフレームデータを取得する
        //using ( DepthImageFrame depthFrame = e.OpenDepthImageFrame() ) {
        //  if ( depthFrame != null ) {
        //    // 距離データを画像化して表示
        //    imageDepth.Source = BitmapSource.Create( depthFrame.Width, depthFrame.Height, 96, 96,
        //        PixelFormats.Bgr32, null, ConvertDepthColor( kinect, depthFrame ),
        //        depthFrame.Width * Bgr32BytesPerPixel );
        //  }
        //}

        // スケルトンのフレームを取得する
        using ( SkeletonFrame skeletonFrame = e.OpenSkeletonFrame() ) {
          if ( skeletonFrame != null ) {
            imageRgb.Source = DrawSkeleton( kinect, skeletonFrame, imageRgb.Source );
          }
        }
      }
      catch ( Exception ex ) {
        MessageBox.Show( ex.Message );
      }
    }

    /// <summary>
    /// 音源方向が変化した
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void AudioSource_SoundSourceAngleChanged( object sender, SoundSourceAngleChangedEventArgs e )
    {
      soundSource.Angle = -e.Angle;
    }

    /// <summary>
    /// ビーム方向が変化した
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void AudioSource_BeamAngleChanged( object sender, BeamAngleChangedEventArgs e )
    {
      beam.Angle = -e.Angle;
    }

    /// <summary>
    /// スケルトンを描画する
    /// </summary>
    /// <param name="kinect"></param>
    /// <param name="skeletonFrame"></param>
    /// <param name="source"></param>
    /// <returns></returns>
    private RenderTargetBitmap DrawSkeleton( KinectSensor kinect,
      SkeletonFrame skeletonFrame, ImageSource source )
    {
      // スケルトンのデータを取得する
      Skeleton[] skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
      skeletonFrame.CopySkeletonDataTo( skeletons );

      DrawingVisual drawingVisual = new DrawingVisual();
      using ( DrawingContext drawingContext = drawingVisual.RenderOpen() ) {
        // ImageSourceを描画する
        drawingContext.DrawImage( source, new Rect( 0, 0, source.Width, source.Height ) );

        // トラッキングされているスケルトンのジョイントを描画する
        const int R = 5;
        foreach ( var skeleton in skeletons ) {
          // スケルトンがトラッキングされていなければ次へ
          if ( skeleton.TrackingState != SkeletonTrackingState.Tracked ) {
            continue;
          }

          // ジョイントを描画する
          foreach ( Joint joint in skeleton.Joints ) {
            // ジョイントがトラッキングされていなければ次へ
            if ( joint.TrackingState != JointTrackingState.Tracked ) {
              continue;
            }

            // スケルトンの座標を、RGBカメラの座標に変換して円を書く
            ColorImagePoint point = kinect.MapSkeletonPointToColor( joint.Position,
              kinect.ColorStream.Format );
            drawingContext.DrawEllipse( new SolidColorBrush( Colors.Red ),
                new Pen( Brushes.Red, 1 ), new Point( point.X, point.Y ), R, R );
          }
        }
      }

      // 描画可能なビットマップを作る
      RenderTargetBitmap bitmap = new RenderTargetBitmap( (int)source.Width, (int)source.Height,
        96, 96, PixelFormats.Default );
      bitmap.Render( drawingVisual );
      return bitmap;
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
        Kinect.DepthStream.Range = (DepthRange)comboBoxRange.SelectedIndex;
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
      StopKinect();
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
        Kinect.ElevationAngle = (int)e.NewValue;
      }
      catch ( Exception ex ) {
        Trace.WriteLine( ex.Message );
      }
    }
  }
}
