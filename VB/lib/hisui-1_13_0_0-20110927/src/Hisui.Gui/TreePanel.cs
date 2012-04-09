using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Hisui.Gui
{
  public partial class TreePanel : UserControl, Core.IBuild
  {
    Graphics.DocumentViews _docviews;
    Core.Breath _breath = new Core.Breath();
    SelectedNodeCollection _selectedNodes;

    public TreePanel()
    {
      InitializeComponent();
      _selectedNodes = new SelectedNodeCollection( treeView );
    }

    public Graphics.DocumentViews DocumentViews
    {
      get { return _docviews; }
      set { _docviews = value; }
    }

    int Core.IBreath.BreathCount
    {
      get { return _breath.BreathCount; }
    }

    void Core.IBreath.Touch()
    {
      _breath.Touch();
    }

    IEnumerable<Core.IBreath> Core.IDependent.Sources
    {
      get
      {
        yield return _docviews;
        treeView.BeginUpdate();
        yield return this.Root;
        treeView.EndUpdate();
      }
    }

    void Core.IBuild.Build()
    {
      if ( Ctrl.Current.Entries.Count > 1 ) {
        // 複数選択の場合
        treeView.BeginUpdate();
        _selectedNodes.Clear();
        Core.IEntry[] entries = new Core.IEntry[Ctrl.Current.Entries.Count];
        Ctrl.Current.Entries.CopyTo( entries, 0 );
        foreach ( Core.IEntry entry in entries ) {
          TreeNode node = TreeNodeBuilder.Find( this.Root, entry.Path );
          if ( node != null ) _selectedNodes.Add( node );
        }
        treeView.EndUpdate();
      }
      else {
        // 単数選択または選択無しの場合
        TreeNode selected1 = null;
        Core.IEntry selEntry = Core.Document.Current.SelectedEntry;
        if ( selEntry != null ) {
          selected1 = TreeNodeBuilder.Find( this.Root, selEntry.Path );
        }

        TreeNode selected2 = _selectedNodes.Value;
        while ( selected2 != null ) {
          if ( selected2.Tag is TreeNodeBuilder ) break;
          selected2 = selected2.Parent;
        }

        if ( selected1 != selected2 ) {
          if ( selected1 != null ) {
            _selectedNodes.Value = selected1;
            selected1.EnsureVisible();
          }
          else {
            _selectedNodes.Value = null;
            SelectNode( null, false );
          }
        }
      }
    }

    TreeNodeBuilder Root
    {
      get
      {
        if ( treeView.Nodes.Count == 0 ) TreeNodeBuilder.New( Core.Document.Current, treeView );
        return (TreeNodeBuilder)treeView.Nodes[0].Tag;
      }
    }

    void SelectNode( TreeNode target, bool append )
    {
      // Ctrl.Current.Self の設定
      Ctrl.Current.Self = (target == null) ? null : target.Tag;
      if ( Ctrl.Current.Self is TreeNodeBuilder ) {
        Ctrl.Current.Self = ((TreeNodeBuilder)Ctrl.Current.Self).Tag;
      }

      // プロパティグリッドの設定
      this.propertyGrid.SelectedObject = Ctrl.Current.Self;

      // 選択された TreeNodeBuilder の探索
      TreeNodeBuilder selected = null;
      for ( TreeNode node = target ; node != null ; node = node.Parent ) {
        selected = node.Tag as TreeNodeBuilder;
        if ( selected != null ) break;
      }

      // SelectedEntries の設定
      Core.Document doc = Core.Document.Current;
      if ( append ) {
        Core.IEntry entry = (selected == null) ? null : selected.Tag as Core.IEntry;
        if ( entry == null ) doc.SelectedEntry = null;
        else doc.SelectedEntries.Add( entry );
        Ctrl.Current.Driver.Abort(); // 現在のオペレーションをアボート
      }
      else {
        Core.IEntry before = doc.SelectedEntry;
        doc.SelectedEntry =
          (selected == null) ? null : selected.Tag as Core.IEntry;
        if ( before != doc.SelectedEntry ) {
          // 選択エントリが変わったときは、現在のオペレーションをアボート
          Ctrl.Current.Driver.Abort();
        }
      }

      _docviews.Invalidate();
    }

    ContextMenuStrip MakeContextMenu( object obj )
    {
      if ( obj is Graphics.ISceneDecorator ) {
        return SceneElementMenu.Get( (Graphics.ISceneDecorator)obj );
      }
      else if ( obj is TreeNodeBuilder ) {
        object target = ((TreeNodeBuilder)obj).Tag;
        if ( target is Core.IEntry ) {
          return Ctrl.MenuItemRegistry.CreateContextMenu();
        }
        else {
          return Ctrl.MenuItemRegistry.CreateContextMenu( target );
        }
      }
      else {
        return Ctrl.MenuItemRegistry.CreateContextMenu( obj );
      }
    }

    ContextMenuStrip MakeContextMenu()
    {
      if ( _selectedNodes.Count == 0 ) return null;
      if ( _selectedNodes.Count == 1 ) {
        // 単数選択の場合
        return MakeContextMenu( _selectedNodes.Value.Tag );
      }
      else {
        // 複数選択の場合
        return Ctrl.MenuItemRegistry.CreateContextMenu();
      }
    }



    //private void treeView_AfterSelect( object sender, TreeViewEventArgs e )
    //{
    //  //SelectNode( e.Node, ModifierKeys == Keys.Control ) ;
    //}

    private void treeView_NodeMouseClick(
        object sender, TreeNodeMouseClickEventArgs e )
    {
      treeView.BeginUpdate();
      if ( ModifierKeys == Keys.Control || ModifierKeys == Keys.Shift ) {
        // Shift か Ctrl キーで追加選択
        _selectedNodes.Add( e.Node );
        SelectNode( e.Node, true );
      }
      else if ( !_selectedNodes.Contains( e.Node ) ) {
        _selectedNodes.Value = e.Node;
        SelectNode( e.Node, false );
      }
      treeView.EndUpdate();

      if ( e.Button == MouseButtons.Right ) {
        ContextMenuStrip menu = MakeContextMenu();
        if ( menu != null ) menu.Show( treeView, e.Location );
      }
    }

    private void propertyGrid_SelectedGridItemChanged( object sender, SelectedGridItemChangedEventArgs e )
    {
      Core.Breath.Increment();
    }

    private void propertyGrid_PropertyValueChanged( object s, PropertyValueChangedEventArgs e )
    {
      Core.Document.Current.History.Commit();
      _docviews.Refresh();
      propertyGrid.Refresh();
    }

    private void treeView_AfterExpand( object sender, TreeViewEventArgs e )
    {
      TreeNodeBuilder builder = e.Node.Tag as TreeNodeBuilder;
      if ( builder != null ) {
        treeView.BeginUpdate();
        Core.Builder.Build( builder );
        treeView.EndUpdate();
      }
    }

    private void treeView_BeforeExpand( object sender, TreeViewCancelEventArgs e )
    {
      TreeNodeBuilder builder = e.Node.Tag as TreeNodeBuilder;
      if ( builder == null ) return;
      if ( builder.SubEntries.Count < 100 ) return;

      DialogResult result = MessageBox.Show(
        string.Format( "子要素が {0} 個あります。展開しますか？", builder.SubEntries.Count ),
        "警告",
        MessageBoxButtons.OKCancel
      );
      if ( result == DialogResult.OK ) Core.Builder.Build( builder );
      else e.Cancel = true;
    }

    private void treeView_ItemDrag( object sender, ItemDragEventArgs e )
    {
      TreeNode source = (TreeNode)e.Item;
      if ( source.Tag is TreeNodeBuilder ) {
        //treeView.SelectedNode = (TreeNode)e.Item;
        _selectedNodes.Value = (TreeNode)e.Item;
        DragDropEffects dde = treeView.DoDragDrop( e.Item, DragDropEffects.Move );
      }
    }

    private void treeView_DragOver( object sender, DragEventArgs e )
    {
      e.Effect = DragDropEffects.None;

      TreeNode source = e.Data.GetData( typeof( TreeNode ) ) as TreeNode;
      if ( source == null ) return;
      if ( !(source.Tag is TreeNodeBuilder) ) return;

      Point pt = treeView.PointToClient( new Point( e.X, e.Y ) );
      TreeNode target = treeView.GetNodeAt( pt );
      if ( target == null || target == source ) return;
      if ( !(target.Tag is TreeNodeBuilder) ) return;
      if ( IsChildNode( source, target ) ) return;

      //treeView.SelectedNode = target;
      _selectedNodes.Value = target;
      e.Effect = DragDropEffects.Move;
    }

    private void treeView_DragDrop( object sender, DragEventArgs e )
    {
      if ( e.Data.GetDataPresent( typeof( TreeNode ) ) ) {
        Point pt = treeView.PointToClient( new Point( e.X, e.Y ) );
        TreeNode target = treeView.GetNodeAt( pt );
        TreeNode source = (TreeNode)e.Data.GetData( typeof( TreeNode ) );
        if ( target == null || target == source ) return;
        if ( !(source.Tag is TreeNodeBuilder) ) return;
        if ( !(target.Tag is TreeNodeBuilder) ) return;
        if ( IsChildNode( source, target ) ) return;

        TreeNodeBuilder targetBuilder = (TreeNodeBuilder)target.Tag;
        TreeNodeBuilder sourceBuilder = (TreeNodeBuilder)source.Tag;

        Core.Breath.Increment();
        Core.IEntryCollection dst = targetBuilder.SubEntries;
        Core.IEntry src = (Core.IEntry)sourceBuilder.Tag;
        CopyEntry( src, dst );
        src.Owner.Remove( src.ID );
        SI.Commit();
      }
    }

    static bool IsChildNode( TreeNode parent, TreeNode child )
    {
      if ( child.Parent == parent ) return true;
      if ( child.Parent == null ) return false;
      return IsChildNode( parent, child.Parent );
    }

    static void CopyEntry( Core.IEntry src, Core.IEntryCollection dst )
    {
      dst = dst.Put( src.Entity ).Entries;
      foreach ( Core.IEntry e in src.Entries ) CopyEntry( e, dst );
    }

    class SelectedNodeCollection : Core.Set<TreeNode>, ICollection<TreeNode>
    {
      readonly TreeView _treeview;

      internal SelectedNodeCollection( TreeView treeview )
      {
        _treeview = treeview;
        _treeview.GotFocus += ( sender, e ) => Highlight( this, true );
        _treeview.LostFocus += ( sender, e ) => Highlight( this, true );
      }

      public TreeNode Value
      {
        get { return _treeview.SelectedNode; }
        set
        {
          this.Clear();
          if ( value != null ) this.Add( value );
        }
      }

      public new void Add( TreeNode item )
      {
        if ( !base.Contains( item ) ) {
          _treeview.SelectedNode = item;
          Highlight( item, true );
          base.Add( item );
        }
      }

      public new bool Remove( TreeNode item )
      {
        if ( base.Remove( item ) ) {
          Highlight( item, false );
          if ( item == _treeview.SelectedNode ) {
            IEnumerator<TreeNode> en = base.GetEnumerator();
            _treeview.SelectedNode = en.MoveNext() ? en.Current : null;
          }
          return true;
        }
        return false;
      }

      public new void Clear()
      {
        if ( base.Count != 0 ) {
          _treeview.SelectedNode = null;
          Highlight( this, false );
          base.Clear();
        }
      }

      static void Highlight( TreeNode node, bool highlighted )
      {
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

      static void Highlight( IEnumerable<TreeNode> nodes, bool highlighted )
      {
        foreach ( var node in nodes ) Highlight( node, highlighted );
      }
    }
  }
}
