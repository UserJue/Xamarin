using System;

namespace HP41CV.Models
{
    public class DigitActionModel : InvokeActionModel
    {
        private int _digit;

        public DigitActionModel(int digit) : base(digit.ToString())
        {
            _digit = digit;
            IsFunction = false;
        }

        public DigitActionModel(string text, int digit) : base(text)
        {
            _digit = digit;
            GetValue = false;
            IsFunction = false;
        }

        public override void Execute(bool noUpn)
        {
            InvokeAction?.Invoke(_digit);
        }
    }
}
