Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Forms
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Shapes

Public Class PaintWindow
    Inherits Window

    Private SelectRegion As Rect

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        InitializeComponent()

        InitWindowSize()

        Me.selectRegion = New Rect()
    End Sub

    ''' <summary>
    ''' 指定された領域の座標を設定
    ''' </summary>
    ''' <param name="value"></param>
    ''' <remarks></remarks>
    Sub SetSelectedRegion(value As Rect)
        Me.SelectRegion = value
    End Sub

    ''' <summary>
    ''' 線を引く
    ''' </summary>
    ''' <param name="startPoint"></param>
    ''' <param name="endPoint"></param>
    ''' <remarks></remarks>
    Sub DrawLine(startPoint As Point, endPoint As Point)
        ' 座標をディスプレイ座標に変換
        startPoint = ConvertCoordinate(startPoint)
        endPoint = ConvertCoordinate(endPoint)

        ' 線を追加
        Dim _line As New Line()
        _line.Stroke = New SolidColorBrush(Color.FromRgb(220, 220, 220))
        _line.X1 = startPoint.X
        _line.Y1 = startPoint.Y
        _line.X2 = endPoint.X
        _line.Y2 = endPoint.Y
        _line.StrokeThickness = 70                      ' 太さ70
        _line.StrokeStartLineCap = PenLineCap.Round     ' 始点を丸める
        _line.StrokeEndLineCap = PenLineCap.Round       ' 終点を丸める

        PaintCanvas.Children.Add(_line)
    End Sub

    ''' <summary>
    ''' ウィンドウサイズの設定
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitWindowSize()
        Me.Left = Screen.AllScreens(0).WorkingArea.Left
        Me.Top = Screen.AllScreens(0).WorkingArea.Top
        Width = Screen.AllScreens(0).WorkingArea.Width
        Height = Screen.AllScreens(0).WorkingArea.Height

        Me.PaintCanvas.Width = Screen.AllScreens(0).WorkingArea.Width
        Me.PaintCanvas.Height = Screen.AllScreens(0).WorkingArea.Height
    End Sub

    ''' <summary>
    ''' 指定した領域座標からディスプレイ座標に変換
    ''' </summary>
    ''' <param name="_point"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ConvertCoordinate(_point As Point) As Point
        Dim p = New Point()

        p.X = PaintCanvas.Width * (1 - (_point.X - SelectRegion.X) / SelectRegion.Width)
        p.Y = PaintCanvas.Height * ((_point.Y - SelectRegion.Y) / SelectRegion.Height)

        Return p
    End Function
End Class
