namespace HP41CV.Models
{
    public class ProgramStepExecuteModel : ProgramStepModel
    {
        private string lable;

        private ProgramModel program;

        private bool ind;
        private Register register;

        public ProgramStepExecuteModel(ProgramModel program, ActionModel model, string lable) : base(model)
        {
            isJump = true;
            this.ind = false;
            register = null;
            this.lable = lable;
            this.program = program;
        }

        public ProgramStepExecuteModel(ProgramModel program, ActionModel model, Register register) : base(model)
        {
            isJump = true;
            ind = true;
            this.register = register;
            this.program = program;
        }

        public override void Execute()
        {
            if (ind)
            {
                lable = register.IsAlpha ? register.Alpha : register.Value.ToString();
            }
            program.ExecuteLable(lable);
        }
    }
}
