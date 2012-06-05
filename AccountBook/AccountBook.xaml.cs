using System;
using System.Windows;
using System.Windows.Threading;

namespace AccountBookWin
{
    /// <summary>
    /// Interaction logic for AccountBook.xaml
    /// </summary>
    public partial class AccountBook : Window
    {
        public AccountBook()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (s, args) =>
            {
                Time.Text = DateTime.Now.ToString("HH:mm:ss yyyy-MM-dd");
            };
            timer.Start();
        }

        private void LoginSuccess(object sender, RoutedEventArgs e)
        {
            Content.Content = new ConsumeRecordList();
        }
    }
}
