using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace Timer
{
    public class TimerScript : MonoBehaviour
    {
        private Queue<(int, Action)> _actions = new Queue<(int, Action)>();

        private ConcurrentDictionary<int, bool> _register = new ConcurrentDictionary<int, bool>();

        void Update()
        {
            while (_actions.Count > 0)
            {
                (int, Action) item = _actions.Dequeue();
                Run(item.Item2);
                // _register.Remove(item.Item1);
                _register.TryRemove(item.Item1, out var res);
            }
        }

        public void AddAction(int id, Action action)
        {
            if (!_register.ContainsKey(id))
            {
                _actions.Enqueue((id, action));
                _register.TryAdd(id, true);
            }
        }

        private void Run(Action action)
        {
            action.Invoke();
        }
    }
}