using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteErasee.Converter
{
    public class ImageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
            {
                return new Bitmap(AssetLoader.Open(new Uri("avares://WriteErasee/Assets/picture.png")));

            }
            else
            {
                try
                {
                    return new Bitmap(AssetLoader.Open(new Uri($"avares://WriteErasee/Assets/{value}")));
                }
                catch
                {
                    return new Bitmap(AssetLoader.Open(new Uri("avares://WriteErasee/Assets/picture.png")));

                }
            }
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
