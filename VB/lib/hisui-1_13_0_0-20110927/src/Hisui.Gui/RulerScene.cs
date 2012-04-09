using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms ;
using System.Drawing ;
using Hisui.Graphics;
using Hisui.OpenGL;

namespace Hisui.Gui
{
  /*
   *  ルーラーを描画する
   */
  public class RulerScene : IScene
  {
    public static void PutTo( Graphics.DocumentViews docviews )
    {
      foreach ( var view in docviews.Views ) PutTo( view );
    }

    public static void PutTo( Graphics.IView view )
    {
      if ( !view.SceneGraph.ForgroundScenes.Any( scene => scene is RulerScene ) ) {
        view.SceneGraph.ForgroundScenes.Add( new RulerScene { View = view } );
      }
    }

    public static void RemoveFrom( Graphics.DocumentViews docviews )
    {
      foreach ( var view in docviews.Views ) RemoveFrom( view );
    }

    public static void RemoveFrom( Graphics.IView view )
    {
      view.SceneGraph.ForgroundScenes
        .Where( scene => scene is RulerScene )
        .ForEach( scene => ((RulerScene)scene).View = null );
      view.SceneGraph.ForgroundScenes.RemoveAll( scene => scene is RulerScene );
    }


    const int MINPOS = 4;

    // ルーラーの中心位置
    Geom.Point2i _pos;

    // ビューのサイズ
    Geom.Size2i _size;

    // ルーラーを描画するコントロール
    IView _owner;


    private RulerScene() { }

    public Graphics.IView View
    {
      get { return _owner; }
      set
      {
        if ( _owner != value ) {
          if ( _owner != null ) {
            _owner.Events.MouseDown -= OnMouseDown;
            _owner.Events.MouseMove -= OnMouseMove;
            _owner.Events.SizeChanged -= OnSizeChanged;
          }
          _owner = value;
          if ( _owner != null ) {
            _owner.Events.MouseDown += OnMouseDown;
            _owner.Events.MouseMove += OnMouseMove;
            _owner.Events.SizeChanged += OnSizeChanged;
            _size = _owner.Camera.ScreenSize;
            _pos = new Geom.Point2i( 16, _size.y - 16 );
          }
        }
      }
    }


    public void Draw( ISceneContext context )
    {
      _size = context.Camera.ScreenSize;
      using ( var whole = context.Push() ) {
        whole.Color = Color.Gray;
        whole.PointSize = 3;
        whole.LineWidth = 1;

        // X, Y 軸（直線）の描画
        context.DrawLines( gl =>
          {
            gl.Vertex( 0, _pos.y );
            gl.Vertex( _size.x, _pos.y );
            gl.Vertex( _pos.x, 0 );
            gl.Vertex( _pos.x, _size.y );
          } );

        // 目盛の間隔
        const int P0 = 16; // 目盛の最小ピクセル数
        double unit = context.Camera.LengthPerPixel;
        double exp = Math.Log10( P0 * unit );
        double delta = Math.Pow( 10, Math.Ceiling( exp ) );
        if ( delta / unit > 5 * P0 ) delta /= 2;

        Geom.Box2d box = new Geom.Box2d(
            new Geom.Point2d( -_pos.x * unit, (_pos.y - _size.y) * unit ),
            new Geom.Point2d( (_size.x - _pos.x) * unit, _pos.y * unit )
          );

        // 目盛を点列として描画
        using ( var gl = HiGL.Begin( GLPrimType.Points ) ) {
          for ( double x = 0.0 ; x < box.Upper.x ; x += delta ) {
            gl.Vertex( _pos.x + (int)(x / unit), _pos.y );
          }
          for ( double x = 0.0 ; x > box.Lower.x ; x -= delta ) {
            gl.Vertex( _pos.x + (int)(x / unit), _pos.y );
          }
          for ( double y = 0.0 ; y < box.Upper.y ; y += delta ) {
            gl.Vertex( _pos.x, _pos.y - (int)(y / unit) );
          }
          for ( double y = 0.0 ; y > box.Lower.y ; y -= delta ) {
            gl.Vertex( _pos.x, _pos.y - (int)(y / unit) );
          }
        }

        // 目盛の数値を描画
        for ( double x = 2 * delta ; x < box.Upper.x ; x += 2 * delta ) {
          DrawNumber( context, _pos.x + (int)(x / unit), _pos.y + 16, x );
        }
        for ( double x = -2 * delta ; x > box.Lower.x ; x -= 2 * delta ) {
          DrawNumber( context, _pos.x + (int)(x / unit), _pos.y + 16, x );
        }
        for ( double y = 2 * delta ; y < box.Upper.y ; y += 2 * delta ) {
          DrawNumber( context, _pos.x + 2, _pos.y - (int)(y / unit), y );
        }
        for ( double y = -2 * delta ; y > box.Lower.y ; y -= 2 * delta ) {
          DrawNumber( context, _pos.x + 2, _pos.y - (int)(y / unit), y );
        }

        // 分度器の描画
        double radius = 100;
        using ( var gl = HiGL.Begin( GLPrimType.LineStrip ) ) {
          for ( int deg = 0 ; deg <= 90 ; deg += 5 ) {
            double rad = Math.PI * deg / 180.0;
            double x = _pos.x + radius * Math.Cos( rad );
            double y = _pos.y - radius * Math.Sin( rad );
            gl.Vertex( x, y );
          }
        }
        using ( var gl = HiGL.Begin( GLPrimType.Points ) ) {
          for ( int deg = 15 ; deg < 90 ; deg += 15 ) {
            double rad = Math.PI * deg / 180.0;
            double x = _pos.x + radius * Math.Cos( rad );
            double y = _pos.y - radius * Math.Sin( rad );
            gl.Vertex( x, y );
          }
        }
      }
    }

