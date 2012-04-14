Imports System
Imports System.Diagnostics
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports Microsoft.Kinect

''' <summary>
''' KinectWindow.xaml の相互作用ロジック
''' </summary>
''' <remarks></remarks>
Partial Public Class KinectWindow
    Inherits UserControl

    Friend WithEvents KinectEvents As KinectSensor
    Friend WithEvents AudioEvents As KinectAudioSource
    Private ReadOnly Bgr32BytesPerPixel As Integer = PixelFormats.Bgr32.BitsPerPixel / 8

    Public Sub New()
        Try
            InitializeComponent()

        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Kinectの動作を開始する
    ''' </summary>
    ''' <param name="sensor"></param>
    ''' <remarks></remarks>
    Public Sub StartKinect(ByVal sensor As KinectSensor)
        Me.KinectEvents = sensor
        Me.AudioEvents = sensor.AudioSource

        ' RGBカメラ、距離カメラ、スケルトン・トラッキング(プレイヤーの認識)を有効にする
        Me.KinectEvents.ColorStream.Enable()
        Me.KinectEvents.DepthStream.Enable()
        Me.KinectEvents.SkeletonStream.Enable()

        ' Kinectの動作を開始する
        Me.KinectEvents.Start()

        ' defaultモードとnearモードの切り替え
        Me.ComboBoxRange.Items.Clear()
        For Each range In [Enum].GetValues(GetType(DepthRange))
            Me.ComboBoxRange.Items.Add(range.ToString)
        Next
        Me.ComboBoxRange.SelectedIndex = 0

        ' チルトモーターを動作させるスライダーを設定する
        Me.SliderTiltAngle.Maximum = Me.KinectEvents.MaxElevationAngle
        Me.SliderTiltAngle.Minimum = Me.KinectEvents.MinElevationAngle
        Me.SliderTiltAngle.Value = Me.KinectEvents.ElevationAngle
    End Sub

    ''' <summary>
    ''' Kinectの動作を停止する
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StopKinect()
        If Me.KinectEvents IsNot Nothing Then

            ' Kinectの停止と、ネイティブリソースを解放する
            Me.KinectEvents.Stop()
            Me.KinectEvents.Dispose()
            Me.KinectEvents = Nothing

            Me.ImageDepth.Source = Nothing
            Me.ImageRgb.Source = Nothing
        End If
    End Sub

    ''' <summary>
    ''' RGBカメラ、距離カメラ、骨格のフレーム更新イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Kinect_AllFramesReady(sender As Object,
                                      e As Microsoft.Kinect.AllFramesReadyEventArgs) _
                                  Handles KinectEvents.AllFramesReady
        Try
            ' センサーのインスタンスを取得する
            Dim sensor As KinectSensor = CType(sender, KinectSensor)
            If sensor Is Nothing Then
                Exit Sub
            End If

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

                    ' スケルトンのフレームを取得する
                    Using _skeletonFrame As SkeletonFrame = e.OpenSkeletonFrame
                        If _skeletonFrame IsNot Nothing Then
                            Call DrawSkeleton(sensor, _skeletonFrame, Me.ImageRgb.Source)
                        End If
                    End Using
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
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
        Me.SoundSource.Angle = -e.Angle
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
        Me.Beam.Angle = -e.Angle
    End Sub


    ''' <summary>
    ''' スケルトンを描画する
    ''' </summary>
    ''' <param name="sensor"></param>
    ''' <param name="_skeletonFrame"></param>
    ''' <param name="source"></param>
    ''' <remarks></remarks>
    Private Function DrawSkeleton(sensor As KinectSensor,
                                  _skeletonFrame As SkeletonFrame,
                                  source As ImageSource) As RenderTargetBitmap
        ' スケルトンのデータを取得する
        Dim skeletons(_skeletonFrame.SkeletonArrayLength - 1) As Skeleton
        _skeletonFrame.CopySkeletonDataTo(skeletons)

        Dim _drawingVisual = New DrawingVisual()
        Using _drawingContext As DrawingContext = _drawingVisual.RenderOpen()
            ' ImageSourceを描画する
            _drawingContext.DrawImage(source, New Rect(0, 0, source.Width, source.Height))

            ' トラッキングされているスケルトンのジョイントを描画する
            Const R As Integer = 5
            For Each _skeleton As Skeleton In skeletons
                ' スケルトンがトラッキングされていなければ次へ
                If _skeleton.TrackingState <> SkeletonTrackingState.Tracked Then
                    Continue For
                End If

                ' ジョイントを描画する
                For Each _joint As Joint In _skeleton.Joints
                    ' ジョイントがトラッキングされていなければ次へ
                    If _joint.TrackingState <> JointTrackingState.Tracked Then
                        Continue For
                    End If

                    ' スケルトンの座標を、RGBカメラの座標に変換して円を書く
                    Dim _point As ColorImagePoint = sensor.MapSkeletonPointToColor(_joint.Position,
                                                                                   sensor.ColorStream.Format)
                    _drawingContext.DrawEllipse(New SolidColorBrush(Colors.Red),
                                                New Pen(Brushes.Red, 1),
                                                New Point(_point.X, _point.Y), R, R)
                Next
            Next
        End Using

        ' 描画可能なビットマップを作る
        Dim bitmap = New RenderTargetBitmap(CType(source.Width, Integer),
                                             CType(source.Height, Integer),
                                             96,
                                             96,
                                             PixelFormats.Default)
        bitmap.Render(_drawingVisual)

        Return bitmap
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
        sensor.MapDepthFrameToColorFrame(depthStream.Format, depthPixel,
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
        Call StopKinect()
    End Sub

    ''' <summary>
    ''' スライダーの位置が変更された
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub SliderTiltAngle_ValueChanged(sender As System.Object,
                                             e As System.Windows.RoutedPropertyChangedEventArgs(Of System.Double))
        Try
            Me.KinectEvents.ElevationAngle = CType(e.NewValue, Integer)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub
End Class
