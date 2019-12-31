using FaClient.FaStorageService;
using Infrastructure.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using NLog;

namespace FaClient
{
    public class CBackgroundWorker : IDisposable
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private readonly List<CRule> _rules;
        private readonly IFaStorageService _storageService;
        private readonly CClientInfo _clientInfo;

        private CEventLogWatcher _watcher;
        private readonly ConcurrentQueue<CEventInfo> _eventsQueue;
        private readonly ConcurrentQueue<(CEventInfo eventInfo, CRule rule)> _eventsNotifyQueue;
        private readonly ManualResetEvent _mreNewEvent;
        private readonly ManualResetEvent _mreNotify;
        private readonly ManualResetEvent _mreStop;
        private readonly Thread _threadStore;
        private readonly Thread _threadNotify;

        public CBackgroundWorker(IFaStorageService storageService, CClientInfo clientInfo)
        {
            _storageService = storageService;
            _rules = _storageService.FindAllRules();
            _clientInfo = clientInfo;
            _eventsQueue = new ConcurrentQueue<CEventInfo>();
            _eventsNotifyQueue = new ConcurrentQueue<(CEventInfo, CRule)>();
            _threadStore = new Thread(WriteEventIntoStore) {IsBackground = true};
            _threadNotify = new Thread(SendNotification) { IsBackground = true };
            _mreNewEvent = new ManualResetEvent(false);
            _mreNotify = new ManualResetEvent(false);
            _mreStop = new ManualResetEvent(false);
        }

        public void Dispose()
        {
            _watcher.Dispose();
            _mreNewEvent.Dispose();
            _mreNotify.Dispose();
            _mreStop.Dispose();
        }

        public event EventHandler<CEventRecordArgs> OnStoredEventRecord;

        public void Start()
        {
            try
            {
                _mreNewEvent.Reset();
                _mreNotify.Reset();
                _mreStop.Reset();
                _threadStore.Start();
                _threadNotify.Start();
                _watcher = new CEventLogWatcher(_rules, _clientInfo.ClientInfoId);
                _watcher.OnNewEventRecord += EnqueueEvent;
                _watcher.StartWatcher();
                s_logger.Trace("Started worker");
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
                Stop();
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                _watcher.StopWatcher();
                _mreStop.Set();
                _threadStore.Join();
                _threadNotify.Join();
                s_logger.Trace("Stopped worker");
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
                throw;
            }
        }
        
        private void EnqueueEvent(object sender, CEventRecordArgs args)
        {
            CEventInfo eventInfo = args.EventInfo;
            CRule rule = args.Rule;
            _eventsQueue.Enqueue(eventInfo);
            s_logger.Trace($"Enqueue {eventInfo.EventRecordId}");
            _mreNewEvent.Set();
            if (rule.Notify)
            {
                _eventsNotifyQueue.Enqueue((eventInfo, rule));
                _mreNotify.Set();
            }
        }

        private void WriteEventIntoStore()
        {
            while (true)
            {
                int waitResult = WaitHandle.WaitAny(new WaitHandle[] { _mreStop, _mreNewEvent });
                if (waitResult == 0)
                {
                    break;
                }
                try
                {
                    if (_eventsQueue.TryPeek(out var eventInfo))
                    {
                        if (_storageService.WriteEventInfo(eventInfo))
                        {
                            _eventsQueue.TryDequeue(out eventInfo);
                            s_logger.Trace($"Recorded into the storage: {eventInfo.EventRecordId}");

                        }
                    }
                }
                catch (Exception ex)
                {
                    s_logger.Error($"Exception occurred during writing to store: {ex}");
                }
            }
        }

        private void SendNotification()
        {
            while (true)
            {
                int waitResult = WaitHandle.WaitAny(new WaitHandle[] { _mreStop, _mreNotify });
                if (waitResult == 0)
                {
                    break;
                }
                try
                {
                    if (_eventsNotifyQueue.TryPeek(out var tuple))
                    {
                        if (_storageService.SendNotification(tuple.eventInfo, tuple.rule))
                        {
                            _eventsNotifyQueue.TryDequeue(out tuple);
                            s_logger.Trace($"Sent notification about event: {tuple.eventInfo.EventRecordId}");
                            OnStoredEventRecord?.Invoke(this,
                                new CEventRecordArgs {EventInfo = tuple.eventInfo, Rule = tuple.rule});
                        }
                    }
                }
                catch (Exception ex)
                {
                    s_logger.Error($"Exception occurred during sending notification: {ex}");
                }
            }
        }
    }
}