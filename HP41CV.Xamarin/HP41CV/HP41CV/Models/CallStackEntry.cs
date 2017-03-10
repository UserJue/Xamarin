namespace HP41CV.Models
{
    public class CallStackEntry
    {
        public ProgramModel Program { get; private set; }
        public int Step { get; private set; }

        public CallStackEntry(ProgramModel program, int step)
        {
            Program = program;
            Step = step;
        }
    }
}
