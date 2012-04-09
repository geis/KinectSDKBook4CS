using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hisui.Std
{
  partial class Menu
  {
    [Ctrl.Command( "編集", Order = 200 )]
    static class Edit
    {
      [Ctrl.Command( "元に戻す", Order = 10, ShortcutKeys = Keys.Control | Keys.Z )]
      class UndoCommand : Ctrl.ICommand
      {
        public System.Type TargetType
        {
          get { return typeof( void ); }
        }

        public Ctrl.CommandOption QueryOption( object target, Ctrl.IContext con )
        {
          bool runnable = Core.Progress.IsBusy ? false : con.Document.History.CanUndo;
          return new Ctrl.CommandOption { QueryRunnable = runnable };
        }

        public void Run( object target, Ctrl.IContext context )
        {
          context.Document.History.Undo();
          context.View.Invalidate();
        }
      }

      [Ctrl.Command( "やり直し", Order = 20, ShortcutKeys = Keys.Control | Keys.Y )]
      class RedoCommand : Ctrl.ICommand
      {
        public System.Type TargetType
        {
          get { return typeof( void ); }
        }

        public Ctrl.CommandOption QueryOption( object target, Ctrl.IContext con )
        {
          bool runnable = Core.Progress.IsBusy ? false : con.Document.History.CanRedo;
          return new Ctrl.CommandOption { QueryRunnable = runnable };
        }

        public void Run( object target, Ctrl.IContext context )
        {
          context.Document.History.Redo();
          context.View.Invalidate();
        }
      }


      [Ctrl.Command( "再構築", Order = 30, ShortcutKeys = Keys.F5 )]
      class RebuildCommand : Ctrl.ICommand
      {
        public System.Type TargetType
        {
          get { return typeof( void ); }
        }

        public Ctrl.CommandOption QueryOption( object target, Ctrl.IContext con )
        {
          return new Ctrl.CommandOption { QueryRunnable = !Core.Progress.IsBusy };
        }

        public void Run( object target, Ctrl.IContext context )
        {
          Core.Builder.RunRebuild();
        }
      }


      [Ctrl.Command( "中断", Order = 40 )]
      class AbortCommand : Ctrl.ICommand
      {
        public System.Type TargetType
        {
          get { return typeof( void ); }
        }

        public Ctrl.CommandOption QueryOption( object target, Ctrl.IContext con )
        {
          bool runnable = Core.Progress.IsBusy || Ctrl.Current.Driver.IsActive;
          return new Ctrl.CommandOption { QueryRunnable = runnable };
        }

        public void Run( object target, Ctrl.IContext context )
        {
          if ( Core.Progress.IsBusy ) {
            Core.Progress.PostAbort();
          }
          else if ( Ctrl.Current.Driver.IsActive ) {
            Ctrl.Current.Driver.Abort();
            if ( Ctrl.Current.Document.History.CanCancel )
              context.Document.History.Cancel();
            else
              Core.Builder.Run();
            Core.Document.Current.SelectedEntry = null;
            context.View.Invalidate();
          }
        }
      }

      [Ctrl.Command( "-", Order = 45 )]
      static void _separator1() { }

      [Ctrl.Command( "全て選択", Order = 50, ShortcutKeys = Keys.Control | Keys.A )]
      static void SelectAll()
      {
        Core.Document.Current.SelectedEntries.Clear();
        foreach ( Core.IEntry entry in Core.Document.Current.ActiveEntries ) {
          Core.Document.Current.SelectedEntries.Add( entry );
        }
      }

      [Ctrl.Command( "選択反転", Order = 60 )]
      static void ReverseSelection()
      {
        if ( Core.Document.Current.SelectedEntries.Count != 0 ) {
          List<Core.IEntry> selection = new List<Hisui.Core.IEntry>();
          foreach ( Core.IEntry entry in Core.Document.Current.ActiveEntries ) {
            if ( !entry.IsSelected ) selection.Add( entry );
          }
          Core.Document.Current.SelectedEntries.Clear();
          foreach ( Core.IEntry entry in selection ) {
            Core.Document.Current.SelectedEntries.Add( entry );
          }
        }
      }

      [Ctrl.Command( "-", Order = 65 )]
      static void _separator2() { }

      [Ctrl.Command( "削除", Order = 70, ShortcutKeys = Keys.Delete )]
      static void Delete( Ctrl.IContext con, Ctrl.CommandOption opt )
      {
        if ( opt.QueryRunnable ) {
          opt.QueryRunnable = (SI.SelectedEntries.Count != 0);
        }
        else {
          Menu.Delete( SI.SelectedEntries, con );
        }
      }
    }
  }
}
