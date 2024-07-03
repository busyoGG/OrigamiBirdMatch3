using System;

namespace ReflectionUI
{
    /// <summary>
    /// 绑定 UI 监听
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class UIListenerBind : Attribute
    {
        public string _name;

        public UIListenerBind(string name)
        {
            _name = name;
        }
    }
}