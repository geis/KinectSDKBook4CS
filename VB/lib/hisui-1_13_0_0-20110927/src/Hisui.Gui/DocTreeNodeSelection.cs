using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Hisui.Gui
{
  class DocTreeNodeSelection : Core.BreathObject, Core.IBuild, IEnumerable<DocTreeNode>
  {
    readonly TreeView _treeView;
    readonly Core.Document.SelectedEntryCollection _entries;
    readonly Func<Core.IEntry, DocTreeNode> _findDocTreeNode;
    readonly Core.Set<DocTreeNode> _collection = new Hisui.Core.Set<DocTreeNode>();

    public DocTreeNodeSelection(
      TreeView treeView,
      Core.Document.SelectedEntryCollection entries, 
      Func<Core.IEntry,DocTreeNode> findNode )
    {
      _findDocTreeNode = findNode;
      _entries = entries;
      _treeView = treeView;
      _treeView.GotFocus += ( sender, e ) => Highlight( _collection, true );
      _treeView.LostFocus += ( sender, e ) => Highlight( _collection, true );
    }

    public bool Contains( DocTreeNode node )
    {
      return (node != null) ? _collection.Contains( node ) : false;
    }

    #region IBuild メンバ

    public IEnumerable<Hisui.Core.IBreath> Sources
    {
      get { yield return _entries; }
    }

    public void Build()
    {
      using ( new TreeViewUpdateZone( _treeView ) ) {
        Highlight( _collection, false );
        _collection.Clear();
        _collection.Add( _entries.Select( e => _findDocTreeNode( e ) ).WhereNotNull() );
        Highlight( _collection, true );

        var node = _findDocTreeNode( _entries.Value );
        if ( node != null || _treeView.SelectedNode != _treeView.Nodes[0] ) {
          _treeView.SelectedNode = (node != null) ? node.TreeNode : null;
        }
      }
    }

    #endregion

    #region IEnumerable<DocTreeNode> メンバ

    public IEnumerator<DocTreeNode> GetEnumerator()
    {
      return _collection.GetEnumerator();
    }

    #endregion

    #region IEnumerable メンバ

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    #endregion

    static void Highlight( TreeNode node, bool highlighted )
    {
      if ( node.TreeView == null ) return;
      if ( highlighted ) {
        if ( node.TreeView.Focused ) {
          node.BackColor = SystemColors.Highlight;
          node.ForeColor = SystemColors.HighlightText;
        }
        else {
          node.BackColor = SystemColors.Control;
          node.ForeColor = SystemColors.ControlText;
        }
      }
      else {
        node.BackColor = SystemColors.Window;
        node.ForeColor = SystemColors.WindowText;
      }
    }

    static void Highlight( IEnumerable<DocTreeNode> nodes, bool highlighted )
    {
      foreach ( var node in nodes ) Highlight( node.TreeNode, highlighted );
    }
  }
}
