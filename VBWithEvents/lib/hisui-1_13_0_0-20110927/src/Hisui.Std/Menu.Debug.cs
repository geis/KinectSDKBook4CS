using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Hisui.Std
{
  partial class Menu
  {
    [Ctrl.Command( "デバッグ", Order = 500 )]
    static class Debug
    {
      [Ctrl.Command( "エラーチェック" )]
      static void CheckInvariants( Ctrl.IContext con )
      {
        Testing.ITestReport report = new Testing.ConsoleReport();
        Testing.TestRegistry.RunChecks( con.Document, report );
        Testing.TestRegistry.RunChecks( con.Document.Entries, report );
        foreach ( Core.IEntry entry in con.Document.Entries.GetEntriesRecursive() ) {
          Testing.TestRegistry.RunChecks( entry, report );
          Testing.TestRegistry.RunChecks( entry.Entries, report );
          Testing.TestRegistry.RunChecks( entry.Entity, report );
          foreach ( object element in entry.Elements ) {
            Testing.TestRegistry.RunChecks( element, report );
          }
        }
      }

      [Ctrl.Command( "エラーチェック" )]
      static void CheckInvariants( Core.IEntry entry )
      {
        Testing.ITestReport report = new Testing.ConsoleReport();
        Testing.TestRegistry.RunChecks( entry, report );
        Testing.TestRegistry.RunChecks( entry.Entries, report );
        Testing.TestRegistry.RunChecks( entry.Entity, report );
        foreach ( object element in entry.Elements ) {
          Testing.TestRegistry.RunChecks( element, report );
        }
      }

      [Ctrl.Command( "単体テスト実行" )]
      static void RunTests()
      {
        Testing.TestRegistry.RunTests();
      }

      [Ctrl.Command( "エラーリスト" )]
      class ShowErrorList : Ctrl.ICommand
      {
        #region ICommand メンバ

        public Type TargetType
        {
          get { return typeof( void ); }
        }

        public Ctrl.CommandOption QueryOption( object target, Ctrl.IContext con )
        {
          bool runnable = Core.Progress.IsBusy ? false : CoreUT.UnhandledErrorReport != null;
          return new Ctrl.CommandOption { QueryRunnable = runnable };
        }

        public void Run( object target, Hisui.Ctrl.IContext context )
        {
          new ErrorListDialog().ShowDialog();
        }

        #endregion
      }

      [Ctrl.Command( "プラグイン一覧" )]
      static void ShowPlugins()
      {
        var sb = new StringBuilder();
        foreach ( var asm in Core.PluginLoader.Plugins ) {
          sb.AppendLine( asm.FullName );
        }
        MessageBox.Show( sb.ToString(), "プラグイン一覧" );
      }

      [Ctrl.Command( "ライセンス一覧" )]
      static void ShowLicenses()
      {
        var sb = new StringBuilder();
        foreach ( var name in Core.LicenseManager.Components ) {
          sb.AppendLine( name );
        }
        MessageBox.Show( sb.ToString(), "ライセンス一覧" );
      }

      [Ctrl.Command( "アセンブリ厳密名を表示 ..." )]
      static void ShowStrictName()
      {
        var dialog = new OpenFileDialog();
        dialog.Filter = "アセンブリファイル (*.dll;*.exe)|*.dll;*.exe";
        if ( dialog.ShowDialog() == DialogResult.OK ) {
          try {
            var asm = System.Reflection.Assembly.LoadFrom( dialog.FileName );
            if ( asm != null ) MessageBox.Show( asm.FullName, "The assembly's strong name" );
          }
          catch {
            MessageBox.Show( 
              "Not .NET assembly", "error", MessageBoxButtons.OK, MessageBoxIcon.Error );
          }
        }
      }

      [Ctrl.Command( "全コマンドをXML出力" )]
      static void DumpCommands()
      {
        var dialog = new SaveFileDialog { Filter = "XMLファイル(*.xml)|*.xml" };
        if ( dialog.ShowDialog() == DialogResult.OK ) {
          SI.WriteCommandCaptionsXML( dialog.FileName );
        }
      }

      [Ctrl.Command( "ガベージコレクション実行" )]
      static void RunGC()
      {
        GC.Collect();
      }

      [Ctrl.Command( "描画速度 FPS 表示" )]
      static void ShowFPS()
      {
        SI.View.Refresh();
        MessageBox.Show( string.Format(
          "{0} fps", 1000.0 / SI.View.MillisecondsPerFrame ) );
      }

      [Ctrl.Command( "全エンティティをHOX形式エクスポート" )]
      static void ExportAllEntities()
      {
        var dialog = new FolderBrowserDialog();
        if ( dialog.ShowDialog() == DialogResult.OK ) {
          foreach ( var e in SI.Document.Entries.GetEntriesRecursive() ) {
            if ( e.Entity != null ) {
              var filename = string.Format( "{0}.{1}.hox", e.PathString, e.Entity.GetType() );
              e.ExportHox( Path.Combine( dialog.SelectedPath, filename ) );
            }
          }
        }
      }

      [Ctrl.Command( "境界ボックスを表示" )]
      static void PutBoundingBox( Core.IEntry entry )
      {
        entry.Entity.As<Geom.IBoundary3d>( boundary => entry.Entries.Put( boundary.BoundingBox ) );
      }
    }
  }
}
