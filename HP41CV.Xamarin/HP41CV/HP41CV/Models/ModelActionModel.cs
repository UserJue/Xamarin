using System;

namespace HP41CV.Models
{
    public class ModelActionModel : ActionModel
    {
        protected CalculatorModel Model;
        protected Action Action;
        protected readonly Func<bool> Validator;
        protected bool LastX { get; set; }

        public ModelActionModel(CalculatorModel model, string text)
            : base(text)
        {
            Model = model;
            Action = null;
            Validator = null;
            LastX = false;
        }

        public ModelActionModel(CalculatorModel model, Action action, string text)
            : base(text)
        {
            Model = model;
            Action = action;
            Validator = null;
            LastX = false;
        }

        public ModelActionModel(CalculatorModel model, Action action, Func<bool> validator, string text)
            : base(text)
        {
            Model = model;
            Action = action;
            Validator = validator;
            LastX = false;
        }

        public override void Execute(bool noUpn)
        {
            if (CanExecute())
            {
                if (Action != null)
                    Action.Invoke();
                //else
                //    Messenger.ShowDialogAsync(Resources.GetString("NotYet"));
            }
        }

        protected override bool CanExecute()
        {
            if (base.CanExecute())
            {
                return true;
            }
            return false;
        }
    }
}
