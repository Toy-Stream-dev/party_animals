using System;
using _Idle.Scripts.Tools;
using UnityEngine;

namespace _Idle.Scripts.Model.Numbers
{
    [Serializable]
    public class BigNumber
    {
        public event Action UpdatedEvent;

        [SerializeField] private double _sourceValue;
        [SerializeField] private double _value;
        [SerializeField] private double _boostK = 1f;
        private double _delta;

        public double Value => _value;
        public double Delta => _delta;
        public double SourceValue => _sourceValue;
        public int IntValue => (int)Math.Round(Value);
        public int BoostK => (int)_boostK;

        public BigNumber()
        {
        }

        public BigNumber(double value)
        {
            Init(value);
        }

        protected void Init(double value)
        {
            SetSourceValue(value);
            _value = _sourceValue * _boostK;
        }

        public virtual void SetValue(double value)
        {
            SetSourceValue(value);
            _value = _sourceValue * _boostK;
            UpdatedEvent?.Invoke();
        }

        public virtual double GetValue(double value)
        {
            return value * _boostK;
        }

        public void Change(BigNumber delta)
        {
            Change(delta.Value);
        }

        public virtual void Change(double delta)
        {
            SetSourceValue(_sourceValue + delta);
            _delta = delta;
            _value += delta;
            UpdatedEvent?.Invoke();
        }

        private void SetSourceValue(double value)
        {
            _sourceValue = value;
        }

        public void CopyFrom(BigNumber source)
        {
            if (!_sourceValue.EqualTo(source._sourceValue))
            {
                SetValue(source._sourceValue);
            }
        }

        // public override string ToString()
        // {
        //     return Value.ToFormattedString();
        // }
        
        public static implicit operator BigNumber(double d) => new BigNumber(d);
    }
}
