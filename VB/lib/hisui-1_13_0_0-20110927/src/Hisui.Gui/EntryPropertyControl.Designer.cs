namespace Hisui.Gui
{
  partial class EntryPropertyControl
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
      this.checkEnabled = new System.Windows.Forms.CheckBox();
      this.checkMaterial = new System.Windows.Forms.CheckBox();
      this.checkPointSize = new System.Windows.Forms.CheckBox();
      this.numPointSize = new System.Windows.Forms.NumericUpDown();
      this.btnMaterial = new System.Windows.Forms.Button();
      this.panel1 = new System.Windows.Forms.Panel();
      this.panel2 = new System.Windows.Forms.Panel();
      this.panel3 = new System.Windows.Forms.Panel();
      this.checkLineWidth = new System.Windows.Forms.CheckBox();
      this.numLineWidth = new System.Windows.Forms.NumericUpDown();
      this.panel4 = new System.Windows.Forms.Panel();
      this.comboLineStipple = new System.Windows.Forms.ComboBox();
      this.checkLineStipple = new System.Windows.Forms.CheckBox();
      this.panel5 = new System.Windows.Forms.Panel();
      this.comboShader = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.numPointSize)).BeginInit();
      this.panel1.SuspendLayout();
      this.panel2.SuspendLayout();
      this.panel3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numLineWidth)).BeginInit();
      this.panel4.SuspendLayout();
      this.panel5.SuspendLayout();
      this.SuspendLayout();
      // 
      // checkEnabled
      // 
      this.checkEnabled.AutoSize = true;
      this.checkEnabled.Location = new System.Drawing.Point( 9, 10 );
      this.checkEnabled.Name = "checkEnabled";
      this.checkEnabled.Size = new System.Drawing.Size( 78, 16 );
      this.checkEnabled.TabIndex = 0;
      this.checkEnabled.Text = "有効/無効";
      this.checkEnabled.UseVisualStyleBackColor = true;
      // 
      // checkMaterial
      // 
      this.checkMaterial.AutoSize = true;
      this.checkMaterial.Location = new System.Drawing.Point( 3, 7 );
      this.checkMaterial.Name = "checkMaterial";
      this.checkMaterial.Size = new System.Drawing.Size( 68, 16 );
      this.checkMaterial.TabIndex = 1;
      this.checkMaterial.Text = "マテリアル";
      this.checkMaterial.UseVisualStyleBackColor = true;
      // 
      // checkPointSize
      // 
      this.checkPointSize.AutoSize = true;
      this.checkPointSize.Location = new System.Drawing.Point( 3, 4 );
      this.checkPointSize.Name = "checkPointSize";
      this.checkPointSize.Size = new System.Drawing.Size( 65, 16 );
      this.checkPointSize.TabIndex = 2;
      this.checkPointSize.Text = "点サイズ";
      this.checkPointSize.UseVisualStyleBackColor = true;
      // 
      // numPointSize
      // 
      this.numPointSize.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.numPointSize.Enabled = false;
      this.numPointSize.Location = new System.Drawing.Point( 77, 3 );
      this.numPointSize.Name = "numPointSize";
      this.numPointSize.Size = new System.Drawing.Size( 128, 19 );
      this.numPointSize.TabIndex = 4;
      // 
      // btnMaterial
      // 
      this.btnMaterial.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.btnMaterial.Enabled = false;
      this.btnMaterial.Location = new System.Drawing.Point( 77, 3 );
      this.btnMaterial.Name = "btnMaterial";
      this.btnMaterial.Size = new System.Drawing.Size( 128, 23 );
      this.btnMaterial.TabIndex = 5;
      this.btnMaterial.Text = "...";
      this.btnMaterial.UseVisualStyleBackColor = true;
      // 
      // panel1
      // 
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel1.Controls.Add( this.checkMaterial );
      this.panel1.Controls.Add( this.btnMaterial );
      this.panel1.Location = new System.Drawing.Point( 9, 32 );
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size( 208, 29 );
      this.panel1.TabIndex = 7;
      // 
      // panel2
      // 
      this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel2.Controls.Add( this.checkPointSize );
      this.panel2.Controls.Add( this.numPointSize );
      this.panel2.Location = new System.Drawing.Point( 9, 67 );
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size( 208, 25 );
      this.panel2.TabIndex = 8;
      // 
      // panel3
      // 
      this.panel3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel3.Controls.Add( this.checkLineWidth );
      this.panel3.Controls.Add( this.numLineWidth );
      this.panel3.Location = new System.Drawing.Point( 9, 98 );
      this.panel3.Name = "panel3";
      this.panel3.Size = new System.Drawing.Size( 208, 25 );
      this.panel3.TabIndex = 9;
      // 
      // checkLineWidth
      // 
      this.checkLineWidth.AutoSize = true;
      this.checkLineWidth.Location = new System.Drawing.Point( 3, 4 );
      this.checkLineWidth.Name = "checkLineWidth";
      this.checkLineWidth.Size = new System.Drawing.Size( 66, 16 );
      this.checkLineWidth.TabIndex = 2;
      this.checkLineWidth.Text = "線の太さ";
      this.checkLineWidth.UseVisualStyleBackColor = true;
      // 
      // numLineWidth
      // 
      this.numLineWidth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.numLineWidth.Enabled = false;
      this.numLineWidth.Location = new System.Drawing.Point( 77, 3 );
      this.numLineWidth.Name = "numLineWidth";
      this.numLineWidth.Size = new System.Drawing.Size( 128, 19 );
      this.numLineWidth.TabIndex = 4;
      // 
      // panel4
      // 
      this.panel4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel4.Controls.Add( this.comboLineStipple );
      this.panel4.Controls.Add( this.checkLineStipple );
      this.panel4.Location = new System.Drawing.Point( 9, 129 );
      this.panel4.Name = "panel4";
      this.panel4.Size = new System.Drawing.Size( 208, 25 );
      this.panel4.TabIndex = 10;
      // 
      // comboLineStipple
      // 
      this.comboLineStipple.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.comboLineStipple.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboLineStipple.Enabled = false;
      this.comboLineStipple.FormattingEnabled = true;
      this.comboLineStipple.Location = new System.Drawing.Point( 76, 3 );
      this.comboLineStipple.Name = "comboLineStipple";
      this.comboLineStipple.Size = new System.Drawing.Size( 129, 20 );
      this.comboLineStipple.TabIndex = 3;
      // 
      // checkLineStipple
      // 
      this.checkLineStipple.AutoSize = true;
      this.checkLineStipple.Location = new System.Drawing.Point( 3, 4 );
      this.checkLineStipple.Name = "checkLineStipple";
      this.checkLineStipple.Size = new System.Drawing.Size( 48, 16 );
      this.checkLineStipple.TabIndex = 2;
      this.checkLineStipple.Text = "線種";
      this.checkLineStipple.UseVisualStyleBackColor = true;
      // 
      // panel5
      // 
      this.panel5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel5.Controls.Add( this.label1 );
      this.panel5.Controls.Add( this.comboShader );
      this.panel5.Location = new System.Drawing.Point( 9, 160 );
      this.panel5.Name = "panel5";
      this.panel5.Size = new System.Drawing.Size( 208, 27 );
      this.panel5.TabIndex = 11;
      // 
      // comboShader
      // 
      this.comboShader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.comboShader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboShader.FormattingEnabled = true;
      this.comboShader.Location = new System.Drawing.Point( 77, 3 );
      this.comboShader.Name = "comboShader";
      this.comboShader.Size = new System.Drawing.Size( 128, 20 );
      this.comboShader.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point( 16, 6 );
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size( 51, 12 );
      this.label1.TabIndex = 1;
      this.label1.Text = "シェーダー";
      // 
      // EntryPropertyControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add( this.panel5 );
      this.Controls.Add( this.panel4 );
      this.Controls.Add( this.panel3 );
      this.Controls.Add( this.panel2 );
      this.Controls.Add( this.panel1 );
      this.Controls.Add( this.checkEnabled );
      this.Name = "EntryPropertyControl";
      this.Size = new System.Drawing.Size( 226, 236 );
      ((System.ComponentModel.ISupportInitialize)(this.numPointSize)).EndInit();
      this.panel1.ResumeLayout( false );
      this.panel1.PerformLayout();
      this.panel2.ResumeLayout( false );
      this.panel2.PerformLayout();
      this.panel3.ResumeLayout( false );
      this.panel3.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numLineWidth)).EndInit();
      this.panel4.ResumeLayout( false );
      this.panel4.PerformLayout();
      this.panel5.ResumeLayout( false );
      this.panel5.PerformLayout();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.CheckBox checkEnabled;
    private System.Windows.Forms.CheckBox checkMaterial;
    private System.Windows.Forms.CheckBox checkPointSize;
    private System.Windows.Forms.NumericUpDown numPointSize;
    private System.Windows.Forms.Button btnMaterial;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.Panel panel3;
    private System.Windows.Forms.CheckBox checkLineWidth;
    private System.Windows.Forms.NumericUpDown numLineWidth;
    private System.Windows.Forms.Panel panel4;
    private System.Windows.Forms.CheckBox checkLineStipple;
    private System.Windows.Forms.ComboBox comboLineStipple;
    private System.Windows.Forms.Panel panel5;
    private System.Windows.Forms.ComboBox comboShader;
    private System.Windows.Forms.Label label1;
  }
}
