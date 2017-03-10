using System;

namespace HP41CV.Models
{
    public class BinaryOperationModel : ModelActionModel
    {
        protected readonly Func<double, double, double> Operation;

        public BinaryOperationModel(CalculatorModel model, Func<double, double, double> operation, string text)
            : base(model, text)
        {
            this.Operation = operation;
            LastX = true;
            IsArithmetic = true;
            HasResult = true;
        }

        public BinaryOperationModel(CalculatorModel model, Func<double, double, double> operation, Func<bool> validator, string text)
            : base(model, null, validator, text)
        {
            this.Operation = operation;
            LastX = true;
            IsArithmetic = true;
            HasResult = true;
        }

        public override void Execute(bool noUpn)
        {
            if (CanExecute())
            {
                if (Model.IsUpn || noUpn)
                {
                    if ((Validator != null) && !Validator())
                        return;
                    if (LastX)
                        Model.LastX = Model.X;
                    var help = Model.StackUp();
                    Model.X = this.Operation(Model.X, help);
                }
                else
                {
                    Model.AddOperator(this);
                }
            }
        }
    }
}
