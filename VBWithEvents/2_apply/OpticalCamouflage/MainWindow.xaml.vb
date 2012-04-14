' BackgroundMaskの続きという設定
Imports System
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports Microsoft.Kinect

''' <summary>
''' MainWindow.xaml の相互作用ロジック
''' </summary>
''' <remarks></remarks>
Class MainWindow
    Inherits Window

    Friend WithEvents KinectEvents As KinectSensor
    Private ReadOnly Bgr32BytesPerPixel As Integer = PixelFormats.Bgr32.BitsPerPixel / 8
    Private BackPixel() As Byte

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

        ' RGBカメラ、距離カメラ、スケルトン・トラッキング(プレイヤーの認識)を有効にする
        Me.KinectEvents.ColorStream.Enable()
        Me.KinectEvents.DepthStream.Enable()
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

            ' 背景をマスクした画像を描画する
            Using colorFrame As ColorImageFrame = e.OpenColorImageFrame
                Using depthFrame As DepthImageFrame = e.OpenDepthImageFrame
                    If colorFrame IsNot Nothing AndAlso depthFrame IsNot Nothing Then
                        Dim bits() As Byte
                        If Me.RadioButtonOpticalCamouflage.IsChecked Then
                            bits = OpticalCamouflage(sensor, colorFrame, depthFrame)
                        Else
                            bits = BackgroundMask(sensor, colorFrame, depthFrame)
                        End If
                        Me.ImageRgb.Source = BitmapSource.Create(colorFrame.Width,
                                                                 colorFrame.Height,
                                                                 96,
                                                                 96,
                                                                 PixelFormats.Bgr32,
                                                                 Nothing,
                                                                 bits,
                                                                 colorFrame.Width * colorFrame.BytesPerPixel)
                    End If
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' プレーヤーだけ表示する
    ''' </summary>
    ''' <param name="sensor"></param>
    ''' <param name="colorFrame"></param>
    ''' <param name="depthFrame"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function BackgroundMask(ByVal sensor As KinectSensor,
                                    ByVal colorFrame As ColorImageFrame,
                                    ByVal depthFrame As DepthImageFrame) As Byte()
        Dim colorStream As ColorImageStream = sensor.ColorStream
        Dim depthStream As DepthImageStream = sensor.DepthStream

        ' RGBカメラのピクセルごとのデータを取得する
        Dim colorPixel(colorFrame.PixelDataLength - 1) As Byte
        colorFrame.CopyPixelDataTo(colorPixel)

        ' 距離カメラのピクセルごとのデータを取得する
        Dim depthPixel(depthFrame.PixelDataLength - 1) As Short
        depthFrame.CopyPixelDataTo(depthPixel)

        ' 距離カメラの座標に対応するRGBカメラの座標を取得する(座標合わせ)
        Dim colorPoint(depthFrame.PixelDataLength - 1) As ColorImagePoint
        sensor.MapDepthFrameToColorFrame(depthStream.Format,
                                         depthPixel,
                                         colorStream.Format,
                                         colorPoint)

        ' 出力バッファ(初期値は白(255,255,255))
        Dim outputColor(colorPixel.Length - 1) As Byte
        For i As Integer = 0 To outputColor.Length - 1 Step Bgr32BytesPerPixel
            outputColor(i + 0) = 255
            outputColor(i + 1) = 255
            outputColor(i + 2) = 255
        Next

        For index As Integer = 0 To depthPixel.Length - 1
            ' プレイヤーを取得する
            Dim player As Integer = depthPixel(index) And DepthImageFrame.PlayerIndexBitmask

            ' 変換した結果が、フレームサイズを超えることがあるため、小さいほうを使う
            Dim x As Integer = Math.Min(colorPoint(index).X, colorStream.FrameWidth - 1)
            Dim y As Integer = Math.Min(colorPoint(index).Y, colorStream.FrameHeight - 1)
            Dim colorIndex As Integer = ((y * depthFrame.Width) + x) * Bgr32BytesPerPixel

            ' プレーヤーを検出した座標だけ、RGBカメラの画像を使う
            If player <> 0 Then
                outputColor(colorIndex + 0) = colorPixel(colorIndex + 0)
                outputColor(colorIndex + 1) = colorPixel(colorIndex + 1)
                outputColor(colorIndex + 2) = colorPixel(colorIndex + 2)
            End If
        Next

        Return outputColor
    End Function

    ''' <summary>
    ''' 光学迷彩
    ''' </summary>
    ''' <param name="kinect"></param>
    ''' <param name="colorFrame"></param>
    ''' <param name="depthFrame"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function OpticalCamouflage(sensor As KinectSensor,
                                       colorFrame As ColorImageFrame,
                                       depthFrame As DepthImageFrame)

        Dim colorStream As ColorImageStream = sensor.ColorStream
        Dim depthStream As DepthImageStream = sensor.DepthStream

        ' RGBカメラのピクセルごとのデータを取得する
        Dim colorPixel(colorFrame.PixelDataLength - 1) As Byte
        colorFrame.CopyPixelDataTo(colorPixel)

        ' 背景がないときは、そのときのフレームを背景として保存する
        If Me.BackPixel Is Nothing Then
            ReDim Me.BackPixel(colorFrame.PixelDataLength - 1)
            Array.Copy(colorPixel, Me.BackPixel, Me.BackPixel.Length)
        End If

        ' 距離カメラのピクセルごとのデータを取得する
        Dim depthPixel(depthFrame.PixelDataLength - 1) As Short
        depthFrame.CopyPixelDataTo(depthPixel)

        ' 距離カメラの座標に対応するRGBカメラの座標を取得する(座標合わせ)
        Dim colorPoint(depthFrame.PixelDataLength - 1) As ColorImagePoint
        sensor.MapDepthFrameToColorFrame(depthStream.Format,
                                         depthPixel,
                                         colorStream.Format,
                                         colorPoint)

        ' 出力バッファ(初期値はRGBカメラの画像)
        Dim outputColor(colorPixel.Length - 1) As Byte
        Array.Copy(colorPixel, outputColor, outputColor.Length)

        For index As Integer = 0 To depthPixel.Length - 1
            ' プレイヤーを取得する
            Dim player As Integer = depthPixel(index) And DepthImageFrame.PlayerIndexBitmask

            ' 変換した結果が、フレームサイズを超えることがあるため、小さいほうを使う
            Dim x As Integer = Math.Min(colorPoint(index).X, colorStream.FrameWidth - 1)
            Dim y As Integer = Math.Min(colorPoint(index).Y, colorStream.FrameHeight - 1)
            Dim colorIndex As Integer = ((y * depthFrame.Width) + x) * Bgr32BytesPerPixel

            ' プレーヤーを検出した座標は、背景画像を使う
            If player <> 0 Then
                outputColor(colorIndex + 0) = Me.BackPixel(colorIndex + 0)
                outputColor(colorIndex + 1) = Me.BackPixel(colorIndex + 1)
                outputColor(colorIndex + 2) = Me.BackPixel(colorIndex + 2)
            End If
        Next

        Return outputColor
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
