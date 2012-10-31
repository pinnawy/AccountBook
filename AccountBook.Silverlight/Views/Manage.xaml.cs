using System;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Browser;
using AccountBook.Silverlight.Helpers;

namespace AccountBook.Silverlight
{
    using System.Windows.Controls;
    using System.Windows.Navigation;

    /// <summary>
    /// <see cref="Page"/> class to present information about the current application.
    /// </summary>
    public partial class Manage : Page
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Manage"/> class.
        /// </summary>
        public Manage()
        {
            InitializeComponent();
            this.Title = ApplicationStrings.ManagePageTitle;
        }

        /// <summary>
        /// Executes when the user navigates to this page.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (AccountBookContext.Instance.ManageModule == null)
            {
                WebClient loadManageXapClient = new WebClient();
                loadManageXapClient.OpenReadCompleted += new OpenReadCompletedEventHandler(ManageXapOpenReadCompleted);
                loadManageXapClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ManageXapDownloadProgressChanged);
                Uri xapUri = new Uri(HtmlPage.Document.DocumentUri, "ClientBin/AccountBook.Manage.xap");
                loadManageXapClient.OpenReadAsync(xapUri);
            }
            else
            {
                LoadManageMoudle(AccountBookContext.Instance.ManageModule);
            }
        }

        /// <summary>
        /// Executes when the user navigated to other page from this page.
        /// </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            UnLoadManageMoudle();
        }

        private void ManageXapDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            XapLoadText.Text = e.ProgressPercentage.ToString();
            XapLoadProgress.Value = e.ProgressPercentage;
        }

        private void ManageXapOpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Assembly assembly = XapHelper.LoadAssemblyFromXap(e.Result);
                UIElement manageModule = assembly.CreateInstance("AccountBook.Manage.MainPage") as UIElement;

                AccountBookContext.Instance.ManageModule = manageModule;
                LoadManageMoudle(manageModule);
            }
            else
            {
                ErrorWindow.CreateNew(e.Error.InnerException);
            }
        }

        /// <summary>
        /// 加载管理模块
        /// </summary>
        /// <param name="manageModule">管理模块元素</param>
        private void LoadManageMoudle(UIElement manageModule)
        {
            this.LayoutRoot.Children.Remove(LoadXapProgressPanel);
            this.LayoutRoot.Children.Add(manageModule);
        }

        /// <summary>
        /// 卸载管理模块
        /// </summary>
        private void UnLoadManageMoudle()
        {
            this.LayoutRoot.Children.Remove(AccountBookContext.Instance.ManageModule);
        }
    }
}