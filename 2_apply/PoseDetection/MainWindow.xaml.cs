using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using Microsoft.Kinect;
using System.Windows.Shapes;

namespace PoseDetection
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

      // スケルトンを有効にして、フレーム更新イベントを登録する
      kinect.SkeletonFrameReady +=
        new EventHandler<SkeletonFrameReadyEventArgs>( kinect_SkeletonFrameReady );
      kinect.SkeletonStream.Enable();

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

          RGBCameraImage.Source = null;
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
            RGBCameraImage.Source = BitmapSource.Create( colorFrame.Width, colorFrame.Height, 96, 96,
                PixelFormats.Bgr32, null, colorPixel, colorFrame.Width * colorFrame.BytesPerPixel );
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
            DrawCrossPoint( kinect, skeletonFrame );
          }
        }
      }
      catch ( Exception ex ) {
        MessageBox.Show( ex.Message );
      }
    }

    /// <summary>
    /// 交差点の描画
    /// </summary>
    /// <param name="kinect"></param>
    /// <param name="skeletonFrame"></param>
    private void DrawCrossPoint( KinectSensor kinect, SkeletonFrame skeletonFrame )
    {
      // 描画する円の半径
      const int R = 5;

      // キャンバスをクリアする
      canvas.Children.Clear();

      // スケルトンから4か所の座標を取得（左肘・手、右肘・手）
      var joints = GetFourSkeletonPosition( skeletonFrame, JointType.ElbowLeft, JointType.HandLeft,
        JointType.ElbowRight, JointType.HandRight );
      if ( joints == null ) {
        return;
      }

      // スケルトン座標をRGB画像の座標に変換し表示する
      ColorImagePoint[] jointImagePosition = new ColorImagePoint[4];
      for ( int i = 0; i < 4; i++ ) {
        jointImagePosition[i] =
            kinect.MapSkeletonPointToColor( joints[i].Position, kinect.ColorStream.Format );

        canvas.Children.Add( new Ellipse()
        {
          Fill = new SolidColorBrush( Colors.Yellow ),
          Margin = new Thickness( jointImagePosition[i].X - R, jointImagePosition[i].Y - R, 0, 0 ),
          Width = R * 2,
          Height = R * 2,
        } );
      }

      // 腕がクロスしているかチェック
      bool isCross = CrossHitCheck( joints[0].Position, joints[1].Position,
        joints[2].Position, joints[3].Position );
      if ( isCross ) {
        // クロスしている点を計算して円を表示する
        ColorImagePoint crossPoint = GetCrossPoint( jointImagePosition[0], jointImagePosition[1],
          jointImagePosition[2], jointImagePosition[3] );

        CrossEllipse.Margin = new Thickness( crossPoint.X - CrossEllipse.Width / 2,
          crossPoint.Y - CrossEllipse.Height / 2, 0, 0 );
        CrossEllipse.Visibility = System.Windows.Visibility.Visible;
        canvas.Children.Add( CrossEllipse );
      }
      else {
        CrossEllipse.Visibility = System.Windows.Visibility.Hidden;
      }
    }

    /// <summary>
    /// 指定した4か所のスケルトンジョイントを取得する
    /// </summary>
    /// <param name="skeletonFrame"></param>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    /// <param name="pos3"></param>
    /// <param name="pos4"></param>
    /// <returns></returns>
    private Joint[] GetFourSkeletonPosition( SkeletonFrame skeletonFrame,
      JointType pos1, JointType pos2, JointType pos3, JointType pos4 )
    {
      JointType[] jointTypes = new JointType[4] {
        pos1, pos2, pos3, pos4
      };

      // スケルトンのデータを取得する
      Skeleton[] skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
      skeletonFrame.CopySkeletonDataTo( skeletons );

      // トラッキングされているスケルトンのジョイントをする
      foreach ( Skeleton skeleton in skeletons ) {
        // スケルトンがトラッキング状態(デフォルトモードの)の場合は、ジョイントを描画する
        if ( skeleton.TrackingState == SkeletonTrackingState.Tracked ) {
          // ジョイントを抽出する
          Joint[] joints = new Joint[4];
          for(int i =  0; i < 4; i++){
            // 指定のジョイントがトラッキングされてない場合はnullを返す
            if ( skeleton.Joints[jointTypes[i]].TrackingState == JointTrackingState.NotTracked ) {
              return null;
            }

            joints[i] = skeleton.Joints[jointTypes[i]];
          }

          return joints;
        }
      }

      return null;
    }

    /// <summary>
    /// 2直線が交差しているか調べる
    /// </summary>
    /// <param name="a1">線分aの始点</param>
    /// <param name="a2">線分aの終点</param>
    /// <param name="b1">線分bの始点</param>
    /// <param name="b2">線分bの終点</param>
    /// <returns>交差しているかどうか</returns>
    private bool CrossHitCheck( SkeletonPoint a1, SkeletonPoint a2,
      SkeletonPoint b1, SkeletonPoint b2 )
    {
      // 外積：ax * by - ay * bx
      // 外積を使用して交差判定を行う
      double v1 = (a2.X - a1.X) * (b1.Y - a1.Y) - (a2.Y - a1.Y) * (b1.X - a1.X);
      double v2 = (a2.X - a1.X) * (b2.Y - a1.Y) - (a2.Y - a1.Y) * (b2.X - a1.X);
      double m1 = (b2.X - b1.X) * (a1.Y - b1.Y) - (b2.Y - b1.Y) * (a1.X - b1.X);
      double m2 = (b2.X - b1.X) * (a2.Y - b1.Y) - (b2.Y - b1.Y) * (a2.X - b1.X);

      // +-, -+だったらマイナス値になるのでそれぞれをかけて確認する
      // 二つとも左右にあった場合は交差している
      if ( (v1 * v2 <= 0) && (m1 * m2 <= 0) ) {
        return true;
      }
      else {
        return false;
      }
    }

    /// <summary>
    /// 2直線の交差点を計算する
    /// </summary>
    /// <param name="a1">線分aの始点</param>
    /// <param name="a2">線分aの終点</param>
    /// <param name="b1">線分bの始点</param>
    /// <param name="b2">線分bの終点</param>
    /// <returns>交差点の座標</returns>
    private ColorImagePoint GetCrossPoint( ColorImagePoint a1, ColorImagePoint a2,
      ColorImagePoint b1, ColorImagePoint b2 )
    {
      // 1つめの式
      float v1a = ((a1.Y - a2.Y) / (float)(a1.X - a2.X));
      float v1b = (a1.X * a2.Y - a1.Y * a2.X) / (float)(a1.X - a2.X);

      // 2つめの式
      float v2a = (b1.Y - b2.Y) / (float)(b1.X - b2.X);
      float v2b = (b1.X * b2.Y - b1.Y * b2.X) / (float)(b1.X - b2.X);

      // 最終的な交点を返す
      ColorImagePoint crossPoint = new ColorImagePoint(); 
      crossPoint.X = (int)((v2b - v1b) / (float)(v1a - v2a));
      crossPoint.Y = (int)(v1a * (float)crossPoint.X + v1b);
      return crossPoint;
    }


    /// <summary>
    /// Windowsが閉じられるときのイベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_Closing( object sender, System.ComponentModel.CancelEventArgs e )
    {
      StopKinect( KinectSensor.KinectSensors[0] );
    }
  }
}
