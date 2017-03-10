using Android.Widget;
using HP41CV.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(HP41CV.Droid.Helpers.PlatformMessenger))]

namespace HP41CV.Droid.Helpers
{
    public class PlatformMessenger : IPlatformMessenger
    {
        public void ShowDialogAsync(string message, string caption )
        {
            Toast.MakeText(null, message, ToastLength.Long).Show(); 
        }
    }
}