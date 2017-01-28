using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extension.ControlSimpleMenu
{
    public class ControlMenuItems:List<ControlMenuItem>
    {
        public ControlMenuItems()
        {

        }

        public ControlMenuItems(List<string> captions)
        {            
            foreach (string caption in captions)
            {
                this.Add(new ControlMenuItem(caption));
            }
        }

        public void Draw(ControlMenu controlMenu)
        {
            //int i = controlMenu._offsetIndex;
            //while (i < this.Count && i-controlMenu._offsetIndex<controlMenu._showedItemsCount)
            //{ 
            //}

            //foreach (ControlMenuItem item in this)
            //{
            //    item.Draw(controlMenu);
            //}
        }
     
    }
}
