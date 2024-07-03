using System;

namespace ReflectionUI
{
    public class UITween
    {
        public int id;

        public float duration;

        private int _time;

        public bool isStop;

        public Action<float> updater;

        public Action callback;

        public void Update(int delta)
        {
            _time += delta;
            if (_time <= duration)
            {
                updater?.Invoke(_time);
            }
            else
            {
                updater?.Invoke(duration);
                callback?.Invoke();
                isStop = true;
                updater = null;
                callback = null;
            }
        }
    }
}