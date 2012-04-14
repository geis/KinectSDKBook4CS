using System.Windows.Forms ;
using System.Drawing ;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace Hisui.Gui
{
  public class MainForm : Form
  {
    public readonly ViewPanel ViewPanel = CreateViewPanel();
    public readonly TreePanel TreePanel = CreateTreePanel();
    public readonly MenuStrip MainMenu;
    public readonly Form VersionDialog;
    public readonly SplitContainer SplitContainer;
    public readonly ToolStripContainer ToolStripContainer = new ToolStripContainer();
    public readonly StatusBar StatusBar = new StatusBar();


    public MainForm( string title, Form versionDlg )
    {
      this.VersionDialog = versionDlg;
      this.MainMenu = CreateMainMenu( versionDlg );
      this.SplitContainer = CreateSplitContainer( this.ViewPanel, this.TreePanel );

      // ツールバーの作成
      foreach ( ToolStrip ts in Ctrl.ToolStripRegistry.ToolStrips ) {
        ToolStripContainer.TopToolStripPanel.Controls.Add( ts );
      }

      // _container
      ToolStripContainer.Dock = DockStyle.Fill;
      ToolStripContainer.TopToolStripPanelVisible = true;
      ToolStripContainer.TopToolStripPanel.Controls.Add( this.MainMenu );
      ToolStripContainer.BottomToolStripPanelVisible = true;
      ToolStripContainer.BottomToolStripPanel.Controls.Add( StatusBar );
      ToolStripContainer.ContentPanel.Controls.Add( SplitContainer );

      // this
      this.Controls.Add( ToolStripContainer );

      // 初期化
      SI.Tasks.Add( this.ViewPanel );
      SI.Tasks.Add( this.TreePanel );
      SI.PushIdlingSelection( SI.DocumentViews, true );  // エントリの選択
      SI.SetupMainForm( this, title );

      // 設定ファイルの読み書き
      SetupFormSettings( this );
    }

    public MainForm( string title )
      : this( title, new AboutBox() )
    { }

    public MainForm()
      : this( "ヒスイ [株式会社カタッチ]" )
    { }


    public static ViewPanel CreateViewPanel()
    {
      //ViewPanel view = new QuadViewPanel() { Dock = DockStyle.Fill };
      ViewPanel view = new SingleViewPanel() { Dock = DockStyle.Fill };
      view.SetUp( Ctrl.Current.DocumentViews );
      return view;
    }


    public static TreePanel CreateTreePanel()
    {
      TreePanel tree = new TreePanel() { Dock = DockStyle.Fill };
      tree.DocumentViews = Hisui.SI.DocumentViews;
      return tree;
    }


    public static MenuStrip CreateMainMenu( Form versionDlg )
    {
      var help = new ToolStripMenuItem( "ヘルプ" );
      var version = new ToolStripMenuItem( "バージョン情報" );
      help.DropDownItems.Add( version );
      version.Click += ( sender, e ) => versionDlg.ShowDialog();
      MenuStrip menu = SI.CreateMainMenu();
      menu.Items.Add( help );
      return menu;
    }


    public static SplitContainer CreateSplitContainer( Control viewPanel, Control treePanel )
    {
      SplitContainer split = new SplitContainer();
      split.Dock = DockStyle.Fill;
      split.SplitterDistance = 7 * split.Width / 10;
      split.Panel1.Controls.Add( viewPanel );
      split.Panel2.Controls.Add( treePanel );
      return split;
    }


    /// <summary>
    /// フォームの位置や大きさの設定ファイルI/Oを設定します。
    /// 設定ファイルを読み込んで値をフォームに設定し、<see cref="Form.FormClosing"/> イベントで設定値を書き込まれるようにします。
    /// </summary>
    /// <param name="form"></param>
    public static void SetupFormSettings( Form form )
    {
      ReadSettings( form, Properties.Settings.Default );
      form.FormClosing += ( sender, e ) => WriteSettings( form, Properties.Settings.Default );
    }


    static void WriteSettings( Form form, Properties.Settings settings )
    {
      // DataBindings を利用するとウィンドウの最大化/最小化を行ったときに
      // 動作がおかしくなってしまうので、DataBindings を利用せずに自分で書き込む。
      settings.MainFormMaximized = (form.WindowState == FormWindowState.Maximized);
      if ( form.WindowState == FormWindowState.Normal ) {
        settings.MainFormClientSize = form.ClientSize;
        settings.MainFormLocation = form.Location;
      }
      settings.Save();
    }

    static void ReadSettings( Form form, Properties.Settings settings )
    {
      form.WindowState = settings.MainFormMaximized ? FormWindowState.Maximized : FormWindowState.Normal;
      var rect = new Rectangle( settings.MainFormLocation, settings.MainFormClientSize );
      if ( Screen.AllScreens.Any( screen => screen.Bounds.IntersectsWith( rect ) ) ) {
        form.StartPosition = FormStartPosition.Manual;
        form.Location = rect.Location;
        form.ClientSize = rect.Size;
      }
      else {
        form.StartPosition = FormStartPosition.CenterScreen;
      }
    }
  }
}
