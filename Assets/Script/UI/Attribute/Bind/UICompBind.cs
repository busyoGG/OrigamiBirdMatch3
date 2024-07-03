using System;

namespace ReflectionUI
{
    /// <summary>
    /// 绑定 UI 组件
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UICompBind : Attribute
    {
        public UIType _type;
        public string _path;

        public UICompBind(UIType type, string path)
        {
            _type = type;
            _path = path;
        }
    }
}