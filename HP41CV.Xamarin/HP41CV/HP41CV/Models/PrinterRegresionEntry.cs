namespace HP41CV.Models
{
    public class PrinterRegresionEntry : PrintEntry
    {
        public double Ax { get; set; }
        public double Ay { get; set; }
        public double Bx { get; set; }
        public double By { get; set; }
        public double R2 { get; set; }
        public double Xmin { get; set; }
        public double Xmax { get; set; }
        public double Ymin { get; set; }
        public double Ymax { get; set; }

        public PrinterRegresionEntry() : base("")
        {

        }
    }
}
