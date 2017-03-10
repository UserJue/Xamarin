using System;

namespace HP41CV.Models
{
    public class SelectActionModel : ModelActionModel
    {
        #region Constants and Fields

        protected new readonly Func<int, bool> Validator;

        private readonly Action<int> _action;

        #endregion

        #region Constructors and Destructors

        public SelectActionModel(CalculatorModel model, Action<int> action, bool select2, string text)
            : base(model, text)
        {
            Select2 = select2;
            Select3 = false;
            this._action = action;
            IsRegister = true;
            IsFunction = false;
            IsSelectFunction = true;
        }

        public SelectActionModel(CalculatorModel model, Action<int> action, Func<int, bool> validator, bool select2,
            string text)
            : base(model, text)
        {
            this.Validator = validator;
            Select2 = select2;
            Select3 = false;
            this._action = action;
            IsRegister = true;
            IsFunction = false;
            IsSelectFunction = true;
        }

        #endregion

        #region Properties

        public bool Select2 { get; private set; }

        public bool Select3 { get; set; }

        #endregion

        #region Public Methods

        public override void Execute(bool noUpn)
        {
            if (CanExecute() && (Model != null))
            {
                Model.SelectAction = this;
            }
        }

        public void Execute(int value)
        {
            if ((this.Validator != null) && !this.Validator(value))
            {
                return;
            }

            if (_action != null)
            {
                this._action(value);
            }
        }

        public void ExecuteIndirect(int register)
        {
            if (!Model.IsRegister(register)) return;
            var value = Model.Indirect(register);
            if ((this.Validator != null) && !this.Validator(value))
            {
                return;
            }

            if (_action != null)
            {
                this._action(value);
            }
        }

        #endregion
    }
}
