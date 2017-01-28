using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace EnsoPlus.SelectionListener
{
    internal class ClipboardListener
    { 
        #region Private Members
        private Listener _listener;
        private Timer _timer;

        private static object _tickSyncObject = new object();
        private string lastTextOnClipboard;
        #endregion

        #region Properties
        #endregion

        #region Constructors And Initialization
        public ClipboardListener(Listener listener)
        {
            this._listener = listener;
            this._timer = new Timer(Timer_Tick,null, Timeout.Infinite, Timeout.Infinite);
        }
        #endregion

        #region Deinitialization And Destructors
        ~ClipboardListener()
        {
            this.Stop();
            this._timer = null;
            this._listener = null;
        }
        #endregion

        #region PublicMethods
        public void Start()
        {
            this._timer.Change( Settings.Current.interval, Settings.Current.interval);
        }

        public void Stop()
        {
            this._timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
        #endregion

        #region Event Handlers
        private void Timer_Tick(object state)
        {
            this._listener.Grab(ListeneerGrabType.Clipboard, false);
        }        
        #endregion

        #region Helpers  
        #endregion
    }
}
