using System;
using System.Collections.Generic;
using System.Text;
using log4net.Appender;
using log4net.Core;
using log4net.Util;
using System.Threading;

namespace Service
{
    /// <summary>
    /// Appender that forwards LoggingEvents asynchronously
    /// </summary>
    /// <remarks>
    /// This appender forwards LoggingEvents to a list of attached appenders.
    /// The events are forwarded asynchronously using the ThreadPool.
    /// This allows the calling thread to be released quickly, however it does
    /// not guarantee the ordering of events delivered to the attached appenders.
    /// </remarks>
    public sealed class AsyncAppender : IAppender, IOptionHandler, IAppenderAttachable
    {
        private string _name;
        private AppenderAttachedImpl _appenderAttachedImpl;
        private FixFlags _fixFlags = FixFlags.All;

        private Thread _workingThread = null;
        private Queue<LoggingEvent> _loggingEvents = new Queue<LoggingEvent>(5000);
        private bool _exiting = false;
        private AutoResetEvent _workTriggerEvent = new AutoResetEvent(false);

        public void ActivateOptions()
        {
            _workingThread = new Thread(new ThreadStart(WorkingProc));
            _workingThread.Priority = ThreadPriority.Lowest;
            _workingThread.Start();
        }

        private void WorkingProc()
        {
            while (!_exiting)
            {
                _workTriggerEvent.WaitOne(1000);


                while(true)
                {
                    LoggingEvent nextEvent = null;

                    lock (_loggingEvents)
                    {
                        if (_loggingEvents.Count > 0)
                        {
                            nextEvent = _loggingEvents.Dequeue();
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (_appenderAttachedImpl != null)
                    {
                        _appenderAttachedImpl.AppendLoopOnAppenders(nextEvent);
                    }

                    //Thread.Sleep(10);
                }
            }
        }

        public void DoAppend(LoggingEvent loggingEvent)
        {
            loggingEvent.Fix = _fixFlags;

            lock (_loggingEvents)
            {
                _loggingEvents.Enqueue(loggingEvent);
                _workTriggerEvent.Set();
            }
        }

        public void Close()
        {
            _exiting = true;
            _workTriggerEvent.Set();
            _workingThread.Join();

            // Remove all the attached appenders
            lock (this)
            {
                if (_appenderAttachedImpl != null)
                {
                    _appenderAttachedImpl.RemoveAllAppenders();
                }
            }
        }

        #region IAppenderAttachable Members

        public void AddAppender(IAppender newAppender)
        {
            if (newAppender == null)
            {
                throw new ArgumentNullException("newAppender");
            }
            lock (this)
            {
                if (_appenderAttachedImpl == null)
                {
                    _appenderAttachedImpl = new log4net.Util.AppenderAttachedImpl();
                }
                _appenderAttachedImpl.AddAppender(newAppender);
            }
        }

        public AppenderCollection Appenders
        {
            get
            {
                lock (this)
                {
                    if (_appenderAttachedImpl == null)
                    {
                        return AppenderCollection.EmptyCollection;
                    }
                    else
                    {
                        return _appenderAttachedImpl.Appenders;
                    }
                }
            }
        }

        public IAppender GetAppender(string name)
        {
            lock (this)
            {
                if (_appenderAttachedImpl == null || name == null)
                {
                    return null;
                }

                return _appenderAttachedImpl.GetAppender(name);
            }
        }

        public void RemoveAllAppenders()
        {
            lock (this)
            {
                if (_appenderAttachedImpl != null)
                {
                    _appenderAttachedImpl.RemoveAllAppenders();
                    _appenderAttachedImpl = null;
                }
            }
        }

        public IAppender RemoveAppender(IAppender appender)
        {
            lock (this)
            {
                if (appender != null && _appenderAttachedImpl != null)
                {
                    return _appenderAttachedImpl.RemoveAppender(appender);
                }
            }
            return null;
        }

        public IAppender RemoveAppender(string name)
        {
            lock (this)
            {
                if (name != null && _appenderAttachedImpl != null)
                {
                    return _appenderAttachedImpl.RemoveAppender(name);
                }
            }
            return null;
        }

        #endregion

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public FixFlags Fix
        {
            get { return _fixFlags; }
            set { _fixFlags = value; }
        }
    }
}
