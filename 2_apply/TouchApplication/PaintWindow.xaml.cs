using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TouchApplication
{
  /// <summary>
  /// PaintWindow.xaml の相互作用ロジック
  /// </summary>
  public partial class PaintWindow : Window
  {
    Rect selectRegion;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PaintWindow()
    {
      InitializeComponent();

      InitWindowSize();

      selectRegion = new Rect();
    }

    /// <summary>
    /// 指定された領域の座標を設定
    /// </summary>
    /// <param name="rect"></param>
    public void SetSelectedRegion( Rect rect )
    {
      selectRegion = rect;
    }

    /// <summary>
    /// 線を引く
    /// </summary>
    /// <param name="start">始点</param>
    /// <param name="end">終点</param>
    public void DrawLine( Point start, Point end )
    {
      // 座標をディスプレイ座標に変換
      start = ConvertCoordinate( start );
      end = ConvertCoordinate( end );

      // 線を追加
      Line line = new Line();
      line.Stroke = new SolidColorBrush( Color.FromRgb( 220, 220, 220 ) );
      line.X1 = start.X;
      line.Y1 = start.Y;
      line.X2 = end.X;
      line.Y2 = end.Y;
      line.StrokeThickness = 70;                      // 太さ70
      line.StrokeStartLineCap = PenLineCap.Round;     // 始点を丸める
      line.StrokeEndLineCap = PenLineCap.Round;       // 終点を丸める

      PaintCanvas.Children.Add( line );
    }

    /// <summary>
    /// ウィンドウサイズの設定
    /// </summary>
    private void InitWindowSize()
    {
      Left = 0;
      Top = 0;
      Width = Screen.AllScreens[0].Bounds.Width;
      Height = Screen.AllScreens[0].Bounds.Height;

      PaintCanvas.Width = Screen.AllScreens[0].Bounds.Width;
      PaintCanvas.Height = Screen.AllScreens[0].Bounds.Height;
    }

    /// <summary>
    /// 指定した領域座標からディスプレイ座標に変換
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    private Point ConvertCoordinate( Point point )
    {
      Point p = new Point();

      p.X = PaintCanvas.Width * (1 - (point.X - selectRegion.X) / selectRegion.Width);
      p.Y = PaintCanvas.Height * ((point.Y - selectRegion.Y) / selectRegion.Height);

      return p;
    }
  }
}
