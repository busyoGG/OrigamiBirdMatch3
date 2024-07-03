using System;
using UnityEngine;

namespace ReflectionUI
{
    public enum TweenEaseType
    {
        Linear,
        SineIn,
        SineOut,
        SineInOut,
        QuadIn,
        QuadOut,
        QuadInOut,
        CubicIn,
        CubicOut,
        CubicInOut,
        QuartIn,
        QuartOut,
        QuartInOut,
        QuintIn,
        QuintOut,
        QuintInOut,
        ExpoIn,
        ExpoOut,
        ExpoInOut,
        CircIn,
        CircOut,
        CircInOut,
        ElasticIn,
        ElasticOut,
        ElasticInOut,
        BackIn,
        BackOut,
        BackInOut,
        BounceIn,
        BounceOut,
        BounceInOut,
        Custom
    }

    public class EaseUtil
    {
        const float _PiOver2 = Mathf.PI * 0.5f;
        const float _TwoPi = Mathf.PI * 2;

        internal static float Evaluate(TweenEaseType TweenEaseType, float time, float duration,
            float overshootOrAmplitude,
            float period)
        {
            if (duration <= 0)
                return 1;

            switch (TweenEaseType)
            {
                case TweenEaseType.Linear:
                    return time / duration;
                case TweenEaseType.SineIn:
                    return -(float)Math.Cos(time / duration * _PiOver2) + 1;
                case TweenEaseType.SineOut:
                    return (float)Math.Sin(time / duration * _PiOver2);
                case TweenEaseType.SineInOut:
                    return -0.5f * ((float)Math.Cos(Mathf.PI * time / duration) - 1);
                case TweenEaseType.QuadIn:
                    return (time /= duration) * time;
                case TweenEaseType.QuadOut:
                    return -(time /= duration) * (time - 2);
                case TweenEaseType.QuadInOut:
                    if ((time /= duration * 0.5f) < 1) return 0.5f * time * time;
                    return -0.5f * ((--time) * (time - 2) - 1);
                case TweenEaseType.CubicIn:
                    return (time /= duration) * time * time;
                case TweenEaseType.CubicOut:
                    return ((time = time / duration - 1) * time * time + 1);
                case TweenEaseType.CubicInOut:
                    if ((time /= duration * 0.5f) < 1) return 0.5f * time * time * time;
                    return 0.5f * ((time -= 2) * time * time + 2);
                case TweenEaseType.QuartIn:
                    return (time /= duration) * time * time * time;
                case TweenEaseType.QuartOut:
                    return -((time = time / duration - 1) * time * time * time - 1);
                case TweenEaseType.QuartInOut:
                    if ((time /= duration * 0.5f) < 1) return 0.5f * time * time * time * time;
                    return -0.5f * ((time -= 2) * time * time * time - 2);
                case TweenEaseType.QuintIn:
                    return (time /= duration) * time * time * time * time;
                case TweenEaseType.QuintOut:
                    return ((time = time / duration - 1) * time * time * time * time + 1);
                case TweenEaseType.QuintInOut:
                    if ((time /= duration * 0.5f) < 1) return 0.5f * time * time * time * time * time;
                    return 0.5f * ((time -= 2) * time * time * time * time + 2);
                case TweenEaseType.ExpoIn:
                    return (time == 0) ? 0 : (float)Math.Pow(2, 10 * (time / duration - 1));
                case TweenEaseType.ExpoOut:
                    if (time == duration) return 1;
                    return (-(float)Math.Pow(2, -10 * time / duration) + 1);
                case TweenEaseType.ExpoInOut:
                    if (time == 0) return 0;
                    if (time == duration) return 1;
                    if ((time /= duration * 0.5f) < 1) return 0.5f * (float)Math.Pow(2, 10 * (time - 1));
                    return 0.5f * (-(float)Math.Pow(2, -10 * --time) + 2);
                case TweenEaseType.CircIn:
                    return -((float)Math.Sqrt(1 - (time /= duration) * time) - 1);
                case TweenEaseType.CircOut:
                    return (float)Math.Sqrt(1 - (time = time / duration - 1) * time);
                case TweenEaseType.CircInOut:
                    if ((time /= duration * 0.5f) < 1) return -0.5f * ((float)Math.Sqrt(1 - time * time) - 1);
                    return 0.5f * ((float)Math.Sqrt(1 - (time -= 2) * time) + 1);
                case TweenEaseType.ElasticIn:
                    float s0;
                    if (time == 0) return 0;
                    if ((time /= duration) == 1) return 1;
                    if (period == 0) period = duration * 0.3f;
                    if (overshootOrAmplitude < 1)
                    {
                        overshootOrAmplitude = 1;
                        s0 = period / 4;
                    }
                    else s0 = period / _TwoPi * (float)Math.Asin(1 / overshootOrAmplitude);

                    return -(overshootOrAmplitude * (float)Math.Pow(2, 10 * (time -= 1)) *
                             (float)Math.Sin((time * duration - s0) * _TwoPi / period));
                case TweenEaseType.ElasticOut:
                    float s1;
                    if (time == 0) return 0;
                    if ((time /= duration) == 1) return 1;
                    if (period == 0) period = duration * 0.3f;
                    if (overshootOrAmplitude < 1)
                    {
                        overshootOrAmplitude = 1;
                        s1 = period / 4;
                    }
                    else s1 = period / _TwoPi * (float)Math.Asin(1 / overshootOrAmplitude);

                    return (overshootOrAmplitude * (float)Math.Pow(2, -10 * time) *
                        (float)Math.Sin((time * duration - s1) * _TwoPi / period) + 1);
                case TweenEaseType.ElasticInOut:
                    float s;
                    if (time == 0) return 0;
                    if ((time /= duration * 0.5f) == 2) return 1;
                    if (period == 0) period = duration * (0.3f * 1.5f);
                    if (overshootOrAmplitude < 1)
                    {
                        overshootOrAmplitude = 1;
                        s = period / 4;
                    }
                    else s = period / _TwoPi * (float)Math.Asin(1 / overshootOrAmplitude);

                    if (time < 1)
                        return -0.5f * (overshootOrAmplitude * (float)Math.Pow(2, 10 * (time -= 1)) *
                                        (float)Math.Sin((time * duration - s) * _TwoPi / period));
                    return overshootOrAmplitude * (float)Math.Pow(2, -10 * (time -= 1)) *
                        (float)Math.Sin((time * duration - s) * _TwoPi / period) * 0.5f + 1;
                case TweenEaseType.BackIn:
                    return (time /= duration) * time * ((overshootOrAmplitude + 1) * time - overshootOrAmplitude);
                case TweenEaseType.BackOut:
                    return ((time = time / duration - 1) * time *
                        ((overshootOrAmplitude + 1) * time + overshootOrAmplitude) + 1);
                case TweenEaseType.BackInOut:
                    if ((time /= duration * 0.5f) < 1)
                        return 0.5f * (time * time *
                                       (((overshootOrAmplitude *= (1.525f)) + 1) * time - overshootOrAmplitude));
                    return 0.5f * ((time -= 2) * time *
                        (((overshootOrAmplitude *= (1.525f)) + 1) * time + overshootOrAmplitude) + 2);
                case TweenEaseType.BounceIn:
                    return Bounce.EaseIn(time, duration);
                case TweenEaseType.BounceOut:
                    return Bounce.EaseOut(time, duration);
                case TweenEaseType.BounceInOut:
                    return Bounce.EaseInOut(time, duration);

                default:
                    return -(time /= duration) * (time - 2);
            }
        }
    }

