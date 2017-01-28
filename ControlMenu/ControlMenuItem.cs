using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Extension.ControlSimpleMenu
{
    public class ControlMenuItem
    {
        public string Caption;
        public object Value;
        public bool HaveChildren;
        public bool Visible;
        public bool Enabled;
        public bool Selected;   

        public ControlMenuItem():this("New Item")
        {
            
        }

        public ControlMenuItem(string caption):this(caption, caption, false)
        {
            
        }

        public ControlMenuItem(string caption, object value, bool haveChildren):this(caption, value, haveChildren, true, true, false)
        {
        }

        public ControlMenuItem(string caption, object value, bool haveChildren, bool visible, bool enabled, bool selected)
        {
            this.Caption = caption;
            this.Value = value;
            this.HaveChildren = haveChildren;
            this.Visible = visible;
            this.Enabled = enabled;
            this.Selected = selected;
        }

        public void Select()
        {
            this.Selected = true;
        }

    }
}
