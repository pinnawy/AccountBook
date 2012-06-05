using System;
using System.Collections.Generic;
using System.Windows.Controls;
using AccountBook.Model;
using AccountBookWin.Common;
using System.ComponentModel;
using Visifire.Charts;

namespace AccountBookWin
{
    /// <summary>
    /// Interaction logic for ConsumeRecordList.xaml
    /// </summary>
    public partial class ConsumeRecordList
    {
        public ConsumeRecordList()
        {
            InitializeComponent();
            InitConsumeTypes();
            InitConsumeUsers();

            DateTimeBegin.SelectedDate = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day);

            Loaded += (s, e) =>
            {
                DateTimeBegin.SelectedDateChanged += DateTimeRangeChanged;
                DateTimeEnd.SelectedDateChanged += DateTimeRangeChanged;

                CmbConsumeType.SelectionChanged += CmbConsumeTypeSelectionChanged;
                CmbConsumeUser.SelectionChanged += CmbConsumeUserSelectionChanged;

                QueryConsumeRecordList();
            };
        }

        /// <summary>
        /// 初始化消费类别下拉框
        /// </summary>
        private void InitConsumeTypes()
        {
            CmbConsumeType.SelectedValuePath = "TypeId";
            var types = BllFactory.GetConsumeTypeBll().GetConsumeTypes(0);
            types.Insert(0, new ConsumeType { TypeId = 0, ParentTypeId = 0, TypeName = "全部" });
            CmbConsumeType.ItemsSource = types;
        }

        /// <summary>
        /// 初始化消费用户下拉框
        /// </summary>
        private void InitConsumeUsers()
        {
            CmbConsumeUser.SelectedValuePath = "UserId";
            var users = BllFactory.GetUserBll().GetUserList();
            users.Insert(0, new User { UserId = 0, FullName = "全部" });
            CmbConsumeUser.ItemsSource = users;
        }

        private void BtnAddRecordClick(object sender, System.Windows.RoutedEventArgs e)
        {
            if (new ConsumeRecordWin().ShowDialog().GetValueOrDefault(false))
            {
                QueryConsumeRecordList();
            }
        }

        /// <summary>
        /// 查询消费列表
        /// </summary>
        private void QueryConsumeRecordList()
        {
            QueryConsumeRecordList(null, SortDir.ASC);
        }

        private void QueryConsumeRecordList(string sortName, SortDir sortDir)
        {
            ConsumeRecortQueryOption option = new ConsumeRecortQueryOption();
            option.BeginTime = DateTimeBegin.SelectedDate == null ? DateTime.MinValue : DateTimeBegin.SelectedDate.Value;
            option.EndTime = DateTimeEnd.SelectedDate == null ? DateTime.MaxValue : DateTimeEnd.SelectedDate.Value.AddDays(1);
            option.PageIndex = RecordListPager.PageIndex ;
            option.PageSize = RecordListPager.PageSize;
            option.UserId = (long)CmbConsumeUser.SelectedValue;
            option.ConsumeType = CmbConsumeType.SelectedItem as ConsumeType;

            if (!string.IsNullOrEmpty(sortName))
            {
                option.SortName = sortName;
                option.SortDir = sortDir;
            }

            if(Content.SelectedIndex == 0)
            {
                int recordCount;
                decimal totalMoney;
                List<ConsumeRecord> records = BllFactory.GetConsumeRecordBll().GetConsumeRecordList(option, out recordCount, out totalMoney);
                RecordList.ItemsSource = records;

                RecordListPager.TotalCount = recordCount;
                TxtTotalMoney.Text = "Total Money:" + totalMoney;
            }
            else
            {
                List<KeyValuePair<string, double>> amountList =  BllFactory.GetConsumeRecordBll().GetConsumeAmountByMonth(option);
                Chart.Series[0].DataPoints.Clear();
                double totalMoney = 0;
                 
                foreach (var amount in amountList)
                {
                    totalMoney += amount.Value;
                    Chart.Series[0].DataPoints.Add(new DataPoint { AxisXLabel = amount.Key, YValue = amount.Value});
                }

                TxtTotalMoney.Text = "Total Money:" + totalMoney;
            }
        }

        private void CmbConsumeTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QueryConsumeRecordList();
        }

        private void CmbConsumeUserSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QueryConsumeRecordList();
        }

        private void DateTimeRangeChanged(object sender, SelectionChangedEventArgs e)
        {
            QueryConsumeRecordList();
        }

        private void RecordListSorting(object sender, DataGridSortingEventArgs e)
        {
            ListSortDirection dir = e.Column.SortDirection.GetValueOrDefault(ListSortDirection.Ascending);
            e.Column.SortDirection = dir;
            QueryConsumeRecordList(e.Column.SortMemberPath, dir == ListSortDirection.Ascending ? SortDir.ASC : SortDir.DESC);
            e.Handled = true;
        }

        private void RecordListPagerPagerPageIndexChanged(object sender, Control.PagerPageIndexChangedEventArgs e)
        {
            QueryConsumeRecordList();
        }

        private void ContentSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QueryConsumeRecordList();
        }
    }
}
