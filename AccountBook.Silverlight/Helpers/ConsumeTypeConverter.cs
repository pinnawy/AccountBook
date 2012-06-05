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
            var consumeType = value as ConsumeType;
            if (consumeType != null)
            {
                for (var index = 0; index < AccountBookContext.Instance.ConsumeTypeList.Count; index++)
                {
                    if (AccountBookContext.Instance.ConsumeTypeList[index].TypeId == consumeType.TypeId)
                    {
                        return index;
                    }
                }
            }

            var firstSubType =AccountBookContext.Instance.ConsumeTypeList.First(type => type.ParentTypeId != 0);
            return AccountBookContext.Instance.ConsumeTypeList.IndexOf(firstSubType);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ConsumeType consumeType = AccountBookContext.Instance.ConsumeTypeList[(int)value];

            return new ConsumeType
            {
                ParentTypeId = consumeType.ParentTypeId,
                TypeId = consumeType.TypeId,
                TypeName = consumeType.TypeName,
                ParentTypeName = consumeType.ParentTypeName
            };

        }
    }
}