using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hisui.Gui
{
  public partial class MaterialPanel : UserControl
  {
    public MaterialPanel()
    {
      InitializeComponent();
    }

    public Color Color
    {
      get { return btnColor.BackColor; }
      set { btnColor.BackColor = value; }
    }

    public Color SpecularColor
    {
      get { return btnSpecularColor.BackColor; }
      set { btnSpecularColor.BackColor = value; }
    }

    public double Shininess
    {
      get
      {
        double max = Graphics.Material.MaxShininess;
        return max * GetClampValue( trackShininess );
      }
      set
      {
        double max = Graphics.Material.MaxShininess;
        SetClampValue( trackShininess, (value / max).Clamp() );
      }
    }

    public double Opacity
    {
      get { return GetClampValue( trackOpacity ); }
      set { SetClampValue( trackOpacity, value.Clamp() ); }
    }

    public Graphics.Material Material
    {
      get
      {
        return new Graphics.Material(
          this.Color, this.SpecularColor, this.Shininess, this.Opacity );
      }
      set
      {
        this.Color = value.Color;
        this.SpecularColor = value.SpecularColor;
        this.Shininess = value.Shininess;
        this.Opacity = value.Opacity;
      }
    }

    static Geom.Clamp GetClampValue( TrackBar track )
    {
      return ((double)track.Value / (track.Maximum - track.Minimum)).Clamp();
    }

    static void SetClampValue( TrackBar track, Geom.Clamp value )
    {
      track.Value = (int)(value * (track.Maximum - track.Minimum));
    }

    private void btnColor_Click( object sender, EventArgs e )
    {
      var dialog = new ColorDialog { Color = this.Color };
      if ( dialog.ShowDialog() == DialogResult.OK ) this.Color = dialog.Color;
    }

    private void btnSpecularColor_Click( object sender, EventArgs e )
    {
      var dialog = new ColorDialog { Color = this.SpecularColor };
      if ( dialog.ShowDialog() == DialogResult.OK ) this.SpecularColor = dialog.Color;
    }
  }
}
