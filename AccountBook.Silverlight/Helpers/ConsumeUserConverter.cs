using System;
using System.Globalization;
using System.Windows.Data;
using AccountBook.Model;

namespace AccountBook.Silverlight
{
    public class ConsumeUserConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var userInfo = value as UserInfo;
            if (userInfo != null)
            {
                for (var index = 0; index < AccountBookContext.Instance.ConsumerList.Count; index++)
                {
                    if (AccountBookContext.Instance.ConsumerList[index].UserId == userInfo.UserId)
                    {
                        return index;
                    }
                }
            }

            return 0;       //TODO: Return Current Authentication User
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var index = (int) value;
            return  index > -1 && index < AccountBookContext.Instance.ConsumerList.Count ? AccountBookContext.Instance.ConsumerList[index].Clone() : null;
        }
    }
}