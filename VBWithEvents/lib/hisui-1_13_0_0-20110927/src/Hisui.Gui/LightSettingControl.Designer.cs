namespace Hisui.Gui
{
  partial class LightSettingControl
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
      this.glViewControl1 = new Hisui.Graphics.GLViewControl();
      this.checkBox1 = new System.Windows.Forms.CheckBox();
      this.button1 = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // glViewControl1
      // 
      this.glViewControl1.Location = new System.Drawing.Point( 3, 32 );
      this.glViewControl1.Name = "glViewControl1";
      this.glViewControl1.Size = new System.Drawing.Size( 247, 203 );
      this.glViewControl1.TabIndex = 0;
      this.glViewControl1.Text = "glViewControl1";
      // 
      // checkBox1
      // 
      this.checkBox1.AutoSize = true;
      this.checkBox1.Location = new System.Drawing.Point( 3, 3 );
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.Size = new System.Drawing.Size( 80, 16 );
      this.checkBox1.TabIndex = 1;
      this.checkBox1.Text = "checkBox1";
      this.checkBox1.UseVisualStyleBackColor = true;
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point( 89, 3 );
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size( 75, 23 );
      this.button1.TabIndex = 2;
      this.button1.Text = "button1";
      this.button1.UseVisualStyleBackColor = true;
      // 
      // LightSettingControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add( this.button1 );
      this.Controls.Add( this.checkBox1 );
      this.Controls.Add( this.glViewControl1 );
      this.Name = "LightSettingControl";
      this.Size = new System.Drawing.Size( 253, 238 );
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private Hisui.Graphics.GLViewControl glViewControl1;
    private System.Windows.Forms.CheckBox checkBox1;
    private System.Windows.Forms.Button button1;
  }
}
