using System.ComponentModel;
using System.Runtime.CompilerServices;
using HP41CV.Annotations;

namespace HP41CV.Models
{
    public class Register : INotifyPropertyChanged
    {
        private string _name;
        private string _alpha;
        private double _value;

        public bool IsAlpha { get; private set; }

        public string Alpha
        {
            get { return _alpha; }
            set
            {
                IsAlpha = true;
                string help = value;
                if (help.Length > 6)
                    _alpha = help.Substring(0, 6);
                else
                    _alpha = help;
                OnPropertyChanged(nameof(Alpha));
                OnPropertyChanged(nameof(IsAlpha));
            }
        }

        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                IsAlpha = false; OnPropertyChanged(nameof(Value)); OnPropertyChanged(nameof(IsAlpha));
            }
        }

        public Register(string name)
        {
            _name = name;
            Clear();
        }

        public void Clear()
        {
            _value = 0;
            _alpha = string.Empty;
            IsAlpha = false;
        }

        public void Set(Register value)
        {
            _value = value._value;
            _alpha = value._alpha;
            IsAlpha = value.IsAlpha;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
