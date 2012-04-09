namespace Hisui.Gui
{
    partial class TreePanel2
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
          this.components = new System.ComponentModel.Container();
          System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( TreePanel2 ) );
          this.treeView = new System.Windows.Forms.TreeView();
          this.iconList = new System.Windows.Forms.ImageList( this.components );
          this.splitContainer = new System.Windows.Forms.SplitContainer();
          this.propertyPanel1 = new Hisui.Gui.PropertyPanel();
          this.splitContainer.Panel1.SuspendLayout();
          this.splitContainer.Panel2.SuspendLayout();
          this.splitContainer.SuspendLayout();
          this.SuspendLayout();
          // 
          // treeView
          // 
          this.treeView.AllowDrop = true;
          this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
          this.treeView.HideSelection = false;
          this.treeView.ImageIndex = 0;
          this.treeView.ImageList = this.iconList;
          this.treeView.Location = new System.Drawing.Point( 0, 0 );
          this.treeView.Name = "treeView";
          this.treeView.SelectedImageIndex = 0;
          this.treeView.Size = new System.Drawing.Size( 221, 245 );
          this.treeView.TabIndex = 0;
          this.treeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler( this.treeView_BeforeExpand );
          this.treeView.DragDrop += new System.Windows.Forms.DragEventHandler( this.treeView_DragDrop );
          this.treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler( this.treeView_NodeMouseClick );
          this.treeView.AfterExpand += new System.Windows.Forms.TreeViewEventHandler( this.treeView_AfterExpand );
          this.treeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler( this.treeView_ItemDrag );
          this.treeView.DragOver += new System.Windows.Forms.DragEventHandler( this.treeView_DragOver );
          // 
          // iconList
          // 
          this.iconList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject( "iconList.ImageStream" )));
          this.iconList.TransparentColor = System.Drawing.Color.Transparent;
          this.iconList.Images.SetKeyName( 0, "scene.png" );
          this.iconList.Images.SetKeyName( 1, "document.png" );
          this.iconList.Images.SetKeyName( 2, "decoration.png" );
          this.iconList.Images.SetKeyName( 3, "entity.png" );
          this.iconList.Images.SetKeyName( 4, "entry.png" );
          this.iconList.Images.SetKeyName( 5, "entry_gray.png" );
          // 
          // splitContainer
          // 
          this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
          this.splitContainer.Location = new System.Drawing.Point( 0, 0 );
          this.splitContainer.Name = "splitContainer";
          this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
          // 
          // splitContainer.Panel1
          // 
          this.splitContainer.Panel1.Controls.Add( this.treeView );
          // 
          // splitContainer.Panel2
          // 
          this.splitContainer.Panel2.Controls.Add( this.propertyPanel1 );
          this.splitContainer.Size = new System.Drawing.Size( 221, 490 );
          this.splitContainer.SplitterDistance = 245;
          this.splitContainer.TabIndex = 1;
          // 
          // propertyPanel1
          // 
          this.propertyPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
          this.propertyPanel1.Location = new System.Drawing.Point( 0, 0 );
          this.propertyPanel1.Name = "propertyPanel1";
          this.propertyPanel1.Size = new System.Drawing.Size( 221, 241 );
          this.propertyPanel1.TabIndex = 0;
          // 
          // TreePanel2
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F );
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.Controls.Add( this.splitContainer );
          this.Name = "TreePanel2";
          this.Size = new System.Drawing.Size( 221, 490 );
          this.splitContainer.Panel1.ResumeLayout( false );
          this.splitContainer.Panel2.ResumeLayout( false );
          this.splitContainer.ResumeLayout( false );
          this.ResumeLayout( false );

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ImageList iconList;
        private PropertyPanel propertyPanel1;
    }
}
