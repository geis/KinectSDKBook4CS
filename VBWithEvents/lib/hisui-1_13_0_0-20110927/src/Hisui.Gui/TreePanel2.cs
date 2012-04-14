using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace Hisui.Gui
{
  public partial class TreePanel2 : BreathControl, Core.IDependent
  {
    Graphics.DocumentViews _docviews;
    DocTreeView _docTreeView;
    
    public TreePanel2()
    {
      InitializeComponent();
    }

    public void SetUp( Graphics.DocumentViews docviews )
    {
      _docviews = docviews;
      _docTreeView = new DocTreeView( _docviews.Document, this.treeView );
      this.propertyPanel1.SetUp( docviews );
    }

    IEnumerable<Core.IBreath> Core.IDependent.Sources
    {
      get
      {
        yield return _docviews;
        yield return this.propertyPanel1;
        yield return _docTreeView;
      }
    }

    void SelectNode( TreeNode target, bool append )
    {
      treeView.SelectedNode = target;

      // 選択された TreeNodeBuilder2
      var selected = (target != null) ? target.Tag as DocTreeNode : null;

      // 選択されたエントリ
      var entry = (selected != null) ? selected.Entry : null;

      // SelectedEntries の設定
      if ( append && selected != null )
        _docviews.Document.SelectedEntries.Add( entry );
      else
        _docviews.Document.SelectedEntry = entry;

      // 現在のオペレーションをアボート
      SI.Driver.Abort();

      SI.Build();
      _docviews.Invalidate();
    }


    private void treeView_NodeMouseClick(
        object sender, TreeNodeMouseClickEventArgs e )
    {
      using ( new TreeViewUpdateZone( this.treeView ) ) {
        if ( ModifierKeys == Keys.Control || ModifierKeys == Keys.Shift )
          this.SelectNode( e.Node, true ); // Shift か Ctrl キーで追加選択
        else if ( !_docTreeView.Selection.Contains( e.Node.Tag as DocTreeNode ) )
          this.SelectNode( e.Node, false );
      }

      if ( e.Button == MouseButtons.Right ) {
        var menu = _docTreeView.CreateContextMenu();
        if ( menu != null ) menu.Show( treeView, e.Location );
      }
    }

    private void treeView_AfterExpand( object sender, TreeViewEventArgs e )
    {
      e.Node.Tag.As<DocTreeNode>( node =>
        { using ( new TreeViewUpdateZone( treeView ) ) SI.Build( node ); } );
    }

    private void treeView_BeforeExpand( object sender, TreeViewCancelEventArgs e )
    {
      var node = e.Node.Tag as DocTreeNode;
      if ( node == null ) return;
      if ( node.Entry.Entries.Count < 100 ) return;

      DialogResult result = MessageBox.Show(
        string.Format( "子要素が {0} 個あります。展開しますか？", node.Entry.Entries.Count ),
        "警告",
        MessageBoxButtons.OKCancel
      );
      if ( result == DialogResult.OK ) Core.Builder.Build( node );
      else e.Cancel = true;
    }

    #region ドラッグ＆ドロップ

    private void treeView_ItemDrag( object sender, ItemDragEventArgs e )
    {
      TreeNode source = (TreeNode)e.Item;
      if ( source.Tag is DocTreeNode ) {
        treeView.DoDragDrop( e.Item, DragDropEffects.Move );
      }
    }

    private void treeView_DragOver( object sender, DragEventArgs e )
    {
      e.Effect = DragDropEffects.None;

      TreeNode source = (TreeNode)e.Data.GetData( typeof( TreeNode ) );
      TreeNode target = treeView.GetNodeAt( treeView.PointToClient( new Point( e.X, e.Y ) ) );

      if ( source == null || target == null || source == target ) return;
      if ( !(source.Tag is DocTreeNode) ) return;
      if ( IsChildNode( source, target ) ) return;

      e.Effect = DragDropEffects.Move;
    }

    private void treeView_DragDrop( object sender, DragEventArgs e )
    {
      if ( e.Data.GetDataPresent( typeof( TreeNode ) ) ) {
        TreeNode source = (TreeNode)e.Data.GetData( typeof( TreeNode ) );
        TreeNode target = treeView.GetNodeAt( treeView.PointToClient( new Point( e.X, e.Y ) ) );

        if ( source == null || target == null || source == target ) return;
        if ( !(source.Tag is DocTreeNode) ) return;
        if ( IsChildNode( source, target ) ) return;

        var src = ((DocTreeNode)source.Tag).Entry;
        var dst = (target.Tag is DocTreeNode) ?
          ((DocTreeNode)target.Tag).Entry.Entries : _docviews.Document.Entries;
        SI.MoveEntry( src, dst );
        SI.Commit();
      }
    }

    static bool IsChildNode( TreeNode parent, TreeNode child )
    {
      if ( child.Parent == parent ) return true;
      if ( child.Parent == null ) return false;
      return IsChildNode( parent, child.Parent );
    }

    #endregion
  }
}
