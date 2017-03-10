using System;

namespace HP41CV.Models
{
    public class UnaryOperationModel : ModelActionModel
    {
        protected readonly Func<double, double> Operation;

        public UnaryOperationModel(CalculatorModel model, Func<double, double> operation, string text)
            : base(model, text)
        {
            this.Operation = operation;
            LastX = true;
            IsArithmetic = true;
            IsUpn = false;
            HasResult = true;
        }

        public UnaryOperationModel(CalculatorModel model, Func<double, double> operation, Func<bool> validator, string text)
            : base(model, null, validator, text)
        {
            this.Operation = operation;
            LastX = true;
            IsArithmetic = true;
            IsUpn = false;
            HasResult = true;
        }

        public override void Execute(bool noUpn)
        {
            if (!CanExecute())
            {
                return;
            }

            if (Model.IsUpn || noUpn)
            {
                if ((Validator != null) && !Validator())
                {
                    return;
                }

                if (LastX)
                {
                    Model.LastX = Model.X;
                }

                Model.X = this.Operation(Model.X);
            }
            else
            {
                Model.AddOperator(this);
            }
        }
    }
}
