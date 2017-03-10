using System;

namespace HP41CV.Models
{
    public class ProgramEntry : ModelActionModel
    {
        public ProgramEntry(CalculatorModel model, string text) : base(model, text)
        {
        }

        public ProgramEntry(CalculatorModel model, Action action, string text) : base(model, action, text)
        {
        }

        public ProgramEntry(CalculatorModel model, Action action, Func<bool> validator, string text) : base(model, action, validator, text)
        {
        }
    }
}
