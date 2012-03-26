using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hisui.Gui
{
  class DocTreeNodeCollection : Core.BreathObject, Core.IBuild, IEnumerable<DocTreeNode>
  {
    readonly Core.IEntryCollection _entries;
    readonly TreeNodeCollection _nodes;

    public DocTreeNodeCollection(
      Core.IEntryCollection entries, TreeNodeCollection nodes )
    {
      _entries = entries;
      _nodes = nodes;
    }

    public bool Find( int id, out DocTreeNode found )
    {
      found = this.FirstOrDefault( item => item.Entry.ID == id );
      return found != null;
    }

    #region IEnumerable<TreeNodeBuilder2> メンバ

    public IEnumerator<DocTreeNode> GetEnumerator()
    {
      return _nodes.Cast<TreeNode>().Select( node => (DocTreeNode)node.Tag ).GetEnumerator();
    }

    #endregion

    #region IEnumerable メンバ

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    #endregion

    #region IBuild メンバ

    public IEnumerable<Hisui.Core.IBreath> Sources
    {
      get { yield return _entries.Owner; }
    }

    public void Build()
    {
      var addingEntries = new HashSet<Core.IEntry>( _entries );
      var removingNodes = new List<TreeNode>();
      foreach ( TreeNode node in _nodes ) {
        if ( !addingEntries.Remove( ((DocTreeNode)node.Tag).Entry ) ) removingNodes.Add( node );
      }
      foreach ( var e in addingEntries ) {
        new DocTreeNode( e, _nodes.Add( "entry" ) );
      }
      foreach ( var node in removingNodes ) {
        _nodes.Remove( node );
      }
    }

    #endregion
  }
}
