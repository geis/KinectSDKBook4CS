Imports System
Imports System.Drawing
Imports System.Windows.Forms
Imports Microsoft.Kinect
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Partial Public Class Form1
    Inherits Form

    Private ReadOnly Bgr32BytesPerPixel As Integer = 4 ' ピクセルあたりのバイト数

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        InitializeComponent()

        Try
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
        kinect.ColorStream.Enable()
        kinect.DepthStream.Enable()
        kinect.SkeletonStream.Enable()

        AddHandler kinect.AllFramesReady, AddressOf kinect_AllFramesReady

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
                RemoveHandler kinect.AllFramesReady, AddressOf kinect_AllFramesReady

                kinect.Stop()
                kinect.Dispose()
                kinect = Nothing

                Me.pictureBoxRgb.Image = Nothing
                Me.pictureBoxDepth.Image = Nothing
            End If
        End If
    End Sub

    ''' <summary>
    ''' RGBカメラ、距離カメラ、骨格のフレーム更新イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Kinect_AllFramesReady(sender As Object,
                                      e As Microsoft.Kinect.AllFramesReadyEventArgs)
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
                    Dim _bitmap As New Bitmap(kinect.ColorStream.FrameWidth,
                                              kinect.ColorStream.FrameHeight,
                                              System.Drawing.Imaging.PixelFormat.Format32bppRgb)

                    Dim rect As New Rectangle(0, 0, _bitmap.Width, _bitmap.Height)
                    Dim data As BitmapData = _bitmap.LockBits(rect,
                                                              ImageLockMode.WriteOnly,
                                                              PixelFormat.Format32bppRgb)
                    Marshal.Copy(colorPixel, 0, data.Scan0, colorPixel.Length)
                    _bitmap.UnlockBits(data)

                    Me.pictureBoxRgb.Image = _bitmap
                End If
            End Using

            ' 書き込み用のビットマップデータを作成(32bit bitmap)
            ' 16bpp グレースケールは表示できない
            ' 距離カメラのフレームデータを取得する
            Using _depthFrame As DepthImageFrame = e.OpenDepthImageFrame
                If _depthFrame IsNot Nothing Then
                    Dim _bitmap As New Bitmap(kinect.DepthStream.FrameWidth,
                                              kinect.DepthStream.FrameHeight,
                                              System.Drawing.Imaging.PixelFormat.Format32bppRgb)

                    Dim rect As New Rectangle(0, 0, _bitmap.Width, _bitmap.Height)
                    Dim data As BitmapData = _bitmap.LockBits(rect, ImageLockMode.WriteOnly,
                                                            System.Drawing.Imaging.PixelFormat.Format32bppRgb)
                    Dim gray() As Byte = ConvertDepthColor(kinect, _depthFrame)
                    Marshal.Copy(gray, 0, data.Scan0, gray.Length)
                    _bitmap.UnlockBits(data)

                    Me.pictureBoxDepth.Image = _bitmap
                End If
            End Using

            ' スケルトンのフレームを取得する
            Using _skeletonFrame As SkeletonFrame = e.OpenSkeletonFrame
                If _skeletonFrame IsNot Nothing Then
                    Dim g As Graphics = Graphics.FromImage(Me.pictureBoxRgb.Image)

                    ' スケルトンのデータを取得する
                    Dim skeletons(_skeletonFrame.SkeletonArrayLength - 1) As Skeleton
                    _skeletonFrame.CopySkeletonDataTo(skeletons)

                    ' トラッキングされているスケルトンのジョイントを描画する
                    For Each _skeleton In skeletons
                        ' スケルトンがトラッキング状態(デフォルトモード)の場合は、ジョイントを描画する
                        If _skeleton.TrackingState = SkeletonTrackingState.Tracked Then
                            ' ジョイントを描画する
                            For Each _joint As Joint In _skeleton.Joints
                                ' ジョイントがトラッキングされていなければ次へ
                                If _joint.TrackingState <> JointTrackingState.Tracked Then
                                    Continue For
                                End If

                                ' スケルトンの座標を、RGBカメラの座標に変換して円を書く
                                Call DrawEllipse(kinect, g, _joint.Position)
                            Next
                            ' スケルトンが位置追跡(ニアモードの)の場合は、スケルトン位置(Center hip)を描画する
                        ElseIf _skeleton.TrackingState = SkeletonTrackingState.PositionOnly Then
                            ' スケルトンの座標を、RGBカメラの座標に変換して円を書く
                            Call DrawEllipse(kinect, g, _skeleton.Position)
                        End If
                    Next
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
    Private Sub DrawEllipse(kinect As KinectSensor, g As Graphics, position As SkeletonPoint)
        Const R As Integer = 5

        ' スケルトンの座標を、RGBカメラの座標に変換する
        Dim _point As ColorImagePoint = kinect.MapSkeletonPointToColor(position, kinect.ColorStream.Format)

        g.DrawEllipse(New Pen(Brushes.Red, R),
                      New Rectangle(_point.X - R, _point.Y - R, R * 2, R * 2))
    End Sub

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
    ''' Windowsが閉じられるときのイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Form1_FormClosing(sender As Object,
                                  e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call StopKinect(KinectSensor.KinectSensors(0))
    End Sub

    ''' <summary>
    ''' 距離カメラの通常/近接モード変更イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub comboBoxRange_SelectionChanged(sender As System.Object,
                                               e As EventArgs)
        Try
            KinectSensor.KinectSensors(0).DepthStream.Range = CType(comboBoxRange.SelectedIndex, DepthRange)
        Catch ex As Exception
            comboBoxRange.SelectedIndex = 0
        End Try

    End Sub
End Class
