using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OSD
{
    internal partial class FormBackground : Form
    {
        public FormBackground()
        {
            InitializeComponent();
            this.BackColor = Settings.Current.backgroundColor;
            this.Opacity = Settings.Current.backgroundOpacity;
            this.TopMost = true;
            this.Shown += new EventHandler(FormBackground_Shown);
            this.Activated += new EventHandler(FormBackground_Activated);
        }

        void FormBackground_Activated(object sender, EventArgs e)
        {
            SetMainFormOnTop();
        }

        void FormBackground_Shown(object sender, EventArgs e)
        {
            SetMainFormOnTop();
        }

        private static void SetMainFormOnTop()
        {
            OSD.formMain.TopLevel = true;
            OSD.formMain.TopMost = true;
            CraftSynth.BuildingBlocks.WindowsNT.Misc.SetForegroundWindow(OSD.formMain.Handle);
        }

        public void UpdateSizeAndLocation(Form master)
        {
            this.Left = master.Left-Settings.Current.backgroundMarginsSize;
            this.Top = master.Top-Settings.Current.backgroundMarginsSize;
            this.Width = master.Width+(2*Settings.Current.backgroundMarginsSize);
            this.Height = master.Height+(2*Settings.Current.backgroundMarginsSize);
        }
    }
}
