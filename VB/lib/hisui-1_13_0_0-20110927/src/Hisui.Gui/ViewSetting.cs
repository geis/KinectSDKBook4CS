using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing ;
using System.Windows.Forms;

namespace Hisui.Gui
{
  /// <summary>
  /// ビュー設定を表すクラスです。
  /// 各種設定をプロパティで set/get 出来るようになっており、
  /// <see cref="PropertyGrid"/> で簡便に編集GUIを作ることが出来ます。
  /// </summary>
  public class ViewSetting : Core.BreathObject
  {
    static readonly Dictionary<Graphics.DocumentViews, ViewSetting>
      _registry = new Dictionary<Hisui.Graphics.DocumentViews, ViewSetting>();

    /// <summary>
    /// <see cref="SI.DocumentViews"/> に対応する設定情報を取得します。
    /// </summary>
    public static ViewSetting Current
    {
      get { return Get( SI.DocumentViews ); }
    }

    public static SI.ApplicationData DataSource
    {
      get { return ViewSettingData.DataSource; }
      set { ViewSettingData.DataSource = value; }
    }

    /// <summary>
    /// 指定された <see cref="Graphics.DocumentViews"/> に対応する設定情報を取得します。
    /// 未登録の場合はインスタンスを生成・登録して返します。
    /// </summary>
    /// <param name="docviews"></param>
    /// <returns></returns>
    public static ViewSetting Get( Graphics.DocumentViews docviews )
    {
      ViewSetting instance = null;
      if ( docviews != null && !_registry.TryGetValue( docviews, out instance ) ) {
        instance = new ViewSetting( docviews );
        ViewSettingData.Instance.CopyTo( instance );
        _registry[docviews] = instance;
      }
      return instance;
    }

    /// <summary>
    /// <see cref="ViewSettingData"/> に <paramref name="docviews"/> の設定値情報をコピーします。
    /// </summary>
    /// <param name="docviews"></param>
    public static void Save( Graphics.DocumentViews docviews )
    {
      if ( _registry.ContainsKey( docviews ) ) _registry[docviews].Save();
    }

    /// <summary>
    /// <see cref="ViewSettingData"/> から設定値情報を取得し <paramref name="docviews"/> に反映します。
    /// </summary>
    /// <param name="docviews"></param>
    public static void Restore( Graphics.DocumentViews docviews )
    {
      Get( docviews ).Restore();
    }

    /// <summary>
    /// 指定されたフォルダに保存された設定ファイルから値を取得し <paramref name="docviews"/> に反映します。
    /// </summary>
    public static void Load( Graphics.DocumentViews docviews, string folderpath )
    {
      Get( docviews ).Load( folderpath );
    }


    readonly Graphics.DocumentViews _docviews;
    readonly LightSetting[] _lights;
    readonly Graphics.StandardViewOperation.Setting _setting = new Hisui.Graphics.StandardViewOperation.Setting();
    bool _compass = true;
    bool _ruler = true;
    Graphics.IScene _axis = new Graphics.Scenes.AxisScene();

    ViewSetting( Graphics.DocumentViews docviews )
    {
      _docviews = docviews;
      _lights = CoreUT.MakeArray( 4, i => new LightSetting( Graphics.Light.Lights[i] ) );
    }

    /// <summary>
    /// <see cref="ViewSettingData"/> に自身の設定値情報をコピーします。
    /// </summary>
    public void Save()
    {
      ViewSettingData.Instance.CopyFrom( this );
    }

    /// <summary>
    /// <see cref="ViewSettingData"/> から設定値情報を取得し自身に反映します。
    /// </summary>
    public void Restore()
    {
      ViewSettingData.Instance.CopyTo( this );
    }

    /// <summary>
    /// 指定されたフォルダに保存された設定ファイルから値を取得し自身に反映します。
    /// </summary>
    /// <param name="folderpath">設定ファイルの保存されているフォルダ</param>
    public void Load( string folderpath )
    {
      SI.AppData[folderpath].Get<ViewSettingData>().CopyTo( this );
    }

    /// <summary>
    /// <see cref="ViewOperationSetting"/> で設定変更可能な <see cref="Graphics.ViewOperation"/> を生成します。
    /// 引数には右クリックで表示されるコンテキストメニューの生成関数を指定します。
    /// </summary>
    /// <param name="createMenu">右クリックで表示されるコンテキストメニューの生成関数</param>
    /// <returns>生成されたビューオペレーション</returns>
    public Graphics.ViewOperation CreateViewOperation( Func<ContextMenuStrip> createMenu )
    {
      return new ViewOperationWithContextMenu( _docviews.Document, _setting, createMenu );
    }

    /// <summary>
    /// ビューオペレーションのキーバインドを取得します。
    /// </summary>
    public Graphics.ViewOperationBindingCollection Bindings
    {
      get { return _setting.Bindings; }
    }

    /// <summary>
    /// ビューオペレーションの設定情報を取得します。
    /// </summary>
    [System.ComponentModel.Browsable( false )]
    public Graphics.StandardViewOperation.Setting ViewOperationSetting
    {
      get { return _setting; }
    }

    [System.ComponentModel.Browsable( false )]
    public Graphics.Material Material
    {
      get { return _docviews.WorldDocumentScene.Material; }
      set { _docviews.WorldDocumentScene.Material = value; this.Touch(); }
    }

    public Color Color
    {
      get { return _docviews.WorldDocumentScene.Color; }
      set { _docviews.WorldDocumentScene.Color = value; this.Touch(); }
    }

    public Color SpecularColor
    {
      get { return _docviews.WorldDocumentScene.SpecularColor; }
      set { _docviews.WorldDocumentScene.SpecularColor = value; Touch(); }
    }

