using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using Hisui.OpenGL;

namespace Hisui.Gui
{
  // ƒJƒƒ‰À•WŒn‚Å•`‰æ
  public class CompassScene : Graphics.IScene
  {
    public static void PutTo( Graphics.DocumentViews docviews )
    {
      foreach ( var view in docviews.Views ) PutTo( view );
    }

    public static void PutTo( Graphics.IView view )
    {
      if ( !view.SceneGraph.CameraScenes.Any( scene => scene is CompassScene ) ) {
        var scene = new CompassScene { View = view };
        view.SceneGraph.CameraScenes.Add( scene );
        scene.FixPos();
      }
    }

    public static void RemoveFrom( Graphics.DocumentViews docviews )
    {
      foreach ( var view in docviews.Views ) RemoveFrom( view );
    }

    public static void RemoveFrom( Graphics.IView view )
    {
      view.SceneGraph.CameraScenes
        .Where( scene => scene is CompassScene )
        .ForEach( scene => ((CompassScene)scene).View = null );
      view.SceneGraph.CameraScenes.RemoveAll( scene => scene is CompassScene );
    }

    Graphics.IView _view;
    Geom.Point2i _pos;
    int _length = 48;

    readonly SceneComponent[] _components = new SceneComponent[] {
      new ConeScene( Geom.CodAxis.X ) { Color = Color.Red },
      new ConeScene( Geom.CodAxis.Y ) { Color = Color.Green },
      new ConeScene( Geom.CodAxis.Z ) { Color = Color.Blue },
      new CylinderScene( Geom.CodAxis.X ) { Color = Color.Pink },
      new CylinderScene( Geom.CodAxis.Y ) { Color = Color.LightGreen },
      new CylinderScene( Geom.CodAxis.Z ) { Color = Color.SkyBlue },
      new SphereScene { Color = Color.Gray },
    };


    private CompassScene() { }


    public Graphics.IView View
    {
      get { return _view; }
      set
      {
        if ( _view != value ) {
          if ( _view != null ) {
            _view.Events.MouseMove -= OnMouseMove;
            _view.Events.MouseDown -= OnMouseDown;
            _view.Events.SizeChanged -= OnSizeChanged;
          }
          _view = value;
          if ( _view != null ) {
            _view.Events.MouseMove += OnMouseMove;
            _view.Events.MouseDown += OnMouseDown;
            _view.Events.SizeChanged += OnSizeChanged;
          }
        }
      }
    }


    abstract class SceneComponent : Graphics.IScene
    {
      protected const double AXIS_RADIUS = 0.05;
      readonly Geom.Point3d _pos;
      readonly HiGL.IRenderObject _obj;

      protected SceneComponent( Geom.Point3d pos )
      {
        _pos = pos;
        _obj = HiGL.CreateDisplayList( () => this.Render() );
      }

      protected abstract void Render();

      public Color Color { get; set; }

      public double GetDepth( Graphics.ISceneContext sc )
      {
        return sc.Camera.WorldToCamera( _pos ).z;
      }

      public void Draw( Graphics.ISceneContext sc )
      {
        using ( var scope = sc.Push() ) { scope.Color = this.Color; _obj.Render(); }
      }
    }

    class ConeScene : SceneComponent
    {
      readonly Geom.Line3d _axis;

      public ConeScene( Geom.CodAxis axis )
        : this( new Geom.Line3d( Geom.Point3d.Zero, Geom.Vector3d.Basis[axis] ) )
      { }

      public ConeScene( Geom.Line3d axis )
        : base( axis.Position( 1.0 ) )
      {
        _axis = axis;
      }

      protected override void Render()
      {
        Graphics.Util.DrawSolidCone( _axis, 2 * AXIS_RADIUS, 0, 0.7, 1.0 );
      }
    }

    class CylinderScene : SceneComponent
    {
      readonly Geom.Line3d _axis;

      public CylinderScene( Geom.CodAxis axis )
        : this( new Geom.Line3d( Geom.Point3d.Zero, Geom.Vector3d.Basis[axis] ) )
      { }

      public CylinderScene( Geom.Line3d axis )
        : base( axis.Position( 0.7 ) )
      {
        _axis = axis;
      }

      protected override void Render()
      {
        Graphics.Util.DrawSolidCylinder( _axis, AXIS_RADIUS, 0.1, 0.7 );
      }
    }

    class SphereScene : SceneComponent
    {
      public SphereScene()
        : base( Geom.Point3d.Zero )
      { }

      protected override void Render()
      {
        var sphere = new Geom.Sphere3d( Geom.Point3d.Zero, 1.5 * AXIS_RADIUS );
        Graphics.Util.DrawSolidSphere( sphere );
      }
    }


    public void Draw( Graphics.ISceneContext sc )
    {
      using ( var scope = sc.Push() ) {
        scope.DepthTest = false;
        scope.BackFaceCulling = true;

        DrawBox( sc, this.BoundingBox );

        scope.Color = Color.Red;
        scope.Lighting = true;

        Geom.Point3d p = sc.Camera.ScreenToCamera( _pos );
        double len = _length * sc.Camera.LengthPerPixel;
        scope.MultMatrix( new Geom.HmMatrix3d(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            p.x / len, p.y / len, 0, 1 / len
          ) );
        scope.MultMatrix( sc.Camera.ViewingPos.r.ToMatrix() );

        // ƒfƒvƒX‚Åƒ\[ƒg‚µA‰œ‚©‚çŽè‘O‚ÉŒü‚©‚Á‚Ä‡‚É•`‰æ‚·‚é‚æ‚¤‚É‚·‚é
        foreach ( var scene in _components.OrderBy( s => s.GetDepth( sc ) ) ) {
          scene.Draw( sc );
        }
      }
    }

