using Xamarin.Forms;

namespace HP41CV.Helpers
{
    public class Messenger
    {
        public static void ShowDialogAsync(string message, string caption = null)
        {
            //            DependencyService.Get<IPlatformMessenger>().ShowDialogAsync(message, caption ?? AppResources.ApplicationTitle);
            Application.Current.MainPage.DisplayAlert(caption ?? AppResources.ApplicationTitle, message, "OK");
        }
    }
}