    /// <summary>
    /// This class contains a C# port of the easing equations created by Robert Penner (http://robertpenner.com/easing).
    /// </summary>
    static class Bounce
    {
        /// <summary>
        /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in: accelerating from zero velocity.
        /// </summary>
        /// <param name="time">
        /// Current time (in frames or seconds).
        /// </param>
        /// <param name="duration">
        /// Expected easing duration (in frames or seconds).
        /// </param>
        /// <returns>
        /// The eased value.
        /// </returns>
        public static float EaseIn(float time, float duration)
        {
            return 1 - EaseOut(duration - time, duration);
        }

        /// <summary>
        /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing out: decelerating from zero velocity.
        /// </summary>
        /// <param name="time">
        /// Current time (in frames or seconds).
        /// </param>
        /// <param name="duration">
        /// Expected easing duration (in frames or seconds).
        /// </param>
        /// <returns>
        /// The eased value.
        /// </returns>
        public static float EaseOut(float time, float duration)
        {
            if ((time /= duration) < (1 / 2.75f))
            {
                return (7.5625f * time * time);
            }

            if (time < (2 / 2.75f))
            {
                return (7.5625f * (time -= (1.5f / 2.75f)) * time + 0.75f);
            }

            if (time < (2.5f / 2.75f))
            {
                return (7.5625f * (time -= (2.25f / 2.75f)) * time + 0.9375f);
            }

            return (7.5625f * (time -= (2.625f / 2.75f)) * time + 0.984375f);
        }

        /// <summary>
        /// Easing equation function for a bounce (exponentially decaying parabolic bounce) easing in/out: acceleration until halfway, then deceleration.
        /// </summary>
        /// <param name="time">
        /// Current time (in frames or seconds).
        /// </param>
        /// <param name="duration">
        /// Expected easing duration (in frames or seconds).
        /// </param>
        /// <returns>
        /// The eased value.
        /// </returns>
        public static float EaseInOut(float time, float duration)
        {
            if (time < duration * 0.5f)
            {
                return EaseIn(time * 2, duration) * 0.5f;
            }

            return EaseOut(time * 2 - duration, duration) * 0.5f + 0.5f;
        }
    }
}