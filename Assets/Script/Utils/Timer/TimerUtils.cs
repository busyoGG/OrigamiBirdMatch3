using System;
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
        /// <param name="delay">延时 单位毫秒</param>
        /// <param name="action">行为</param>
        /// <returns></returns>
        public static TimerChain Once(int delay, Action action)
        {
            return new TimerChain(_timeWheel).Once(delay, action);
        }

        /// <summary>
        /// 同步循环 第一次触发时间为 interval + delay
        /// </summary>
        /// <param name="interval">循环间隔时间</param>
        /// <param name="action">行为</param>
        /// <param name="delay">延时</param>
        /// <param name="loopTimes">循环次数 默认为-1 无限循环</param>
        /// <returns></returns>
        public static TimerChain Loop(int interval, Action action, int delay = 0, int loopTimes = -1)
        {
            return new TimerChain(_timeWheel).Loop(interval, action, delay, loopTimes);
        }

        /// <summary>
        /// 异步定时器
        /// </summary>
        /// <param name="delay">延时 单位毫秒</param>
        /// <param name="action">行为</param>
        /// <returns></returns>
        public static TimerChain OnceAsync(int delay, Action action)
        {
            return new TimerChain(_timeWheel).OnceAsync(delay, action);
        }


        /// <summary>
        /// 异步循环 第一次触发时间为 interval + delay
        /// </summary>
        /// <param name="interval">循环间隔时间</param>
        /// <param name="action">行为</param>
        /// <param name="delay">延时</param>
        /// <param name="loopTimes">循环次数 默认为-1 无限循环</param>
        /// <returns></returns>
        public static TimerChain LoopAsync(int interval, Action action, int delay = 0, int loopTimes = -1)
        {
            return new TimerChain(_timeWheel).LoopAsync(interval, action, delay, loopTimes);
        }

        public static TimerChain Clear(TimerChain chain)
        {
            chain.Clear();
            return chain;
        }

        public static void AddAction(int id, Action action)
        {
            _timerScript.AddAction(id, action);
        }
    }
}