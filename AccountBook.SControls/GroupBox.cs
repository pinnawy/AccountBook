using System.Windows;
using System.Windows.Controls;

namespace AccountBook.SControls
{
    public class GroupBox : ContentControl
    {
        public GroupBox()
        {
            this.DefaultStyleKey = typeof(GroupBox);
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
    "Header",
    typeof(string)
    , typeof(GroupBox),
    new PropertyMetadata(TitleChanged));

        private static void TitleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
    }
}
