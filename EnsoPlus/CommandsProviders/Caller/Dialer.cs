using System;
using System.Collections.Generic;
using System.Text;

using DotRas;
using Common;
using Extension;
using EnsoPlus.Entities;

namespace EnsoPlus.CommandsProviders.Caller
{
    class Dialer
    {
        public static IEnsoService service;

        private static Contact lastDialedContact;
        public static Contact contactToDial;

        public static bool hangUpAfterDialing
        {
            get
            {
                return Settings.Current.CallerHangUpAfterDialing;
            }
        }
        public static int hangUpAfterSeconds
        {
            get
            {
                return Settings.Current.CallerHangUpAfterSeconds;
            }
        }
        private static System.Timers.Timer hangUpTimer;
        private static int leftToThick;


        public static void Dial()
        {
            Dialer.Dial(contactToDial);
        }

        public static void Dial(Contact contact)
        {
            if (string.IsNullOrEmpty(contact.number))
            {
                MessagesHandler.Display( "No number found");
            }
            else
            {

                lastDialedContact = contact;
                TargetDotRas.Dial(contact);
                if (hangUpAfterDialing)
                {
                    hangUpTimer = new System.Timers.Timer();
                    hangUpTimer.AutoReset = true;
                    hangUpTimer.Elapsed += new System.Timers.ElapsedEventHandler(hangUpTimer_Elapsed);
                    hangUpTimer.Interval = 1000;
                    leftToThick = hangUpAfterSeconds;
                    hangUpTimer.Start();
                }

            }
        }

        static void hangUpTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (leftToThick <= 0)
            {
                hangUpTimer.Stop();
                Dialer.Cancel();
            }
            else
            {

                //StringBuilder sb = new StringBuilder();
                //int i = leftToThick;
                //while (i-- > 0) sb.Append('*');

                string message = string.Empty;
                //string subMessage = string.Empty;
                if (contactToDial is ContactUnknown)
                {
                    message = string.Format("Calling {0} ...", contactToDial.number);
                }
                else
                {
                    message = string.Format("Calling {0} ...", contactToDial.name);
                    //subMessage = string.Format("{0}", contactToDial.number);
                }

                MessagesHandler.Display( message, string.Format("Use phone now. {0} seconds to hang up...", leftToThick));
                leftToThick--;
            }
        }

      

        public static void Redial()
        {
            if (lastDialedContact == null)
            {
                MessagesHandler.Display( "No calls since program start");
            }else
            {
                Dialer.Dial(lastDialedContact);
            }
        }

        public static void Cancel()
        {
            if (hangUpTimer != null)
            {
                hangUpTimer.Stop();
            }

            if (TargetDotRas.Cancel())
            {   
                Logging.AddActionLog("Caller : Call cancelled");
            }
            else
            {
                MessagesHandler.Display( "Line is not open");
            }
        }

        public static void dialer_StateChanged(object sender, StateChangedEventArgs e)
        {
            string message = null;
            string subMessage = null;
            string log = null;

            if (e.State == RasConnectionState.ConnectDevice)
            {
                if (contactToDial is ContactUnknown)
                {
                    message = string.Format("Calling {0} ...", contactToDial.number);
                    log = message;
                }
                else
                {
                    message = string.Format("Calling {0} ...", contactToDial.name);
                    subMessage = string.Format("{0}", contactToDial.number);
                    log = string.Format("{0} - {1}", message, subMessage);
                }

            }
            else
            {
                message = e.State.ToString();
            }

            if (message != null)
            {
                if (subMessage != null)
                {
                    MessagesHandler.Display( message, subMessage);

                }
                else
                {
                    MessagesHandler.Display( message);
                }
            }

            if (log == null)
            {
                log = e.State.ToString();
            }

            Logging.AddActionLog("Caller : " + log);
        }

        public static void dialer_DialCompleted(object sender, DialCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessagesHandler.Display( e.Error.Message);
            }
            else if (e.TimedOut)
            {
                MessagesHandler.Display( "Call attempt timed out");
            }
            else if (e.Cancelled)
            {
                MessagesHandler.Display( "Call cancelled");
            }
            else if (e.Connected)
            {
                MessagesHandler.Display( "Connected");
            }

            
        }

    }
}