    static void DrawNumber( Graphics.ISceneContext sc, int x, int y, double num )
    {
      sc.DrawStrokeRoman( num.ToString(), x, y, 0.1 );
    }

    Cursor GetMouseCursor( int x, int y )
    {
      bool Y_axis = (_pos.x - 4 < x && x < _pos.x + 4);
      bool X_axis = (_pos.y - 4 < y && y < _pos.y + 4);
      if ( X_axis && Y_axis ) return Cursors.SizeAll;
      if ( X_axis ) return Cursors.HSplit;
      if ( Y_axis ) return Cursors.VSplit;
      return Cursors.Arrow;
    }

    void FixPos()
    {
      if ( _pos.x < MINPOS ) _pos.x = MINPOS;
      if ( _pos.y < MINPOS ) _pos.y = MINPOS;
      if ( _pos.x > _size.x - MINPOS ) _pos.x = _size.x - MINPOS;
      if ( _pos.y > _size.y - MINPOS ) _pos.y = _size.y - MINPOS;
    }

    void OnMouseDown( object sender, MouseEventArgs e )
    {
      if ( e.Button != MouseButtons.Left ) return;
      Cursor cursor = this.GetMouseCursor( e.X, e.Y );
      if ( cursor != Cursors.Arrow ) {
        Cursor.Current = cursor;
        Ctrl.Current.Driver.Interrupt( this.DragRuler( Cursor.Current ) );
      }
    }

    void OnMouseMove( object sender, MouseEventArgs e )
    {
      if ( e.Button == MouseButtons.None ) {
        Cursor cursor = this.GetMouseCursor( e.X, e.Y );
        if ( cursor != Cursors.Arrow ) Cursor.Current = cursor;
      }
    }

    void OnSizeChanged( object sender, EventArgs e )
    {
      double ratio_x = _size.x > 0 ? (double)_pos.x / _size.x : _pos.x;
      double ratio_y = _size.y > 0 ? (double)_pos.y / _size.y : _pos.y;
      _size = _owner.Camera.ScreenSize;
      _pos.x = (int)(ratio_x * _size.x);
      _pos.y = (int)(ratio_y * _size.y);
      this.FixPos();
    }

    IEnumerator<Ctrl.IOperation> DragRuler( Cursor cursor )
    {
      if ( cursor == Cursors.VSplit ) return DragRulerX();
      if ( cursor == Cursors.HSplit ) return DragRulerY();
      if ( cursor == Cursors.SizeAll ) return DragRulerXY();
      return null;
    }

    IEnumerator<Ctrl.IOperation> DragRulerX()
    {
      Ctrl.MouseMove drag = new Ctrl.MouseMove( _owner.Events );
      while ( true ) {
        yield return drag;
        if ( drag.EventArgs.Button != MouseButtons.Left ) yield break;
        _pos.x = drag.EventArgs.X;
        this.FixPos();
        _owner.Invalidate();
      }
    }

    IEnumerator<Ctrl.IOperation> DragRulerY()
    {
      Ctrl.MouseMove drag = new Ctrl.MouseMove( _owner.Events );
      while ( true ) {
        yield return drag;
        if ( drag.EventArgs.Button != MouseButtons.Left ) yield break;
        _pos.y = drag.EventArgs.Y;
        this.FixPos();
        _owner.Invalidate();
      }
    }

    IEnumerator<Ctrl.IOperation> DragRulerXY()
    {
      Ctrl.MouseMove drag = new Ctrl.MouseMove( _owner.Events );
      while ( true ) {
        yield return drag;
        if ( drag.EventArgs.Button != MouseButtons.Left ) yield break;
        _pos.x = drag.EventArgs.X;
        _pos.y = drag.EventArgs.Y;
        this.FixPos();
        _owner.Invalidate();
      }
    }
  }
}
