Imports System
Imports System.Linq
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports Microsoft.Kinect
Imports System.Windows.Shapes

''' <summary>
''' MainWindow.xaml の相互作用ロジック
''' </summary>
''' <remarks></remarks>
Class MainWindow
    Inherits Window

    ' 音源方向
    Private SoundSourceAngle As Integer = 0

    Private Const PlayerCount As Integer = 7
    Private EnablePlayer(PlayerCount - 1) As Boolean
    Private playerAngles(PlayerCount - 1) As Integer
    Private playerColor() As Color = {Colors.White,
                                      Colors.Red,
                                      Colors.Blue,
                                      Colors.Green,
                                      Colors.Yellow,
                                      Colors.Pink,
                                      Colors.Black
                                     }

    Private ReadOnly Bgr32BytesPerPixel As Integer = PixelFormats.Bgr32.BitsPerPixel / 8

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

        ' 音源方向通知を設定して、音声処理を開始する
        AddHandler kinect.AudioSource.SoundSourceAngleChanged, AddressOf AudioSource_SoundSourceAngleChanged

        kinect.AudioSource.Start()
    End Sub

    ''' <summary>
    ''' 音源方向が通知される
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub AudioSource_SoundSourceAngleChanged(sender As Object,
                                                    e As SoundSourceAngleChangedEventArgs)
        If e.ConfidenceLevel > 0.5 Then
            Const Range As Integer = 5      ' 誤差範囲
            SoundSourceAngle = CType(e.Angle, Integer)

            For i As Integer = 1 To playerAngles.Length - 1
                ' 無効なプレイヤー
                If playerAngles(i) = -1 Then
                    Continue For
                End If

                ' 音源と頭の角度が一定範囲内にあれば、その人の音とみなす
                If ((SoundSourceAngle - Range) <= playerAngles(i)) AndAlso
                    (playerAngles(i) <= (SoundSourceAngle + Range)) Then
                    Me.EnablePlayer(i) = True
                End If

                ' ユーザーの位置を表示する
                Me.labelSoundSource.Content = String.Format("音源方向:{0}, プレイヤー方向:{1}",
                                                            SoundSourceAngle.ToString(), playerAngles(i))
            Next
        End If
    End Sub

    ''' <summary>
    ''' スケルトンを描画する
    ''' </summary>
    ''' <param name="kinect"></param>
    ''' <param name="_skeletonFrame"></param>
    ''' <remarks></remarks>
    Private Sub DrawSkeleton(kinect As KinectSensor,
                             _skeletonFrame As SkeletonFrame)
        ' スケルトンのデータを取得する
        Dim skeletons(_skeletonFrame.SkeletonArrayLength - 1) As Skeleton
        _skeletonFrame.CopySkeletonDataTo(skeletons)

        canvasSkeleton.Children.Clear()

        ' トラッキングされているスケルトンのジョイントを描画する
        For i As Integer = 0 To skeletons.Length - 1
            ' プレーヤーインデックスは、skeleton配列のインデックス + 1
            Dim playerIndex As Integer = i + 1

            ' プレイヤー位置を初期化する
            playerAngles(playerIndex) = -1

            ' スケルトンがトラッキング状態でなければ次へ
            If skeletons(i).TrackingState <> SkeletonTrackingState.Tracked Then
                Continue For
            End If

            Dim _skeleton As Skeleton = skeletons(i)

            ' 頭の位置を取得する
            Dim _joint As Joint = _skeleton.Joints(JointType.Head)
            If _joint.TrackingState = JointTrackingState.NotTracked Then
                Continue For
            End If

            ' ジョイントの座標を描く
            Call DrawEllipse(kinect, _joint.Position)

            ' プレイヤーの角度を取得する
            playerAngles(playerIndex) = GetPlayerAngle(_joint)

        Next
    End Sub

    ''' <summary>
    ''' プレイヤーの角度を取得する
    ''' </summary>
    ''' <param name="_joint"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetPlayerAngle(_joint As Joint) As Integer
        ' 頭の角度(Kinect 中心)
        Dim a As Decimal = Math.Abs(_joint.Position.X * 100)
        Dim b As Decimal = Math.Abs(_joint.Position.Z * 100)
        Dim c As Double = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2))
        Dim theta As Integer = CType((Math.Acos(a / c) * 180 / Math.PI), Integer)

        theta = CType(Math.Abs(theta - 90), Integer)
        If _joint.Position.X < 0 Then
            theta = -theta
        End If

        Return theta
    End Function

    ''' <summary>
    ''' 距離データをカラー画像に変換する
    ''' </summary>
    ''' <param name="kinect"></param>
    ''' <param name="depthFrame"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ConvertDepthColor(kinect As KinectSensor,
                                       depthFrame As DepthImageFrame) As Byte()
        Dim colorStream As ColorImageStream = kinect.ColorStream
        Dim depthStream As DepthImageStream = kinect.DepthStream

        ' 距離カメラのピクセルごとのデータを取得する
        Dim depthPixel(depthFrame.PixelDataLength - 1) As Short
        depthFrame.CopyPixelDataTo(depthPixel)

        ' 距離カメラの座標に対応するRGBカメラの座標を取得する(座標合わせ)
        Dim colorPoint(depthFrame.PixelDataLength - 1) As ColorImagePoint
        kinect.MapDepthFrameToColorFrame(depthStream.Format,
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
                ' 有効なプレーヤーに色付けする
                If EnablePlayer(player) Then
                    depthColor(colorIndex + 0) = playerColor(player).B
                    depthColor(colorIndex + 1) = playerColor(player).G
                    depthColor(colorIndex + 2) = playerColor(player).R
                End If
            End If
        Next

        Return depthColor
    End Function

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

                Me.imageRgb.Source = Nothing
                Me.imageDepth.Source = Nothing
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
    ''' ジョイントの円を描く
    ''' </summary>
    ''' <param name="kinect"></param>
    ''' <param name="position"></param>
    ''' <remarks></remarks>
    Private Sub DrawEllipse(kinect As KinectSensor,
                            position As SkeletonPoint)
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
