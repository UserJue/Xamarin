using System.Collections.Generic;

namespace HP41CV.Models
{
    public class PrintEntry
    {
        private string _line;

        public PrintEntry(string line)
        {
            Line = line;
        }

        public IList<string> SaveEntry()
        {
            var result = new List<string> { Line };
            return result;
        }

        public string Line
        {
            get { return string.IsNullOrWhiteSpace(PlotLine) ? _line : PlotLine; }
            protected set { _line = value; }
        }

        public string PlotLine { get; set; }
    }
}
