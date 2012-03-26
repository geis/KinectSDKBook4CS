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
using Kinect.Toolbox;

namespace KinectToolboxSandbox
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window
  {
    readonly int Bgr32BytesPerPixel = PixelFormats.Bgr32.BitsPerPixel / 8;

    ColorStreamManager colorManager = new ColorStreamManager();
    SwipeGestureDetector swipeDetector = new SwipeGestureDetector();
    AlgorithmicPostureDetector postureDetector = new AlgorithmicPostureDetector();

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
      // RGBカメラを有効にする
      kinect.ColorStream.Enable();
      kinect.ColorFrameReady +=
        new EventHandler<ColorImageFrameReadyEventArgs>( kinect_ColorFrameReady );

      // スケルトン・トラッキングを有効にする
      kinect.SkeletonStream.Enable();
      kinect.SkeletonFrameReady +=
        new EventHandler<SkeletonFrameReadyEventArgs>( kinect_SkeletonFrameReady );

      // ジェスチャーの検出を行うコールバックを登録する
      swipeDetector.OnGestureDetected +=
        new Action<string>( swipeDetector_OnGestureDetected );
      // ジェスチャーの動作の軌跡を表示する
      swipeDetector.TraceTo( canvasTrack, Colors.Red );

      // ポーズの検出を通知するコールバックを登録する
      postureDetector.PostureDetected +=
        new Action<string>( postureDetector_PostureDetected );

      // Kinectの動作を開始する
      kinect.Start();
    }

    /// <summary>
    /// RGBカメラの更新通知
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void kinect_ColorFrameReady( object sender, ColorImageFrameReadyEventArgs e )
    {
      try {
        // RGBカメラのフレームデータを取得する
        using ( ColorImageFrame colorFrame = e.OpenColorImageFrame() ) {
          if ( colorFrame != null ) {
            colorManager.Update( colorFrame );
            imageRgb.Source = colorManager.Bitmap;
          }
        }
      }
      catch ( Exception ex ) {
        MessageBox.Show( ex.Message );
      }
    }

    /// <summary>
    /// スケルトンの更新通知
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
            Skeleton skeleton = skeletonFrame.GetFirstTrackedSkeleton();
            if ( skeleton != null ) {
              // ポーズの検出用に、スケルトンデータを追加する
              postureDetector.TrackPostures( skeleton );

              // 右手がトラッキングされていた場合、ジェスチャーの検出用にジョイントを追加する
              Joint hand = skeleton.Joints[JointType.HandRight];
              if ( hand.TrackingState == JointTrackingState.Tracked ) {
                swipeDetector.Add( hand.Position, kinect );

              }
            }
          }
        }
      }
      catch ( Exception ex ) {
        MessageBox.Show( ex.Message );
      }
    }

    /// <summary>
    /// ポーズの検出を通知する
    /// </summary>
    /// <param name="obj"></param>
    void postureDetector_PostureDetected( string obj )
    {
      textBlockPose.Text = obj;
    }

    /// <summary>
    /// ジェスチャーの検出を通知する
    /// </summary>
    /// <param name="obj"></param>
    void swipeDetector_OnGestureDetected( string obj )
    {
      textBlockGesture.Text = obj;
    }

    /// <summary>
    /// Kinectの動作を停止する
    /// </summary>
    /// <param name="kinect"></param>
    private void StopKinect( KinectSensor kinect )
    {
      if ( kinect != null ) {
        if ( kinect.IsRunning ) {
          // Kinectの停止と、ネイティブリソースを解放する
          kinect.Stop();
          kinect.Dispose();

          imageRgb.Source = null;
        }
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