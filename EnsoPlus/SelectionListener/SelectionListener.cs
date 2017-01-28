//I added these references just for this SelectionListener:
//BuildingBlocks.Common.Exceptions.EnterprizeLibrary, CustomXMLSerializer, BuildingBlocks.Common.Commmon


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;
using OSD;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using EnsoPlus.Entities;
using Extension;

namespace EnsoPlus.SelectionListener
{
    public class SelectionListener
    {
        #region Private Members        
        private Extension.IEnsoService _service;
        private Command _command;
        private static Thread formSelectionListenerUIThread;
        private ClipboardListener _clipboardListener;
        #endregion

        
        #region Properties
        public static bool Started { get; internal set; }

        private static SelectionListener _current;
        internal static SelectionListener Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new SelectionListener();
                }
                return _current;
            }
        }
        #endregion

        #region Public Methods
        public void Start(Extension.IEnsoService service, Command command)
        {
            if (Started)
            {
                MessagesHandler.Display("Allready started.");
            }
            else
            {
                Started = true;
                this._service = service;
                this._command = command;

                formSelectionListenerUIThread = new Thread(this.Start_NewThread);
                formSelectionListenerUIThread.Name = "FormSelectionListenerUIThread";
                formSelectionListenerUIThread.SetApartmentState(System.Threading.ApartmentState.STA);
                formSelectionListenerUIThread.Start();
            }
        }

        public void DisplayMessage(string text)
        {
            MessagesHandler.Display(text);
        }

        
        /// <param name="doneMessage">pass null for not to show message</param>        
        public void Paste(string text, string doneMessage, bool switchForegroundWindow)
        {
            Thread t = new Thread(PasteInNewThread);
            t.Start(new string[] { text, doneMessage,switchForegroundWindow.ToString() });
        }
        
        public void Stop()
        {
            formSelectionListenerUIThread = new Thread(this.Stop_NewThread);
            formSelectionListenerUIThread.Start();
        }

        public void AssignPasteKeyCombination(IEnsoService service)
        {
            List<Keys> selectedKeyCombination = KeyCombinationRetriever.Execute(null);
            if (selectedKeyCombination != null)
            {
				Settings.Current.pasteKeyCombination = CraftSynth.BuildingBlocks.UI.WindowsForms.KeysHelper.ToCommaSeparatedCodesString(selectedKeyCombination);
                Settings.Save();

				MessagesHandler.Display( CraftSynth.BuildingBlocks.UI.WindowsForms.KeysHelper.ToUserFriendlyString(selectedKeyCombination) + " is now key combination for paste operation. Please restart Selection Listener.");
            }
        }
        #endregion

        #region Constructors And Initialization
        #endregion

        #region Deinitialization And Destructors
        #endregion

        #region Event Handlers
        #endregion

        #region Helpers  
        private void Start_NewThread()
        {
            //show window
            FormSelectionListener.Current.TriggerConstruction();

            FormSelectionListener.Current.Show();
            FormSelectionListener.Current.Focus();
            FormSelectionListener.Current.TopLevel = true;
            FormSelectionListener.Current.TopMost = true;
            CraftSynth.BuildingBlocks.WindowsNT.Misc.SetForegroundWindow(FormSelectionListener.Current.Text);
             
            //start clipboard listener
            this._clipboardListener = new ClipboardListener(Listener.Current);
            this._clipboardListener.Start();

            //initialize listener and start if that was saved
            Listener.Current.Init(this._service, FormSelectionListener.Current);
            if (Settings.Current.active)
            {
            }

            FormSelectionListener.Current._thread = Thread.CurrentThread;
            FormSelectionListener.Current._thread.Priority = ThreadPriority.Lowest;
            FormSelectionListener.Current._thread.IsBackground = true;

            DisplayMessage("Started. Select some text ...");

            if (FormSelectionListener.Current.IsHandleCreated)
            {
                CraftSynth.BuildingBlocks.WindowsNT.Misc.SetForegroundWindow(FormSelectionListener.Current.Handle);
            }
            else
            {
                CraftSynth.BuildingBlocks.WindowsNT.Misc.SetForegroundWindow(FormSelectionListener.Current.Text);
            }

            //keep UI thread alive 
            while (true)
            {
                System.Threading.Thread.Sleep(200);
                Application.DoEvents();
            }
        }

        private void PasteInNewThread(object parameters)
        {
            string[] parametersArray = (string[])parameters;
            string text = parametersArray[0];
            string doneMessage = parametersArray[1];
            bool switchForegroundWindow = bool.Parse(parametersArray[2]);

            Focuser.Current.Stop();
            Listener.Current.BlockByPasteOperation();
            if (switchForegroundWindow)
            {
                Focuser.Current.SetBackLastForegroundWindow();
                Application.DoEvents();
            }
            HandlerForSelection.Put(text.ToString());
            //CraftSynth.BuildingBlocks.WindowsNT.Misc.SetForegroundWindow(this._parentForm.Handle);
            if (switchForegroundWindow)
            {
                Focuser.Current.SetBackLastForegroundWindow();
                Application.DoEvents();
            }
            Focuser.Current.Start();
            Listener.Current.UnblockByPasteOperation();
            if (doneMessage!=null)
            {
                SelectionListener.Current.DisplayMessage(doneMessage);
            }
        }
        
        private void Stop_NewThread()
        {
            FormSelectionListener.Current.Deinit();
        }
        #endregion
    }
}
