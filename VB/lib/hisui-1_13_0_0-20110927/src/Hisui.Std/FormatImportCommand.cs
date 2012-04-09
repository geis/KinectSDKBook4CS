using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Hisui.Std
{
  class FormatImportCommand : Ctrl.Command
  {
    public static IEnumerable<Ctrl.CommandItem>
      CreateCommandItems( Hix.IFormatImporter importer )
    {
      var format = Hix.FormatRegistry.FindFormat( importer.FormatType );
      if ( format != null ) {
        string name = "Import." + format.Name;
        string caption = string.Format( "{0} ({1})", format.Caption, format.FileFilter );
        yield return CreateCommandItem( importer, typeof( void ), "File." + name, caption );
        yield return CreateCommandItem( importer, typeof( Core.Document ), name, caption );
        yield return CreateCommandItem( importer, typeof( Core.IEntry ), name, caption );
      }
    }

    public static IEnumerable<Ctrl.CommandItem> CreateCommandItems()
    {
      string name = "Import.AnyFormat";
      string caption = "Any formats";
      yield return CreateCommandItem( null, typeof( void ), "File." + name, caption );
      yield return CreateCommandItem( null, typeof( Core.Document ), name, caption );
      yield return CreateCommandItem( null, typeof( Core.IEntry ), name, caption );
    }

    static Ctrl.CommandItem CreateCommandItem( 
      Hix.IFormatImporter importer, Type target, string name, string caption )
    {
      return new Hisui.Ctrl.CommandItem(
        new FormatImportCommand( importer, target ), name, caption );
    }

    readonly Hix.IFormatImporter _importer;
    readonly Type _target;

    FormatImportCommand( Hix.IFormatImporter importer, Type target )
    {
      _importer = importer;
      _target = target;
    }

    public override Type TargetType
    {
      get { return _target; }
    }

    public override Hisui.Ctrl.CommandOption QueryOption( object target, Hisui.Ctrl.IContext con )
    {
      return new Ctrl.CommandOption { QueryRunnable = !Core.Progress.IsBusy };
    }

    protected override bool Execute( object target, Hisui.Ctrl.IContext con )
    {
      var filter = (_importer == null) ? GetFilterString() : GetFilterString( _importer );
      var dialog = new OpenFileDialog { Filter = filter, Multiselect = true };
      if ( dialog.ShowDialog() == DialogResult.OK ) {
        var imports = dialog.FileNames
          .Select( file => new { FileName = file, Importer = this.GetImporter( file ) } )
          .Where( item => item.Importer != null )
          .ToArray();
        using ( var progress = SI.StartProgress( "importing" ) ) {
          foreach ( var item in imports ) {
            progress.Step( 1.0 / imports.Length );
            this.Import( item.FileName, item.Importer, con );
            SI.Increment();
          }
        }
      }
      return true;
    }

    void Import( string filename, Hix.IFormatImporter importer, Ctrl.IContext con )
    {
      using ( var progress = SI.PushProgress( "importing: " + filename ) ) {
        var entry = this.GetDestination( con ).Put( null );
        using ( var stream = File.OpenRead( filename ) ) {
          progress.Step( 0.8 );
          importer.Import( stream, entry );
        }
        entry.Caption = Path.GetFileNameWithoutExtension( filename );
        progress.Step( 0.2 );
        con.View.Fit();
      }
    }

    Hix.IFormatImporter GetImporter( string filename )
    {
      return (_importer == null) ? Hix.FormatRegistry.FindImporterByFileExtension( filename ) : _importer;
    }

    Core.IEntryCollection GetDestination( Ctrl.IContext con )
    {
      if ( _target == typeof( Core.IEntry ) ) return con.Selected.Entries;
      if ( _target == typeof( Core.Document ) ) return con.Document.Entries;
      return con.ActiveEntries;
    }

    static string GetFilterString( Hix.IFormatImporter importer )
    {
      var format = Hix.FormatRegistry.FindFormat( importer.FormatType );
      return string.Format( "{0} ({1})|{1}|All files (*.*)|*.*", format.Caption, format.FileFilter );
    }

    static string GetFilterString()
    {
      return string.Format( "Any formats ({0})|{0}|All files (*.*)|*.*", Hix.FormatRegistry.Formats.GetFileFilter() );
    }
  }
}
