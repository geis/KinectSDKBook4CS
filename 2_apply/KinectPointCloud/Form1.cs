using System.Windows.Forms;

namespace KinectPointCloud
{
  public partial class Form1 : Form
  {
    Hisui.Graphics.GLViewControl
       _view = new Hisui.Graphics.GLViewControl();

    public Form1()
    {
      InitializeComponent();

      // ビューをフォームに配置
      _view.Dock = DockStyle.Fill;
      this.Controls.Add( _view );

      // ビューを DocumentViews に設定し、ビルドグラフに組み込む
      Hisui.SI.DocumentViews.AddView( _view );
      Hisui.SI.Tasks.Add( Hisui.SI.DocumentViews );

      // シーンを起動する
      Hisui.SI.View.SceneGraph.WorldScenes.Add( new PointCloudScene() );
    }
  }
}
