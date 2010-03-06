using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SKYPE4COMLib;
using System.Threading;

namespace SkypeAgent
{
    class SkypeWrapper
    {
        private SkypeClass Skype;
        public SkypeWrapper()
        {
            Skype = new SkypeClass();
            if (!Skype.Client.IsRunning)
            {
                Skype.Client.Start(false, true);
                Thread.Sleep(10000);
            };
            
            Skype.Attach(5, false);

            if (Skype.CurrentUserStatus ==
              Skype.Convert.
                 TextToUserStatus("OFFLINE"))
            {
                Skype.ChangeUserStatus(
                  Skype.Convert.
                    TextToUserStatus("ONLINE"));
            }
        }

        static readonly string TeleNum = "+79169238475";

        public void PlaceCall()
        {
            Skype.PlaceCall(TeleNum, "", "", "");
        }

        public void SendSMS(string SMSText)
        {
            SmsMessageClass SMS = (SmsMessageClass)Skype.SendSms(TeleNum, SMSText, "");
        }
    
    }
}
