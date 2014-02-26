using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;
using AccountBook.Model;
using AccountBook.SControls;
using AccountBook.Silverlight;
using AccountBook.Silverlight.Helpers;
using Visifire.Charts;

namespace AccountBook.Statistics
{
    public partial class MainPage : BasePage
    {
        private Dictionary<string, double> _dataInfos;

        private DateTime? _beginDate;
        private DateTime? _endDate;
        private AccountType _accountType;
        private UserInfo _consumer;
        private string _keyword;
        private RenderAs _renderAs = RenderAs.Column;
        private string _statisticsRange = "Month";
        private AccountCategory _accountCategory = AccountCategory.Expense;
        private StatisticsType _statisticsType = StatisticsType.AccountType;
        private bool _showAccessorial;

        public MainPage()
        {
            InitializeComponent();

            this.DataContext = this;

            this.Loaded += new System.Windows.RoutedEventHandler(MainPageLoaded);
        }

        void MainPageLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            QueryCondionPanel.QueryConditionChanged -= QueryPanelQueryConditionChanged;
            QueryCondionPanel.QueryConditionChanged += QueryPanelQueryConditionChanged;
        }

        private void QueryPanelQueryConditionChanged(object sender, Silverlight.Events.QueryConditionChangedEventArgs e)
        {
            _beginDate = e.BeginTime;
            _endDate = e.EndTime;
            _accountType = e.AccountType;
            _consumer = e.Consumer;
            _keyword = e.Keyword;
            _accountCategory = e.AccountCategory;
            _showAccessorial = e.ShowAccessorial;

            QueryRecords();
        }

        /// <summary>
        /// 查询消费记录
        /// </summary>
        private void QueryRecords()
        {
            if (this.CurrQueryOperation != null && !this.CurrQueryOperation.IsComplete)
            {
                return;
            }

            if (_accountType == null)
            {
                _accountType = AccountBookContext.Instance.DefaultAccountType;
            }

            var option = new AccountRecordQueryOption
            {
                AccountType = _accountType.Clone(),
                UserId = _consumer == null ? AccountBookContext.Instance.DefaultConsumer.UserId : _consumer.UserId,
                PageIndex = 0,
                PageSize = int.MaxValue,
                BeginTime = _beginDate.HasValue ? _beginDate.Value : DateTime.MinValue,
                EndTime = _endDate.HasValue ? _endDate.Value.AddDays(1) : DateTime.MaxValue,
                KeyWord = _keyword,
                AccountCategory = _accountCategory,
                ShowAccessorial = _showAccessorial
            };

            if (_statisticsType == StatisticsType.AmountInfo)
            {
                if (_statisticsRange == "Month")
                {
                    // 获取服务端消费记录数据
                    this.CurrQueryOperation = ContextFactory.RecordsContext.GetConsumeAmountByMonth(option, result =>
                    {
                        if (result.IsCanceled)
                        {
                            return;
                        }

                        if (result.HasError)
                        {
                            ErrorWindow.CreateNew(result.Error);
                        }
                        else
                        {
                            _dataInfos = result.Value;
                            DrawChart(_dataInfos, _renderAs);
                        }

                    }, null);
                }
                else
                {
                    // 获取服务端消费记录数据
                    this.CurrQueryOperation = ContextFactory.RecordsContext.GetConsumeAmountByYear(option, result =>
                    {
                        if (result.IsCanceled)
                        {
                            return;
                        }

                        if (result.HasError)
                        {
                            ErrorWindow.CreateNew(result.Error);
                        }
                        else
                        {
                            _dataInfos = result.Value;
                            DrawChart(_dataInfos, _renderAs);
                        }

                    }, null);
                }
            }
            else if(_statisticsType == StatisticsType.AccountType)
            {
                this.CurrQueryOperation = ContextFactory.RecordsContext.GetAccountTypeInfo(option, 2, result =>
                {
                    if (result.IsCanceled)
                    {
                        return;
                    }

                    if (result.HasError)
                    {
                        ErrorWindow.CreateNew(result.Error);
                    }
                    else
                    {
                        _dataInfos = result.Value;
                        DrawChart(_dataInfos, _renderAs);
                    }
                }, null);
            }
        }

