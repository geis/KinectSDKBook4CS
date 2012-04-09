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
  public partial class PropertyPanel : BreathControl, Core.IBuild
  {
    readonly PropertyRegistry _properties = new PropertyRegistry();
    Graphics.DocumentViews _docviews;

    public PropertyPanel()
    {
      InitializeComponent();
    }

    public void SetUp( Graphics.DocumentViews docviews )
    {
      _docviews = docviews;
    }

    #region IBuild メンバ

    public IEnumerable<Hisui.Core.IBreath> Sources
    {
      get
      {
        yield return _docviews.Document.SelectedEntries;
        yield return _docviews.Document.SelectedEntry;
      }
    }

    public void Build()
    {
      var entry = _docviews.Document.SelectedEntry;
      this.labelCaption.Text = (entry == null) ? "(none)" : entry.ToString();

      // 現在表示されるべきプロパティコントロールを集める
      var controls = _properties.GetControls( entry )
        .WhereNotNull()
        .Distinct()
        .ToArray();

      // 不要な TabPage を削除
      this.tabProperties.TabPages.Cast<TabPage>()
        .Where( page => !controls.Contains( page.Controls[0] as PropertyControl ) )
        .ForEach( page => this.tabProperties.TabPages.Remove( page ) );

      // 不足している TabPage を挿入
      for ( int i = 0 ; i < controls.Length ; ++i ) {
        if ( this.tabProperties.TabPages.Count <= i ||
             this.tabProperties.TabPages[i].Controls[0] as PropertyControl != controls[i] ) {
          this.tabProperties.TabPages.Insert( i, CreateTabPage( controls[i] ) );
        }
      }
    }

    #endregion

    static TabPage CreateTabPage( PropertyControl control )
    {
      var page = new TabPage( control.Caption );
      page.Controls.Add( control );
      control.Dock = DockStyle.Fill;
      return page;
    }
  }
}
