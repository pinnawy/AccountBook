using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using AccountBook.Model;

namespace AccountBook.Records.Helper
{
    public class AccountCategoryConverter : IValueConverter
    {
        private readonly SolidColorBrush _expenseColor = new SolidColorBrush(Colors.Green);
        private readonly SolidColorBrush _incomeColor = new SolidColorBrush(Colors.Red);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           var category = (AccountCategory) value;
            if(targetType == typeof(Brush))
            {
                return category == AccountCategory.Expense ? _expenseColor : _incomeColor;
            }

            return category == AccountCategory.Expense ? "‐" : "+";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
