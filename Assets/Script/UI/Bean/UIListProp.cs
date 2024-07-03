using System;
using System.Collections.Generic;

namespace ReflectionUI
{
    public class UIListProp<T>
    {
        private List<T> _value;

        private List<T> val
        {
            get { return _value; }
            set
            {
                _value = value;
                Invoke();
            }
        }

        public int Count { get; set; }

        private Action<int> _onValueChange = null;

        public UIListProp()
        {
        }

        public UIListProp(List<T> value)
        {
            //this.val = value;
            _value = value;
        }

        public List<T> Get()
        {
            return val;
        }

        public void Set(List<T> value)
        {
            val = value;
            Count = val.Count;
        }

        public void Invoke()
        {
            if (val != null)
            {
                _onValueChange?.Invoke(val.Count);
            }
        }

        public override string ToString()
        {
            string res = "[";
            foreach (var item in val)
            {
                res += item.ToString() + ",";
            }

            res += "]";
            return res;
        }
    }
}
