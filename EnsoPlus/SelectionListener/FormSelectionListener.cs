using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace EnsoPlus.SelectionListener
{
    public partial class FormSelectionListener : Form
    {
        #region Private Members
        public Thread _thread;
        private string _userActivityBroadcasterFilePath
        {
            get
            {
                return Path.Combine(CraftSynth.BuildingBlocks.UI.Console.ApplicationPhysicalPath, "EnsoPlus.UserActivityBroadcaster.exe");
            }
        }
        SelectionItem _fixedSelectionItem;
        #endregion

        #region Properties
        private static FormSelectionListener _current;
        internal static FormSelectionListener Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new FormSelectionListener();
                }
                return _current;
            }
        }

        internal bool LeftMouseButtonIsDown
        {
            get;
            set;
        }
        internal Point LeftMouseButtonDownLocation
        {
            get;
            set;
        }
        #endregion

        #region Public Methods 
        internal void TriggerConstruction()
        {
        }

        internal void AddSelectionItem(string selection)
        {   
            FormSelectionListener.Current.flpItems.Invoke((Action)(()=>{                
                SelectionItem newSelectionItem = null;
                
                //search old items
                int i = this.flpItems.Controls.Count-1;
                while (i >= 0)
                {
                    SelectionItem currentSelectionItem  = (SelectionItem)this.flpItems.Controls[i];
                    if (string.Compare(selection, currentSelectionItem.Content, StringComparison.Ordinal) == 0)
                    {
                        newSelectionItem = currentSelectionItem;
                        break;
                    }
                    i--;
                }
                
                if (newSelectionItem==null)                
                {
                    if (this.flpItems.Controls.Count >= Settings.Current.queueLength)
                    {
                        this.flpItems.Controls.RemoveAt(0);
                    }
                    newSelectionItem = new SelectionItem(FormSelectionListener.Current, selection);
                    //newSelectionItem.Width = this.GetItemWidth();
                    this.flpItems.Controls.Add(newSelectionItem);
                    UpdateAllItemsWidths();
                }

                if (!Focuser.Current.IsFocused)
                {
                    this.flpItems.ScrollControlIntoView(newSelectionItem);
                }
                this.SetCurrentSelectionItem(newSelectionItem);
            }));
            
            Application.DoEvents();
        }

        internal bool UpdateLastSelectionItem(string lastAddedSelection, string selection)
        {
            bool updated = false;
            FormSelectionListener.Current.flpItems.Invoke((Action)(() =>
            {
                if (this.flpItems.Controls.Count > 0)
                {
                    SelectionItem lastSelectionItem = (SelectionItem)this.flpItems.Controls[this.flpItems.Controls.Count - 1];
                    if (lastSelectionItem.Content == lastAddedSelection)
                    {
                        lastSelectionItem.Content = selection;
                        updated = true;
                    }
                }
            }));
            return updated;
        }

        internal void LockSelectionItem(SelectionItem item)
        {
            this.flpItems.Controls.Remove(item);
            //item.Width = this.GetLockedItemWidth();
            item.cbLock.BackgroundImage = global::EnsoPlus.Properties.Resources.Locked;
            this.flpLockedItems.Controls.Add(item);
            this.flpLockedItems.ScrollControlIntoView(item);
            UpdateAllLockedItemsWidths();
            UpdateAllItemsWidths();
        }

        internal void UnlockSelectionItem(SelectionItem item)
        {
            this.flpLockedItems.Controls.Remove(item);
            //item.Width = this.GetItemWidth();
            item.cbLock.BackgroundImage = global::EnsoPlus.Properties.Resources.Unlocked;
            this.flpItems.Controls.Add(item);
            this.flpItems.ScrollControlIntoView(item);
            UpdateAllLockedItemsWidths();
            UpdateAllItemsWidths();
        }

        internal void RemoveSelectionItem(SelectionItem item)
        {
            if (item.cbLock.Checked)
            {
                this.flpLockedItems.Controls.Remove(item);
                UpdateAllLockedItemsWidths();
            }
            else
            {
                this.flpItems.Controls.Remove(item);
                this.UpdateAllItemsWidths();
            }
        }

        internal new void Hide()
        {
            this.flpItems.Invoke((Action)(() =>
            {
                base.Hide();
            }));
        }

        internal int GetLockedItemWidth()
        {
            int scrollBarWidth = (this.flpLockedItems.Controls.Count * 26 > this.flpLockedItems.Height) ? 18 : 0;
            return this.flpLockedItems.Width - 2 - scrollBarWidth;
        }

        internal int GetItemWidth()
        {
            int scrollBarWidth = (this.flpItems.Controls.Count * 26 > this.flpItems.Height) ? 18 : 0;
            return this.flpItems.Width - 2 - scrollBarWidth;
        }

        internal void SetCbListen(bool value)
        {
            FormSelectionListener.Current.flpItems.Invoke((Action)(() =>
            {
                this.cbListen.Checked = value;
            }));
        }

        private SelectionItem _currentSelectionItem;
        internal void SetCurrentSelectionItem(SelectionItem selectionItem)
        {
            if (this._currentSelectionItem != null)
            {
                if (this._currentSelectionItem == this._pickedSelectionItem)
                {
                    this._currentSelectionItem.BackColor = Color.Green;
                }
                else
                {
                    this._currentSelectionItem.BackColor = SystemColors.ButtonFace;
                }
            }
            this._currentSelectionItem = selectionItem;
            if (this._currentSelectionItem != this._pickedSelectionItem)
            {
                this._currentSelectionItem.BackColor = Color.DarkBlue;
            }
        }

        private SelectionItem _pickedSelectionItem;
        internal void SetPickedSelectionItem(SelectionItem selectionItem)
        {
            if (this._pickedSelectionItem != null)
            {
                if (this._pickedSelectionItem == this._currentSelectionItem)
                {
                    this._currentSelectionItem.BackColor = Color.DarkBlue;
                }
                else if(this._pickedSelectionItem!=this._fixedSelectionItem)
                {
                    this._pickedSelectionItem.BackColor = SystemColors.ButtonFace;
                }
            }
            this._pickedSelectionItem = selectionItem;
            this._pickedSelectionItem.BackColor = Color.Green;

            this._fixedSelectionItem.Content = this._pickedSelectionItem.Content;
        }

        #endregion

        #region Constructors And Initialization 
        public FormSelectionListener()
        {
            InitializeComponent();

            this.Location = Settings.Current.location;            
            this.Size = Settings.Current.size;
            this.scItems.SplitterDistance = Settings.Current.splitterPosition;
            this.cbListen.Checked = Settings.Current.active;

            this.StartUserActivityBroadcaster();

            this.cbListen.CheckedChanged += new System.EventHandler(this.cbListen_CheckedChanged);
            this.btnClearLockedSelectionItems.Click += new System.EventHandler(this.btnClearLockedSelectionItems_Click);
            this.btnClearSelectionItems.Click += new System.EventHandler(this.btnClearSelectionItems_Click);
            this.Move += new EventHandler(FormSelectionListener_Move);
            this.Resize += new EventHandler(FormSelectionListener_Resize);
            this.scItems.SplitterMoved += new SplitterEventHandler(scItems_SplitterMoved);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormSelectionListener_FormClosed);

            this._fixedSelectionItem = new SelectionItem(this, "(Nothing hovered so far)");
            this.flpPickedSelectionItem.Controls.Add(this._fixedSelectionItem);
            this._fixedSelectionItem.BackColor = Color.Green;
            this.UpdateFixedItemWidth();

            Focuser.Current.Start();
        }
        #endregion

        #region Deinitialization And Destructors

        public void Deinit()
        {
            Focuser.Current.Stop();

            Settings.Current.active = this.cbListen.Checked;
            Settings.Save();
            Listener.Current.Deinit();

            this.StopUserActivityBroadcaster();

            this.cbListen.CheckedChanged -= new System.EventHandler(this.cbListen_CheckedChanged);
            this.btnClearLockedSelectionItems.Click -= new System.EventHandler(this.btnClearLockedSelectionItems_Click);
            this.btnClearSelectionItems.Click -= new System.EventHandler(this.btnClearSelectionItems_Click);
            this.Move -= new EventHandler(FormSelectionListener_Move);
            this.Resize -= new EventHandler(FormSelectionListener_Resize);
            this.scItems.SplitterMoved -= new SplitterEventHandler(scItems_SplitterMoved);
            this.FormClosed -= new System.Windows.Forms.FormClosedEventHandler(this.FormSelectionListener_FormClosed);
            
            this.Dispose(true);
            FormSelectionListener._current = null;

            SelectionListener.Started = false;

            Thread aborterThread = new Thread(AbortThread);
            aborterThread.Start();         
        }
        #endregion

        #region Event Handlers


        private void cbListen_CheckedChanged(object sender, EventArgs e)
        {
            if (this.cbListen.Checked)
            {                
                Listener.Current.UnblockByCheckbox();
            }
            else
            {
                Listener.Current.BlockByCheckbox();
            }
        }

        private void btnClearLockedSelectionItems_Click(object sender, EventArgs e)
        {
            while (this.flpLockedItems.Controls.Count > 0)
            {
                this.RemoveSelectionItem((SelectionItem)this.flpLockedItems.Controls[0]);
            }
        }

        private void btnClearSelectionItems_Click(object sender, EventArgs e)
        {
            while (this.flpItems.Controls.Count > 0)
            {
                this.RemoveSelectionItem((SelectionItem)this.flpItems.Controls[0]);
            }
        }

        void FormSelectionListener_Move(object sender, EventArgs e)
        {
            Settings.Current.location = new Point(this.Left, this.Top);
            Settings.Save();
        }

        void FormSelectionListener_Resize(object sender, EventArgs e)
        {
            UpdateAllLockedItemsWidths();
            UpdateAllItemsWidths();
            UpdateFixedItemWidth();

            Settings.Current.size = this.Size;
            Settings.Save();
        }

        void scItems_SplitterMoved(object sender, SplitterEventArgs e)
        {
            UpdateAllLockedItemsWidths();
            UpdateAllItemsWidths();

            Settings.Current.splitterPosition = this.scItems.SplitterDistance;
            Settings.Save();
        }


        protected override void WndProc(ref Message m)
        {
			if (CraftSynth.BuildingBlocks.WindowsNT.WindowsMessage.GetCustomMessage(m.Msg) == 1)
            {                
                 Listener.Current.Grab( ListeneerGrabType.Selection, true);
            }else
				if (CraftSynth.BuildingBlocks.WindowsNT.WindowsMessage.GetCustomMessage(m.Msg) == 2)
            {
                this.LeftMouseButtonIsDown = false;
            }
            else
                if (CraftSynth.BuildingBlocks.WindowsNT.WindowsMessage.GetCustomMessage(m.Msg) == 3)
                {
                    if (this._pickedSelectionItem == null)
                    {
                        SelectionListener.Current.DisplayMessage("Please hover with mouse on some item from list first.");
                    }
                    else
                    {
                        SelectionListener.Current.Paste(this._pickedSelectionItem.Content, null,false);
                    }
                }
            base.WndProc(ref m);
        }


        private void FormSelectionListener_FormClosed(object sender, FormClosedEventArgs e)
        {
            Deinit();            
        }

        #endregion

        #region Helpers
        private void UpdateSelectionItemColor(SelectionItem selectionItem)
        {
            if (selectionItem == this._pickedSelectionItem)
            {

            }
            else if (selectionItem == this._currentSelectionItem)
            {
            }
            else
            {
            }
        }

        private void UpdateAllLockedItemsWidths()
        {
            int lockedItemWidth = this.GetLockedItemWidth();
            foreach (SelectionItem item in this.flpLockedItems.Controls)
            {
                item.Width = lockedItemWidth;
            }
        }

        private void UpdateAllItemsWidths()
        {
             int itemWidth = GetItemWidth();
            foreach (SelectionItem item in this.flpItems.Controls)
            {
                item.Width = itemWidth;
            }
        }

        private void UpdateFixedItemWidth()
        {
            this._fixedSelectionItem.Width = this.flpPickedSelectionItem.Width - 2;
        }

        private void StartUserActivityBroadcaster()
        {
            StopUserActivityBroadcaster();
            CraftSynth.BuildingBlocks.WindowsNT.Misc.OpenFile(this._userActivityBroadcasterFilePath, "-PasteKeyCombination:"+Settings.Current.pasteKeyCombination);      
        }

        private void StopUserActivityBroadcaster()
        {
            CraftSynth.BuildingBlocks.WindowsNT.Misc.OpenFile(this._userActivityBroadcasterFilePath, "-kill");
            int i = 0;
            do
            {
                i++;
                Thread.Sleep(500);
            } while (i < 10 && Process.GetProcessesByName(Path.GetFileNameWithoutExtension(this._userActivityBroadcasterFilePath)).Length > 0);            
        }

        private void AbortThread()
        {
            Thread.Sleep(2000);
            this._thread.Abort();
        }
        #endregion








      
    }
}
