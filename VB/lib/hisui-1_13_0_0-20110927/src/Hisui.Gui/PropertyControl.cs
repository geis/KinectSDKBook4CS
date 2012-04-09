using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hisui.Gui
{
  public class PropertyControl : BreathControl
  {
    readonly AbstractProperty _property;

    public PropertyControl() 
      : this( new DefaultProperty() ) 
    { }
    
    protected PropertyControl( AbstractProperty property ) 
    {
      _property = property;
      _property.SetUp( this );
    }

    protected override void OnCreateControl()
    {
      base.OnCreateControl();
      _property.Initialize();
    }

    public string Caption
    {
      get { return _property.Caption; }
    }

    public object TargetObject
    {
      get { return _property.TargetObject; }
      set { _property.TargetObject = value; }
    }

    protected abstract class AbstractProperty
    {
      public virtual void SetUp( PropertyControl ctrl ) { }
      public abstract void Initialize();
      public abstract string Caption { get; }
      public abstract object TargetObject { get; set; }
    }

    protected class DefaultProperty : AbstractProperty
    {
      public override void Initialize() { }
      public override string Caption { get { return typeof( object ).Name; } }
      public override object TargetObject { get; set; }
    }

    protected abstract class AbstractProperty<TControl> : AbstractProperty
      where TControl : PropertyControl
    {
      protected TControl _self;
      public override void SetUp( PropertyControl ctrl ) { _self = (TControl)ctrl; }
    }

    protected abstract class AbstractProperty<TTarget, TControl> : AbstractProperty<TControl>
      where TControl : PropertyControl
    {
      TTarget _target;
      bool _locked;

      void LockExecute( Action action )
      {
        if ( !_locked ) {
          _locked = true;
          try { action(); }
          finally { _locked = false; }
        }
      }

      public override string Caption
      {
        get { return typeof( TTarget ).Name; }
      }

      public override object TargetObject
      {
        get { return _target; }
        set { _target = (TTarget)value; this.LockExecute( () => this.CopyFrom( _target ) ); }
      }

      protected void Commit()
      {
        if ( !_locked && !this.IsEqualTo( _target ) ) {
          this.LockExecute( () => { this.CopyTo( _target ); SI.Commit(); } );
        }
      }

      protected void RefreshView()
      {
        if ( !_locked && !this.IsEqualTo( _target ) ) {
          this.LockExecute( () => { this.CopyTo( _target ); SI.View.Refresh(); } );
        }
      }

      protected abstract bool IsEqualTo( TTarget target );
      protected abstract void CopyFrom( TTarget target );
      protected abstract void CopyTo( TTarget target );
    }
  }
}
