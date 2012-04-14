Imports System
Imports System.Windows
Imports Coding4Fun.Kinect.Wpf
Imports Microsoft.Kinect

''' <summary>
''' MainWindow.xaml の相互作用ロジック
''' </summary>
''' <remarks></remarks>
Class MainWindow
    Inherits Window

    Friend WithEvents KinectEvents As KinectSensor

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
    ''' <param name="sensor"></param>
    ''' <remarks></remarks>
    Private Sub StartKinect(ByVal sensor As KinectSensor)
        Me.KinectEvents = sensor

        ' Colorを有効にする
        Me.KinectEvents.ColorStream.Enable()

        ' Depthを有効にする
        Me.KinectEvents.DepthStream.Enable()

        ' Skeletonを有効にすることで、プレイヤーが取得できる
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
                                       e As ColorImageFrameReadyEventArgs) _
                                   Handles KinectEvents.ColorFrameReady
        Using colorFrame As ColorImageFrame = e.OpenColorImageFrame
            If colorFrame IsNot Nothing Then
                Me.ImageRgbCamera.Source = colorFrame.ToBitmapSource()
            End If
        End Using
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
        Using depthFrame As DepthImageFrame = e.OpenDepthImageFrame
            If depthFrame IsNot Nothing Then
                Me.ImageDepthCamera.Source = depthFrame.ToBitmapSource()
            End If
        End Using
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
