using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Hisui.Std
{
  partial class Menu
  {
    [Ctrl.Command( "ビュー", Order = 300 )]
    static class View
    {
      [Ctrl.Command( "視点 元に戻す", Order = 110 )]
      static void Prev( Ctrl.IContext con, Ctrl.CommandOption opt )
      {
        if ( opt.QueryRunnable )
          opt.QueryRunnable = SI.DocumentViews.ActiveHistory.HasPrev;
        else
          SI.DocumentViews.ActiveHistory.Prev();
      }

      [Ctrl.Command( "視点 やり直し", Order = 120 )]
      static void Next( Ctrl.IContext con, Ctrl.CommandOption opt )
      {
        if ( opt.QueryRunnable )
          opt.QueryRunnable = SI.DocumentViews.ActiveHistory.HasNext;
        else
          SI.DocumentViews.ActiveHistory.Next();
      }

      [Ctrl.Command( "フィット", Order = 130 )]
      static class Fit
      {
        [Ctrl.Command( "フィット", Order = 1, ShortcutKeys = Keys.Control | Keys.Home )]
        static void FitAsIs() { SI.DocumentViews.Fit(); }

        [Ctrl.Command( "XY", Order = 2 )]
        static void FitXY() { SI.DocumentViews.Fit( Geom.CodAxis.Z ); }

        [Ctrl.Command( "YZ", Order = 3 )]
        static void FitYZ() { SI.DocumentViews.Fit( Geom.CodAxis.X ); }

        [Ctrl.Command( "ZX", Order = 4 )]
        static void FitZX() { SI.DocumentViews.Fit( Geom.CodAxis.Y ); }
      }

      [Ctrl.Command( "断面表示", Order = 140 )]
      static class ClipPlane
      {
        [Ctrl.Command( "断面 ON/OFF", Order = 1 )]
        static void OnOff( Ctrl.IContext con, Ctrl.CommandOption opt )
        {
          if ( opt.QueryRunnable )
            opt.IsChecked = (Gui.ClipPlane.Find( SI.DocumentViews ) != null);
          else {
            if ( Gui.ClipPlane.Find( SI.DocumentViews ) != null )
              Gui.ClipPlane.Remove( SI.DocumentViews );
            else
              Gui.ClipPlane.Get( SI.DocumentViews );
          }
        }

        [Ctrl.Command( "-", Order = 2 )]
        static void _1() { }

        [Ctrl.Command( "ビュー", Order = 3 )]
        static void View( Ctrl.IContext con, Ctrl.CommandOption opt )
        {
          ToggleClipPlane( Gui.ClipDirections.View, opt );
        }

        [Ctrl.Command( "+X", Order = 4 )]
        static void PlusX( Ctrl.IContext con, Ctrl.CommandOption opt )
        {
          ToggleClipPlane( Gui.ClipDirections.PlusX, opt );
        }

        [Ctrl.Command( "-X", Order = 5 )]
        static void MinusX( Ctrl.IContext con, Ctrl.CommandOption opt )
        {
          ToggleClipPlane( Gui.ClipDirections.MinusX, opt );
        }

        [Ctrl.Command( "+Y", Order = 6 )]
        static void PlusY( Ctrl.IContext con, Ctrl.CommandOption opt )
        {
          ToggleClipPlane( Gui.ClipDirections.PlusY, opt );
        }

        [Ctrl.Command( "-Y", Order = 7 )]
        static void MinusY( Ctrl.IContext con, Ctrl.CommandOption opt )
        {
          ToggleClipPlane( Gui.ClipDirections.MinusY, opt );
        }

        [Ctrl.Command( "+Z", Order = 8 )]
        static void PlusZ( Ctrl.IContext con, Ctrl.CommandOption opt )
        {
          ToggleClipPlane( Gui.ClipDirections.PlusZ, opt );
        }

        [Ctrl.Command( "-Z", Order = 9 )]
        static void MinusZ( Ctrl.IContext con, Ctrl.CommandOption opt )
        {
          ToggleClipPlane( Gui.ClipDirections.MinusZ, opt );
        }

        static void ToggleClipPlane( Gui.ClipDirections dir, Ctrl.CommandOption opt )
        {
          if ( opt.QueryRunnable )
            opt.IsChecked = (Gui.ClipPlane.GetClipDirection( SI.DocumentViews ) == dir);
          else
            Gui.ClipPlane.SetClipDirection( SI.DocumentViews, dir );
        }

        [Ctrl.Command( "-", Order = 10 )]
        static void _2() { } ///////////////////////////////////////////////////////

        [Ctrl.Command( "キャッピング", Order = 11 )]
        static void Capping( Ctrl.IContext con, Ctrl.CommandOption opt )
        {
          if ( opt.QueryRunnable )
            opt.IsChecked = Gui.ClipPlane.Capping;
          else
            Gui.ClipPlane.Capping = !Gui.ClipPlane.Capping;
        }
      }

      [Ctrl.Command( "-", Order = 200 )]
      static void _10() { } ///////////////////////////////////////////////////////

      [Ctrl.Command( "回転", Order = 210 )]
      static Ctrl.IOperation Rotate( Ctrl.IContext con, Ctrl.CommandOption opt )
      {
        return RunViewOperation( Graphics.ViewOperations.Rotation, con, opt );
      }

      [Ctrl.Command( "パン", Order = 220 )]
      static Ctrl.IOperation Pan( Ctrl.IContext con, Ctrl.CommandOption opt )
      {
        return RunViewOperation( Graphics.ViewOperations.Pan, con, opt );
      }

      [Ctrl.Command( "ズーム", Order = 230 )]
      static Ctrl.IOperation Zoom( Ctrl.IContext con, Ctrl.CommandOption opt )
      {
        return RunViewOperation( Graphics.ViewOperations.Zoom, con, opt );
      }

      [Ctrl.Command( "スピン", Order = 240 )]
      static Ctrl.IOperation Spin( Ctrl.IContext con, Ctrl.CommandOption opt )
      {
        return RunViewOperation( Graphics.ViewOperations.Spin, con, opt );
      }

      static Ctrl.IOperation RunViewOperation( 
        Graphics.ViewOperations operation, Ctrl.IContext con, Ctrl.CommandOption opt )
      {
        var viewop = con.View.Operation as Graphics.StandardViewOperation;
        if ( opt.QueryRunnable ) {
          opt.QueryRunnable = (viewop != null);
          opt.IsChecked = (viewop.TemporaryBindings.Find( operation ) != null);
          return null;
        }
        if ( SI.Driver.ActiveOperation is ViewOperation &&
          ((ViewOperation)SI.Driver.ActiveOperation).OperationType == operation ) {
          SI.Driver.Abort();
          return null;
        }
        return new ViewOperation( con, operation );
      }

      class ViewOperation : Ctrl.Operation
      {
        public readonly Graphics.ViewOperations OperationType;

        public ViewOperation( Ctrl.IContext con, Graphics.ViewOperations operation )
          : base( con )
        {
          this.OperationType = operation;
          var viewop = con.View.Operation as Graphics.StandardViewOperation;
          var binding = new Graphics.ViewOperationBinding( operation, MouseButtons.Left, Keys.None );
          this.Entered += ( sender, e ) => viewop.TemporaryBindings.Reset( binding );
          this.Exited += ( sender, e ) => viewop.TemporaryBindings.Clear();
          this.Cursor = GetCursor( operation );
        }

        static Cursor GetCursor( Graphics.ViewOperations operation )
        {
          switch ( operation ) {
            case Graphics.ViewOperations.Rotation:
              return new Cursor( Properties.Resources.rotate.GetHicon() );
            case Graphics.ViewOperations.Pan:
              return new Cursor( Properties.Resources.pan.GetHicon() );
            case Graphics.ViewOperations.Zoom:
              return new Cursor( Properties.Resources.zoom.GetHicon() );
            case Graphics.ViewOperations.Spin:
              return new Cursor( Properties.Resources.spin.GetHicon() );
            default:
              return Cursors.Default;
          }
        }
      }

      [Ctrl.Command( "-", Order = 300 )]
      static void _20() { } ///////////////////////////////////////////////////////

      [Ctrl.Command( "裏側も描画", Order = 310 )]
      static void RenderBothFace( Ctrl.IContext con, Ctrl.CommandOption opt )
      {
        if ( opt.QueryRunnable )
          opt.IsChecked = Gui.ViewSetting.Current.Backside;
        else
          Gui.ViewSetting.Current.Backside = true;
      }

      [Ctrl.Command( "表側のみ描画", Order = 320 )]
      static void RenderFrontFace( Ctrl.IContext con, Ctrl.CommandOption opt )
      {
        if ( opt.QueryRunnable )
          opt.IsChecked = !Gui.ViewSetting.Current.Backside;
        else
          Gui.ViewSetting.Current.Backside = false;
      }

      [Ctrl.Command( "-", Order = 400 )]
      static void _30() { } ///////////////////////////////////////////////////////

      [Ctrl.Command( "シェーディング", Order = 410 )]
      static void PolygonStyleFace( Ctrl.IContext con, Ctrl.CommandOption opt )
      {
        SetPolygonStyle( opt, Graphics.PolygonStyles.Face );
      }

      [Ctrl.Command( "ワイヤフレーム", Order = 420 )]
      static void PolygonStyleEdge( Ctrl.IContext con, Ctrl.CommandOption opt )
      {
        SetPolygonStyle( opt, Graphics.PolygonStyles.Edge );
      }

      [Ctrl.Command( "シェーディング+ワイヤ", Order = 430 )]
      static void PolygonStyleFaceEdge( Ctrl.IContext con, Ctrl.CommandOption opt )
      {
        SetPolygonStyle( opt, Graphics.PolygonStyles.Face | Graphics.PolygonStyles.Edge );
      }

      static void SetPolygonStyle( Ctrl.CommandOption opt, Graphics.PolygonStyles style )
      {
        if ( opt.QueryRunnable )
          opt.IsChecked = (Gui.ViewSetting.Current.PolygonStyle == style);
        else
          Gui.ViewSetting.Current.PolygonStyle = style;
      }

      [Ctrl.Command( "-", Order = 500 )]
      static void _40() { } ///////////////////////////////////////////////////////

      [Ctrl.Command( "平行投影", Order = 510 )]
      static void Ortho( Ctrl.IContext con, Ctrl.CommandOption opt )
      {
        if ( opt.QueryRunnable )
          opt.IsChecked = !Gui.ViewSetting.Current.Perspective;
        else
          Gui.ViewSetting.Current.Perspective = false;
      }

      [Ctrl.Command( "透視投影", Order = 520 )]
      static void Perspective( Ctrl.IContext con, Ctrl.CommandOption opt )
      {
        if ( opt.QueryRunnable )
          opt.IsChecked = Gui.ViewSetting.Current.Perspective;
        else
          Gui.ViewSetting.Current.Perspective = true;
      }

      [Ctrl.Command( "-", Order = 600 )]
      static void _50() { } ///////////////////////////////////////////////////////

      [Ctrl.Command( "座標軸表示", Order = 610 )]
      static void ShowAxis( Ctrl.IContext con, Ctrl.CommandOption opt )
      {
        if ( opt.QueryRunnable )
          opt.IsChecked = Gui.ViewSetting.Current.ShowAxis;
        else
          Gui.ViewSetting.Current.ShowAxis = !Gui.ViewSetting.Current.ShowAxis;
      }

      [Ctrl.Command( "コンパス表示", Order = 620 )]
      static void ShowCompass( Ctrl.IContext con, Ctrl.CommandOption opt )
      {
        if ( opt.QueryRunnable )
          opt.IsChecked = Gui.ViewSetting.Current.ShowCompass;
        else
          Gui.ViewSetting.Current.ShowCompass = !Gui.ViewSetting.Current.ShowCompass;
      }

      [Ctrl.Command( "ルーラー表示", Order = 630 )]
      static void ShowRuler( Ctrl.IContext con, Ctrl.CommandOption opt )
      {
        if ( opt.QueryRunnable )
          opt.IsChecked = Gui.ViewSetting.Current.ShowRuler;
        else
          Gui.ViewSetting.Current.ShowRuler = !Gui.ViewSetting.Current.ShowRuler;
      }

      [Ctrl.Command( "-", Order = 700 )]
      static void _60() { } ///////////////////////////////////////////////////////

      [Ctrl.Command( "画像を保存", Order = 710 )]
      static void SaveImage()
      {
        SaveFileDialog dialog = new SaveFileDialog();
        dialog.Title = "ビューを画像として保存";
        dialog.Filter = "PNG files (*.png)|*.png|JPEG files (*.jpg)|*.jpg|All files (*.*)|*.*";
        if ( dialog.ShowDialog() == DialogResult.OK ) {
          Image image = SI.DocumentViews.ActiveView.GetImage();
          if ( dialog.FileName.EndsWith( "png" ) ) {
            image.Save( dialog.FileName, System.Drawing.Imaging.ImageFormat.Png );
          }
          else if ( dialog.FileName.EndsWith( "jpg" ) ) {
            image.Save( dialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg );
          }
          else {
            image.Save( dialog.FileName );
          }
        }
      }

      [Ctrl.Command( "画像をコピー", Order = 720 )]
      static void CopyImage()
      {
        Image image = SI.DocumentViews.ActiveView.GetImage();
        Clipboard.SetDataObject( image );
      }

      [Ctrl.Command( "-", Order = 800 )]
      static void _70() { } ///////////////////////////////////////////////////////

      [Ctrl.Command( "Z軸が傾かないビュー回転", Order = 810 )]
      static void ZKeptViewOperation( Ctrl.IContext con, Ctrl.CommandOption opt )
      {
        var viewop = con.View.Operation as Graphics.StandardViewOperation;
        if ( opt.QueryRunnable ) {
          opt.QueryRunnable = (viewop != null);
          opt.IsChecked = (viewop != null && viewop.ZKeptRotation);
        }
        else if ( viewop != null ) {
          viewop.ZKeptRotation = !viewop.ZKeptRotation;
        }
      }

      [Ctrl.Command( "マウス位置を中心にズーム", Order = 820 )]
      static void ZoomToMousePosition( Ctrl.IContext con, Ctrl.CommandOption opt )
      {
        var viewop = con.View.Operation as Graphics.StandardViewOperation;
        if ( opt.QueryRunnable ) {
          opt.QueryRunnable = (viewop != null);
          opt.IsChecked = (viewop != null && viewop.ZoomToMousePosition);
        }
        else if ( viewop != null ) {
          viewop.ZoomToMousePosition = !viewop.ZoomToMousePosition;
        }
      }

      [Ctrl.Command( "設定", Order = 830 )]
      static void Setting( Ctrl.IContext con )
      {
        var dialog = new Gui.ViewSettingDialog( SI.DocumentViews, Gui.ViewSetting.Current );
        dialog.Owner = con.View.FindForm();
        dialog.StartPosition = FormStartPosition.CenterParent;
        dialog.ShowDialog();
      }

      [Ctrl.Command( "環境情報", Order = 840 )]
      static void EnvInfo()
      {
        var dialog = new Gui.EnvInfoDialog();
        dialog.StartPosition = FormStartPosition.CenterParent;
        dialog.ShowDialog();
      }
    }
  }
}
