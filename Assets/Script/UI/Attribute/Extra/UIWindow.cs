using System;

namespace ReflectionUI
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class)]
    public class UIWindow: Attribute
    {
        private bool _reTop;

        private string _operateItemPath;

        public UIWindow(bool reTop, string operateItemPath = "")
        {
            _reTop = reTop;
            _operateItemPath = operateItemPath;
        }

        public bool IsReTop()
        {
            return _reTop;
        }

        public string GetOperateItemPath()
        {
            return _operateItemPath;
        }
    }
}