using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using HP41CV.Annotations;

namespace HP41CV.ViewModels
{
    public class HelpKeyViewModel : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public HelpKeyViewModel(string name)
        {
            Name = name;
            Description = "";
        }

        public HelpKeyViewModel(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
