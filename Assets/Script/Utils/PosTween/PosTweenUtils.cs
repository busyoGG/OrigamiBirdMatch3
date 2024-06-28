using System;
using UnityEngine;

namespace PosTween
{
    public class PosTweenUtils
    {
        private static TweenUpdater _updater;
        
        public static void Init()
        {
            GameObject tween = new GameObject();
            tween.name = "PosTween";

            _updater = tween.AddComponent<TweenUpdater>();
        }
        
        public static void Move(GridScript grid,Vector3 start, Vector3 end,int duration,int delay = 0,Action callback = null)
        {
            Vector3 diff = end - start;

            TweenData tweenData = new TweenData(grid,duration,delay,start,diff,callback);
            
            _updater.Add(tweenData);
        }
    }
}