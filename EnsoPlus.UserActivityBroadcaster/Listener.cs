using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace UserActivityBroadcaster
{
    /// <summary>
    /// Rises Grab event on:
    ///  -CTRL+C : ctrl was pressed, a was pressed, a is released 67,ctrl 162
    ///  -CTRL+X 68
    ///  -CTRL+A 65
    ///  -LEFT MOUSE BUTTON DOUBLE CLICK
    ///  -DRAG END WITH LEFT MOUSE BUTTON
    ///
    ///  -TODO: SHIFT+ARROW KEY,HOME,END,PGUP OR PG DOWN
    /// </summary>
    public class Listener
    {
        #region Private Members
        private const int KEY_CTRL = 162;
        private const int KEY_C = 67;
        private const int KEY_X = 68;
        private const int KEY_A = 65;

		private CraftSynth.BuildingBlocks.WindowsNT.UserActivityHookNamespace.UserActivityHook _hook;

        private List<int> _pressedKeys;
        private bool _buttonLeftPressed;

        private System.Threading.Timer _leftMouseDoubleClickTimer;
        private bool _chanceForDoubleClickActive;

        private int _dragStartX;
        private int _dragStartY;
        private bool _dragStarted;

        private int _targetWindowHandler;

        private List<Keys> _pasteKeyCombination;
        //private System.Threading.Timer _pasteDelayTimer;
        //bool _pasteDelayActive;
        #endregion

        #region Properties
        #endregion

        #region Public Methods
        public void Start()
        {
            //this._hook.Start();
			this._hook = new CraftSynth.BuildingBlocks.WindowsNT.UserActivityHookNamespace.UserActivityHook(true, true);
			this._targetWindowHandler = (int)CraftSynth.BuildingBlocks.WindowsNT.Misc.GetWindowByCaption("ENSO+ Selection Listener");

			this._hook.OnMouseActivity += new CraftSynth.BuildingBlocks.WindowsNT.UserActivityHookNamespace.UserActivityHook.MouseEventHandlerFull(this._hook_OnMouseActivity);
            
            this._hook.KeyDown += new System.Windows.Forms.KeyEventHandler(_hook_KeyDown);
            this._hook.KeyUp += new System.Windows.Forms.KeyEventHandler(_hook_KeyUp);
        }

        public void Stop()
        {
			 _hook.OnMouseActivity -= new CraftSynth.BuildingBlocks.WindowsNT.UserActivityHookNamespace.UserActivityHook.MouseEventHandlerFull(_hook_OnMouseActivity);
           
             _hook.KeyDown -= new System.Windows.Forms.KeyEventHandler(_hook_KeyDown);
             _hook.KeyUp -= new System.Windows.Forms.KeyEventHandler(_hook_KeyUp);

            _hook.Stop(true,true,false);
        }
        #endregion

        #region Constructors And Initialization
        public Listener(List<Keys> pasteKeyCombination)
        {
			this._hook = new CraftSynth.BuildingBlocks.WindowsNT.UserActivityHookNamespace.UserActivityHook(false, false);
            this._leftMouseDoubleClickTimer = new System.Threading.Timer(LeftMouseDoubleClickTimer_Tick, null, Timeout.Infinite, Timeout.Infinite);

            //this._pasteDelayTimer = new System.Threading.Timer(PasteDelayTimer_Tick, null, Timeout.Infinite, Timeout.Infinite);
            this._pasteKeyCombination = pasteKeyCombination;
            this._pressedKeys = new List<int>();
            this._buttonLeftPressed = false;
            this._dragStarted = false;
            this._chanceForDoubleClickActive = false;            
        }
        #endregion

        #region Deinitialization And Destructors
        #endregion

        #region Event Handlers
        void _hook_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (!this._pressedKeys.Contains(e.KeyValue))
            {
                this._pressedKeys.Add(e.KeyValue);

                if (this._pressedKeys.Contains(KEY_CTRL) &&
                    (e.KeyValue == KEY_A)
                    )
                {
                    this.Grab();
                }
                else 
                {
                    bool pasteKeyCombinationSatisfied = true;
                    foreach (Keys pasteKey in _pasteKeyCombination)
                    {
                        if(!_pressedKeys.Contains((int)pasteKey))
                        {
                            pasteKeyCombinationSatisfied = false;
                            break;
                        }
                    }

                    if (pasteKeyCombinationSatisfied)
                    {
                        //if (this._pasteDelayActive)
                        //{
                        //    e.Handled = true;
                        //    e.SuppressKeyPress = true;
                        //}
                        //else
                        //{
                        //    this._pasteDelayActive = true;
                        //    this._pasteDelayTimer.Change(2000, Timeout.Infinite);
                            //Paste();
						CraftSynth.BuildingBlocks.WindowsNT.WindowsMessage.SendCustomMessage(this._targetWindowHandler, 3, 0, 0);
                        //}
                    }
                }

            }
        }

        void _hook_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if(this._pressedKeys.Contains(e.KeyValue))
            {
                this._pressedKeys.Remove(e.KeyValue);
            }
        }

        void _hook_OnMouseActivity(object sender, CraftSynth.BuildingBlocks.WindowsNT.UserActivityHookNamespace.MouseEventArgsFull e)
        {
            if (this._buttonLeftPressed)
            {
                if (!this._dragStarted &&
                    (
                    e.X - this._dragStartX > 5 || e.X - this._dragStartX < -5 ||
                    e.Y - this._dragStartY > 5 || e.Y - this._dragStartY < -5
                    )
                )
                {
                    this._dragStarted = true;
                }
            }
      

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                
                if (!e.IsReleased)
                {
                    if (!this._buttonLeftPressed)
                    {
                        this._buttonLeftPressed = true;
                        this._dragStartX = e.X;
                        this._dragStartY = e.Y;
                    }                    
                }else
                if(e.IsReleased)
                {
                    this._buttonLeftPressed = false;
                    if (!this._chanceForDoubleClickActive)
                    {
                        this._chanceForDoubleClickActive = true;
                        this._leftMouseDoubleClickTimer.Change(1000, Timeout.Infinite);                    
                    }
                    else
                    {
                        this._chanceForDoubleClickActive = false;
                        this._leftMouseDoubleClickTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        this.Grab();
                    }

                    if (this._dragStarted)
                    {
                        this._dragStarted = false;
                        this.Grab();
                    }

					CraftSynth.BuildingBlocks.WindowsNT.WindowsMessage.SendCustomMessage(this._targetWindowHandler, 2, 0, 0);
                }  
            }
        }

        void LeftMouseDoubleClickTimer_Tick(object state)
        {
            this._chanceForDoubleClickActive = false;
        }

        //void PasteDelayTimer_Tick(object state)
        //{
        //    this._pasteDelayActive = false;
        //}
        #endregion

        #region Helpers
        private void Grab()
        {
            Thread t = new Thread(Grab_NewThread);
            t.Start();
        }

        private void Grab_NewThread()
        {
            //BuildingBlocks.UI.Console.BeepInNewThread(20000, 200);
			CraftSynth.BuildingBlocks.WindowsNT.WindowsMessage.SendCustomMessage(this._targetWindowHandler, 1, 0, 0);
        }
        #endregion

        
    }
}
