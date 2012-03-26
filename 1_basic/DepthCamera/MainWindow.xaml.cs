using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;

namespace DepthCamera
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
      // RGBカメラを有効にして、フレーム更新イベントを登録する
      kinect.ColorStream.Enable();
      kinect.ColorFrameReady +=
        new EventHandler<ColorImageFrameReadyEventArgs>( kinect_ColorFrameReady );

      // 距離カメラを有効にして、フレーム更新イベントを登録する
      kinect.DepthStream.Enable();
      kinect.DepthFrameReady +=
        new EventHandler<DepthImageFrameReadyEventArgs>( kinect_DepthFrameReady );

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
            imageRgb.Source = BitmapSource.Create( colorFrame.Width,
              colorFrame.Height, 96, 96, PixelFormats.Bgr32, null, colorPixel,
              colorFrame.Width * colorFrame.BytesPerPixel );
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
        // Kinectのインスタンスを取得する
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

      byte[] depthColor = new byte[depthFrame.PixelDataLength * Bgr32BytesPerPixel];
      for ( int index = 0; index < depthPixel.Length; index++ ) {
        // 距離カメラのデータから、距離を取得する
        int distance = depthPixel[index] >> DepthImageFrame.PlayerIndexBitmaskWidth;

        // 変換した結果が、フレームサイズを超えることがあるため、小さいほうを使う
        int x = Math.Min( colorPoint[index].X, colorStream.FrameWidth - 1 );
        int y = Math.Min( colorPoint[index].Y, colorStream.FrameHeight - 1 );

        // 動作が遅くなる場合、MapDepthFrameToColorFrame を外すと速くなる場合が
        // あります。外す場合のx,yはこちらを使用してください。
        //int x = index % depthFrame.Width;
        //int y = index / depthFrame.Width;

        int colorIndex = ((y * depthFrame.Width) + x) * Bgr32BytesPerPixel;

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
    /// Windowsが閉じられるときのイベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_Closing( object sender,
      System.ComponentModel.CancelEventArgs e )
    {
      StopKinect( KinectSensor.KinectSensors[0] );
    }
  }
}