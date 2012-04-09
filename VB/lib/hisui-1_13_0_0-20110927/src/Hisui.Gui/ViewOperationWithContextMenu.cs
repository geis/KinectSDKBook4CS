using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Hisui.Gui
{
  class ViewOperationWithContextMenu : Graphics.DocumentViewOperation
  {
    readonly Func<ContextMenuStrip> _createContextMenu;
    
    public ViewOperationWithContextMenu(
      Core.Document document, Graphics.StandardViewOperation.Setting setting, Func<ContextMenuStrip> createContextMenu )
      : base( document, setting )
    {
      _createContextMenu = createContextMenu;
    }

    public ViewOperationWithContextMenu(
      Core.Document document, Func<ContextMenuStrip> createContextMenu )
      : base( document )
    {
      _createContextMenu = createContextMenu;
    }

    public ViewOperationWithContextMenu( Core.Document document, Graphics.StandardViewOperation.Setting setting )
      : this( document, setting, SI.CreateContextMenu )
    { }

    protected override void OnMouseUp( MouseButtons button, Keys modifier, Hisui.Geom.Point2i pos )
    {
      if ( !this.Operating && button == MouseButtons.Right ) {
        ContextMenuStrip menu = _createContextMenu();
        if ( menu != null ) menu.Show( (Control)base.View, pos );
      }
      base.OnMouseUp( button, modifier, pos );
    }
  }
}
