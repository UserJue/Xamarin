using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HP41CV.Annotations;
using HP41CV.Helpers;
using Xamarin.Forms;

namespace HP41CV.ViewModels
{
    public class OptionViewModel : INotifyPropertyChanged
    {
        public string Name { get; private set; }

        private string key;

        public string Description { get; }

        public string Description1 { get;  }

        public string Description2 { get;  }

        public string Price { get; private set; }

        public bool CanBuy { get; private set; }

        public bool ShowInfo { get; private set; }

        public ICommand BuyCommand { get; private set; }

        public OptionViewModel(string key, string name, string price, bool activ, bool inPreparation,ICollection<string> descriptions)
        {
            this.key = key;
            Name = name;
            bool isEnabled = false;
            if (inPreparation)
                Price = AppResources.InPreparation;
            else if (activ)
                Price = AppResources.Activ;
            else
            {
                isEnabled = true;
                var format = AppResources.BuyButton;
                Price = string.Format(format, price);
            }
            CanBuy = isEnabled;
            ShowInfo = !isEnabled;
            var index = 0;
            foreach (var description in descriptions)
            {
                if (index == 0)
                    Description = description;
                else if (index == 1)
                    Description1 = description;
                else if (index == 2)
                    Description2 = description;
                index++;
            }
            BuyCommand = new Command(async () =>
            {
                if (await MarketPlace.BuyOption(key))
                {
                    Price = AppResources.Activ;
                    CanBuy = false;
                    ShowInfo = true;
                    OnPropertyChanged(nameof(Price));
                    OnPropertyChanged(nameof(CanBuy));
                    OnPropertyChanged(nameof(ShowInfo));
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
