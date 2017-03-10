using System.ComponentModel;
using System.Runtime.CompilerServices;
using HP41CV.Annotations;

namespace HP41CV.ViewModels
{
    public class ByteViewModel : INotifyPropertyChanged
    {
        private readonly byte value;
        private readonly int pos;

        public string Fill0
        {
            get
            {
                return ((this.value & 0x01) == 0x01) ? "Black" : "White";
            }
        }

        public string Fill1
        {
            get
            {
                return ((this.value & 0x02) == 0x02) ? "Black" : "White";
            }
        }

        public string Fill2
        {
            get
            {
                return ((this.value & 0x04) == 0x04) ? "Black" : "White";
            }
        }

        public string Fill3
        {
            get
            {
                return ((this.value & 0x08) == 0x08) ? "Black" : "White";
            }
        }

        public string Fill4
        {
            get
            {
                return ((this.value & 0x10) == 0x10) ? "Black" : "White";
            }
        }

        public string Fill5
        {
            get
            {
                return ((this.value & 0x20) == 0x20) ? "Black" : "White";
            }
        }

        public string Fill6
        {
            get
            {
                return ((this.value & 0x40) == 0x40) ? "Black" : "White";
            }
        }

        public string Fill7
        {
            get
            {
                return ((this.value & 0x80) == 0x80) ? "Black" : "White";
            }
        }

        public string Bit
        {
            get
            {
                return (pos * 8).ToString();
            }
        }

        public double Width { get; private set; }

        public ByteViewModel(byte value, int pos, bool small)
        {
            this.pos = pos;
            this.value = value;
            Width = small ? 55 : 100;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
