using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;

namespace EnsoPlus.WorkItemsProviders.Shortcuts
{
    public partial class FormLnkFileReaderProxy : Form
    {
        private static object _responseDataSync = new object();
        private string _responseData;
        private string ResponseData
        {
            get
            {
                //string result = null;
                //lock (_responseDataSync)
                //{
                //    result = _responseData;
                //}
                //return result;
                return _responseData;
            }
            set
            {
                lock (_responseDataSync)
                {
                    this._responseData = value;
                }
            }
        }
        private Thread _uiThread;
        private IntPtr _handle;

        private static FormLnkFileReaderProxy _current;
        internal static FormLnkFileReaderProxy Current
        {
            get
            {
                if (_current == null || _current.IsDisposed)
                {
                    _current = new FormLnkFileReaderProxy();
                }
                return _current;
            }
        }

        public FormLnkFileReaderProxy()
        {
            InitializeComponent();
        }

        public static void Initialize()
        {
            FormLnkFileReaderProxy.Current.ResponseData = null;
            
            //start form if not started
            if (FormLnkFileReaderProxy.Current._uiThread == null)
            {
                FormLnkFileReaderProxy.Current.ResponseData = null;

                FormLnkFileReaderProxy.Current._uiThread = new Thread(FormLnkFileReaderProxy.Current.StartUI_NewThread);
                FormLnkFileReaderProxy.Current._uiThread.Name = "FormLnkFileReaderProxy";
                FormLnkFileReaderProxy.Current._uiThread.Start();

				FormLnkFileReaderProxy._lastTimeNeeded = DateTime.UtcNow;
				_inactivityPunisher = new System.Threading.Timer(Timer_Tick, null, new TimeSpan(10000), new TimeSpan(10000));
            }

            //start x86 process that will read .lnk file
            Process[] matchedProcesses = Process.GetProcessesByName("EnsoPlus.LnkFileReader");
            if (matchedProcesses.Length == 0)
            {
                string lnkFileReaderFilePath = Path.Combine(Common.Helper.GetEnsoPlusWorkingFolder(), "EnsoPlus.LnkFileReader.exe");
                CraftSynth.BuildingBlocks.WindowsNT.Misc.OpenFile(lnkFileReaderFilePath, "-destWinCaption:" + FormLnkFileReaderProxy.Current.Text);
            }
        }

	    private static System.Threading.Timer _inactivityPunisher;

		private static void Timer_Tick(object state)
		{
			if ((_lastTimeNeeded == null || DateTime.Compare(_lastTimeNeeded.AddSeconds(15), DateTime.UtcNow) < 0)
				&& _current != null && FormLnkFileReaderProxy._current._uiThread!=null)
			{//inactive for more than 15s

				if (_inactivityPunisher != null)
				{
					//stop punisher
					_inactivityPunisher.Change(Timeout.Infinite, Timeout.Infinite);
					_inactivityPunisher = null;
				}

				//abort LnkFileReader
				Process[] matchedProcesses = Process.GetProcessesByName("EnsoPlus.LnkFileReader");
				if (matchedProcesses.Length == 1)
				{
					if (!matchedProcesses[0].HasExited)
					{
						try
						{
							matchedProcesses[0].Kill();
							Thread.Sleep(2000);
						}
						catch (Exception)
						{
							
						}
					}
				}

				//abort FormLnkFileReaderProxy
				//_current.Close();
				FormLnkFileReaderProxy._current._uiThread.Abort();
			}
		}     

	    private static DateTime _lastTimeNeeded;
        public static string WaitForLnkTarget(string lnkFilePath)
        {
	        FormLnkFileReaderProxy._lastTimeNeeded = DateTime.UtcNow;

            IntPtr targetWindowHandle = CraftSynth.BuildingBlocks.WindowsNT.Misc.GetWindowByCaption("LnkFileReader");
            CraftSynth.BuildingBlocks.WindowsNT.WindowsMessageCopyData.SendMessageWithData(targetWindowHandle, lnkFilePath, FormLnkFileReaderProxy.Current._handle);

            string result = FormLnkFileReaderProxy.Current.ResponseData; 
            int i = 0;
	        DateTime sTime = DateTime.Now;
	        bool retried = false;
            while (result == null && DateTime.Now.Subtract(sTime)<new TimeSpan(0,0,4))
            {//loop for 4 seconds
                result = FormLnkFileReaderProxy.Current.ResponseData;
				//Thread.Sleep(100);  //we are on UI thread that is used for other windows as well so we must not pause it
				Application.DoEvents();
                if ( DateTime.Now.Subtract(sTime)>new TimeSpan(0,0,2) && !retried)
                {//retry after 2s
	                retried = true;
                    CraftSynth.BuildingBlocks.WindowsNT.WindowsMessageCopyData.SendMessageWithData(targetWindowHandle, lnkFilePath, FormLnkFileReaderProxy.Current._handle);
                }
                i++;
            }
            return result;
        }

        private void StartUI_NewThread()
        {
            FormLnkFileReaderProxy.Current.Show();
            this._handle = this.Handle;

			Application.Run(this);
			////keep UI thread alive 
			//	while (true)
			//	{
			//		//System.Threading.Thread.Sleep(20);
			//		Application.DoEvents();
			//	}
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
                            receivedString = receivedString.Replace("\0", string.Empty);
                            this.ResponseData = receivedString??string.Empty;

                            m.Result = (IntPtr)1;
                        }
                    }
                    break;
            }
            base.WndProc(ref m);
        }
    }
}
