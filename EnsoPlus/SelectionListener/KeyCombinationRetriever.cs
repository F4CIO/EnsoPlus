using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CraftSynth.BuildingBlocks.UI.WindowsForms;

namespace EnsoPlus.SelectionListener
{
    public partial class KeyCombinationRetriever : Form
    {
        #region Private Members
       

        private static KeyCombinationRetriever _current;
		private CraftSynth.BuildingBlocks.WindowsNT.UserActivityHookNamespace.UserActivityHook _userActivityHook;

        private List<Keys> _pressedModifierKeys;

        private List<Keys> _lastKeyCombination;
        private Keys _lastPressedKey;

        #endregion

        #region Properties
        #endregion

        #region Public Methods
        public static List<Keys> Execute(string infoMessage)
        {            
            List<Keys> result = null;
            _current = new KeyCombinationRetriever();
            if (infoMessage != null)
            {
                _current.Text = infoMessage;
            }

            DialogResult dialogResult;
            try
            {
				_current._userActivityHook = new CraftSynth.BuildingBlocks.WindowsNT.UserActivityHookNamespace.UserActivityHook(false, true);

                _current._pressedModifierKeys = new List<Keys>();
                _current._lastKeyCombination = new List<Keys>();
                _current._lastPressedKey = Keys.None;
                _current._userActivityHook.KeyDown += new KeyEventHandler(_current.userActivityHook_KeyDown);
                _current._userActivityHook.KeyUp += new KeyEventHandler(_current._userActivityHook_KeyUp);
                _current.Shown += new EventHandler(_current.OnShown);
                dialogResult = _current.ShowDialog();                
            }
            finally
            {
                _current._userActivityHook.Stop(false, true, true);
            }

            if (dialogResult == DialogResult.OK)
            {
                result = _current._lastKeyCombination;
            }

            return result;
        }

       
        #endregion

        #region Constructors And Initialization
        public KeyCombinationRetriever()
        {
            InitializeComponent();
        }
        #endregion

     
        #region Deinitialization And Destructors
        #endregion

        #region Event Handlers
        private void OnShown(object sender, EventArgs e)
        {
            CraftSynth.BuildingBlocks.WindowsNT.Misc.SetForegroundWindow(this.Handle);
        }

        private void tbKeyCombination_KeyDown(object sender, KeyEventArgs e)
        {
            this.tbKeyCombination.Text = e.KeyValue.ToString();
            e.Handled = true;
        }      

        private void userActivityHook_KeyDown(object sender, KeyEventArgs e)
        {
            string pressedKeyCombinationString = string.Empty;
            foreach (Keys modifierKey in this._pressedModifierKeys)
            {
                if (pressedKeyCombinationString != string.Empty)
                {
                    pressedKeyCombinationString = pressedKeyCombinationString + " + ";
                }
                pressedKeyCombinationString = pressedKeyCombinationString + Enum.GetName(typeof(Keys), modifierKey);
            }

            if (KeysHelper.ModifierKeysList.Contains(e.KeyCode))
            {
                if (!this._pressedModifierKeys.Contains(e.KeyCode))
                {
                    this._pressedModifierKeys.Add(e.KeyCode);

                    if (pressedKeyCombinationString != string.Empty)
                    {
                        pressedKeyCombinationString = pressedKeyCombinationString + " + ";
                    }
                    pressedKeyCombinationString = pressedKeyCombinationString + Enum.GetName(typeof(Keys), e.KeyCode);
                }
            }
            else
            {
                this._lastPressedKey = e.KeyCode;

                if (pressedKeyCombinationString != string.Empty)
                {
                    pressedKeyCombinationString = pressedKeyCombinationString + " + ";
                }
                pressedKeyCombinationString = pressedKeyCombinationString + Enum.GetName(typeof(Keys), e.KeyCode);
            }

            this._lastKeyCombination.Clear();
            this._lastKeyCombination.AddRange(this._pressedModifierKeys);
            if (this._lastPressedKey != Keys.None)
            {
                this._lastKeyCombination.Add(_lastPressedKey);
            }

            this.tbKeyCombination.Text = pressedKeyCombinationString;
            e.Handled = true;
        }

        private void _userActivityHook_KeyUp(object sender, KeyEventArgs e)
        {
            if (this._pressedModifierKeys.Contains(e.KeyCode))
            {
                this._pressedModifierKeys.Remove(e.KeyCode);
            }
        }

        #endregion

        #region Helpers
        #endregion
    }
}
