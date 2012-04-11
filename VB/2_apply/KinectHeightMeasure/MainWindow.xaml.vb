Imports System
Imports System.Linq
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports Microsoft.Kinect
Imports System.Diagnostics
Imports System.Windows.Shapes
Imports System.Windows.Controls

''' <summary>
''' MainWindow.xaml の相互作用ロジック
''' </summary>
''' <remarks></remarks>
Class MainWindow
    Inherits Window

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            InitializeComponent()

            ' Kinectが接続されているかどうかを確認する
            If (KinectSensor.KinectSensors.Count = 0) Then
                Throw New Exception("Kinectを接続してください")
            End If

            ' Kinectの動作を開始する
            Call StartKinect(KinectSensor.KinectSensors(0))
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Me.Close()
        End Try
    End Sub

    ''' <summary>
    ''' Kinectの動作を開始する
    ''' </summary>
    ''' <param name="kinect"></param>
    ''' <remarks></remarks>
    Private Sub StartKinect(ByVal kinect As KinectSensor)
        ' RGBカメラ、距離カメラ、スケルトン・トラッキング(プレイヤーの認識)を有効にする
        kinect.ColorStream.Enable()
        kinect.DepthStream.Enable()
        kinect.SkeletonStream.Enable()

        ' RGBカメラ、距離カメラ、スケルトンのフレーム更新イベントを登録する
        AddHandler kinect.AllFramesReady, AddressOf Kinect_AllFramesReady

        ' Kinectの動作を開始する
        kinect.Start()
    End Sub

    ''' <summary>
    ''' Kinectの動作を停止する
    ''' </summary>
    ''' <param name="kinect"></param>
    ''' <remarks></remarks>
    Private Sub StopKinect(ByVal kinect As KinectSensor)
        If kinect IsNot Nothing Then
            If kinect.IsRunning Then
                ' フレーム更新イベントを削除する
                RemoveHandler kinect.AllFramesReady, AddressOf Kinect_AllFramesReady
                ' Kinectの停止と、ネイティブリソースを解放する
                kinect.Stop()
                kinect.Dispose()

                Me.imageRgb.Source = Nothing
            End If
        End If
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
            If kinect Is Nothing Then
                Exit Sub
            End If

            ' RGBカメラのフレームデータを取得する
            Using colorFrame As ColorImageFrame = e.OpenColorImageFrame
                If colorFrame IsNot Nothing Then
                    ' RGBカメラの画像を表示する
                    Dim colorPixel(colorFrame.PixelDataLength - 1) As Byte
                    colorFrame.CopyPixelDataTo(colorPixel)
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


            ' 距離カメラのフレームデータを取得する
            Using depthFrame As DepthImageFrame = e.OpenDepthImageFrame
                ' スケルトンのフレームを取得する
                Using _skeletonFrame As SkeletonFrame = e.OpenSkeletonFrame
                    If depthFrame IsNot Nothing AndAlso _skeletonFrame IsNot Nothing Then
                        ' 身長を表示する
                        Call HeightMeasure(kinect, depthFrame, _skeletonFrame)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 距離データをカラー画像に変換する
    ''' </summary>
    ''' <param name="kinect"></param>
    ''' <param name="depthFrame"></param>
    ''' <param name="_skeletonFrame"></param>
    ''' <remarks></remarks>
    Private Sub HeightMeasure(kinect As KinectSensor,
                              depthFrame As DepthImageFrame,
                              _skeletonFrame As SkeletonFrame)

        Dim colorStream As ColorImageStream = kinect.ColorStream
        Dim depthStream As DepthImageStream = kinect.DepthStream

        ' トラッキングされている最初のスケルトンを取得する
        ' インデックスはプレイヤーIDに対応しているのでとっておく
        Dim skeletons(_skeletonFrame.SkeletonArrayLength - 1) As Skeleton
        _skeletonFrame.CopySkeletonDataTo(skeletons)

        Dim playerIndex As Integer = 0
        Do While (playerIndex < skeletons.Length)
            If skeletons(playerIndex).TrackingState = SkeletonTrackingState.Tracked Then
                Exit Do
            End If
            playerIndex += 1
        Loop

        If playerIndex = skeletons.Length Then
            Exit Sub
        End If

        ' トラッキングされている最初のスケルトン
        Dim _skeleton As Skeleton = skeletons(playerIndex)

        ' 実際のプレイヤーIDは、スケルトンのインデックス+1
        playerIndex += 1

        ' 頭、足先がトラッキングされていない場合は、そのままRGBカメラのデータを返す
        Dim head As Joint = _skeleton.Joints(JointType.Head)
        Dim leftFoot As Joint = _skeleton.Joints(JointType.FootLeft)
        Dim rightFoot As Joint = _skeleton.Joints(JointType.FootRight)
        If (head.TrackingState <> JointTrackingState.Tracked) OrElse
            (leftFoot.TrackingState <> JointTrackingState.Tracked) OrElse
            (rightFoot.TrackingState <> JointTrackingState.Tracked) Then
            Exit Sub
        End If

        ' 距離カメラのピクセルごとのデータを取得する
        Dim depthPixel(depthFrame.PixelDataLength - 1) As Short
        depthFrame.CopyPixelDataTo(depthPixel)

        ' 距離カメラの座標に対応するRGBカメラの座標を取得する(座標合わせ)
        Dim colorPoint(depthFrame.PixelDataLength - 1) As ColorImagePoint
        kinect.MapDepthFrameToColorFrame(depthStream.Format,
                                         depthPixel,
                                         colorStream.Format,
                                         colorPoint)

        ' 頭のてっぺんを探す
        Dim headDepth As DepthImagePoint = depthFrame.MapFromSkeletonPoint(head.Position)
        Dim top As Integer = 0
        For i As Integer = 0 To headDepth.Y - i - 1
            ' 一つ上のY座標を取得し、プレイヤーがいなければ、現在の座標が最上位
            Dim index As Integer = ((headDepth.Y - i) * depthFrame.Width) + headDepth.X
            Dim player As Integer = depthPixel(index) And DepthImageFrame.PlayerIndexBitmask
            If player = playerIndex Then
                top = i
            End If
        Next

        ' 頭のてっぺんを3次元座標に戻し、足の座標(下にあるほう)を取得する
        ' この差分で身長を測る
        head.Position = depthFrame.MapToSkeletonPoint(headDepth.X, headDepth.Y - top)
        Dim foot As Joint = IIf(leftFoot.Position.Y < rightFoot.Position.Y, leftFoot, rightFoot)

        ' 背丈を表示する
        Call DrawMeasure(kinect, colorStream, head, foot)
    End Sub

    ''' <summary>
    ''' 背丈を表示する
    ''' </summary>
    ''' <param name="kinect"></param>
    ''' <param name="colorStream"></param>
    ''' <param name="head"></param>
    ''' <param name="foot"></param>
    ''' <remarks></remarks>
    Private Sub DrawMeasure(kinect As KinectSensor,
                            colorStream As ColorImageStream,
                            head As Joint,
                            foot As Joint)

        ' 頭と足の座標の差分を身長とする(メートルからセンチメートルに変換する)
        Dim height As Integer = CType((Math.Abs(head.Position.Y - foot.Position.Y) * 100), Integer)

        ' 頭と足のスケルトン座標を、RGBカメラの座標に変換する
        Dim headColor As ColorImagePoint = kinect.MapSkeletonPointToColor(head.Position,
                                                                          colorStream.Format)
        Dim footColor As ColorImagePoint = kinect.MapSkeletonPointToColor(foot.Position,
                                                                          colorStream.Format)

        ' RGBカメラの座標を、表示する画面の座標に変換する
        Dim headScalePoint As New Point(
            ScaleTo(headColor.X,
                    colorStream.FrameWidth,
                    canvasMeasure.Width),
            ScaleTo(headColor.Y,
                    colorStream.FrameHeight,
                    canvasMeasure.Height)
            )

        Dim footScalePoint As New Point(
            ScaleTo(footColor.X,
                    colorStream.FrameWidth,
                    canvasMeasure.Width),
            ScaleTo(footColor.Y,
                    colorStream.FrameHeight,
                    canvasMeasure.Height)
          )

        Const lineLength As Integer = 50
        Const thickness As Integer = 10
        canvasMeasure.Children.Clear()
        ' 頭の位置
        canvasMeasure.Children.Add(New Line With {
                                 .Stroke = New SolidColorBrush(Colors.Red),
                                 .X1 = headScalePoint.X,
                                 .Y1 = headScalePoint.Y,
                                 .X2 = headScalePoint.X + lineLength,
                                 .Y2 = headScalePoint.Y,
                                 .StrokeThickness = thickness
                             })

        ' 足の位置
        canvasMeasure.Children.Add(New Line With {
                                   .Stroke = New SolidColorBrush(Colors.Red),
                                   .X1 = footScalePoint.X,
                                   .Y1 = footScalePoint.Y,
                                   .X2 = headScalePoint.X + lineLength,
                                   .Y2 = footScalePoint.Y,
                                   .StrokeThickness = thickness
                               })

        ' 頭と足を結ぶ線
        canvasMeasure.Children.Add(New Line With {
                                   .Stroke = New SolidColorBrush(Colors.Red),
                                   .X1 = headScalePoint.X + lineLength,
                                   .Y1 = headScalePoint.Y,
                                   .X2 = headScalePoint.X + lineLength,
                                   .Y2 = footScalePoint.Y,
                                   .StrokeThickness = thickness
                               })

        ' 身長の表示Y位置
        Dim Y As Double = Math.Abs(headScalePoint.Y + footScalePoint.Y) / 2
        canvasMeasure.Children.Add(New TextBlock With {
                                   .Margin = New Thickness(headScalePoint.X + lineLength, Y, 0, 0),
                                   .Text = height.ToString(),
                                   .Height = 36,
                                   .Width = 60,
                                   .FontSize = 24,
                                   .FontWeight = FontWeights.Bold,
                                   .Background = New SolidColorBrush(Colors.White)
                               })
    End Sub

    ''' <summary>
    ''' スケールを変換する
    ''' </summary>
    ''' <param name="value"></param>
    ''' <param name="source"></param>
    ''' <param name="dest"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ScaleTo(value As Double, source As Double, dest As Double) As Double
        Return (value * dest) / source
    End Function

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