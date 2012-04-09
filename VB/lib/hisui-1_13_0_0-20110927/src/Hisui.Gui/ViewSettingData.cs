using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;
using Hisui.OpenGL;

namespace Hisui.Gui
{
  class ViewSettingData
  {
    static readonly SI.SettingHolder<ViewSettingData> _holder = SI.CreateSettingHolder( new ViewSettingData(), SI.UserAppData );

    public static ViewSettingData Instance
    {
      get { return _holder.Get(); }
    }

    public static SI.ApplicationData DataSource
    {
      get { return _holder.DataSource; }
      set { _holder.DataSource = value; }
    }

    static ViewSettingData()
    {
      Hix.SerializerFactory.Register( typeof( Serializer_0 ) );
      Hix.SerializerFactory.Register( typeof( Serializer_1 ) );
      Hix.SerializerFactory.Register( typeof( Serializer_2 ) );
      Hix.SerializerFactory.Register( typeof( Serializer_3 ) );
      Hix.SerializerFactory.Register( typeof( Serializer_4 ) );
      Hix.SerializerFactory.Register( typeof( Serializer_5 ) );
      Hix.SerializerFactory.Register( typeof( Serializer_6 ) );
    }

    ViewSettingData()
    {
      for ( int i = 0 ; i < 4 ; ++i ) {
        this.Lights[i] = new LightSettingData( Graphics.Light.Lights[i] );
      }
      this.LowerZoomScale = new Graphics.StandardViewOperation.ZoomScale { Value = 1, IsPercent = true };
      this.UpperZoomScale = new Graphics.StandardViewOperation.ZoomScale { Value = 1000, IsPercent = true };
    }


    public Graphics.Material Material = new Graphics.Material( Color.LightBlue, Color.Gray );
    public Color BackgroundColor = Color.FromArgb( 6, 85, 115 );
    public bool UseVertexBuffer = HiGL.UseVertexBuffer;
    public bool UseDrawElements = HiGL.UseDrawElements;
    public bool EnableCameraAnimation = GraphicsUT.EnableCameraAnimation;
    public float PolygonOffsetFactor = GraphicsUT.PolygonOffsetFactor;
    public float PolygonOffsetUnits = GraphicsUT.PolygonOffsetUnits;
    public int SelectionBufferSize = GraphicsUT.SelectionBufferSize;
    public int DragZoomSign = 1;
    public int WheelZoomSign = 1;
    public Graphics.StandardViewOperation.ZoomScale LowerZoomScale;
    public Graphics.StandardViewOperation.ZoomScale UpperZoomScale;
    public bool ShowAxis = true;
    public bool ShowCompass = true;
    public bool ShowRuler = true;
    public string ShaderName = "";
    public bool AutoFocus = true;
    public bool ZoomToMousePosition = true;
    public bool ZKeptRotation = false;
    public readonly LightSettingData[] Lights = new LightSettingData[4];

    public Graphics.ViewOperationBindingCollection 
      Bindings = Graphics.ViewOperationBindingCollection.Default;

    public void CopyTo( ViewSetting vs )
    {
      vs.Material = this.Material;
      vs.BackgroundColor = this.BackgroundColor;
      vs.UseVertexBuffer = this.UseVertexBuffer;
      vs.UseDrawElements = this.UseDrawElements;
      vs.EnableCameraAnimation = this.EnableCameraAnimation;
      vs.PolygonOffsetFactor = this.PolygonOffsetFactor;
      vs.PolygonOffsetUnits = this.PolygonOffsetUnits;
      vs.SelectionBufferSize = this.SelectionBufferSize;
      vs.DragZoomSign = this.DragZoomSign;
      vs.WheelZoomSign = this.WheelZoomSign;
      vs.LowerZoomScale = this.LowerZoomScale;
      vs.UpperZoomScale = this.UpperZoomScale;
      vs.ShowAxis = this.ShowAxis;
      vs.ShowCompass = this.ShowCompass;
      vs.ShowRuler = this.ShowRuler;
      vs.ShaderName = this.ShaderName;
      vs.AutoFocus = this.AutoFocus;
      vs.ZoomToMousePosition = this.ZoomToMousePosition;
      vs.ZKeptRotation = this.ZKeptRotation;
      this.Bindings.CopyTo( vs.Bindings );
      for ( int i = 0 ; i < 4 ; ++i ) this.Lights[i].CopyTo( vs.Lights[i] );
    }

