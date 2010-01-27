using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Runtime.CompilerServices;
using System.IO;
using System.Reflection;
using log4net.Config;
using log4net.Core;

namespace Core
{
    public class LogWrapper : ILog
    {
        private ILog wrapped;
        private LogWrapper(ILog logger)
        {
            wrapped = logger;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ILog GetLogger(string name)
        {
            if (!isConfigured)
            {
                Stream configStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Core.logging.config");
                XmlConfigurator.Configure(configStream);

                isConfigured = true;
            }

            return new LogWrapper(LogManager.GetLogger(name));
        }

        private static bool isConfigured = false;

        #region ILog Members

        public void Debug(object message, Exception exception)
        {
            wrapped.Debug(message, exception);
        }

        public void Debug(object message)
        {
            wrapped.Debug(message);
        }

        public void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            wrapped.DebugFormat(provider, format, args);
        }

        public void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            wrapped.DebugFormat(format, arg0, arg1, arg2);
        }

        public void DebugFormat(string format, object arg0, object arg1)
        {
            wrapped.DebugFormat(format, arg0, arg1);
        }

        public void DebugFormat(string format, object arg0)
        {
            wrapped.DebugFormat(format, arg0);
        }

        public void DebugFormat(string format, params object[] args)
        {
            wrapped.DebugFormat(format, args);
        }

        public void Error(object message, Exception exception)
        {
            wrapped.Error(message, exception);
        }

        public void Error(object message)
        {
            wrapped.Error(message);
        }

        public void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            wrapped.ErrorFormat(provider, format, args);
        }

        public void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            wrapped.ErrorFormat(format, arg0, arg1, arg2);
        }

        public void ErrorFormat(string format, object arg0, object arg1)
        {
            wrapped.ErrorFormat(format, arg0, arg1);
        }

        public void ErrorFormat(string format, object arg0)
        {
            wrapped.ErrorFormat(format, arg0);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            wrapped.ErrorFormat(format, args);
        }

        public void Fatal(object message, Exception exception)
        {
            wrapped.Fatal(message, exception);
        }

        public void Fatal(object message)
        {
            wrapped.Fatal(message);
        }

        public void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            wrapped.FatalFormat(provider, format, args);
        }

        public void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            wrapped.FatalFormat(format, arg0, arg1, arg2);
        }

        public void FatalFormat(string format, object arg0, object arg1)
        {
            wrapped.FatalFormat(format, arg0, arg1);
        }

        public void FatalFormat(string format, object arg0)
        {
            wrapped.FatalFormat(format, arg0);
        }

        public void FatalFormat(string format, params object[] args)
        {
            wrapped.FatalFormat(format, args);
        }

        public void Info(object message, Exception exception)
        {
            wrapped.Info(message, exception);
        }

        public void Info(object message)
        {
            wrapped.Info(message);
        }

        public void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            wrapped.InfoFormat(provider, format, args);
        }

        public void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            wrapped.InfoFormat(format, arg0, arg1, arg2);
        }

        public void InfoFormat(string format, object arg0, object arg1)
        {
            wrapped.InfoFormat(format, arg0, arg1);
        }

        public void InfoFormat(string format, object arg0)
        {
            wrapped.InfoFormat(format, arg0);
        }

        public void InfoFormat(string format, params object[] args)
        {
            wrapped.InfoFormat(format, args);
        }

        public bool IsDebugEnabled
        {
            get { return wrapped.IsDebugEnabled; }
        }

        public bool IsErrorEnabled
        {
            get { return wrapped.IsErrorEnabled; }
        }

        public bool IsFatalEnabled
        {
            get { return wrapped.IsFatalEnabled; }
        }

        public bool IsInfoEnabled
        {
            get { return wrapped.IsInfoEnabled; }
        }

        public bool IsWarnEnabled
        {
            get { return wrapped.IsWarnEnabled; }
        }

        public void Warn(object message, Exception exception)
        {
            wrapped.Warn(message, exception);
        }

        public void Warn(object message)
        {
            wrapped.Warn(message);
        }

        public void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            wrapped.WarnFormat(provider, format, args);
        }

        public void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            wrapped.WarnFormat(format, arg0, arg1, arg2);
        }

        public void WarnFormat(string format, object arg0, object arg1)
        {
            wrapped.WarnFormat(format, arg0, arg1);
        }

        public void WarnFormat(string format, object arg0)
        {
            wrapped.WarnFormat(format, arg0);
        }

        public void WarnFormat(string format, params object[] args)
        {
            wrapped.WarnFormat(format, args);
        }

        public ILogger Logger
        {
            get { return wrapped.Logger; }
        }

        #endregion

    }

}
