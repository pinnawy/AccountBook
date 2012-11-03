using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel.DomainServices.Client;
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
        private Dictionary<string, double> _amountInfos;

        private DateTime? _beginDate;
        private DateTime? _endDate;
        private AccountType _accountType;
        private UserInfo _consumer;
        private string _keyword;
        private RenderAs _renderAs = RenderAs.Column;
        private string _statisticsRange = "Month";


        public MainPage()
        {
            InitializeComponent();

            this.DataContext = this;

            this.Loaded += new System.Windows.RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
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
                EndTime = _endDate.HasValue ? _endDate.Value : DateTime.MaxValue,
                KeyWord = _keyword
            };

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
                        _amountInfos = result.Value;
                        DrawChart(_amountInfos, _renderAs);
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
                        _amountInfos = result.Value;
                        DrawChart(_amountInfos, _renderAs);
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

            TxtTotalMoney.Text = "TotalMoney:" + totalMoney;
        }

        private void CmbChartShapeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                _renderAs = (RenderAs)e.AddedItems[0];
                DrawChart(_amountInfos, _renderAs);
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
    }
}
