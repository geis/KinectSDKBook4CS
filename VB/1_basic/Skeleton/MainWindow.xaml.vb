Imports System
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports Microsoft.Kinect
Imports System.Windows.Shapes

Partial Public Class MainWindow
    Inherits Window

    Private ReadOnly Bgr32BytesPerPixel As Integer = PixelFormats.Bgr32.BitsPerPixel / 8
    Private IsContinue As Boolean = True

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
        ' RGBカメラを有効にして、フレーム更新イベントを登録する
        kinect.ColorStream.Enable()
        AddHandler kinect.ColorFrameReady, AddressOf kinect_ColorFrameReady

        ' 距離カメラを有効にして、フレーム更新イベントを登録する
        kinect.DepthStream.Enable()
        AddHandler kinect.DepthFrameReady, AddressOf kinect_DepthFrameReady

        ' スケルトンを有効にして、フレーム更新イベントを登録する
        kinect.SkeletonStream.Enable()
        AddHandler kinect.SkeletonFrameReady, AddressOf kinect_SkeletonFrameReady

        ' Kinectの動作を開始する
        kinect.Start()

        ' defaultモードとnearモードの切り替え
        Me.comboBoxRange.Items.Clear()
        For Each range In [Enum].GetValues(GetType(DepthRange))
            Me.comboBoxRange.Items.Add(range.ToString)
        Next
        Me.comboBoxRange.SelectedIndex = 0
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
                RemoveHandler kinect.ColorFrameReady, AddressOf kinect_ColorFrameReady
                RemoveHandler kinect.DepthFrameReady, AddressOf kinect_DepthFrameReady
                RemoveHandler kinect.SkeletonFrameReady, AddressOf kinect_SkeletonFrameReady

                ' Kinectの停止と、ネイティブリソースを解放する
                kinect.Stop()
                kinect.Dispose()

                Me.imageDepth.Source = Nothing
                Me.imageRgb.Source = Nothing
            End If
        End If
    End Sub

    ''' <summary>
    ''' RGBカメラのフレーム更新イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub kinect_ColorFrameReady(sender As Object,
                                       e As ColorImageFrameReadyEventArgs)
        Try
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
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 距離カメラのフレーム更新イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub kinect_DepthFrameReady(sender As Object,
                                       e As DepthImageFrameReadyEventArgs)
        Try
            ' センサーのインスタンスを取得する
            Dim kinect As KinectSensor = CType(sender, KinectSensor)
            If kinect Is Nothing Then
                Exit Sub
            End If

            ' 距離カメラのフレームデータを取得する
            Using depthFrame As DepthImageFrame = e.OpenDepthImageFrame
                If depthFrame IsNot Nothing Then
                    ' 距離データを画像化して表示
                    Me.imageDepth.Source = BitmapSource.Create(depthFrame.Width,
                                                               depthFrame.Height,
                                                               96,
                                                               96,
                                                               PixelFormats.Bgr32,
                                                               Nothing,
                                                               ConvertDepthColor(kinect, depthFrame),
                                                               depthFrame.Width * Bgr32BytesPerPixel)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' スケルトンのフレーム更新イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub kinect_SkeletonFrameReady(sender As Object,
                                          e As SkeletonFrameReadyEventArgs)
        Try
            ' センサーのインスタンスを取得する
            Dim kinect As KinectSensor = CType(sender, KinectSensor)
            If kinect Is Nothing Then
                Exit Sub
            End If

            ' スケルトンのフレームを取得する
            Using skeletonFrame As SkeletonFrame = e.OpenSkeletonFrame
                If skeletonFrame IsNot Nothing Then
                    Call DrawSkeleton(kinect, skeletonFrame)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' スケルトンを描画する
    ''' </summary>
    ''' <param name="kinect"></param>
    ''' <param name="skeletonFrame"></param>
    ''' <remarks></remarks>
    Private Sub DrawSkeleton(kinect As KinectSensor, skeletonFrame As SkeletonFrame)
        ' スケルトンのデータを取得する
        Dim skeletons(skeletonFrame.SkeletonArrayLength - 1) As Microsoft.Kinect.Skeleton
        skeletonFrame.CopySkeletonDataTo(skeletons)

        canvasSkeleton.Children.Clear()

        ' トラッキングされているスケルトンのジョイントを描画する
        For Each _skeleton As Microsoft.Kinect.Skeleton In skeletons
            If _skeleton.TrackingState = SkeletonTrackingState.Tracked Then
                ' ジョイントを描画する
                For Each _joint As Joint In _skeleton.Joints
                    ' ジョイントがトラッキングされていなければ次へ
                    If _joint.TrackingState = JointTrackingState.NotTracked Then
                        Continue For
                    End If

                    ' ジョイントの座標を描く
                    Call DrawEllipse(kinect, _joint.Position)
                Next
                ' スケルトンが位置追跡(ニアモードの)の場合は、スケルトン位置(Center hip)を描画する
            ElseIf _skeleton.TrackingState = SkeletonTrackingState.PositionOnly Then
                ' スケルトンの座標を描く
                Call DrawEllipse(kinect, _skeleton.Position)
            End If
        Next
    End Sub

    ''' <summary>
    ''' ジョイントの円を描く
    ''' </summary>
    ''' <param name="kinect"></param>
    ''' <param name="position"></param>
    ''' <remarks></remarks>
    Private Sub DrawEllipse(kinect As KinectSensor, position As SkeletonPoint)
        Const R As Integer = 5

        ' スケルトンの座標を、RGBカメラの座標に変換する
        Dim _point As ColorImagePoint = kinect.MapSkeletonPointToColor(position, kinect.ColorStream.Format)

        ' 座標を画面のサイズに変換する
        _point.X = CType(ScaleTo(_point.X, kinect.ColorStream.FrameWidth, canvasSkeleton.Width), Integer)
        _point.Y = CType(ScaleTo(_point.Y, kinect.ColorStream.FrameHeight, canvasSkeleton.Height), Integer)

        ' 円を描く
        canvasSkeleton.Children.Add(New Ellipse() With {
                                    .Fill = New SolidColorBrush(Colors.Red),
                                    .Margin = New Thickness(_point.X - R, _point.Y - R, 0, 0),
                                    .Width = R * 2,
                                    .Height = R * 2})
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
    ''' 距離データをカラー画像に変換する
    ''' </summary>
    ''' <param name="kinect"></param>
    ''' <param name="depthFrame"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ConvertDepthColor(kinect As KinectSensor, depthFrame As DepthImageFrame) As Byte()
        Dim colorStream As ColorImageStream = kinect.ColorStream
        Dim depthStream As DepthImageStream = kinect.DepthStream

        ' 距離カメラのピクセルごとのデータを取得する
        Dim depthPixel(depthFrame.PixelDataLength - 1) As Short
        depthFrame.CopyPixelDataTo(depthPixel)

        ' 距離カメラの座標に対応するRGBカメラの座標を取得する(座標合わせ)
        Dim colorPoint(depthFrame.PixelDataLength - 1) As ColorImagePoint
        kinect.MapDepthFrameToColorFrame(depthStream.Format, depthPixel,
                                         colorStream.Format, colorPoint)

        Dim depthColor(depthFrame.PixelDataLength * Bgr32BytesPerPixel - 1) As Byte
        For index As Integer = 0 To depthPixel.Length - 1
            ' 距離カメラのデータから、プレイヤーIDと距離を取得する
            Dim player As Integer = depthPixel(index) And DepthImageFrame.PlayerIndexBitmask
            Dim distance As Integer = depthPixel(index) >> DepthImageFrame.PlayerIndexBitmaskWidth

            ' 変換した結果が、フレームサイズを超えることがあるため、小さいほうを使う
            Dim x As Integer = Math.Min(colorPoint(index).X, colorStream.FrameWidth - 1)
            Dim y As Integer = Math.Min(colorPoint(index).Y, colorStream.FrameHeight - 1)
            Dim colorIndex As Integer = ((y * depthFrame.Width) + x) * Bgr32BytesPerPixel

            If player <> 0 Then
                ' プレイヤーがいるピクセルの場合
                depthColor(colorIndex) = 255
                depthColor(colorIndex + 1) = 255
                depthColor(colorIndex + 2) = 255
            Else
                ' プレイヤーではないピクセルの場合
                If distance = depthStream.UnknownDepth Then
                    ' サポート外 0-40cm
                    depthColor(colorIndex) = 0
                    depthColor(colorIndex + 1) = 0
                    depthColor(colorIndex + 2) = 255
                ElseIf distance = depthStream.TooNearDepth Then
                    ' 近すぎ 40cm-80cm(default mode)
                    depthColor(colorIndex) = 0
                    depthColor(colorIndex + 1) = 255
                    depthColor(colorIndex + 2) = 0
                ElseIf distance = depthStream.TooFarDepth Then
                    ' 遠すぎ 3m(Near),4m(Default)-8m
                    depthColor(colorIndex) = 255
                    depthColor(colorIndex + 1) = 0
                    depthColor(colorIndex + 2) = 0
                Else
                    ' 有効な距離データ
                    depthColor(colorIndex) = 0
                    depthColor(colorIndex + 1) = 255
                    depthColor(colorIndex + 2) = 255
                End If
            End If
        Next

        Return depthColor
    End Function

    ''' <summary>
    ''' 距離カメラの通常/近接モード変更イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub comboBoxRange_SelectionChanged(sender As System.Object,
                                               e As System.Windows.Controls.SelectionChangedEventArgs)
        Try
            KinectSensor.KinectSensors(0).DepthStream.Range = CType(comboBoxRange.SelectedIndex, DepthRange)
        Catch ex As Exception
            comboBoxRange.SelectedIndex = 0
        End Try
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
