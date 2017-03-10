namespace HP41CV.Models
{
    public class ProgramLink : ProgramEntry
    {
        private ProgramModel program;

        public ProgramLink(ProgramModel program, CalculatorModel model, string text) : base(model, text)
        {
            this.program = program;
        }

        public ProgramModel GetProgram()
        {
            return (program.GotoLable(Text, true)) ? program : null;
        }

        public override void Execute(bool noUpn)
        {
            Model.StartProgram(this);
        }
    }
}
