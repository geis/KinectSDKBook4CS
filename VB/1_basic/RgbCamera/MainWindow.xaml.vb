Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.Threading
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Shapes
Imports Microsoft.Kinect

Partial Public Class MainWindow
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
        ' RGBカメラを有効にして、フレーム更新イベントを登録する
        kinect.ColorStream.Enable()
        AddHandler kinect.ColorFrameReady, AddressOf kinect_ColorFrameReady

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
                RemoveHandler kinect.ColorFrameReady, AddressOf kinect_ColorFrameReady

                ' Kinectの停止と、ネイティブリソースを解放する
                kinect.Stop()
                kinect.Dispose()
                kinect = Nothing
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
