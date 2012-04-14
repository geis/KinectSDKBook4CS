namespace Hisui.Gui
{
  partial class MaterialPanel
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
      this.btnColor = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.trackShininess = new System.Windows.Forms.TrackBar();
      this.label2 = new System.Windows.Forms.Label();
      this.btnSpecularColor = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.label4 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.trackOpacity = new System.Windows.Forms.TrackBar();
      ((System.ComponentModel.ISupportInitialize)(this.trackShininess)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.trackOpacity)).BeginInit();
      this.SuspendLayout();
      // 
      // btnColor
      // 
      this.btnColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.btnColor.BackColor = System.Drawing.SystemColors.Control;
      this.btnColor.Location = new System.Drawing.Point( 62, 8 );
      this.btnColor.Name = "btnColor";
      this.btnColor.Size = new System.Drawing.Size( 155, 23 );
      this.btnColor.TabIndex = 0;
      this.btnColor.UseVisualStyleBackColor = false;
      this.btnColor.Click += new System.EventHandler( this.btnColor_Click );
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point( 15, 13 );
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size( 41, 12 );
      this.label1.TabIndex = 1;
      this.label1.Text = "通常色";
      // 
      // trackShininess
      // 
      this.trackShininess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.trackShininess.Location = new System.Drawing.Point( 6, 18 );
      this.trackShininess.Name = "trackShininess";
      this.trackShininess.Size = new System.Drawing.Size( 188, 45 );
      this.trackShininess.TabIndex = 2;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point( 15, 42 );
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size( 41, 12 );
      this.label2.TabIndex = 3;
      this.label2.Text = "反射色";
      // 
      // btnSpecularColor
      // 
      this.btnSpecularColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.btnSpecularColor.Location = new System.Drawing.Point( 62, 37 );
      this.btnSpecularColor.Name = "btnSpecularColor";
      this.btnSpecularColor.Size = new System.Drawing.Size( 155, 23 );
      this.btnSpecularColor.TabIndex = 4;
      this.btnSpecularColor.UseVisualStyleBackColor = false;
      this.btnSpecularColor.Click += new System.EventHandler( this.btnSpecularColor_Click );
      // 
      // groupBox1
      // 
      this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox1.Controls.Add( this.label4 );
      this.groupBox1.Controls.Add( this.label3 );
      this.groupBox1.Controls.Add( this.trackShininess );
      this.groupBox1.Location = new System.Drawing.Point( 17, 66 );
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size( 200, 74 );
      this.groupBox1.TabIndex = 5;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "反射特性";
      // 
      // label4
      // 
      this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point( 174, 54 );
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size( 17, 12 );
      this.label4.TabIndex = 4;
      this.label4.Text = "狭";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point( 12, 54 );
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size( 17, 12 );
      this.label3.TabIndex = 3;
      this.label3.Text = "広";
      // 
      // groupBox2
      // 
      this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.groupBox2.Controls.Add( this.label5 );
      this.groupBox2.Controls.Add( this.label6 );
      this.groupBox2.Controls.Add( this.trackOpacity );
      this.groupBox2.Location = new System.Drawing.Point( 17, 146 );
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size( 200, 78 );
      this.groupBox2.TabIndex = 6;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "透明度";
      // 
      // label5
      // 
      this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point( 153, 55 );
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size( 41, 12 );
      this.label5.TabIndex = 4;
      this.label5.Text = "不透明";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point( 8, 55 );
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size( 29, 12 );
      this.label6.TabIndex = 3;
      this.label6.Text = "透明";
      // 
      // trackOpacity
      // 
      this.trackOpacity.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.trackOpacity.Location = new System.Drawing.Point( 6, 18 );
      this.trackOpacity.Name = "trackOpacity";
      this.trackOpacity.Size = new System.Drawing.Size( 188, 45 );
      this.trackOpacity.TabIndex = 2;
      // 
      // MaterialPanel
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add( this.groupBox2 );
      this.Controls.Add( this.groupBox1 );
      this.Controls.Add( this.btnSpecularColor );
      this.Controls.Add( this.label2 );
      this.Controls.Add( this.label1 );
      this.Controls.Add( this.btnColor );
      this.Name = "MaterialPanel";
      this.Size = new System.Drawing.Size( 238, 237 );
      ((System.ComponentModel.ISupportInitialize)(this.trackShininess)).EndInit();
      this.groupBox1.ResumeLayout( false );
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout( false );
      this.groupBox2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.trackOpacity)).EndInit();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnColor;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TrackBar trackShininess;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button btnSpecularColor;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TrackBar trackOpacity;
    private System.Windows.Forms.Label label6;
  }
}
