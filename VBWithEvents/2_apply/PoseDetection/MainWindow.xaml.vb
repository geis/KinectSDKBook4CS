Imports System
Imports System.Windows
Imports System.Windows.Media
Imports System.Windows.Media.Imaging

Imports Microsoft.Kinect
Imports System.Windows.Shapes

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

        ' RGBカメラを有効にする
        Me.KinectEvents.ColorStream.Enable()

        ' スケルトンを有効にする
        Me.KinectEvents.SkeletonStream.Enable()

        ' Kinectの動作を開始する
        Me.KinectEvents.Start()
    End Sub

    ''' <summary>
    ''' Kinectの動作を停止する
    ''' </summary>
    ''' <param name="kinect"></param>
    ''' <remarks></remarks>
    Private Sub StopKinect(sensor As KinectSensor)
        If Me.KinectEvents IsNot Nothing Then
            If Me.KinectEvents.IsRunning Then
                ' Kinectの停止と、ネイティブリソースを解放する
                Me.KinectEvents.Stop()
                Me.KinectEvents.Dispose()

                Me.RGBCameraImage.Source = Nothing
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
                    Me.RGBCameraImage.Source = BitmapSource.Create(colorFrame.Width,
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
    Private Sub kinect_SkeletonFrameReady(sender As Object,
                                          e As SkeletonFrameReadyEventArgs) _
                                      Handles KinectEvents.SkeletonFrameReady
        Try
            ' Kinectのインスタンスを取得する
            Dim sensor As KinectSensor = CType(sender, KinectSensor)
            If sensor Is Nothing Then
                Exit Sub
            End If

            ' スケルトンのフレームを取得する
            Using _skeletionFrame As SkeletonFrame = e.OpenSkeletonFrame
                If _skeletionFrame IsNot Nothing Then
                    Call DrawCrossPoint(sensor, _skeletionFrame)
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' 交差点の描画
    ''' </summary>
    ''' <param name="kinect"></param>
    ''' <param name="_skeletonFrame"></param>
    ''' <remarks></remarks>
    Private Sub DrawCrossPoint(sensor As KinectSensor,
                               _skeletonFrame As SkeletonFrame)
        ' 描画する円の半径
        Const R As Integer = 5

        ' キャンバスをクリアする
        canvas.Children.Clear()

        ' スケルトンから4か所の座標を取得（左肘・手、右肘・手）
        Dim joints As Joint() = GetFourSkeletonPosition(_skeletonFrame,
                                                        JointType.ElbowLeft,
                                                        JointType.HandLeft,
                                                        JointType.ElbowRight,
                                                        JointType.HandRight)
        If joints Is Nothing Then
            Exit Sub
        End If

        ' スケルトン座標をRGB画像の座標に変換し表示する
        Dim jointImagePosition(4 - 1)
        For i As Integer = 0 To 4 - 1
            jointImagePosition(i) = sensor.MapSkeletonPointToColor(joints(i).Position,
                                                                   sensor.ColorStream.Format)

            canvas.Children.Add(New Ellipse() With
            {
              .Fill = New SolidColorBrush(Colors.Yellow),
              .Margin = New Thickness(jointImagePosition(i).X - R,
                                      jointImagePosition(i).Y - R,
                                      0,
                                      0),
              .Width = R * 2,
              .Height = R * 2
            })
        Next

        ' 腕がクロスしているかチェック
        Dim isCross As Boolean = CrossHitCheck(joints(0).Position,
                                               joints(1).Position,
                                               joints(2).Position,
                                               joints(3).Position)
        If isCross Then
            ' クロスしている点を計算して円を表示する
            Dim crossPoint As ColorImagePoint = GetCrossPoint(jointImagePosition(0),
                                                              jointImagePosition(1),
                                                              jointImagePosition(2),
                                                              jointImagePosition(3))

            Me.CrossEllipse.Margin = New Thickness(crossPoint.X - CrossEllipse.Width / 2,
                                                   crossPoint.Y - CrossEllipse.Height / 2,
                                                   0,
                                                   0)
            Me.CrossEllipse.Visibility = System.Windows.Visibility.Visible
            Me.canvas.Children.Add(CrossEllipse)
        Else
            Me.CrossEllipse.Visibility = System.Windows.Visibility.Hidden
        End If
    End Sub

    ''' <summary>
    ''' 指定した4か所のスケルトンジョイントを取得する
    ''' </summary>
    ''' <param name="_skeletonFrame"></param>
    ''' <param name="pos1"></param>
    ''' <param name="pos2"></param>
    ''' <param name="pos3"></param>
    ''' <param name="pos4"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function GetFourSkeletonPosition(_skeletonFrame As SkeletonFrame,
                                             pos1 As JointType,
                                             pos2 As JointType,
                                             pos3 As JointType,
                                             pos4 As JointType) As Joint()

        Dim jointTypes() = New JointType() {pos1, pos2, pos3, pos4}

        ' スケルトンのデータを取得する
        Dim skeletons(_skeletonFrame.SkeletonArrayLength - 1) As Skeleton
        _skeletonFrame.CopySkeletonDataTo(skeletons)

        ' トラッキングされているスケルトンのジョイントをする
        For Each _skeleton As Skeleton In skeletons
            ' スケルトンがトラッキング状態(デフォルトモードの)の場合は、ジョイントを描画する
            If _skeleton.TrackingState = SkeletonTrackingState.Tracked Then
                ' ジョイントを抽出する
                Dim joints(4 - 1) As Joint
                For i As Integer = 0 To 4 - 1
                    ' 指定のジョイントがトラッキングされてない場合はnullを返す
                    If _skeleton.Joints(jointTypes(i)).TrackingState = JointTrackingState.NotTracked Then
                        Return Nothing
                    End If
                    joints(i) = _skeleton.Joints(jointTypes(i))
                Next

                Return joints
            End If
        Next

        Return Nothing
    End Function

    ''' <summary>
    ''' 2直線が交差しているか調べる
    ''' </summary>
    ''' <param name="a1"></param>
    ''' <param name="a2"></param>
    ''' <param name="b1"></param>
    ''' <param name="b2"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CrossHitCheck(a1 As SkeletonPoint,
                                   a2 As SkeletonPoint,
                                   b1 As SkeletonPoint,
                                   b2 As SkeletonPoint) As Boolean
        ' 外積：ax * by - ay * bx
        ' 外積を使用して交差判定を行う
        Dim v1 As Double = (a2.X - a1.X) * (b1.Y - a1.Y) - (a2.Y - a1.Y) * (b1.X - a1.X)
        Dim v2 As Double = (a2.X - a1.X) * (b2.Y - a1.Y) - (a2.Y - a1.Y) * (b2.X - a1.X)
        Dim m1 As Double = (b2.X - b1.X) * (a1.Y - b1.Y) - (b2.Y - b1.Y) * (a1.X - b1.X)
        Dim m2 As Double = (b2.X - b1.X) * (a2.Y - b1.Y) - (b2.Y - b1.Y) * (a2.X - b1.X)

        ' +-, -+だったらマイナス値になるのでそれぞれをかけて確認する
        ' 二つとも左右にあった場合は交差している
        If (v1 * v2 <= 0) AndAlso (m1 * m2 <= 0) Then
            Return True
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' 2直線の交差点を計算する
    ''' </summary>
    ''' <param name="a1">線分aの始点</param>
    ''' <param name="a2">線分aの終点</param>
    ''' <param name="b1">線分bの始点</param>
    ''' <param name="b2">線分bの終点</param>
    ''' <returns>交差点の座標</returns>
    ''' <remarks></remarks>
    Private Function GetCrossPoint(a1 As ColorImagePoint,
                                   a2 As ColorImagePoint,
                                   b1 As ColorImagePoint,
                                   b2 As ColorImagePoint) As ColorImagePoint

        ' 1つめの式
        Dim v1a As Single = (a1.Y - a2.Y) / CType((a1.X - a2.X), Single)
        Dim v1b As Single = (a1.X * a2.Y - a1.Y * a2.X) / CType((a1.X - a2.X), Single)

        ' 2つめの式
        Dim v2a As Single = (b1.Y - b2.Y) / CType((b1.X - b2.X), Single)
        Dim v2b As Single = (b1.X * b2.Y - b1.Y * b2.X) / CType((b1.X - b2.X), Single)

        ' 最終的な交点を返す
        Dim crossPoint = New ColorImagePoint()
        crossPoint.X = CType((v2b - v1b) / CType((v1a - v2a), Single), Integer)
        crossPoint.Y = CType((v1a * CType(crossPoint.X, Single) + v1b), Integer)
        Return crossPoint
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
