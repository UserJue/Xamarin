using System;

namespace HP41CV.Models
{
    public class TrigeometricOperatorModel : UnaryOperationModel
    {
        #region Constants and Fields

        private readonly bool _invers;

        #endregion

        #region Constructors and Destructors

        public TrigeometricOperatorModel(CalculatorModel model, Func<double, double> operation, string text)
            : base(model, operation, text)
        {
            this._invers = false;
            IsTrigonometric = true;
        }

        public TrigeometricOperatorModel(CalculatorModel model, Func<double, double> operation, bool invers, string text)
            : base(model, operation, text)
        {
            this._invers = invers;
            IsTrigonometric = true;
        }

        public TrigeometricOperatorModel(CalculatorModel model, Func<double, double> operation, bool invers, Func<bool> validator, string text)
            : base(model, operation, validator, text)
        {
            this._invers = invers;
            IsTrigonometric = true;
        }

        #endregion

        #region Public Methods

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

                var x = Model.X;
                if (!this._invers)
                {
                    if (Model.IsFlagSet(42))
                    {
                        x *= Math.PI / 200.0;
                    }
                    else if (!Model.IsFlagSet(43))
                    {
                        x *= Math.PI / 180.0;
                    }
                }

                var y = Operation(x);
                if (this._invers)
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
            }
            else
            {
                Model.AddOperator(this);
            }
        }

        #endregion
    }

}
