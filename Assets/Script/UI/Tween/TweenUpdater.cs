using System.Collections.Generic;
using UnityEngine;

namespace ReflectionUI
{
    public class TweenUpdater : MonoBehaviour
    {
        private Dictionary<int, UITween> _actions = new Dictionary<int, UITween>();

        private List<int> _removeActionIndexes = new List<int>();

        void Update()
        {
            int delta = (int)(Time.deltaTime * 1000);
            foreach (var action in _actions)
            {
                UITween vt = action.Value;

                if (vt.isStop)
                {
                    _removeActionIndexes.Add(vt.id);
                }
                else
                {
                    vt.Update(delta);
                }
            }

            for (int i = _removeActionIndexes.Count - 1; i >= 0; i--)
            {
                _actions.Remove(_removeActionIndexes[i]);
                _removeActionIndexes.RemoveAt(i);
            }
        }

        public void AddTween(UITween tween)
        {
            _actions.Add(tween.id, tween);
        }

        public void StopTween(int id)
        {
            UITween tween;
            _actions.TryGetValue(id, out tween);

            if (tween != null)
            {
                tween.isStop = true;
            }
        }
    }
}