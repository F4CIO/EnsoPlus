using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using EnsoPlus.MessageHandler;

namespace ControlCornerLauncher
{
    /// <summary>
    
    /// </summary>
    public class KeysListener
    {
        #region Private Members
        private const Keys ActivatorKey = Keys.CapsLock;

		private CraftSynth.BuildingBlocks.WindowsNT.UserActivityHookNamespace.UserActivityHook _hook;

        private List<Keys> _pressedKeys;
		
        private int _targetWindowHandler;

	    private bool _listeningTemporarlyDisabled;
        #endregion

        #region Properties
	    public delegate void OnActivateCornerLauncherDelegate();
		public event OnActivateCornerLauncherDelegate OnActivateCornerLauncher;
		
	    public delegate void OnDeactivateCornerLauncherDelegate();
	    public event OnDeactivateCornerLauncherDelegate OnDeactivateCornerLauncher;

	    public delegate void OnKeyPressedInCornerLauncherDelegate(Keys keyCode, int keyValue);
	    public event OnKeyPressedInCornerLauncherDelegate OnKeyPressedInCornerLauncher;
        #endregion

        #region Public Methods
        public void Start()
        {
            //this._hook.Start();
			this._hook = new CraftSynth.BuildingBlocks.WindowsNT.UserActivityHookNamespace.UserActivityHook(false, true);
			//this._targetWindowHandler = (int)CraftSynth.BuildingBlocks.WindowsNT.Misc.GetWindowByCaption("ENSO+ Selection Listener");

            this._hook.KeyDown += _hook_KeyDown;
            this._hook.KeyUp += _hook_KeyUp;
        }

        public void Stop()
        {
			Thread thread = new Thread(() => StopInNewThread(null));
			thread.Start();
			// _hook.KeyDown -= _hook_KeyDown;
			// _hook.KeyUp -= _hook_KeyUp;

			//_hook.Stop(false,true,false);
        }

	    public void StopInNewThread(object p)
	    {
		        _hook.KeyDown -= _hook_KeyDown;
             _hook.KeyUp -= _hook_KeyUp;

            _hook.Stop(false,true,false);
	    }

