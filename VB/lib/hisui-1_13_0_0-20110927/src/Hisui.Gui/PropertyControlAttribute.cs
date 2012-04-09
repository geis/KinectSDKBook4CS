using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hisui.Gui
{
  public class PropertyControlAttribute : Core.PluginAttribute
  {
    public readonly Type SourceType;
    public PropertyControlAttribute( Type type ) { this.SourceType = type; }
  }
}
