namespace Hisui.Gui
{
  partial class StoragePropertyControl
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

    #region コンポーネント デザイナで生成されたコード

    /// <summary> 
    /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
    /// コード エディタで変更しないでください。
    /// </summary>
    private void InitializeComponent()
    {
      this.label1 = new System.Windows.Forms.Label();
      this.labelCount = new System.Windows.Forms.Label();
      this.txtID = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.panelPropertyControl = new System.Windows.Forms.Panel();
      this.label3 = new System.Windows.Forms.Label();
      this.txtValue = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point( 13, 11 );
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size( 35, 12 );
      this.label1.TabIndex = 0;
      this.label1.Text = "count:";
      // 
      // labelCount
      // 
      this.labelCount.AutoSize = true;
      this.labelCount.Location = new System.Drawing.Point( 54, 11 );
      this.labelCount.Name = "labelCount";
      this.labelCount.Size = new System.Drawing.Size( 11, 12 );
      this.labelCount.TabIndex = 1;
      this.labelCount.Text = "0";
      // 
      // txtID
      // 
      this.txtID.Location = new System.Drawing.Point( 35, 36 );
      this.txtID.Name = "txtID";
      this.txtID.Size = new System.Drawing.Size( 41, 19 );
      this.txtID.TabIndex = 2;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point( 13, 39 );
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size( 16, 12 );
      this.label2.TabIndex = 3;
      this.label2.Text = "ID";
      // 
      // panelPropertyControl
      // 
      this.panelPropertyControl.Location = new System.Drawing.Point( 15, 61 );
      this.panelPropertyControl.Name = "panelPropertyControl";
      this.panelPropertyControl.Size = new System.Drawing.Size( 242, 222 );
      this.panelPropertyControl.TabIndex = 4;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point( 96, 39 );
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size( 34, 12 );
      this.label3.TabIndex = 5;
      this.label3.Text = "Value";
      // 
      // txtValue
      // 
      this.txtValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.txtValue.Location = new System.Drawing.Point( 136, 36 );
      this.txtValue.Name = "txtValue";
      this.txtValue.ReadOnly = true;
      this.txtValue.Size = new System.Drawing.Size( 121, 19 );
      this.txtValue.TabIndex = 6;
      this.txtValue.TabStop = false;
      // 
      // StoragePropertyControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add( this.txtValue );
      this.Controls.Add( this.label3 );
      this.Controls.Add( this.panelPropertyControl );
      this.Controls.Add( this.label2 );
      this.Controls.Add( this.txtID );
      this.Controls.Add( this.labelCount );
      this.Controls.Add( this.label1 );
      this.Name = "StoragePropertyControl";
      this.Size = new System.Drawing.Size( 272, 297 );
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label labelCount;
    private System.Windows.Forms.TextBox txtID;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Panel panelPropertyControl;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox txtValue;
  }
}
