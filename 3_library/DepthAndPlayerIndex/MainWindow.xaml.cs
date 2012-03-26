using System;
using System.Windows;
using Coding4Fun.Kinect.Wpf;
using Microsoft.Kinect;

namespace DepthAndPlayerIndex
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            try {
                if ( KinectSensor.KinectSensors.Count == 0 ) {
                    throw new Exception( "Kinectが接続されていません" );
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
            // Colorを有効にする
            kinect.ColorFrameReady +=
                    new EventHandler<ColorImageFrameReadyEventArgs>( kinect_ColorFrameReady );
            kinect.ColorStream.Enable();

            // Depthを有効にする
            kinect.DepthFrameReady +=
                    new EventHandler<DepthImageFrameReadyEventArgs>( kinect_DepthFrameReady );
            kinect.DepthStream.Enable();

            // Skeletonを有効にすることで、プレイヤーが取得できる
            kinect.SkeletonStream.Enable();

            // Kinectの動作を開始する
            kinect.Start();
        }

        /// <summary>
        /// Kinectの動作を停止する
        /// </summary>
        /// <param name="kinect"></param>
        private static void StopKinect( KinectSensor kinect )
        {
            if ( kinect.IsRunning ) {
                kinect.Stop();
                kinect.Dispose();
            }
        }

        /// <summary>
        /// RGBカメラのフレーム更新イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void kinect_ColorFrameReady( object sender, ColorImageFrameReadyEventArgs e )
        {
            using ( var colorFrame = e.OpenColorImageFrame() ) {
                if ( colorFrame != null ) {
                    imageRgbCamera.Source = colorFrame.ToBitmapSource();
                }
            }
        }

        /// <summary>
        /// 距離カメラのフレーム更新イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void kinect_DepthFrameReady( object sender, DepthImageFrameReadyEventArgs e )
        {
            using ( var depthFrame = e.OpenDepthImageFrame() ) {
                if ( depthFrame != null ) {
                    imageDepthCamera.Source = depthFrame.ToBitmapSource();
                }
            }
        }

        /// <summary>
        /// Windowsが閉じられるイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing( object sender, System.ComponentModel.CancelEventArgs e )
        {
            StopKinect( KinectSensor.KinectSensors[0] );
        }
    }
}
