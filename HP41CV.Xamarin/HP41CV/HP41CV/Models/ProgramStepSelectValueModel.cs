namespace HP41CV.Models
{
    public class ProgramStepSelectValueModel : ProgramStepModel
    {
        private readonly int value;

        private bool indirect;

        public ProgramStepSelectValueModel(SelectActionModel model, int value, bool indirect = false)
            : base(model)
        {
            this.value = value;
            this.indirect = indirect;
        }



        public override void Execute()
        {
            if (indirect)
                ((SelectActionModel)Model).ExecuteIndirect(value);
            else
                ((SelectActionModel)Model).Execute(value);
        }
    }
}
