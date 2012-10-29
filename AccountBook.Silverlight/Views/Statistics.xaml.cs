using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Resources;
using System.Xml;

namespace AccountBook.Silverlight
{
    using System.Windows.Controls;
    using System.Windows.Navigation;

    /// <summary>
    /// <see cref="Page"/> class to present information about the current application.
    /// </summary>
    public partial class Statistics : Page
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Manage"/> class.
        /// </summary>
        public Statistics()
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

            if (AccountBookContext.Instance.StatisticsModule == null)
            {
                WebClient loadManageXapClient = new WebClient();
                loadManageXapClient.OpenReadCompleted += new OpenReadCompletedEventHandler(ManageXapOpenReadCompleted);
                loadManageXapClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ManageXapDownloadProgressChanged);
                Uri xapUri = new Uri(HtmlPage.Document.DocumentUri, "ClientBin/AccountBook.Statistics.xap");
                loadManageXapClient.OpenReadAsync(xapUri);
            }
            else
            {
                LoadManageMoudle(AccountBookContext.Instance.StatisticsModule);
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
                Assembly assembly = GetAssemblyFromXap(e.Result, "AccountBook.Statistics.dll");
                UIElement statisticsModule = assembly.CreateInstance("AccountBook.Statistics.MainPage") as UIElement;

                AccountBookContext.Instance.StatisticsModule = statisticsModule;
                LoadManageMoudle(statisticsModule);
            }
            else
            {
                ErrorWindow.CreateNew(e.Error.InnerException);
            }
        }

        /// <summary>
        /// 加载统计模块
        /// </summary>
        /// <param name="statisticsModule">统计模块元素</param>
        private void LoadManageMoudle(UIElement statisticsModule)
        {
            this.LayoutRoot.Children.Remove(LoadXapProgressPanel);
            this.LayoutRoot.Children.Add(statisticsModule);
        }

        /// <summary>
        /// 卸载统计模块
        /// </summary>
        private void UnLoadManageMoudle()
        {
            this.LayoutRoot.Children.Remove(AccountBookContext.Instance.StatisticsModule);
        }

        /// <summary>
        /// 从XAP包中返回程序集信息
        /// </summary>
        /// <param name="packageStream"></param>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        private Assembly GetAssemblyFromXap(Stream packageStream, String assemblyName)
        {
            Stream stream = Application.GetResourceStream(new StreamResourceInfo(packageStream, null), new Uri("AppManifest.xaml", UriKind.Relative)).Stream;
            Assembly asm = null;
            XmlReader xmlReader = XmlReader.Create(stream);
            xmlReader.MoveToContent();
            if (xmlReader.ReadToFollowing("Deployment.Parts"))
            {
                string str = xmlReader.ReadInnerXml();
                Regex reg = new Regex("Source=\"(.+?)\"");
                Match match = reg.Match(str);
                string sSource = "";
                if (match.Groups.Count == 2)
                {
                    sSource = match.Groups[1].Value;
                }
                AssemblyPart assemblyPart = new AssemblyPart();
                StreamResourceInfo streamInfo = Application.GetResourceStream(new StreamResourceInfo(packageStream, "application/binary"), new Uri(sSource, UriKind.Relative));
                if (sSource == assemblyName)
                {
                    asm = assemblyPart.Load(streamInfo.Stream);
                }
            }

            return asm;
        }
    }
}