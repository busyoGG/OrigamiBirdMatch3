using System;

namespace ReflectionUI
{
    public enum AdaptiveType
    {
        Width,
        Height,
        All
    }
    
    /// <summary>
    /// UI 自适应类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class)]
    public class UIAdaptive: Attribute
    {
        public AdaptiveType _type;

        public UIAdaptive(AdaptiveType type)
        {
            _type = type;
        }

        public AdaptiveType GetAdaptiveType()
        {
            return _type;
        }
    }
}