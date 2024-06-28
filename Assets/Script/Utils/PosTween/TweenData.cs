using System;
using UnityEngine;

namespace PosTween
{
    public class TweenData
    {
        private int _duration;

        private int _delay;

        private float _time;

        private GridScript _grid;

        private Vector3 _start;

        private Vector3 _diff;

        private Action _callback;

        public TweenData(GridScript grid,int duration,int delay,Vector3 start,Vector3 diff,Action callback)
        {
            _grid = grid;
            _duration = duration;
            _delay = delay;
            _start = start;
            _diff = diff;
            _callback = callback;
        }

        public void DoAction()
        {
            if (_delay <= 0)
            {
                _time += Time.deltaTime * 1000;
                float ratio = _time / _duration;
                if (ratio > 1)
                {
                    ratio = 1;
                }
                _grid.pos = _start + _diff * ratio;
            }
            else
            {
                _delay -= (int)(Time.deltaTime * 1000);
            }
        }

        public bool CheckEnd()
        {
            return _time > _duration;
        }

        public void DoCallback()
        {
            _callback?.Invoke();
        }
    }
}