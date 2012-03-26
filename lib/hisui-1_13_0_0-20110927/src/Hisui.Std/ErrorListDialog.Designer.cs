namespace Hisui.Std
{
  partial class ErrorListDialog
  {
    /// <summary>
    /// 必要なデザイナ変数です。
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// 使用中のリソースをすべてクリーンアップします。
    /// </summary>
    /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
    protected override void Dispose( bool disposing )
    {
      if ( disposing && (components != null) ) {
        components.Dispose();
      }
      base.Dispose( disposing );
    }

    #region Windows フォーム デザイナで生成されたコード

    /// <summary>
    /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
    /// コード エディタで変更しないでください。
    /// </summary>
    private void InitializeComponent()
    {
      this.listExceptions = new System.Windows.Forms.CheckedListBox();
      this.btnClose = new System.Windows.Forms.Button();
      this.labelExceptionType = new System.Windows.Forms.Label();
      this.labelMessage = new System.Windows.Forms.Label();
      this.gridExceptionProperty = new System.Windows.Forms.PropertyGrid();
      this.btnSelectAll = new System.Windows.Forms.Button();
      this.btnClearSelection = new System.Windows.Forms.Button();
      this.btnPut = new System.Windows.Forms.Button();
      this.labelReporter = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // listExceptions
      // 
      this.listExceptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)));
      this.listExceptions.FormattingEnabled = true;
      this.listExceptions.Location = new System.Drawing.Point( 12, 12 );
      this.listExceptions.Name = "listExceptions";
      this.listExceptions.Size = new System.Drawing.Size( 196, 284 );
      this.listExceptions.TabIndex = 0;
      this.listExceptions.SelectedIndexChanged += new System.EventHandler( this.listExceptions_SelectedIndexChanged );
      // 
      // btnClose
      // 
      this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnClose.Location = new System.Drawing.Point( 411, 303 );
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size( 75, 23 );
      this.btnClose.TabIndex = 1;
      this.btnClose.Text = "閉じる";
      this.btnClose.UseVisualStyleBackColor = true;
      // 
      // labelExceptionType
      // 
      this.labelExceptionType.AutoSize = true;
      this.labelExceptionType.Location = new System.Drawing.Point( 214, 58 );
      this.labelExceptionType.Name = "labelExceptionType";
      this.labelExceptionType.Size = new System.Drawing.Size( 55, 12 );
      this.labelExceptionType.TabIndex = 2;
      this.labelExceptionType.Text = "例外タイプ";
      // 
      // labelMessage
      // 
      this.labelMessage.AutoSize = true;
      this.labelMessage.Location = new System.Drawing.Point( 214, 33 );
      this.labelMessage.Name = "labelMessage";
      this.labelMessage.Size = new System.Drawing.Size( 50, 12 );
      this.labelMessage.TabIndex = 3;
      this.labelMessage.Text = "メッセージ";
      // 
      // gridExceptionProperty
      // 
      this.gridExceptionProperty.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.gridExceptionProperty.Location = new System.Drawing.Point( 216, 73 );
      this.gridExceptionProperty.Name = "gridExceptionProperty";
      this.gridExceptionProperty.Size = new System.Drawing.Size( 270, 223 );
      this.gridExceptionProperty.TabIndex = 6;
      // 
      // btnSelectAll
      // 
      this.btnSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnSelectAll.Location = new System.Drawing.Point( 12, 303 );
      this.btnSelectAll.Name = "btnSelectAll";
      this.btnSelectAll.Size = new System.Drawing.Size( 50, 23 );
      this.btnSelectAll.TabIndex = 7;
      this.btnSelectAll.Text = "全選択";
      this.btnSelectAll.UseVisualStyleBackColor = true;
      this.btnSelectAll.Click += new System.EventHandler( this.btnSelectAll_Click );
      // 
      // btnClearSelection
      // 
      this.btnClearSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnClearSelection.Location = new System.Drawing.Point( 68, 303 );
      this.btnClearSelection.Name = "btnClearSelection";
      this.btnClearSelection.Size = new System.Drawing.Size( 50, 23 );
      this.btnClearSelection.TabIndex = 8;
      this.btnClearSelection.Text = "解除";
      this.btnClearSelection.UseVisualStyleBackColor = true;
      this.btnClearSelection.Click += new System.EventHandler( this.btnClearSelection_Click );
      // 
      // btnPut
      // 
      this.btnPut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.btnPut.Location = new System.Drawing.Point( 124, 303 );
      this.btnPut.Name = "btnPut";
      this.btnPut.Size = new System.Drawing.Size( 84, 23 );
      this.btnPut.TabIndex = 9;
      this.btnPut.Text = "登録";
      this.btnPut.UseVisualStyleBackColor = true;
      this.btnPut.Click += new System.EventHandler( this.btnPut_Click );
      // 
      // labelReporter
      // 
      this.labelReporter.AutoSize = true;
      this.labelReporter.Location = new System.Drawing.Point( 214, 12 );
      this.labelReporter.Name = "labelReporter";
      this.labelReporter.Size = new System.Drawing.Size( 52, 12 );
      this.labelReporter.TabIndex = 10;
      this.labelReporter.Text = "レポーター";
      // 
      // ErrorListDialog
      // 
      this.AcceptButton = this.btnClose;
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size( 498, 338 );
      this.Controls.Add( this.labelReporter );
      this.Controls.Add( this.btnPut );
      this.Controls.Add( this.btnClearSelection );
      this.Controls.Add( this.btnSelectAll );
      this.Controls.Add( this.gridExceptionProperty );
      this.Controls.Add( this.labelMessage );
      this.Controls.Add( this.labelExceptionType );
      this.Controls.Add( this.btnClose );
      this.Controls.Add( this.listExceptions );
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Name = "ErrorListDialog";
      this.Text = "エラーリスト";
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.CheckedListBox listExceptions;
    private System.Windows.Forms.Button btnClose;
    private System.Windows.Forms.Label labelExceptionType;
    private System.Windows.Forms.Label labelMessage;
    private System.Windows.Forms.PropertyGrid gridExceptionProperty;
    private System.Windows.Forms.Button btnSelectAll;
    private System.Windows.Forms.Button btnClearSelection;
    private System.Windows.Forms.Button btnPut;
    private System.Windows.Forms.Label labelReporter;
  }
}