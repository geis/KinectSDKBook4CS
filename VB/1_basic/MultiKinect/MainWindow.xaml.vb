Imports System
Imports System.Windows
Imports Microsoft.Kinect

Partial Public Class MainWindow
    Inherits Window

    Private KinectWindows() As KinectWindow

    Public Sub New()
        Try
            InitializeComponent()

            Me.KinectWindows = New KinectWindow() {
                kinectWindow1,
                kinectWindow2,
                kinectWindow3,
                kinectWindow4
            }
            AddHandler KinectSensor.KinectSensors.StatusChanged, AddressOf KinectSensors_StatusChanged

            ' 接続されているKinectの動作を開始する
            For i As Integer = 0 To KinectSensor.KinectSensors.Count - 1
                If KinectSensor.KinectSensors(i).Status = KinectStatus.Connected Then
                    Me.KinectWindows(i).StartKinect(KinectSensor.KinectSensors(i))
                    Me.comboBoxSkeleton.Items.Add(i.ToString)
                End If
            Next
            comboBoxSkeleton.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Close()
        End Try
    End Sub

    ''' <summary>
    ''' Kinectの接続状態が変わった時に呼び出される
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KinectSensors_StatusChanged(sender As Object, e As StatusChangedEventArgs)
        If e.Status = KinectStatus.Connected Then
            ' デバイスが接続された
            Call [Start](e)
        ElseIf e.Status = KinectStatus.Disconnected Then
            ' デバイスが切断された
            Call [Stop](e)
        ElseIf e.Status = KinectStatus.NotPowered Then
            ' ACが抜けてる
            Call [Stop](e)
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
    ''' Kinectの動作を開始する
    ''' </summary>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub [Start](e As StatusChangedEventArgs)
        For i As Integer = 0 To Me.KinectWindows.Length - 1
            If Me.KinectWindows(i).Kinect Is Nothing Then
                Me.KinectWindows(i).StartKinect(e.Sensor)
                Me.comboBoxSkeleton.Items.Add(i.ToString())
                Exit For
            End If
        Next
    End Sub

    ''' <summary>
    ''' Kinectの動作を停止する
    ''' </summary>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub [Stop](e As StatusChangedEventArgs)
        For i As Integer = 0 To Me.KinectWindows.Length - 1
            If Me.KinectWindows(i).Kinect Is e.Sensor Then
                Me.KinectWindows(i).StopKinect()
                Me.comboBoxSkeleton.Items.Remove(i.ToString())
                Exit For
            End If
        Next
    End Sub

    ''' <summary>
    ''' Windowsが閉じられるときのイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Window_Closing(sender As System.Object,
                               e As System.ComponentModel.CancelEventArgs)
        For i As Integer = 0 To Me.KinectWindows.Length - 1
            If Me.KinectWindows(i).Kinect IsNot Nothing Then
                Me.KinectWindows(i).StopKinect()
            End If
        Next
    End Sub

    ''' <summary>
    ''' スケルトン・トラッキングさせるデバイスが変更された
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub comboBox1_SelectionChanged(sender As System.Object,
                                           e As System.Windows.Controls.SelectionChangedEventArgs)
        For i As Integer = 0 To Me.KinectWindows.Length - 1
            If Me.KinectWindows(i).Kinect IsNot Nothing AndAlso
                Me.KinectWindows(i).Kinect.SkeletonStream.IsEnabled Then
                Me.KinectWindows(i).Kinect.SkeletonStream.Disable()
                Exit For
            End If
        Next

        Me.KinectWindows(comboBoxSkeleton.SelectedIndex).Kinect.SkeletonStream.Enable()
    End Sub
End Class
