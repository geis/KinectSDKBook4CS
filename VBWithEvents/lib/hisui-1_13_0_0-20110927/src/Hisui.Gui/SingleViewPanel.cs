using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Hisui.Gui
{
  public class SingleViewPanel : ViewPanel
  {
    readonly Graphics.GLViewControl _view = new Hisui.Graphics.GLViewControl();

    public override void SetUp( Hisui.Graphics.DocumentViews docviews )
    {
      _view.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
      base.SetUp( docviews );
      this.Controls.Add( _view );
      this.AddView( _view );
    }

    protected override void UpdateLayout( Rectangle client )
    {
      _view.Bounds = client;
    }
  }
}
