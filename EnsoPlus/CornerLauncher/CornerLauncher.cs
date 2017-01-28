using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Extension;
using EnsoPlus.Entities;

namespace ControlCornerLauncher
{
	public class CornerLauncher
	{
		#region Private Members
		private CornerLauncherMenu _menu;
		private KeysListener _listener;
		private IntPtr _foregroundWindowForGrab;
		#endregion

		#region Properties

		public delegate void OnCloseDelegate(EnsoCommand ensoCommand, string postfix, IntPtr foregroundWindowForGrab);

		private event OnCloseDelegate _onClose;
		#endregion

		#region Public Methods

		public void TurnOnCapsLock()
		{
			this._listener.TurnOnCapsLock();
		}

		public void TurnOffCapsLock()
		{
			this._listener.TurnOffCapsLock();
		}
		#endregion

		#region Constructors And Initialization	
		public CornerLauncher(Dictionary<string, MergedCommand> mergedCommands, OnCloseDelegate onClose)
		{
			this._onClose = onClose;
			this._menu = new CornerLauncherMenu(mergedCommands);
			//Thread thread = new Thread(() => StartKeysListenerInNewThread(null));
			//thread.Start();
			this._listener = new KeysListener(OnActivateCornerLauncher, OnDeactivateCornerLauncher, OnKeyPressedInCornerLauncher);
			this._listener.Start();
		}


		public void StartKeysListenerInNewThread(object p)
		{
			this._listener = new KeysListener(OnActivateCornerLauncher, OnDeactivateCornerLauncher, OnKeyPressedInCornerLauncher);
			this._listener.Start();
		}
		#endregion

		#region Deinitialization And Destructors

		~CornerLauncher()
		{
			this._listener.Stop();
		}
		#endregion

		#region Event Handlers	
		private void OnActivateCornerLauncher()
		{
			this._foregroundWindowForGrab = CraftSynth.BuildingBlocks.WindowsNT.Misc.GetForegroundWindow();
			this._menu.Show();
			this._listener.Start();
		}

		public static object sync_obj = new object();

		private void OnKeyPressedInCornerLauncher(Keys keyCode, int keyValue)
		{
			//if (Monitor.TryEnter(sync_obj, TimeSpan.FromMilliseconds(1)))
			//{
			//	try
			//	{
					switch (keyCode)
					{
						case Keys.Up:
							this._menu.SelectPrevious();
							break;
						case Keys.Down:
							this._menu.SelectNext();
							break;
						case Keys.Tab:
							this._menu.SelectWhole();
							break;
						case Keys.Back:
							this._menu.RemoveLastTypedCharacter();
							break;
						default:
							//Accept only alpha-numeric and few special characters------------
							char? c = null;
							switch (keyCode)
							{
									case Keys.Oemplus:c='+';break;
									case Keys.OemMinus:c='-';break;
									case Keys.Space:c=' ';break;
									case Keys.OemQuotes:c='\'';break;
									case Keys.OemPeriod:c='.';break;
									case Keys.Oemcomma:c=',';break;
									case Keys.OemCloseBrackets:c=')';break;
									case Keys.OemOpenBrackets:c='(';break;
							}
							if(c==null){
								c = (char)keyValue; 
								if ((c >= 'a' && c<='z') || (c>='A' && c<='Z') || (c>='0' && c<='9'))
								{//alpha-numeric

								}
								else
								{
									c = null;
								}
							}
							//----------------------------------------------------------------
							if (c != null)
							{
								this._menu.AddTypedCharacter(c.Value);
							}
							break;
					}
			//	}
			//	finally
			//	{
			//		Monitor.Exit(sync_obj);
			//	}
			//}
		}

		private int i = 0;
		private void OnDeactivateCornerLauncher()
		{
			var c = this._menu.GetSelectedCommandAndPostfix();
			this._menu.Hide();
			
			Application.DoEvents();
			if (this._onClose != null && c!=null)
			{
				this._onClose.Invoke(c.Value.Key, c.Value.Value, this._foregroundWindowForGrab);
			}
		}
		#endregion

		#region Private Methods
		#endregion

		#region Helpers
		#endregion
	}
}
