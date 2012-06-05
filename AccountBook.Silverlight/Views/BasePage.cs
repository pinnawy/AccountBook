using System;
using System.Windows.Controls;
using AccountBook.Silverlight.Controls;

namespace AccountBook.Silverlight.Views
{
    public class BasePage : Page
    {
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (WebContext.Current.Authentication.User.Identity.IsAuthenticated)
            {
                base.OnNavigatedTo(e);
            }
            else
            {
                this.NavigationService.Navigate(new Uri("/Home", UriKind.Relative));
                TipWindow.Alert("BasePage::你还没有登录, 请先登录!");
            }
        }
    }
}
