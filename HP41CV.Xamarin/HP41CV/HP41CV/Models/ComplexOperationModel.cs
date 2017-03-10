using System;

namespace HP41CV.Models
{
    public class ComplexOperationModel : ModelActionModel
    {
        private readonly Func<double, double, double> operation1;
        private readonly Func<double, double, double> operation2;

        public bool ToAngle { get; set; }
        public bool FromAngle { get; set; }

        public ComplexOperationModel(CalculatorModel model, Func<double, double, double> operation1, Func<double, double, double> operation2, string text)
            : base(model, text)
        {
            this.operation1 = operation1;
            this.operation2 = operation2;
            LastX = true;
            IsArithmetic = true;
            ToAngle = false;
            FromAngle = false;
            HasResult = true;
        }

        public ComplexOperationModel(CalculatorModel model, Func<double, double, double> operation1, Func<double, double, double> operation2, Func<bool> validator, string text)
            : base(model, null, validator, text)
        {
            this.operation1 = operation1;
            this.operation2 = operation2;
            LastX = true;
            IsArithmetic = true;
            ToAngle = false;
            FromAngle = false;
            HasResult = true;
        }

        public override void Execute(bool noUpn)
        {
            if ((Validator != null) && !Validator())
                return;
            if (LastX)
                Model.LastX = Model.X;
            var x1 = Model.StackUp();
            var x2 = Model.X;
            if (FromAngle)
            {
                if (Model.IsFlagSet(42))
                {
                    x2 *= Math.PI / 200.0;
                }
                else if (!Model.IsFlagSet(43))
                {
                    x2 *= Math.PI / 180.0;
                }
            }
            var y = this.operation1(x1, x2);
            if (ToAngle)
            {
                if (Model.IsFlagSet(42))
                {
                    y *= 200.0 / Math.PI;
                }
                else if (!Model.IsFlagSet(43))
                {
                    y *= 180.0 / Math.PI;
                }
            }
            Model.X = y;
            Model.StackDown(this.operation2(x1, x2));
        }

    }
}
