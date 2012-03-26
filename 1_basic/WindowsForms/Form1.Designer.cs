namespace WindowsForms
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
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

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
      this.pictureBoxRgb = new System.Windows.Forms.PictureBox();
      this.pictureBoxDepth = new System.Windows.Forms.PictureBox();
      this.comboBoxRange = new System.Windows.Forms.ComboBox();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRgb)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDepth)).BeginInit();
      this.SuspendLayout();
      // 
      // pictureBoxRgb
      // 
      this.pictureBoxRgb.Location = new System.Drawing.Point(12, 51);
      this.pictureBoxRgb.Name = "pictureBoxRgb";
      this.pictureBoxRgb.Size = new System.Drawing.Size(640, 480);
      this.pictureBoxRgb.TabIndex = 0;
      this.pictureBoxRgb.TabStop = false;
      // 
      // pictureBoxDepth
      // 
      this.pictureBoxDepth.Location = new System.Drawing.Point(658, 51);
      this.pictureBoxDepth.Name = "pictureBoxDepth";
      this.pictureBoxDepth.Size = new System.Drawing.Size(640, 480);
      this.pictureBoxDepth.TabIndex = 1;
      this.pictureBoxDepth.TabStop = false;
      // 
      // comboBoxRange
      // 
      this.comboBoxRange.FormattingEnabled = true;
      this.comboBoxRange.Location = new System.Drawing.Point(12, 12);
      this.comboBoxRange.Name = "comboBoxRange";
      this.comboBoxRange.Size = new System.Drawing.Size(121, 20);
      this.comboBoxRange.TabIndex = 2;
      this.comboBoxRange.SelectedIndexChanged += new System.EventHandler(this.comboBoxRange_SelectedIndexChanged);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1317, 543);
      this.Controls.Add(this.comboBoxRange);
      this.Controls.Add(this.pictureBoxDepth);
      this.Controls.Add(this.pictureBoxRgb);
      this.Name = "Form1";
      this.Text = "WindowsForms";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxRgb)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDepth)).EndInit();
      this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxRgb;
        private System.Windows.Forms.PictureBox pictureBoxDepth;
        private System.Windows.Forms.ComboBox comboBoxRange;
    }
}

