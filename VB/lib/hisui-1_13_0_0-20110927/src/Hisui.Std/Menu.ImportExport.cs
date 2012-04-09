using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hisui.Std
{
  partial class Menu
  {
    [Ctrl.Command]
    static IEnumerable<Ctrl.CommandItem> ImportCommands
    {
      get
      {
        yield return new Ctrl.CommandItem( "Import", "インポート" );
        yield return new Ctrl.CommandItem( "File.Import", "インポート" );

        foreach ( var item in FormatImportCommand.CreateCommandItems() ) yield return item;
        foreach ( var importer in Hix.FormatRegistry.Importers ) {
          foreach ( var item in FormatImportCommand.CreateCommandItems( importer ) )
            yield return item;
        }

        foreach ( var importer in Hix.ImporterRegistry.Importers ) {
          yield return ImportCommand.CreateCommandItem( importer, typeof( Core.IEntry ) );
          yield return ImportCommand.CreateCommandItem( importer, typeof( Core.Document ) );
          yield return ImportCommand.CreateCommandItem( importer, typeof( void ) );
        }
      }
    }

    [Ctrl.Command]
    static IEnumerable<Ctrl.CommandItem> ExportCommands
    {
      get
      {
        yield return new Ctrl.CommandItem( "Export", "エクスポート" );
        foreach ( var exporter in Hix.FormatRegistry.Exporters ) {
          if ( exporter.FormatType == typeof( Hix.HixFormat ) ) continue;
          var item = FormatExportCommand.CreateCommandItem( exporter );
          if ( item != null ) yield return item;
        }
      }
    }


    [Ctrl.Command( "ファイルに保存" )]
    class ExportHix : FormatExportCommand
    {
      public ExportHix()
        : base( Hix.FormatRegistry.Exporters.First(
        exporter => exporter.FormatType == typeof( Hix.HixFormat ) ) )
      { }
    }
  }
}
