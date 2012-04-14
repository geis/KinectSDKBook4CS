using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hisui.Gui
{
  class DocTreeNode : Core.BreathObject, Core.IBuild
  {
    readonly Core.IEntry _entry;
    readonly TreeNode _node;
    readonly DocTreeNodeCollection _docNodes;

    public DocTreeNode( Core.IEntry entry, TreeNode node )
    {
      _entry = entry;
      _node = node;
      _node.Tag = this;
      _docNodes = new DocTreeNodeCollection( _entry.Entries, _node.Nodes );
    }

    public Core.IEntry Entry
    {
      get { return _entry; }
    }

    public TreeNode TreeNode
    {
      get { return _node; }
    }

    public DocTreeNodeCollection DocNodes
    {
      get { return _docNodes; }
    }

    public IEnumerable<Core.IBreath> Sources
    {
      get
      {
        yield return this.Entry;
        yield return _docNodes;
        if ( _node.IsExpanded ) {
          foreach ( var node in _docNodes ) yield return node;
        }
        else {
          yield return _docNodes.FirstOrDefault();
        }
      }
    }

    public void Build()
    {
      string image = (this.Entry.Enabled && this.Entry.IsVisible()) ? "entry.png" : "entry_gray.png";
      _node.ImageKey = image;
      _node.SelectedImageKey = image;
      _node.Text = this.Entry.ToString();
    }
  }
}
