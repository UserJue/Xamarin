using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using HP41CV.Annotations;
using HP41CV.Helpers;
using Xamarin.Forms;

namespace HP41CV.ViewModels
{
    public class OptionsViewModel : INotifyPropertyChanged
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

        public Action AboutAction;

        public ICommand AboutCommand { get; private set; }

        public string AboutText { get; set; }

        public string Title { get; set; }

        public ObservableCollection<OptionViewModel> Options { get; private set; }

        public OptionsViewModel()
        {
            bool useAppResource = ((App)App.Current).UseAppResource;
            back1 = true;
            Title = !useAppResource ? "OptionsTitle" : AppResources.OptionsTitle;
            AboutText = !useAppResource ? "AboutCommand" : AppResources.AboutCommand;
            BackCommand = new Command(() => BackAction?.Invoke());
            AboutCommand = new Command(() => AboutAction?.Invoke());

            Options = new ObservableCollection<OptionViewModel>();
            LoadOptions();
        }

        public async Task LoadOptions()
        {
            var option = await MarketPlace.GetOptions();
            Options.Clear();
            var entry = "Statistic";
            var description = new Collection<string>() {AppResources.OptionStatistic};
            if (option.ContainsKey(entry))
                Options.Add(new OptionViewModel(entry, option[entry].Item1, option[entry].Item2, option[entry].Item3, false,description));
            else
                Options.Add(new OptionViewModel(entry, entry, "", false, true, description));
            entry = "Persistence";
            description = new Collection<string>() { AppResources.OptionPersistence, AppResources.OptionPersistence1 };
            if (option.ContainsKey(entry))
                Options.Add(new OptionViewModel(entry, option[entry].Item1, option[entry].Item2, option[entry].Item3, false, description));
            else
                Options.Add(new OptionViewModel(entry, entry, "", false, true, description));
            entry = "Programming";
            description = new Collection<string>() { AppResources.OptionProgramming };
            if (option.ContainsKey(entry))
                Options.Add(new OptionViewModel(entry, option[entry].Item1, option[entry].Item2, option[entry].Item3, false, description));
            else
                Options.Add(new OptionViewModel(entry, entry, "", false, true, description));
            entry = "RobustStatistic";
            description = new Collection<string>() { AppResources.OptionRobustStatistic, AppResources.OptionRobustStatistic1, AppResources.OptionRobustStatistic2 };
            if (option.ContainsKey(entry))
                Options.Add(new OptionViewModel(entry, option[entry].Item1, option[entry].Item2, option[entry].Item3, false, description));
            else
                Options.Add(new OptionViewModel(entry, entry, "", false, true, description));
            entry = "Printer";
            description = new Collection<string>() { AppResources.OptionPrinter, AppResources.OptionPrinter1 };
            if (option.ContainsKey(entry))
                Options.Add(new OptionViewModel(entry, option[entry].Item1, option[entry].Item2, option[entry].Item3, false, description));
            else
                Options.Add(new OptionViewModel(entry, entry, "", false, true, description));
            entry = "Constants";
            description = new Collection<string>() { AppResources.OptionConstants, AppResources.OptionConstants1 };
            if (option.ContainsKey(entry))
                Options.Add(new OptionViewModel(entry, option[entry].Item1, option[entry].Item2, option[entry].Item3, true, description));
            else
                Options.Add(new OptionViewModel(entry, entry, "", false, true, description));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
