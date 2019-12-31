using System;
using System.Collections.Generic;

namespace Infrastructure.Messenger
{
    public class CMediator
    {
        private static readonly IDictionary<EMessageTypes, List<Action>> _map = new Dictionary<EMessageTypes, List<Action>>();

        private CMediator()
        {
        }

        public void Register(EMessageTypes messageType, Action callback)
        {
            if (!_map.ContainsKey(messageType))
            {
                var list = new List<Action> {callback};
                _map.Add(messageType, list);
            }
            else
            {
                bool found = false;
                foreach (var item in _map[messageType])
                    if (item.Method.ToString() == callback.Method.ToString())
                        found = true;
                if (!found)
                    _map[messageType].Add(callback);
            }
        }

        public static CMediator Instance { get; } = new CMediator();

        public void UnRegister(EMessageTypes messageType, Action callback)
        {
            if (_map.ContainsKey(messageType))
                _map[messageType].Remove(callback);
        }

        public void NotifyColleagues(EMessageTypes messageType, object args)
        {
            if (_map.ContainsKey(messageType))
                foreach (var callback in _map[messageType])
                    callback();
        }
    }
}