    public void CopyFrom( ViewSetting vs )
    {
      this.Material = vs.Material;
      this.BackgroundColor = vs.BackgroundColor;
      this.UseVertexBuffer = vs.UseVertexBuffer;
      this.UseDrawElements = vs.UseDrawElements;
      this.EnableCameraAnimation = vs.EnableCameraAnimation;
      this.PolygonOffsetFactor = vs.PolygonOffsetFactor;
      this.PolygonOffsetUnits = vs.PolygonOffsetUnits;
      this.SelectionBufferSize = vs.SelectionBufferSize;
      this.DragZoomSign = vs.DragZoomSign;
      this.WheelZoomSign = vs.WheelZoomSign;
      this.LowerZoomScale = vs.LowerZoomScale;
      this.UpperZoomScale = vs.UpperZoomScale;
      this.ShowAxis = vs.ShowAxis;
      this.ShowCompass = vs.ShowCompass;
      this.ShowRuler = vs.ShowRuler;
      this.ShaderName = vs.ShaderName;
      this.AutoFocus = vs.AutoFocus;
      this.ZoomToMousePosition = vs.ZoomToMousePosition;
      this.ZKeptRotation = vs.ZKeptRotation;
      vs.Bindings.CopyTo( this.Bindings );
      for ( int i = 0 ; i < 4 ; ++i ) this.Lights[i].CopyFrom( vs.Lights[i] );
    }


    [Hix.Serializer( typeof( ViewSettingData ), Version = 6 )]
    class Serializer_6 : Serializer_5
    {
      public override void Write( XmlWriter writer, Hisui.Hix.INameResolver names )
      {
        base.Write( writer, names );
        writer.WriteHixData( "AutoFocus", Target.AutoFocus );
        writer.WriteHixData( "ZoomToMousePosition", Target.ZoomToMousePosition );
        writer.WriteHixData( "ZKeptRotation", Target.ZKeptRotation );
      }

      public override void Read( XmlReader reader, Hisui.Hix.IReferenceResolver refs )
      {
        base.Read( reader, refs );
        Target.AutoFocus = reader.ReadHixData<bool>( "AutoFocus" );
        Target.ZoomToMousePosition = reader.ReadHixData<bool>( "ZoomToMousePosition" );
        Target.ZKeptRotation = reader.ReadHixData<bool>( "ZKeptRotation" );
      }
    }

    [Hix.Serializer( typeof( ViewSettingData ), Version = 5 )]
    class Serializer_5 : Serializer_4
    {
      public override void Write( XmlWriter writer, Hisui.Hix.INameResolver names )
      {
        base.Write( writer, names );
        writer.WriteHixData( "LowerZoomScale.Value", Target.LowerZoomScale.Value );
        writer.WriteHixData( "LowerZoomScale.IsPercent", Target.LowerZoomScale.IsPercent );
        writer.WriteHixData( "UpperZoomScale.Value", Target.UpperZoomScale.Value );
        writer.WriteHixData( "UpperZoomScale.IsPercent", Target.UpperZoomScale.IsPercent );
      }

      public override void Read( XmlReader reader, Hisui.Hix.IReferenceResolver refs )
      {
        base.Read( reader, refs );
        Target.LowerZoomScale = new Hisui.Graphics.StandardViewOperation.ZoomScale
        {
          Value = reader.ReadHixData<double>( "LowerZoomScale.Value" ),
          IsPercent = reader.ReadHixData<bool>( "LowerZoomScale.IsPercent" )
        };
        Target.UpperZoomScale = new Hisui.Graphics.StandardViewOperation.ZoomScale
        {
          Value = reader.ReadHixData<double>( "UpperZoomScale.Value" ),
          IsPercent = reader.ReadHixData<bool>( "UpperZoomScale.IsPercent" )
        };
      }
    }


    [Hix.Serializer( typeof( ViewSettingData ), Version = 4 )]
    class Serializer_4 : Serializer_3
    {
      public override void Write( XmlWriter writer, Hisui.Hix.INameResolver names )
      {
        base.Write( writer, names );
        writer.WriteHixData( "ShaderName", Target.ShaderName );
      }

      public override void Read( XmlReader reader, Hisui.Hix.IReferenceResolver refs )
      {
        base.Read( reader, refs );
        Target.ShaderName = reader.ReadHixData<string>( "ShaderName" );
      }
    }


