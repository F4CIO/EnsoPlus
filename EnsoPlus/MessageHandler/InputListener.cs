using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using IWshRuntimeLibrary;

namespace EnsoPlus.MessageHandler
{
	/// <summary>

	/// </summary>
	public class InputListener
	{
		#region Private Members
		private const Keys ActivatorKey = Keys.CapsLock;

		private CraftSynth.BuildingBlocks.WindowsNT.UserActivityHookNamespace.UserActivityHook _hook;

		private List<Keys> _pressedKeys = new List<Keys>();
		private event OnActivityDelegate OnActivity;
		#endregion

		#region Properties
		internal delegate void OnActivityDelegate();
		#endregion

		#region Public Methods
		internal void Subscribe(OnActivityDelegate onActivity)
		{
			this.OnActivity+=onActivity;
		}

		internal void Unsubscribe(OnActivityDelegate onActivity)
		{
			this.OnActivity-=onActivity;
		}

		internal void Restart()
		{
			this.StopHook();
			this.StartHook();
		}
		#endregion

		#region Constructors And Initialization
		internal InputListener()
		{
			this.StartHook();
		}
		#endregion

		#region Deinitialization And Destructors
		~InputListener()
		{
			this.StopHook();
		}
		#endregion

		#region Event Handlers
		void _hook_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if (e.KeyCode != ActivatorKey) // && this._pressedKeys.Count == 0)
			{
				if (this.OnActivity != null)
				{
					this.OnActivity.Invoke();
				}
			}

			e.Handled = false;
		}	
		
		void _hook_OnMouseActivity(object sender, CraftSynth.BuildingBlocks.WindowsNT.UserActivityHookNamespace.MouseEventArgsFull e)
		{
			if (this.OnActivity != null)
			{
				this.OnActivity.Invoke();
			}
		}

		void _hook_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			e.Handled = false;
		}
		#endregion

		#region Private Members	
		private void StartHook()
		{
			this._hook = new CraftSynth.BuildingBlocks.WindowsNT.UserActivityHookNamespace.UserActivityHook(true, true);

			this._hook.KeyDown += _hook_KeyDown;
			this._hook.KeyUp += _hook_KeyUp;
			this._hook.OnMouseActivity += _hook_OnMouseActivity;
		}

		private void StopHook()
		{
			try
			{
				_hook.KeyDown -= _hook_KeyDown;
				_hook.KeyUp -= _hook_KeyUp;
				_hook.OnMouseActivity -= _hook_OnMouseActivity;
			}
			catch
			{
				
			}

			try
			{
				_hook.Stop(true, true, false);
			}
			catch
			{
				
			}
		}
		#endregion

		#region Helpers
		#endregion
	}
}
