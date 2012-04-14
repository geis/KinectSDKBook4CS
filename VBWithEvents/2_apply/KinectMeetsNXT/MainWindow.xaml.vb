Imports System
Imports System.IO.Ports
Imports System.Linq
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Shapes
Imports Microsoft.Kinect
Imports NxtNet

''' <summary>
''' MainWindow.xaml の相互作用ロジック
''' </summary>
''' <remarks></remarks>
Class MainWindow
    Inherits Window

    Friend WithEvents KinectEvents As KinectSensor
    Private _Nxt = New Nxt()
    Private IsConneted As String = False

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

            ' NXT関連の初期化を行う
            Call InitNxt()

            ' Kinectの動作を開始する
            Call StartKinect(KinectSensor.KinectSensors(0))
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Me.Close()
        End Try
    End Sub

    ''' <summary>
    ''' NXT関連の初期化を行う
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitNxt()
        ' NXTはBluetoothをシリアルポートとして接続する
        If SerialPort.GetPortNames().Length = 0 Then
            Throw New Exception("NXTの接続先が見つかりません")
        End If

        Me.comboBoxPort.IsEnabled = True
        Me.buttonConnect.IsEnabled = True
        Me.buttonDisconnect.IsEnabled = False

        ' シリアルポートの一覧を表示する
        For Each port As String In SerialPort.GetPortNames()
            Me.comboBoxPort.Items.Add(port)
        Next
        Me.comboBoxPort.SelectedIndex = 0
    End Sub

    ''' <summary>
    ''' 接続ボタン
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ButtonConnect_Click(sender As Object,
                                    e As RoutedEventArgs)
        Call Connect()
    End Sub

    ''' <summary>
    ''' 切断ボタン
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub ButtonDisconnect_Click(sender As Object,
                                       e As RoutedEventArgs)
        Call Disconnect()
    End Sub

    ''' <summary>
    ''' Kinectの動作を開始する
    ''' </summary>
    ''' <param name="sensor"></param>
    ''' <remarks></remarks>
    Private Sub StartKinect(ByVal sensor As KinectSensor)
        Me.KinectEvents = sensor

        ' RGBカメラとスケルトンを有効にする
        Me.KinectEvents.ColorStream.Enable()
        Me.KinectEvents.SkeletonStream.Enable()

        ' Kinectの動作を開始する
        Me.KinectEvents.Start()
    End Sub

    ''' <summary>
    ''' Kinectの動作を停止する
    ''' </summary>
    ''' <param name="sensor"></param>
    ''' <remarks></remarks>
    Private Sub StopKinect(ByVal sensor As KinectSensor)
        If Me.KinectEvents IsNot Nothing Then
            If Me.KinectEvents.IsRunning Then
                ' Kinectの停止と、ネイティブリソースを解放する
                Me.KinectEvents.Stop()
                Me.KinectEvents.Dispose()

                Me.ImageRgb.Source = Nothing
            End If
        End If
    End Sub

    ''' <summary>
    ''' RGBカメラのフレーム更新イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Kinect_ColorFrameReady(sender As Object,
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
    ''' スケルトンのフレーム更新イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Kinect_SkeletonFrameReady(sender As Object,
                                          e As SkeletonFrameReadyEventArgs) _
                                      Handles KinectEvents.SkeletonFrameReady
        Try
            ' センサーのインスタンスを取得する
            Dim sendor As KinectSensor = CType(sender, KinectSensor)
            If sendor Is Nothing Then
                Exit Sub
            End If

            ' スケルトンのフレームを取得する
            Using skeletonFrame As SkeletonFrame = e.OpenSkeletonFrame
                If skeletonFrame IsNot Nothing Then
                    Call Power(sendor, skeletonFrame)
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
    Private Sub Power(sensor As KinectSensor,
                      _skeletonFrame As SkeletonFrame)
        ' スケルトンのデータを取得する
        Dim _skeleton As Skeleton = _skeletonFrame.GetFirstTrackedSkeleton()
        If _skeleton Is Nothing Then
            Exit Sub
        End If

        ' 操作に必要なジョイントを取得する
        Dim rightHand As Joint = _skeleton.Joints(JointType.HandRight)
        Dim rightElbow As Joint = _skeleton.Joints(JointType.ElbowRight)
        Dim leftHand As Joint = _skeleton.Joints(JointType.HandLeft)
        Dim leftElbow As Joint = _skeleton.Joints(JointType.ElbowLeft)

        ' ジョイントの描画
        Call DrawSkeleton(sensor, New Joint() {rightHand, rightElbow, leftHand, leftElbow})

        ' ジョイントすべてがトラッキング状態のときのみ操作する
        If (rightHand.TrackingState <> JointTrackingState.Tracked) OrElse
            (rightElbow.TrackingState <> JointTrackingState.Tracked) OrElse
            (leftHand.TrackingState <> JointTrackingState.Tracked) OrElse
            (leftElbow.TrackingState <> JointTrackingState.Tracked) Then
            Exit Sub
        End If

        ' 腕の角度を、モーターのパワーに変換して、NXTに送信する
        Dim rightPower As SByte = 0
        Dim leftPower As SByte = 0
        If rightHand.Position.Y > rightElbow.Position.Y Then
            rightPower = GetPower(rightHand, rightElbow)
        End If

        If leftHand.Position.Y > leftElbow.Position.Y Then
            leftPower = GetPower(leftHand, leftElbow)
        End If

        Call SetMotorPower(rightPower, leftPower)
    End Sub

    ''' <summary>
    ''' トラッキングされているスケルトンのジョイントを描画する
    ''' </summary>
    ''' <param name="sensor"></param>
    ''' <param name="joints"></param>
    ''' <remarks></remarks>
    Private Sub DrawSkeleton(sensor As KinectSensor,
                             joints As Joint())
        ' 描画する円の半径
        Const R As Integer = 5

        ' キャンバスをクリアする
        Me.CanvasSkeleton.Children.Clear()

        ' ジョイントを描画する
        For Each _joint As Joint In joints
            ' ジョイントがトラッキングされていなければ次へ
            If _joint.TrackingState <> JointTrackingState.Tracked Then
                Exit Sub
            End If

            ' スケルトンの座標を、RGBカメラの座標に変換して円を書く
            Dim _point As ColorImagePoint = sensor.MapSkeletonPointToColor(_joint.Position,
                                                                           sensor.ColorStream.Format)

            canvasSkeleton.Children.Add(New Ellipse With
            {
              .Fill = New SolidColorBrush(Colors.Red),
              .Margin = New Thickness(_point.X - R, _point.Y - R, 0, 0),
              .Width = R * 2,
              .Height = R * 2
            })
        Next
    End Sub

    ''' <summary>
    ''' 接続
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Connect()
        Try
            _Nxt.Connect(CType(ComboBoxPort.Items(Me.ComboBoxPort.SelectedIndex), String))
            Me.isConneted = True

            Me.ComboBoxPort.IsEnabled = False
            Me.buttonConnect.IsEnabled = False
            Me.buttonDisconnect.IsEnabled = True
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 切断
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub Disconnect()
        Try
            If Me.IsConneted Then
                Me.IsConneted = False

                Me.comboBoxPort.IsEnabled = True
                Me.buttonConnect.IsEnabled = True
                Me.buttonDisconnect.IsEnabled = False
            End If
            _Nxt.Disconnect()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' モーターのパワーを設定する
    ''' </summary>
    ''' <param name="right"></param>
    ''' <param name="left"></param>
    ''' <remarks></remarks>
    Private Sub SetMotorPower(right As SByte, left As SByte)
        If Me.IsConneted Then
            _Nxt.SetOutputState(MotorPort.PortA,
                                right,
                                MotorModes.On,
                                MotorRegulationMode.Idle,
                                100,
                                MotorRunState.Running,
                                0)
            _Nxt.SetOutputState(MotorPort.PortB,
                                left,
                                MotorModes.On,
                                MotorRegulationMode.Idle,
                                100,
                                MotorRunState.Running,
                                0)
        End If
    End Sub

    ''' <summary>
    ''' モーターのパワーを取得する
    ''' </summary>
    ''' <param name="hand"></param>
    ''' <param name="elbow"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetPower(hand As Joint, elbow As Joint) As SByte
        ' 手と肘の座標から、腕の角度を求める
        Dim a As Single = Math.Abs(hand.Position.Y - elbow.Position.Y)
        Dim b As Single = Math.Abs(hand.Position.Z - elbow.Position.Z)
        Dim c As Double = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2))
        Dim theta As SByte = CType(Math.Acos(a / c) * 180 / Math.PI, SByte)

        ' 腕の角度をモーターの強さにする
        theta *= 2
        If theta >= 100 Then
            theta = 100
        End If

        ' 手と肘の前後関係で、モーターの回転方向を変える
        Return CType(IIf(hand.Position.Z > elbow.Position.Z, CType(-theta, SByte), theta), SByte)
    End Function

    ''' <summary>
    ''' Windowsが閉じられるときのイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Window_Closing(sender As System.Object,
                               e As System.ComponentModel.CancelEventArgs)
        Call Disconnect()
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
