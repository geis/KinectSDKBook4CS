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
  public class BreathControl : UserControl, Core.IBreath
  {
    Core.Breath _breath;

    public BreathControl()
    {
      if ( !(this is Core.IDependent) ) _breath.Touch();
    }

    #region IBreath メンバ

    public int BreathCount
    {
      get { return _breath.BreathCount; }
    }

    public void Touch()
    {
      _breath.Touch();
    }

    #endregion
  }
}
