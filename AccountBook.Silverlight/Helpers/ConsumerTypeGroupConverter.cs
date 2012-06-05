using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace AccountBook.Silverlight.Helpers
{
    public class ConsumerTypeGroupConverter : IValueConverter
    {
        private static Brush headerBg = new SolidColorBrush(Colors.LightGray);
        private static Brush itemBg = new SolidColorBrush(Colors.Transparent); 

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isHeader = (long) value == 0;
            switch (parameter.ToString())
            {
                case "Background":
                    return isHeader ? headerBg : itemBg;
                case "IsEnabled":
                    return !isHeader;
                case "FontWeight":
                    return isHeader ? FontWeights.Bold : FontWeights.Normal;
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
