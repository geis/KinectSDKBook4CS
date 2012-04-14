using System.Windows.Forms ;
using Hisui.Graphics ;
using System ;
namespace Hisui.Gui {

static class SceneElementMenu
{
  static public ContextMenuStrip Get( ISceneDecorator target )
  {
    _target = target ;
    foreach ( ToolStripMenuItem item in _menu.Items ) {
      item.Checked = target.ContainsDecoration( (Type)item.Tag ) ;
    }
    return _menu ;
  }

  static void MenuItemClick( object sender, EventArgs args )
  {
    Core.Breath.Increment() ;
    ToolStripMenuItem item = (ToolStripMenuItem)sender ;
    if ( item.Checked ) {
      _target.RemoveDecoration( (Type)item.Tag ) ;
      item.Checked = false ;
    } else {
      Type type = (Type)item.Tag ;
      object obj = type.GetConstructor( Type.EmptyTypes ).Invoke( null ) ;
      _target.PutDecoration( type, (IDecoration)obj ) ;
      item.Checked = true ;
    }
    Core.Document.Current.History.Commit() ;
  }

  static SceneElementMenu()
  {
    foreach ( Type type in Graphics.DecoratorFactory.DecorationTypes ) {
      ToolStripMenuItem item = new ToolStripMenuItem( type.Name ) ;
      item.Tag = type ;
      item.Click += MenuItemClick ;
      _menu.Items.Add( item ) ;
    }
  }

  static ISceneDecorator  _target ;
  static ContextMenuStrip _menu = new ContextMenuStrip() ;
}

}
