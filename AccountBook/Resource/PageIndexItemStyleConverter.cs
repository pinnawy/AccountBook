
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace AccountBookWin.Resource
{
    public class PageIndexItemStyleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int currPageIndex = (int)values[0];
            int pageIndex = int.Parse(((Button)values[1]).Content.ToString());

            switch (parameter.ToString())
            {
                case "BorderBrush":
                    return currPageIndex == pageIndex
                  ? new SolidColorBrush(Colors.IndianRed)
                  : new SolidColorBrush(Colors.CornflowerBlue);
                case "Background":
                    return currPageIndex == pageIndex
                      ? new SolidColorBrush(Colors.Pink)
                      : new SolidColorBrush(Colors.AliceBlue);
                default:
                    return null;

            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
