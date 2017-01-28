using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace EnsoPlus.SelectionListener
{
    
    public class Focuser
    {
        #region Private Members     
        private bool _started;
        private static object _syncObject = new object();
        private System.Threading.Timer _timer;
        private const int _interval = 750;
        private const string _processName = "EnsoPlus";
        private IntPtr _lastForegroundWindow;

        #endregion

        #region Properties
        private static Focuser _current;
        internal static Focuser Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new Focuser();
                }
                return _current;
            }
        }

        private bool _isFocused;
        internal bool IsFocused
        {
            get
            {               
                return _isFocused;
            }
            set
            {
                if (value != this._isFocused)
                {
                    if (value)
                    {
                        Listener.Current.BlockByMouseHover();
                        _lastForegroundWindow = CraftSynth.BuildingBlocks.WindowsNT.Misc.GetForegroundWindow();
                         FormSelectionListener.Current.Invoke((Action)(() =>
                        {
                            CraftSynth.BuildingBlocks.WindowsNT.Misc.SetForegroundWindow(FormSelectionListener.Current.Handle);
                         }));
                        Application.DoEvents();
                    }
                    else
                    {                        
                        ExecuteSetBackForegroundWindowInner();
                        _lastForegroundWindow = IntPtr.Zero;
                        Listener.Current.UnblockByMouseHover();
                    }
                    this._isFocused = value;
                }
            }
        }
        #endregion

        #region Public Methods
        internal void Start()
        {
            if (!this._started)
            {                
                    _timer.Change(_interval, _interval);
                    this._started = true;                
            }
        }

        internal void Stop()
        {
            if (this._started)
            {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                    this._started = false;
            }
        }

        internal bool SetBackLastForegroundWindow()
        {
            lock(_syncObject)
            {
                return ExecuteSetBackForegroundWindowInner();
            }
        }
        #endregion

        #region Constructors And Initialization
        private Focuser()
        {
            _timer = new System.Threading.Timer(timer_Thick, null, Timeout.Infinite, Timeout.Infinite);
            this._started = false;
        }
        #endregion

        #region Deinitialization And Destructors
        ~Focuser()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _timer = null;
        }
        #endregion

        #region Event Handlers
        public void timer_Thick(object state)
        {
            if (Monitor.TryEnter(_syncObject))
            {
                try
                {
                    IntPtr hWnd = CraftSynth.BuildingBlocks.WindowsNT.Misc.GetWindowUnderMouseCursor();
                    Process processUnderMouseCursor = CraftSynth.BuildingBlocks.WindowsNT.Misc.GetWindowProcess(hWnd);
                    IsFocused = processUnderMouseCursor.ProcessName == _processName;
                }
                finally
                {
                    Monitor.Exit(_syncObject);
                }
            }
        }
       
        #endregion

        #region Helpers
        private bool ExecuteSetBackForegroundWindowInner()
        {
            bool wasSetBack = false;
            if (_lastForegroundWindow != null && _lastForegroundWindow != IntPtr.Zero)
            {
                CraftSynth.BuildingBlocks.WindowsNT.Misc.SetForegroundWindow(_lastForegroundWindow);
                int i = 3;
                while (i > 0)
                {
                    if (_lastForegroundWindow == CraftSynth.BuildingBlocks.WindowsNT.Misc.GetForegroundWindow())
                    {
                        break;
                    }
                    Thread.Sleep(200);
                    i--;
                }
                wasSetBack = i > 0;
            }
            return wasSetBack;
        }
        #endregion
    }
}
