using System;

namespace ReflectionUI
{
    public class StringUIProp
    {
        public string _value;

        private string val
        {
            get { return _value; }
            set
            {
                _value = value;
                InvokeUI();
            }
        }

        private Action<string> _onValueChange = null;
        private Func<string> _onUIChange = null;

        public StringUIProp()
        {
        }

        public StringUIProp(string value)
        {
            _value = value;
        }

        public void Set(string value)
        {
            this.val = value;
        }

        public void Set<T>(T value)
        {
            this.val = value.ToString();
        }

        public string Get()
        {
            InvokeValue();
            return this.val;
        }

        public void InvokeValue()
        {
            if (_onUIChange != null)
            {
                _value = _onUIChange.Invoke();
            }
        }

        public void InvokeUI()
        {
            _onValueChange?.Invoke(val);
        }

        public override string ToString()
        {
            return val;
        }
    }
}