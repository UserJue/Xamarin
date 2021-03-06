﻿using System.ComponentModel;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using HP41CV.Annotations;
using HP41CV.Helpers;
using Xamarin.Forms;

namespace HP41CV.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool helpVisible;

        private bool optionVisible;

        public bool OneView { get; set; }

        public bool ThreeViews { get; set; }

        public bool IsVisible => true;

        public CalculatorViewModel CalculatorViewModel { get; }

        public AboutViewModel AboutViewModel { get; }

        public HelpViewModel HelpViewModel { get; }

        public OptionsViewModel OptionsViewModel { get; }

        public MainViewModel()
        {
            bool useAppResource = ((App)App.Current).UseAppResource; ;
            OneView = true;
            CalculatorViewModel = new CalculatorViewModel();
            AboutViewModel = new AboutViewModel {Title = !useAppResource ? "HP41-CV" : AppResources.ApplicationTitle,SubTitle = !useAppResource ? "Windows Phone" : AppResources.ForW,Version = "4.1",BackText = !useAppResource ? "Close" : AppResources.Close };
            switch (Device.OS)
            {
                case TargetPlatform.iOS:
                    AboutViewModel.SubTitle = AppResources.ForI;
                    break;
                case TargetPlatform.Android:
                    AboutViewModel.SubTitle = AppResources.ForD;
                    break;
            }
            HelpViewModel = new HelpViewModel() { BackText = !useAppResource ? "Close" : AppResources.Close };
            OptionsViewModel = new OptionsViewModel() { BackText = !useAppResource ? "Close" : AppResources.Close };

            CalculatorViewModel.IsVisible = true;
            AboutViewModel.IsVisible = helpVisible;
            AboutViewModel.IsBack2Visible = true;
            CalculatorViewModel.ShowHelpAction = () =>
            {
                if (optionVisible)
                    optionVisible = false;
                else
                    helpVisible = !helpVisible;
                HelpViewModel.IsVisible = helpVisible;
                OptionsViewModel.IsVisible = optionVisible;
                AboutViewModel.IsVisible = false;
                CalculatorViewModel.IsVisible = !helpVisible || !OneView;
            };
            HelpViewModel.BackAction = () =>
            {
                helpVisible = !helpVisible;
                HelpViewModel.IsVisible = helpVisible;
                CalculatorViewModel.IsVisible = !helpVisible || !OneView;
            };
            HelpViewModel.AboutAction = () =>
            {
                AboutViewModel.IsVisible = true;
                HelpViewModel.IsVisible = ThreeViews;
            };
            HelpViewModel.OptionAction = async () =>
            {
                await OptionsViewModel.LoadOptions();
                optionVisible = true;
                helpVisible = false;
                OptionsViewModel.IsVisible = optionVisible;
                HelpViewModel.IsVisible = helpVisible;
            };
            AboutViewModel.BackAction = () =>
            {
                AboutViewModel.IsVisible = false;
                if (OneView)
                {
                    helpVisible = false;
                    optionVisible = false;
                }
                HelpViewModel.IsVisible = helpVisible;
                OptionsViewModel.IsVisible = optionVisible;
                CalculatorViewModel.IsVisible = true;
            };
            OptionsViewModel.IsVisible = false;
            OptionsViewModel.BackAction = () =>
            {
                helpVisible = true;
                optionVisible = false;
                HelpViewModel.IsVisible = helpVisible;
                OptionsViewModel.IsVisible = optionVisible;
                CalculatorViewModel.IsVisible = !helpVisible || !OneView;
            };
            OptionsViewModel.AboutAction = () =>
            {
                AboutViewModel.IsVisible = true;
                OptionsViewModel.IsVisible = ThreeViews;
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
