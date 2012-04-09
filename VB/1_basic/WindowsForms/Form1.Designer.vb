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
        Me.comboBoxRange = New System.Windows.Forms.ComboBox()
        Me.pictureBoxDepth = New System.Windows.Forms.PictureBox()
        Me.pictureBoxRgb = New System.Windows.Forms.PictureBox()
        CType(Me.pictureBoxDepth, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pictureBoxRgb, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'comboBoxRange
        '
        Me.comboBoxRange.FormattingEnabled = True
        Me.comboBoxRange.Location = New System.Drawing.Point(12, 12)
        Me.comboBoxRange.Name = "comboBoxRange"
        Me.comboBoxRange.Size = New System.Drawing.Size(121, 20)
        Me.comboBoxRange.TabIndex = 0
        '
        'pictureBoxDepth
        '
        Me.pictureBoxDepth.Location = New System.Drawing.Point(658, 51)
        Me.pictureBoxDepth.Name = "pictureBoxDepth"
        Me.pictureBoxDepth.Size = New System.Drawing.Size(640, 480)
        Me.pictureBoxDepth.TabIndex = 4
        Me.pictureBoxDepth.TabStop = False
        '
        'pictureBoxRgb
        '
        Me.pictureBoxRgb.Location = New System.Drawing.Point(12, 51)
        Me.pictureBoxRgb.Name = "pictureBoxRgb"
        Me.pictureBoxRgb.Size = New System.Drawing.Size(640, 480)
        Me.pictureBoxRgb.TabIndex = 3
        Me.pictureBoxRgb.TabStop = False
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1020, 505)
        Me.Controls.Add(Me.comboBoxRange)
        Me.Controls.Add(Me.pictureBoxDepth)
        Me.Controls.Add(Me.pictureBoxRgb)
        Me.Name = "Form1"
        Me.Text = "WindowsForms"
        CType(Me.pictureBoxDepth, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pictureBoxRgb, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents comboBoxRange As System.Windows.Forms.ComboBox
    Private WithEvents pictureBoxDepth As System.Windows.Forms.PictureBox
    Private WithEvents pictureBoxRgb As System.Windows.Forms.PictureBox

End Class
