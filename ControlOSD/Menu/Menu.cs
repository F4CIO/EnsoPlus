using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace OSD.Menu
{
    public class Menu:IDisposable
    {
        private Control hostControl;
        public Menu parentMenu;
        public string header;
        public MenuItems items;
        private MenuItems showedItems;
        public MenuItemChosenDelegate menuItemChosenDelegate;        

        private int offset;
        private int _selectedIndex;
        private int selectedIndex
        {
            get
            {
                return this._selectedIndex;
            }
            set
            {
                if (this.items != null)
                {
                    if (this._selectedIndex >= 0 && this.items.Count > this._selectedIndex)
                    {
                        this.items[this._selectedIndex].selected = false;
                    }

                    if (this.items !=null && value >= 0)
                    {
                        this.items[value].selected = true;
                    }
                }
                this._selectedIndex = value;
            }
        }
        internal MenuItem SelectedItem
        {
            get
            {
                if (this.selectedIndex == -1 || this.items == null)
                {
                    return null;
                }
                else
                {
                    return this.items[this.selectedIndex];
                }                
            }
        }

        private bool inSlideProcess = false;

        public event MenuItemChosenDelegate MenuItemClickedEvent;

        public Menu()
        {
        }
                
        public Menu(Menu parentMenu, string header, string folderPath, bool isRootMenu, bool includeFilesFromRoot, bool includeFiles, bool navigateToEmptyFolder, string parentFolderNameAsFirstItem, bool addBracesToFolderNames, MenuItemChosenDelegate menuItemChosenDelegate)
        {
            this.header = header;
            this.parentMenu = parentMenu;
            this.menuItemChosenDelegate = menuItemChosenDelegate;
            this.items = new MenuItems();
            if(!isRootMenu && !string.IsNullOrEmpty(parentFolderNameAsFirstItem))
            {
                MenuItem parentFolderAsFirstItem = new MenuItem(parentFolderNameAsFirstItem, this);
                parentFolderAsFirstItem.tag = folderPath;
                this.items.Add(parentFolderAsFirstItem);
            }
            foreach (string subFolderPath in CraftSynth.BuildingBlocks.IO.FileSystem.GetFolderPaths(folderPath))
            {                
                MenuItem menuItem = new MenuItem();

                string subMenuName = Path.GetFileName(subFolderPath);

                Menu subMenu = new Menu(this, subMenuName, subFolderPath, false, includeFilesFromRoot, includeFiles, navigateToEmptyFolder, parentFolderNameAsFirstItem, addBracesToFolderNames, menuItemChosenDelegate);
                if(addBracesToFolderNames)subMenuName = string.Format("[ {0} ]", subMenuName);                

                menuItem.text = subMenuName;
                menuItem.haveChildren = subMenu.items != null && subMenu.items.Count > 0;
                if (subMenu.items.Count > 0)
                {
                    menuItem.value = subMenu;
                }
                else
                {
                    if (navigateToEmptyFolder)
                    {
                        menuItem.value = subMenu;
                    }
                    else
                    {
                        menuItem.value = subFolderPath;
                    }
                }
                menuItem.tag = subFolderPath;

                this.items.Add(menuItem);
            }
            if ((includeFiles && !isRootMenu)||
                (includeFiles && isRootMenu && includeFilesFromRoot))            
            {
                foreach (string filePath in CraftSynth.BuildingBlocks.IO.FileSystem.GetFilePaths(folderPath))
                {
                    MenuItem menuItem = new MenuItem();

                    menuItem.text = Path.GetFileName(filePath);
                    menuItem.haveChildren = false;
                    menuItem.value = filePath;
                    menuItem.tag = filePath;

                    this.items.Add(menuItem);
                }
            }
        }

        public Menu(string header, MenuItems menuItems):this(null, header, menuItems, null)
        {
        }

        public Menu(Menu parentMenu, string header, MenuItems menuItems): this(parentMenu, header, menuItems, null)
        {
        }

        public Menu(Menu parentMenu, string header, MenuItems menuItems, MenuItemChosenDelegate menuItemChosenDelegate)
        {
            if (parentMenu != null)
            {
                this.parentMenu = parentMenu;
            }
            this.header = header;
            this.items = menuItems;
            this.menuItemChosenDelegate = menuItemChosenDelegate;
        }

        internal void Init(Menu parentMenu, Control hostControl, string header, MenuItems items, int? selectedIndex, MenuItemChosenDelegate menuItemChosenDelegate)
        {

            if (menuItemChosenDelegate != null)
            {
                this.MenuItemClickedEvent += menuItemChosenDelegate;
            }
            if (parentMenu != null)
            {
                this.parentMenu = parentMenu;
            }
            
            this.hostControl = hostControl;
            this.hostControl.PreviewKeyDown += new PreviewKeyDownEventHandler(this.formMain_PreviewKeyDown);
            this.hostControl.Resize += new EventHandler(this.hostControl_Resize);
            if (selectedIndex.HasValue)
            {
                this.selectedIndex = selectedIndex.Value;
            }
            this.menuItemChosenDelegate = menuItemChosenDelegate;

            this.header = header;
            this.items = items;

            this.items.SelectOnly(this.selectedIndex);

            this.UpdateShowedItems();

            RefreshSelection();
        }

        private void RefreshSelection()
        {
            if (this.MoveDown())
            {
                this.MoveUp();
            }
            else
            {
                if (this.MoveUp())
                {
                    this.MoveDown();
                }
            }
        }

        internal void Draw(Graphics g, Rectangle area)
        {
            //mainHeader
            Brush headerBackgroundBrush = new SolidBrush(Settings.Current.mainHeaderBackColor);
            Rectangle headerArea = new Rectangle(area.X, area.Y, area.Width, Settings.Current.mainHeaderHeight);
            g.FillRectangle(headerBackgroundBrush, headerArea);

            Brush headerForegroundBrush = new SolidBrush(Settings.Current.mainHeaderForeColor);
            Point headerLocation = Helper.GetTextLocation(g, this.header, Settings.Current.mainHeaderFont.font, headerArea.Location, headerArea.Size, Settings.Current.menuHeaderAlignment, Settings.Current.margins);
            g.DrawString(this.header, Settings.Current.mainHeaderFont.font, headerForegroundBrush, headerLocation);

            //separator
            Pen framePen = new Pen(new SolidBrush(Settings.Current.frameColor), Settings.Current.frameThickness);
            g.DrawLine(framePen, area.Left-Settings.Current.margins, Settings.Current.mainHeaderHeight + 1, area.Right+Settings.Current.margins, Settings.Current.mainHeaderHeight + 1);

            //items
           DrawItems(g, area);
            
        }

        private void DrawItems(Graphics g, Rectangle area)
        {
            Rectangle itemsArea = new Rectangle(area.X, area.Y + Settings.Current.mainHeaderHeight + Settings.Current.frameThickness, area.Width, 7000);
            showedItems.DrawAll(g, itemsArea);
        }

        private void FixParameters()
        {
            if (this.offset < 0) this.offset = 0;
            if (this.items.Count <= Settings.Current.menuMaximalDisplayedItemsCount) this.offset = 0;
            //if(this.offset>this.items.Count+Settings.Current.menuMaximalDisplayedItemsCount
            if (this.selectedIndex < 0) this.selectedIndex = 0;
            if (this.selectedIndex > this.items.Count - 1) this.selectedIndex = this.items.Count-1;
            if(this.offset<0 || this.offset<this.selectedIndex-Settings.Current.menuMaximalDisplayedItemsCount)
            {
              int r =0;
              this.offset=Math.Max(this.selectedIndex-Math.DivRem(Settings.Current.menuMaximalDisplayedItemsCount, 2, out r), 0);
            }
        }

        private void UpdateShowedItems()
        {
            this.FixParameters();

            this.showedItems = new MenuItems();

            Settings.Current.menuMaximalDisplayedItemsCount = GetItemsCountThatCanBeAccommodatedByHeight();

            if (ArrowsVisible())
            {
                this.showedItems.Add(MenuItem.CreateArrowTop(ArrowTopEnabled()));
            }

            for (int i = 0; i < Math.Min(GetShowedItemsCapacity(), this.items.Count); i++)
            {
                this.showedItems.Add(this.items[this.offset + i]);
            }

            if (ArrowsVisible())
            {
                this.showedItems.Add(MenuItem.CreateArrowBottom(ArrowBottomEnabled()));
            }

        }

        private int GetItemsCountThatCanBeAccommodatedByHeight()
        {
            int availableHeight = this.hostControl.Height - Settings.Current.margins - Settings.Current.mainHeaderHeight - (Settings.Current.frameThickness * 3)-Settings.Current.margins;
            int r = 0;
            return Math.DivRem(availableHeight, Settings.Current.menuItemHeight, out r);
        }

        private bool ArrowsVisible()
        {
            return this.items.Count > Settings.Current.menuMaximalDisplayedItemsCount;
        }

        private int GetShowedItemsCapacity()
        {
            return Settings.Current.menuMaximalDisplayedItemsCount - ((ArrowsVisible()) ? 2 : 0);
        }

        private bool ArrowBottomEnabled()
        {
            return  this.items.Count-1 >= this.offset + GetShowedItemsCapacity();
        }

        private bool ArrowTopEnabled()
        {
            return this.offset > 0;
        }

        private void RefreshShowedItemsWithoutArrows()
        {
        }

        private void RefreshTwoItems(MenuItem item1, MenuItem item2)
        {
        }

        #region Events

        internal bool MoveUp()
        {
            bool moved = false;
            if (this.selectedIndex > 0)
            {
                this.selectedIndex--;

                if (this.selectedIndex == this.offset-1)
                {
                    this.offset--;
                    this.UpdateShowedItems();
                }

                this.hostControl.Refresh();
                moved = true;
            }
            return moved;
        }

        internal bool MoveDown()
        {
            bool moved = false;
            if (this.selectedIndex < this.items.Count-1)
            {
                int currentShowedItemsCapacity = this.GetShowedItemsCapacity();

                this.selectedIndex++;

                if (this.selectedIndex - this.offset > currentShowedItemsCapacity-1)
                {
                    this.offset++;
                    this.UpdateShowedItems();
                }
                
                this.hostControl.Refresh();
                moved = true;
            }
            return moved;
        }


        private void Back()
        {
            if (this.parentMenu != null)
            {
                Menu.ChangePage(this.parentMenu, CraftSynth.BuildingBlocks.UI.WindowsForms.Slider.SlideType.SlideToRight);
            }
        }

        internal void PickCurrentItem() 
        {
            //fire event
            MenuItemChosenEventArgs e = new MenuItemChosenEventArgs();
            e.selectedMenuItem = this.SelectedItem;
            e.currentMenu = this;

            if (this.MenuItemClickedEvent != null)
            {
                this.MenuItemClickedEvent(this, e);
            }
            else
            {
                OSD.Close();
            }

        }

        public void GoToSubMenu()
        {
            //if item is submenu change page
            if (this.SelectedItem.value is Menu)
            {
                Menu newMenu = (Menu)this.SelectedItem.value;
                newMenu.parentMenu = this;
                Menu.ChangePage(newMenu);
            }
        }

        void formMain_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down: this.MoveDown(); break;
                case Keys.Up: this.MoveUp(); break;
                case Keys.Return:
                case Keys.Space: if (!this.inSlideProcess) { this.PickCurrentItem(); } break;
                case Keys.Right: if (!this.inSlideProcess) { this.GoToSubMenu(); } break;
                case Keys.Back:
                case Keys.Left: if (!this.inSlideProcess) { this.Back(); } break;
                case Keys.Escape: if (!this.inSlideProcess)
                    {
                        this.selectedIndex = -1;
                        OSD.Close();
                    }
                    break;
            }            
        }

        void hostControl_Resize(object sender, EventArgs e)
        {
            Settings.Current.menuMaximalDisplayedItemsCount = GetItemsCountThatCanBeAccommodatedByHeight();
            this.UpdateShowedItems();
            this.hostControl.Refresh();
        }

        public static MenuItem ShowMenu(Menu menu)
        {
            return ShowMenu(menu.parentMenu, menu.header, menu.items, menu.selectedIndex, menu.menuItemChosenDelegate);
        }

        public static MenuItem ShowMenu(Menu menu, MenuItemChosenDelegate menuItemChosenDelegate)
        {
            return ShowMenu(menu.parentMenu, menu.header, menu.items, menu.selectedIndex, (menuItemChosenDelegate==null)?menu.menuItemChosenDelegate:menuItemChosenDelegate);
        }

        public static MenuItem ShowMenu(string header, MenuItems menuItems)
        {
            return ShowMenu(header, menuItems,null, null);
        }

        public static MenuItem ShowMenu(string header, MenuItems menuItems, MenuItemChosenDelegate menuItemChosenDelegate)
        {
            return ShowMenu(null, header, menuItems, null, menuItemChosenDelegate);
        }

        public static MenuItem ShowMenu(Menu parentMenu, string header, MenuItems menuItems, MenuItemChosenDelegate menuItemChosenDelegate)
        {
            return ShowMenu(null, parentMenu, header, menuItems, null, menuItemChosenDelegate);
        }

        public static MenuItem ShowMenu(string header, MenuItems menuItems, int? selectedIndex)
        {
            return ShowMenu(header, menuItems, selectedIndex, null);
        }

        public static MenuItem ShowMenu(string header, MenuItems menuItems, int? selectedIndex, MenuItemChosenDelegate menuItemChosenDelegate)
        {
            return ShowMenu(null, header, menuItems, selectedIndex, menuItemChosenDelegate);
        }

        public static MenuItem ShowMenu(Menu parentMenu, string header, MenuItems menuItems, int? selectedIndex, MenuItemChosenDelegate menuItemChosenDelegate)
        {
            return ShowMenu(null, parentMenu, header, menuItems, selectedIndex, menuItemChosenDelegate);
        }

        public static MenuItem ShowMenu(Form ownerForm, Menu parentMenu, string header, MenuItems menuItems, int? selectedIndex, MenuItemChosenDelegate menuItemChosenDelegate)
        {
            OSD.InitializeIfNeeded();

            OSD.menu = new Menu();
            OSD.menu.Init(parentMenu, OSD.formMain, header, menuItems, selectedIndex, menuItemChosenDelegate);
            OSD.formMain.pnlContent.Tag = true;
            OSD.formMain.pnlContent2.Tag = false;

            OSD.formMain.Text = "OSD";
            OSD.formMain.TopLevel = true;
            OSD.formMain.TopMost = true;
            OSD.formMain.Owner = ownerForm;
            OSD.formDisplayer.ShowForm(OSD.formMain);
            OSD.formMain.ShowDialog();            
            return OSD.menu.SelectedItem;
        }

        public static void ChangePage(Menu menu)
        {
            Menu.ChangePage(menu.parentMenu, menu.header, menu.items, menu.selectedIndex, menu.menuItemChosenDelegate);
        }

        public static void ChangePage(string header, MenuItems menuItems)
        {
            Menu.ChangePage(header, menuItems, null, null);
        }

        public static void ChangePage(string header, MenuItems menuItems, MenuItemChosenDelegate menuItemChosenDelegate)
        {
			Menu.ChangePage(null, header, menuItems, null, CraftSynth.BuildingBlocks.UI.WindowsForms.Slider.SlideType.SlideToLeft, menuItemChosenDelegate);
        }

        public static void ChangePage(Menu parentMenu, string header, MenuItems menuItems, MenuItemChosenDelegate menuItemChosenDelegate)
        {
			Menu.ChangePage(parentMenu, header, menuItems, null, CraftSynth.BuildingBlocks.UI.WindowsForms.Slider.SlideType.SlideToLeft, menuItemChosenDelegate);
        }

		public static void ChangePage(Menu menu, CraftSynth.BuildingBlocks.UI.WindowsForms.Slider.SlideType slideType)
        {
            Menu.ChangePage(menu.parentMenu, menu.header, menu.items, menu.selectedIndex, slideType, menu.menuItemChosenDelegate);
        }

		public static void ChangePage(string header, MenuItems menuItems, CraftSynth.BuildingBlocks.UI.WindowsForms.Slider.SlideType slideType)
        {
            Menu.ChangePage(header, menuItems, slideType, null);
        }

		public static void ChangePage(string header, MenuItems menuItems, CraftSynth.BuildingBlocks.UI.WindowsForms.Slider.SlideType slideType, MenuItemChosenDelegate menuItemChosenDelegate)
        {
            Menu.ChangePage(null, header, menuItems, null, slideType, menuItemChosenDelegate);
        }

        public static void ChangePage(string header, MenuItems menuItems, int? selectedIndex)
        {

            Menu.ChangePage(header, menuItems, selectedIndex, null);
        }

        public static void ChangePage(string header, MenuItems menuItems, int? selectedIndex, MenuItemChosenDelegate menuItemChosenDelegate)
        {
			Menu.ChangePage(null, header, menuItems, selectedIndex, CraftSynth.BuildingBlocks.UI.WindowsForms.Slider.SlideType.SlideToLeft, menuItemChosenDelegate);
        }

        public static void ChangePage(Menu parentMenu, string header, MenuItems menuItems, int? selectedIndex, MenuItemChosenDelegate menuItemChosenDelegate)
        {
			Menu.ChangePage(parentMenu, header, menuItems, selectedIndex, CraftSynth.BuildingBlocks.UI.WindowsForms.Slider.SlideType.SlideToLeft, menuItemChosenDelegate);
        }

		public static void ChangePage(string header, MenuItems menuItems, int? selectedIndex, CraftSynth.BuildingBlocks.UI.WindowsForms.Slider.SlideType slideType)
        {
            Menu.ChangePage(header, menuItems, selectedIndex, slideType, null);
        }

        public static void ChangePage(string header, MenuItems menuItems, int? selectedIndex, CraftSynth.BuildingBlocks.UI.WindowsForms.Slider.SlideType slideType, MenuItemChosenDelegate menuItemChosenDelegate)
        {
            Menu.ChangePage(null, header, menuItems, selectedIndex, slideType, menuItemChosenDelegate);
        }

        public static void ChangePage(Menu parentMenu, string header, MenuItems menuItems, int? selectedIndex, CraftSynth.BuildingBlocks.UI.WindowsForms.Slider.SlideType slideType, MenuItemChosenDelegate menuItemChosenDelegate)
        {
            OSD.menu2 = new Menu();

            OSD.menu.inSlideProcess = true;
            OSD.menu2.inSlideProcess = true;

            if (menuItemChosenDelegate != null)
            {
                OSD.menu.menuItemChosenDelegate = menuItemChosenDelegate;
            }

            OSD.menu2.Init(parentMenu, OSD.formMain, header, menuItems, selectedIndex,  OSD.menu.menuItemChosenDelegate);

            OSD.formMainSlider.Execute(OSD.formMain.pnlContent, OSD.formMain.pnlContent2, slideType, true);

            //switch back panels
            OSD.formMain.pnlContent.Tag = !((bool)OSD.formMain.pnlContent.Tag);
            OSD.formMain.pnlContent2.Tag = !((bool)OSD.formMain.pnlContent2.Tag);

            Panel pnlTemp = OSD.formMain.pnlContent;
            OSD.formMain.pnlContent = OSD.formMain.pnlContent2;
            OSD.formMain.pnlContent2 = pnlTemp;
            

            //switch back menus
            Menu tempMenu = OSD.menu;
            OSD.menu = OSD.menu2;
            tempMenu.Dispose();

            //refresh
            //int selectedIndex = OSD.menu.selectedIndex;
            //OSD.menu.MoveDown();OSD.menu.ref

            OSD.formMain.pnlContent.BringToFront();
            OSD.formMain.pnlContentParent.Refresh();
            Application.DoEvents();

            OSD.menu.inSlideProcess = false ;            
        }

        public void Dispose()
        {
            if (menuItemChosenDelegate != null && this.MenuItemClickedEvent != null)
            {
                this.MenuItemClickedEvent -= menuItemChosenDelegate;
            }
            this.hostControl.PreviewKeyDown -= new PreviewKeyDownEventHandler(this.formMain_PreviewKeyDown);
            this.hostControl.Resize -= new EventHandler(this.hostControl_Resize);
        }

        public static void Close()
        {      
            OSD.Close();          
        }

        #endregion Events
    }
}
