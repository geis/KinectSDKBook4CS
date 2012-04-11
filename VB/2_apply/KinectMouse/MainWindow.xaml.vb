Imports System
Imports System.Linq
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports Microsoft.Kinect
Imports Geis.Win32
Imports System.Collections.Generic

''' <summary>
''' MainWindow.xaml の相互作用ロジック
''' </summary>
''' <remarks></remarks>
Class MainWindow
    Inherits Window
    Private Kinect As KinectSensor

    Private ReadOnly Bgr32BytesPerPixel As Integer = PixelFormats.Bgr32.BitsPerPixel / 8

    Private Const R As Integer = 5        ' 手の位置を描画する円の大きさ

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            InitializeComponent()

            If KinectSensor.KinectSensors.Count = 0 Then
                Throw New Exception("Kinectが接続されていません")
            End If

            Call StartKinect(KinectSensor.KinectSensors(0))
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Me.Close()
        End Try
    End Sub

    ''' <summary>
    ''' RGBカメラ、距離カメラ、骨格のフレーム更新イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Kinect_AllFramesReady(sender As Object,
                                      e As Microsoft.Kinect.AllFramesReadyEventArgs)
        Try
            ' センサーのインスタンスを取得する
            Dim kinect As KinectSensor = CType(sender, KinectSensor)

            ' RGBカメラのフレームデータを取得する
            Using colorFrame As ColorImageFrame = e.OpenColorImageFrame
                If colorFrame IsNot Nothing Then
                    ' RGBカメラのピクセルデータを取得する
                    Dim colorPixel(colorFrame.PixelDataLength - 1) As Byte
                    colorFrame.CopyPixelDataTo(colorPixel)

                    ' ピクセルデータをビットマップに変換する
                    Me.imageRgb.Source = BitmapSource.Create(colorFrame.Width,
                                                             colorFrame.Height,
                                                             96,
                                                             96,
                                                             PixelFormats.Bgr32,
                                                             Nothing,
                                                             colorPixel,
                                                             colorFrame.Width * colorFrame.BytesPerPixel)
                End If
            End Using

            Using _skeletonFrame As SkeletonFrame = e.OpenSkeletonFrame
                If _skeletonFrame IsNot Nothing Then
                    ' トラッキングされているスケルトンのジョイントを描画する
                    Dim _skeleton As Skeleton = _skeletonFrame.GetFirstTrackedSkeleton()

                    If (_skeleton IsNot Nothing) AndAlso (_skeleton.TrackingState = SkeletonTrackingState.Tracked) Then
                        Dim hand As Joint = _skeleton.Joints(JointType.HandRight)
                        If hand.TrackingState = JointTrackingState.Tracked Then
                            Dim source As ImageSource = imageRgb.Source
                            Dim drawingVisual As DrawingVisual = New DrawingVisual

                            Using _drawingContext As DrawingContext = drawingVisual.RenderOpen
                                'バイト列をビットマップに展開
                                '描画可能なビットマップを作る
                                _drawingContext.DrawImage(imageRgb.Source,
                                                          New Rect(0,
                                                                   0,
                                                                   source.Width,
                                                                   source.Height))

                                ' 手の位置に円を描画
                                Call DrawSkeletonPoint(_drawingContext, hand)
                            End Using

                            ' 描画可能なビットマップを作る
                            ' http://stackoverflow.com/questions/831860/generate-bitmapsource-from-uielement
                            Dim bitmap As New RenderTargetBitmap(CType(source.Width, Integer),
                                                                 CType(source.Height, Integer),
                                                                 96,
                                                                 96,
                                                                 PixelFormats.Default)
                            bitmap.Render(drawingVisual)

                            Me.imageRgb.Source = bitmap

                            ' Frame中の手の位置をディスプレイの位置に対応付ける
                            ' スケルトン座標 → RGB画像座標
                            Dim _point As ColorImagePoint = kinect.MapSkeletonPointToColor(hand.Position,
                                                                                           kinect.ColorStream.Format)
                            Dim _screen As System.Windows.Forms.Screen = System.Windows.Forms.Screen.AllScreens(0) ' メインディスプレイの情報を取得
                            _point.X = (_point.X * _screen.Bounds.Width) / kinect.ColorStream.FrameWidth
                            _point.Y = (_point.Y * _screen.Bounds.Height) / kinect.ColorStream.FrameHeight

                            ' マウスカーソルの移動
                            SendInput.MouseMove(_point.X, _point.Y, _screen)

                            ' クリック動作
                            If Me.IsClicked(_skeletonFrame, _point) Then
                                SendInput.LeftClick()
                            End If
                        End If
                    End If
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Kinectの動作を開始する
    ''' </summary>
    ''' <param name="kin"></param>
    ''' <remarks></remarks>
    Private Sub StartKinect(ByVal kin As KinectSensor)
        Try
            Me.Kinect = kin

            Me.Kinect.ColorStream.Enable()
            Me.Kinect.SkeletonStream.Enable(New TransformSmoothParameters With
                                             {
                                                 .Smoothing = 0.7F,
                                                 .Correction = 0.3F,
                                                 .Prediction = 0.4F,
                                                 .JitterRadius = 0.1F,
                                                 .MaxDeviationRadius = 0.5F
                                             })

            AddHandler Kinect.AllFramesReady, AddressOf Kinect_AllFramesReady

            ' Kinectの動作を開始する
            Kinect.Start()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Close()
        End Try
    End Sub

    ''' <summary>
    ''' Kinectの動作を停止する
    ''' </summary>
    ''' <param name="kinect"></param>
    ''' <remarks></remarks>
    Private Sub StopKinect(ByVal kinect As KinectSensor)
        If kinect IsNot Nothing Then
            If kinect.IsRunning Then
                RemoveHandler kinect.AllFramesReady, AddressOf Kinect_AllFramesReady

                kinect.Stop()
                kinect.Dispose()
            End If
        End If
    End Sub

    ''' <summary>
    ''' クリック動作を行ったのかチェック
    ''' </summary>
    ''' <param name="_skeletonFrame"></param>
    ''' <param name="_point"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function IsClicked(_skeletonFrame As SkeletonFrame, _point As ColorImagePoint) As Boolean
        Return IsSteady(_skeletonFrame, _point)
    End Function

    ' <summary>
    ' 座標管理用構造体
    ' </summary>
    Private Structure FramePoint
        Public Point As ColorImagePoint
        Public TimeStamp As Long
    End Structure

    Private milliseconds As Integer = 2000        ' 認識するまでの停止時間の設定
    Private threshold As Integer = 10             ' 座標の変化量の閾値

    ' 基点となるポイント。
    ' この座標からあまり動かない場合 Steady状態であると認識する。
    Private basePoint As FramePoint = New FramePoint()

    ''' <summary>
    ''' 停止状態にあるかチェックする
    ''' </summary>
    ''' <param name="_skeletonFrame"></param>
    ''' <param name="_point"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function IsSteady(_skeletonFrame As SkeletonFrame, _point As ColorImagePoint) As Boolean
        Dim currentPoint = New FramePoint With {.Point = _point,
                                                .TimeStamp = _skeletonFrame.Timestamp}

        ' milliseconds時間経過したら steady
        If (currentPoint.TimeStamp - basePoint.TimeStamp) > milliseconds Then
            basePoint = currentPoint
            Return True
        End If
        ' 座標の変化量がthreshold以上ならば、basePointを更新して初めから計測
        If Math.Abs(currentPoint.Point.X - basePoint.Point.X) > threshold OrElse
            Math.Abs(currentPoint.Point.Y - basePoint.Point.Y) > threshold Then

            ' 座標が動いたので基点を動いた位置にずらして、最初から計測
            basePoint = currentPoint
        End If

        Return False
    End Function

    ''' <summary>
    ''' jointの座標に円を描く
    ''' </summary>
    ''' <param name="_drawingContext"></param>
    ''' <param name="_joint"></param>
    ''' <remarks></remarks>
    Private Sub DrawSkeletonPoint(_drawingContext As DrawingContext, _joint As Joint)
        If _joint.TrackingState <> JointTrackingState.Tracked Then
            Exit Sub
        End If

        ' 円を書く
        Dim _point As ColorImagePoint = Me.Kinect.MapSkeletonPointToColor(_joint.Position,
                                                                          Me.Kinect.ColorStream.Format)
        _drawingContext.DrawEllipse(New SolidColorBrush(Colors.Red),
                                    New Pen(Brushes.Red, 1),
                                    New Point(_point.X,
                                              _point.Y),
                                          R,
                                          R)
    End Sub

    ''' <summary>
    ''' Windowsが閉じられるときのイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Window_Closing(sender As System.Object,
                               e As System.ComponentModel.CancelEventArgs)
        Call StopKinect(KinectSensor.KinectSensors(0))
    End Sub

End Class

''' <summary>
''' 初めにトラッキングしたスケルトンを取得する拡張メソッド
''' </summary>
''' <remarks></remarks>
Module SkeletonExtensions
    <Runtime.CompilerServices.Extension()>
    Public Function GetFirstTrackedSkeleton(_skeletonFrame As SkeletonFrame) As Skeleton
        Dim _skeleton(_skeletonFrame.SkeletonArrayLength - 1) As Skeleton
        _skeletonFrame.CopySkeletonDataTo(_skeleton)
        Return (From s In _skeleton
                Where s.TrackingState = SkeletonTrackingState.Tracked
                Select s).FirstOrDefault()
    End Function
End Module
