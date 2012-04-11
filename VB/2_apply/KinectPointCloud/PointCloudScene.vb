Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Microsoft.Kinect
Imports Hisui.OpenGL

Public Class PointCloudScene
    Implements Hisui.Graphics.IScene

    Private Const PointSize As Single = 1.0F

    Private _Rgb() As RGB
    Private _Depth() As Depth

    ''' <summary>
    ''' RGBの値
    ''' </summary>
    Public Class RGB
        Public R As Single, G As Single, B As Single

        Public Sub New(_r As Single, _g As Single, _b As Single)
            R = _r / 255
            G = _g / 255
            B = _b / 255
        End Sub
    End Class

    ''' <summary>
    ''' 奥行きの値
    ''' </summary>
    Public Class Depth
        Public Shared FrameWidth As Single = 640
        Public Shared FrameHeight As Single = 480
        Public Shared MaxDepth As Single = 4000

        Public X As Single, Y As Single, Z As Single
        Public Sub New(_x As Single, _y As Single, _z As Single)
            ' -1.0～1.0の間に正規化する
            X = CType(_x * (2.0 / FrameWidth) - 1.0, Single)
            Y = CType((FrameHeight - _y) * (2.0 / FrameHeight) - 1.0, Single)
            Z = CType(_z * (2.0 / MaxDepth) - 1.0, Single)
        End Sub
    End Class

    Public Sub New()
        ' Kinectが接続されているかどうかを確認する
        If KinectSensor.KinectSensors.Count = 0 Then
            Throw New Exception("Kinectを接続してください")
        End If

        ' Kinectの動作を開始する
        Call StartKinect(KinectSensor.KinectSensors(0))
    End Sub

    ''' <summary>
    ''' Kinectの動作を開始する
    ''' </summary>
    ''' <param name="kinect"></param>
    ''' <remarks></remarks>
    Public Sub StartKinect(kinect As KinectSensor)
        ' RGBカメラ、距離カメラ、スケルトン・トラッキング(プレイヤーの認識)を有効にする
        kinect.ColorStream.Enable()
        kinect.DepthStream.Enable()

        kinect.Start()

        ' 変換のための基準値を設定
        Depth.FrameWidth = kinect.DepthStream.FrameWidth
        Depth.FrameHeight = kinect.DepthStream.FrameHeight
        Depth.MaxDepth = kinect.DepthStream.MaxDepth
    End Sub

    ''' <summary>
    ''' Kinectの動作を停止する
    ''' </summary>
    ''' <param name="kinect"></param>
    ''' <remarks></remarks>
    Public Sub StopKinect(kinect As KinectSensor)
        If kinect IsNot Nothing Then
            If kinect.IsRunning Then
                ' Kinectの停止と、ネイティブリソースを解放する
                kinect.Stop()
                kinect.Dispose()
            End If
        End If
    End Sub

    ''' <summary>
    ''' Kinectのデータを描画する
    ''' </summary>
    ''' <param name="sc"></param>
    ''' <remarks></remarks>
    Public Sub Draw(sc As Hisui.Graphics.ISceneContext) Implements Hisui.Graphics.IScene.Draw
        'Kinect からデータ取得
        If GetPoint() Then
            '点群を描画
            Using scope As Hisui.Graphics.IScope = sc.Push
                scope.Lighting = False
                GL.glPointSize(PointSize)
                GL.glBegin(GL.GL_POINTS)
                For i As Integer = 0 To Me._Rgb.Length - 1
                    GL.glColor3d(Me._Rgb(i).R,
                                 Me._Rgb(i).G,
                                 Me._Rgb(i).B)
                    GL.glVertex3d(Me._Depth(i).X,
                                  Me._Depth(i).Y,
                                  Me._Depth(i).Z)
                Next
                GL.glEnd()
            End Using
        End If

        ' 描画が終わる度に次の描画のリクエストを発行する
        Hisui.SI.View.Invalidate()
    End Sub

    ''' <summary>
    ''' Kinectで取得したデータを点群に変換する
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetPoint() As Boolean
        Dim kinect As KinectSensor = KinectSensor.KinectSensors(0)
        Dim colorStream As ColorImageStream = kinect.ColorStream
        Dim depthStream As DepthImageStream = kinect.DepthStream

        ' RGBカメラと距離カメラのフレームデータを取得する
        Using colorFrame As ColorImageFrame = kinect.ColorStream.OpenNextFrame(100)
            Using depthFrame As DepthImageFrame = kinect.DepthStream.OpenNextFrame(100)
                If colorFrame Is Nothing OrElse depthFrame Is Nothing Then
                    Return False
                End If

                ' RGBカメラのデータを作成する
                Dim colorPixel(colorFrame.PixelDataLength - 1) As Byte
                colorFrame.CopyPixelDataTo(colorPixel)

                ReDim Me._Rgb(colorFrame.Width * colorFrame.Height - 1)
                For i As Integer = 0 To _rgb.Length - 1
                    Dim colorIndex As Integer = i * 4
                    _rgb(i) = New RGB(colorPixel(colorIndex + 2),
                                      colorPixel(colorIndex + 1),
                                      colorPixel(colorIndex + 0))
                Next

                ' 距離カメラのピクセルデータを取得する
                Dim depthPixel(depthFrame.PixelDataLength - 1) As Short
                depthFrame.CopyPixelDataTo(depthPixel)

                ' 距離カメラの座標に対応するRGBカメラの座標を取得する(座標合わせ)
                Dim colorPoint(depthFrame.PixelDataLength - 1) As ColorImagePoint
                kinect.MapDepthFrameToColorFrame(depthStream.Format,
                                                 depthPixel,
                                                 colorStream.Format,
                                                 colorPoint)

                ' 距離データを作成する
                ReDim Me._Depth(depthFrame.Width * depthFrame.Height - 1)
                For i As Integer = 0 To _depth.Length - 1
                    Dim x As Integer = Math.Min(colorPoint(i).X, colorStream.FrameWidth - 1)
                    Dim y As Integer = Math.Min(colorPoint(i).Y, colorStream.FrameHeight - 1)
                    Dim distance As Integer = depthPixel(i) >> DepthImageFrame.PlayerIndexBitmaskWidth

                    Me._Depth(i) = New Depth(x, y, distance)
                Next
            End Using
        End Using

        Return True
    End Function
End Class