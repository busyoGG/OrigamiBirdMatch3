using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Timer
{
    public class TimerUtils
    {
        private static TimeWheel _timeWheel;
        private static Thread _thread;
        private static bool _isRunning = false;
        private static TimerScript _timerScript;

        private static Dictionary<string, Dictionary<int, TimerChain>> _timerNodes =
            new Dictionary<string, Dictionary<int, TimerChain>>();

        public static void Init()
        {
            _timeWheel = new TimeWheel();
            _isRunning = true;

#if UNITY_WEBGL
            Debug.Log("当前是WEBGL平台");
#else
            Debug.Log("当前不是WEBGL平台");
            _thread = new Thread(Update);
            _thread.Start();
#endif

            GameObject obj = new GameObject();
            obj.name = "TimerUtils";
            _timerScript = obj.AddComponent<TimerScript>();
        }

        public static void Stop()
        {
            _isRunning = false;
            //_thread.Abort();
        }

        public static void Update()
        {
#if UNITY_WEBGL
            _timeWheel.Update();
#else
            while (_isRunning)
            {
                _timeWheel.Update();
            }
#endif
        }

        /// <summary>
        /// 同步定时器
        /// </summary>
        /// <param name="id">节点id</param>
        /// <param name="delay">延时 单位毫秒</param>
        /// <param name="action">行为</param>
        /// <returns></returns>
        public static TimerChain Once(string id,int delay, Action action)
        {
            var timeChain = new TimerChain(_timeWheel);

            void Callback()
            {
                Remove(id,timeChain.GetId());
                action?.Invoke();
            }

            timeChain.Once(delay, Callback);
            
            if (!_timerNodes.ContainsKey(id))
            {
                _timerNodes.Add(id,new Dictionary<int, TimerChain>());
            }

            _timerNodes[id][timeChain.GetId()] = timeChain;
            
            return timeChain;
        }

        /// <summary>
        /// 同步循环 第一次触发时间为 interval + delay
        /// </summary>
        /// <param name="id">节点id</param>
        /// <param name="interval">循环间隔时间</param>
        /// <param name="action">行为</param>
        /// <param name="delay">延时</param>
        /// <param name="loopTimes">循环次数 默认为-1 无限循环</param>
        /// <returns></returns>
        public static TimerChain Loop(string id,int interval, Action action, int delay = 0, int loopTimes = -1)
        {
            var timeChain = new TimerChain(_timeWheel);
            
            void Callback()
            {
                Remove(id,timeChain.GetId());
                action?.Invoke();
            }

            timeChain.Loop(interval, Callback, delay, loopTimes);
            
            if (!_timerNodes.ContainsKey(id))
            {
                _timerNodes.Add(id,new Dictionary<int, TimerChain>());
            }

            _timerNodes[id][timeChain.GetId()] = timeChain;
            return timeChain;
        }

        /// <summary>
        /// 异步定时器
        /// </summary>
        /// <param name="id">节点id</param>
        /// <param name="delay">延时 单位毫秒</param>
        /// <param name="action">行为</param>
        /// <returns></returns>
        public static TimerChain OnceAsync(string id,int delay, Action action)
        {
            var timeChain = new TimerChain(_timeWheel);
            
            void Callback()
            {
                Remove(id,timeChain.GetId());
                action?.Invoke();
            }

            timeChain.OnceAsync(delay, Callback);
            
            if (!_timerNodes.ContainsKey(id))
            {
                _timerNodes.Add(id,new Dictionary<int, TimerChain>());
            }

            _timerNodes[id][timeChain.GetId()] = timeChain;
            
            return timeChain;
        }


        /// <summary>
        /// 异步循环 第一次触发时间为 interval + delay
        /// </summary>
        /// <param name="id">节点id</param>
        /// <param name="interval">循环间隔时间</param>
        /// <param name="action">行为</param>
        /// <param name="delay">延时</param>
        /// <param name="loopTimes">循环次数 默认为-1 无限循环</param>
        /// <returns></returns>
        public static TimerChain LoopAsync(string id,int interval, Action action, int delay = 0, int loopTimes = -1)
        {
            var timeChain = new TimerChain(_timeWheel);
            
            void Callback()
            {
                Remove(id,timeChain.GetId());
                action?.Invoke();
            }

            timeChain.LoopAsync(interval, Callback, delay, loopTimes);
            
            if (!_timerNodes.ContainsKey(id))
            {
                _timerNodes.Add(id,new Dictionary<int, TimerChain>());
            }

            _timerNodes[id][timeChain.GetId()] = timeChain;
            return timeChain;
        }

        public static TimerChain Clear(TimerChain chain)
        {
            chain.Clear();
            return chain;
        }

        private static void Remove(string id,int timerChainId)
        {
            _timerNodes[id].Remove(timerChainId);
        }

        /// <summary>
        /// 移除节点上的所有定时器
        /// </summary>
        /// <param name="id"></param>
        public static void ClearAll(string id)
        {
            if (_timerNodes.ContainsKey(id))
            {
                foreach (var data in _timerNodes[id])
                {
                    Clear(data.Value);
                }

                _timerNodes.Remove(id);
            }
        }

        public static void AddAction(int id, Action action)
        {
            _timerScript.AddAction(id, action);
        }
    }
}