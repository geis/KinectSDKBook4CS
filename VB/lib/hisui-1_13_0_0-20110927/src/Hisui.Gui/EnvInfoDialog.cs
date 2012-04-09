using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Hisui.OpenGL;

namespace Hisui.Gui
{
  public partial class EnvInfoDialog : Form
  {
    public EnvInfoDialog()
    {
      InitializeComponent();
      textBox1.Text = GetEnvironmentString();
    }

    static string GetEnvironmentString()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendLine( "[OS]" );
      sb.AppendFormat( " - {0}", Environment.OSVersion.ToString() );
      sb.AppendLine(); sb.AppendLine();
      sb.AppendLine( "[OpenGL Version]" );
      sb.AppendFormat( " - {0}", HiGL.VersionString );
      sb.AppendLine(); sb.AppendLine();
      sb.AppendLine( "[OpenGL Vendor]" );
      sb.AppendFormat( " - {0}", HiGL.VendorString );
      sb.AppendLine(); sb.AppendLine();
      sb.AppendLine( "[OpenGL Renderer]" );
      sb.AppendFormat( " - {0}", HiGL.RendererString );
      sb.AppendLine(); sb.AppendLine();
      sb.AppendLine( "[OpenGL Extensions]" );
      foreach ( var ext in HiGL.ExtensionStrings ) {
        sb.AppendFormat( " - {0}", ext );
        sb.AppendLine();
      }
      return sb.ToString();
    }
  }
}
