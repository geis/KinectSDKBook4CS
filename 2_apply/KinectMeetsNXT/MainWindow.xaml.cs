using System;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Kinect;
using NxtNet;

namespace KinectMeetsNXT
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window
  {
    Nxt nxt = new Nxt();
    bool isConneted = false;

    readonly int Bgr32BytesPerPixel = PixelFormats.Bgr32.BitsPerPixel / 8;

    public MainWindow()
    {
      try {
        InitializeComponent();

        // Kinectが接続されているかどうかを確認する
        if ( KinectSensor.KinectSensors.Count == 0 ) {
          throw new Exception( "Kinectを接続してください" );
        }

        // NXT関連の初期化を行う
        InitNxt();

        // Kinectの動作を開始する
        StartKinect( KinectSensor.KinectSensors[0] );
      }
      catch ( Exception ex ) {
        MessageBox.Show( ex.Message );
        Close();
      }
    }

    /// <summary>
    /// NXT関連の初期化を行う
    /// </summary>
    private void InitNxt()
    {
      // NXTはBluetoothをシリアルポートとして接続する
      if ( SerialPort.GetPortNames().Length == 0 ) {
        throw new Exception( "NXTの接続先が見つかりません" );
      }

      comboBoxPort.IsEnabled = true;
      buttonConnect.IsEnabled = true;
      buttonDisconnect.IsEnabled = false;

      // シリアルポートの一覧を表示する
      foreach ( var port in SerialPort.GetPortNames() ) {
        comboBoxPort.Items.Add( port );
      }

      comboBoxPort.SelectedIndex = 0;
    }

    /// <summary>
    /// 接続ボタン
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void buttonConnect_Click( object sender, RoutedEventArgs e )
    {
      Connect();
    }

    /// <summary>
    /// 切断ボタン
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void buttonDisconnect_Click( object sender, RoutedEventArgs e )
    {
      Disconnect();
    }

    /// <summary>
    /// Kinectの動作を開始する
    /// </summary>
    /// <param name="kinect"></param>
    private void StartKinect( KinectSensor kinect )
    {
      // RGBカメラとスケルトンを有効にする
      kinect.ColorStream.Enable();
      kinect.SkeletonStream.Enable();

      // RGBカメラとスケルトンのフレーム更新イベントを登録する
      kinect.ColorFrameReady +=
        new EventHandler<ColorImageFrameReadyEventArgs>( kinect_ColorFrameReady );
      kinect.SkeletonFrameReady +=
        new EventHandler<SkeletonFrameReadyEventArgs>( kinect_SkeletonFrameReady );

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
          kinect.ColorFrameReady -= kinect_ColorFrameReady;
          kinect.SkeletonFrameReady -= kinect_SkeletonFrameReady;

          // Kinectの停止と、ネイティブリソースを解放する
          kinect.Stop();
          kinect.Dispose();

          imageRgb.Source = null;
        }
      }
    }

    /// <summary>
    /// RGBカメラのフレーム更新
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
    /// スケルトンのフレーム更新
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
            Power( kinect, skeletonFrame );
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
    /// <param name="source"></param>
    /// <returns></returns>
    private void Power( KinectSensor kinect, SkeletonFrame skeletonFrame )
    {
      // スケルトンのデータを取得する
      Skeleton skeleton = skeletonFrame.GetFirstTrackedSkeleton();
      if ( skeleton == null ) {
        return;
      }

      // 操作に必要なジョイントを取得する
      Joint rightHand = skeleton.Joints[JointType.HandRight];
      Joint rightElbow = skeleton.Joints[JointType.ElbowRight];
      Joint leftHand = skeleton.Joints[JointType.HandLeft];
      Joint leftElbow = skeleton.Joints[JointType.ElbowLeft];

      // ジョイントの描画
      DrawSkeleton( kinect, new Joint[] { rightHand, rightElbow, leftHand, leftElbow });

      // ジョイントすべてがトラッキング状態のときのみ操作する
      if ( (rightHand.TrackingState != JointTrackingState.Tracked) ||
          (rightElbow.TrackingState != JointTrackingState.Tracked) ||
          (leftHand.TrackingState != JointTrackingState.Tracked) ||
          (leftElbow.TrackingState != JointTrackingState.Tracked) ) {
            return;
      }

      // 腕の角度を、モーターのパワーに変換して、NXTに送信する
      sbyte rightPower = 0, leftPower = 0;
      if ( rightHand.Position.Y > rightElbow.Position.Y ) {
        rightPower = GetPower( rightHand, rightElbow );
      }

      if ( leftHand.Position.Y > leftElbow.Position.Y ) {
        leftPower = GetPower( leftHand, leftElbow );
      }

      SetMotorPower( rightPower, leftPower );
    }

    /// <summary>
    /// トラッキングされているスケルトンのジョイントを描画する
    /// </summary>
    /// <param name="kinect"></param>
    /// <param name="joints"></param>
    private void DrawSkeleton( KinectSensor kinect, Joint[] joints )
    {
      // 描画する円の半径
      const int R = 5;

      // キャンバスをクリアする
      canvasSkeleton.Children.Clear();

      // ジョイントを描画する
      foreach ( Joint joint in joints ) {
        // ジョイントがトラッキングされていなければ次へ
        if ( joint.TrackingState != JointTrackingState.Tracked ) {
          continue;
        }

        // スケルトンの座標を、RGBカメラの座標に変換して円を書く
        ColorImagePoint point = kinect.MapSkeletonPointToColor( joint.Position,
          kinect.ColorStream.Format );

        canvasSkeleton.Children.Add( new Ellipse()
        {
          Fill = new SolidColorBrush( Colors.Red ),
          Margin = new Thickness( point.X - R, point.Y - R, 0, 0 ),
          Width = R * 2,
          Height = R * 2,
        } );
      }
    }

    /// <summary>
    /// 接続
    /// </summary>
    void Connect()
    {
      try {
        nxt.Connect( comboBoxPort.Items[comboBoxPort.SelectedIndex] as string );
        isConneted = true;

        comboBoxPort.IsEnabled = false;
        buttonConnect.IsEnabled = false;
        buttonDisconnect.IsEnabled = true;
      }
      catch ( Exception ex ) {
        MessageBox.Show( ex.Message );
      }
    }

    /// <summary>
    /// 切断
    /// </summary>
    void Disconnect()
    {
      try {
        if ( isConneted ) {
          nxt.Disconnect();
          isConneted = false;

          comboBoxPort.IsEnabled = true;
          buttonConnect.IsEnabled = true;
          buttonDisconnect.IsEnabled = false;
        }
      }
      catch ( Exception ex ) {
        MessageBox.Show( ex.Message );
      }
    }

    /// <summary>
    /// モーターのパワーを設定する
    /// </summary>
    /// <param name="right"></param>
    /// <param name="left"></param>
    void SetMotorPower( SByte right, SByte left )
    {
      if ( isConneted ) {
        nxt.SetOutputState( MotorPort.PortA, right, MotorModes.On,
          MotorRegulationMode.Idle, 100, MotorRunState.Running, 0 );
        nxt.SetOutputState( MotorPort.PortB, left, MotorModes.On,
          MotorRegulationMode.Idle, 100, MotorRunState.Running, 0 );
      }
    }

    /// <summary>
    /// モーターのパワーを取得する
    /// </summary>
    /// <param name="hand"></param>
    /// <param name="elbow"></param>
    /// <returns></returns>
    sbyte GetPower( Joint hand, Joint elbow )
    {
      // 手と肘の座標から、腕の角度を求める
      var a = Math.Abs( hand.Position.Y - elbow.Position.Y );
      var b = Math.Abs( hand.Position.Z - elbow.Position.Z );
      var c = Math.Sqrt( Math.Pow( a, 2 ) + Math.Pow( b, 2 ) );
      var theta = (sbyte)(Math.Acos( a / c ) * 180 / Math.PI);

      // 腕の角度をモーターの強さにする
      theta *= 2;
      if ( theta >= 100 ) {
        theta = 100;
      }

      // 手と肘の前後関係で、モーターの回転方向を変える
      return (hand.Position.Z > elbow.Position.Z) ? (sbyte)-theta : theta;
    }

    /// <summary>
    /// Windowが閉じられるときのイベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_Closing( object sender, System.ComponentModel.CancelEventArgs e )
    {
      Disconnect();
      StopKinect( KinectSensor.KinectSensors[0] );
    }
  }

  /// <summary>
  /// 初めにトラッキングしたスケルトンを取得する拡張メソッド
  /// </summary>
  public static class SkeletonExtensions
  {
    public static Skeleton GetFirstTrackedSkeleton( this SkeletonFrame skeletonFrame )
    {
      Skeleton[] skeleton = new Skeleton[skeletonFrame.SkeletonArrayLength];
      skeletonFrame.CopySkeletonDataTo( skeleton );
      return (from s in skeleton
              where s.TrackingState == SkeletonTrackingState.Tracked
              select s).FirstOrDefault();
    }
  }
}