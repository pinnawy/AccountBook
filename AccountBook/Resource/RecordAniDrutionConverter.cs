using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using AccountBook.Model;
using Microsoft.Expression.Interactivity.Core;

namespace AccountBookWin.Resource
{
    public class RecordAniDrutionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<ConsumeRecord> records = (List<ConsumeRecord>)values[0];
            ConsumeRecord cr = (ConsumeRecord)values[1];
            return (double)(records.IndexOf(cr) * 100 + 100);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
