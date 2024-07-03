using System;

namespace ReflectionUI
{
    public class DoubleUIProp
    {
        public double _value;

        private double val
        {
            get { return _value; }
            set
            {
                _value = value;
                InvokeUI();
            }
        }

        private Action<double> _onValueChange = null;
        private Func<double> _onUIChange = null;

        public DoubleUIProp()
        {
        }

        public DoubleUIProp(double value)
        {
            _value = value;
        }

        public void Set(double value)
        {
            this.val = value;
        }

        public double Get()
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
            return val.ToString();
        }
    }
}