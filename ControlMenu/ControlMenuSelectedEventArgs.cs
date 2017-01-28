using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extension.ControlSimpleMenu
{
    public enum ControlMenuSelectedPostAction
    {
        None,
        Close,
        ChangeScreen
    }

    public class ControlMenuSelectedEventArgs:EventArgs
    {

        public ControlMenuItem selectedControlMenuItem;        
        public int selectedControlMenuItemIndex;

        public ControlMenuSelectedPostAction postAction;
        public string newHeader;
        public ControlMenuItems newItems;
        public int newOffsetIndex;
        public int newSelectedIndex;
        public ChangeScreenTransition changeScreenTransition;
    }
}
