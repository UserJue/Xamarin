namespace HP41CV.Helpers
{
    public interface IPlatformMessenger
    {
        void ShowDialogAsync(string message, string caption);
    }
}
