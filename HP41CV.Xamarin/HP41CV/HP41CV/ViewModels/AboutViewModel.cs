using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HP41CV.Annotations;
using Xamarin.Forms;

namespace HP41CV.ViewModels
{
    public class AboutViewModel : INotifyPropertyChanged
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

        public string BackText { get; set; }

        public string Title { get; set; }

        public string SubTitle { get; set; }

        public string Version { get; set; }

        public string Copyright1 { get; set; }

        public string Copyright2 { get; set; }

        public string HomePage { get; set; }

        public string EMail { get; set; }

        public AboutViewModel()
        {
            back1 = true;
            Copyright1 = "Copyright 2014 -2017";
            Copyright2 = "Ströhmer";
            HomePage = "userjue.azurewebsites.net";
            EMail = "juergen@fam-stroehmer.de";
            BackCommand = new Command(() => BackAction?.Invoke());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
