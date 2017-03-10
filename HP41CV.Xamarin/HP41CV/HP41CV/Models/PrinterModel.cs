using System;
using System.Collections.ObjectModel;

namespace HP41CV.Models
{
    public class PrinterModel
    {
        private bool _on;
        private Action printAction;
        private string printBuffer = string.Empty;
        private byte[] printCols = new byte[168];
        private int actPrintCol;
        private const string spaces = "                                            ";

        public enum Mode
        {
            MAN,
            TRACE,
            NORM
        }

        public Mode PrintMode { get; set; }
        public Collection<PrintEntry> Entrys { get; private set; }

        public PrinterModel(Action printAction)
        {
            this.printAction = printAction;
            Entrys = new Collection<PrintEntry>();
            On = true;
            PrintMode = Mode.MAN;
        }

        public bool On
        {
            get { return _on; }
            set { _on = value; }
        }

        public void PrintBuffer(bool right = false)
        {
        }

        public void PrintMan(string line, bool right = false, bool man = true, bool marker = true)
        {
        }

        public void Print(string line)
        {
        }

        public void AddToPrintBuffer(string value)
        {
        }

        public void AddColumn(byte value)
        {
        }

        public void SkipColumns(int value)
        {
        }

        public void PlotLine(double x, double min, double max, int size, double zero)
        {
        }

        public void Print(string line, Mode mode, bool right = false) { }

    }
}