    public double Shininess
    {
      get { return _docviews.WorldDocumentScene.Shininess; }
      set { _docviews.WorldDocumentScene.Shininess = value; Touch(); }
    }

    public double Opacity
    {
      get { return _docviews.WorldDocumentScene.Opacity; }
      set { _docviews.WorldDocumentScene.Opacity = value; Touch(); }
    }

    public Graphics.PolygonStyles PolygonStyle
    {
      get { return _docviews.WorldDocumentScene.PolygonStyle; }
      set { _docviews.WorldDocumentScene.PolygonStyle = value; this.Touch(); }
    }

    public string ShaderName
    {
      get { return _docviews.WorldDocumentScene.ShaderName; }
      set { _docviews.WorldDocumentScene.ShaderName = value; }
    }

    public bool Backside
    {
      get { return !_docviews.WorldDocumentScene.BackFaceCulling; }
      set { _docviews.WorldDocumentScene.BackFaceCulling = !value; this.Touch(); }
    }

    public Color BackgroundColor
    {
      get { return _docviews.BackColor; }
      set { _docviews.BackColor = value; this.Touch(); }
    }

    public bool Perspective
    {
      get { return _docviews.Perspective; }
      set { _docviews.Perspective = value; this.Touch(); }
    }

    public LightSetting[] Lights
    {
      get { return _lights; }
    }

    public bool UseVertexBuffer
    {
      get { return OpenGL.HiGL.UseVertexBuffer; }
      set { OpenGL.HiGL.UseVertexBuffer = value; }
    }

    public bool UseDrawElements
    {
      get { return OpenGL.HiGL.UseDrawElements; }
      set { OpenGL.HiGL.UseDrawElements = value; }
    }

    public bool EnableCameraAnimation
    {
      get { return GraphicsUT.EnableCameraAnimation; }
      set { GraphicsUT.EnableCameraAnimation = value; }
    }

    public float PolygonOffsetFactor
    {
      get { return GraphicsUT.PolygonOffsetFactor; }
      set { GraphicsUT.PolygonOffsetFactor = value; }
    }

    public float PolygonOffsetUnits
    {
      get { return GraphicsUT.PolygonOffsetUnits; }
      set { GraphicsUT.PolygonOffsetUnits = value; }
    }

    public int SelectionBufferSize
    {
      get { return GraphicsUT.SelectionBufferSize; }
      set { GraphicsUT.SelectionBufferSize = value; }
    }

    public int DragZoomSign
    {
      get { return _setting.DragZoomSign; }
      set { _setting.DragZoomSign = value; }
    }

    public int WheelZoomSign
    {
      get { return _setting.WheelZoomSign; }
      set { _setting.WheelZoomSign = value; }
    }

    public Graphics.StandardViewOperation.ZoomScale LowerZoomScale
    {
      get { return _setting.LowerZoomScale; }
      set { _setting.LowerZoomScale = value; }
    }

    public Graphics.StandardViewOperation.ZoomScale UpperZoomScale
    {
      get { return _setting.UpperZoomScale; }
      set { _setting.UpperZoomScale = value; }
    }

    public bool AutoFocus
    {
      get { return _setting.AutoFocus; }
      set { _setting.AutoFocus = value; }
    }

    public bool ZoomToMousePosition
    {
      get { return _setting.ZoomToMousePosition; }
      set { _setting.ZoomToMousePosition = value; }
    }

    public bool ZKeptRotation
    {
      get { return _setting.ZKeptRotation; }
      set { _setting.ZKeptRotation = value; }
    }

    public Graphics.IScene AxisScene
    {
      get { return _axis; }
      set
      {
        if ( _axis != value ) {
          bool shown = (_axis != null && _docviews.WorldScenes.Remove( _axis ));
          _axis = value;
          if ( shown && _axis != null ) _docviews.WorldScenes.Add( _axis );
        }
      }
    }

    public bool ShowAxis
    {
      get { return this.AxisScene != null && _docviews.WorldScenes.Contains( this.AxisScene ); }
      set
      {
        if ( this.AxisScene != null && this.ShowAxis != value ) {
          if ( value )
            _docviews.WorldScenes.Add( this.AxisScene );
          else
            _docviews.WorldScenes.Remove( this.AxisScene );
        }
      }
    }

    public bool ShowCompass
    {
      get { return _compass; }
      set
      {
        if ( _compass != value ) {
          _compass = value;
          if ( _compass )
            CompassScene.PutTo( _docviews );
          else
            CompassScene.RemoveFrom( _docviews );
        }
      }
    }

    public bool ShowRuler
    {
      get { return _ruler; }
      set
      {
        if ( _ruler != value ) {
          _ruler = value;
          if ( _ruler )
            RulerScene.PutTo( _docviews );
          else
            RulerScene.RemoveFrom( _docviews );
        }
      }
    }

    public Graphics.ViewOperationBinding ZoomBinding
    {
      get { return _setting.Bindings.Find( Graphics.ViewOperations.Zoom ); }
    }

    public Graphics.ViewOperationBinding PanBinding
    {
      get { return _setting.Bindings.Find( Graphics.ViewOperations.Pan ); }
    }

    public Graphics.ViewOperationBinding RotationBinding
    {
      get { return _setting.Bindings.Find( Graphics.ViewOperations.Rotation ); }
    }

    public Graphics.ViewOperationBinding SpinBinding
    {
      get { return _setting.Bindings.Find( Graphics.ViewOperations.Spin ); }
    }

    public Graphics.ViewOperationBinding FocusBinding
    {
      get { return _setting.Bindings.Find( Graphics.ViewOperations.Focus ); }
    }

    public Graphics.ViewOperationBinding PerspectiveBinding
    {
      get { return _setting.Bindings.Find( Graphics.ViewOperations.Perspective ); }
    }

  }
}
