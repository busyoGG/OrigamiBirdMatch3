using System;
using System.Collections.Generic;
using UnityEngine;

namespace Timer
{
    public class TimerScript : MonoBehaviour
    {
        private Queue<(int, Action)> _actions = new Queue<(int, Action)>();

        private Dictionary<int, bool> _register = new Dictionary<int, bool>();

        void Update()
        {
            while (_actions.Count > 0)
            {
                (int, Action) item = _actions.Dequeue();
                Run(item.Item2);
                _register.Remove(item.Item1);
            }
        }

        public void AddAction(int id, Action action)
        {
            if (!_register.ContainsKey(id))
            {
                _actions.Enqueue((id, action));
                _register.Add(id, true);
            }
        }

        private void Run(Action action)
        {
            action.Invoke();
        }
    }
}
