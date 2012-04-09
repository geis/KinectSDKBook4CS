using System.Windows.Forms ;
using System.Collections.Generic ;
using System.Drawing ;
using Hisui.Graphics ;
namespace Hisui.Gui
{
  class TreeNodeBuilder
    : Core.BreathObject
    , Core.IBuild
  {
    public static TreeNodeBuilder New(
        Core.Document document,
        TreeView treeView
      )
    {
      string name = document.ToString();
      string text = document.ToString();
      string icon = "document.png";
      TreeNode node = treeView.Nodes.Add( name, text, icon, icon );
      node.Tag = new TreeNodeBuilder( document, node );
      return (TreeNodeBuilder)node.Tag;
    }

    public static TreeNodeBuilder New(
        Core.Document document,
        Core.IEntry entry,
        TreeNodeCollection nodes
      )
    {
      string name = MakeNodeName( entry.ID );
      string text = entry.ToString();
      string icon = "entry.png";
      TreeNode node = nodes.Add( name, text, icon, icon );
      node.Tag = new TreeNodeBuilder( document, entry, node );
      return (TreeNodeBuilder)node.Tag;
    }

    public static TreeNode Find( TreeNodeBuilder root, int[] path )
    {
      TreeNode found = null;
      TreeNodeBuilder it = root;
      foreach ( int id in path ) {
        found = it.Find( id );
        if ( found == null ) return null;
        it = (TreeNodeBuilder)found.Tag;
      }
      return found;
    }

    public static TreeNode ActiveNode
    {
      get { return _active; }
      set
      {
        if ( _active == value ) return;
        if ( _active != null ) _active.NodeFont = null;
        _active = value;
        if ( _active != null ) {
          _active.NodeFont = new Font( _active.TreeView.Font, FontStyle.Bold );
          _active.Text = _active.Text;
        }
      }
    }

    static TreeNode _active;

    /////////////////////////////////


    readonly Core.Document _document;
    readonly int[] _path;
    readonly TreeNode _node;

    TreeNodeBuilder( Core.Document document, Core.IEntry entry, TreeNode node )
    {
      _document = document;
      _path = entry.Path;
      _node = node;
    }

    TreeNodeBuilder( Core.Document document, TreeNode node )
    {
      _document = document;
      _path = null;
      _node = node;
    }

    public Core.IBreath Tag
    {
      get { return _path == null ? (Core.IBreath)_document : (Core.IBreath)_document.FindEntry( _path ); }
    }

    public Core.IEntryCollection SubEntries
    {
      get { return _path == null ? _document.Entries : _document.FindEntry( _path ).Entries; }
    }

    public override void Touch()
    {
      base.Touch();
      this.Tag.Touch();
    }

    public IEnumerable<Core.IBreath> Sources
    {
      get
      {
        yield return this.Tag;

        if ( _path != null ) {
          Core.IEntry entry = _document.FindEntry( _path );
          yield return entry.GetElement<Graphics.ISceneDecorator>();
        }

        foreach ( Core.IEntry child in this.SubEntries ) {
          TreeNode childNode = this.Find( child.ID );
          if ( childNode == null )
            yield return New( _document, child, _node.Nodes );
          else
            yield return (Core.IBuild)childNode.Tag;

          if ( child.Entries == _document.ActiveEntries ) {
            ActiveNode = this.Find( child.ID );
          }

          if ( !_node.IsExpanded ) break;
        }
      }
    }

    public void Build()
    {
      Core.IBreath src = this.Tag;
      _node.Text = src.ToString();
      Core.IEntry entry = src as Core.IEntry;

      // entry
      if ( entry != null ) {
        ISceneDecorator deco = DecoratorFactory.GetEntryDecorator( entry );
        string image = (entry.Enabled && entry.IsVisible()) ? "entry.png" : "entry_gray.png";
        _node.ImageKey = image;
        _node.SelectedImageKey = image;
      }

      // scene
      if ( entry != null ) {
        TreeNode node = this.Find( "scene" );
        if ( node == null ) {
          node = _node.Nodes.Insert(
              0, "scene", "ï`âÊèÓïÒ", "scene.png", "scene.png"
            );
        }
        ISceneDecorator elm = entry.GetElement<ISceneDecorator>();
        node.Tag = elm;
        node.Nodes.Clear();
        string icon = "decoration.png";
        foreach ( IDecoration d in elm.Decorations ) {
          string name = d.GetType().Name;
          TreeNode dec = node.Nodes.Add( name, name, icon, icon );
          dec.Tag = d;
        }
      }

      // entity
      if ( entry != null ) {
        TreeNode node = this.Find( "entity" );
        if ( entry.Entity != null ) {
          string name = entry.Entity.GetType().Name;
          if ( node == null ) {
            node = _node.Nodes.Insert(
                1, "entity", name, "entity.png", "entity.png" );
          }
          else if ( node.Text != name ) {
            node.Text = name;
          }
          node.Tag = entry.Entity;
        }
        else if ( node != null ) {
          _node.Nodes.Remove( node );
        }
      }

      // ïsóvÇ» TreeNode ÇçÌèú
      {
        List<TreeNode> removing = new List<TreeNode>();
        foreach ( TreeNode node in _node.Nodes ) {
          if ( node.Name == "scene" ) continue;
          if ( node.Name == "entity" ) continue;
          TreeNodeBuilder tnb = (TreeNodeBuilder)node.Tag;
          Core.IEntry e = tnb.Tag as Core.IEntry;
          if ( e == null || !this.SubEntries.Contains( e.ID ) ) {
            removing.Add( node );
          }
        }
        foreach ( TreeNode node in removing ) {
          _node.Nodes.Remove( node );
        }
      }
    }

    static string MakeNodeName( int id )
    {
      return string.Format( "e{0}", id );
    }

    public TreeNode Find( int id )
    {
      return Find( MakeNodeName( id ) );
    }

    TreeNode Find( string name )
    {
      TreeNode[] founds = _node.Nodes.Find( name, false );
      return (founds.Length != 0) ? founds[0] : null;
    }
  }
}

