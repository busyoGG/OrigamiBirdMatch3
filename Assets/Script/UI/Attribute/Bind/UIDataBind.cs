using System;

namespace ReflectionUI
{
    /// <summary>
    /// 绑定 UI 组件和数据，使其根据数据修改UI
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class UIDataBind : Attribute
    {
        public UIType _type;
        public string _path;

        public UIDataBind(UIType type, string path)
        {
            _type = type;
            _path = path;
        }
    }
}