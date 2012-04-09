using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace Hisui.Gui
{
  /// <summary>
  /// 光源設定を表すクラスです。
  /// <see cref="Graphics.Light"/> クラスのラッパーです。
  /// </summary>
  [TypeConverter( typeof( ExpandableObjectConverter ) )]
  public class LightSetting
  {
    Graphics.Light _light;

    /// <summary>
    /// コンストラクタ。
    /// ラップ対象の光源を指定して構築します。
    /// </summary>
    /// <param name="light">ラップ対象の光源</param>
    public LightSetting( Graphics.Light light )
    {
      _light = light;
    }

    /// <summary>
    /// 光源のON/OFFを set/get します。
    /// </summary>
    public bool Enabled
    {
      get { return _light.Enabled; }
      set { _light.Enabled = value; }
    }

    /// <summary>
    /// 光源をワールド座標系に置くかカメラ座標系に置くかを set/get します。
    /// <see cref="Position"/>プロパティがワールド座標系の座標とみなすか、カメラ座標系の座標とみなすか、が設定できます。
    /// </summary>
    public bool IsWorldCoordinate
    {
      get { return _light.IsWorldCoordinate; }
      set { _light.IsWorldCoordinate = value; }
    }

    /// <summary>
    /// 環境光を set/get します。
    /// </summary>
    public Color AmbientColor
    {
      get { return _light.AmbientColor; }
      set { _light.AmbientColor = value; }
    }

    /// <summary>
    /// 拡散光を set/get します。
    /// </summary>
    public Color DiffuseColor
    {
      get { return _light.DiffuseColor; }
      set { _light.DiffuseColor = value; }
    }

    /// <summary>
    /// 反射光を set/get します。
    /// </summary>
    public Color SpecularColor
    {
      get { return _light.SpecularColor; }
      set { _light.SpecularColor = value; }
    }

    /// <summary>
    /// 光源の位置を同次座標で set/get します。
    /// <c>Position.w == 0</c> の場合は無限遠点に置かれた光源と解釈され、平行光源となります。
    /// <c>Position.w != 0</c> の場合は点光源となります。
    /// </summary>
    public Geom.HmCod3d Position
    {
      get { return _light.Position; }
      set { _light.Position = value; }
    }
  }
}
