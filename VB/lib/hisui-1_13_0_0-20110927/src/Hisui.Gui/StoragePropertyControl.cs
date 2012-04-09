using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hisui.Gui
{
  public partial class StoragePropertyControl : PropertyControl
  {
    public StoragePropertyControl()
      : base( new Property() )
    {
      InitializeComponent();
    }


    class Property : AbstractProperty<StoragePropertyControl>
    {
      Core.IStorage _storage;
      string _lookupKey;

      void LookupItem()
      {
        if ( _self.txtID.Text == _lookupKey ) return;
        _lookupKey = _self.txtID.Text;
        _self.panelPropertyControl.Controls.Clear();
        try {
          var obj = _storage[int.Parse( _self.txtID.Text )];
          _self.txtValue.Text = obj.ToString();
          foreach ( var key in CoreUT.SortInheritanceDAG( obj.GetType() ) ) {
            var control = PropertyControlFactory.NewInstance( key );
            if ( control != null ) {
              control.TargetObject = obj;
              control.Dock = DockStyle.Fill;
              _self.panelPropertyControl.Controls.Add( control );
            }
          }
        }
        catch ( Exception ) {
          _self.txtID.Focus();
          _self.txtID.SelectAll();
          _self.txtValue.Text = "(none)";
        }
      }

      public override void Initialize()
      {
        _self.txtID.KeyDown += ( sender, e ) => { if ( e.KeyCode == Keys.Enter ) this.LookupItem(); };
        _self.txtID.Leave += ( sender, e ) => this.LookupItem();
      }

      public override string Caption
      {
        get { return "Storage"; }
      }

      public override object TargetObject
      {
        get { return _storage; }
        set
        {
          if ( _storage != value ) {
            if ( _storage != null ) _self.labelCount.Text = "(none)";
            _storage = (Core.IStorage)value;
            _lookupKey = string.Empty;
            _self.txtID.Text = string.Empty;
            _self.txtValue.Text = string.Empty;
            if ( _storage != null ) _self.labelCount.Text = _storage.Count.ToString();
          }
        }
      }
    }
  }
}
