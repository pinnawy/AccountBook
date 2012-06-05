using System;
using AccountBook.Silverlight.Controls;

namespace AccountBook.Silverlight
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Navigation;
    using AccountBook.Silverlight.LoginUI;

    /// <summary>
    /// <see cref="UserControl"/> class providing the main UI for the application.
    /// </summary>
    public partial class MainPage : UserControl
    {
        private Uri _lastFailUri;

        /// <summary>
        /// Creates a new <see cref="MainPage"/> instance.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// After the Frame navigates, ensure the <see cref="HyperlinkButton"/> representing the current page is selected
        /// </summary>
        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            foreach (UIElement child in LinksStackPanel.Children)
            {
                HyperlinkButton hb = child as HyperlinkButton;
                if (hb != null && hb.NavigateUri != null)
                {
                    if (hb.NavigateUri.ToString().Equals(e.Uri.ToString()))
                    {
                        VisualStateManager.GoToState(hb, "ActiveLink", true);
                    }
                    else
                    {
                        VisualStateManager.GoToState(hb, "InactiveLink", true);
                    }
                }
            }
        }

        /// <summary>
        /// If an error occurs during navigation, show an error window
        /// </summary>
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            ErrorWindow.CreateNew(e.Exception);
        }

        private void ContentFrame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            Uri homeUri = new Uri("/Home", UriKind.Relative);
            if (e.Uri != homeUri && !WebContext.Current.Authentication.User.Identity.IsAuthenticated)
            {
                _lastFailUri = e.Uri;
                e.Cancel = true;
                NavigationService naviSvc = (NavigationService)sender;
                naviSvc.Navigate(homeUri);
                TipWindow.Alert("你还没有登录, 请先登录!");
            }
        }

        private void LoginStatus_LoginStatusChanged(object sender, Events.LoginStatusChangedEventArgs e)
        {
            if (e.LoginStatus == Events.LoginStatus.Login)
            {
                if (_lastFailUri != null && ContentFrame.Source != _lastFailUri)
                {
                    ContentFrame.Navigate(_lastFailUri);
                }
            }
            else
            {
                ContentFrame.Navigate(new Uri("/Home", UriKind.Relative));
            }
        }
    }
}