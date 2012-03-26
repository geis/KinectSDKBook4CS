using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using Geis.Win32;
using System.Collections.Generic;

namespace KinectMouse
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window
  {
    KinectSensor kinect;

    readonly int Bgr32BytesPerPixel = PixelFormats.Bgr32.BitsPerPixel / 8;

    const int R = 5;        // 手の位置を描画する円の大きさ

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public MainWindow()
    {
      InitializeComponent();

      StartKinect(KinectSensor.KinectSensors[0]);
    }

    /// <summary>
    /// RGBカメラ、スケルトンのフレーム更新イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void kinect_AllFramesReady( object sender, AllFramesReadyEventArgs e )
    {
      try {
        KinectSensor kinect = sender as KinectSensor;

        using ( ColorImageFrame colorFrame = e.OpenColorImageFrame() ) {
          if ( colorFrame != null ) {
            byte[] colorPixel = new byte[colorFrame.PixelDataLength];
            colorFrame.CopyPixelDataTo( colorPixel );
            imageRgb.Source = BitmapSource.Create( colorFrame.Width, colorFrame.Height, 96, 96,
                PixelFormats.Bgr32, null, colorPixel, colorFrame.Width * colorFrame.BytesPerPixel );
          }
        }

        using ( SkeletonFrame skeletonFrame = e.OpenSkeletonFrame() ) {
          if ( skeletonFrame != null ) {
            // トラッキングされているスケルトンのジョイントを描画する
            Skeleton skeleton = skeletonFrame.GetFirstTrackedSkeleton();
            
            if ( (skeleton != null) && (skeleton.TrackingState == SkeletonTrackingState.Tracked) ) {
              Joint hand = skeleton.Joints[JointType.HandRight];
              if ( hand.TrackingState == JointTrackingState.Tracked ) {
                ImageSource source = imageRgb.Source;
                DrawingVisual drawingVisual = new DrawingVisual();
                
                using ( DrawingContext drawingContext = drawingVisual.RenderOpen() ) {
                  //バイト列をビットマップに展開
                  //描画可能なビットマップを作る
                  drawingContext.DrawImage( imageRgb.Source,
                    new Rect( 0, 0, source.Width, source.Height ) );

                  // 手の位置に円を描画
                  DrawSkeletonPoint( drawingContext, hand );
                }

                // 描画可能なビットマップを作る
                // http://stackoverflow.com/questions/831860/generate-bitmapsource-from-uielement
                RenderTargetBitmap bitmap = new RenderTargetBitmap( (int)source.Width,
                  (int)source.Height, 96, 96, PixelFormats.Default );
                bitmap.Render( drawingVisual );

                imageRgb.Source = bitmap;

                // Frame中の手の位置をディスプレイの位置に対応付ける
                ColorImagePoint point = kinect.MapSkeletonPointToColor( hand.Position,        // スケルトン座標 → RGB画像座標
                  kinect.ColorStream.Format );                                                
                System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.AllScreens[0];   // メインディスプレイの情報を取得
                point.X = (point.X * screen.Bounds.Width) / kinect.ColorStream.FrameWidth;
                point.Y = (point.Y * screen.Bounds.Height) / kinect.ColorStream.FrameHeight;

                // マウスカーソルの移動
                SendInput.MouseMove( point.X, point.Y, screen );

                // クリック動作
                if ( IsClicked( skeletonFrame, point ) ) {
                  SendInput.LeftClick();
                }
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
    /// Kinectの動作を開始する
    /// </summary>
    /// <param name="kin"></param>
    private void StartKinect( KinectSensor kin )
    {
      try {
        if ( KinectSensor.KinectSensors.Count == 0 ) {
          throw new Exception( "Kinectが接続されていません" );
        }

        kinect = kin;
        kinect.ColorStream.Enable();
        kinect.SkeletonStream.Enable( new TransformSmoothParameters()
        {
          Smoothing = 0.7f,
          Correction = 0.3f,
          Prediction = 0.4f,
          JitterRadius = 0.10f,
          MaxDeviationRadius = 0.5f,
        } );

        kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>( kinect_AllFramesReady );

        kinect.Start();
      }
      catch ( Exception ex ) {
        MessageBox.Show( ex.Message );
        Close();
      }
    }

    /// <summary>
    /// Kinectの動作を停止する
    /// </summary>
    /// <param name="kinect"></param>
    private void StopKinect( KinectSensor kinect )
    {
      if ( kinect != null ) {
        if ( kinect.IsRunning ) {
          kinect.AllFramesReady -= kinect_AllFramesReady;

          kinect.Stop();
          kinect.Dispose();
        }
      }
    }

    /// <summary>
    /// クリック動作を行ったのかチェック
    /// </summary>
    /// <param name="skeletonFrame"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    private bool IsClicked( SkeletonFrame skeletonFrame, ColorImagePoint point )
    {
      return IsSteady( skeletonFrame, point );
    }

    /// <summary>
    /// 座標管理用構造体
    /// </summary>
    struct FramePoint
    {
      public ColorImagePoint Point;
      public long TimeStamp;
    }

    int milliseconds = 2000;        // 認識するまでの停止時間の設定
    int threshold = 10;             // 座標の変化量の閾値

    // 基点となるポイント。
    // この座標からあまり動かない場合 Steady状態であると認識する。
    FramePoint basePoint = new FramePoint(); 

    /// <summary>
    /// 停止状態にあるかチェックする
    /// </summary>
    /// <param name="skeletonFrame"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    bool IsSteady( SkeletonFrame skeletonFrame, ColorImagePoint point )
    {
      var currentPoint = new FramePoint()
      {
        Point = point,
        TimeStamp = skeletonFrame.Timestamp,
      };

      // milliseconds時間経過したら steady
      if ( (currentPoint.TimeStamp - basePoint.TimeStamp) > milliseconds ) {
        basePoint = currentPoint;
        return true;
      }

      // 座標の変化量がthreshold以上ならば、basePointを更新して初めから計測
      if ( Math.Abs( currentPoint.Point.X - basePoint.Point.X ) > threshold 
        || Math.Abs( currentPoint.Point.Y - basePoint.Point.Y ) > threshold ) {
        
        // 座標が動いたので基点を動いた位置にずらして、最初から計測
        basePoint = currentPoint;
      }
    
      return false;
    }

    /// <summary>
    /// jointの座標に円を描く
    /// </summary>
    /// <param name="drawingContext"></param>
    /// <param name="joint"></param>
    private void DrawSkeletonPoint( DrawingContext drawingContext, Joint joint )
    {
      if ( joint.TrackingState != JointTrackingState.Tracked ) {
        return;
      }

      // 円を書く
      ColorImagePoint point = kinect.MapSkeletonPointToColor( joint.Position,
        kinect.ColorStream.Format );
      drawingContext.DrawEllipse( new SolidColorBrush( Colors.Red ),
          new Pen( Brushes.Red, 1 ), new Point( point.X, point.Y ), R, R );
    }

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
