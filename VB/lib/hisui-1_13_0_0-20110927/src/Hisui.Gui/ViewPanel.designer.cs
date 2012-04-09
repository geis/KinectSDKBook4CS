namespace Hisui.Gui
{
  partial class ViewPanel
  {
    /// <summary> 
    /// 必要なデザイナ変数です。
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// 使用中のリソースをすべてクリーンアップします。
    /// </summary>
    /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
    protected override void Dispose( bool disposing )
    {
      if ( disposing && (components != null) ) {
        components.Dispose();
      }
      base.Dispose( disposing );
    }

    #region コンポーネント デザイナで生成されたコード

    /// <summary> 
    /// デザイナ サポートに必要なメソッドです。このメソッドの内容を 
    /// コード エディタで変更しないでください。
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ViewPanel ) );
      this.toolStrip1 = new System.Windows.Forms.ToolStrip();
      this.btnPrev = new System.Windows.Forms.ToolStripButton();
      this.btnNext = new System.Windows.Forms.ToolStripButton();
      this.btnFit = new System.Windows.Forms.ToolStripSplitButton();
      this.menuFitXY = new System.Windows.Forms.ToolStripMenuItem();
      this.menuFitYZ = new System.Windows.Forms.ToolStripMenuItem();
      this.menuFitZX = new System.Windows.Forms.ToolStripMenuItem();
      this.btnClipOnOff = new System.Windows.Forms.ToolStripSplitButton();
      this.menuClipView = new System.Windows.Forms.ToolStripMenuItem();
      this.menuClipPlusX = new System.Windows.Forms.ToolStripMenuItem();
      this.menuClipMinusX = new System.Windows.Forms.ToolStripMenuItem();
      this.menuClipPlusY = new System.Windows.Forms.ToolStripMenuItem();
      this.menuClipMinusY = new System.Windows.Forms.ToolStripMenuItem();
      this.menuClipPlusZ = new System.Windows.Forms.ToolStripMenuItem();
      this.menuClipMinusZ = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.btnVisibleSideBoth = new System.Windows.Forms.ToolStripButton();
      this.btnVisibleSideFront = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
      this.btnStyleFace = new System.Windows.Forms.ToolStripButton();
      this.btnStyleEdge = new System.Windows.Forms.ToolStripButton();
      this.btnStyleFaceEdge = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
      this.btnOrtho = new System.Windows.Forms.ToolStripButton();
      this.btnPerspective = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.btnSaveImage = new System.Windows.Forms.ToolStripButton();
      this.btnCopyImage = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
      this.btnSetting = new System.Windows.Forms.ToolStripButton();
      this.btnEnvInfo = new System.Windows.Forms.ToolStripButton();
      this.toolStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // toolStrip1
      // 
      this.toolStrip1.AccessibleDescription = null;
      this.toolStrip1.AccessibleName = null;
      resources.ApplyResources( this.toolStrip1, "toolStrip1" );
      this.toolStrip1.BackgroundImage = null;
      this.toolStrip1.Font = null;
      this.toolStrip1.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.btnPrev,
            this.btnNext,
            this.btnFit,
            this.btnClipOnOff,
            this.toolStripSeparator1,
            this.btnVisibleSideBoth,
            this.btnVisibleSideFront,
            this.toolStripSeparator4,
            this.btnStyleFace,
            this.btnStyleEdge,
            this.btnStyleFaceEdge,
            this.toolStripSeparator5,
            this.btnOrtho,
            this.btnPerspective,
            this.toolStripSeparator2,
            this.btnSaveImage,
            this.btnCopyImage,
            this.toolStripSeparator3,
            this.btnSetting,
            this.btnEnvInfo} );
      this.toolStrip1.Name = "toolStrip1";
      // 
      // btnPrev
      // 
      this.btnPrev.AccessibleDescription = null;
      this.btnPrev.AccessibleName = null;
      resources.ApplyResources( this.btnPrev, "btnPrev" );
      this.btnPrev.BackgroundImage = null;
      this.btnPrev.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnPrev.Name = "btnPrev";
      this.btnPrev.Click += new System.EventHandler( this.btnPrev_Click );
      // 
      // btnNext
      // 
      this.btnNext.AccessibleDescription = null;
      this.btnNext.AccessibleName = null;
      resources.ApplyResources( this.btnNext, "btnNext" );
      this.btnNext.BackgroundImage = null;
      this.btnNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnNext.Name = "btnNext";
      this.btnNext.Click += new System.EventHandler( this.btnNext_Click );
      // 
      // btnFit
      // 
      this.btnFit.AccessibleDescription = null;
      this.btnFit.AccessibleName = null;
      resources.ApplyResources( this.btnFit, "btnFit" );
      this.btnFit.BackgroundImage = null;
      this.btnFit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnFit.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.menuFitXY,
            this.menuFitYZ,
            this.menuFitZX} );
      this.btnFit.Name = "btnFit";
      this.btnFit.ButtonClick += new System.EventHandler( this.btnFit_ButtonClick );
      this.btnFit.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler( this.btnFit_DropDownItemClicked );
      // 
      // menuFitXY
      // 
      this.menuFitXY.AccessibleDescription = null;
      this.menuFitXY.AccessibleName = null;
      resources.ApplyResources( this.menuFitXY, "menuFitXY" );
      this.menuFitXY.BackgroundImage = null;
      this.menuFitXY.Name = "menuFitXY";
      this.menuFitXY.ShortcutKeyDisplayString = null;
      // 
      // menuFitYZ
      // 
      this.menuFitYZ.AccessibleDescription = null;
      this.menuFitYZ.AccessibleName = null;
      resources.ApplyResources( this.menuFitYZ, "menuFitYZ" );
      this.menuFitYZ.BackgroundImage = null;
      this.menuFitYZ.Name = "menuFitYZ";
      this.menuFitYZ.ShortcutKeyDisplayString = null;
      // 
      // menuFitZX
      // 
      this.menuFitZX.AccessibleDescription = null;
      this.menuFitZX.AccessibleName = null;
      resources.ApplyResources( this.menuFitZX, "menuFitZX" );
      this.menuFitZX.BackgroundImage = null;
      this.menuFitZX.Name = "menuFitZX";
      this.menuFitZX.ShortcutKeyDisplayString = null;
      // 
      // btnClipOnOff
      // 
      this.btnClipOnOff.AccessibleDescription = null;
      this.btnClipOnOff.AccessibleName = null;
      resources.ApplyResources( this.btnClipOnOff, "btnClipOnOff" );
      this.btnClipOnOff.BackgroundImage = null;
      this.btnClipOnOff.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnClipOnOff.DropDownItems.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.menuClipView,
            this.menuClipPlusX,
            this.menuClipMinusX,
            this.menuClipPlusY,
            this.menuClipMinusY,
            this.menuClipPlusZ,
            this.menuClipMinusZ} );
      this.btnClipOnOff.Name = "btnClipOnOff";
      this.btnClipOnOff.ButtonClick += new System.EventHandler( this.btnClipOnOff_ButtonClick );
      this.btnClipOnOff.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler( this.btnClipOnOff_DropDownItemClicked );
      // 
      // menuClipView
      // 
      this.menuClipView.AccessibleDescription = null;
      this.menuClipView.AccessibleName = null;
      resources.ApplyResources( this.menuClipView, "menuClipView" );
      this.menuClipView.BackgroundImage = null;
      this.menuClipView.Name = "menuClipView";
      this.menuClipView.ShortcutKeyDisplayString = null;
      // 
      // menuClipPlusX
      // 
      this.menuClipPlusX.AccessibleDescription = null;
      this.menuClipPlusX.AccessibleName = null;
      resources.ApplyResources( this.menuClipPlusX, "menuClipPlusX" );
      this.menuClipPlusX.BackgroundImage = null;
      this.menuClipPlusX.Name = "menuClipPlusX";
      this.menuClipPlusX.ShortcutKeyDisplayString = null;
      // 
      // menuClipMinusX
      // 
      this.menuClipMinusX.AccessibleDescription = null;
      this.menuClipMinusX.AccessibleName = null;
      resources.ApplyResources( this.menuClipMinusX, "menuClipMinusX" );
      this.menuClipMinusX.BackgroundImage = null;
      this.menuClipMinusX.Name = "menuClipMinusX";
      this.menuClipMinusX.ShortcutKeyDisplayString = null;
      // 
      // menuClipPlusY
      // 
      this.menuClipPlusY.AccessibleDescription = null;
      this.menuClipPlusY.AccessibleName = null;
      resources.ApplyResources( this.menuClipPlusY, "menuClipPlusY" );
      this.menuClipPlusY.BackgroundImage = null;
      this.menuClipPlusY.Name = "menuClipPlusY";
      this.menuClipPlusY.ShortcutKeyDisplayString = null;
      // 
      // menuClipMinusY
      // 
      this.menuClipMinusY.AccessibleDescription = null;
      this.menuClipMinusY.AccessibleName = null;
      resources.ApplyResources( this.menuClipMinusY, "menuClipMinusY" );
      this.menuClipMinusY.BackgroundImage = null;
      this.menuClipMinusY.Name = "menuClipMinusY";
      this.menuClipMinusY.ShortcutKeyDisplayString = null;
      // 
      // menuClipPlusZ
      // 
      this.menuClipPlusZ.AccessibleDescription = null;
      this.menuClipPlusZ.AccessibleName = null;
      resources.ApplyResources( this.menuClipPlusZ, "menuClipPlusZ" );
      this.menuClipPlusZ.BackgroundImage = null;
      this.menuClipPlusZ.Name = "menuClipPlusZ";
      this.menuClipPlusZ.ShortcutKeyDisplayString = null;
      // 
      // menuClipMinusZ
      // 
      this.menuClipMinusZ.AccessibleDescription = null;
      this.menuClipMinusZ.AccessibleName = null;
      resources.ApplyResources( this.menuClipMinusZ, "menuClipMinusZ" );
      this.menuClipMinusZ.BackgroundImage = null;
      this.menuClipMinusZ.Name = "menuClipMinusZ";
      this.menuClipMinusZ.ShortcutKeyDisplayString = null;
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.AccessibleDescription = null;
      this.toolStripSeparator1.AccessibleName = null;
      resources.ApplyResources( this.toolStripSeparator1, "toolStripSeparator1" );
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      // 
      // btnVisibleSideBoth
      // 
      this.btnVisibleSideBoth.AccessibleDescription = null;
      this.btnVisibleSideBoth.AccessibleName = null;
      resources.ApplyResources( this.btnVisibleSideBoth, "btnVisibleSideBoth" );
      this.btnVisibleSideBoth.BackgroundImage = null;
      this.btnVisibleSideBoth.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnVisibleSideBoth.Name = "btnVisibleSideBoth";
      this.btnVisibleSideBoth.Click += new System.EventHandler( this.btnVisibleSideBoth_Click );
      // 
      // btnVisibleSideFront
      // 
      this.btnVisibleSideFront.AccessibleDescription = null;
      this.btnVisibleSideFront.AccessibleName = null;
      resources.ApplyResources( this.btnVisibleSideFront, "btnVisibleSideFront" );
      this.btnVisibleSideFront.BackgroundImage = null;
      this.btnVisibleSideFront.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnVisibleSideFront.Name = "btnVisibleSideFront";
      this.btnVisibleSideFront.Click += new System.EventHandler( this.btnVisibleSideFront_Click );
      // 
      // toolStripSeparator4
      // 
      this.toolStripSeparator4.AccessibleDescription = null;
      this.toolStripSeparator4.AccessibleName = null;
      resources.ApplyResources( this.toolStripSeparator4, "toolStripSeparator4" );
      this.toolStripSeparator4.Name = "toolStripSeparator4";
      // 
      // btnStyleFace
      // 
      this.btnStyleFace.AccessibleDescription = null;
      this.btnStyleFace.AccessibleName = null;
      resources.ApplyResources( this.btnStyleFace, "btnStyleFace" );
      this.btnStyleFace.BackgroundImage = null;
      this.btnStyleFace.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnStyleFace.Name = "btnStyleFace";
      this.btnStyleFace.Click += new System.EventHandler( this.btnStyle_ButtonClick );
      // 
      // btnStyleEdge
      // 
      this.btnStyleEdge.AccessibleDescription = null;
      this.btnStyleEdge.AccessibleName = null;
      resources.ApplyResources( this.btnStyleEdge, "btnStyleEdge" );
      this.btnStyleEdge.BackgroundImage = null;
      this.btnStyleEdge.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnStyleEdge.Name = "btnStyleEdge";
      this.btnStyleEdge.Click += new System.EventHandler( this.btnStyle_ButtonClick );
      // 
      // btnStyleFaceEdge
      // 
      this.btnStyleFaceEdge.AccessibleDescription = null;
      this.btnStyleFaceEdge.AccessibleName = null;
      resources.ApplyResources( this.btnStyleFaceEdge, "btnStyleFaceEdge" );
      this.btnStyleFaceEdge.BackgroundImage = null;
      this.btnStyleFaceEdge.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnStyleFaceEdge.Name = "btnStyleFaceEdge";
      this.btnStyleFaceEdge.Click += new System.EventHandler( this.btnStyle_ButtonClick );
      // 
      // toolStripSeparator5
      // 
      this.toolStripSeparator5.AccessibleDescription = null;
      this.toolStripSeparator5.AccessibleName = null;
      resources.ApplyResources( this.toolStripSeparator5, "toolStripSeparator5" );
      this.toolStripSeparator5.Name = "toolStripSeparator5";
      // 
      // btnOrtho
      // 
      this.btnOrtho.AccessibleDescription = null;
      this.btnOrtho.AccessibleName = null;
      resources.ApplyResources( this.btnOrtho, "btnOrtho" );
      this.btnOrtho.BackgroundImage = null;
      this.btnOrtho.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnOrtho.Name = "btnOrtho";
      this.btnOrtho.Click += new System.EventHandler( this.btnOrtho_Click );
      // 
      // btnPerspective
      // 
      this.btnPerspective.AccessibleDescription = null;
      this.btnPerspective.AccessibleName = null;
      resources.ApplyResources( this.btnPerspective, "btnPerspective" );
      this.btnPerspective.BackgroundImage = null;
      this.btnPerspective.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnPerspective.Name = "btnPerspective";
      this.btnPerspective.Click += new System.EventHandler( this.btnPerspective_Click );
      // 
      // toolStripSeparator2
      // 
      this.toolStripSeparator2.AccessibleDescription = null;
      this.toolStripSeparator2.AccessibleName = null;
      resources.ApplyResources( this.toolStripSeparator2, "toolStripSeparator2" );
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      // 
      // btnSaveImage
      // 
      this.btnSaveImage.AccessibleDescription = null;
      this.btnSaveImage.AccessibleName = null;
      resources.ApplyResources( this.btnSaveImage, "btnSaveImage" );
      this.btnSaveImage.BackgroundImage = null;
      this.btnSaveImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnSaveImage.Name = "btnSaveImage";
      this.btnSaveImage.Click += new System.EventHandler( this.btnSaveImage_Click );
      // 
      // btnCopyImage
      // 
      this.btnCopyImage.AccessibleDescription = null;
      this.btnCopyImage.AccessibleName = null;
      resources.ApplyResources( this.btnCopyImage, "btnCopyImage" );
      this.btnCopyImage.BackgroundImage = null;
      this.btnCopyImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnCopyImage.Name = "btnCopyImage";
      this.btnCopyImage.Click += new System.EventHandler( this.btnCopyImage_Click );
      // 
      // toolStripSeparator3
      // 
      this.toolStripSeparator3.AccessibleDescription = null;
      this.toolStripSeparator3.AccessibleName = null;
      resources.ApplyResources( this.toolStripSeparator3, "toolStripSeparator3" );
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      // 
      // btnSetting
      // 
      this.btnSetting.AccessibleDescription = null;
      this.btnSetting.AccessibleName = null;
      resources.ApplyResources( this.btnSetting, "btnSetting" );
      this.btnSetting.BackgroundImage = null;
      this.btnSetting.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnSetting.Name = "btnSetting";
      this.btnSetting.Click += new System.EventHandler( this.btnSetting_Click );
      // 
      // btnEnvInfo
      // 
      this.btnEnvInfo.AccessibleDescription = null;
      this.btnEnvInfo.AccessibleName = null;
      resources.ApplyResources( this.btnEnvInfo, "btnEnvInfo" );
      this.btnEnvInfo.BackgroundImage = null;
      this.btnEnvInfo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnEnvInfo.Name = "btnEnvInfo";
      this.btnEnvInfo.Click += new System.EventHandler( this.btnEnvInfo_Click );
      // 
      // ViewPanel
      // 
      this.AccessibleDescription = null;
      this.AccessibleName = null;
      resources.ApplyResources( this, "$this" );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = null;
      this.Controls.Add( this.toolStrip1 );
      this.Font = null;
      this.Name = "ViewPanel";
      this.toolStrip1.ResumeLayout( false );
      this.toolStrip1.PerformLayout();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ToolStrip toolStrip1;
    //private Graphics.OpenGL.GLViewControl viewControl;
    private System.Windows.Forms.ToolStripButton btnPrev;
    private System.Windows.Forms.ToolStripButton btnNext;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripButton btnPerspective;
    private System.Windows.Forms.ToolStripSplitButton btnFit;
    private System.Windows.Forms.ToolStripMenuItem menuFitXY;
    private System.Windows.Forms.ToolStripMenuItem menuFitYZ;
    private System.Windows.Forms.ToolStripMenuItem menuFitZX;
    private System.Windows.Forms.ToolStripButton btnSetting;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripButton btnSaveImage;
    private System.Windows.Forms.ToolStripButton btnCopyImage;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    private System.Windows.Forms.ToolStripButton btnVisibleSideBoth;
    private System.Windows.Forms.ToolStripButton btnVisibleSideFront;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    private System.Windows.Forms.ToolStripButton btnStyleFace;
    private System.Windows.Forms.ToolStripButton btnStyleEdge;
    private System.Windows.Forms.ToolStripButton btnStyleFaceEdge;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
    private System.Windows.Forms.ToolStripButton btnOrtho;
    private System.Windows.Forms.ToolStripButton btnEnvInfo;
    private System.Windows.Forms.ToolStripSplitButton btnClipOnOff;
    private System.Windows.Forms.ToolStripMenuItem menuClipView;
    private System.Windows.Forms.ToolStripMenuItem menuClipPlusX;
    private System.Windows.Forms.ToolStripMenuItem menuClipMinusX;
    private System.Windows.Forms.ToolStripMenuItem menuClipPlusY;
    private System.Windows.Forms.ToolStripMenuItem menuClipMinusY;
    private System.Windows.Forms.ToolStripMenuItem menuClipPlusZ;
    private System.Windows.Forms.ToolStripMenuItem menuClipMinusZ;
  }
}
