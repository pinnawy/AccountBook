using System.Windows;
using System.Windows.Controls;

namespace AccountBook.Silverlight.Controls
{
    public class CustomPager : DataPager
    {
        public static readonly DependencyProperty PageSizeListProperty;

        static CustomPager()
        {
            PageSizeListProperty = DependencyProperty.Register(
                "PageSizeList",
                typeof(int[]),
                typeof(CustomPager),
                new PropertyMetadata(new[] { 5, 10, 15, 20 }));
        }

        public CustomPager()
        {
            this.DataContext = this;
        }

        public int[] PageSizeList
        {
            get { return (int[])GetValue(PageSizeListProperty); }
            set { SetValue(PageSizeListProperty, value); }
        }
    }
}