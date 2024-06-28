using System;
using System.Threading.Tasks;

namespace Timer
{
    public class TimerTask
    {
        public int id;

        public int delay;

        public int interval;

        public int loopTimes;

        public Action action;

        public DateTime dateTime;

        public TimerType type;

        public bool isRemove;

        public TimerTask(int id, int interval, Action action, int loopTimes, int delay, TimerType type)
        {
            this.id = id;
            this.interval = interval;
            this.action = action;
            this.loopTimes = loopTimes;
            this.delay = delay;
            this.type = type;
            isRemove = false;
            dateTime = DateTime.Now.AddMilliseconds(interval + delay);
        }

        public void Run()
        {
            if (type == TimerType.Sync)
            {
                TimerUtils.AddAction(id, action);
            }
            else
            {
                RunAsync();
            }
        }

        public async Task RunAsync()
        {
            await Task.Run(() => { action.Invoke(); });
        }

        public bool CheckLoop()
        {
            if (isRemove)
            {
                return false;
            }

            if (loopTimes < 0)
            {
                dateTime = DateTime.Now.AddMilliseconds(interval);
                return true;
            }
            else
            {
                loopTimes--;
                dateTime = DateTime.Now.AddMilliseconds(interval);
                return loopTimes > 0;
            }
        }
    }
}
