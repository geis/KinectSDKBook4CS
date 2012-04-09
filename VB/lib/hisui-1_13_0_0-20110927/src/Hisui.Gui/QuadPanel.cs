using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Hisui.Gui
{
  class QuadPanel : UserControl
  {
    const int SPLITTER_WIDTH = 4;

    readonly Panel _topleft = new Panel();
    readonly Panel _topright = new Panel();
    readonly Panel _botleft = new Panel();
    readonly Panel _botright = new Panel();

    Geom.Point2d _ratio = new Hisui.Geom.Point2d( 0.5, 0.5 );

    public QuadPanel()
    {
      this.Controls.Add( _topleft );
      this.Controls.Add( _topright );
      this.Controls.Add( _botleft );
      this.Controls.Add( _botright );
      this.SizeChanged += delegate( object sender, EventArgs e ) { this.LayoutPanels(); };
      this.LayoutPanels();
    }

    public Panel TopLeftPanel
    {
      get { return _topleft; }
    }

    public Panel TopRightPanel
    {
      get { return _topright; }
    }

    public Panel BottomLeftPanel
    {
      get { return _botleft; }
    }

    public Panel BottomRightPanel
    {
      get { return _botright; }
    }

    public Point CenterLocation
    {
      get
      {
        return new Point(
          (int)Math.Round( _ratio.x * this.Width ),
          (int)Math.Round( _ratio.y * this.Height ) );
      }
      set
      {
        Point center = value;
        if ( center.X < SPLITTER_WIDTH ) center.X = SPLITTER_WIDTH;
        if ( center.Y < SPLITTER_WIDTH ) center.Y = SPLITTER_WIDTH;
        if ( center.X > this.Width - SPLITTER_WIDTH ) center.X = this.Width - SPLITTER_WIDTH;
        if ( center.Y > this.Height - SPLITTER_WIDTH ) center.Y = this.Height - SPLITTER_WIDTH;
        _ratio.x = (double)center.X / this.Width;
        _ratio.y = (double)center.Y / this.Height;
        this.LayoutPanels();
      }
    }

    void LayoutPanels()
    {
      this.SuspendLayout();

      int offset = SPLITTER_WIDTH / 2;
      Point center = this.CenterLocation;

      _topleft.Location = new Point( 0, 0 );
      _topleft.Size = new Size( center.X - offset, center.Y - offset );

      _topright.Location = new Point( center.X + offset, 0 );
      _topright.Size = new Size( this.Width - center.X - offset, center.Y - offset );

      _botleft.Location = new Point( 0, center.Y + offset );
      _botleft.Size = new Size( center.X - offset, this.Height - center.Y - offset );

      _botright.Location = new Point( center.X + offset, center.Y + offset );
      _botright.Size = new Size( this.Width - center.X - offset, this.Height - center.Y - offset );

      this.ResumeLayout();
    }
  }
}
