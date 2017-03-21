using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace _15Puzzle.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private bool isVisible;

        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                if (value == isVisible) return;
                isVisible = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsBack1Visible));
                OnPropertyChanged(nameof(IsBack2Visible));
            }
        }

        private bool back1;

        public bool IsBack1Visible
        {
            get { return isVisible && back1; }
            set
            {
                if (value == isVisible) return;
                isVisible = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsBack2Visible));
            }
        }
        private bool isBack2Visible;

        public bool IsBack2Visible
        {
            get { return isVisible && !back1; }
            set
            {
                if (value == !back1) return;
                back1 = !value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsBack1Visible));
            }
        }

        public Action BackAction;

        public ICommand BackCommand { get; private set; }

        public ICommand AboutCommand { get; private set; }

        public string BackText { get; set; }

        public Action AboutAction;
        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsViewModel()
        {
            back1 = true;
            BackCommand = new Command(() => BackAction?.Invoke());
            AboutCommand = new Command(() => AboutAction?.Invoke());

        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
