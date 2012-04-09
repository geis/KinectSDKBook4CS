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
    Private ReadOnly Bgr32BytesPerPixel As Integer = PixelFormats.Bgr32.BitsPerPixel / 8

    Public WithEvents Kinect As KinectSensor

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
    ''' <param name="kinect"></param>
    ''' <remarks></remarks>
    Public Sub StartKinect(ByVal kinect As KinectSensor)
        Me.Kinect = kinect

        ' RGBカメラ、距離カメラ、スケルトン・トラッキング(プレイヤーの認識)を有効にする
        Me.Kinect.ColorStream.Enable()
        'Me.Kinect.DepthStream.Enable()
        Me.Kinect.SkeletonStream.Enable()

        ' Kinectの動作を開始する
        Me.Kinect.Start()

        ' 音源推定のビームと、音源方向が変更した際に通知されるイベントを登録する
        AddHandler Me.Kinect.AudioSource.BeamAngleChanged, AddressOf AudioSource_BeamAngleChanged
        AddHandler Me.Kinect.AudioSource.SoundSourceAngleChanged, AddressOf AudioSource_SoundSourceAngleChanged

        ' defaultモードとnearモードの切り替え
        Me.comboBoxRange.Items.Clear()
        For Each range In [Enum].GetValues(GetType(DepthRange))
            Me.comboBoxRange.Items.Add(range.ToString)
        Next
        Me.comboBoxRange.SelectedIndex = 0

        ' チルトモーターを動作させるスライダーを設定する
        Me.sliderTiltAngle.Maximum = Me.Kinect.MaxElevationAngle
        Me.sliderTiltAngle.Minimum = Me.Kinect.MinElevationAngle
        Me.sliderTiltAngle.Value = Me.Kinect.ElevationAngle
    End Sub

    ''' <summary>
    ''' Kinectの動作を停止する
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StopKinect()
        If Me.Kinect IsNot Nothing Then

            ' Kinectの停止と、ネイティブリソースを解放する
            Me.Kinect.Stop()
            Me.Kinect.Dispose()
            Me.Kinect = Nothing

            Me.imageDepth.Source = Nothing
            Me.imageRgb.Source = Nothing
        End If
    End Sub

    ''' <summary>
    ''' RGBカメラ、距離カメラ、骨格のフレーム更新イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Kinect_AllFramesReady(sender As Object,
                                      e As Microsoft.Kinect.AllFramesReadyEventArgs) Handles Kinect.AllFramesReady
        Try
            ' センサーのインスタンスを取得する
            Dim kinect As KinectSensor = CType(sender, KinectSensor)
            If kinect Is Nothing Then
                Exit Sub
            End If

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

                    ' スケルトンのフレームを取得する
                    Using skeletonFrame As SkeletonFrame = e.OpenSkeletonFrame
                        If skeletonFrame IsNot Nothing Then
                            Call DrawSkeleton(kinect, skeletonFrame, Me.imageRgb.Source)
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
                                                    e As SoundSourceAngleChangedEventArgs)
        soundSource.Angle = -e.Angle
    End Sub

    ''' <summary>
    ''' ビーム方向が変化した
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub AudioSource_BeamAngleChanged(sender As Object,
                                             e As BeamAngleChangedEventArgs)
        beam.Angle = -e.Angle
    End Sub


    ''' <summary>
    ''' スケルトンを描画する
    ''' </summary>
    ''' <param name="kinect"></param>
    ''' <param name="skeletonFrame"></param>
    ''' <param name="source"></param>
    ''' <remarks></remarks>
    Private Function DrawSkeleton(kinect As KinectSensor,
                                  skeletonFrame As SkeletonFrame,
                                  source As ImageSource) As RenderTargetBitmap
        ' スケルトンのデータを取得する
        Dim skeletons(skeletonFrame.SkeletonArrayLength - 1) As Skeleton
        skeletonFrame.CopySkeletonDataTo(skeletons)

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
                    Dim _point As ColorImagePoint = kinect.MapSkeletonPointToColor(_joint.Position,
                                                                                   kinect.ColorStream.Format)
                    _drawingContext.DrawEllipse(New SolidColorBrush(Colors.Red),
                          New Pen(Brushes.Red, 1), New Point(_point.X, _point.Y), R, R)
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
        Call StopKinect()
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
            Me.Kinect.ElevationAngle = CType(e.NewValue, Integer)
        Catch ex As Exception
            Trace.WriteLine(ex.Message)
        End Try
    End Sub
End Class
