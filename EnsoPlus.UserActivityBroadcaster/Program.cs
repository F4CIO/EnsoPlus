using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace UserActivityBroadcaster
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(params string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool kill = false;            
            List<int> keysToWatch = new List<int>();
            List<Keys> pasteKeyCombination = null;
            ProcessArguments(args, ref kill, ref pasteKeyCombination);

            if (kill)
            {
				CraftSynth.BuildingBlocks.WindowsNT.Misc.CloseProcesses("UserActivityBroadcaster", true);                
                Application.Exit();
            }  

            Listener listener = new Listener(pasteKeyCombination);

            Murderer murderer = new Murderer(listener, "ENSO+ Selection Listener");

            listener.Start();
            Application.Run();            
        }

        private static void ProcessArguments(string[] args, ref bool kill, ref List<Keys> pasteKeyCombination)
        {
            if (args.Length == 0)
            {
            }
            else
            {
                foreach (string arg in args)
                {
                    if(arg.StartsWith("-PasteKeyCombination:"))
                    {
                        string argValue = arg.Split(':')[1];
						pasteKeyCombination = CraftSynth.BuildingBlocks.UI.WindowsForms.KeysHelper.BuildKeyListFromCommaSeparatedCodesString(argValue);
                    }else
                    
                    if (string.Compare(arg, "-kill", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        kill = true;
                    }
                }
            }
        }
    }
}
