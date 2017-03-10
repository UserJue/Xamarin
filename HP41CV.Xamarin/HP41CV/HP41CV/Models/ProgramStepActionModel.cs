namespace HP41CV.Models
{
    public class ProgramStepActionModel : ProgramStepModel
    {
        private readonly string value;

        public ProgramStepActionModel(ProgramActionModel model, string value)
            : base(model)
        {
            this.value = value;
        }

        public override void Execute()
        {
            ((ProgramActionModel)Model).Execute(value);
        }
    }
}
