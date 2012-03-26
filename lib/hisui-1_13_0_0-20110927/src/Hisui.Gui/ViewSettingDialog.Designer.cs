namespace Hisui.Gui
{
  partial class ViewSettingDialog
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

    #region Windows フォーム デザイナで生成されたコード

    /// <summary>
    /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
    /// コード エディタで変更しないでください。
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager( typeof( ViewSettingDialog ) );
      Hisui.Graphics.StandardViewOperation standardViewOperation2 = new Hisui.Graphics.StandardViewOperation();
      this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
      this.viewControl = new Hisui.Graphics.GLViewControl();
      this.groupLight = new System.Windows.Forms.GroupBox();
      this.radio3 = new System.Windows.Forms.RadioButton();
      this.radio2 = new System.Windows.Forms.RadioButton();
      this.radio1 = new System.Windows.Forms.RadioButton();
      this.radio0 = new System.Windows.Forms.RadioButton();
      this.checkEnabled = new System.Windows.Forms.CheckBox();
      this.groupLight.SuspendLayout();
      this.SuspendLayout();
      // 
      // propertyGrid1
      // 
      this.propertyGrid1.AccessibleDescription = null;
      this.propertyGrid1.AccessibleName = null;
      resources.ApplyResources( this.propertyGrid1, "propertyGrid1" );
      this.propertyGrid1.BackgroundImage = null;
      this.propertyGrid1.Font = null;
      this.propertyGrid1.Name = "propertyGrid1";
      this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.NoSort;
      this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler( this.propertyGrid1_PropertyValueChanged );
      // 
      // viewControl
      // 
      this.viewControl.AccessibleDescription = null;
      this.viewControl.AccessibleName = null;
      resources.ApplyResources( this.viewControl, "viewControl" );
      this.viewControl.BackColor = System.Drawing.SystemColors.HotTrack;
      this.viewControl.BackgroundImage = null;
      this.viewControl.Font = null;
      this.viewControl.Name = "viewControl";
      standardViewOperation2.AutoFocus = true;
      standardViewOperation2.DragZoomSign = 1;
      standardViewOperation2.WheelZoomSign = 1;
      this.viewControl.Operation = standardViewOperation2;
      this.viewControl.MouseMove += new System.Windows.Forms.MouseEventHandler( this.viewControl_MouseMove );
      // 
      // groupLight
      // 
      this.groupLight.AccessibleDescription = null;
      this.groupLight.AccessibleName = null;
      resources.ApplyResources( this.groupLight, "groupLight" );
      this.groupLight.BackgroundImage = null;
      this.groupLight.Controls.Add( this.radio3 );
      this.groupLight.Controls.Add( this.radio2 );
      this.groupLight.Controls.Add( this.radio1 );
      this.groupLight.Controls.Add( this.radio0 );
      this.groupLight.Font = null;
      this.groupLight.Name = "groupLight";
      this.groupLight.TabStop = false;
      // 
      // radio3
      // 
      this.radio3.AccessibleDescription = null;
      this.radio3.AccessibleName = null;
      resources.ApplyResources( this.radio3, "radio3" );
      this.radio3.BackgroundImage = null;
      this.radio3.Font = null;
      this.radio3.Name = "radio3";
      this.radio3.UseVisualStyleBackColor = true;
      this.radio3.CheckedChanged += new System.EventHandler( this.ActiveLightChanged );
      // 
      // radio2
      // 
      this.radio2.AccessibleDescription = null;
      this.radio2.AccessibleName = null;
      resources.ApplyResources( this.radio2, "radio2" );
      this.radio2.BackgroundImage = null;
      this.radio2.Font = null;
      this.radio2.Name = "radio2";
      this.radio2.UseVisualStyleBackColor = true;
      this.radio2.CheckedChanged += new System.EventHandler( this.ActiveLightChanged );
      // 
      // radio1
      // 
      this.radio1.AccessibleDescription = null;
      this.radio1.AccessibleName = null;
      resources.ApplyResources( this.radio1, "radio1" );
      this.radio1.BackgroundImage = null;
      this.radio1.Font = null;
      this.radio1.Name = "radio1";
      this.radio1.UseVisualStyleBackColor = true;
      this.radio1.CheckedChanged += new System.EventHandler( this.ActiveLightChanged );
      // 
      // radio0
      // 
      this.radio0.AccessibleDescription = null;
      this.radio0.AccessibleName = null;
      resources.ApplyResources( this.radio0, "radio0" );
      this.radio0.BackgroundImage = null;
      this.radio0.Checked = true;
      this.radio0.Font = null;
      this.radio0.Name = "radio0";
      this.radio0.TabStop = true;
      this.radio0.UseVisualStyleBackColor = true;
      this.radio0.CheckedChanged += new System.EventHandler( this.ActiveLightChanged );
      // 
      // checkEnabled
      // 
      this.checkEnabled.AccessibleDescription = null;
      this.checkEnabled.AccessibleName = null;
      resources.ApplyResources( this.checkEnabled, "checkEnabled" );
      this.checkEnabled.BackgroundImage = null;
      this.checkEnabled.Font = null;
      this.checkEnabled.Name = "checkEnabled";
      this.checkEnabled.UseVisualStyleBackColor = true;
      this.checkEnabled.CheckedChanged += new System.EventHandler( this.LightEnabledChanged );
      // 
      // ViewSettingDialog
      // 
      this.AccessibleDescription = null;
      this.AccessibleName = null;
      resources.ApplyResources( this, "$this" );
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = null;
      this.Controls.Add( this.checkEnabled );
      this.Controls.Add( this.groupLight );
      this.Controls.Add( this.viewControl );
      this.Controls.Add( this.propertyGrid1 );
      this.Font = null;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.Icon = null;
      this.Name = "ViewSettingDialog";
      this.groupLight.ResumeLayout( false );
      this.groupLight.PerformLayout();
      this.ResumeLayout( false );
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PropertyGrid propertyGrid1;
    private Hisui.Graphics.GLViewControl viewControl;
    private System.Windows.Forms.GroupBox groupLight;
    private System.Windows.Forms.RadioButton radio0;
    private System.Windows.Forms.RadioButton radio3;
    private System.Windows.Forms.RadioButton radio2;
    private System.Windows.Forms.RadioButton radio1;
    private System.Windows.Forms.CheckBox checkEnabled;
  }
}
