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
Imports OpenCvSharp
Imports OpenCvSharp.Extensions


''' <summary>
''' MainWindow.xaml の相互作用ロジック
''' </summary>
''' <remarks></remarks>
Class MainWindow
    Inherits Window

    Friend WithEvents KinectEvents As KinectSensor
    Private OutputImage As WriteableBitmap

    ' 画像データ配列
    Private BGR32PixelData() As Byte
    Private RGB24PixelData() As Byte

    ' OpenCV用
    Private OpenCVImage As IplImage
    Private OpenCVGrayImage As IplImage
    Private Storage As CvMemStorage
    Private Cascade As CvHaarClassifierCascade

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        InitializeComponent()

        If KinectSensor.KinectSensors.Count = 0 Then
            Throw New Exception("Kinectが接続されていません")
        End If

        Call StartKinect(KinectSensor.KinectSensors(0))
        Call InitOpenCV()
    End Sub

    ''' <summary>
    ''' Kinectの動作を開始する
    ''' </summary>
    ''' <param name="sensor"></param>
    ''' <remarks></remarks>
    Private Sub StartKinect(ByVal sensor As KinectSensor)
        Try
            Me.KinectEvents = sensor
            Me.KinectEvents.ColorStream.Enable()
            Me.KinectEvents.SkeletonStream.Enable()

            Me.KinectEvents.Start()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Me.Close()
        End Try
    End Sub

    ''' <summary>
    ''' Kinectの動作を停止する
    ''' </summary>
    ''' <param name="sensor"></param>
    ''' <remarks></remarks>
    Private Sub StopKinect(ByVal sensor As KinectSensor)
        If Me.KinectEvents IsNot Nothing Then
            If Me.KinectEvents.IsRunning Then
                Me.KinectEvents.Stop()
                Me.KinectEvents.Dispose()

                Me.Image1.Source = Nothing
                Me.Image2.Source = Nothing
            End If
        End If
    End Sub

    ''' <summary>
    ''' OpenCV関連の変数初期化
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InitOpenCV()
        Me.OpenCVImage = New IplImage(Me.KinectEvents.ColorStream.FrameWidth,
                                      Me.KinectEvents.ColorStream.FrameHeight,
                                      BitDepth.U8,
                                      3)

        Me.OpenCVGrayImage = New IplImage(Me.KinectEvents.ColorStream.FrameWidth,
                                          Me.KinectEvents.ColorStream.FrameHeight,
                                          BitDepth.U8,
                                          1)

        Me.Storage = New CvMemStorage
        Me.Cascade = CvHaarClassifierCascade.FromFile(System.IO.Path.Combine("./haarcascade_frontalface_alt2.xml"))
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

        ' RGBカメラのフレームデータを取得する
        Using colorFrame As ColorImageFrame = e.OpenColorImageFrame()
            If colorFrame IsNot Nothing Then
                ' ColorImageFrame -> ImageSource (WriteableBitmap)　変換
                Me.Image1.Source = ConvertToBitmap(colorFrame)
                ' スケルトンフレームデータを取得する
                Using _skeletonFrame As SkeletonFrame = e.OpenSkeletonFrame()
                    If _skeletonFrame IsNot Nothing Then
                        ' スケルトンデータを取得する
                        Dim skeletonData(_skeletonFrame.SkeletonArrayLength - 1) As Skeleton
                        _skeletonFrame.CopySkeletonDataTo(skeletonData)

                        Dim _point = New ColorImagePoint()

                        ' プレーヤーごとのスケルトンから頭の位置を取得
                        For Each _skeleton As Skeleton In skeletonData
                            Dim head As Joint = _skeleton.Joints(JointType.Head)
                            If head.TrackingState = JointTrackingState.Tracked Then
                                _point = Me.KinectEvents.MapSkeletonPointToColor(head.Position,
                                                                           Me.KinectEvents.ColorStream.Format)
                            End If
                        Next

                        ' 鼻眼鏡の描画
                        ' ・スケルトンが認識出来なかった場合は処理をしない
                        ' ・6フレーム毎に顔検出を行う
                        If colorFrame IsNot Nothing AndAlso
                            (_point.X <> 0 OrElse _point.Y <> 0 AndAlso
                             colorFrame.FrameNumber Mod 6 = 0) Then
                            Dim _rect As Rect = CheckFacePosition(_point)

                            Me.Image2.Margin = New Thickness(_rect.X, _rect.Y, 0, 0)
                            Me.Image2.Width = _rect.Width
                            Me.Image2.Height = _rect.Height
                            Me.Image2.Visibility = System.Windows.Visibility.Visible
                        End If
                    End If
                End Using
            End If
        End Using
    End Sub

    ''' <summary>
    ''' WriteableBitmapに変換する
    ''' </summary>
    ''' <param name="cif"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ConvertToBitmap(ByVal cif As ColorImageFrame) As ImageSource
        ' 変数のインスタンス作成
        If Me.OutputImage Is Nothing OrElse BGR32PixelData Is Nothing Then
            ReDim Me.BGR32PixelData(cif.PixelDataLength - 1)
            Me.OutputImage = New WriteableBitmap(cif.Width,
                                                 cif.Height,
                                                 96,
                                                 96, PixelFormats.Rgb24,
                                                 Nothing)
        End If

        cif.CopyPixelDataTo(Me.BGR32PixelData)

        ' Kinectから得る画像データをOpenCVで処理できるように変換する
        Me.RGB24PixelData = ConvertBGR32toRGB24(Me.BGR32PixelData)

        ' 画像情報の書き込み
        Me.OutputImage.WritePixels(New Int32Rect(0,
                                                 0,
                                                 cif.Width,
                                                 cif.Height),
                                   Me.RGB24PixelData,
                                   Me.OutputImage.BackBufferStride,
                                   0)

        Return Me.OutputImage
    End Function

    ''' <summary>
    ''' 32bitBGR構成の画像バイト配列を24bitRGB構成に変換する
    ''' </summary>
    ''' <param name="pixels"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ConvertBGR32toRGB24(pixels As Byte()) As Byte()
        Dim BGR32Bits As Integer = 4
        Dim RGB24Bits As Integer = 3

        Dim bytes(pixels.Length / BGR32Bits * RGB24Bits - 1) As Byte

        For i As Integer = 0 To pixels.Length / BGR32Bits - 1
            ' BGR32bitからRGB24に変換
            bytes(i * RGB24Bits + 0) = pixels(i * BGR32Bits + 2)    ' R
            bytes(i * RGB24Bits + 1) = pixels(i * BGR32Bits + 1)    ' G
            bytes(i * RGB24Bits + 2) = pixels(i * BGR32Bits + 0)    ' B
        Next
        Return bytes
    End Function

    ''' <summary>
    ''' 顔の位置を取得
    ''' </summary>
    ''' <param name="headPosition">スケルトンの頭の位置座標</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckFacePosition(headPosition As ColorImagePoint) As Rect
        '切り取る領域の範囲
        Dim snipWidth As Integer = 200
        Dim snipHeight As Integer = 200

        ' 返却用Rect (初期値はスケルトンの頭の座標とimage2画像の幅)
        Dim reRect As New Rect(headPosition.X,
                               headPosition.Y,
                               Me.image2.Width,
                               Me.image2.Height)

        Me.Storage.Clear()
        Me.OpenCVGrayImage.ResetROI()           ' たまにROIがセットされた状態で呼ばれるためROIをリセット

        Me.OpenCVImage.CopyFrom(Me.OutputImage)                                     ' WriteableBitmap -> IplImage
        Cv.CvtColor(Me.OpenCVImage, Me.OpenCVGrayImage, ColorConversion.BgrToGray)  ' 画像をグレイスケール化
        Cv.EqualizeHist(OpenCVGrayImage, OpenCVGrayImage)                           ' 画像の平滑化

        ' 顔認識
        Try
            ' 画像の切り取り
            Dim snipImage = SnipFaceImage(Me.OpenCVGrayImage,
                                          headPosition,
                                          snipWidth,
                                          snipHeight)
            If snipImage IsNot Nothing Then
                Dim faces As CvSeq(Of CvAvgComp) = Cv.HaarDetectObjects(snipImage, Me.Cascade, Me.Storage)
                ' 顔を検出した場合
                If faces.Total > 0 Then
                    reRect.X = faces(0).Value.Rect.X + (headPosition.X - snipWidth / 2)
                    reRect.Y = faces(0).Value.Rect.Y + (headPosition.Y - snipHeight / 2)
                    reRect.Width = faces(0).Value.Rect.Width
                    reRect.Height = faces(0).Value.Rect.Height
                End If
            End If
        Catch ex As Exception
        End Try
        Return reRect
    End Function

    ''' <summary>
    ''' 画像を指定した領域で切り取る
    ''' </summary>
    ''' <param name="src">切り取る元画像</param>
    ''' <param name="centerPosition">切り取る領域の中心座標</param>
    ''' <param name="snipWidth">切り取る横幅</param>
    ''' <param name="snipHeight">切り取る縦幅</param>
    ''' <returns>切り取った画像</returns>
    ''' <remarks></remarks>
    Private Function SnipFaceImage(src As IplImage,
                                   centerPosition As ColorImagePoint,
                                   snipWidth As Integer,
                                   snipHeight As Integer) As IplImage
        Dim faceX As Integer
        Dim faceY As Integer

        ' 画面からはみ出している場合は切り取り処理しない
        If centerPosition.X - snipWidth / 2 < 0 OrElse
             centerPosition.Y - snipHeight / 2 < 0 Then
            Return Nothing
        Else
            faceX = centerPosition.X - snipWidth / 2
            faceY = centerPosition.Y - snipHeight / 2
        End If

        ' 切り取り領域の設定
        Dim faceRect As New CvRect(faceX,
                                   faceY,
                                   snipWidth,
                                   snipHeight)
        Dim part As New IplImage(faceRect.Size,
                                 BitDepth.U8,
                                 1)

        src.SetROI(faceRect)            ' 切り取り範囲を設定
        Cv.Copy(src, part)              ' データをコピー
        src.ResetROI()                  ' 指定した範囲のリセット

        Return part
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
