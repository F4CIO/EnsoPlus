using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using System.Runtime.InteropServices;

namespace LnkFileReader
{
    public partial class FormProxy : Form
    {
        public FormProxy()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {            
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
                            Program.ServeRequest(receivedString, this.Handle);

                            m.Result = (IntPtr)1;
                        }
                    }
                    break;
            }
            base.WndProc(ref m);
        }
    }
}
