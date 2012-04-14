using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml;

namespace Hisui.Gui
{
  class LightSettingData
  {
    public bool Enabled;
    public bool IsWorldCoordinate;
    public Color AmbientColor;
    public Color DiffuseColor;
    public Color SpecularColor;
    public Geom.HmCod3d Position;

    public LightSettingData( Graphics.Light src )
    {
      this.Enabled = src.Enabled;
      this.IsWorldCoordinate = src.IsWorldCoordinate;
      this.AmbientColor = src.AmbientColor;
      this.DiffuseColor = src.DiffuseColor;
      this.SpecularColor = src.SpecularColor;
      this.Position = src.Position;
    }

    LightSettingData() { }

    public void CopyTo( LightSetting ls )
    {
      ls.Enabled = this.Enabled;
      ls.IsWorldCoordinate = this.IsWorldCoordinate;
      ls.AmbientColor = this.AmbientColor;
      ls.DiffuseColor = this.DiffuseColor;
      ls.SpecularColor = this.SpecularColor;
      ls.Position = this.Position;
    }

    public void CopyFrom( LightSetting ls )
    {
      this.Enabled = ls.Enabled;
      this.IsWorldCoordinate = ls.IsWorldCoordinate;
      this.AmbientColor = ls.AmbientColor;
      this.DiffuseColor = ls.DiffuseColor;
      this.SpecularColor = ls.SpecularColor;
      this.Position = ls.Position;
    }

    static LightSettingData()
    {
      Hix.SerializerFactory.Register( typeof( Serializer ) );
    }


    [Hix.Serializer( typeof( LightSettingData ) )]
    class Serializer : Hix.AbstractSerializer<LightSettingData>
    {
      public override void Write( XmlWriter writer, Hisui.Hix.INameResolver names )
      {
        writer.WriteHixData( "Enabled", Target.Enabled );
        writer.WriteHixData( "IsWorldCoordinate", Target.IsWorldCoordinate );
        writer.WriteHixData( "AmbientColor", Target.AmbientColor );
        writer.WriteHixData( "DiffuseColor", Target.DiffuseColor );
        writer.WriteHixData( "SpecularColor", Target.SpecularColor );
        writer.WriteHixData( "Position", Target.Position );
      }

      public override void Read( XmlReader reader, Hisui.Hix.IReferenceResolver refs )
      {
        Target = new LightSettingData
        {
          Enabled = reader.ReadHixData<bool>( "Enabled" ),
          IsWorldCoordinate = reader.ReadHixData<bool>( "IsWorldCoordinate" ),
          AmbientColor = reader.ReadHixData<Color>( "AmbientColor" ),
          DiffuseColor = reader.ReadHixData<Color>( "DiffuseColor" ),
          SpecularColor = reader.ReadHixData<Color>( "SpecularColor" ),
          Position = reader.ReadHixData<Geom.HmCod3d>( "Position" )
        };
      }
    }
  }
}
