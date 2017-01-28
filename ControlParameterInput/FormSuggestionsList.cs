using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Extension
{
    public partial class FormSuggestionsList : Form
    {
        public FormSuggestionsList()
        {
            InitializeComponent();
        }

        public Font suggestionFont
        {
            get
            {
                return this.lblSuggestionTemplate.Font;
            }
        }

        public int suggestionHeight
        {
            get
            {
                return this.lblSuggestionTemplate.Height;
            }
        }

        public void DrawAll(List<string> suggestionsToDisplay, int offset, string stringToHighlight, int selectedIndex)
        {
            this.Height = suggestionsToDisplay.Count * (suggestionHeight);

            //clear suggestions
            for (int i2=this.Controls.Count-1;i2>=0;i2--)
            {

                Control c = this.Controls[i2];
                if (c.Name != "lblSuggestionTemplate")
                {
                    this.Controls.Remove(c);
                }
            }

            //add suggestions
            int i = 0;
            foreach (string suggestionToDisplay in suggestionsToDisplay)
            {

                DrawSuggestion(i, suggestionToDisplay, stringToHighlight, selectedIndex==i+offset);
                i++;
            }
        }        

        private void DrawSuggestion(int order,string suggestionText, string stringToHighlight, bool selected)
        {
            int y = order * (suggestionHeight);
            
            //slice
            suggestionText = suggestionText.Trim();
            string[] nonhighlightedStringsArray = suggestionText.Split(new string[]{stringToHighlight}, StringSplitOptions.None);
            List<string> nonhighlightedStrings = new List<string>(nonhighlightedStringsArray);
            if (nonhighlightedStrings.Count > 0 && nonhighlightedStrings[0] == string.Empty) nonhighlightedStrings.RemoveAt(0);                
            if (nonhighlightedStrings.Count > 0 && nonhighlightedStrings[nonhighlightedStrings.Count-1] == string.Empty) nonhighlightedStrings.RemoveAt(nonhighlightedStrings.Count-1);                
            

            //insert stringToHightlight in between
            List<string> suggestionTextParts = new List<string>();
            if(suggestionText.StartsWith(stringToHighlight))suggestionTextParts.Add(stringToHighlight);

            foreach (string nonhighlightedString in nonhighlightedStrings)
            {
                suggestionTextParts.Add(nonhighlightedString);
                suggestionTextParts.Add(stringToHighlight);
            }
            if (suggestionTextParts.Count>0 && !suggestionText.EndsWith(stringToHighlight)) suggestionTextParts.RemoveAt(suggestionTextParts.Count - 1);

            float x=0;
            Graphics graphics = this.CreateGraphics();
            Brush brush = new SolidBrush((selected)?ParameterInput.itemBackgroundSelected:ParameterInput.itemBackground);
            graphics.FillRectangle(brush, 0, y, this.Width, suggestionHeight);
            foreach (string suggestionTextPart in suggestionTextParts)
            {

                Color forecolor = (suggestionTextPart == stringToHighlight) ? ParameterInput.itemForegroundHighlighted : ParameterInput.itemForeground;    
                Pen pen = new Pen(forecolor);
                brush = new SolidBrush(forecolor);
                graphics.DrawString(suggestionTextPart, suggestionFont, brush, x, y);
                x = x + (TextRenderer.MeasureText(suggestionTextPart, suggestionFont).Width)-12;
                if (x < 0) x = 0;

            }
            graphics.Flush();

        }
    }
}
