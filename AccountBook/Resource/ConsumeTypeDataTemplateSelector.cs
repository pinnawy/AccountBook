using System.Windows;
using System.Windows.Controls;
using AccountBook.Model;

namespace AccountBookWin.Resource
{
    public class ConsumeTypeDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item is ConsumeType)
            {
                ConsumeType type = item as ConsumeType;

                if (type.ParentTypeId == 0)
                {
                    return element.FindResource("PrimaryTypeTemplate") as DataTemplate;
                }
                    
                return element.FindResource("SubTypeTemplate") as DataTemplate;
            }

            return null;
        }
    }
}
