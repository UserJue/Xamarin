namespace HP41CV.Models
{
    public class ProgramStepModel
    {
        protected bool isJump = false;
        protected ActionModel Model;

        public ProgramStepModel(ActionModel model)
        {
            this.Model = model;
        }

        public bool IsJump { get { return isJump; } }

        public bool IsCondition { get { return (Model != null) && Model.IsCondition; } }

        public virtual void Execute()
        {
            if (Model != null)
                Model.Execute();
        }
    }
}
