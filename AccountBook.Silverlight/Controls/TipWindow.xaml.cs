using System;
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
            BtnCancel.Visibility = isConfirmWin ? Visibility.Visible : Visibility.Collapsed;
            this.DataContext = this;
        }

        public string Message
        {
            get;
            private set;
        }

        public static void Alert(string message, string title = "Alert", Action<bool> callback = null)
        {
            var win = new TipWindow(message, title);
            win.Closed += (s, e) =>
            {
                if (callback != null)
                {
                    callback(win.DialogResult.HasValue && win.DialogResult.Value);
                }
            };
            win.Show();
        }

        public static void Confirm(string message, Action<bool> callback = null)
        {
            Confirm(message, "Confirm", callback);
        }

        public static void Confirm(string message, string title = "Confirm", Action<bool> callback = null)
        {
            var win = new TipWindow(message, title, true);
            win.Closed += (s, e) =>
            {
                if (callback != null)
                {
                    callback(win.DialogResult.HasValue && win.DialogResult.Value);
                }
            };
            win.Show();
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
