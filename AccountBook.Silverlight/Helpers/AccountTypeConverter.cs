using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using AccountBook.Model;

namespace AccountBook.Silverlight
{
    public class AccountTypeConverter : IValueConverter
    {
        private readonly ObservableCollection<AccountType> _accountTypeList;

        public AccountTypeConverter(ObservableCollection<AccountType> accountTypeList)
        {
            _accountTypeList = accountTypeList;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var consumeType = value as AccountType;
            if (consumeType != null)
            {
                for (var index = 0; index < _accountTypeList.Count; index++)
                {
                    if (_accountTypeList[index].TypeId == consumeType.TypeId)
                    {
                        return index;
                    }
                }
            }

            var firstSubType = _accountTypeList.First(type => type.ParentTypeId != 0);
            return _accountTypeList.IndexOf(firstSubType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _accountTypeList[(int)value];
        }
    }
}