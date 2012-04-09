using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hisui.Gui
{
  class PropertyRegistry
  {
    readonly Dictionary<Type, PropertyControl> _controls = new Dictionary<Type, PropertyControl>();


    internal IEnumerable<PropertyControl> GetControls( Core.IEntry entry )
    {
      if ( entry == null ) yield break;
      if ( entry.Entity != null ) {
        var control = this.GetControl( entry.Entity );
        yield return control != null ? control : new PropertyGridControl { TargetObject = entry.Entity };
      }
      foreach ( var pair in entry.Elements ) yield return this.GetControl( pair.Value );
      yield return this.GetControl( entry );
    }

    PropertyControl GetControl( object target )
    {
      if ( target == null ) return null;
      var control = this.GetControl( target.GetType() );
      if ( control != null ) { control.TargetObject = target; control.UseWaitCursor = false; }
      return control;
    }

    PropertyControl GetControl( Type targetType )
    {
      foreach ( Type key in Hisui.CoreUT.SortInheritanceDAG( targetType ) ) {
        if ( _controls.ContainsKey( key ) ) return _controls[key];
        var control = PropertyControlFactory.NewInstance( key );
        if ( control != null ) {
          _controls.Add( key, control );
          return control;
        }
      }
      return null;
    }
  }
}
