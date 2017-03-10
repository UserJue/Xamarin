using System;

namespace HP41CV.Models
{
    public class ProgramActionModel : ModelActionModel
    {
        #region Constants and Fields

        private readonly Action<string> _action;

        #endregion

        #region Constructors and Destructors

        public ProgramActionModel(CalculatorModel model, Action<string> action, string text)
            : base(model, text)
        {
            this._action = action;
            IsRegister = true;
        }

        #endregion

        #region Public Methods

        public void Execute(string value)
        {
            if (_action != null)
            {
                this._action(value);
            }
        }

        #endregion
    }
}
