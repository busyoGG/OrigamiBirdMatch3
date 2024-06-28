using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace PosTween
{
    public class TweenUpdater: MonoBehaviour
    {
        private List<TweenData> _actions = new List<TweenData>();

        public void Add(TweenData action)
        {
            _actions.Add(action);
        }
        
        private void Update()
        {
            for (int i = _actions.Count - 1; i >= 0; i--)
            {
                var action = _actions[i];
                action.DoAction();
                if (action.CheckEnd())
                {
                    action.DoCallback();
                    _actions.RemoveAt(i);
                }
            }
        }
    }
}