using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;
using Hisui.OpenGL;

namespace KinectPointCloud
{

  public class PointCloudScene : Hisui.Graphics.IScene
  {
    const float PointSize = 1.0f;

    RGB[] rgb;
    Depth[] depth;

    /// <summary>
    /// RGBの値
    /// </summary>
    public class RGB
    {
      public float R, G, B;

      public RGB( float _r, float _g, float _b )
      {
        R = _r / 255;
        G = _g / 255;
        B = _b / 255;
      }
    }

    /// <summary>
    /// 奥行きの値
    /// </summary>
    public class Depth
    {
      public static float FrameWidth = 640;
      public static float FrameHeight = 480;
      public static float MaxDepth = 4000;

      public float X,Y, Z;
      public Depth( float _x, float _y, float _z )
      {
        // -1.0～1.0の間に正規化する
        X = (float)(_x * (2.0 / FrameWidth) - 1.0);
        Y = (float)((FrameHeight - _y) * (2.0 / FrameHeight) - 1.0);
        Z = (float)(_z * (2.0 / MaxDepth) - 1.0);
      }
    }

    public PointCloudScene()
    {
      // Kinectが接続されているかどうかを確認する
      if ( KinectSensor.KinectSensors.Count == 0 ) {
        throw new Exception( "Kinectを接続してください" );
      }

      // Kinectの動作を開始する
      StartKinect( KinectSensor.KinectSensors[0] );
    }

    /// <summary>
    /// Kinectの動作を開始する
    /// </summary>
    /// <param name="kinect"></param>
    private void StartKinect( KinectSensor kinect )
    {
      // RGBカメラ、距離カメラ、スケルトン・トラッキング(プレイヤーの認識)を有効にする
      kinect.ColorStream.Enable();
      kinect.DepthStream.Enable();

      kinect.Start();

      // 変換のための基準値を設定
      Depth.FrameWidth = kinect.DepthStream.FrameWidth;
      Depth.FrameHeight = kinect.DepthStream.FrameHeight;
      Depth.MaxDepth = kinect.DepthStream.MaxDepth;
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
        }
      }
    }

    /// <summary>
    /// Kinectのデータを描画する
    /// </summary>
    /// <param name="sc"></param>
    public void Draw( Hisui.Graphics.ISceneContext sc )
    {
      //Kinect からデータ取得
      if ( GetPoint() ) {
        //点群を描画
        using ( var scope = sc.Push() ) {
          scope.Lighting = false;
          GL.glPointSize( PointSize );
          GL.glBegin( GL.GL_POINTS );
          for ( int i = 0; i < rgb.Length; i++ ) {
            GL.glColor3d( rgb[i].R, rgb[i].G, rgb[i].B );
            GL.glVertex3d( depth[i].X, depth[i].Y, depth[i].Z );
          }
          GL.glEnd();
        }
      }

      //描画が終わる度に次の描画のリクエストを発行する
      Hisui.SI.View.Invalidate();
    }

    /// <summary>
    /// Kinectで取得したデータを点群に変換する
    /// </summary>
    /// <returns></returns>
    bool GetPoint()
    {
      KinectSensor kinect = KinectSensor.KinectSensors[0];
      ColorImageStream colorStream = kinect.ColorStream;
      DepthImageStream depthStream = kinect.DepthStream;

      // RGBカメラと距離カメラのフレームデータを取得する
      using ( ColorImageFrame colorFrame = kinect.ColorStream.OpenNextFrame( 100 ) ) {
        using ( DepthImageFrame depthFrame = kinect.DepthStream.OpenNextFrame( 100 ) ) {
          if ( colorFrame == null || depthFrame == null ) {
            return false;
          }

          // RGBカメラのデータを作成する
          byte[] colorPixel = new byte[colorFrame.PixelDataLength];
          colorFrame.CopyPixelDataTo( colorPixel );

          rgb = new RGB[colorFrame.Width * colorFrame.Height];
          for ( int i = 0; i < rgb.Length; i++ ) {
            int colorIndex = i * 4;
            rgb[i] = new RGB( colorPixel[colorIndex + 2], colorPixel[colorIndex + 1],
              colorPixel[colorIndex] );
          }

          // 距離カメラのピクセルデータを取得する
          short[] depthPixel = new short[depthFrame.PixelDataLength];
          depthFrame.CopyPixelDataTo( depthPixel );

          // 距離カメラの座標に対応するRGBカメラの座標を取得する(座標合わせ)
          ColorImagePoint[] colorPoint = new ColorImagePoint[depthFrame.PixelDataLength];
          kinect.MapDepthFrameToColorFrame( depthStream.Format, depthPixel,
            colorStream.Format, colorPoint );

          // 距離データを作成する
          depth = new Depth[depthFrame.Width * depthFrame.Height];
          for ( int i = 0; i < depth.Length; i++ ) {
            int x = Math.Min( colorPoint[i].X, colorStream.FrameWidth - 1 );
            int y = Math.Min( colorPoint[i].Y, colorStream.FrameHeight - 1 );
            int distance = depthPixel[i] >> DepthImageFrame.PlayerIndexBitmaskWidth;

            depth[i] = new Depth( x, y, distance );
          }
        }
      }

      return true;
    }
  }
}
