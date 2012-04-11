Imports System
Imports System.Collections.Generic
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Shapes

Imports Microsoft.Kinect

''' <summary>
''' MainWindow.xaml の相互作用ロジック
''' </summary>
''' <remarks></remarks>
Class MainWindow
    Inherits Window

    ''' <summary>
    ''' セレクトモード
    ''' </summary>
    ''' <remarks></remarks>
    Enum SelectMode
        NOTSELECTED     ' 領域が選択されていない
        SELECTING       ' 領域を選択中
        SELECTED        ' 領域が選択されている
    End Enum

    Private ReadOnly Bgr32BytesPerPixel As Integer = PixelFormats.Bgr32.BitsPerPixel / 8
    Private ReadOnly ERROR_OF_POINT As Integer = -100     ' タッチポイントのエラー値設定

    Private Kinect As KinectSensor
    Private _PaintWindow As PaintWindow
    Private CurrentMode As SelectMode = SelectMode.NOTSELECTED      ' 現在の領域選択モード

    Private StartPointOfRect As Point         ' 指定領域の始点
    Private PreTouchPoint As Point            ' 前フレームのタッチ座標

    Private SelectRegion As Rect              ' 指定した領域
    Private BackgroundDepthData() As Short    ' 領域内のデプスマップ
    Private DepthPixel() As Short
    Private ColorImagePixelPoint() As ColorImagePoint

    ''' <summary>
    ''' コンストラクタ
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        Try
            InitializeComponent()

            If KinectSensor.KinectSensors.Count = 0 Then
                Throw New Exception("Kinectが接続されていません")
            End If

            Call StartKinect(KinectSensor.KinectSensors(0))

            Me.PreTouchPoint = New Point(ERROR_OF_POINT, ERROR_OF_POINT)
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Me.Close()
        End Try
    End Sub

    ''' <summary>
    ''' Kinectの動作を開始する
    ''' </summary>
    ''' <param name="kin"></param>
    ''' <remarks></remarks>
    Private Sub StartKinect(ByVal kin As KinectSensor)
        Try
            Me.Kinect = kin
            Me.Kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30)
            Me.Kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30)
            AddHandler Me.Kinect.AllFramesReady, AddressOf Kinect_AllFramesReady

            Me.Kinect.Start()
        Catch ex As Exception
            MessageBox.Show(ex.Message)
            Me.Close()
        End Try
    End Sub

    ''' <summary>
    ''' Kinectの動作を停止する
    ''' </summary>
    ''' <param name="kinect"></param>
    ''' <remarks></remarks>
    Private Sub StopKinect(ByVal kinect As KinectSensor)
        If kinect IsNot Nothing Then
            If kinect.IsRunning Then
                RemoveHandler kinect.AllFramesReady, AddressOf Kinect_AllFramesReady

                kinect.Stop()
                kinect.Dispose()
            End If
        End If
    End Sub

    ''' <summary>
    ''' RGBカメラ、距離カメラのフレーム更新イベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Kinect_AllFramesReady(sender As Object, e As AllFramesReadyEventArgs)
        Try
            Using colorFrame As ColorImageFrame = e.OpenColorImageFrame
                Using depthFrame As DepthImageFrame = e.OpenDepthImageFrame
                    If colorFrame IsNot Nothing AndAlso depthFrame IsNot Nothing AndAlso Me.Kinect.IsRunning Then
                        If Me.DepthPixel Is Nothing Then
                            ReDim Me.DepthPixel(depthFrame.PixelDataLength - 1)
                            ReDim ColorImagePixelPoint(depthFrame.PixelDataLength - 1)
                        End If

                        ' 描画を3フレームに1回にする
                        If depthFrame.FrameNumber Mod 3 <> 0 Then
                            Exit Sub
                        End If
                        depthFrame.CopyPixelDataTo(DepthPixel)

                        ' Depthデータの座標をRGB画像の座標に変換する
                        Me.Kinect.MapDepthFrameToColorFrame(Me.Kinect.DepthStream.Format,
                                                            Me.DepthPixel,
                                                            Me.Kinect.ColorStream.Format,
                                                            Me.ColorImagePixelPoint)

                        ' カメラ画像の描画
                        Dim colorPixel(colorFrame.PixelDataLength - 1) As Byte
                        colorFrame.CopyPixelDataTo(colorPixel)

                        ' RGB画像の位置を距離画像の位置に補正
                        colorPixel = CoordinateColorImage(ColorImagePixelPoint, colorPixel)

                        Me.CameraImage.Source = BitmapSource.Create(colorFrame.Width,
                                                                    colorFrame.Height,
                                                                    96,
                                                                    96,
                                                                    PixelFormats.Bgr32,
                                                                    Nothing,
                                                                    colorPixel,
                                                                    colorFrame.Width * colorFrame.BytesPerPixel)

                    End If
                End Using
            End Using

            ' モードに応じた処理
            Select Case Me.CurrentMode
                Case SelectMode.SELECTING
                    ' 領域を指定中ならば描画も更新
                    Call UpdateRectPosition()
                Case SelectMode.SELECTED
                    ' 領域内を触っているかチェック
                    Dim _point As Point = CheckThePointTouchingTheRegion()
                    Call UpdateTouchingPointEllipse(_point)
                    Call UpdatePaintCanvas(_point)
            End Select
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 選択領域を表すRectangleの描画を更新
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub UpdateRectPosition()
        ' 現在のマウスの位置を取得
        Dim currentPoint As Point = Mouse.GetPosition(CameraImage)

        Dim _rect As Rect
        ' 始点と終点の位置によって値を変更
        If currentPoint.X < StartPointOfRect.X AndAlso currentPoint.Y < StartPointOfRect.Y Then
            ' 終点がスタート位置の左上
            _rect = New Rect(currentPoint.X,
                             currentPoint.Y,
                             Math.Abs(StartPointOfRect.X - currentPoint.X),
                             Math.Abs(StartPointOfRect.Y - currentPoint.Y)
                             )
        ElseIf currentPoint.X < StartPointOfRect.X Then
            ' 終点がスタート位置の左下
            _rect = New Rect(currentPoint.X,
                             StartPointOfRect.Y,
                             Math.Abs(StartPointOfRect.X - currentPoint.X),
                             Math.Abs(StartPointOfRect.Y - currentPoint.Y)
                             )
        ElseIf currentPoint.Y < StartPointOfRect.Y Then
            ' 終点がスタート位置の右上
            _rect = New Rect(StartPointOfRect.X,
                             currentPoint.Y,
                             Math.Abs(StartPointOfRect.X - currentPoint.X),
                             Math.Abs(StartPointOfRect.Y - currentPoint.Y)
                             )

        Else
            ' 終点がスタート位置の右下
            _rect = New Rect(StartPointOfRect.X,
                             StartPointOfRect.Y,
                             Math.Abs(StartPointOfRect.X - currentPoint.X),
                             Math.Abs(StartPointOfRect.Y - currentPoint.Y)
                             )
        End If

        ' Rectangleの配置
        Canvas.SetLeft(SelectRectangle, _rect.X)
        Canvas.SetTop(SelectRectangle, _rect.Y)
        SelectRectangle.Width = _rect.Width
        SelectRectangle.Height = _rect.Height

        ' 選択領域の保存
        SelectRegion = _rect
    End Sub

    ''' <summary>
    ''' タッチしている所を表すEllipse(円)の描画を更新
    ''' </summary>
    ''' <param name="p"></param>
    ''' <remarks></remarks>
    Private Sub UpdateTouchingPointEllipse(p As Point)
        Me.TouchPoint.Width = 20
        Me.TouchPoint.Height = 20
        Canvas.SetLeft(Me.TouchPoint, p.X - Me.TouchPoint.Width / 2)
        Canvas.SetTop(Me.TouchPoint, p.Y - Me.TouchPoint.Height / 2)
    End Sub

    ''' <summary>
    ''' ペイント用ウィンドウの更新
    ''' </summary>
    ''' <param name="p"></param>
    ''' <remarks></remarks>
    Private Sub UpdatePaintCanvas(p As Point)
        ' 座標変化なし・ 初期値やエラー値は除く
        If p.X = PreTouchPoint.X AndAlso p.Y = PreTouchPoint.Y Then
            ' タッチ座標の変化なし
        ElseIf (p.X = ERROR_OF_POINT AndAlso p.Y = ERROR_OF_POINT) OrElse
            (PreTouchPoint.X = ERROR_OF_POINT AndAlso PreTouchPoint.Y = ERROR_OF_POINT) Then
            ' 現在のフレームでタッチはされていない orelse
            ' 前フレームでタッチはされていない
            PreTouchPoint = p
        Else
            ' 線を引く
            If Me._PaintWindow IsNot Nothing Then
                Me._PaintWindow.DrawLine(PreTouchPoint, p)
            End If
            PreTouchPoint = p
        End If
    End Sub

    ''' <summary>
    ''' RGBカメラからの画像を距離カメラの画像の位置に合わせる
    ''' </summary>
    ''' <param name="points">RGB画像と距離の対応付けのデータ</param>
    ''' <param name="colorPixels">RGB画像のバイト配列</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CoordinateColorImage(points As ColorImagePoint(), colorPixels As Byte()) As Byte()
        Dim colorStream As ColorImageStream = Me.Kinect.ColorStream

        ' 出力バッファ(初期値はRGBカメラの画像)
        Dim outputColor(colorPixels.Length - 1) As Byte
        For i As Integer = 0 To outputColor.Length - 1 Step Bgr32BytesPerPixel
            outputColor(i + 0) = colorPixels(i + 0)
            outputColor(i + 1) = colorPixels(i + 1)
            outputColor(i + 2) = colorPixels(i + 2)
        Next

        For index As Integer = 0 To DepthPixel.Length - 1
            ' 変換した結果が、フレームサイズを超えることがあるため、小さいほうを使う
            Dim x As Integer = Math.Min(points(index).X, colorStream.FrameWidth - 1)
            Dim y As Integer = Math.Min(points(index).Y, colorStream.FrameHeight - 1)
            Dim colorIndex As Integer = ((y * Kinect.DepthStream.FrameWidth) + x) * Bgr32BytesPerPixel
            Dim outputIndex As Integer = index * Bgr32BytesPerPixel

            ' カラー画像のピクセルを調整された座標値に変換する
            outputColor(outputIndex + 0) = colorPixels(colorIndex + 0)
            outputColor(outputIndex + 1) = colorPixels(colorIndex + 1)
            outputColor(outputIndex + 2) = colorPixels(colorIndex + 2)
        Next
        Return outputColor
    End Function

    ''' <summary>
    ''' 指定された領域内の現在の距離情報を保存する
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub SaveBackgroundDepth()
        Dim depthStream As DepthImageStream = Me.Kinect.DepthStream

        ReDim Me.BackgroundDepthData(CType(SelectRegion.Width * SelectRegion.Height, Integer) - 1)

        Dim counter As Integer = 0
        For y As Integer = CType(SelectRegion.Y, Integer) To SelectRegion.Y + SelectRegion.Height - 1
            For x As Integer = CType(SelectRegion.X, Integer) To SelectRegion.X + SelectRegion.Width - 1
                Me.BackgroundDepthData(counter) = CType(DepthPixel(y * depthStream.FrameWidth + x) >> DepthImageFrame.PlayerIndexBitmaskWidth, Short)
                counter += 1
            Next
        Next
    End Sub

    ''' <summary>
    ''' タッチ判定・座標の取得
    ''' </summary>
    ''' <returns>タッチ座標</returns>
    ''' <remarks></remarks>
    Private Function CheckThePointTouchingTheRegion() As Point
        Dim depthStream As DepthImageStream = Me.Kinect.DepthStream

        Dim distanceThreshold As Integer = 30                 ' BackgroundDepthDataとの最大誤差値
        Dim distanceBetweenWallThreshold As Integer = 20      ' BackgroundDepthDataとの最小誤差値
        Dim touchPoints As New List(Of Point)()
        Dim rePoint As New Point(ERROR_OF_POINT, ERROR_OF_POINT)  ' 返却用座標

        ' 深度の変化のポイントをピクセル毎に探査
        Dim counter As Integer = 0
        For y As Integer = CType(SelectRegion.Y, Integer) To SelectRegion.Y + SelectRegion.Height - 1
            For x As Integer = CType(SelectRegion.X, Integer) To SelectRegion.X + SelectRegion.Width - 1
                Dim currentDepthVal As Short = CType(DepthPixel(y * depthStream.FrameWidth + x) >> DepthImageFrame.PlayerIndexBitmaskWidth, Short)

                ' 保存したデプスマップより深度が近くなっておりかつその深度の変化が
                ' distanceThreshold と distanceBetweenWallThreshold 内であるポイントを探査
                If BackgroundDepthData(counter) > currentDepthVal AndAlso
                             (BackgroundDepthData(counter) - currentDepthVal) < distanceThreshold AndAlso
                             (BackgroundDepthData(counter) - currentDepthVal) > distanceBetweenWallThreshold Then
                    touchPoints.Add(New Point(x, y))
                End If
                counter += 1
            Next
        Next

        ' 検出した変化Pointから重心を計算
        Dim numThreshold As Integer = 50      ' 変化を検出したポイントの閾値

        If touchPoints.Count > numThreshold Then
            Dim xSum As Double = 0
            Dim ySum As Double = 0
            For Each p As Point In touchPoints
                xSum += p.X
                ySum += p.Y
            Next

            ' タッチされた座標を設定
            rePoint.X = xSum / touchPoints.Count
            rePoint.Y = ySum / touchPoints.Count
        End If

        Return rePoint
    End Function

    ''' <summary>
    ''' マウスの左クリックが押された時のイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Window_MouseLeftButtonDown(sender As System.Object,
                                           e As System.Windows.Input.MouseButtonEventArgs)

        If e.LeftButton = MouseButtonState.Pressed Then
            ' 始点を保存
            StartPointOfRect = e.MouseDevice.GetPosition(SelectCanvas)

            CurrentMode = SelectMode.SELECTING
        End If
    End Sub

    ''' <summary>
    ''' マウスの左クリックが離された時のイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub Window_MouseLeftButtonUp(sender As System.Object,
                                           e As System.Windows.Input.MouseButtonEventArgs)

        If e.LeftButton = MouseButtonState.Released AndAlso CurrentMode = SelectMode.SELECTING Then
            ' 現在の距離情報を追加
            SaveBackgroundDepth()

            Me.CurrentMode = SelectMode.SELECTED
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

    ''' <summary>
    ''' StartButtonコントロールが押された時のイベント
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub StartButton_Click(sender As System.Object,
                                  e As RoutedEventArgs)
        If CurrentMode = SelectMode.SELECTED Then
            ' ペイント用ウィンドウの作成・表示
            Me._PaintWindow = New PaintWindow()
            Me._PaintWindow.SetSelectedRegion(SelectRegion)
            Me._PaintWindow.Show()
        End If
    End Sub
End Class
