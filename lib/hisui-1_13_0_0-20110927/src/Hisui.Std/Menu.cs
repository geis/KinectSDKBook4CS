using System.Windows.Forms ;
using System.Collections.Generic ;
using System.Text;
using System;
using System.Linq;

namespace Hisui.Std
{
  [Ctrl.Command]
  static partial class Menu
  {
    [Ctrl.Command( "削除" )]
    static void Delete( IEnumerable<Core.IEntry> self, Ctrl.IContext con )
    {
      // 注：self.ToArray() してシーケンスをコピーしておかないと Remove で落ちる 
      foreach ( var item in self.ToArray() ) item.Owner.Remove( item.ID );
    }

    [Ctrl.Command( "クリア" )]
    public static void Clear( Core.Document self )
    {
      Ctrl.CommandHelper.Clear();
    }

    //[Ctrl.Command( "表示/非表示" )]
    static void ShowHide( Graphics.ISceneDecorator deco, Ctrl.IContext con, Ctrl.CommandOption opt )
    {
      if ( opt.QueryRunnable ) { opt.IsChecked = deco.Visible; return; }
      deco.Visible = !deco.Visible;
    }

    [Ctrl.Command( "有効/無効" )]
    static void EnableDisable( Core.IEntry entry, Ctrl.IContext con, Ctrl.CommandOption opt )
    {
      if ( opt.QueryRunnable ) { opt.IsChecked = entry.Enabled; return; }
      entry.Enabled = !entry.Enabled;
    }

    [Ctrl.Command( "アクティブフォルダに設定" )]
    static void Activate( Core.IEntry self, Ctrl.IContext con )
    {
      con.Document.ActiveEntries = self.Entries;
    }
  }
}

