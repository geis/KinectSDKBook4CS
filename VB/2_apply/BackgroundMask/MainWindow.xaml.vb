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
        ' RGBカメラ、距離カメラ、スケルトン・トラッキング(プレイヤーの認識)を有効にする
        kinect.ColorStream.Enable()
        kinect.DepthStream.Enable()
        kinect.SkeletonStream.Enable()

        ' RGBカメラ、距離カメラ、スケルトンのフレーム更新イベントを登録する
        AddHandler kinect.AllFramesReady, AddressOf Kinect_AllFramesReady

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
                RemoveHandler kinect.AllFramesReady, AddressOf Kinect_AllFramesReady
                ' Kinectの停止と、ネイティブリソースを解放する
                kinect.Stop()
                kinect.Dispose()

                Me.imageRgb.Source = Nothing
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

            ' 背景をマスクした画像を描画する
            Using colorFrame As ColorImageFrame = e.OpenColorImageFrame
                Using depthFrame As DepthImageFrame = e.OpenDepthImageFrame
                    If colorFrame IsNot Nothing AndAlso depthFrame IsNot Nothing Then
                        Me.imageRgb.Source = BitmapSource.Create(colorFrame.Width,
                                                                 colorFrame.Height,
                                                                 96,
                                                                 96,
                                                                 PixelFormats.Bgr32,
                                                                 Nothing,
                                                                 BackgroundMask(kinect, colorFrame, depthFrame),
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
    ''' <param name="kinect"></param>
    ''' <param name="colorFrame"></param>
    ''' <param name="depthFrame"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function BackgroundMask(ByVal kinect As KinectSensor,
                                    ByVal colorFrame As ColorImageFrame,
                                    ByVal depthFrame As DepthImageFrame) As Byte()
        Dim colorStream As ColorImageStream = kinect.ColorStream
        Dim depthStream As DepthImageStream = kinect.DepthStream

        ' RGBカメラのピクセルごとのデータを取得する
        Dim colorPixel(colorFrame.PixelDataLength - 1) As Byte
        colorFrame.CopyPixelDataTo(colorPixel)

        ' 距離カメラのピクセルごとのデータを取得する
        Dim depthPixel(depthFrame.PixelDataLength - 1) As Short
        depthFrame.CopyPixelDataTo(depthPixel)

        ' 距離カメラの座標に対応するRGBカメラの座標を取得する(座標合わせ)
        Dim colorPoint(depthFrame.PixelDataLength - 1) As ColorImagePoint
        kinect.MapDepthFrameToColorFrame(depthStream.Format,
                                         depthPixel,
                                         colorStream.Format,
                                         colorPoint)

        ' 出力バッファ(初期値は白(255,255,255))
        Dim outputColor(colorPixel.Length - 1) As Byte
        For i As Integer = 0 To outputColor.Length - 1 Step Bgr32BytesPerPixel
            outputColor(i) = 255
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
                outputColor(colorIndex) = colorPixel(colorIndex)
                outputColor(colorIndex + 1) = colorPixel(colorIndex + 1)
                outputColor(colorIndex + 2) = colorPixel(colorIndex + 2)
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
