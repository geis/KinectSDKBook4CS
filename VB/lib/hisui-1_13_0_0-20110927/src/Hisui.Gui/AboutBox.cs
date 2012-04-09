using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;

namespace Hisui.Gui
{
  partial class AboutBox : Form
  {
    public AboutBox()
    {
      InitializeComponent();

      //  アセンブリ情報からの製品情報を表示する情報ボックスを初期化します。
      //  アプリケーションのアセンブリ情報設定を次のいずれかにて変更します:
      //  - [プロジェクト] メニューの [プロパティ] にある [アプリケーション] の [アセンブリ情報]
      //  - AssemblyInfo.cs
      this.Text = String.Format( "{0} のバージョン情報", AssemblyTitle );
      this.labelProductName.Text = AssemblyProduct;
      this.labelVersion.Text = String.Format( "バージョン {0}.{1}", AssemblyVersionMajor, AssemblyVersionMinor );
      this.labelCopyright.Text = AssemblyCopyright;
      this.labelCompanyName.Text = AssemblyCompany;
      this.textBoxDescription.Text = AssemblyDescription;
    }

    #region アセンブリ属性アクセサ

    public string AssemblyTitle
    {
      get { return "ヒスイ"; }
    }

    public int AssemblyVersionMajor
    {
      get { return Assembly.GetExecutingAssembly().GetName().Version.Major; }
    }

    public int AssemblyVersionMinor
    {
      get { return Assembly.GetExecutingAssembly().GetName().Version.Minor; }
    }

    public string AssemblyDescription
    {
      get
      {
        // このアセンブリ上の説明属性をすべて取得します
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes( typeof( AssemblyDescriptionAttribute ), false );
        // 説明属性がない場合、空の文字列を返します
        if ( attributes.Length == 0 )
          return "";
        // 説明属性がある場合、その値を返します
        return ((AssemblyDescriptionAttribute)attributes[0]).Description;
      }
    }

    public string AssemblyProduct
    {
      get { return "ヒスイ - C# による OpenGL プラットフォーム"; }
    }

    public string AssemblyCopyright
    {
      get
      {
        // このアセンブリ上の著作権属性をすべて取得します
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes( typeof( AssemblyCopyrightAttribute ), false );
        // 著作権属性がない場合、空の文字列を返します
        if ( attributes.Length == 0 )
          return "";
        // 著作権属性がある場合、その値を返します
        return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
      }
    }

    public string AssemblyCompany
    {
      get
      {
        // このアセンブリ上の会社属性をすべて取得します
        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes( typeof( AssemblyCompanyAttribute ), false );
        // 会社属性がない場合、空の文字列を返します
        if ( attributes.Length == 0 )
          return "";
        // 会社属性がある場合、その値を返します
        return ((AssemblyCompanyAttribute)attributes[0]).Company;
      }
    }
    #endregion
  }
}
