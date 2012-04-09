using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Hisui.Gui
{
  /// <summary>
  /// クリップ平面の方向を表す列挙型です。
  /// </summary>
  public enum ClipDirections
  {
    None,
    View, 
    PlusX, MinusX,
    PlusY, MinusY,
    PlusZ, MinusZ,
  }


  /// <summary>
  /// クリップ平面クラスです。
  /// 平面の縁をマウスでドラッグすると断面位置を移動することが出来ます。
  /// </summary>
  public class ClipPlane : Graphics.IScene, Graphics.IDecoration
  {
    static readonly Dictionary<Graphics.DocumentViews, ClipPlane>
      _registry = new Dictionary<Hisui.Graphics.DocumentViews, ClipPlane>();

    /// <summary>
    /// <see cref="Graphics.DocumentViews"/> に設定されているクリップ平面を取得します。
    /// 見つからない場合は null を返します。
    /// </summary>
    /// <param name="docviews">クリップ平面の探索先</param>
    /// <returns>設定されているクリップ平面</returns>
    public static ClipPlane Find( Graphics.DocumentViews docviews )
    {
      return docviews.WorldDocumentScene.Decorator.FindDecoration<ClipPlane>();
    }

    /// <summary>
    /// <see cref="Graphics.DocumentViews"/> に設定されているクリップ平面を取得します。
    /// 存在しない場合は、クリップ平面を生成・登録して返します。
    /// </summary>
    /// <param name="docviews">クリップ平面の登録先</param>
    /// <returns>設定されているクリップ平面</returns>
    public static ClipPlane Get( Graphics.DocumentViews docviews )
    {
      var clip = Find( docviews );
      if ( clip == null ) {
        if ( !_registry.TryGetValue( docviews, out clip ) ) {
          clip = new ClipPlane { DocumentViews = docviews };
          _registry.Add( docviews, clip );
        }
      }
      if ( clip.DocumentViews == null ) {
        clip.DocumentViews = docviews;
      }
      return clip;
    }

    /// <summary>
    /// クリップ面の設定を解除します。
    /// </summary>
    /// <param name="docviews">クリップ平面の登録先</param>
    /// <returns>もともとクリップ平面が設定されていなかった場合は false</returns>
    public static bool Remove( Graphics.DocumentViews docviews )
    {
      var clip = Find( docviews );
      if ( clip != null ) {
        clip.DocumentViews = null;
        return true;
      }
      return false;
    }

    /// <summary>
    /// 設定されているクリップ平面の向きを返します。
    /// クリップ平面が設定されていない場合は <see cref="ClipDirections.None"/> を返します。
    /// </summary>
    /// <param name="docviews">クリップ平面の探索先</param>
    /// <returns>クリップ平面の向き</returns>
    public static ClipDirections GetClipDirection( Graphics.DocumentViews docviews )
    {
      var clip = Find( docviews );
      return (clip == null) ? ClipDirections.None : clip.Direction;
    }

    /// <summary>
    /// <paramref name="docviews"/> にクリップ平面の向きを設定します。
    /// <paramref name="dir"/> に <see cref="ClipDirections.None"/> を指定するとクリップ平面が削除されます。
    /// </summary>
    /// <param name="docviews">クリップ平面の登録先</param>
    /// <param name="dir">クリップ平面の向き</param>
    public static void SetClipDirection( Graphics.DocumentViews docviews, ClipDirections dir )
    {
      if ( dir == ClipDirections.None ) {
        Remove( docviews );
      }
      else {
        var clip = Get( docviews );
        if ( clip.Direction != dir ) {
          clip.Direction = dir;
          clip.Position = (0.5).Clamp();
        }
      }
    }

    /// <summary>
    /// 断面のキャッピング（蓋をした表示）の有効/無効を取得/設定します。
    /// </summary>
    public static bool Capping
    {
      get { return Graphics.ClipPlane.Instances[ClipPlaneNo].Capping; }
      set { Graphics.ClipPlane.Instances[ClipPlaneNo].Capping = value; }
    }

    /// <summary>
    /// このクラスが使用する OpenGL のクリップ平面の番号で、5 と定義されています。
    /// </summary>
    public const int ClipPlaneNo = 5;

    readonly Graphics.ClipPlane _clip = Graphics.ClipPlane.Instances[ClipPlaneNo];
    Graphics.DocumentViews _docviews;
    int _highlightIndex = -1;

    public ClipPlane()
    {
      this.Position = (0.5).Clamp();
      this.Direction = ClipDirections.PlusX;
    }

    /// <summary>
    /// クリップ平面の位置を 0～1 の値で取得/設定します。
    /// </summary>
    public Hisui.Geom.Clamp Position { get; set; }

    /// <summary>
    /// クリップ平面の向きを取得/設定します。
    /// </summary>
    public ClipDirections Direction { get; set; }

    /// <summary>
    /// クリップ平面の登録先を取得/設定します。
    /// </summary>
    public Graphics.DocumentViews DocumentViews
    {
      get { return _docviews; }
      set
      {
        if ( _docviews != value ) {
          if ( _docviews != null ) {
            _docviews.WorldDocumentScene.Decorator.RemoveDecoration<ClipPlane>();
            _docviews.WorldScenes.Remove( this );
            _docviews.Events.MouseDown -= this.OnMouseDown;
            _docviews.Events.MouseMove -= this.OnMouseMove;
          }
          _docviews = value;
          if ( _docviews != null ) {
            _docviews.WorldDocumentScene.Decorator.PutDecoration( this );
            _docviews.WorldScenes.Add( this );
            _docviews.Events.MouseDown += this.OnMouseDown;
            _docviews.Events.MouseMove += this.OnMouseMove;
          }
        }
      }
    }

    public void PreDraw( Hisui.Graphics.ISceneContext sc )
    {
      if ( this.Direction == ClipDirections.None ) return;
      var sphere = _docviews.WorldDocumentScene.BoundingSphere;
      var range = new Hisui.Geom.Range( -sphere.Radius, sphere.Radius );
      var codsys = Geom.CodSys3d.Unit;
      switch ( this.Direction ) {
        case ClipDirections.View:
          codsys.r = sc.Camera.ViewingPos.r;
          codsys.SetRotation( codsys.v, codsys.u );
          break;
        case ClipDirections.PlusX:
          codsys.SetRotation( Geom.Vector3d.Ey, Geom.Vector3d.Ez );
          break;
        case ClipDirections.PlusY:
          codsys.SetRotation( Geom.Vector3d.Ez, Geom.Vector3d.Ex );
          break;
        case ClipDirections.PlusZ:
          codsys.SetRotation( Geom.Vector3d.Ex, Geom.Vector3d.Ey );
          break;
        case ClipDirections.MinusX:
          codsys.SetRotation( Geom.Vector3d.Ez, Geom.Vector3d.Ey );
          break;
        case ClipDirections.MinusY:
          codsys.SetRotation( Geom.Vector3d.Ex, Geom.Vector3d.Ez );
          break;
        case ClipDirections.MinusZ:
          codsys.SetRotation( Geom.Vector3d.Ey, Geom.Vector3d.Ex );
          break;
      }
      codsys.o = sphere.Center + range.ParamToValue( this.Position ) * codsys.n;
      _clip.CodSys = codsys;
      _clip.PreDraw( sc );
    }

    public void PostDraw( Hisui.Graphics.ISceneContext sc )
    {
      if ( this.Direction == ClipDirections.None ) return;
      _clip.PostDraw( sc );
    }

    public void Draw( Graphics.ISceneContext sc )
    {
      if ( this.Direction == ClipDirections.None ) return;
      using ( var scope = sc.Push() ) {
        var epsilon = 1.0e-3 * _docviews.WorldDocumentScene.BoundingSphere.Radius;
        var codsys = _clip.CodSys;
        codsys.o -= epsilon * codsys.n;
        scope.MultMatrix( codsys );
        scope.Color = Color.FromArgb( 16, Color.Lime );
        scope.DepthMask = false;
        scope.Lighting = false;
        var radius = _docviews.WorldDocumentScene.BoundingSphere.Radius;
        sc.DrawQuads( gl =>
        {
          gl.Vertex( radius, radius );
          gl.Vertex( -radius, radius );
          gl.Vertex( -radius, -radius );
          gl.Vertex( radius, -radius );
        } );
        scope.Color = (_highlightIndex == -1) ? Color.Lime : Color.Orange;
        sc.DrawLineLoop( gl =>
        {
          gl.Vertex( radius, radius );
          gl.Vertex( -radius, radius );
          gl.Vertex( -radius, -radius );
          gl.Vertex( radius, -radius );
        } );
      }
    }

    void OnMouseDown( object sender, MouseEventArgs e )
    {
      if ( this.Direction == ClipDirections.None ) return;
      if ( _highlightIndex != -1 && e.Button == MouseButtons.Left ) {
        SI.Driver.Interrupt( this.DragPlane( (Graphics.IView)sender, e ) );
      }
    }

    void OnMouseMove( object sender, MouseEventArgs e )
    {
      if ( this.Direction == ClipDirections.None ) return;
      if ( e.Button == MouseButtons.None ) {
        _highlightIndex = -1;
        var view = (Graphics.IView)sender;
        var length = this.GetQuadPoints().CyclicEach2().Select( ( p1, p2 ) => (p2 - p1).Length ).ToArray();
        var planes = this.GetQuadPlanes();
        var eyeshot = view.Camera.GetEyeshotLine( e.Location );
        var tole = Hisui.Ctrl.Pick.APERTURE * view.Camera.LengthPerPixel;
        for ( int i = 0 ; i < 4 ; ++i ) {
          //if ( GeomUT.Dot( planes[i].n, eyeshot.Direction ) > 0 ) continue;
          var uv = GetIntersection( planes[i], eyeshot );
          if ( Math.Abs( uv.y ) < tole && 0 <= uv.x && uv.x <= length[i] ) {
            _highlightIndex = i;
            break;
          }
        }
        view.Invalidate();
      }
    }

    IEnumerator<Ctrl.IOperation> DragPlane( Graphics.IView view, MouseEventArgs e )
    {
      var plane = this.GetQuadPlanes()[_highlightIndex];
      var sphere = _docviews.WorldDocumentScene.BoundingSphere;
      var range = new Hisui.Geom.Range( -sphere.Radius, sphere.Radius );
      var z0 = range.ParamToValue( this.Position );

      var eyeshot1 = view.Camera.GetEyeshotLine( e.Location );
      var y1 = GetIntersection( plane, eyeshot1 ).y;

      var up = new Ctrl.LButtonUp( view.Events );
      up.MouseMove += ( ss, ee ) =>
      {
        var eyeshot2 = view.Camera.GetEyeshotLine( ee.Location );
        var z = z0 + GetIntersection( plane, eyeshot2 ).y - y1;
        this.Position = range.ValueToParam( z );
        view.Invalidate();
      };
      yield return up;
      view.Invalidate();
    }


    Hisui.Geom.Point3d[] GetQuadPoints()
    {
      var radius = _docviews.WorldDocumentScene.BoundingSphere.Radius;
      var points = CoreUT.MakeArray(
        GeomUT.P( radius, radius ),
        GeomUT.P( -radius, radius ),
        GeomUT.P( -radius, -radius ),
        GeomUT.P( radius, -radius ) );
      return CoreUT.MakeArray( 4, i => _clip.CodSys.Globalize( points[i] ) );
    }


    Hisui.Geom.CodSys3d[] GetQuadPlanes()
    {
      var points = this.GetQuadPoints();
      var planes = new Hisui.Geom.CodSys3d[4];
      for ( int i = 0 ; i < 4 ; ++i ) {
        var u = (points[(i + 1) % 4] - points[i]).Normalize();
        var v = _clip.CodSys.n;
        planes[i] = new Hisui.Geom.CodSys3d( u, v, points[i] );
      }
      return planes;
    }


    static Hisui.Geom.Point2d GetIntersection( Hisui.Geom.CodSys3d plane, Hisui.Geom.Line3d line )
    {
      var t = GeomUT.Dot( plane.o - line.Origin, plane.n ) / GeomUT.Dot( line.Direction, plane.n );
      var r = line.Position( t ) - plane.o;
      return GeomUT.P( GeomUT.Dot( plane.u, r ), GeomUT.Dot( plane.v, r ) );
    }
  }
}