        private void DrawChart(Dictionary<string, double> amountInfos, RenderAs renderAs)
        {
            if (amountInfos == null)
            {
                return;
            }

            double totalMoney = 0;

            var chart = new Chart { Watermark = false };
            chart.AxesY.Add(new Axis { Suffix = "元" });

            var dataSeries = new DataSeries { RenderAs = renderAs, LabelEnabled = true };

            foreach (var amountInfo in amountInfos)
            {
                var month = amountInfo.Key;
                var money = amountInfo.Value;
                totalMoney += money;
                dataSeries.DataPoints.Add(new DataPoint { AxisXLabel = month, YValue = money });
            }
            chart.Series.Add(dataSeries);

            ChartPanel.Content = chart;

            TxtTotalMoney.Text =  totalMoney.ToString();
            if(_statisticsType == StatisticsType.AmountInfo)
            {
                if(_statisticsRange == "Month")
                {
                    int monthCount;

                    if(amountInfos.Count < 1)
                    { 
                        monthCount = 0;
                    }
                    else if(amountInfos.Count < 2)
                    {
                        monthCount = 1;
                    }
                    else
                    {
                        List<DateTime> months = new List<DateTime>();
                        foreach(var time in amountInfos.Keys)
                        {
                            months.Add(DateTime.Parse(time.Replace("年", "-").Replace("月", "-") + "1 0:0:0"));
                        }
                        monthCount = GetMonthCount(months.Min(), months.Max());
                    }

                    if(monthCount > 0)
                    {
                        TxtAverageMoney.Text = Math.Round((totalMoney / monthCount), 2).ToString();
                    }
                    else
                    {
                        TxtAverageMoney.Text = "0";
                    }
                }
                else
                {
                   
                    int yearCount;
                    if(amountInfos.Count < 1)
                    {
                        yearCount = 0;
                    }else if(amountInfos.Count < 2)
                    {
                        yearCount = 1;
                    }
                    else
                    {
                        List<DateTime> years = new List<DateTime>();
                        foreach (var time in amountInfos.Keys)
                        {
                            years.Add(DateTime.Parse(time.Replace("年", "-1-1 0:0:0")));

                        }

                        yearCount = years.Max().Year - years.Min().Year + 1;
                    }
                    if (yearCount > 0)
                    {
                        TxtAverageMoney.Text = Math.Round((totalMoney / yearCount),2).ToString();
                    }
                    else
                    {
                        TxtAverageMoney.Text = "0";
                    }
                }
            }
        }

        private int GetMonthCount(DateTime c1, DateTime c2)
        {
            int count = 0;
            if (c1 > c2)
            {
                DateTime tmp = c1;
                c1 = c2;
                c2 = tmp;
            }
            while (c2 >= c1)
            {
                count++;
                //MessageBox.Show(c1.Month.ToString());
                c1 = c1.AddMonths(1);
            }

            return count;
        }

        private void CmbChartShapeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                _renderAs = (RenderAs)e.AddedItems[0];
                DrawChart(_dataInfos, _renderAs);
            }
        }

        private void RdbStatisticRnageChecked(object sender, System.Windows.RoutedEventArgs e)
        {
            _statisticsRange = ((RadioButton)sender).Content.ToString();
            QueryRecords();
        }

        private void BtnExportClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var chart = ChartPanel.Content as Chart;
            if (chart != null)
            {
                chart.Export();
            }
        }

        private void CmbStatisticsTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count > 0)
            {
                _statisticsType = (StatisticsType)e.AddedItems[0];

                if (StatisticRangePanel != null && QueryCondionPanel!= null)
                {
                    if (_statisticsType == StatisticsType.AccountType)
                    {
                        StatisticRangePanel.Visibility = System.Windows.Visibility.Collapsed;
                        QueryCondionPanel.ShowSelectType = false;
                        TxtAverage.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        StatisticRangePanel.Visibility = System.Windows.Visibility.Visible;
                        QueryCondionPanel.ShowSelectType = true;
                        TxtAverage.Visibility = System.Windows.Visibility.Visible;
                    }
                }
                
                QueryRecords();
            }
        }
    }
}
