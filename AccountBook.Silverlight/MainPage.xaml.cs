﻿using System;
using AccountBook.SControls;
using AccountBook.Silverlight.Events;

namespace AccountBook.Silverlight
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Navigation;

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
        private void ContentFrameNavigated(object sender, NavigationEventArgs e)
        {
            foreach (UIElement child in LinksStackPanel.Children)
            {
                var hb = child as HyperlinkButton;
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
        private void ContentFrameNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Handled = true;
            ErrorWindow.CreateNew(e.Exception);
        }

        private void ContentFrameNavigating(object sender, NavigatingCancelEventArgs e)
        {
            var homeUri = new Uri("/Home", UriKind.Relative);

            if (e.Uri != homeUri
                && Application.Current.Resources.Contains("AuthenticationCompleted")
                && !WebContext.Current.Authentication.User.Identity.IsAuthenticated)
            {
                _lastFailUri = e.Uri;
                e.Cancel = true;
                var naviSvc = (NavigationService)sender;
                naviSvc.Navigate(homeUri);
                TipWindow.Confirm("你还没有登录, 请先登录!", "提示", result => { if (result) LoginStatusBar.ShowLoginForm(); });
            }
        }

        private void LoginStatusLoginStatusChanged(object sender, LoginStatusChangedEventArgs e)
        {
            if (e.LoginStatus == LoginStatus.Login)
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

        private void BtnFullScreenClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
        }
    }
}