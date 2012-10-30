using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel.DomainServices.Client;
using System.Windows.Controls;
using AccountBook.Model;
using AccountBook.Silverlight;
using AccountBook.Silverlight.Helpers;
using Visifire.Charts;

namespace AccountBook.Statistics
{
    public partial class MainPage : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private OperationBase _currQueryOperation;
        internal OperationBase CurrQueryOperation
        {
            get
            {
                return _currQueryOperation;
            }
            set
            {
                if (_currQueryOperation != value)
                {
                    if (_currQueryOperation != null)
                    {
                        _currQueryOperation.Completed -= CurrQueryOperationChanged;
                    }

                    _currQueryOperation = value;

                    if (_currQueryOperation != null)
                    {
                        _currQueryOperation.Completed += CurrQueryOperationChanged;
                    }

                    this.CurrentOperationChanged();
                }
            }
        }

        private void CurrQueryOperationChanged(object sender, EventArgs e)
        {
            this.CurrentOperationChanged();
        }

        /// <summary>
        /// Gets a value indicating whether the user is presently being registered or logged in.
        /// </summary>
        public bool IsOperationing
        {
            get
            {
                return this.CurrQueryOperation != null && !this.CurrQueryOperation.IsComplete;
            }
        }

        public bool CanQueryRecords
        {
            get { return !IsOperationing; }
        }

        /// <summary>
        /// Helper method for when the current operation changes.
        /// Used to raise appropriate property change notifications.
        /// </summary>
        private void CurrentOperationChanged()
        {
            this.NotifyPropertyChanged("IsOperationing");
            this.NotifyPropertyChanged("CanQueryRecords");
        }

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
            QueryRecords(e.BeginTime, e.EndTime, e.ConsumeType, e.Consumer);
        }

        /// <summary>
        /// 查询消费记录
        /// </summary>
        private void QueryRecords(DateTime? beginDate, DateTime? endDate, ConsumeType consumeType, UserInfo consumer)
        {
            if (this.CurrQueryOperation != null && !this.CurrQueryOperation.IsComplete)
            {
                return;
            }

            if (consumeType == null)
            {
                consumeType = AccountBookContext.Instance.DefaultConsumeType;
            }
            else
            {
                consumeType = new ConsumeType { TypeId = consumeType.TypeId, ParentTypeId = consumeType.ParentTypeId };
            }

            var option = new ConsumeRecordQueryOption
            {
                ConsumeType = consumeType,
                UserId = consumer == null ? AccountBookContext.Instance.DefaultConsumer.UserId : consumer.UserId,
                PageIndex = 0,
                PageSize = int.MaxValue,
                BeginTime = beginDate.HasValue ? beginDate.Value : DateTime.MinValue,
                EndTime = endDate.HasValue ? endDate.Value : DateTime.MaxValue
            };

            // 获取服务端消费记录数据
            this.CurrQueryOperation = ContextFactory.RecordsContext.GetConsumeAmountByMonth(option, result =>
            {
                if (result.HasError)
                {
                    ErrorWindow.CreateNew(result.Error);
                }
                else
                {
                    double totalMoney = 0;
                    if (MonthChart != null)
                    {
                        MonthChart.Series[0].DataPoints.Clear();
                        foreach (var amountInfo in result.Value)
                        {
                            var infos = amountInfo.Split(new[] { '_' });
                            var month = infos[0];
                            var money = double.Parse(infos[1]);
                            totalMoney += money;
                            MonthChart.Series[0].DataPoints.Add(new DataPoint { AxisXLabel = month, YValue = money });
                        }

                        TxtTotalMoney.Text = "TotalMoney:" + totalMoney;
                    }
                }

            }, null);
        }
    }
}
