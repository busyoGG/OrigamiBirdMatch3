using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ReflectionUI
{
    public class UITweenManager : Singleton<UITweenManager>
    {
        private TweenUpdater _updater;

        private int _id = 0;

        public void Init()
        {
            GameObject obj = new GameObject();
            obj.name = "TweenUpdater";
            _updater = obj.AddComponent<TweenUpdater>();
        }

        public int AddTween(UGUIData obj, TweenTarget target, float end, int duration,
            TweenEaseType ease = TweenEaseType.Linear, Action callback = null)
        {
            UITween vt = new UITween();
            vt.id = _id++;
            vt.duration = duration;
            vt.updater = GenerateUpdater(obj, target, end, duration, ease);
            vt.callback = callback;

            _updater.AddTween(vt);

            return vt.id;
        }

        public int AddTween(UGUIData obj, TweenTarget target, Vector2 end, int duration,
            TweenEaseType ease = TweenEaseType.Linear, Action callback = null)
        {
            UITween vt = new UITween();
            vt.id = _id++;
            vt.duration = duration;
            vt.updater = GenerateUpdater(obj, target, end, duration, ease);
            vt.callback = callback;

            _updater.AddTween(vt);
            return vt.id;
        }

        public void StopTween(int id)
        {
            _updater.StopTween(id);
        }

        private Action<float> GenerateUpdater(UGUIData obj, TweenTarget target, float end, float duration,
            TweenEaseType ease)
        {
            float origin = 0;
            switch (target)
            {
                case TweenTarget.X:
                    origin = obj.x;
                    break;
                case TweenTarget.Y:
                    origin = obj.y;
                    break;
                case TweenTarget.ScaleX:
                    origin = obj.scaleX;
                    break;
                case TweenTarget.ScaleY:
                    origin = obj.scaleY;
                    break;
                case TweenTarget.Rotation:
                    origin = obj.rotation;
                    break;
                case TweenTarget.Alpha:
                    origin = obj.alpha;
                    break;
                case TweenTarget.Heihgt:
                    origin = obj.height;
                    break;
                case TweenTarget.Width:
                    origin = obj.width;
                    break;
            }

            void Action(float time)
            {
                float ratio = EaseUtil.Evaluate(ease, time, duration, 1.7f, 0);
                switch (target)
                {
                    case TweenTarget.X:
                        obj.x = origin + ratio * end;
                        break;
                    case TweenTarget.Y:
                        obj.y = origin + ratio * end;
                        break;
                    case TweenTarget.ScaleX:
                        obj.scaleX = origin + ratio * end;
                        break;
                    case TweenTarget.ScaleY:
                        obj.scaleY = origin + ratio * end;
                        break;
                    case TweenTarget.Rotation:
                        obj.rotation = origin + ratio * end;
                        break;
                    case TweenTarget.Alpha:
                        obj.alpha = origin + ratio * end;
                        break;
                    case TweenTarget.Heihgt:
                        obj.height = origin + ratio * end;
                        break;
                    case TweenTarget.Width:
                        obj.width = origin + ratio * end;
                        break;
                }
            }

            return Action;
        }

        private Action<float> GenerateUpdater(UGUIData obj, TweenTarget target, Vector2 end, float duration,
            TweenEaseType ease)
        {
            Vector2 origin = Vector2.zero;
            switch (target)
            {
                case TweenTarget.Position:
                    origin = obj.xy;
                    break;
                case TweenTarget.Scale:
                    origin = obj.scale;
                    break;
                case TweenTarget.Size:
                    origin = obj.size;
                    break;
            }

            void Action(float time)
            {
                float ratio = EaseUtil.Evaluate(ease, time, duration, 1.7f, 0);
                switch (target)
                {
                    case TweenTarget.Position:
                        obj.xy = origin + ratio * end;
                        break;
                    case TweenTarget.Scale:
                        obj.scale = origin + ratio * end;
                        break;
                    case TweenTarget.Size:
                        obj.size = origin + ratio * end;
                        break;
                }
            }

            return Action;
        }
    }
}