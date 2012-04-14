Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.Threading
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports Microsoft.Kinect
Imports Geis.Win32
Imports System.Windows.Shapes

Partial Public Class MainWindow
    Inherits Window

    Friend WithEvents KinectCollection As KinectSensorCollection
    Friend WithEvents KinectEvents As KinectSensor
    Friend WithEvents AudioEvents As KinectAudioSource
    Private ReadOnly Bgr32BytesPerPixel As Integer = PixelFormats.Bgr32.BitsPerPixel / 8
    Private IsContinue As Boolean = True

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            InitializeComponent()

            ' Kinectが1台以上接続されていれば、最初から開始する
            If KinectSensor.KinectSensors.Count >= 1 AndAlso
                 KinectSensor.KinectSensors(0).Status = KinectStatus.Connected Then
                Call StartKinect(KinectSensor.KinectSensors(0))
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Me.Close()
        End Try
    End Sub

    ''' <summary>
    ''' Kinectの動作を開始する
    ''' </summary>
    ''' <param name="sensor"></param>
    ''' <remarks></remarks>
    Private Sub StartKinect(ByVal sensor As KinectSensor)
        Me.KinectCollection = KinectSensor.KinectSensors
        Me.KinectEvents = sensor
        Me.AudioEvents = sensor.AudioSource

        ' RGBカメラを有効にする
        Me.KinectEvents.ColorStream.Enable()

        ' 距離カメラを有効にする
        Me.KinectEvents.DepthStream.Enable()

        ' スケルトンを有効にする
        Me.KinectEvents.SkeletonStream.Enable()

        ' Kinectの動作を開始する
        Me.KinectEvents.Start()

        ' 音声入出力スレッド
        Dim _thread As Thread = New Thread(
                                New ThreadStart(Sub()
                                                    ' ストリーミングプレイヤー
                                                    Dim player = New StreamingWavePlayer(16000, 16, 1, 100)
                                                    ' 音声入力用のバッファ
                                                    Dim buffer(1024 - 1) As Byte
                                                    ' エコーのキャンセルと抑制を有効にする
                                                    Me.AudioEvents.EchoCancellationMode = EchoCancellationMode.CancellationAndSuppression
                                                    ' 音声の入力を開始する
                                                    Using _stream As Stream = Me.AudioEvents.Start()
                                                        Do While (IsContinue)
                                                            ' 音声を入力し、スピーカーに出力する
                                                            _stream.Read(buffer, 0, buffer.Length)
                                                            player.Output(buffer)
                                                        Loop
                                                    End Using
                                                End Sub))

        ' スレッドの動作を開始する
        _thread.Start()

        ' defaultモードとnearモードの切り替え
        Me.ComboBoxRange.Items.Clear()
        For Each range In [Enum].GetValues(GetType(DepthRange))
            Me.ComboBoxRange.Items.Add(range.ToString)
        Next
        Me.ComboBoxRange.SelectedIndex = 0

        ' チルトモーターを動作させるスライダーを設定する
        sliderTiltAngle.Maximum = KinectEvents.MaxElevationAngle
        sliderTiltAngle.Minimum = KinectEvents.MinElevationAngle
        sliderTiltAngle.Value = KinectEvents.ElevationAngle
    End Sub

    ''' <summary>
    ''' Kinectの動作を停止する
    ''' </summary>
    ''' <param name="sensor"></param>
    ''' <remarks></remarks>
    Private Sub StopKinect(ByVal sensor As KinectSensor)
        If Me.KinectEvents IsNot Nothing Then
            If Me.KinectEvents.IsRunning Then
                ' 音声のスレッドを停止する
                Me.IsContinue = False
                Me.AudioEvents.Stop()

                ' Kinectの停止と、ネイティブリソースを解放する
                Me.KinectEvents.Stop()
                Me.KinectEvents.Dispose()

                Me.ImageDepth.Source = Nothing
                Me.ImageRgb.Source = Nothing
            End If
        End If
    End Sub

    ''' <summary>
    ''' Kinectの接続状態が変わった時に呼び出される
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KinectSensors_StatusChanged(sender As Object,
                                            e As StatusChangedEventArgs) _
                                        Handles KinectCollection.StatusChanged
        If e.Status = KinectStatus.Connected Then
            ' デバイスが接続された
            Call StartKinect(e.Sensor)
        ElseIf e.Status = KinectStatus.Disconnected Then
            ' デバイスが切断された
            Call StopKinect(e.Sensor)
        ElseIf e.Status = KinectStatus.NotPowered Then
            ' ACが抜けてる
            Call StopKinect(e.Sensor)
            MessageBox.Show("電源ケーブルを接続してください")
        ElseIf e.Status = KinectStatus.DeviceNotSupported Then
            ' Kinect for Xbox 360
            MessageBox.Show("Kinect for Xbox 360 はサポートされません")
        ElseIf e.Status = KinectStatus.InsufficientBandwidth Then
            ' USBの帯域が足りない
            MessageBox.Show("USBの帯域が足りません")
        End If
    End Sub

    ''' <summary>
    ''' 音源方向が変化した
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub AudioSource_SoundSourceAngleChanged(sender As Object,
                                                    e As SoundSourceAngleChangedEventArgs) _
                                                Handles AudioEvents.SoundSourceAngleChanged
        soundSource.Angle = -e.Angle
    End Sub

    ''' <summary>
    ''' ビーム方向が変化した
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub AudioSource_BeamAngleChanged(sender As Object,
                                             e As BeamAngleChangedEventArgs) _
                                         Handles AudioEvents.BeamAngleChanged
        beam.Angle = -e.Angle
    End Sub

    ''' <summary>
    ''' RGBカメラのフレーム更新イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub kinect_ColorFrameReady(sender As Object,
                                       e As ColorImageFrameReadyEventArgs) _
                                   Handles KinectEvents.ColorFrameReady
        Try
            ' RGBカメラのフレームデータを取得する
            Using colorFrame As ColorImageFrame = e.OpenColorImageFrame
                If colorFrame IsNot Nothing Then
                    ' RGBカメラのピクセルデータを取得する
                    Dim colorPixel(colorFrame.PixelDataLength - 1) As Byte
                    colorFrame.CopyPixelDataTo(colorPixel)

                    ' ピクセルデータをビットマップに変換する
                    Me.ImageRgb.Source = BitmapSource.Create(colorFrame.Width,
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
                                       e As DepthImageFrameReadyEventArgs) _
                                   Handles KinectEvents.DepthFrameReady
        Try
            ' センサーのインスタンスを取得する
            Dim sensor As KinectSensor = CType(sender, KinectSensor)
            If sensor Is Nothing Then
                Exit Sub
            End If

            ' 距離カメラのフレームデータを取得する
            Using depthFrame As DepthImageFrame = e.OpenDepthImageFrame
                If depthFrame IsNot Nothing Then
                    ' 距離データを画像化して表示
                    Me.ImageDepth.Source = BitmapSource.Create(depthFrame.Width,
                                                               depthFrame.Height,
                                                               96,
                                                               96,
                                                               PixelFormats.Bgr32,
                                                               Nothing,
                                                               ConvertDepthColor(sensor, depthFrame),
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
                                          e As SkeletonFrameReadyEventArgs) _
                                      Handles KinectEvents.SkeletonFrameReady
        Try
            ' センサーのインスタンスを取得する
            Dim sensor As KinectSensor = CType(sender, KinectSensor)
            If sensor Is Nothing Then
                Exit Sub
            End If

            ' スケルトンのフレームを取得する
            Using _skeletonFrame As SkeletonFrame = e.OpenSkeletonFrame
                If _skeletonFrame IsNot Nothing Then
                    Call DrawSkeleton(sensor, _skeletonFrame)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' スケルトンを描画する
    ''' </summary>
    ''' <param name="sensor"></param>
    ''' <param name="_skeletonFrame"></param>
    ''' <remarks></remarks>
    Private Sub DrawSkeleton(sensor As KinectSensor,
                             _skeletonFrame As SkeletonFrame)
        ' スケルトンのデータを取得する
        Dim skeletons(_skeletonFrame.SkeletonArrayLength - 1) As Skeleton
        _skeletonFrame.CopySkeletonDataTo(skeletons)

        Me.CanvasSkeleton.Children.Clear()

        ' トラッキングされているスケルトンのジョイントを描画する
        For Each _skeleton As Skeleton In skeletons
            If _skeleton.TrackingState = SkeletonTrackingState.Tracked Then
                ' ジョイントを描画する
                For Each _joint As Joint In _skeleton.Joints
                    ' ジョイントがトラッキングされていなければ次へ
                    If _joint.TrackingState = JointTrackingState.NotTracked Then
                        Continue For
                    End If

                    ' ジョイントの座標を描く
                    Call DrawEllipse(sensor, _joint.Position)
                Next
                ' スケルトンが位置追跡(ニアモードの)の場合は、スケルトン位置(Center hip)を描画する
            ElseIf _skeleton.TrackingState = SkeletonTrackingState.PositionOnly Then
                ' スケルトンの座標を描く
                Call DrawEllipse(sensor, _skeleton.Position)
            End If
        Next
    End Sub

    ''' <summary>
    ''' ジョイントの円を描く
    ''' </summary>
    ''' <param name="sensor"></param>
    ''' <param name="position"></param>
    ''' <remarks></remarks>
    Private Sub DrawEllipse(sensor As KinectSensor,
                            position As SkeletonPoint)
        Const R As Integer = 5

        ' スケルトンの座標を、RGBカメラの座標に変換する
        Dim _point As ColorImagePoint = sensor.MapSkeletonPointToColor(position, sensor.ColorStream.Format)

        ' 座標を画面のサイズに変換する
        _point.X = CType(ScaleTo(_point.X, sensor.ColorStream.FrameWidth, CanvasSkeleton.Width), Integer)
        _point.Y = CType(ScaleTo(_point.Y, sensor.ColorStream.FrameHeight, CanvasSkeleton.Height), Integer)

        ' 円を描く
        CanvasSkeleton.Children.Add(New Ellipse() With {
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
    ''' <param name="sensor"></param>
    ''' <param name="depthFrame"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ConvertDepthColor(sensor As KinectSensor,
                                       depthFrame As DepthImageFrame) As Byte()
        Dim colorStream As ColorImageStream = sensor.ColorStream
        Dim depthStream As DepthImageStream = sensor.DepthStream

        ' 距離カメラのピクセルごとのデータを取得する
        Dim depthPixel(depthFrame.PixelDataLength - 1) As Short
        depthFrame.CopyPixelDataTo(depthPixel)

        ' 距離カメラの座標に対応するRGBカメラの座標を取得する(座標合わせ)
        Dim colorPoint(depthFrame.PixelDataLength - 1) As ColorImagePoint
        sensor.MapDepthFrameToColorFrame(depthStream.Format,
                                         depthPixel,
                                         colorStream.Format,
                                         colorPoint)

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
            KinectSensor.KinectSensors(0).DepthStream.Range = CType(Me.ComboBoxRange.SelectedIndex, DepthRange)
        Catch ex As Exception
            Me.ComboBoxRange.SelectedIndex = 0
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

    ''' <summary>
    ''' スライダーの位置が変更された
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub sliderTiltAngle_ValueChanged(sender As System.Object,
                                             e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double))
        Try
            KinectSensor.KinectSensors(0).ElevationAngle = CType(e.NewValue, Integer)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub
End Class
