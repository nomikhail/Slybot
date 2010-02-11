using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
//using System.Threading;
using TestMaster.Common.Utilites;
using System.Diagnostics;
using System.Threading;
using log4net;

namespace Core
{
    public class QuikManager
    {
        static ILog logger = LogWrapper.GetLogger("QuikManager");
        static ILog apiLogger = LogManager.GetLogger("QuikAPI");

        public static bool PushOrder(string orderStr)
        {
            apiLogger.Debug("Send -> (" + orderStr + ")");
            return Trans2QuikAPI.SendAsyncTransaction(orderStr) == 0;
        }

        public static void StartProcessing()
        {
            processingThread = new Thread(new ThreadStart(WorkingProc));
            processingThread.Start();
        }

        public static void Terminate()
        {
            active = false;
            termEvent.Set();
        }

        static bool active = true;
        static ManualResetEvent termEvent = new ManualResetEvent(false);

        #region Connection management

        public static void TransactionsReply(int result, int extendedErrorCode, int replyCode, uint transactionID, double orderNumber, string replyMessage)
        {
            try
            {
                string msg = string.Format("Recieve <- ({0}; {1}; {2}; {3}; {4}; \"{5}\")", result, extendedErrorCode, replyCode, transactionID, orderNumber, replyMessage);
                if (result == 0 && replyCode == 3)
                    apiLogger.Debug(msg);
                else
                    apiLogger.Warn(msg);

                Order.TransactionsReply(result, extendedErrorCode, replyCode, transactionID, orderNumber, replyMessage);
            }
            catch (Exception ex)
            {
                Util.ProcessFatalException(ex);
            }
        }


        static Trans2QuikAPI.TransactionsReplyCallback callback = new Trans2QuikAPI.TransactionsReplyCallback(TransactionsReply);

        static void WorkingProc()
        {
            try
            {
                logger.Debug("Quik Manager working thread started.");

                // TODO: check whether I need this
                EstablishConnection();

                bool quikFailReported = false;
                while (active)
                {
                    if (Util.IsOperationTime())
                    {
                        if (quikConnected)
                            quikConnected = Trans2QuikAPI.IsDLLConnected() && Trans2QuikAPI.IsQuikConnected();

                        if (quikConnected)
                            Portfolio.SynchronizePosition();

                        if (quikConnected)
                        {
                            logger.Debug("Quik connection check OK.");
                        }
                        else
                        {
                            if (!quikFailReported && !Util.IsInitiationTime())
                            {
                                logger.Error("Quik connection lost and quik is being restarted.");
                                quikFailReported = true;
                            }

                            Trans2QuikAPI.Disconnect();
                            LaunchQuik.Launch();
                            termEvent.WaitOne(25000);
                            EstablishConnection();

                            if (quikConnected && quikFailReported)
                            {
                                logger.Warn("Quik connection was successfully recovered.");
                                quikFailReported = false;
                            }
                        }
                    }

                    termEvent.WaitOne(10000);
                }

                logger.Debug("Quik Manager working thread finished.");
            }
            catch (Exception ex)
            {
                Util.ProcessFatalException(ex);
            }
        }

        static void EstablishConnection()
        {
            quikConnected = (Trans2QuikAPI.Connect(Settings.quikDir) == 0);

            if(quikConnected)
                quikConnected = Trans2QuikAPI.IsDLLConnected() && Trans2QuikAPI.IsQuikConnected();

            if (quikConnected)
                quikConnected = (Trans2QuikAPI.SetTransactionsReplyCallback(callback) == 0);

            logger.DebugFormat("Establish quik connection result: {0}.", quikConnected ? "Success" : "Fail");
        }
        
        static Thread processingThread = null;

        static bool quikConnected = false;
        public static bool Connected { get { return Util.IsOperationTime() && quikConnected; } }

        #endregion
    }

    #region LaunchQuik

    public class LaunchQuik
    {
        // declare the delegate
        public delegate bool WindowEnumDelegate(IntPtr hwnd,
                                                 int lParam);

        // declare the API function to enumerate child windows
        [DllImport("user32.dll")]
        public static extern int EnumChildWindows(IntPtr hwnd,
                                                  WindowEnumDelegate del,
                                                  int lParam);

        [DllImport("user32.dll")]
        static extern int GetWindowText(int hWnd, StringBuilder text, int count);

        static string GetText(IntPtr hWnd)
        {
            StringBuilder builder = new StringBuilder(512);
            GetWindowText(hWnd.ToInt32(), builder, 512);
            return builder.ToString();
        }

        public static void Launch()
        {
            LaunchQuik lq = new LaunchQuik();
            lq.LaunchInternal(null);
        }

        void LaunchInternal(object stateInfo)
        {
            Process oldProc = Process.GetProcessesByName("info").SingleOrDefault();
            if (oldProc != null)
                oldProc.Kill();

            Thread.Sleep(3000);

            Process process = new Process();
            process.StartInfo.FileName = Settings.quikDir + "info.exe";
            process.StartInfo.WorkingDirectory = Settings.quikDir;

            process.Start();

            Thread.Sleep(3000);

            IntPtr enterButton, loginBox, pwdBox;

            while (!TryFindAuthenticationControls(out enterButton, out loginBox, out pwdBox))
                Thread.Sleep(1000);


            UIHelper.SimulateKeyboardInput(loginBox, "Новицкий Михаил Дмитриевич");
            UIHelper.SimulateKeyboardInput(pwdBox, "2KotaSignature");
            UIHelper.PerformClick(enterButton, 10, 10);
        }


        private bool TryFindAuthenticationControls(out IntPtr enterButton, out IntPtr loginBox, out IntPtr pwdBox)
        {
            enterButton = IntPtr.Zero;
            loginBox = IntPtr.Zero;
            pwdBox = IntPtr.Zero;

            targetWnd = IntPtr.Zero;
            EnumChildWindows(IntPtr.Zero, new WindowEnumDelegate(FindAuthWindow), 0);
            if (targetWnd == IntPtr.Zero)
                return false;

            editBoxes.Clear();
            enterButtonInt = IntPtr.Zero;

            EnumChildWindows(targetWnd, new WindowEnumDelegate(FindEditBoxes), 0);

            if (enterButtonInt == IntPtr.Zero || editBoxes.Count != 2)
                return false;

            enterButton = enterButtonInt;
            loginBox = editBoxes[0];
            pwdBox = editBoxes[1];

            return true;
        }

        IntPtr targetWnd = IntPtr.Zero;
        List<IntPtr> editBoxes = new List<IntPtr>();
        IntPtr enterButtonInt = IntPtr.Zero;

        public bool FindAuthWindow(IntPtr hwnd, int lParam)
        {
            string text = GetText(hwnd);

            if (text.Contains("Идентификация"))
                targetWnd = hwnd;

            return true;
        }

        public bool FindEditBoxes(IntPtr hwnd, int lParam)
        {
            string text = GetText(hwnd);
            if (text.Contains("Ввод"))
                enterButtonInt = hwnd;

            string wndClass = UIHelper.GetWindowClassName(hwnd);
            if (wndClass == "Edit")
                editBoxes.Add(hwnd);

            return true;
        }
    }

    #endregion
}
