using System;
using System.Collections.Generic;
using System.Text;

namespace OSD.Menu
{
    public delegate void MenuItemChosenDelegate(object sender, MenuItemChosenEventArgs e);

    public class MenuItemChosenEventArgs:EventArgs
    {        
        public MenuItem selectedMenuItem;
        public Menu currentMenu;
    }
}
