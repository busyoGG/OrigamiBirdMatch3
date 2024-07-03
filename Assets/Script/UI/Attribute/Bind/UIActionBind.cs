using System;

namespace ReflectionUI
{
    /// <summary>
    /// 绑定 UI 组件和动作事件，使其支持交互
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class UIActionBind : Attribute
    {
        public UIAction _type;

        public string _path;

        public UIActionBind(UIAction type, string path)
        {
            _type = type;
            _path = path;
        }
    }
}