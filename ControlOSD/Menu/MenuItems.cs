using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace OSD.Menu
{
    public class MenuItems:List<MenuItem>
    {
        public MenuItems():base()
        {            
        }

        public MenuItems(List<string> stringList)
        {
            //base = new List<MenuItem>();
            foreach (string itemString in stringList)
            {
                MenuItem item = new MenuItem(itemString);
                base.Add(item);
            }
        }

        public MenuItems(Dictionary<string, object> stringObjectList)
        {
            //base = new List<MenuItem>();
            foreach (var pair in stringObjectList)
            {
                MenuItem item = new MenuItem(pair.Key, pair.Value);
                base.Add(item);
            }
        }

        internal void SelectOnly(int selectedIndex)
        {
            for (int i = 0; i < this.Count; i++)
            {
                this[i].selected = i == selectedIndex;
            }
        }

        internal void DrawAll(Graphics g, Rectangle area)
        {
            Rectangle itemArea = new Rectangle(area.X, area.Y, area.Width, Settings.Current.menuItemHeight);
            foreach (MenuItem menuItem in this)
            {                
                menuItem.Draw(g,itemArea);
                itemArea.Y += Settings.Current.menuItemHeight;
            }
        }
    }
}
