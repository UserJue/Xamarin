using Windows.UI.Popups;
using HP41CV.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(HP41CV.UWP.Helpers.PlatformMessenger))]

namespace HP41CV.UWP.Helpers
{
    public class PlatformMessenger : IPlatformMessenger
    {
        public void ShowDialogAsync(string message, string caption)
        {
            var dialog = new MessageDialog(message,caption);
            dialog.ShowAsync();
        }
    }
}
