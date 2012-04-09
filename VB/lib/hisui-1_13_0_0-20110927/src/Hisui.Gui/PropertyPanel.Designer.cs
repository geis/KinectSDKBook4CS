namespace Hisui.Gui
{
  partial class PropertyPanel
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
      this.tabProperties = new System.Windows.Forms.TabControl();
      this.labelCaption = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // tabProperties
      // 
      this.tabProperties.Alignment = System.Windows.Forms.TabAlignment.Bottom;
      this.tabProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.tabProperties.Location = new System.Drawing.Point( 3, 24 );
      this.tabProperties.Name = "tabProperties";
      this.tabProperties.SelectedIndex = 0;
      this.tabProperties.Size = new System.Drawing.Size( 235, 265 );
      this.tabProperties.TabIndex = 0;
      // 
      // labelCaption
      // 
      this.labelCaption.AutoSize = true;
      this.labelCaption.Location = new System.Drawing.Point( 5, 7 );
      this.labelCaption.Name = "labelCaption";
      this.labelCaption.Size = new System.Drawing.Size( 35, 12 );
      this.labelCaption.TabIndex = 1;
      this.labelCaption.Text = "label1";
      // 
      // PropertyPanel
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add( this.labelCaption );
      this.Controls.Add( this.tabProperties );
      this.Name = "PropertyPanel";
      this.Size = new System.Drawing.Size( 241, 292 );
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TabControl tabProperties;
    private System.Windows.Forms.Label labelCaption;
  }
}
