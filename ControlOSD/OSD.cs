using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using OSD.Menu;
using System.Threading;
using System.Windows.Forms;

namespace OSD
{
    public class OSD
    {
        internal static bool initialized = false;
		internal static CraftSynth.BuildingBlocks.UI.WindowsForms.FormDisplayer formDisplayer;
        internal static FormMain formMain;
        internal static CraftSynth.BuildingBlocks.UI.WindowsForms.FormDriver formMainDriver;
        internal static CraftSynth.BuildingBlocks.UI.WindowsForms.FormDriver formMainDriver2;
        internal static CraftSynth.BuildingBlocks.UI.WindowsForms.Slider formMainSlider;
        internal static FormBackground formBackground;
        internal static Menu.Menu menu;
        /// <summary>
        /// Live during slide only
        /// </summary>
        internal static Menu.Menu menu2; 

        internal static void InitializeIfNeeded()
        {
            if (!OSD.initialized)
            {
                Settings.Initialize();
				OSD.formDisplayer = new CraftSynth.BuildingBlocks.UI.WindowsForms.FormDisplayer(Settings.Current.showHideEffect, Settings.Current.showHideEffectDuration, Settings.Current.showHideEffectUpdateInterval);
                OSD.formMain = new FormMain();
                OSD.formMain.Location = Settings.Current.location;
                OSD.formMain.Size = Settings.Current.size;
                OSD.formMain.Resize+=new EventHandler(OSD.formMain.formMain_UpdateSettingsOnResize);
                OSD.formMain.Move+=new EventHandler(OSD.formMain.FormMain_UpdateSettingsOnMove);
				OSD.formMainDriver = new CraftSynth.BuildingBlocks.UI.WindowsForms.FormDriver(OSD.formMain, formMain.pnlContent);
				OSD.formMainDriver2 = new CraftSynth.BuildingBlocks.UI.WindowsForms.FormDriver(OSD.formMain, formMain.pnlContent2);
				OSD.formMainSlider = new CraftSynth.BuildingBlocks.UI.WindowsForms.Slider(OSD.formMain.pnlContentParent, new TimeSpan(0, 0, 1), new TimeSpan(0, 0, 0, 0, 100));

                OSD.initialized = true;
            }
        }

        public static string SettingsFilePath
        {
            get
            {
                return Settings.FilePath;
            }
            set
            {
                Settings.FilePath = value;
            }
        }

        public static void ShowMessage(string message)
        {
            OSD.InitializeIfNeeded();

            OSD.formDisplayer.ShowForm(OSD.formMain);
            OSD.formMain.ShowDialog();            
        }

        internal static void Close()
        {
            if (OSD.initialized && OSD.formMain != null)
            {
                OSD.formMain.Invoke((Action)(() =>
                {
                    Settings.Save();
                    OSD.formDisplayer.HideForm(OSD.formMain);
                    while (OSD.formMain.Opacity != 0)
                    {
                        Application.DoEvents();
                    }
                    OSD.formMain.Close();

                }));
                Application.DoEvents();    
            }

            //TODO: improve this lame bugfix
            OSD.formBackground.Dispose();
            OSD.formMain.Dispose();
            OSD.initialized = false;
        }

    }


}
