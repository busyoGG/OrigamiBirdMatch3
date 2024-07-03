using System;

namespace ReflectionUI
{
    /// <summary>
    /// UI 选项
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class)]
    public class UIOptions: Attribute
    {
        private string[] _options;

        public UIOptions(params string[] options)
        {
            _options = options;
        }

        public string[] GetOptions()
        {
            return _options;
        }
    }
}