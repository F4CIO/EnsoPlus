using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IntPtr targetWindowHandle = CraftSynth.BuildingBlocks.WindowsNT.Misc.GetWindowByCaption("LnkFileReader");
            CraftSynth.BuildingBlocks.WindowsNT.WindowsMessageCopyData.SendMessageWithData(targetWindowHandle, @"D:\Trash\ccnet.lnk", this.Handle);
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                // program receives WM_COPYDATA Message from target app
                case CraftSynth.BuildingBlocks.WindowsNT.NativeMethods.WM_COPYDATA:
                    if (m.Msg == CraftSynth.BuildingBlocks.WindowsNT.NativeMethods.WM_COPYDATA)
                    {
                        // get the data
                        CraftSynth.BuildingBlocks.WindowsNT.NativeMethods.COPYDATASTRUCT cds = new CraftSynth.BuildingBlocks.WindowsNT.NativeMethods.COPYDATASTRUCT();
                        cds = (CraftSynth.BuildingBlocks.WindowsNT.NativeMethods.COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam,
                         typeof(CraftSynth.BuildingBlocks.WindowsNT.NativeMethods.COPYDATASTRUCT));
                        if (cds.cbData > 0)
                        {
                            byte[] data = new byte[cds.cbData];
                            Marshal.Copy(cds.lpData, data, 0, cds.cbData);
                            Encoding unicodeStr = Encoding.ASCII;
                            string receivedString = new string(unicodeStr.GetChars(data));
                            this.Text = receivedString;

                            m.Result = (IntPtr)1;
                        }
                    }
                    break;
            }
            base.WndProc(ref m);
        }
    }
}
