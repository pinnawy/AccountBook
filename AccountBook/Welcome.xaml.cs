using System.Windows;
using System.Windows.Controls;
using AccountBook.Model;
using AccountBookWin.Common;

namespace AccountBookWin
{
    /// <summary>
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class Welcome : UserControl
    {
        /// <summary>
        /// 登录事件回调
        /// </summary>
        public event RoutedEventHandler LoginSuccess;

        public Welcome()
        {
            InitializeComponent();
            Loaded += (sender, e) =>
                {
#if DEBUG
                    if (LoginSuccess != null)
                    {
                        AccountBookContext.CurrentUser = BllFactory.GetUserBll().GetUser("yuwang");
                        LoginSuccess(sender, e);
                    }
#endif
                    TxtUserName.Focus();
                };
        }

        private void BtnGoClick(object sender, RoutedEventArgs e)
        {
            User user = BllFactory.GetUserBll().GetUser(TxtUserName.Text);

            if (user != null)
            {
                AccountBookContext.CurrentUser = user;

                if (LoginSuccess != null)
                {
                    LoginSuccess(sender, e);
                }
            }
            else
            {
                MessageBox.Show("用户名不存在!");
            }
        }

        private void UserControlKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                BtnGoClick(sender, e);
            }
        }
    }
}
