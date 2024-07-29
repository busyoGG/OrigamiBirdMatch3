using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PosTween
{
    public class TweenUpdater: MonoBehaviour
    {
        private Dictionary<string,List<TweenData>> _actions = new ();

        private List<string> _keys = new();

        public void Add(string id,TweenData action)
        {
            if (!_actions.ContainsKey(id))
            {
                _actions.Add(id,new());
            }
            _actions[id].Add(action);
        }

        public void ClearAll(string id)
        {
            _actions.Remove(id);
        }
        
        private void Update()
        {
            _keys.Clear();
            _keys.AddRange(_actions.Keys);

            foreach (var key in _keys)
            {
                var actions = _actions[key];
                for (int i = actions.Count - 1; i >= 0; i--)
                {
                    var action = actions[i];
                    action.DoAction();
                    if (action.CheckEnd())
                    {
                        action.DoCallback();
                        actions.RemoveAt(i);
                    }
                }

                if (actions.Count == 0)
                {
                    _actions.Remove(key);
                }
            }
        }
    }
}