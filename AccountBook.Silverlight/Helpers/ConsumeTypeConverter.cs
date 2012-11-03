using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using AccountBook.Model;

namespace AccountBook.Silverlight
{
    public class ConsumeTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var consumeType = value as AccountType;
            if (consumeType != null)
            {
                for (var index = 0; index < AccountBookContext.Instance.ExpenseTypeList.Count; index++)
                {
                    if (AccountBookContext.Instance.ExpenseTypeList[index].TypeId == consumeType.TypeId)
                    {
                        return index;
                    }
                }
            }

            var firstSubType =AccountBookContext.Instance.ExpenseTypeList.First(type => type.ParentTypeId != 0);
            return AccountBookContext.Instance.ExpenseTypeList.IndexOf(firstSubType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return AccountBookContext.Instance.ExpenseTypeList[(int)value];
        }
    }
}