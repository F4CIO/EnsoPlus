using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Extension;
using System.Threading;

namespace EnsoPlus.SelectionListener
{
    internal enum ListeneerGrabType
    {
        Selection,
        Clipboard
    }

    internal class Listener
    {
        #region Private Members
        
        private static Listener _Current;
        internal static Listener Current
        {
            get
            {
                if (_Current == null)
                {
                    _Current = new Listener();
                }
                return _Current;
            }
        }        
        private IEnsoService _service;
        private FormSelectionListener _formSelectionListener;
        private string _lastAddedSelection;

        private bool _blockedByMouseHover;
        private bool _blockedByCheckbox;
        private bool _blockedByPasteOperation;
        private bool _blockedByModifierKey;
        private bool _blockedByMouseWheel;

        private static object _tickSyncObject = new object();
        private static object _pauseContinueSyncObject = new object();
        private string _lastTextOnClipboard;
        #endregion

        #region Properties
        #endregion

        #region Constructors And Initialization
        public Listener()
        { 
        }

        public void Init(IEnsoService service, FormSelectionListener formSelectionListener)
        {            
            this._service = service;
            this._formSelectionListener = formSelectionListener;
            this._lastAddedSelection = null;
            this._blockedByMouseHover = false;
            this._blockedByCheckbox = false;
            this._blockedByPasteOperation = false;
            this._blockedByModifierKey = false;
            this._blockedByMouseWheel = false;
        }
        #endregion

        #region PublicMethods
    
        #endregion

        #region Deinitialization And Destructors
        public void Deinit()
        {
                this._blockedByMouseHover = false;
                this._blockedByCheckbox = false;
                this._blockedByPasteOperation = false;
                this._blockedByModifierKey = false;
                this._blockedByMouseWheel = false;
                this._service = null;
                this._formSelectionListener = null;
        }
        #endregion

        #region Public Methods
        public void BlockByMouseHover()
        {
            lock (_pauseContinueSyncObject)
            {
                if (!this._blockedByMouseHover)
                {
                    this._blockedByMouseHover = true;
                }
            }
        }

        public void UnblockByMouseHover()
        {
            lock (_pauseContinueSyncObject)
            {
                if (this._blockedByMouseHover)
                {
                    this._blockedByMouseHover = false;
                }
            }
        }

        public void BlockByCheckbox()
        {
            lock (_pauseContinueSyncObject)
            {
                if (!this._blockedByCheckbox)
                {
                    this._blockedByCheckbox = true;
                }
            }
        }

        public void UnblockByCheckbox()
        {
            lock (_pauseContinueSyncObject)
            {
                if (this._blockedByCheckbox)
                {
                    this._blockedByCheckbox = false;
                }
            }
        }

        public void BlockByPasteOperation()
        {
            lock (_pauseContinueSyncObject)
            {
                if (!this._blockedByPasteOperation)
                {
                    this._blockedByPasteOperation = true;
                }
            }
        }

        public void UnblockByPasteOperation()
        {
            lock (_pauseContinueSyncObject)
            {
                if (this._blockedByPasteOperation)
                {
                    this._blockedByPasteOperation = false;
                }
            }
        }

        public void BlockByModifierKey()
        {
            lock (_pauseContinueSyncObject)
            {
                if (!this._blockedByModifierKey)
                {
                    this._blockedByModifierKey = true;
                }
            }
        }

        public void UnblockByModifierKey()
        {
            lock (_pauseContinueSyncObject)
            {
                if (this._blockedByModifierKey)
                {
                    this._blockedByModifierKey = false;
                }
            }
        }

        public void BlockByMouseWheel()
        {
            lock (_pauseContinueSyncObject)
            {
                if (!this._blockedByMouseWheel)
                {
                    this._blockedByMouseWheel = true;
                }
            }
        }

        public void UnblockByMouseWheel()
        {
            lock (_pauseContinueSyncObject)
            {
                if (this._blockedByMouseWheel)
                {
                    this._blockedByMouseWheel = false;
                }
            }
        }

        internal void Grab(ListeneerGrabType grabType, bool force)
        {            
            if (!this._blockedByCheckbox && !this._blockedByMouseHover && !this._blockedByPasteOperation)
            {
                if (force)
                {
                    lock (_tickSyncObject)
                    {
                        GrabUnprotected(grabType);
                    }
                }
                else
                {
                    if (Monitor.TryEnter(_tickSyncObject))
                    {
                        try
                        {
                            GrabUnprotected(grabType);
                        }
                        finally
                        {
                            Monitor.Exit(_tickSyncObject);
                        }
                    }
                }
            }

        }

     
        #endregion

        #region Event Handlers
        
        #endregion

        #region Helpers  

        private void GrabUnprotected(ListeneerGrabType grabType)
        {
            //BuildingBlocks.UI.Console.BeepInNewThread(20000, 300);
            string selection = null;
            try
            {
                switch (grabType)
                {
                    case ListeneerGrabType.Selection: selection = this._service.GetUnicodeSelection();
                        break;
                    case ListeneerGrabType.Clipboard:
						string clipboardText = CraftSynth.BuildingBlocks.IO.Clipboard.GetTextFromClipboard();
                        if (clipboardText == this._lastTextOnClipboard)
                        {
                            selection = null;
                        }
                        else
                        {
                            selection = clipboardText;
                            this._lastTextOnClipboard = selection;
                        }
                        break;
                }
            }
            catch
            {
                selection = null;
            }

            if (selection != null)
            {
                FineTuneSelection(ref selection);
                if (IsSelectionGoodEnough(selection))
                {
                    AddSelectionToQueue(selection);
                }
            }
            //BuildingBlocks.UI.Console.BeepInNewThread(10000, 200);
        }

        private bool IsSelectionGoodEnough(string selection)
        {
            bool result = true;
            result = result && !string.IsNullOrEmpty(selection);
            result = result && 
                (this._lastAddedSelection==null ||
                (this._lastAddedSelection!=null && selection != this._lastAddedSelection));
            //result = result && 
            return result;
        }

        private void FineTuneSelection(ref string selection)
        {
            selection = selection.Trim();
        }
        
        private void AddSelectionToQueue(string selection)
        {
            if (this._formSelectionListener != null)
            {
                bool handled = false;

                if (this._lastAddedSelection != null && (selection.StartsWith(this._lastAddedSelection)||selection.EndsWith(this._lastAddedSelection)))
                {
                    handled = this._formSelectionListener.UpdateLastSelectionItem(this._lastAddedSelection, selection);
                }

                if(!handled)
                {
                    this._formSelectionListener.AddSelectionItem(selection);
                }

                this._lastAddedSelection = selection;
            }
        }
        #endregion

      
    }
}
