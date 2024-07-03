using System;

namespace ReflectionUI
{
    /// <summary>
    /// UI 条件判定
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class)]
    public class UICondition: Attribute
    {
        private string _strCondition;

        private bool _boolCondition;

        private int _intCondition;

        private float _floatCondition;

        private double _doubleCondition;

        public UICondition(string condition)
        {
            _strCondition = condition;
        }

        public UICondition(int condition)
        {
            _intCondition = condition;
        }

        public UICondition(bool condition)
        {
            _boolCondition = condition;
        }

        public UICondition(float condition)
        {
            _floatCondition = condition;
        }

        public UICondition(double condition)
        {
            _doubleCondition = condition;
        }

        public string GetString()
        {
            return _strCondition;
        }

        public bool GetBool()
        {
            return _boolCondition;
        }

        public int GetInt()
        {
            return _intCondition;
        }

        public float GetFloat()
        {
            return _floatCondition;
        }

        public double GetDouble()
        {
            return _doubleCondition;
        }
    }
}