    [Hix.Serializer( typeof( ViewSettingData ), Version = 3 )]
    class Serializer_3 : Serializer_2
    {
      public override IEnumerable<object> References
      {
        get { return base.References.Concat( Target.Lights ); }
      }

      public override void Write( XmlWriter writer, Hisui.Hix.INameResolver names )
      {
        base.Write( writer, names );
        writer.WriteHixArrayRef( "Lights", names, Target.Lights ); 
      }

      public override void Read( XmlReader reader, Hisui.Hix.IReferenceResolver refs )
      {
        base.Read( reader, refs );
        var lights = reader.ReadHixArrayRef<LightSettingData>( "Lights", refs );
        for ( int i = 0 ; i < 4 ; ++i ) Target.Lights[i] = lights[i];
      }
    }


    [Hix.Serializer( typeof( ViewSettingData ), Version = 2 )]
    class Serializer_2 : Serializer_1
    {
      public override void Write( XmlWriter writer, Hisui.Hix.INameResolver names )
      {
        base.Write( writer, names );
        writer.WriteHixData( "SelectionBufferSize", Target.SelectionBufferSize );
      }

      public override void Read( XmlReader reader, Hisui.Hix.IReferenceResolver refs )
      {
        base.Read( reader, refs );
        Target.SelectionBufferSize = reader.ReadHixData<int>( "SelectionBufferSize" );
      }
    }


    [Hix.Serializer( typeof( ViewSettingData ), Version = 1 )]
    class Serializer_1 : Serializer_0
    {
      public override void Write( XmlWriter writer, Hisui.Hix.INameResolver names )
      {
        base.Write( writer, names );
        writer.WriteHixData( "ShowAxis", Target.ShowAxis );
        writer.WriteHixData( "ShowCompass", Target.ShowCompass );
        writer.WriteHixData( "ShowRuler", Target.ShowRuler );
      }

      public override void Read( XmlReader reader, Hisui.Hix.IReferenceResolver refs )
      {
        base.Read( reader, refs );
        Target.ShowAxis = reader.ReadHixData<bool>( "ShowAxis" );
        Target.ShowCompass = reader.ReadHixData<bool>( "ShowCompass" );
        Target.ShowRuler = reader.ReadHixData<bool>( "ShowRuler" );
      }
    }


    [Hix.Serializer( typeof( ViewSettingData ), Version = 0 )]
    class Serializer_0 : Hix.AbstractSerializer<ViewSettingData>
    {
      public override IEnumerable<object> References
      {
        get { yield return Target.Bindings; }
      }

      public override void Write( XmlWriter writer, Hisui.Hix.INameResolver names )
      {
        writer.WriteHixData( "Material", Target.Material );
        writer.WriteHixData( "BackgroundColor", Target.BackgroundColor );
        writer.WriteHixData( "UseVertexBuffer", Target.UseVertexBuffer );
        writer.WriteHixData( "EnableCameraAnimation", Target.EnableCameraAnimation );
        writer.WriteHixData( "PolygonOffsetFactor", Target.PolygonOffsetFactor );
        writer.WriteHixData( "PolygonOffsetUnits", Target.PolygonOffsetUnits );
        writer.WriteHixData( "DragZoomSign", Target.DragZoomSign );
        writer.WriteHixData( "WheelZoomSign", Target.WheelZoomSign );
        writer.WriteHixRef( "Bindings", names, Target.Bindings );
      }

      public override void Read( XmlReader reader, Hisui.Hix.IReferenceResolver refs )
      {
        this.Target = new ViewSettingData();
        Target.Material = reader.ReadHixData<Graphics.Material>( "Material" );
        Target.BackgroundColor = reader.ReadHixData<Color>( "BackgroundColor" );
        Target.UseVertexBuffer = reader.ReadHixData<bool>( "UseVertexBuffer" );
        Target.EnableCameraAnimation = reader.ReadHixData<bool>( "EnableCameraAnimation" );
        Target.PolygonOffsetFactor = reader.ReadHixData<float>( "PolygonOffsetFactor" );
        Target.PolygonOffsetUnits = reader.ReadHixData<float>( "PolygonOffsetUnits" );
        Target.DragZoomSign = reader.ReadHixData<int>( "DragZoomSign" );
        Target.WheelZoomSign = reader.ReadHixData<int>( "WheelZoomSign" );
        Target.Bindings = reader.ReadHixRef<Graphics.ViewOperationBindingCollection>( "Bindings", refs );
      }
    }
  }
}
