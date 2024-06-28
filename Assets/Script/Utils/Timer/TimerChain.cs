using System;

namespace Timer
{
    public class TimerChain
    {
        private TimeWheel _timeWheel;

        //private int _id = -1;

        private TimerTask _task;

        private int _delay = 0;

        public TimerChain(TimeWheel wheel)
        {
            _timeWheel = wheel;
        }

        public TimerChain Once(int delay, Action action)
        {
            _task = _timeWheel.SetTimeout(_task != null ? _task.id : -1, delay + _delay, action);
            _delay = delay;
            return this;
        }

        public TimerChain Loop(int interval, Action action, int delay = 0, int loopTimes = -1)
        {
            _task = _timeWheel.SetInterval(_task != null ? _task != null ? _task.id : -1 : -1, interval, action,
                delay + _delay, loopTimes);
            ;
            _delay = interval * loopTimes + delay;
            return this;
        }

        public TimerChain OnceAsync(int delay, Action action)
        {
            _task = _timeWheel.SetTimeoutAsync(_task != null ? _task.id : -1, delay + _delay, action);
            _delay = delay;
            return this;
        }

        public TimerChain LoopAsync(int interval, Action action, int delay = 0, int loopTimes = -1)
        {
            _task = _timeWheel.SetIntervalAsync(_task != null ? _task.id : -1, interval, action, delay + _delay,
                loopTimes);
            _delay = interval * loopTimes + delay;
            return this;
        }

        public TimerChain Clear()
        {
            _task.isRemove = true;
            _timeWheel.ClearInterval(_task != null ? _task.id : -1, true);
            //ConsoleUtils.Log("�������Chain", _task.id);
            return this;
        }

        public int GetId()
        {
            return _task != null ? _task.id : -1;
        }
    }
}
