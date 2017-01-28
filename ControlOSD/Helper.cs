using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace OSD
{
    public class Helper
    {
        public static int GetMiddle(int a, int b)
        {
            long r=0;
            return (int)Math.DivRem(Math.Abs(a + b), 2, out r);
        }

        public static int GetCenteredCoordinate(int parentSize, int childSize)
        {
            int center = GetMiddle(0, parentSize);
            return center - GetMiddle(0, childSize);
        }

        public static Size GetTextSize(Graphics graphics, string text, Font font)
        {
            return graphics.MeasureString(text, font).ToSize();
        }

        public static Point GetTextLocation(Graphics graphics, string text, Font font, Point parentLocation, Size parentSize, ContentAlignment alignment, int margins)
        {
           
            int x = 0;
            int y = 0;
            Size textSize = GetTextSize(graphics, text, font);
            switch (alignment)
            {
                case ContentAlignment.TopLeft:
                case ContentAlignment.BottomLeft:
                case ContentAlignment.MiddleLeft:
                    x = parentLocation.X + margins;
                    y = parentLocation.Y + GetCenteredCoordinate(parentSize.Height, textSize.Height);
                    break;
                case ContentAlignment.TopCenter:
                case ContentAlignment.BottomCenter:
                case ContentAlignment.MiddleCenter:                    
                    x = parentLocation.X + GetCenteredCoordinate(parentSize.Width, textSize.Width);                    
                    y = parentLocation.Y + GetCenteredCoordinate(parentSize.Height, textSize.Height);
                    break;
                case ContentAlignment.TopRight:
                case ContentAlignment.BottomRight:
                case ContentAlignment.MiddleRight:
                    x = parentLocation.X + parentSize.Width - margins - textSize.Width;
                    y = parentLocation.Y + GetCenteredCoordinate(parentSize.Height, textSize.Height);
                    break;
            }

            Point textLocation = new Point(x,y);
            return textLocation;
        }

    }
}
