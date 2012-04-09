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
  public partial class EntryPropertyControl : PropertyControl
  {
    Graphics.MaterialDecoration _material;

    public EntryPropertyControl()
      : base( new Property() )
    {
      InitializeComponent();

      comboLineStipple.Items.Add( Graphics.LineStipple.Solid );
      comboLineStipple.Items.Add( Graphics.LineStipple.Dot );
      comboLineStipple.Items.Add( Graphics.LineStipple.Dash );
      comboLineStipple.Items.Add( Graphics.LineStipple.Dash2 );
      comboLineStipple.SelectedIndex = 0;

      comboShader.Enabled = OpenGL.HiGL.GLSLIsAvailable;
    }


    Graphics.MaterialDecoration MaterialDecoration
    {
      get
      {
        btnMaterial.Enabled = checkMaterial.Checked;
        return checkMaterial.Checked ? _material : null;
      }
      set
      {
        checkMaterial.Checked = btnMaterial.Enabled = (value != null);
        _material = value;
      }
    }

    Graphics.PointSizeDecoration PointSizeDecoration
    {
      get
      {
        numPointSize.Enabled = checkPointSize.Checked;
        return checkPointSize.Checked ?
          new Graphics.PointSizeDecoration( (float)numPointSize.Value ) : null;
      }
      set
      {
        checkPointSize.Checked = numPointSize.Enabled = ( value != null );
        if ( value != null ) numPointSize.Value = (decimal)value.Size;
      }
    }

    Graphics.LineWidthDecoration LineWidthDecoration
    {
      get
      {
        numLineWidth.Enabled = checkLineWidth.Checked;
        return checkLineWidth.Checked ?
          new Graphics.LineWidthDecoration( (float)numLineWidth.Value ) : null;
      }
      set
      {
        checkLineWidth.Checked = numLineWidth.Enabled = (value != null);
        if ( value != null ) numLineWidth.Value = (decimal)value.Width;
      }
    }

    Graphics.LineStippleDecoration LineStippleDecoration
    {
      get
      {
        comboLineStipple.Enabled = checkLineStipple.Checked;
        return checkLineStipple.Checked ?
          new Graphics.LineStippleDecoration( (Graphics.LineStipple)comboLineStipple.SelectedItem ) : null;
      }
      set
      {
        checkLineStipple.Checked = comboLineStipple.Enabled = (value != null);
        if ( value != null ) comboLineStipple.SelectedItem = value.Pattern;
      }
    }

    Graphics.ShaderDecoration ShaderDecoration
    {
      get
      {
        if ( comboShader.SelectedIndex >= 2 ) {
          var name = comboShader.SelectedItem.ToString();
          return new Hisui.Graphics.ShaderDecoration( name );
        }
        return null;
      }
      set
      {
        this.SetShaderDropDownList();
        if ( value == null ) comboShader.SelectedIndex = 0;
        else if ( value.ShaderName == null ) comboShader.SelectedIndex = 0;
        else {
          int index = comboShader.Items.IndexOf( value.ShaderName );
          comboShader.SelectedIndex = (index <= 1) ? index = 0 : index;
        }
      }
    }

    void SetShaderDropDownList()
    {
      comboShader.Items.Clear();
      comboShader.Items.Add( "(Nothing)" );
      comboShader.Items.Add( "(Add ...)" );
      comboShader.Items.AddRange( Graphics.ShaderRegistry.Instance.Names.ToArray() );
    }


    class Property : AbstractProperty<Core.IEntry, EntryPropertyControl>
    {

      public override void Initialize()
      {
        _self.checkEnabled.CheckedChanged += ( sender, e ) => base.Commit(); 
        _self.checkMaterial.CheckedChanged += ( sender, e ) => base.Commit();
        _self.checkPointSize.CheckedChanged += ( sender, e ) => base.Commit();
        _self.checkLineWidth.CheckedChanged += ( sender, e ) => base.Commit();
        _self.checkLineStipple.CheckedChanged += ( sender, e ) => base.Commit();
        _self.numPointSize.ValueChanged += ( sender, e ) => base.Commit();
        _self.numLineWidth.ValueChanged += ( sender, e ) => base.Commit();
        _self.comboLineStipple.SelectedValueChanged += ( sender, e ) => base.Commit();
        _self.comboShader.SelectedValueChanged += ( sender, e ) => base.Commit();

        _self.btnMaterial.Click += ( sender, e ) =>
          {
            var dialog = new MaterialDialog();
            dialog.MaterialDecoration = _self.MaterialDecoration;
            if ( dialog.ShowDialog() == DialogResult.OK ) {
              _self.MaterialDecoration = dialog.MaterialDecoration;
              base.Commit();
            }
          };

        _self.comboShader.SelectedIndexChanged += ( sender, e ) =>
          {
            if ( _self.comboShader.SelectedIndex == 1 ) {
              MessageBox.Show( "シェーダー追加（未実装）" );
              _self.comboShader.SelectedIndex = 0;
            }
          };
      }

      protected override bool IsEqualTo( Hisui.Core.IEntry target )
      {
        return
          _self.checkEnabled.Checked == target.Enabled &&
          AreEqual( _self.MaterialDecoration, target.FindDecoration<Graphics.MaterialDecoration>() ) &&
          AreEqual( _self.PointSizeDecoration, target.FindDecoration<Graphics.PointSizeDecoration>() ) &&
          AreEqual( _self.LineWidthDecoration, target.FindDecoration<Graphics.LineWidthDecoration>() ) &&
          AreEqual( _self.LineStippleDecoration, target.FindDecoration<Graphics.LineStippleDecoration>() ) &&
          AreEqual( _self.ShaderDecoration, target.FindDecoration<Graphics.ShaderDecoration>() );
      }

      protected override void CopyFrom( Hisui.Core.IEntry target )
      {
        _self.checkEnabled.Checked = target.Enabled;
        _self.MaterialDecoration = target.FindDecoration<Graphics.MaterialDecoration>();
        _self.PointSizeDecoration = target.FindDecoration<Graphics.PointSizeDecoration>();
        _self.LineWidthDecoration = target.FindDecoration<Graphics.LineWidthDecoration>();
        _self.LineStippleDecoration = target.FindDecoration<Graphics.LineStippleDecoration>();
        _self.ShaderDecoration = target.FindDecoration<Graphics.ShaderDecoration>();
      }

      protected override void CopyTo( Hisui.Core.IEntry target )
      {
        target.Enabled = _self.checkEnabled.Checked;
        SetDecoration( target, _self.MaterialDecoration );
        SetDecoration( target, _self.PointSizeDecoration );
        SetDecoration( target, _self.LineWidthDecoration );
        SetDecoration( target, _self.LineStippleDecoration );
        SetDecoration( target, _self.ShaderDecoration );
      }

      static void SetDecoration<T>( Core.IEntry entry, T deco ) where T : Graphics.IDecoration
      {
        if ( deco == null )
          entry.RemoveDecoration<T>();
        else
          entry.PutDecoration( deco );
      }

      static bool AreEqual( Graphics.MaterialDecoration x, Graphics.MaterialDecoration y )
      {
        if ( x != null && y != null ) {
          return
            x.UseBackMaterial == y.UseBackMaterial &&
            x.Material == y.Material &&
            x.BackMaterial == y.BackMaterial;
        }
        return x == null && y == null;
      }

      static bool AreEqual( Graphics.PointSizeDecoration x, Graphics.PointSizeDecoration y )
      {
        if ( x != null && y != null ) return x.Size == y.Size;
        return x == null && y == null;
      }

      static bool AreEqual( Graphics.LineWidthDecoration x, Graphics.LineWidthDecoration y )
      {
        if ( x != null && y != null ) return x.Width == y.Width;
        return x == null && y == null;
      }

      static bool AreEqual( Graphics.LineStippleDecoration x, Graphics.LineStippleDecoration y )
      {
        if ( x != null && y != null ) return x.Pattern == y.Pattern;
        return x == null && y == null;
      }

      static bool AreEqual( Graphics.ShaderDecoration x, Graphics.ShaderDecoration y )
      {
        if ( x != null && y != null ) return x.ShaderName == y.ShaderName;
        return x == null && y == null;
      }
    }
  }
}