    public Geom.Box2i BoundingBox
    {
      get { return MakeBox( _pos, _length + 4 ); }
    }

    static Geom.Box2i MakeBox( Geom.Point2i center, int size )
    {
      Geom.Point2i lower = center - size * Geom.Vector2i.One;
      Geom.Point2i upper = center + size * Geom.Vector2i.One;
      return new Geom.Box2i( lower, upper );
    }

    static void DrawBox( Graphics.ISceneContext sc, Geom.Box2i box )
    {
      const int SHADOW = 4;

      GL.glColor4f( 0.5f, 0.5f, 0.5f, 0.5f );
      GL.glBegin( GL.GL_QUADS );
      foreach ( var p in new Geom.Point2i[] {
          // ‰E‘¤‚Ì‰e
          new Geom.Point2i( box.Upper.x, box.Lower.y + SHADOW ),
          new Geom.Point2i( box.Upper.x, box.Upper.y ),
          new Geom.Point2i( box.Upper.x + SHADOW, box.Upper.y ),
          new Geom.Point2i( box.Upper.x + SHADOW, box.Lower.y + SHADOW ),
          // ‰º‘¤‚Ì‰e
          new Geom.Point2i( box.Lower.x + SHADOW, box.Upper.y ),
          new Geom.Point2i( box.Lower.x + SHADOW, box.Upper.y + SHADOW ),
          new Geom.Point2i( box.Upper.x, box.Upper.y + SHADOW ),
          new Geom.Point2i( box.Upper.x, box.Upper.y ),
          // ‰E‰º‚Ì‰e
          new Geom.Point2i( box.Upper.x, box.Upper.y ),
          new Geom.Point2i( box.Upper.x, box.Upper.y + SHADOW ),
          new Geom.Point2i( box.Upper.x + SHADOW, box.Upper.y + SHADOW ),
          new Geom.Point2i( box.Upper.x + SHADOW, box.Upper.y )
        }.Select( pt => sc.Camera.ScreenToCamera( pt ) ) ) {
        GL.glVertex3dv( p );
      }
      GL.glEnd();

      GL.glColor4f( 1, 1, 1, 0.5f );
      GL.glBegin( GL.GL_QUADS );
      foreach ( var p in new Geom.Point2i[] {
          new Geom.Point2i( box.Lower.x, box.Lower.y ),
          new Geom.Point2i( box.Lower.x, box.Upper.y ),
          new Geom.Point2i( box.Upper.x, box.Upper.y ),
          new Geom.Point2i( box.Upper.x, box.Lower.y )
        }.Select( pt => sc.Camera.ScreenToCamera( pt ) ) ) {
        GL.glVertex3dv( p );
      }
      GL.glEnd();
    }

    Cursor GetMouseCursor( int x, int y )
    {
      return BoundingBox.Includes( new Geom.Point2i( x, y ) ) ? Cursors.SizeAll : Cursors.Arrow;
    }

    void OnMouseMove( object sender, MouseEventArgs e )
    {
      if ( e.Button == MouseButtons.None ) {
        Cursor cursor = this.GetMouseCursor( e.X, e.Y );
        if ( cursor != Cursors.Arrow ) Cursor.Current = cursor;
      }
    }

    void OnMouseDown( object sender, MouseEventArgs e )
    {
      if ( e.Button == MouseButtons.Left ) {
        Cursor cursor = this.GetMouseCursor( e.X, e.Y );
        if ( cursor == Cursors.SizeAll ) {
          Ctrl.Current.Driver.Interrupt( this.DragCompass( e.X, e.Y ) );
        }
      }
    }

    void OnSizeChanged( object sender, EventArgs e )
    {
      this.FixPos();
    }

    IEnumerator<Ctrl.IOperation> DragCompass( int x0, int y0 )
    {
      Cursor.Current = Cursors.SizeAll;
      Geom.Point2i pos0 = _pos;
      Ctrl.MouseMove drag = new Ctrl.MouseMove( _view.Events );
      while ( true ) {
        yield return drag;
        if ( drag.EventArgs.Button != MouseButtons.Left ) yield break;
        _pos.x = pos0.x + (drag.EventArgs.X - x0);
        _pos.y = pos0.y + (drag.EventArgs.Y - y0);
        this.FixPos();
        _view.Invalidate();
      }
    }

    void FixPos()
    {
      const int MARGIN = 5;
      Geom.Size2i size = _view.Camera.ScreenSize;
      Geom.Box2i box = this.BoundingBox;
      if ( box.Lower.x < MARGIN ) _pos.x += MARGIN - box.Lower.x;
      if ( box.Lower.y < MARGIN ) _pos.y += MARGIN - box.Lower.y;
      if ( box.Upper.x + MARGIN > size.x ) _pos.x -= box.Upper.x + MARGIN - size.x;
      if ( box.Upper.y + MARGIN > size.y ) _pos.y -= box.Upper.y + MARGIN - size.y;
    }
  }
}
