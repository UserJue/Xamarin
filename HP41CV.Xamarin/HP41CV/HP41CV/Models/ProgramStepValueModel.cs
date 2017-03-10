using System;

namespace HP41CV.Models
{
    public class ProgramStepValueModel : ProgramStepModel
    {
        public enum Actions
        {
            None,
            Value,
            AVIEW
        }

        protected ProgramStepValueModel.Actions Action;

        private readonly bool isText;
        private readonly double value;
        private readonly string text;
        private readonly CalculatorModel model;

        public ProgramStepValueModel(CalculatorModel model, double value)
            : base(null)
        {
            this.value = value;
            this.model = model;
            isText = false;
            Action = Actions.Value;
        }

        public ProgramStepValueModel(CalculatorModel model, string value)
            : base(null)
        {
            this.text = value;
            this.model = model;
            isText = true;
            Action = Actions.Value;
        }

        public ProgramStepValueModel(CalculatorModel model, ProgramStepValueModel.Actions action)
            : base(null)
        {
            this.model = model;
            Action = action;
        }

        public override void Execute()
        {
            switch (Action)
            {
                case Actions.None:
                    break;
                case Actions.Value:
                    if (isText)
                    {
                        if ((text[0] == '˫') || (text[0] == 'Ⱶ'))
                            model.AddAlpha(text.Substring(1));
                        else
                            model.SetAlpha(text);
                    }
                    else if (model.NoStackLift)
                        model.X = value;
                    else
                        model.StackDown(value);
                    break;
                case Actions.AVIEW:
                    model.Aview();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
