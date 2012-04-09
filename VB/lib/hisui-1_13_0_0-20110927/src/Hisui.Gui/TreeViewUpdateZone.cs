using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hisui.Gui
{
  class TreeViewUpdateZone : IDisposable
  {
    public readonly TreeView TreeView;
    
    public TreeViewUpdateZone( TreeView treeView )
    {
      this.TreeView = treeView;
      this.TreeView.BeginUpdate();
    }

    public void Dispose()
    {
      this.TreeView.EndUpdate();
    }
  }
}
