using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CraftSynth.BuildingBlocks.IO
{
	public class Clipboard
	{
		//public static void Set(string text)
		//{
		//	Exception threadEx = null;
		//	Thread staThread = new Thread(
		//		delegate()
		//		{
		//			try
		//			{
		//				Clipboard.Clear();
		//				Clipboard.SetText(text);
		//			}

		//			catch (Exception ex) 
		//			{
		//				threadEx = ex;            
		//			}
		//		});
		//	staThread.SetApartmentState(ApartmentState.STA);
		//	staThread.Start();
		//	staThread.Join();
		//	if (threadEx != null)
		//	{
		//		throw threadEx;
		//	}
		//}

		//public static string Get()
		//{
		//	string data = null;
		//	Exception threadEx = null;
		//	Thread staThread = new Thread(
		//		delegate()
		//		{
		//			try
		//			{
		//				data = Clipboard.GetText();
		//			}

		//			catch (Exception ex) 
		//			{
		//				threadEx = ex;            
		//			}
		//		});
		//	staThread.SetApartmentState(ApartmentState.STA);
		//	staThread.Start();
		//	staThread.Join();
		//	if (threadEx != null)
		//	{
		//		throw threadEx;
		//	}
		//	return data;
		//}


		private static Thread clipboardHost;

		[STAThread]
		private static void CopySTA(object text)
		{
			System.Windows.Forms.Clipboard.SetText((string)text);
			Thread.CurrentThread.Abort();
		}
		public static void SetTextToClipboard(string text)
		{
			ParameterizedThreadStart ts = new ParameterizedThreadStart(CopySTA);
			clipboardHost = new Thread(ts);
			clipboardHost.SetApartmentState(ApartmentState.STA);
			clipboardHost.Start(text);
			clipboardHost.Join();
		}

		[STAThread]
		private static void GetClipboardFormatSTA(object o)
		{
			clipboardContentsFormatName = null;
			if (System.Windows.Forms.Clipboard.GetDataObject() != null)
			{
				if (System.Windows.Forms.Clipboard.GetDataObject().GetDataPresent(DataFormats.Bitmap)) clipboardContentsFormatName = DataFormats.Bitmap;
				else
					if (System.Windows.Forms.Clipboard.GetDataObject().GetDataPresent(DataFormats.UnicodeText)) clipboardContentsFormatName = DataFormats.UnicodeText;
					else
						if (System.Windows.Forms.Clipboard.GetDataObject().GetDataPresent(DataFormats.Text)) clipboardContentsFormatName = DataFormats.Text;
			}
			Thread.CurrentThread.Abort();
		}
		private static string clipboardContentsFormatName;
		public static string GetClipboardFormatName()
		{
			ParameterizedThreadStart ts = new ParameterizedThreadStart(GetClipboardFormatSTA);
			clipboardHost = new Thread(ts);
			clipboardHost.SetApartmentState(ApartmentState.STA);
			clipboardHost.Start(null);
			clipboardHost.Join();

			return clipboardContentsFormatName;
		}

		private static void GetTextFromClipboardSTA(object o)
		{
			clipboardText = null;
			if (System.Windows.Forms.Clipboard.GetDataObject() != null)
			{
				clipboardText = System.Windows.Forms.Clipboard.GetText();
			}
			Thread.CurrentThread.Abort();
		}
		private static string clipboardText;
		public static string GetTextFromClipboard()
		{
			ParameterizedThreadStart ts = new ParameterizedThreadStart(GetTextFromClipboardSTA);
			clipboardHost = new Thread(ts);
			clipboardHost.SetApartmentState(ApartmentState.STA);
			clipboardHost.Start(null);
			clipboardHost.Join();

			return clipboardText;
		}

		private static void GetImageFromClipboardSTA(object o)
		{
			clipboardImage = null;
			if (System.Windows.Forms.Clipboard.GetDataObject() != null)
			{
				clipboardImage = System.Windows.Forms.Clipboard.GetImage();
			}
			Thread.CurrentThread.Abort();
		}
		private static Image clipboardImage;
		public static Image GetImageFromClipboard()
		{
			ParameterizedThreadStart ts = new ParameterizedThreadStart(GetImageFromClipboardSTA);
			clipboardHost = new Thread(ts);
			clipboardHost.SetApartmentState(ApartmentState.STA);
			clipboardHost.Start(null);
			clipboardHost.Join();

			return clipboardImage;
		}
	}
}
