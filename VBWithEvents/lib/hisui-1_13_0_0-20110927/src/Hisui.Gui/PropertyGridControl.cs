using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hisui.Gui
{
  class PropertyGridControl : PropertyControl
  {
    readonly PropertyGrid _grid = new PropertyGrid { Dock = DockStyle.Fill };

    public PropertyGridControl()
      : base( new Property() )
    {
      this.Controls.Add( _grid );
    }

    class Property : AbstractProperty<PropertyGridControl>
    {
      public override void Initialize()
      {
        _self._grid.PropertyValueChanged += ( sender, e ) => SI.Commit();
      }

      public override string Caption
      {
        get { return "エンティティ"; }
      }

      public override object TargetObject
      {
        get { return _self._grid.SelectedObject; }
        set { _self._grid.SelectedObject = value; }
      }
    }
  }
}
