using System.Windows.Forms ;
using System.Drawing ;
using System.Collections.Generic;
using System.IO;
using System;

namespace Hisui.Gui
{
  public class MainForm2 : Form
  {
    public readonly ViewPanel ViewPanel = CreateViewPanel();
    public readonly TreePanel2 TreePanel = CreateTreePanel();
    public readonly MenuStrip MainMenu;
    public readonly Form VersionDialog;
    public readonly SplitContainer SplitContainer;
    public readonly ToolStripContainer ToolStripContainer = new ToolStripContainer();
    public readonly StatusBar StatusBar = new StatusBar();

    public MainForm2( string title, Form versionDlg )
    {
      // プログレスバーのダイアログ表示
      new ProgressDialog().SetUp( this, false );

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

    public MainForm2( string title )
      : this( title, new AboutBox() )
    { }

    public MainForm2()
      : this( "ヒスイ [株式会社カタッチ]" )
    { }


    /// <summary>
    /// <see cref="SingleViewPanel"/> インスタンスを生成し、<c>SetUp()</c> 関数を引数に
    /// <see cref="SI.DocumentViews"/> を指定して呼び出して初期化し、返します。
    /// </summary>
    /// <returns>初期化済みの <see cref="SingleViewPanel"/> インスタンス</returns>
    public static ViewPanel CreateViewPanel()
    {
      //ViewPanel view = new QuadViewPanel() { Dock = DockStyle.Fill };
      ViewPanel view = new SingleViewPanel() { Dock = DockStyle.Fill };
      view.SetUp( SI.DocumentViews );
      return view;
    }


    /// <summary>
    /// <see cref="TreePanel2"/> インスタンスを生成し、<see cref="TreePanel2.SetUp"/> 関数を引数に
    /// <see cref="SI.DocumentViews"/> を指定して呼び出して初期化し、返します。
    /// </summary>
    /// <returns>初期化済みの <see cref="TreePanel2"/> インスタンス</returns>
    public static TreePanel2 CreateTreePanel()
    {
      var tree = new TreePanel2 { Dock = DockStyle.Fill };
      tree.SetUp( SI.DocumentViews );
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
      MainForm.SetupFormSettings( form );
    }
  }
}
