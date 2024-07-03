using System;
using UnityEngine;

namespace ReflectionUI
{
    /// <summary>
    /// UI 颜色
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class UIColor : Attribute
    {
        public Color color;

        public UIColor(float r, float g, float b, float a)
        {
            color = new Color(r, g, b, a);
        }
    }
}