using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Hisui.Gui
{
  public partial class ViewSettingDialog : Form
  {
    readonly Graphics.DocumentViews _docviews;
    readonly ViewSetting _setting;
    Geom.Point2i _prevPos;

    class WorldSphere : Graphics.IScene, Geom.IBoundary3d
    {
      void Graphics.IScene.Draw( Graphics.ISceneContext con ) { }

      public Geom.Box3d BoundingBox
      {
        get { return BoundingSphere.BoundingBox; }
      }

      public Geom.Sphere3d BoundingSphere
      {
        get { return new Geom.Sphere3d( Geom.Point3d.Zero, 3.0 ); }
      }
    }

    public ViewSettingDialog( Graphics.DocumentViews docviews, ViewSetting setting )
    {
      InitializeComponent();
      
      _docviews = docviews;
      _setting = setting;

      propertyGrid1.SelectedObject = _setting;
      viewControl.BackColor = _setting.BackgroundColor;

      viewControl.SceneGraph.WorldScenes.Add( new WorldSphere() );

      viewControl.SceneGraph.WorldScenes.Add(
          delegate( Graphics.ISceneContext context )
          {
            using ( var scope = context.Push() ) {
              scope.Color = this.ActiveLight.Enabled ? _setting.Color : Color.LightGray;
              Graphics.Util.DrawSolidSphere(
                new Geom.Sphere3d( Geom.Point3d.Zero, 1.0 ) );
            }
          }
        );

      viewControl.SceneGraph.CameraScenes.Add(
          delegate( Graphics.ISceneContext context )
          {
            if ( !this.ActiveLight.Enabled ) return;

            Geom.HmCod3d p = this.ActiveLight.Position;
            Geom.Vector3d d = new Geom.Vector3d( p.X, p.Y, p.Z ).Normalize();

            Geom.CodSys3d sys = new Geom.CodSys3d( d, new Geom.Point3d( d.x, d.y, d.z ) );
            Geom.Point3d top = Geom.Point3d.Zero;
            Geom.Point3d end = new Geom.Point3d( 0, 0, 2 );
            Geom.Point3d px1 = new Geom.Point3d( 0.3, 0, 0.8 );
            Geom.Point3d px2 = new Geom.Point3d( -0.3, 0, 0.8 );
            Geom.Point3d py1 = new Geom.Point3d( 0, 0.3, 0.8 );
            Geom.Point3d py2 = new Geom.Point3d( 0, -0.3, 0.8 );
            top = sys.Globalize( top );
            end = sys.Globalize( end );
            px1 = sys.Globalize( px1 );
            px2 = sys.Globalize( px2 );
            py1 = sys.Globalize( py1 );
            py2 = sys.Globalize( py2 );

            using ( var scope = context.Push() ) {
              scope.Color = Color.White;
              context.DrawLineStrip( gl => gl.Vertices( end, top, px1, px2, top, py1, py2, top ) );
            }
          }
        );

      this.ActiveLightChanged( null, EventArgs.Empty );
      viewControl.Fit();
    }

    int ActiveLightIndex
    {
      get
      {
        if ( radio0.Checked ) return 0;
        if ( radio1.Checked ) return 1;
        if ( radio2.Checked ) return 2;
        if ( radio3.Checked ) return 3;
        return -1;
      }
    }

    Graphics.Light ActiveLight
    {
      get { return Graphics.Light.Lights[ActiveLightIndex]; }
    }

    void SetLightViewBackColor()
    {
      viewControl.BackColor =
        ActiveLight.Enabled ? _setting.BackgroundColor : Color.Gray;
    }

    private void propertyGrid1_PropertyValueChanged( object s, PropertyValueChangedEventArgs e )
    {
      this.SetLightViewBackColor();
      Core.Builder.Build();
      viewControl.Invalidate();
      _docviews.Invalidate();
    }

    void ActiveLightChanged( object sender, EventArgs e )
    {
      checkEnabled.Checked = ActiveLight.Enabled;
      this.SetLightViewBackColor();
      viewControl.Invalidate();
      _docviews.Invalidate();
    }

    void LightEnabledChanged( object sender, EventArgs e )
    {
      ActiveLight.Enabled = checkEnabled.Checked;
      this.SetLightViewBackColor();
      viewControl.Invalidate();
      _docviews.Invalidate();
    }

    void viewControl_MouseMove( object sender, MouseEventArgs e )
    {
      Geom.Point2i p = new Geom.Point2i( e.X, e.Y );
      if ( (e.Button & MouseButtons.Left) != 0 && p != _prevPos ) {
        Graphics.ICamera camera = viewControl.Camera;
        Geom.Vector3d vec = camera.ScreenToCamera( p - _prevPos );
        Geom.Vector3d axis = Geom.Vector3d.Ez * vec;
        double depth = 0.5 * camera.ViewingDepth;
        double theta = System.Math.Atan2( vec.Length, depth );
        Geom.Rotation3d rot =
          new Geom.Rotation3d( axis.Normalize(), theta );

        Geom.HmCod3d c = this.ActiveLight.Position;
        Geom.Vector3d d = new Geom.Vector3d( c.X, c.Y, c.Z );
        d = rot.Rotate( d );
        this.ActiveLight.Position = new Geom.HmCod3d( d.x, d.y, d.z, 0.0 );
        viewControl.Invalidate();
        _docviews.Invalidate();
      }
      _prevPos = p;
    }
  }
}
