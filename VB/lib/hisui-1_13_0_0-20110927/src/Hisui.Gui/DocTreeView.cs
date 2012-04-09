using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace Hisui.Gui
{
  class DocTreeView : Core.BreathObject, Core.IBuild
  {
    readonly Core.Document _doc;
    readonly TreeView _treeView;
    readonly TreeNode _rootNode;
    readonly DocTreeNodeCollection _nodes;
    readonly DocTreeNodeSelection _selection;
    readonly ActiveNode _activeNode;

    public DocTreeView( Core.Document doc, TreeView treeView )
    {
      _doc = doc;
      _treeView = treeView;
      _rootNode = new TreeNode { ImageKey = "document.png", SelectedImageKey = "document.png" };
      _treeView.Nodes.Add( _rootNode );
      _nodes = new DocTreeNodeCollection( _doc.Entries, _rootNode.Nodes );
      _selection = new DocTreeNodeSelection( _treeView, _doc.SelectedEntries, this.Find );
      _activeNode = new ActiveNode { Font = new Font( _treeView.Font, FontStyle.Bold ) };
    }

    public DocTreeNodeSelection Selection
    {
      get { return _selection; }
    }

    public ContextMenuStrip CreateContextMenu()
    {
      if ( _treeView.SelectedNode == _rootNode ) {
        Ctrl.Current.Self = _doc;
        return SI.CreateContextMenu( _doc );
      }
      else {
        Ctrl.Current.Self = null;
        return SI.CreateContextMenu( _doc.SelectedEntries );
      }
    }

    DocTreeNode Find( Core.IEntry entry )
    {
      return (entry != null) ? this.Find( entry.Path ) : null;
    }

    DocTreeNode Find( int[] path )
    {
      var items = _nodes;
      var found = default( DocTreeNode );
      for ( int i = 0 ; i < path.Length ; ++i ) {
        if ( !items.Find( path[i], out found ) ) return null;
        items = found.DocNodes;
      }
      return found;
    }

    #region IBuild メンバ

    public IEnumerable<Hisui.Core.IBreath> Sources
    {
      get
      {
        using ( new TreeViewUpdateZone( _treeView ) ) {
          yield return _doc;
          yield return _nodes;
          foreach ( var node in _nodes ) yield return node;
          yield return _selection;
        }
      }
    }

    public void Build()
    {
      using ( new TreeViewUpdateZone( _treeView ) ) {
        _rootNode.Text = _doc.ToString();

        // アクティブノードを強調表示
        var activeEntry = _doc.ActiveEntries.Owner as Core.IEntry;
        _activeNode.Value = (activeEntry != null) ? this.Find( activeEntry ).TreeNode : _rootNode;
      }
    }

    #endregion


    class ActiveNode
    {
      TreeNode _value;

      public Font Font;

      public TreeNode Value
      {
        get { return _value; }
        set
        {
          if ( _value != value ) {
            if ( _value != null ) { _value.NodeFont = null; _value.Text = _value.Text; }
            _value = value;
            if ( _value != null ) { _value.NodeFont = this.Font; _value.Text = _value.Text; }
          }
        }
      }
    }

  }
}

