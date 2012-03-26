using System;
using System.Windows;
using Microsoft.Kinect;

namespace MultiKinect
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window
  {
    KinectWindow[] kinectWindows;

    public MainWindow()
    {
      try {
        InitializeComponent();

        kinectWindows =  new KinectWindow[] {
          kinectWindow1,
          kinectWindow2,
          kinectWindow3,
          kinectWindow4,
        };

        // Kinectの状態変更通知を受け取る
        KinectSensor.KinectSensors.StatusChanged +=
          new EventHandler<StatusChangedEventArgs>( KinectSensors_StatusChanged );

        // 接続されているKinectの動作を開始する
        for ( int i = 0; i < KinectSensor.KinectSensors.Count; i++ ) {
          if ( KinectSensor.KinectSensors[i].Status == KinectStatus.Connected ) {
            kinectWindows[i].StartKinect( KinectSensor.KinectSensors[i] );
            comboBoxSkeleton.Items.Add( i.ToString() );
          }
        }

        comboBoxSkeleton.SelectedIndex = 0;
      }
      catch ( Exception ex ) {
        MessageBox.Show( ex.Message );
        Close();
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
        Start( e );
      }
      // デバイスが切断された
      else if ( e.Status == KinectStatus.Disconnected ) {
        Stop( e );
      }
      // ACが抜けてる
      else if ( e.Status == KinectStatus.NotPowered ) {
        Stop( e );

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
    /// Kinectの動作を開始する
    /// </summary>
    /// <param name="e"></param>
    private void Start( StatusChangedEventArgs e )
    {
      for ( int i = 0; i < kinectWindows.Length; i++ ) {
        if ( kinectWindows[i].Kinect == null ) {
          kinectWindows[i].StartKinect( e.Sensor );
          comboBoxSkeleton.Items.Add( i.ToString() );
          break;
        }
      }
    }

    /// <summary>
    /// Kinectの動作を停止する
    /// </summary>
    /// <param name="e"></param>
    private void Stop( StatusChangedEventArgs e )
    {
      for ( int i = 0; i < kinectWindows.Length; i++ ) {
        if ( kinectWindows[i].Kinect == e.Sensor ) {
          kinectWindows[i].StopKinect();
          comboBoxSkeleton.Items.Remove( i.ToString() );
          break;
        }
      }
    }

    /// <summary>
    /// Windowが閉じられた
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_Closing( object sender, System.ComponentModel.CancelEventArgs e )
    {
      for ( int i = 0; i < kinectWindows.Length; i++ ) {
        if ( kinectWindows[i].Kinect != null ) {
          kinectWindows[i].StopKinect();
        }
      }
    }

    /// <summary>
    /// スケルトン・トラッキングさせるデバイスが変更された
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void comboBox1_SelectionChanged( object sender,
      System.Windows.Controls.SelectionChangedEventArgs e )
    {
      for ( int i = 0; i < kinectWindows.Length; i++ ) {
        if ( (kinectWindows[i].Kinect != null) &&
          (kinectWindows[i].Kinect.SkeletonStream.IsEnabled) ) {
          kinectWindows[i].Kinect.SkeletonStream.Disable();
          break;
        }
      }

      kinectWindows[comboBoxSkeleton.SelectedIndex].Kinect.SkeletonStream.Enable();
    }
  }
}
