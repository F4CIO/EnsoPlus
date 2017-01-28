using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Common;

namespace Extension
{
    public partial class FormParameterBox : Form
    {
        public FormParameterBox()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Color backColor = (ParameterInput.suggestionsSelectionIndex == -1) ? ParameterInput.itemBackgroundSelected : ParameterInput.itemBackground;
            this.tbParameter.BackColor = backColor;
            Rectangle panelArea = new Rectangle(0, 0, parameterBoxRectangle.Width - 2, parameterBoxRectangle.Height - 2);
            e.Graphics.FillRectangle(new SolidBrush(backColor), panelArea);
            e.Graphics.DrawRectangle(new Pen(Color.Orange, 6), panelArea);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!ParameterInput._readOnly)
            {
                ParameterInput.EnteredTextChanged();
            }
        }

        private void tbParameter_KeyDown(object sender, KeyEventArgs e)
		{
            if (e.KeyCode == Keys.Up)
            {
                ParameterInput.MoveUpSuggesstionSelection();
                e.SuppressKeyPress = true;
            }
            else if(e.KeyCode==Keys.Down)
            {
                ParameterInput.MoveDownSuggesstionSelection();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                ParameterInput.Accept();
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                ParameterInput.Cancel();
                e.SuppressKeyPress = true;
            }
            else
            {
                e.SuppressKeyPress = ParameterInput._readOnly;
            }
        }

        private void FormParameterBox_VisibilityChanged(object sender, EventArgs e)
        {
	        if (this.Visible)
	        {
		        if (ParameterInput._readOnly)
		        {
			        ParameterInput.EnteredTextChanged();
		        }

		        Focus();

		        this._tickCount = 0;
		        this.timer1.Enabled = true;
	        }
        } 

	    private void lblCaption_Resize(object sender, EventArgs e)
        {
            this.lblCaption.Left = this.parameterBoxRectangle.Left - 10 - this.lblCaption.Size.Width;
        }

	    private int _tickCount;

		private void timer1_Tick(object sender, EventArgs e)
		{
			Focus();

			this._tickCount++;
			if (this.timer1.Interval*this._tickCount >= 3000)//turn off focusing after 3 seconds
			{
				this.timer1.Enabled = false;
			}

			//CraftSynth.BuildingBlocks.Common.Misc.BeepInNewThread(5000,500);
		}
		
		private void Focus()
	    {
			if (this.Visible)
			{
				CraftSynth.BuildingBlocks.WindowsNT.Misc.SetForegroundWindow("ParameterBox");

				((Control) this).Focus();
				this.tbParameter.Focus();
			}
	    }
    }
}
