<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.ComboBoxRange = New System.Windows.Forms.ComboBox()
        Me.PictureBoxDepth = New System.Windows.Forms.PictureBox()
        Me.PictureBoxRgb = New System.Windows.Forms.PictureBox()
        CType(Me.PictureBoxDepth, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBoxRgb, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ComboBoxRange
        '
        Me.ComboBoxRange.FormattingEnabled = True
        Me.ComboBoxRange.Location = New System.Drawing.Point(12, 12)
        Me.ComboBoxRange.Name = "ComboBoxRange"
        Me.ComboBoxRange.Size = New System.Drawing.Size(121, 20)
        Me.ComboBoxRange.TabIndex = 0
        '
        'PictureBoxDepth
        '
        Me.PictureBoxDepth.Location = New System.Drawing.Point(658, 51)
        Me.PictureBoxDepth.Name = "PictureBoxDepth"
        Me.PictureBoxDepth.Size = New System.Drawing.Size(640, 480)
        Me.PictureBoxDepth.TabIndex = 4
        Me.PictureBoxDepth.TabStop = False
        '
        'PictureBoxRgb
        '
        Me.PictureBoxRgb.Location = New System.Drawing.Point(12, 51)
        Me.PictureBoxRgb.Name = "PictureBoxRgb"
        Me.PictureBoxRgb.Size = New System.Drawing.Size(640, 480)
        Me.PictureBoxRgb.TabIndex = 3
        Me.PictureBoxRgb.TabStop = False
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1020, 505)
        Me.Controls.Add(Me.ComboBoxRange)
        Me.Controls.Add(Me.PictureBoxDepth)
        Me.Controls.Add(Me.PictureBoxRgb)
        Me.Name = "Form1"
        Me.Text = "WindowsForms"
        CType(Me.PictureBoxDepth, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBoxRgb, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents ComboBoxRange As System.Windows.Forms.ComboBox
    Private WithEvents PictureBoxDepth As System.Windows.Forms.PictureBox
    Private WithEvents PictureBoxRgb As System.Windows.Forms.PictureBox

End Class
