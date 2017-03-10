using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HP41CV.Annotations;
using Xamarin.Forms;

namespace HP41CV.ViewModels
{
    public class ToggleViewModel : INotifyPropertyChanged
    {
        private Action action;

        public string Text { get; private set; }
        public ICommand PerfomKeyCommand { get; private set; }

        public ToggleViewModel(string text, Action action)
        {
            this.action = action;
            Text = text;
            PerfomKeyCommand = new Command(PerformKey);
        }

        public virtual void PerformKey()
        {
            action?.Invoke();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
