using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Windows.Forms;

namespace EnsoPlus.MessageHandler
{
	public class MessageHandler
	{
		#region Private Members
		private static InputListener _inputListener = new InputListener();
		private static DateTime _inputListener_LastActivityAsUtc = DateTime.MinValue;
		private static FormMessageHandler _formMessageHandler = new FormMessageHandler();
		private static DateTime _formShowingMomentAsUtc = DateTime.MinValue;
		#endregion

		#region Properties
		#endregion

		#region Public Methods

		private delegate void ShowMessageDelegate(string text, string subtext);
		public static void ShowMessage(string text, string subtext)
		{
			if (_formMessageHandler.InvokeRequired)
			{
				_formMessageHandler.Invoke(new ShowMessageDelegate(ShowMessage), new object[] {text, subtext});
			}
			else
			{
				CenterFormOnScreen(_formMessageHandler, Screen.FromPoint(Cursor.Position));
				_formMessageHandler.tbText.Text = text ?? "null";
				_formMessageHandler.tbText.SelectAll();
				_formMessageHandler.tbText.DeselectAll();
				_formMessageHandler.tbSubtext.Text = subtext ?? string.Empty;
				_formMessageHandler.Show();
				_formShowingMomentAsUtc = DateTime.UtcNow;
				Application.DoEvents();
				_inputListener.Subscribe(OnActivity);
			}
		}

		public static void HideAllMessages()
		{
			HideAllMessagesInner();
		}

		public static void RestartInputListenerIfStailed()
		{
			if (DateTime.Compare(DateTime.UtcNow.AddSeconds(-1), _formShowingMomentAsUtc)>0  &&
				DateTime.Compare(DateTime.UtcNow.AddSeconds(-3), _inputListener_LastActivityAsUtc) > 0)
			{
				_inputListener_LastActivityAsUtc = DateTime.UtcNow;
				_inputListener.Restart();
				if (Debugger.IsAttached)
				{
					CraftSynth.BuildingBlocks.Common.Misc.BeepInNewThread();
				}
			}
		}
		#endregion

		#region Constructors And Initialization
		#endregion

		#region Deinitialization And Destructors
		#endregion

		#region Event Handlers

		private static void OnActivity()
		{
			_inputListener_LastActivityAsUtc = DateTime.UtcNow;
			HideAllMessagesInner();
		}

		private static void HideAllMessagesInner()
		{
			_formMessageHandler.Visible = false;
			_inputListener.Unsubscribe(OnActivity);
		}

		#endregion

		#region Private Methods

		#endregion

		#region Helpers

		private delegate void CenterFormOnScreenDelegate(Form f, Screen screen);
		private static void CenterFormOnScreen(Form f, Screen screen)
		{
			if (f.InvokeRequired)
			{
				f.Invoke(new CenterFormOnScreenDelegate(CenterFormOnScreen), new object[] {f, screen});
			}
			else
			{
				var s = screen ?? Screen.PrimaryScreen;
				int x = s.Bounds.X + ((s.Bounds.Width - f.Width)/2);
				int y = s.Bounds.Y + (s.Bounds.Height - f.Height)/2;
				f.DesktopLocation = new Point(x, y);
			}
		}
		#endregion
	}
}
