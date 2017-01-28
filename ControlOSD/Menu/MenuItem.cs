using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace OSD.Menu
{
    public class MenuItem
    {
        public string text;
        public bool selected;
        public bool haveChildren;
        public object value;
        public object tag;

        public MenuItem()
            : this("Untitled menu")
        {
        }

        public MenuItem(string text)
            : this(text, false)
        {
        }

        public MenuItem(string text, object value)
            : this(text, false, value)
        {
        }

        public MenuItem(string text, bool haveChildren)
            : this(text, haveChildren, false, null)
        {
        }

        public MenuItem(string text, bool haveChildren, object value)
            : this(text, haveChildren, false, value)
        {
        }

        public MenuItem(string text, bool haveChildren, bool selected, object value)
        {
            this.text = text;
            this.selected = selected;
            this.haveChildren = haveChildren;
            this.value = value;
        }

        internal static MenuItem CreateArrowTop(bool enabled)
        {
            return new MenuItem( (enabled)? Settings.Current.menuArrowTopText : string.Empty, false, false);
        }

        internal static MenuItem CreateArrowBottom(bool enabled)
        {
            return new MenuItem( (enabled)? Settings.Current.menuArrowBottomText : string.Empty, false, false);
        }

        internal void Draw(Graphics g, Rectangle area)
        {
            string text = this.haveChildren ? this.text + " " + Settings.Current.menuArrowRightText : this.text;

            Color backgroundColor = this.selected ? Settings.Current.selectedTextBackColor:Settings.Current.normalTextBackColor;
            Brush backgroundBrush = new SolidBrush( backgroundColor);
            g.FillRectangle(backgroundBrush, area);
            
            Color color = this.selected ? Settings.Current.selectedTextForeColor:Settings.Current.normalTextForeColor;
            Brush brush = new SolidBrush( color);
            Point textLocation = Helper.GetTextLocation(g, text, Settings.Current.normalFont.font, area.Location, area.Size, Settings.Current.menuItemsAlignment, Settings.Current.margins);
            g.DrawString(text, Settings.Current.normalFont.font, brush, textLocation);
            
            //if (this.haveChildren)
            //{
            //    Size textSize = Helper.GetTextSize(g, this.text, Settings.Current.normalFont);
            //    Point a = new Point(textLocation.X
            //    g.FillPolygon(
            //}
        }
    }
}
