using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace LnkFileReader
{
    /// <summary>
    /// Periodicaly checks if any of targetwindows exists
    /// and if none exists stops listener and exits application.
    /// </summary>
    internal class Murderer
    {
        #region Private Members
        private const int _interval = 5000;
        private System.Threading.Timer _timer;
        string _targetWindowsCaption;
        #endregion

        #region Properties
        #endregion

        #region Public Methods

        public void Postpone()
        {
            this._timer.Change(_interval, _interval);
        }

        public static void Kill()
        {
            Thread.Sleep(2000);
            Application.Exit();
        }
        #endregion

        #region Constructors And Initialization
        public Murderer(string targetWindowsCaption)
        {            
            this._targetWindowsCaption = targetWindowsCaption;
            this._timer = new System.Threading.Timer(OnKill, null, _interval, _interval);
        }
        #endregion

        #region Deinitialization And Destructors
        #endregion

        #region Event Handlers
        private void OnKill(object state)
        {
			if (!CraftSynth.BuildingBlocks.WindowsNT.Misc.IsWindowOpen(_targetWindowsCaption))
            {
                Kill();
            }
        }

       
        #endregion

        #region Helpers
        #endregion
    }
}
