using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Timer
{
    public class TimeWheel
    {
        private Dictionary<int, LinkedList<TimerTask>> _month = new Dictionary<int, LinkedList<TimerTask>>();
        private Dictionary<int, LinkedList<TimerTask>> _day = new Dictionary<int, LinkedList<TimerTask>>();
        private Dictionary<int, LinkedList<TimerTask>> _hour = new Dictionary<int, LinkedList<TimerTask>>();
        private Dictionary<int, LinkedList<TimerTask>> _minute = new Dictionary<int, LinkedList<TimerTask>>();
        private Dictionary<int, LinkedList<TimerTask>> _second = new Dictionary<int, LinkedList<TimerTask>>();

        private Dictionary<int, LinkedList<TimerTask>> _millisecond = new Dictionary<int, LinkedList<TimerTask>>();

        //private long _curTime = 0;
        private int _id = 0;
        private object _lock = new object();

        public TimeWheel()
        {
            Debug.Log(DateTime.Now);
            for (int i = 0; i < 13; i++)
            {
                _month.Add(i, new LinkedList<TimerTask>());
            }

            for (int i = 0; i < 31; i++)
            {
                _day.Add(i, new LinkedList<TimerTask>());
            }

            for (int i = 0; i < 24; i++)
            {
                _hour.Add(i, new LinkedList<TimerTask>());
            }

            for (int i = 0; i < 60; i++)
            {
                _minute.Add(i, new LinkedList<TimerTask>());
            }

            for (int i = 0; i < 60; i++)
            {
                _second.Add(i, new LinkedList<TimerTask>());
            }

            //毫秒级粒度为50ms
            for (int i = 0; i < 20; i++)
            {
                _millisecond.Add(i, new LinkedList<TimerTask>());
            }
        }

        public void Update()
        {
            lock (_lock)
            {
                DateTime now = DateTime.Now;

                LinkedList<TimerTask> month = _month[now.Month];
                LinkedList<TimerTask> day = _day[now.Day];
                LinkedList<TimerTask> hour = _hour[now.Hour];
                LinkedList<TimerTask> minute = _minute[now.Minute];
                LinkedList<TimerTask> second = _second[now.Second];

                int milliSecondDelta = now.Millisecond / 50;
                LinkedList<TimerTask> millisecond = _millisecond[milliSecondDelta];

                while (month.Count > 0)
                {
                    LinkedListNode<TimerTask> node = month.First;
                    month.RemoveFirst();
                    //添加到日轮
                    _day[node.Value.dateTime.Day].AddLast(node);
                }

                while (day.Count > 0)
                {
                    LinkedListNode<TimerTask> node = day.First;
                    day.RemoveFirst();
                    //添加到小时轮
                    _hour[node.Value.dateTime.Hour].AddLast(node);
                }

                while (hour.Count > 0)
                {
                    LinkedListNode<TimerTask> node = hour.First;
                    hour.RemoveFirst();
                    //添加到分轮
                    _minute[node.Value.dateTime.Minute].AddLast(node);
                }

                while (minute.Count > 0)
                {
                    LinkedListNode<TimerTask> node = minute.First;
                    minute.RemoveFirst();
                    //添加到秒轮
                    _second[node.Value.dateTime.Second].AddLast(node);
                }

                while (second.Count > 0)
                {
                    LinkedListNode<TimerTask> node = second.First;
                    second.RemoveFirst();
                    //添加到毫秒轮
                    _millisecond[node.Value.dateTime.Millisecond / 50].AddLast(node);
                }

                while (millisecond.Count > 0)
                {
                    //LinkedListNode<TimerTask> node = second.First;
                    //second.RemoveFirst();
                    ////添加到毫秒轮
                    //_millisecond[node.Value.dateTime.Millisecond / 50].AddLast(node);

                    foreach (var task in millisecond.ToList())
                    {
                        if (task != null && DateTime.Now >= task.dateTime)
                        {
                            millisecond.Remove(task);

                            //ConsoleUtils.Log("执行任务", task.id);
                            //task.RunAsync();
                            task.Run();

                            if (task.CheckLoop())
                            {
                                AddTask(task);
                            }
                        }
                    }
                }
            }
        }

        public TimerTask SetInterval(int id, int interval, Action action, int delay, int loopTimes,
            bool isRemove = false)
        {
            if (id == -1)
            {
                id = GetId();
            }

            TimerTask task = new TimerTask(id, interval, action, loopTimes, delay, TimerType.Sync);
            AddTask(task);
            return task;
        }

        public TimerTask SetTimeout(int id, int delay, Action action)
        {
            if (id == -1)
            {
                id = GetId();
            }

            TimerTask task = new TimerTask(id, 0, action, 1, delay, TimerType.Sync);
            AddTask(task);
            return task;
        }

        public TimerTask SetIntervalAsync(int id, int interval, Action action, int delay, int loopTimes)
        {
            if (id == -1)
            {
                id = GetId();
            }

            TimerTask task = new TimerTask(id, interval, action, loopTimes, delay, TimerType.Async);
            AddTask(task);
            return task;
        }

        public TimerTask SetTimeoutAsync(int id, int delay, Action action)
        {
            if (id == -1)
            {
                id = GetId();
            }

            TimerTask task = new TimerTask(id, 0, action, 1, delay, TimerType.Async);
            AddTask(task);
            return task;
        }

        public void ClearInterval(int id, bool isAll = false)
        {
            if (isAll)
            {
                RemoveTask(_month, id, isAll);
                RemoveTask(_day, id, isAll);
                RemoveTask(_hour, id, isAll);
                RemoveTask(_minute, id, isAll);
                RemoveTask(_second, id, isAll);
                RemoveTask(_millisecond, id, isAll);
            }
            else
            {
                if (RemoveTask(_month, id))
                {
                    return;
                }

                if (RemoveTask(_day, id))
                {
                    return;
                }

                if (RemoveTask(_hour, id))
                {
                    return;
                }

                if (RemoveTask(_minute, id))
                {
                    return;
                }

                if (RemoveTask(_second, id))
                {
                    return;
                }

                if (RemoveTask(_millisecond, id))
                {
                    return;
                }
            }
        }

        private void AddTask(TimerTask task)
        {
            lock (_lock)
            {
                _month[task.dateTime.Month].AddLast(task);
            }
        }

        private bool RemoveTask(Dictionary<int, LinkedList<TimerTask>> wheel, int id, bool isAll = false)
        {
            if (isAll)
            {
                bool res = false;
                foreach (var item in wheel)
                {
                    LinkedList<TimerTask> tasks = item.Value;
                    foreach (var task in tasks.ToList())
                    {
                        if (task.id == id)
                        {
                            //ConsoleUtils.Log("清除任务", task.id, task.isRemove);
                            tasks.Remove(task);
                            res = true;
                        }
                    }
                }

                return res;
            }
            else
            {
                foreach (var item in wheel)
                {
                    LinkedList<TimerTask> tasks = item.Value;
                    foreach (var task in tasks)
                    {
                        if (task.id == id)
                        {
                            tasks.Remove(task);
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        private int GetId()
        {
            return _id++;
        }
    }
}