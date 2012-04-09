namespace Hisui.Gui
{
  partial class ProgressDialog
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ProgressDialog ) );
      this.progressBar = new System.Windows.Forms.ProgressBar();
      this.labelMessage = new System.Windows.Forms.Label();
      this.btnAbort = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // progressBar
      // 
      this.progressBar.AccessibleDescription = null;
      this.progressBar.AccessibleName = null;
      resources.ApplyResources( this.progressBar, "progressBar" );
      this.progressBar.BackgroundImage = null;
      this.progressBar.Font = null;
      this.progressBar.Name = "progressBar";
      this.progressBar.Step = 1;
      this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
      // 
      // labelMessage
      // 
      this.labelMessage.AccessibleDescription = null;
      this.labelMessage.AccessibleName = null;
      resources.ApplyResources( this.labelMessage, "labelMessage" );
      this.labelMessage.Font = null;
      this.labelMessage.Name = "labelMessage";
      // 
      // btnAbort
      // 
      this.btnAbort.AccessibleDescription = null;
      this.btnAbort.AccessibleName = null;
      resources.ApplyResources( this.btnAbort, "btnAbort" );
      this.btnAbort.BackgroundImage = null;
      this.btnAbort.DialogResult = System.Windows.Forms.DialogResult.Abort;
      this.btnAbort.Font = null;
      this.btnAbort.Name = "btnAbort";
      this.btnAbort.UseVisualStyleBackColor = true;
      // 
      // ProgressDialog
      // 
      this.AccessibleDescription = null;
      this.AccessibleName = null;
      resources.ApplyResources( this, "$this" );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = null;
      this.ControlBox = false;
      this.Controls.Add( this.btnAbort );
      this.Controls.Add( this.labelMessage );
      this.Controls.Add( this.progressBar );
      this.Font = null;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Icon = null;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ProgressDialog";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ProgressBar progressBar;
    private System.Windows.Forms.Label labelMessage;
    private System.Windows.Forms.Button btnAbort;
  }
}