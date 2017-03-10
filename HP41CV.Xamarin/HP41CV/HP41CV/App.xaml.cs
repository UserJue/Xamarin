using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HP41CV.Helpers;
using Xamarin.Forms;

namespace HP41CV
{
    public partial class App : Application
    {
        public App(bool useAppResource = true)
        {
            UseAppResource = useAppResource;
            InitializeComponent();

            if (Device.OS == TargetPlatform.iOS || Device.OS == TargetPlatform.Android)
            {
                var ci = DependencyService.Get<IPlattformLocalize>().GetCurrentCultureInfo();
                AppResources.Culture = ci; // set the RESX for resource localization
                DependencyService.Get<IPlattformLocalize>().SetLocale(ci); // set the Thread for locale-aware methods
            }
            MainPage = new HP41CV.MainPage();
        }

        public bool UseAppResource { get; set; }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
