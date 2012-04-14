using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Hisui.Gui
{
  public class QuadViewPanel : ViewPanel
  {
    readonly QuadPanel _panel = new QuadPanel
    {
      Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
    };

    readonly Graphics.GLViewControl[] _views =
      new Hisui.Graphics.GLViewControl[4] {
        new Graphics.GLViewControl { Dock = DockStyle.Fill },
        new Graphics.GLViewControl { Dock = DockStyle.Fill },
        new Graphics.GLViewControl { Dock = DockStyle.Fill },
        new Graphics.GLViewControl { Dock = DockStyle.Fill }
      };

    public Graphics.IView[] Views
    {
      get { return _views; }
    }

    public Graphics.IView TopLeftView
    {
      get { return _views[0]; }
    }

    public Graphics.IView TopRightView
    {
      get { return _views[1]; }
    }

    public Graphics.IView BottomLeftView
    {
      get { return _views[2]; }
    }

    public Graphics.IView BottomRightView
    {
      get { return _views[3]; }
    }

    const int SplitterWidth = 5;

    public override void SetUp( Hisui.Graphics.DocumentViews docviews )
    {
      base.SetUp( docviews );
      this.Controls.Add( _panel );

      _panel.TopLeftPanel.Controls.Add( _views[0] );
      _panel.TopRightPanel.Controls.Add( _views[1] );
      _panel.BottomLeftPanel.Controls.Add( _views[2] );
      _panel.BottomRightPanel.Controls.Add( _views[3] );

#if true
      foreach ( Graphics.GLViewControl view in _views ) {
        this.AddView( view );
      }
#else
      this.AddView( _views[0] );
      this.AddView( _views[1], new int[] { 1 } );
      this.AddView( _views[2], new int[] { 2 } );
      this.AddView( _views[3], new int[] { 3 } );
#endif
    }

    protected override void UpdateLayout( Rectangle client )
    {
      _panel.Bounds = client;
    }
  }
}
