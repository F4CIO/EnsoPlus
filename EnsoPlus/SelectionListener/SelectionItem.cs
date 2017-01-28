using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace EnsoPlus.SelectionListener
{
    public partial class SelectionItem : UserControl
    {
        #region Private Members

        private ToolTip _tooltip;

        /// <summary>
        /// Used to track the source control from which data is being dragged.
        /// This allows the DragEnter and DragDrop event handlers to determine
        /// which control on the form is the originator of the DragDrop operation.
        /// </summary>
        private Control sourceControl;

        /// <summary>
        /// Used to track the mouse button that initiated the DragDrop operation.
        /// The mouse button is recorded in the MouseDown event of the source controls
        /// and is then accessible to the DragEnter and DragDrop event handlers of the 
        /// target control.
        /// </summary>
        private MouseButtons mouseButton;       
        
        #endregion

        #region Properties
        private FormSelectionListener _parentForm;
        public FormSelectionListener ParentForm
        {
            get
            {
                return _parentForm;
            }
            set
            {
                _parentForm = value;
            }
        }

        private string _content;
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
                this.tbContent.Text = _content;
            }
        }
        #endregion

        #region Constructors And Initialization
        public SelectionItem(FormSelectionListener parentForm, string content)
        {
            this._parentForm = parentForm;
            this._content = content;
            InitializeComponent();

            this.cbLock.BackgroundImage = global::EnsoPlus.Properties.Resources.Unlocked;

            this.tbContent.Text = this._content;
            this.tbContent.BackColor = Settings.Current.backgroundColor;
            this.tbContent.ForeColor = Settings.Current.normalTextForeColor;
            this.tbContent.Font = Settings.Current.normalFont.font;

            UpdateTooltip();

            this.tbContent.MouseHover += new EventHandler(SelectionItem_MouseHover);
            this.MouseHover += new EventHandler(SelectionItem_MouseHover);
            this.btnCopy.MouseHover += new EventHandler(SelectionItem_MouseHover);
            this.btnRemove.MouseHover += new EventHandler(SelectionItem_MouseHover);
            this.btnView.MouseHover += new EventHandler(SelectionItem_MouseHover);
            this.cbLock.MouseHover += new EventHandler(SelectionItem_MouseHover);

            this.cbLock.CheckedChanged += new System.EventHandler(this.cbLock_CheckedChanged);
            this.tbContent.TextChanged += new System.EventHandler(this.tbContent_TextChanged);

            this.tbContent.MouseDown += new MouseEventHandler(this.tbContent_MouseDown);
            this.tbContent.DoubleClick += new EventHandler(tbContent_DoubleClick);
            this.tbContent.MouseMove += new MouseEventHandler(tbContent_MouseMove);
        }

        #endregion

        #region Deinitialization And Destructors
        ~SelectionItem()
        {

            this.tbContent.MouseHover -= new EventHandler(SelectionItem_MouseHover);
            this.MouseHover -= new EventHandler(SelectionItem_MouseHover);
            this.btnCopy.MouseHover -= new EventHandler(SelectionItem_MouseHover);
            this.btnRemove.MouseHover -= new EventHandler(SelectionItem_MouseHover);
            this.btnView.MouseHover -= new EventHandler(SelectionItem_MouseHover);
            this.cbLock.MouseHover -= new EventHandler(SelectionItem_MouseHover);

            this.cbLock.CheckedChanged -= new System.EventHandler(this.cbLock_CheckedChanged);
            this.tbContent.TextChanged -= new System.EventHandler(this.tbContent_TextChanged);

            this.tbContent.MouseDown += new MouseEventHandler(this.tbContent_MouseDown);
            this.tbContent.DoubleClick -= new EventHandler(tbContent_DoubleClick);
            this.tbContent.MouseMove -= new MouseEventHandler(tbContent_MouseMove);            
            this._tooltip = null;
            this._content = null;
            this._parentForm = null;
        }
        #endregion

        #region Event Handlers
        void SelectionItem_MouseHover(object sender, EventArgs e)
        {
            this.ParentForm.SetPickedSelectionItem(this);
        }

        void tbContent_DoubleClick(object sender, EventArgs e)
        {
            SelectionListener.Current.Paste(this._content, null, true);          
        }

        private void cbLock_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbLock.Checked)
            {
                this._parentForm.LockSelectionItem(this);
            }
            else
            {
                this._parentForm.UnlockSelectionItem(this);
            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            CraftSynth.BuildingBlocks.WindowsNT.Misc.OpenTextInNotepad(this._content);
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
			CraftSynth.BuildingBlocks.IO.Clipboard.SetTextToClipboard(this._content);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            this._parentForm.RemoveSelectionItem(this);
        }

        private void tbContent_TextChanged(object sender, EventArgs e)
        {
            this._content = this.tbContent.Text;
            UpdateTooltip();
        }

        private void tbContent_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this._parentForm.LeftMouseButtonDownLocation = e.Location;
                this._parentForm.LeftMouseButtonIsDown = true;
            }
        }

        void tbContent_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._parentForm.LeftMouseButtonIsDown &&
                (e.Location.X - this._parentForm.LeftMouseButtonDownLocation.X > 5 ||
                 e.Location.X - this._parentForm.LeftMouseButtonDownLocation.X < -5 ||
                 e.Location.Y - this._parentForm.LeftMouseButtonDownLocation.Y > 5 ||
                 e.Location.Y - this._parentForm.LeftMouseButtonDownLocation.Y < -5
                 )
                )
            {
                this.tbContent.DeselectAll();

                // Sets a reference to the control that initiated the DragDrop operation, so 
                // that the target control can implement logic to handle data dragged from
                // specific controls differently.  This control reference is also used in the 
                // case of a Move effect to remove the data from the source control after a drop.
                sourceControl = this.tbContent;
                // Record the mouse button that initiated this operation in order to allow
                // target controls to respond differently to drags with the right or left
                // mouse buttons.
                mouseButton = e.Button;
                // This initiates a DragDrop operation, specifying that the data to be dragged
                // with be the text stored in the textBox1 control.  This also specifies
                // that both Copy and Move effects will be allowed.
                this.tbContent.DoDragDrop(this._content, DragDropEffects.Move | DragDropEffects.Copy);
            }
        }

       
        #endregion

        #region Helpers
        private void UpdateTooltip()
        {
            if (this._tooltip == null)
            {
                this._tooltip = new ToolTip();
                this._tooltip.ShowAlways = true;
                this._tooltip.UseFading = false;
                this._tooltip.UseAnimation = false;
                this._tooltip.AutoPopDelay = 100000;
                this._tooltip.ReshowDelay = 0;
                this._tooltip.InitialDelay = 0;
            }
            this._tooltip.SetToolTip(this.tbContent, this._content);
        }

        #endregion
    }
}
