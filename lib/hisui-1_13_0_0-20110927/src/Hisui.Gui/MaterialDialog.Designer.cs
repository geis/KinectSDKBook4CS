namespace Hisui.Gui
{
  partial class MaterialDialog
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose( bool disposing )
    {
      if ( disposing && (components != null) ) {
        components.Dispose();
      }
      base.Dispose( disposing );
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.panelFrontMaterial = new Hisui.Gui.MaterialPanel();
      this.label1 = new System.Windows.Forms.Label();
      this.panelBackMaterial = new Hisui.Gui.MaterialPanel();
      this.checkBackMaterial = new System.Windows.Forms.CheckBox();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnOK = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // panelFrontMaterial
      // 
      this.panelFrontMaterial.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.panelFrontMaterial.Color = System.Drawing.SystemColors.Control;
      this.panelFrontMaterial.Location = new System.Drawing.Point( 12, 34 );
      this.panelFrontMaterial.Name = "panelFrontMaterial";
      this.panelFrontMaterial.Opacity = 0;
      this.panelFrontMaterial.Shininess = 0;
      this.panelFrontMaterial.Size = new System.Drawing.Size( 238, 237 );
      this.panelFrontMaterial.SpecularColor = System.Drawing.SystemColors.Control;
      this.panelFrontMaterial.TabIndex = 0;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point( 10, 16 );
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size( 45, 12 );
      this.label1.TabIndex = 1;
      this.label1.Text = "おもて面";
      // 
      // panelBackMaterial
      // 
      this.panelBackMaterial.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.panelBackMaterial.Color = System.Drawing.SystemColors.Control;
      this.panelBackMaterial.Enabled = false;
      this.panelBackMaterial.Location = new System.Drawing.Point( 256, 34 );
      this.panelBackMaterial.Name = "panelBackMaterial";
      this.panelBackMaterial.Opacity = 0;
      this.panelBackMaterial.Shininess = 0;
      this.panelBackMaterial.Size = new System.Drawing.Size( 238, 237 );
      this.panelBackMaterial.SpecularColor = System.Drawing.SystemColors.Control;
      this.panelBackMaterial.TabIndex = 2;
      // 
      // checkBackMaterial
      // 
      this.checkBackMaterial.AutoSize = true;
      this.checkBackMaterial.Location = new System.Drawing.Point( 256, 12 );
      this.checkBackMaterial.Name = "checkBackMaterial";
      this.checkBackMaterial.Size = new System.Drawing.Size( 51, 16 );
      this.checkBackMaterial.TabIndex = 3;
      this.checkBackMaterial.Text = "うら面";
      this.checkBackMaterial.UseVisualStyleBackColor = true;
      this.checkBackMaterial.CheckedChanged += new System.EventHandler( this.checkBackMaterial_CheckedChanged );
      // 
      // btnCancel
      // 
      this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point( 419, 277 );
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size( 75, 23 );
      this.btnCancel.TabIndex = 4;
      this.btnCancel.Text = "キャンセル";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // btnOK
      // 
      this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnOK.Location = new System.Drawing.Point( 338, 277 );
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size( 75, 23 );
      this.btnOK.TabIndex = 5;
      this.btnOK.Text = "OK";
      this.btnOK.UseVisualStyleBackColor = true;
      // 
      // MaterialDialog
      // 
      this.AcceptButton = this.btnOK;
      this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size( 507, 309 );
      this.Controls.Add( this.btnOK );
      this.Controls.Add( this.btnCancel );
      this.Controls.Add( this.checkBackMaterial );
      this.Controls.Add( this.panelBackMaterial );
      this.Controls.Add( this.label1 );
      this.Controls.Add( this.panelFrontMaterial );
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Name = "MaterialDialog";
      this.Text = "マテリアルの設定";
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private MaterialPanel panelFrontMaterial;
    private System.Windows.Forms.Label label1;
    private MaterialPanel panelBackMaterial;
    private System.Windows.Forms.CheckBox checkBackMaterial;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnOK;
  }
}