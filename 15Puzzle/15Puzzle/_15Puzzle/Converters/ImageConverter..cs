using System;
using System.Globalization;
using Xamarin.Forms;

namespace _15Puzzle.Converters
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var name = value as string;
            if (name == null) return null;
            var source = "_15Puzzle.Pictures." + name + ".png";
            return ImageSource.FromResource(source);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
