using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Microsoft.Kinect;

namespace TouchApplication
{
  /// <summary>
  /// MainWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class MainWindow : Window
  {
    /// <summary>
    /// セレクトモード
    /// </summary>
    enum SelectMode
    {
      NOTSELECTED,        // 領域が選択されていない
      SELECTING,          // 領域を選択中
      SELECTED            // 領域が選択されている
    };

    readonly int Bgr32BytesPerPixel = PixelFormats.Bgr32.BitsPerPixel / 8;
    readonly int ERROR_OF_POINT = -100;     // タッチポイントのエラー値設定

    KinectSensor kinect;
    PaintWindow paintWindow;
    SelectMode currentMode = SelectMode.NOTSELECTED;      // 現在の領域選択モード

    Point startPointOfRect;         // 指定領域の始点
    Point preTouchPoint;            // 前フレームのタッチ座標

    Rect selectRegion;              // 指定した領域
    short[] backgroundDepthData;    // 領域内のデプスマップ
    short[] depthPixel;
    ColorImagePoint[] colorImagePixelPoint;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public MainWindow()
    {
      try {
        InitializeComponent();

        StartKinect( KinectSensor.KinectSensors[0] );

        preTouchPoint = new Point( ERROR_OF_POINT, ERROR_OF_POINT );
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
        kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
        kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
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
    /// RGBカメラ、距離カメラのフレーム更新イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void kinect_AllFramesReady( object sender, AllFramesReadyEventArgs e )
    {
      
      try {
        using ( ColorImageFrame colorFrame = e.OpenColorImageFrame() ) {
          using ( DepthImageFrame depthFrame = e.OpenDepthImageFrame() ) {
            if ( depthFrame != null && colorFrame != null && kinect.IsRunning) {
              if ( depthPixel == null ) {
                depthPixel = new short[depthFrame.PixelDataLength];
                colorImagePixelPoint = new ColorImagePoint[depthFrame.PixelDataLength];
              }

              // 描画を3フレームに1回にする
              if ( depthFrame.FrameNumber % 3 != 0 )
                return;

              depthFrame.CopyPixelDataTo( depthPixel );
             
              // Depthデータの座標をRGB画像の座標に変換する
              kinect.MapDepthFrameToColorFrame( kinect.DepthStream.Format, depthPixel,
                kinect.ColorStream.Format, colorImagePixelPoint );
              
              // カメラ画像の描画
              byte[] colorPixel = new byte[colorFrame.PixelDataLength];
              colorFrame.CopyPixelDataTo( colorPixel );

              // RGB画像の位置を距離画像の位置に補正
              colorPixel = CoordinateColorImage( colorImagePixelPoint, colorPixel );
              
              CameraImage.Source = BitmapSource.Create( colorFrame.Width, colorFrame.Height, 96, 96,
                PixelFormats.Bgr32, null, colorPixel,
                colorFrame.Width * colorFrame.BytesPerPixel );
            }
          }
        }
      }
      catch ( Exception ex ) {
        MessageBox.Show( ex.Message );
      }

      // モードに応じた処理
      switch ( currentMode ) {
      case SelectMode.SELECTING:
        // 領域を指定中ならば描画も更新
        UpdateRectPosition();

        break;

      case SelectMode.SELECTED:
        // 領域内を触っているかチェック
        Point point = CheckThePointTouchingTheRegion();
        UpdateTouchingPointEllipse( point );
        UpdatePaintCanvas( point );

        break;
      }
    }

    /// <summary>
    /// 選択領域を表すRectangleの描画を更新
    /// </summary>
    private void UpdateRectPosition()
    {
      // 現在のマウスの位置を取得
      Point currentPoint = Mouse.GetPosition( CameraImage );

      Rect rect;
      // 始点と終点の位置によって値を変更
      //
      // 終点がスタート位置の左上
      if ( currentPoint.X < startPointOfRect.X && currentPoint.Y < startPointOfRect.Y ) {
        rect = new Rect( currentPoint.X,
                        currentPoint.Y,
                        Math.Abs( startPointOfRect.X - currentPoint.X ),
                        Math.Abs( startPointOfRect.Y - currentPoint.Y )
                     );
      } // 終点がスタート位置の左下
      else if ( currentPoint.X < startPointOfRect.X ) {
        rect = new Rect( currentPoint.X,
                        startPointOfRect.Y,
                        Math.Abs( startPointOfRect.X - currentPoint.X ),
                        Math.Abs( startPointOfRect.Y - currentPoint.Y )
                     );
      } // 終点がスタート位置の右上
      else if ( currentPoint.Y < startPointOfRect.Y ) {
        rect = new Rect( startPointOfRect.X,
                        currentPoint.Y,
                        Math.Abs( startPointOfRect.X - currentPoint.X ),
                        Math.Abs( startPointOfRect.Y - currentPoint.Y )
                     );
      } // 終点がスタート位置の右下
      else {
        rect = new Rect( startPointOfRect.X,
                        startPointOfRect.Y,
                        Math.Abs( startPointOfRect.X - currentPoint.X ),
                        Math.Abs( startPointOfRect.Y - currentPoint.Y )
                     );
      }

      // Rectangleの配置
      Canvas.SetLeft( SelectRectangle, rect.X );
      Canvas.SetTop( SelectRectangle, rect.Y );
      SelectRectangle.Width = rect.Width;
      SelectRectangle.Height = rect.Height;

      // 選択領域の保存
      selectRegion = rect;
    }

    /// <summary>
    /// タッチしている所を表すEllipse(円)の描画を更新
    /// </summary>
    /// <param name="p"></param>
    private void UpdateTouchingPointEllipse( Point p )
    {
      TouchPoint.Width = 20;
      TouchPoint.Height = 20;
      Canvas.SetLeft( TouchPoint, p.X - TouchPoint.Width / 2 );
      Canvas.SetTop( TouchPoint, p.Y - TouchPoint.Height / 2 );
    }

    /// <summary>
    /// ペイント用ウィンドウの更新
    /// </summary>
    /// <param name="p"></param>
    private void UpdatePaintCanvas( Point p )
    {
      // 座標変化なし・ 初期値やエラー値は除く
      if ( p.X == preTouchPoint.X && p.Y == preTouchPoint.Y )         // タッチ座標の変化なし
        return;
      else if ( (p.X == ERROR_OF_POINT && p.Y == ERROR_OF_POINT) ||   // 現在のフレームでタッチはされていない
                (preTouchPoint.X == ERROR_OF_POINT && preTouchPoint.Y == ERROR_OF_POINT) ) {  // 前フレームでタッチはされていない
        preTouchPoint = p;
        return;
      }

      // 線を引く
      if ( paintWindow != null ) {
        paintWindow.DrawLine( preTouchPoint, p );
      }

      preTouchPoint = p;
    }

    /// <summary>
    /// RGBカメラからの画像を距離カメラの画像の位置に合わせる
    /// </summary>
    /// <param name="points">RGB画像と距離の対応付けのデータ</param>
    /// <param name="colorPixels">RGB画像のバイト配列</param>
    /// <returns></returns>
    private byte[] CoordinateColorImage( ColorImagePoint[] points, byte[] colorPixels )
    {
      ColorImageStream colorStream = kinect.ColorStream;

      // 出力バッファ(初期値はRGBカメラの画像)
      byte[] outputColor = new byte[colorPixels.Length];
      for ( int i = 0; i < outputColor.Length; i += Bgr32BytesPerPixel ) {
        outputColor[i] = colorPixels[i];
        outputColor[i + 1] = colorPixels[i + 1];
        outputColor[i + 2] = colorPixels[i + 2];
      }
      
      for ( int index = 0; index < depthPixel.Length; index++ ) {

        // 変換した結果が、フレームサイズを超えることがあるため、小さいほうを使う
        int x = Math.Min( points[index].X, colorStream.FrameWidth - 1 );
        int y = Math.Min( points[index].Y, colorStream.FrameHeight - 1 );
        int colorIndex = ((y * kinect.DepthStream.FrameWidth) + x) * Bgr32BytesPerPixel;
        int outputIndex = index * Bgr32BytesPerPixel;

        // カラー画像のピクセルを調整された座標値に変換する
        outputColor[outputIndex] = colorPixels[colorIndex];
        outputColor[outputIndex + 1] = colorPixels[colorIndex + 1];
        outputColor[outputIndex + 2] = colorPixels[colorIndex + 2];
        
      }

      return outputColor;
    }
    
    /// <summary>
    /// 指定された領域内の現在の距離情報を保存する
    /// </summary>
    private void SaveBackgroundDepth()
    {
      DepthImageStream depthStream = kinect.DepthStream;

      backgroundDepthData = new short[(int)(selectRegion.Width * selectRegion.Height)];

      for ( int counter = 0, y = (int)selectRegion.Y; y < (selectRegion.Y + selectRegion.Height); y++ ) {
        for ( int x = (int)selectRegion.X; x < (selectRegion.X + selectRegion.Width); x++, counter++ ) {
          backgroundDepthData[counter] = (short)(depthPixel[y * depthStream.FrameWidth + x] >> DepthImageFrame.PlayerIndexBitmaskWidth);
        }
      }
    }

    /// <summary>
    /// タッチ判定・座標の取得
    /// </summary>
    /// <returns>タッチ座標</returns>
    private Point CheckThePointTouchingTheRegion()
    {
      DepthImageStream depthStream = kinect.DepthStream;

      int distanceThreshold = 30;                 // BackgroundDepthDataとの最大誤差値
      int distanceBetweenWallThreshold = 20;      // BackgroundDepthDataとの最小誤差値
      var touchPoints = new List<Point>();
      var rePoint = new Point( ERROR_OF_POINT, ERROR_OF_POINT );  // 返却用座標

      // 深度の変化のポイントをピクセル毎に探査
      for ( int counter = 0, y = (int)selectRegion.Y; y < (selectRegion.Y + selectRegion.Height); y++ ) {
        for ( int x = (int)selectRegion.X; x < (selectRegion.X + selectRegion.Width); x++, counter++ ) {
          short currentDepthVal = (short)(depthPixel[y * depthStream.FrameWidth + x] >> DepthImageFrame.PlayerIndexBitmaskWidth);

          // 保存したデプスマップより深度が近くなっておりかつその深度の変化が
          // distanceThreshold と distanceBetweenWallThreshold 内であるポイントを探査
          if ( backgroundDepthData[counter] > currentDepthVal &&
                       (backgroundDepthData[counter] - currentDepthVal) < distanceThreshold &&
                       (backgroundDepthData[counter] - currentDepthVal) > distanceBetweenWallThreshold ) {
                         touchPoints.Add( new Point( x, y ) );
          }
        }
      }

      // 検出した変化Pointから重心を計算
      int numThreshold = 50;      // 変化を検出したポイントの閾値

      if ( touchPoints.Count > numThreshold ) {
        double xSum = 0;
        double ySum = 0;
        foreach ( Point p in touchPoints ) {
          xSum += p.X;
          ySum += p.Y;
        }
  
        // タッチされた座標を設定
        rePoint.X = xSum / touchPoints.Count;
        rePoint.Y = ySum / touchPoints.Count;
      }

      return rePoint;
    }

    /// <summary>
    /// マウスの左クリックが押された時のイベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_MouseLeftButtonDown( object sender, System.Windows.Input.MouseButtonEventArgs e )
    {
      if ( e.LeftButton == MouseButtonState.Pressed ) {
        // 始点を保存
        startPointOfRect = e.MouseDevice.GetPosition( SelectCanvas );

        currentMode = SelectMode.SELECTING;
      }
    }

    /// <summary>
    /// マウスの左クリックが離された時のイベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_MouseLeftButtonUp( object sender, System.Windows.Input.MouseButtonEventArgs e )
    {
      if ( e.LeftButton == MouseButtonState.Released && currentMode == SelectMode.SELECTING ) {
        // 現在の距離情報を追加
        SaveBackgroundDepth();

        currentMode = SelectMode.SELECTED;
      }
    }

    /// <summary>
    /// ウィンドウを閉じている
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Window_Closing( object sender, System.ComponentModel.CancelEventArgs e )
    {
      StopKinect( KinectSensor.KinectSensors[0] );
    }

    /// <summary>
    /// StartButtonコントロールが押された時のイベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void StartButton_Click( object sender, RoutedEventArgs e )
    {
      if ( currentMode == SelectMode.SELECTED ) {
        
        // ペイント用ウィンドウの作成・表示
        paintWindow = new PaintWindow();
        paintWindow.SetSelectedRegion( selectRegion );
        paintWindow.Show();
      }
    }
  }
}
