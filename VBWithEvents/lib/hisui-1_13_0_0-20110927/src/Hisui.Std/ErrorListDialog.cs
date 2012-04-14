using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Hisui.Std
{
  public partial class ErrorListDialog : Form
  {
    class ErrorItem
    {
      public readonly Core.Error Error;
      public ErrorItem( Core.Error e ) { this.Error = e; }
      public override string ToString() { return this.Error.InnerException.Message; }
    }

    public ErrorListDialog()
    {
      InitializeComponent();

      if ( CoreUT.UnhandledErrorReport != null ) {
        foreach ( var err in CoreUT.UnhandledErrorReport.Errors ) {
          listExceptions.Items.Add( new ErrorItem( err ), true );
        }
        listExceptions.SelectedIndex = 0;
      }
    }

    private void listExceptions_SelectedIndexChanged( object sender, EventArgs e )
    {
      ErrorItem item = (ErrorItem)listExceptions.SelectedItem;
      labelReporter.Text = item.Error.Reporter.ToString();
      labelExceptionType.Text = item.Error.InnerException.GetType().FullName;
      labelMessage.Text = item.Error.InnerException.Message;
      gridExceptionProperty.SelectedObject = item.Error.InnerException;
    }

    private void btnSelectAll_Click( object sender, EventArgs e )
    {
      for ( int i = 0 ; i < listExceptions.Items.Count ; ++i ) {
        listExceptions.SetItemChecked( i, true );
      }
    }

    private void btnClearSelection_Click( object sender, EventArgs e )
    {
      for ( int i = 0 ; i < listExceptions.Items.Count ; ++i ) {
        listExceptions.SetItemChecked( i, false );
      }
    }

    private void btnPut_Click( object sender, EventArgs e )
    {
      for ( int i = 0 ; i < listExceptions.Items.Count ; ++i ) {
        if ( listExceptions.GetItemChecked( i ) ) {
          ErrorItem item = (ErrorItem)listExceptions.Items[i];
          Ctrl.Current.Document.ActiveEntries.Put( item.Error.Reporter );
        }
      }
      Ctrl.Current.Document.History.Commit();
    }
  }
}