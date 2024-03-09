using System;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace RaceTo21
{
    //This class is to conver cards ID into images
    public class IDToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string ID = value as string;
            if (!string.IsNullOrEmpty(ID))
            {
                // Find corresponding images in the Images folder.
                string imagePath = $"pack://application:,,,/Images/{ID}.png";
                return new BitmapImage(new Uri(imagePath));
            }
            
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}


