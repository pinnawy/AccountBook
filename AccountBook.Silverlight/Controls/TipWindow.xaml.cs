using System.Windows;
using System.Windows.Controls;

namespace AccountBook.Silverlight.Controls
{
    public partial class TipWindow : ChildWindow
    {
        protected TipWindow(string message, string title, bool isConfirmWin = false)
        {
            InitializeComponent();
            Message = message;
            Title = title;
            BtnCancelVisibility = isConfirmWin ? Visibility.Visible : Visibility.Collapsed;
            this.DataContext = this;
        }

        public string Message
        {
            get;
            private set;
        }

        private Visibility BtnCancelVisibility
        {
            get;
            set;
        }

        public static void Alert(string message, string title = "Alert")
        {
            new TipWindow(message, title).Show();
        }

        public static bool Confirm(string message, string title = "Confirm")
        {
            TipWindow win = new TipWindow(message, title, true);
            win.Show();
            return win.DialogResult.HasValue && win.DialogResult.Value;
        }

        private void BtnOKClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
