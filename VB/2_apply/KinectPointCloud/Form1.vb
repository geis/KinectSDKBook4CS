Public Class Form1
    Private View = New Hisui.Graphics.GLViewControl()

    Public Sub New()
        InitializeComponent()

        ' ビューをフォームに配置
        Me.View.Dock = DockStyle.Fill
        Me.Controls.Add(Me.View)

        ' ビューを DocumentViews に設定し、ビルドグラフに組み込む
        Hisui.SI.DocumentViews.AddView(Me.View)
        Hisui.SI.Tasks.Add(Hisui.SI.DocumentViews)

        ' シーンを起動する
        Hisui.SI.View.SceneGraph.WorldScenes.Add(New PointCloudScene())
    End Sub
End Class
