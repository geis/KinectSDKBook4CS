using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hisui.Gui
{
  public partial class ProgressDialog : Form
  {
    readonly System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();

    public ProgressDialog()
    {
      InitializeComponent();
      this.CompletionSleepMilliseconds = 200;
      this.DialogHiddenMilliseconds = 100;
      this.btnAbort.Click += ( sender, e ) => Core.Progress.PostAbort();
    }

    public void SetUp( Form owner, bool abortButton )
    {
      this.Owner = owner;
      this.btnAbort.Visible = abortButton;
      SI.ProgressChanged += this.OnProgressChanged;
    }

    public void Detach()
    {
      SI.ProgressChanged -= this.OnProgressChanged;
    }

    public int CompletionSleepMilliseconds { get; set; }
    public int DialogHiddenMilliseconds { get; set; }

    new void Show()
    {
      if ( !this.Visible ) {
        if ( this.Owner != null ) {
          var s = this.Owner.Size - this.Size;
          this.Location = this.Owner.Location + new Size( s.Width / 2, s.Height / 2 );
          this.StartPosition = FormStartPosition.Manual;
        }
        else {
          this.StartPosition = FormStartPosition.CenterScreen;
        }
        base.Show();
      }
    }

    void OnProgressChanged( object sender, Core.ProgressEventArgs e )
    {
      SI.UpdateToolStrips();
      SI.DocumentViews.Invalidate();
      if ( e.Value >= 1.0 ) {
        if ( this.Visible ) {
          // 100% になったプログレスバーを一瞬表示
          this.progressBar.Value = this.progressBar.Maximum;
          this.Refresh();
          System.Threading.Thread.Sleep( this.CompletionSleepMilliseconds );
        }
        _stopwatch.Reset();
        this.progressBar.Value = 0;
        this.labelMessage.Text = "";
        this.Hide();
      }
      else if ( !_stopwatch.IsRunning ) {
        _stopwatch.Start();
      }
      else if ( _stopwatch.ElapsedMilliseconds > this.DialogHiddenMilliseconds ) {
        this.progressBar.Value = (int)(e.Value * (this.progressBar.Maximum - this.progressBar.Minimum));
        this.labelMessage.Text = e.Caption;
        this.Show();
        Application.DoEvents();
      }
    }
  }
}
