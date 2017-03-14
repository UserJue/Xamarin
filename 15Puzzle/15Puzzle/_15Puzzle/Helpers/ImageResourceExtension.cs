using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace _15Puzzle.Helpers
{
    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            var result = Source == null ? null : ImageSource.FromResource(Source);
            return result;
        }
    }
}
