using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LnkFileReader
{
    /// <summary>
    /// Open this app with argument -destWinCaption:YourConsumerAppCaption
    /// On start other instances are killed by sending 'kill' message,
    /// then this app opens hidden window and listen for WM_COPYDATA. 
    /// Once received, using goten filePath it reads .lnk file and emits 
    /// lnk target to your app specified by caption in startup argument.
    /// </summary>
    static class Program
    {
        private static string _destinationWindowCaption;
        private static Murderer _murderer;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
			Application.SetUnhandledExceptionMode( UnhandledExceptionMode.CatchException );
			Application.ThreadException += Application_ThreadException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var form = new FormProxy();

            //send 'kill' to other open instances
			IntPtr otherInstance = CraftSynth.BuildingBlocks.WindowsNT.Misc.GetWindowByCaption(form.Text);
            if (otherInstance != IntPtr.Zero)
            {
				CraftSynth.BuildingBlocks.WindowsNT.WindowsMessageCopyData.SendMessageWithData(otherInstance, "kill", IntPtr.Zero);
            }

            foreach (string arg in args)
            {
                if (arg.StartsWith("-destWinCaption:", StringComparison.OrdinalIgnoreCase))
                {
                    _destinationWindowCaption = arg.Replace("-destWinCaption:", string.Empty);
                }
            }


            _murderer = new Murderer(_destinationWindowCaption);

            Application.Run(form);
        }

		static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			CraftSynth.BuildingBlocks.Logging.Misc.AddTimestampedExceptionInfoToApplicationWideLog(e.Exception);
		}

        public static void ServeRequest(string lnkFilePath, IntPtr handle)
        {
            if (lnkFilePath == "kill")
            {
                Murderer.Kill();
            }
            else
            {
                _murderer.Postpone();

                lnkFilePath = lnkFilePath.Trim();
                lnkFilePath = lnkFilePath.Remove(lnkFilePath.IndexOf("\0"));

                string target = Helper.GetLnkTarget(lnkFilePath);
                //System.IO.File.WriteAllText(@"D:\mmm.txt", lnkFilePath + "\r\n" + _destinationWindowCaption + "\r\n" + target);
				IntPtr targetWindowHandle = CraftSynth.BuildingBlocks.WindowsNT.Misc.GetWindowByCaption(_destinationWindowCaption);
				CraftSynth.BuildingBlocks.WindowsNT.WindowsMessageCopyData.SendMessageWithData(targetWindowHandle, target, handle);
				//CraftSynth.BuildingBlocks.Common.Misc.BeepInNewThread(5000,50);
            }
        }
    }
}