	    private const int VK_CAPITAL = 0x14;
		const int KEYEVENTF_KEYDOWN = 0x0;
		const uint KEYEVENTF_KEYUP = 0x0002;
		const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
		[DllImport("user32.dll")]
		public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)]
		public static extern short GetKeyState(int keyCode);
	    public void TurnOnCapsLock()
	    {
			bool stateIsOn = (((ushort)GetKeyState(0x14)) & 0xffff) != 0;
		    if (!stateIsOn)
		    {
				//_hook.KeyDown -= _hook_KeyDown;
				//_hook.KeyUp -= _hook_KeyUp;
			   // while (!(_hook.KeyUpIsNull() && _hook.KeyDownIsNull()))
			   // {
				//    Thread.Sleep(1000);
			   // }
			    this._listeningTemporarlyDisabled = true;
				
				//press key
				keybd_event(VK_CAPITAL, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN, 0);
				keybd_event(VK_CAPITAL, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);

			    this._listeningTemporarlyDisabled = false;
			    //_hook.KeyDown += _hook_KeyDown;
			    //_hook.KeyUp += _hook_KeyUp;
		    }

			stateIsOn = (((ushort)GetKeyState(0x14)) & 0xffff) != 0;
			if (stateIsOn)
			{
				MessageHandler.ShowMessage("Caps Lock", "ON");
			}
			else
			{
				    MessageHandler.ShowMessage("Unable to toggle Caps Lock. Please try again.","");
			}
	    }

		public void TurnOffCapsLock()
		{
			bool stateIsOn = (((ushort)GetKeyState(0x14)) & 0xffff) != 0;
			if (stateIsOn)
			{
				//_hook.KeyDown -= _hook_KeyDown;
				//_hook.KeyUp -= _hook_KeyUp;
				//while (!(_hook.KeyUpIsNull() && _hook.KeyDownIsNull()))
				//{
				//	Thread.Sleep(1000);
				//}
				this._listeningTemporarlyDisabled = true;
				//press key
				keybd_event(VK_CAPITAL, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN, 0);
				keybd_event(VK_CAPITAL, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);

				this._listeningTemporarlyDisabled = false;
				//_hook.KeyDown += _hook_KeyDown;
				//_hook.KeyUp += _hook_KeyUp;
			}

			stateIsOn = (((ushort)GetKeyState(0x14)) & 0xffff) != 0;
			if (!stateIsOn)
			{
				MessageHandler.ShowMessage("Caps Lock", "OFF");
			}
			else
			{
				MessageHandler.ShowMessage("Unable to toggle Caps Lock. Please try again.", "");
			}
		}
        #endregion

        #region Constructors And Initialization
        public KeysListener(OnActivateCornerLauncherDelegate onActivateCornerLauncher, OnDeactivateCornerLauncherDelegate onDeactivateCornerLauncher, OnKeyPressedInCornerLauncherDelegate onKeyPressedInCornerLauncher )
        {
		    //this._hook = new CraftSynth.BuildingBlocks.WindowsNT.UserActivityHookNamespace.UserActivityHook(false, false);
            this._pressedKeys = new List<Keys>();
			
			this.OnActivateCornerLauncher = onActivateCornerLauncher;
	        this.OnDeactivateCornerLauncher = onDeactivateCornerLauncher;
	        this.OnKeyPressedInCornerLauncher = onKeyPressedInCornerLauncher;
        }
        #endregion

        #region Deinitialization And Destructors
        #endregion

        #region Event Handlers
        void _hook_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
			if (_listeningTemporarlyDisabled)
	        {
		        e.Handled = false;
		        e.SuppressKeyPress = false;
			}
			else {
				if (e.KeyCode == ActivatorKey && this._pressedKeys.Count==0)
				{//CAPS-LOCK pressed
					this._pressedKeys.Add(e.KeyCode);
				
					if(this.OnActivateCornerLauncher != null)
					{
						this.OnActivateCornerLauncher.Invoke();
					}

					e.Handled = true;
				}
				else if (e.KeyCode == ActivatorKey && this._pressedKeys.Contains(ActivatorKey))
				{//just CAPS-LOCK is being held
					e.Handled = true;
				}
				else if (e.KeyCode != ActivatorKey && this._pressedKeys.Contains(ActivatorKey))
				{//some key pressed while CAPS-LOCK is being held
					this._pressedKeys.Add(e.KeyCode);

					if (this.OnKeyPressedInCornerLauncher != null)
					{
						this.OnKeyPressedInCornerLauncher.Invoke(e.KeyCode, e.KeyValue);
					}

					if (e.KeyCode == Keys.Enter)
					{//used to confirm selection and also to unbolck if something hangs
						this._pressedKeys.Clear();
					
						if (this.OnDeactivateCornerLauncher != null)
						{
							this.OnDeactivateCornerLauncher.Invoke();
						}
					}
					e.Handled = true;
				}
				else {//CAPS-LOCK not being held -pass event to other applications 
					e.Handled = false;
				}

				e.SuppressKeyPress = e.Handled;
			}
        }

        void _hook_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        { 

			if (_listeningTemporarlyDisabled)
	        {
		        e.Handled = false;
		        e.SuppressKeyPress = false;
			}
			else { 
				if (this._pressedKeys.Contains(ActivatorKey))
				{ 
					if (e.KeyCode == ActivatorKey)
					{
						this._pressedKeys.Clear();
						if (this.OnDeactivateCornerLauncher != null)
						{
							this.OnDeactivateCornerLauncher.Invoke();
						}
					}

					if (this._pressedKeys.Contains(e.KeyCode))
					{
						this._pressedKeys.Remove(e.KeyCode);
					}

					e.Handled = true;
				}
				else
				{
					e.Handled = false;
				}
			}
        }
        #endregion

        #region Helpers
		//private void Grab()
		//{
		//	Thread t = new Thread(Grab_NewThread);
		//	t.Start();
		//}

		//private void Grab_NewThread()
		//{
		//	//BuildingBlocks.UI.Console.BeepInNewThread(20000, 200);
		//	CraftSynth.BuildingBlocks.WindowsNT.WindowsMessage.SendCustomMessage(this._targetWindowHandler, 1, 0, 0);
		//}
        #endregion

        
    }
}
