using System;

namespace HP41CV.Models
{
    public class BinaryOperationPModel : BinaryOperationModel
    {
        public BinaryOperationPModel(CalculatorModel model, Func<double, double, double> operation, string text) : base(model, operation, text)
        {
        }

        public BinaryOperationPModel(CalculatorModel model, Func<double, double, double> operation, Func<bool> validator, string text) : base(model, operation, validator, text)
        {
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
                    Model.X = Operation(Model.X, Model.Y);
                }
                else
                {
                    Model.AddOperator(this);
                }
            }
        }
    }
}
