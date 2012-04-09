using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Drawing;
using System.Windows.Forms;

namespace Hisui.Gui
{
  public static class PropertyControlFactory
  {
    static readonly Dictionary<Type, Func<PropertyControl>>
      _factories = new Dictionary<Type, Func<PropertyControl>>();

    public static bool Show( Control target )
    {
      return Show( target.Parent, target.Location, target.Tag );
    }

    public static bool Show( Control owner, Point location, object target )
    {
      var control = Gui.PropertyControlFactory.NewInstance( target );
      if ( control != null ) {
        control.Dock = DockStyle.Fill;
        var form = new Form
        {
          ControlBox = false,
          FormBorderStyle = FormBorderStyle.FixedSingle,
          StartPosition = FormStartPosition.Manual,
          Location = owner.PointToScreen( location ),
          ClientSize = control.Size
        };
        form.Controls.Add( control );
        form.Deactivate += ( ss, ee ) => form.Close();
        form.Show( owner );
        form.Focus();
        return true;
      }
      return false;
    }

    public static PropertyControl NewInstance( object target )
    {
      if ( target == null ) return null;
      var control = CoreUT.SortInheritanceDAG( target.GetType() )
        .Select( type => PropertyControlFactory.NewInstance( type ) )
        .WhereNotNull()
        .FirstOrDefault();
      if ( control != null ) control.TargetObject = target;
      return control;
    }

    public static PropertyControl NewInstance( Type key )
    {
      return _factories.ContainsKey( key ) ? _factories[key]() : null;
    }

    public static void RegisterToPluginLoader()
    {
      Core.PluginLoader.TypeLoadEvent += type =>
      {
        var atr = CoreUT.GetPluginAttribute<PropertyControlAttribute>( type );
        if ( atr != null ) Register( atr.SourceType, type );
      };
    }

    public static void Register( Type key, Func<PropertyControl> factory )
    {
      _factories[key] = factory;
    }

    public static void Register<TKey, TControl>() where TControl : PropertyControl
    {
      Register( typeof( TKey ), typeof( TControl ) );
    }

    static void Register( Type key, Type control )
    {
      Register( key, control.GetConstructor( Type.EmptyTypes ) );
    }

    static void Register( Type key, ConstructorInfo ctor )
    {
      Register( key, () => ctor.Invoke( null ) as PropertyControl );
    }

    static PropertyControlFactory()
    {
      Register<Core.IEntry, EntryPropertyControl>();
      Register<Core.IStorage, StoragePropertyControl>();
    }
  }
}
