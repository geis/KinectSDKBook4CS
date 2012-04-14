Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Navigation
Imports System.Windows.Shapes
Imports Microsoft.Kinect
Imports Kinect.Toolbox

''' <summary>
''' MainWindow.xaml の相互作用ロジック
''' </summary>
''' <remarks></remarks>
Class MainWindow
    Inherits Window

    Friend WithEvents Kinect As KinectSensor
    Friend WithEvents SwipeDetector As New SwipeGestureDetector
    Friend WithEvents PostureDetector As New AlgorithmicPostureDetector

    Private ReadOnly Bgr32BytesPerPixel As Integer = PixelFormats.Bgr32.BitsPerPixel / 8

    Private ColorManager As New ColorStreamManager

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
        Me.Kinect = sensor

        '  RGBカメラを有効にする
        Me.Kinect.ColorStream.Enable()

        ' スケルトン・トラッキングを有効にする
        Me.Kinect.SkeletonStream.Enable()

        ' ジェスチャーの検出を有効にする
        Me.SwipeDetector.TraceTo(CanvasTrack, Colors.Red)

        ' Kinectの動作を開始する
        Me.Kinect.Start()
    End Sub

    ''' <summary>
    ''' RGBカメラの更新通知
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub kinect_ColorFrameReady(sender As Object,
                                       e As ColorImageFrameReadyEventArgs) _
                                   Handles Kinect.ColorFrameReady
        Try
            ' RGBカメラのフレームデータを取得する
            Using colorFrame As ColorImageFrame = e.OpenColorImageFrame
                If colorFrame IsNot Nothing Then
                    Me.ColorManager.Update(colorFrame)
                    Me.ImageRgb.Source = ColorManager.Bitmap
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' スケルトンの更新通知
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub kinect_SkeletonFrameReady(sender As Object,
                                          e As SkeletonFrameReadyEventArgs) _
                                      Handles Kinect.SkeletonFrameReady
        Try
            ' Kinectのインスタンスを取得する
            Dim sensor As KinectSensor = CType(sender, KinectSensor)
            If sensor Is Nothing Then
                Exit Sub
            End If

            ' スケルトンのフレームを取得する
            Using _skeletonFrame As SkeletonFrame = e.OpenSkeletonFrame
                If _skeletonFrame IsNot Nothing Then
                    Dim _skeleton As Skeleton = _skeletonFrame.GetFirstTrackedSkeleton()
                    If _skeleton IsNot Nothing Then
                        ' ポーズの検出用に、スケルトンデータを追加する
                        Me.PostureDetector.TrackPostures(_skeleton)

                        ' 右手がトラッキングされていた場合、ジェスチャーの検出用にジョイントを追加する
                        Dim hand As Joint = _skeleton.Joints(JointType.HandRight)
                        If hand.TrackingState = JointTrackingState.Tracked Then
                            Me.SwipeDetector.Add(hand.Position, sensor)
                        End If
                    End If
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' ポーズの検出を通知する
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <remarks></remarks>
    Private Sub postureDetector_PostureDetected(obj As String) Handles PostureDetector.PostureDetected
        Me.TextBlockPose.Text = obj
    End Sub

    ''' <summary>
    ''' ジェスチャーの検出を通知する
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <remarks></remarks>
    Private Sub swipeDetector_OnGestureDetected(obj As String) Handles SwipeDetector.OnGestureDetected
        Me.TextBlockGesture.Text = obj
    End Sub

    ''' <summary>
    ''' Kinectの動作を停止する
    ''' </summary>
    ''' <param name="sensor"></param>
    ''' <remarks></remarks>
    Private Sub StopKinect(ByVal sensor As KinectSensor)
        If Me.Kinect IsNot Nothing Then
            If Me.Kinect.IsRunning Then
                ' Kinectの停止と、ネイティブリソースを解放する
                Me.Kinect.Stop()
                Me.Kinect.Dispose()

                Me.ImageRgb.Source = Nothing
            End If
        End If
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
