using System.Globalization;

namespace HP41CV.Helpers
{
    public interface IPlattformLocalize
    {
        CultureInfo GetCurrentCultureInfo();
        void SetLocale(CultureInfo ci);
    }
}
