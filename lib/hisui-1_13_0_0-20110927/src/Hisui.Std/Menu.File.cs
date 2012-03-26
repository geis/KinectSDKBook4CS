using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hisui.Std
{
  partial class Menu
  {
    [Ctrl.Command( "ファイル", Order = 100 )]
    static class File
    {
      [Ctrl.Command( "クリア", Order = 10, ShortcutKeys = Keys.Control | Keys.N )]
      static void Clear()
      {
        Ctrl.CommandHelper.Clear();
      }

      [Ctrl.Command( "開く", Order = 20, ShortcutKeys = Keys.Control | Keys.O )]
      static void Open()
      {
        Ctrl.CommandHelper.Open();
      }

      [Ctrl.Command( "上書保存", Order = 30, ShortcutKeys = Keys.Control | Keys.S )]
      static void Save()
      {
        Ctrl.CommandHelper.Save();
      }

      [Ctrl.Command( "名前をつけて保存", Order = 40 )]
      static void SaveAs()
      {
        Ctrl.CommandHelper.SaveAs();
      }
    }
  }
}
