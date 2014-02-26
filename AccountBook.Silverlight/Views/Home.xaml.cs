using System;
using System.Collections.Generic;
using System.Linq;
using AccountBook.Silverlight.Models;

namespace AccountBook.Silverlight
{
    using System.Windows.Controls;
    using System.Windows.Navigation;

    /// <summary>
    /// Home page for the application.
    /// </summary>
    public partial class Home : Page
    {
        private Dictionary<string, string> _changeLogs = new Dictionary<string, string>();

        /// <summary>
        /// Creates a new <see cref="Home"/> instance.
        /// </summary>
        public Home()
        {
            InitializeComponent();

            this.Title = ApplicationStrings.HomePageTitle;

            _changeLogs.Add("2014-02-26", "增加设置附加记录功能");
            _changeLogs.Add("2012-12-15", "修复修改记录条目导致消费分类显示不正确的Bug、修复统计页面初始化查询出错的Bug、增加消费分类统计功能");
            _changeLogs.Add("2012-11-13", "修复同一类别账目无法同时添加的Bug、修复查询结束时间不准确的Bug");
            _changeLogs.Add("2012-11-04", "增加收入账目录入功能");
            _changeLogs.Add("2012-11-02", "消费记录列表模块化、界面微调、修复部分已知Bug");
            _changeLogs.Add("2012-10-31", "增加消费信息统计页面、增加消费记录列表排序功能、增加消费记录按消费备注关键字查询功能、优化页面加载速度、修复部分已知Bug");
            _changeLogs.Add("2012-06-08", "增加空白管理页面、修复记录添加时Save+Add报错的Bug、添加删除消费记录功能");
            _changeLogs.Add("2012-06-05", "增加首页身份验证等待遮罩、增加分页控件页码显示设置");
            _changeLogs.Add("2012-06-03", "增加消费类别分组显示");
            _changeLogs.Add("2012-06-01", "第一测试版本上线....");

            var changeLogs = _changeLogs.Select(log => new ChangeLogItem { Date = DateTime.Parse(log.Key), Content = log.Value }).ToList();
            ChangeLogItemsPanel.ItemsSource = changeLogs;
        }

        /// <summary>
        /// Executes when the user navigates to this page.
        /// </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}