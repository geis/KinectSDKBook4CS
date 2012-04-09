using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Hisui.Std
{
  class FormatExportCommand : Ctrl.Command
  {
    public static Ctrl.CommandItem
      CreateCommandItem( Hix.IFormatExporter exporter )
    {
      var format = Hix.FormatRegistry.FindFormat( exporter.FormatType );
      if ( format == null ) return null;

      string caption = string.Format( "{0} ({1})", format.Caption, format.FileFilter );
      return new Ctrl.CommandItem( 
        new FormatExportCommand( exporter ), "Export." + format.Name, caption );
    }

    readonly Hix.IFormatExporter _exporter;

    public FormatExportCommand( Hix.IFormatExporter exporter )
    {
      _exporter = exporter;
    }

    public override Type TargetType
    {
      get { return _exporter.EntityType; }
    }

    public override Hisui.Ctrl.CommandOption QueryOption( object target, Hisui.Ctrl.IContext con )
    {
      return new Ctrl.CommandOption { QueryRunnable = !Core.Progress.IsBusy };
    }

    protected override bool Execute( object target, Hisui.Ctrl.IContext con )
    {
      var format = Hix.FormatRegistry.FindFormat( _exporter.FormatType );
      var filter = string.Format( "{0} ({1})|{1}|All files (*.*)|*.*", format.Caption, format.FileFilter );
      var dialog = new SaveFileDialog { Filter = filter };
      if ( dialog.ShowDialog() == DialogResult.OK ) {
        using ( var prg = Core.Progress.Start( "exporting: " + dialog.FileName ) ) {
          prg.Step( 1.0 );
          using ( var stream = File.Create( dialog.FileName ) ) {
            _exporter.Export( target, stream );
          }
        }
      }
      return true;
    }
  }
}
