using System.ComponentModel;
using System.Runtime.CompilerServices;
using HP41CV.Annotations;

namespace HP41CV.ViewModels
{
    public class LineViewModel : INotifyPropertyChanged
    {
        public string Text { get; }

        public LineViewModel(string text)
        {
            Text = text;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
