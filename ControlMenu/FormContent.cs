using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Extension.ControlSimpleMenu
{
    public partial class FormContent : Form
    {
        internal ControlMenu _controlMenu;

        internal bool _editable;

        internal bool _dragActive;
        private int _dragOldX;
        private int _dragOldY;
        internal string _arrowTopText;
        internal string _arrowBottomText;
        internal string _arrowRightText;

        List<Label> _showedItems;

        public Panel PnlCurrent
        {
            get { return this.pnlCurrent; }
            set { this.pnlCurrent = value; }
        }
               
        public FormContent(ControlMenu controlMenu)
        {
            InitializeComponent();
            this._controlMenu = controlMenu;
        }

        private void ChangeState(bool editable)
        {
            this.ChangeState(editable, false);
        }

        private void ChangeState(bool editable, bool force)
        {
            if (force || editable!=this._editable)
            {
                this._editable = editable;
                this.ControlBox = editable;
                this.FormBorderStyle = ((this._editable)?FormBorderStyle.Sizable:FormBorderStyle.None);                
            }
        }

        private void FormContent_Load(object sender, EventArgs e)
        {
            ControlMenu controlMenu = this.Tag as ControlMenu;

            switch (controlMenu._frameVisibility)
            {
                case FrameVisibility.Full: 
                    this.pnlFrame.BackColor = controlMenu._frameColor;
                    this.splHeaderItems.BackColor = controlMenu._frameColor;
                    break;
                case FrameVisibility.HeaderOnly:
                    this.pnlFrame.BackColor = this.TransparencyKey;
                    this.splHeaderItems.BackColor = controlMenu._frameColor;
                    break;
                case FrameVisibility.None:
                    this.pnlFrame.BackColor = this.TransparencyKey;
                    this.splHeaderItems.BackColor = this.TransparencyKey;
                    break;
            }
            this.lblHeader.Visible = !string.IsNullOrEmpty(controlMenu._headerText);
            if(!string.IsNullOrEmpty(controlMenu._headerText))
            {
                lblHeader.Text = controlMenu._headerText;
            }
            this.lblHeader.ForeColor = controlMenu._frameColor;
            this.lblHeader.Font = controlMenu._headerFont;
            this.lblHeader.TextAlign = controlMenu._headerAlign;
            this.lblHeader.Height = controlMenu._itemHeight;
            this.lblArrowTop.Font = controlMenu._itemsFont;            
            this.lblArrowTop.ForeColor = controlMenu._itemsColor;
            this.lblArrowTop.Height = controlMenu._itemHeight;
            this.lblArrowBottom.Font = controlMenu._itemsFont;            
            this.lblArrowBottom.ForeColor = controlMenu._itemsColor;
            this.lblArrowBottom.Height = controlMenu._itemHeight;
            this.lblHeader.Focus();
            this._dragActive = false;
            this.ChangeState(false, true);            
        }

        public void FocusHeader()
        {
            this.lblHeader.Focus();
        }

        private void FormContent_SizeChanged(object sender, EventArgs e)
        {            
            this.Owner.Size = this.Size;
            (this.Tag as ControlMenu)._size = this.Size;
        }

        private void FormContent_LocationChanged(object sender, EventArgs e)
        {
            this.Owner.Location = this.Location;
            (this.Tag as ControlMenu)._location = this.Location;
        }

        private void FormContent_DoubleClick(object sender, EventArgs e)
        {
            this.ChangeState(!_editable);
        }

        private void lblHeader_DoubleClick(object sender, EventArgs e)
        {
            this.ChangeState(!_editable);
        }

        private void lblHeader_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this._dragOldX = e.X;
                this._dragOldY = e.Y;
                this._dragActive = true;
            }
        }

        private void lblHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._dragActive)
            {
                this.Left += e.X - this._dragOldX;
                this.Top += e.Y - this._dragOldY;
            }
        }

        private void lblHeader_MouseUp(object sender, MouseEventArgs e)
        {
            if (this._dragActive && e.Button== MouseButtons.Left)
            {
                this.Left += e.X - this._dragOldX;
                this.Top += e.Y - this._dragOldY;
                this._dragActive = false;
            }
        }

        private void lblHeader_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (this._dragActive && e.KeyCode == Keys.Escape)
            {
                this._dragActive = false;
            }
            else
            {
                switch(e.KeyCode)
                {
                    case Keys.Up:this._controlMenu.MoveUp();break;
                    case Keys.Down:this._controlMenu.MoveDown();break;
                }
            }
            e.IsInputKey = false;
        }

        internal bool TopArrowVisible
        {
            get
            {
                return this.lblArrowTop.Visible;
            }
            set
            {
                this.lblArrowTop.Visible = value;
            }
        }

        internal bool BottomArrowVisible
        {
            get
            {
                return this.lblArrowBottom.Visible;
            }
            set
            {
                this.lblArrowBottom.Visible = value;
            }
        }

        internal string TopArrowText
        {
            get
            {
                return this.lblArrowTop.Text;
            }
            set
            {
                this.lblArrowTop.Text = value;
            }
        }

        internal string BottomArrowText
        {
            get
            {
                return this.lblArrowBottom.Text;
            }
            set
            {
                this.lblArrowBottom.Text = value;
            }
        }

        internal void InitializeItems(ControlMenu controlMenu, int count )
        {
            Label arrowTop = this.lblArrowTop;

            if (this._showedItems != null)
            {
                foreach (Label showedItem in this._showedItems)
                {
                    pnlItems.Controls.Remove(showedItem);
                }
                this._showedItems.Clear();
                this.pnlItems.Controls.Remove((this.lblArrowTop));
            }
            else
            {
                this._showedItems = new List<Label>();
            }


            for (int i = 0; i < count; i++)
            {
                Label showedItem = new Label();
                showedItem.Dock = DockStyle.Top;
                showedItem.TextAlign = controlMenu._itemsAlignment;
                showedItem.Font = controlMenu._itemsFont;
                showedItem.Height = controlMenu._itemHeight;
                this._showedItems.Add(showedItem);
            }

            for (int i = count-1; i >=0; i--)
            {
                this.pnlItems.Controls.Add(this._showedItems[i]);
            }
            this.pnlItems.Controls.Add(arrowTop);
        }

        //public void UpdateAllItems(ControlMenu controlMenu)
        //{
        //    for (int i = 0;i<this._showedItems.Count;i++)
        //    {
        //        this.UpdateItem(controlMenu, i, controlMenu._);
        //    }
        //}

        public void UpdateItem(ControlMenu controlMenu, int index, ControlMenuItem controlMenuItem)
        {
            Label showedItem = this._showedItems[index];

            string prefix = null;
            switch (showedItem.TextAlign)
            {
                case ContentAlignment.MiddleCenter:
                case ContentAlignment.BottomCenter:
                case ContentAlignment.TopCenter:
                    prefix = (controlMenuItem.HaveChildren) ? "    " : string.Empty;
                    break;
                default:
                    prefix = string.Empty;
                    break;
            }
            showedItem.Text = string.Format("{0}{1}{2}",
                prefix,
                controlMenuItem.Caption,
                ((controlMenuItem.HaveChildren) ? this._arrowRightText : string.Empty)
                );

            if (controlMenuItem.Selected)
            {
                showedItem.ForeColor = Color.Silver;
                showedItem.BackColor = controlMenu._itemsColor;
            }
            else
            {
                showedItem.ForeColor = controlMenu._itemsColor;
                showedItem.BackColor = controlMenu._formContent.TransparencyKey;
            }
        }

        private void FormContent_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (this._dragActive && e.KeyCode == Keys.Escape)
            {
                this._dragActive = false;
            }
            else
            {
                switch (e.KeyCode)
                {
                    case Keys.Return:
                    case Keys.Space: this._controlMenu.SelectCurrentItem(); break;
                    case Keys.Up: this._controlMenu.MoveUp(); break;
                    case Keys.Down: this._controlMenu.MoveDown(); break;
                }
            }
            e.IsInputKey = false;
        }

        private void FormContent_FormClosed(object sender, FormClosedEventArgs e)
        {
        
        }
    }
}
