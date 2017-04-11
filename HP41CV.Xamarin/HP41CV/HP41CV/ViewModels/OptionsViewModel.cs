using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HP41CV.Annotations;

namespace HP41CV.ViewModels
{
    public class OptionsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<OptionViewModel> Options { get; private set; }

        public OptionsViewModel()
        {
            Options = new ObservableCollection<OptionViewModel>();
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
