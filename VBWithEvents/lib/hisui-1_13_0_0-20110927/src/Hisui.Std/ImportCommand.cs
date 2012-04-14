using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Hisui.Std
{
  class ImportCommand : Ctrl.Command
  {
    public static Ctrl.CommandItem CreateCommandItem( Hix.IImporter importer, Type target )
    {
      var command = new ImportCommand( importer, target );
      return new Ctrl.CommandItem( command, command.Name, importer.MakeCaptionString() );
    }

    readonly Hix.IImporter _importer;
    readonly Type _target;

    public ImportCommand( Hix.IImporter importer, Type target )
    {
      _importer = importer;
      _target = target;
    }

    public string Name
    {
      get
      {
        string name = _importer.GetType().Name;
        if ( _target == typeof( Core.IEntry ) ) return "Import." + name;
        if ( _target == typeof( Core.Document ) ) return "Import." + name;
        return "File.Import." + name;
      }
    }

    override public Type TargetType
    {
      get { return _target; }
    }

    override public Ctrl.CommandOption QueryOption( object target, Ctrl.IContext con )
    {
      return new Ctrl.CommandOption { QueryRunnable = !Core.Progress.IsBusy };
    }

    protected override bool Execute( object target, Hisui.Ctrl.IContext con )
    {
      var filter = string.Format( "{0}|{1}|All files (*.*)|*.*",
        _importer.MakeCaptionString(), _importer.MakeFileFilterString() );
      var dialog = new OpenFileDialog { Filter = filter };
      if ( dialog.ShowDialog() == DialogResult.OK ) {
        using ( var prg = Core.Progress.Start( "importing: " + dialog.FileName ) ) {
          prg.Step( 1.0 );
          var dst = this.GetDestination( con );
          _importer.Import( dst.Put( null ), dialog.FileName );
          con.View.Fit();
        }
      }
      return true;
    }

    Core.IEntryCollection GetDestination( Ctrl.IContext con )
    {
      if ( _target == typeof( Core.IEntry ) ) return con.Selected.Entries;
      if ( _target == typeof( Core.Document ) ) return con.Document.Entries;
      return con.ActiveEntries;
    }
  }
}
