using HP41CV.Helpers;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(HP41CV.iOS.Helpers.PlatformMessenger))]

namespace HP41CV.iOS.Helpers
{
    public class PlatformMessenger : IPlatformMessenger
    {
        public void ShowDialogAsync(string message, string caption)
        {
            UIAlertView messageBox = new UIAlertView(caption, message, null, "Ok", null);
            messageBox.Show();
        }
    }
}
