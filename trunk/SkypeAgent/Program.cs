using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core;
using log4net;
using System.Threading;

namespace SkypeAgent
{
    class Program
    {
        static ILog logger = LogWrapper.GetLogger("SkypeAgent");
        static int failsAllowed = 3;

        static void Main(string[] args)
        {
            while (true)
            {
                var dataContext = new SlyBotDataContext();
                var skypeMsg = dataContext.SkypeMessages.Where(msg => !msg.IsSent).FirstOrDefault();

                if (skypeMsg == null)
                    return;

                try
                {
                    var skype = new SkypeWrapper();
                    skype.SendSMS(skypeMsg.Message);
                    skypeMsg.IsSent = true;
                    dataContext.SubmitChanges();

                    logger.WarnFormat("Message \"{0}\" was sent to skype sms gate.", skypeMsg.Message);
                }
                catch (Exception skypeEx)
                {
                    logger.Fatal("Failed to report to skype.", skypeEx);
                    --failsAllowed;

                    if (failsAllowed == 0)
                        return;

                    Thread.Sleep(10000);
                }                
            }
        }
    }
}
