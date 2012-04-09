using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hisui.Gui
{
  public partial class MaterialDialog : Form
  {
    public MaterialDialog()
    {
      InitializeComponent();
    }

    public Graphics.Material FrontMaterial
    {
      get { return panelFrontMaterial.Material; }
      set { panelFrontMaterial.Material = value; }
    }

    public Graphics.Material BackMaterial
    {
      get { return panelBackMaterial.Material; }
      set { panelBackMaterial.Material = value; }
    }

    public bool UseBackMaterial
    {
      get { return checkBackMaterial.Checked; }
      set { checkBackMaterial.Checked = value; }
    }

    public Graphics.MaterialDecoration MaterialDecoration
    {
      get
      {
        return new Graphics.MaterialDecoration
        {
          Material = this.FrontMaterial,
          BackMaterial = this.BackMaterial,
          UseBackMaterial = this.UseBackMaterial
        };
      }
      set
      {
        if ( value != null ) {
          this.FrontMaterial = value.Material;
          this.BackMaterial = value.BackMaterial;
          this.UseBackMaterial = value.UseBackMaterial;
        }
        else {
          this.FrontMaterial = Graphics.Material.Default;
          this.BackMaterial = Graphics.Material.Default;
          this.UseBackMaterial = false;
        }
      }
    }

    private void checkBackMaterial_CheckedChanged( object sender, EventArgs e )
    {
      panelBackMaterial.Enabled = checkBackMaterial.Checked;
    }
  }
}
