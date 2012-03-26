using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Kinect;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace WindowsForms
{
  public partial class Form1 : Form
  {
    readonly int Bgr32BytesPerPixel = 4;  // ピクセルあたりのバイト数

    public Form1()
    {
      InitializeComponent();

      try {
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
      kinect.ColorStream.Enable();
      kinect.DepthStream.Enable();
      kinect.SkeletonStream.Enable();

      kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>( kinect_AllFramesReady );

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
          kinect.AllFramesReady -= kinect_AllFramesReady;

          kinect.Stop();
          kinect.Dispose();
          kinect = null;

          pictureBoxRgb.Image = null;
          pictureBoxDepth.Image = null;
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
            Bitmap bitmap = new Bitmap( kinect.ColorStream.FrameWidth, kinect.ColorStream.FrameHeight,
                        System.Drawing.Imaging.PixelFormat.Format32bppRgb );

            Rectangle rect = new Rectangle( 0, 0, bitmap.Width, bitmap.Height );
            BitmapData data = bitmap.LockBits( rect, ImageLockMode.WriteOnly,
                                    PixelFormat.Format32bppRgb );
            Marshal.Copy( colorPixel, 0, data.Scan0, colorPixel.Length );
            bitmap.UnlockBits( data );

            pictureBoxRgb.Image = bitmap;
          }
        }

        // 書き込み用のビットマップデータを作成(32bit bitmap)
        // 16bpp グレースケールは表示できない
        // 距離カメラのフレームデータを取得する
        using ( DepthImageFrame depthFrame = e.OpenDepthImageFrame() ) {
          if ( depthFrame != null ) {
            // 距離データを画像化して表示
            Bitmap bitmap = new Bitmap( kinect.DepthStream.FrameWidth, kinect.DepthStream.FrameHeight,
                        System.Drawing.Imaging.PixelFormat.Format32bppRgb );

            Rectangle rect = new Rectangle( 0, 0, bitmap.Width, bitmap.Height );
            BitmapData data = bitmap.LockBits( rect, ImageLockMode.WriteOnly,
                                    System.Drawing.Imaging.PixelFormat.Format32bppRgb );
            byte[] gray = ConvertDepthColor( kinect, depthFrame );
            Marshal.Copy( gray, 0, data.Scan0, gray.Length );
            bitmap.UnlockBits( data );

            pictureBoxDepth.Image = bitmap;
          }
        }

        // スケルトンのフレームを取得する
        using ( SkeletonFrame skeletonFrame = e.OpenSkeletonFrame() ) {
          if ( skeletonFrame != null ) {
            Graphics g = Graphics.FromImage( pictureBoxRgb.Image );

            // スケルトンのデータを取得する
            Skeleton[] skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
            skeletonFrame.CopySkeletonDataTo( skeletons );

            // トラッキングされているスケルトンのジョイントを描画する
            foreach ( var skeleton in skeletons ) {
              // スケルトンがトラッキング状態(デフォルトモード)の場合は、ジョイントを描画する
              if ( skeleton.TrackingState == SkeletonTrackingState.Tracked ) {
                // ジョイントを描画する
                foreach ( Joint joint in skeleton.Joints ) {
                  // ジョイントがトラッキングされていなければ次へ
                  if ( joint.TrackingState != JointTrackingState.Tracked ) {
                    continue;
                  }

                  // スケルトンの座標を、RGBカメラの座標に変換して円を書く
                  DrawEllipse( kinect, g, joint.Position );
                }
              }
              // スケルトンが位置追跡(ニアモードの)の場合は、スケルトン位置(Center hip)を描画する
              else if ( skeleton.TrackingState == SkeletonTrackingState.PositionOnly ) {
                // スケルトンの座標を、RGBカメラの座標に変換して円を書く
                DrawEllipse( kinect, g, skeleton.Position );
              }
            }
          }
        }
      }
      catch ( Exception ex ) {
        MessageBox.Show( ex.Message );
      }
    }

    private static void DrawEllipse( KinectSensor kinect, Graphics g, SkeletonPoint position )
    {
      const int R = 5;

      ColorImagePoint point = kinect.MapSkeletonPointToColor( position,
        kinect.ColorStream.Format );
      g.DrawEllipse( new Pen( Brushes.Red, R ),
        new Rectangle( point.X - R, point.Y - R, R * 2, R * 2 ) );
    }


    /// <summary>
    /// 距離データをカラー画像に変換する
    /// </summary>
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
    /// Windowが閉じられるときのイベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Form1_FormClosing( object sender, FormClosingEventArgs e )
    {
      StopKinect( KinectSensor.KinectSensors[0] );
    }

    /// <summary>
    /// 距離カメラの通常/近接モード変更イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void comboBoxRange_SelectedIndexChanged( object sender, EventArgs e )
    {
      try {
        KinectSensor.KinectSensors[0].DepthStream.Range = (DepthRange)comboBoxRange.SelectedIndex;
      }
      catch ( Exception ) {
        comboBoxRange.SelectedIndex = 0;
      }
    }
  }
}
