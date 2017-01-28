using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Extension.ControlSimpleMenu
{  
    public class ControlMenu
    {
        internal double _backgroundOpacity;
        internal Color _backgroundColor;
        internal Color _frameColor;
        internal FrameVisibility _frameVisibility;
        internal ContentAlignment _headerAlign;
        internal Font _headerFont;
        internal ContentAlignment _itemsAlignment;
        internal Color _itemsColor;
        internal Font _itemsFont;
        internal int _itemHeight;
        internal int _showedItemsCount;
        internal int _showedItemsCountWithArrows;
        internal Size? _size;
        internal Point? _location;
        internal FormBackground _formBackground;
        internal FormContent _formContent;
        internal string _arrowTopText;
        internal string _arrowBottomText;
        internal string _arrowRightText;

        internal string _headerText;
        internal ControlMenuItems _items;
        internal int _offsetIndex;
        private int _selectedIndex;
        public delegate void ControlMenuItemSelectedDelegate(object sender, ControlMenuSelectedEventArgs e);
        private event ControlMenuItemSelectedDelegate OnItemSelected;

        internal int SelectedIndex
        {
            get { return _selectedIndex; }
            set {
                //multiselect not supported - deselect old selection
                if (value >= 0 && this._selectedIndex != value && this._selectedIndex <= this._items.Count)
                {
                    this._items[this._selectedIndex].Selected = false;
                }
                _selectedIndex = value;
                this._items[this._selectedIndex].Selected = true;
            }
        }      

        private void DeselectAll()
        {
            foreach (var item in this._items)
            {
                item.Selected = false;
            }
        }

        private void Select(int index)
        {
            this._selectedIndex = index;
        }

        /// <summary>
        /// Constructor that uses default values.
        /// </summary>
        public ControlMenu():this(
            //Defaults:
                0.75,//backgroundOpacity
                Color.Black,//backgroundColor
                Color.Orange,//frameColor 
                FrameVisibility.Full,//frameVisibility
                ContentAlignment.MiddleCenter,//headerAlign
                new Font(new FontFamily("Times New Roman"), 28, FontStyle.Regular, GraphicsUnit.Pixel),//headerFont                
                ContentAlignment.MiddleCenter,//itemsAlign
                Color.DarkOliveGreen,//itemsColor
                new Font(new FontFamily("Arial"), 28, FontStyle.Regular, GraphicsUnit.Pixel),//itemsFont
                50,//itemHeight
                5,//showedItemsCount
                "▲",
                "▼",
                "►"
                )
        {
        }

        public ControlMenu(
             double backgroundOpacity,
             Color backgroundColor,
             Color frameColor,
             FrameVisibility frameVisibility,
             ContentAlignment headerAlign,
             Font headerFont,      
             ContentAlignment itemsAlignment,
             Color itemsColor,
             Font itemsFont,     
             int itemHeight,
             int showedItemsCountWithArrows,
             string arrowTopText,
             string arrowBottomText,
             string arrowRightText
            )
        {
            this._backgroundOpacity = backgroundOpacity;
            this._backgroundColor = backgroundColor;
            this._frameColor = frameColor;
            this._frameVisibility = frameVisibility;
            this._headerAlign = headerAlign;
            this._headerFont  = headerFont;
            this._itemsAlignment = itemsAlignment;
            this._itemsColor = itemsColor;
            this._itemsFont = itemsFont;
            this._itemHeight = itemHeight;
            this._showedItemsCountWithArrows = showedItemsCountWithArrows;
            this._arrowTopText = arrowTopText;
            this._arrowBottomText = arrowBottomText;
            this._arrowRightText = arrowRightText;
        }

        public void Execute(string headerText, ControlMenuItems items, int offsetIndex, int selectedIndex, ControlMenuItemSelectedDelegate onItemSelectedDelegate)
        {
            FormBackground formBackground = new FormBackground();
            formBackground.Tag = this;
            //rest of initialization is in Load event    
            this._formBackground = formBackground;
            formBackground.TopMost = true;
            formBackground.Show();

            FormContent formContent = new FormContent(this);
            formContent.Tag = this;
            formContent.Owner = formBackground;
            formContent.TopMost = true;
            formContent.ShowInTaskbar = false;
            formContent.Size = (this._size.HasValue) ? this._size.Value : formContent.Size;
            formContent.Location = (this._location.HasValue) ? this._location.Value : formContent.Location;
            formContent.Owner.Size = formContent.Size;
            formContent.Owner.Location = formContent.Location;
            formContent._arrowTopText = this._arrowTopText;
            formContent._arrowBottomText = this._arrowBottomText;
            formContent._arrowRightText = this._arrowRightText;
            //rest of initialization is in Load event  
            this._formContent = formContent;
            
            
            this._headerText = headerText;
            this._items = items;
            this._formContent.BottomArrowVisible = this._items.Count > this._showedItemsCountWithArrows;
            this._formContent.TopArrowVisible = this._items.Count > this._showedItemsCountWithArrows;
            
            this._showedItemsCount = (this._items.Count > this._showedItemsCountWithArrows)?
                Helper.Min(this._showedItemsCountWithArrows-2, this._items.Count):
                Helper.Min(this._showedItemsCountWithArrows, this._items.Count)
                ;       
            this._offsetIndex = offsetIndex;
            this.SelectedIndex = selectedIndex;
           
            this._formContent.InitializeItems(this, Helper.Min(this._showedItemsCount, this._items.Count));
            this.UpdateAllShowedItems();

            this.OnItemSelected -= onItemSelectedDelegate;
            this.OnItemSelected += onItemSelectedDelegate;
            formContent.ShowDialog();
            
        }

        public void ChangeScreen(string headerText, ControlMenuItems items, int offsetIndex, int selectedIndex, ChangeScreenTransition transition)
        {
            //not implemented
        }

        public void Close()
        {
            if(this._formContent!=null)
            {
                this._formContent.Close();
                this._formBackground.Close();
                if (this._formContent != null) this._formContent = null;
                if (this._formBackground != null) this._formBackground = null;
            }
        }

        private void UpdateAllShowedItems()
        {            
            int itemIndex = this._offsetIndex;
            this._formContent.TopArrowText = (this._offsetIndex > 0)?this._arrowTopText:string.Empty;            
            int labelIndex = 0;
            while(itemIndex<this._items.Count && itemIndex-this._offsetIndex<this._showedItemsCount)
            {
                this._formContent.UpdateItem(this, labelIndex, this._items[itemIndex]);
                itemIndex++;
                labelIndex++;
            }
            this._formContent.BottomArrowText = (this._offsetIndex + this._showedItemsCount < this._items.Count-1)?this._arrowBottomText:string.Empty;
            this._formContent.FocusHeader();
        }

        public bool MoveUp()
        {
            int oldSelectedIndex = this.SelectedIndex;
            int oldSelectedLabelIndex = this.SelectedIndex-this._offsetIndex;
            int oldOffsetIndex = this._offsetIndex;

            if (this._offsetIndex < this.SelectedIndex)
            {
                this.SelectedIndex--;
            }
            else if (this._offsetIndex == this.SelectedIndex)
            {
                if (this._offsetIndex > 0)
                {
                    this._offsetIndex--;
                    this.SelectedIndex--;
                }
            }
            else if (this._offsetIndex > this.SelectedIndex)
            {
                this.SelectedIndex = this._offsetIndex;
            }

            if (oldOffsetIndex != this._offsetIndex)
            {
                this.UpdateAllShowedItems();
            }else
            if (oldSelectedIndex !=this.SelectedIndex)
            {
                this._formContent.UpdateItem(this, oldSelectedLabelIndex, this._items[oldSelectedIndex]);
                this._formContent.UpdateItem(this, this.SelectedIndex - this._offsetIndex, this._items[this.SelectedIndex]);
            }

            return (oldOffsetIndex != this._offsetIndex)||(oldSelectedIndex != this.SelectedIndex);
        }

        public bool MoveDown()
        {
            int oldSelectedIndex = this.SelectedIndex;
            int oldSelectedLabelIndex = this.SelectedIndex - this._offsetIndex;
            int oldOffsetIndex = this._offsetIndex;

            if (this._offsetIndex+this._showedItemsCount-1 > this.SelectedIndex)
            {
                this.SelectedIndex++;
            }
            else if (this._offsetIndex + this._showedItemsCount - 1 == this.SelectedIndex)
            {
                if (this._offsetIndex + 1 < this._items.Count - this._showedItemsCount)
                {
                    this._offsetIndex++;
                    this.SelectedIndex++;
                }
            }

            if (oldOffsetIndex != this._offsetIndex)
            {
                this.UpdateAllShowedItems();
            }
            else
            if (oldSelectedIndex != this.SelectedIndex)
            {
                this._formContent.UpdateItem(this, oldSelectedLabelIndex, this._items[oldSelectedIndex]);
                this._formContent.UpdateItem(this, this.SelectedIndex - this._offsetIndex, this._items[this.SelectedIndex]);
            }
            return (oldOffsetIndex != this._offsetIndex) || (oldSelectedIndex != this.SelectedIndex);
        }


        internal void SelectCurrentItem()
        {
            ControlMenuSelectedEventArgs onItemSelectedEventArgs = new ControlMenuSelectedEventArgs();
            onItemSelectedEventArgs.selectedControlMenuItem = this._items[this.SelectedIndex];
            onItemSelectedEventArgs.selectedControlMenuItemIndex = this.SelectedIndex;
            onItemSelectedEventArgs.postAction = ControlMenuSelectedPostAction.None;
            onItemSelectedEventArgs.newHeader = this._headerText;
            onItemSelectedEventArgs.newItems = this._items;
            onItemSelectedEventArgs.newOffsetIndex = this._offsetIndex;
            onItemSelectedEventArgs.newSelectedIndex = this._selectedIndex;
            onItemSelectedEventArgs.changeScreenTransition = ChangeScreenTransition.Slide;

            this.OnItemSelected(this, onItemSelectedEventArgs);

            switch (onItemSelectedEventArgs.postAction)
            {
                case ControlMenuSelectedPostAction.None: break;
                case ControlMenuSelectedPostAction.Close: this.Close(); break;
                case ControlMenuSelectedPostAction.ChangeScreen:
                    this.ChangeScreen(onItemSelectedEventArgs.newHeader, onItemSelectedEventArgs.newItems, onItemSelectedEventArgs.newOffsetIndex, onItemSelectedEventArgs.newSelectedIndex, onItemSelectedEventArgs.changeScreenTransition);
                    break;
            }
        }
    }

    public enum FrameVisibility
    {
        None,
        HeaderOnly,
        Full
    }

    public enum ChangeScreenTransition
    {
        None,
        Slide,
        Fade,
        SlideAndFade
    }
}
