using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using AccountBook.Model;
using AccountBook.Silverlight.Controls;
using AccountBook.Silverlight.Helpers;

namespace AccountBook.Silverlight.Views
{
    public partial class Records : BasePage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private readonly List<int> _recordsCounts = new List<int>();
        private int _currPageIndex = -1;

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

        public Records()
        {
            InitializeComponent();
            this.DataContext = this;
#if !DEBUG
            BeginDate.SelectedDate = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day);
#endif
            InitConsumeTypes();
            InitConsumeUsers();
        }

        /// <summary>
        /// 初始化消费类别下拉框
        /// </summary>
        private void InitConsumeTypes()
        {
            Binding bind = new Binding("ExtConsumeTypeList");
            bind.Source = AccountBookContext.Instance;
            CmbConsumeType.SetBinding(ItemsControl.ItemsSourceProperty, bind);
            CmbConsumeType.SelectedIndex = 0;

            ContextFactory.ConsumeTypeContext.GetConsumeTypes(0, result =>
            {
                AccountBookContext.Instance.SetConsumeTypeList(result.Value);
                CmbConsumeType.SelectedIndex = 0;
            }, null);
        }

        /// <summary>
        /// 初始化消费用户下拉框
        /// </summary>
        private void InitConsumeUsers()
        {
            CmbConsumeUser.SelectedValuePath = "UserId";
            CmbConsumeUser.DisplayMemberPath = "FriendlyName";

            Binding bind = new Binding("ExtUserInfoList");
            bind.Source = AccountBookContext.Instance;
            CmbConsumeUser.SetBinding(ItemsControl.ItemsSourceProperty, bind);
            CmbConsumeUser.SelectedIndex = 0;

            ContextFactory.UserContext.GetUserList(result =>
            {
                AccountBookContext.Instance.SetConsumerList(result.Value);
                CmbConsumeUser.SelectedIndex = 0;
            }, null);
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
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

            ConsumeType consumeType;
            if (CmbConsumeType.SelectedItem == null)
            {
                consumeType = AccountBookContext.Instance.DefaultConsumeType;
            }
            else
            {
                var selectedConsumeType = (ConsumeType)CmbConsumeType.SelectedItem;
                consumeType = new ConsumeType { TypeId = selectedConsumeType.TypeId, ParentTypeId = selectedConsumeType.ParentTypeId };
            }

            var option = new ConsumeRecordQueryOption
            {
                ConsumeType = consumeType,
                UserId = CmbConsumeUser.SelectedValue == null ? AccountBookContext.Instance.DefaultConsumer.UserId : (long)CmbConsumeUser.SelectedValue,
                PageIndex = RecordsPager.PageIndex == -1 ? 0 : RecordsPager.PageIndex,
                PageSize = RecordsPager.PageSize,
                BeginTime = BeginDate.SelectedDate.HasValue ? BeginDate.SelectedDate.Value : DateTime.MinValue,
                EndTime = EndDate.SelectedDate.HasValue ? EndDate.SelectedDate.Value : DateTime.MaxValue,
                SortName = "RecordTime",
                SortDir = SortDir.ASC
            };

            // 获取服务端消费记录数据
            this.CurrQueryOperation = ContextFactory.RecordsContext.GetConsumeRecordList(option, records =>
            {
                this.RecordsGrid.ItemsSource = records.Value.Records;
                TotalMoney.Text = records.Value.TotalMoney.ToString();

                if (records.Value.TotalCount != RecordsPager.ItemCount)
                {
                    _recordsCounts.Clear();
                    for (int count = 0; count < records.Value.TotalCount; count++)
                    {
                        _recordsCounts.Add(count);
                    }
                    PagedCollectionView pcv = new PagedCollectionView(_recordsCounts);
                    this.RecordsPager.Source = pcv;
                    this.RecordsPager.PageIndex = _currPageIndex;
                }
            }, null);
        }

        private void RecordsPagerPageIndexChanged(object sender, EventArgs e)
        {
            if (RecordsPager.PageIndex != _currPageIndex)
            {
                QueryRecords();
            }

            _currPageIndex = RecordsPager.PageIndex;
        }

        private void RecordsPagerPageIndexChanging(object sender, CancelEventArgs e)
        {
            _currPageIndex = RecordsPager.PageIndex;
        }

        private void BtnCreateRecordClick(object sender, RoutedEventArgs e)
        {
            ConsumeRecord record = new ConsumeRecord
            {
                ConsumeTime = DateTime.Now
            };
            AddOrEditRecord(record);
        }

        private void QueryConditionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox && ((ComboBox)sender).ItemsSource == null)
            {
                return;
            }

            if (RecordsPager.Source != null && RecordsPager.PageSize > 0)
            {
                RecordsPager.PageIndex = 0;
            }

            QueryRecords();
        }

        private void RecordsGridBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            ConsumeRecord record = e.Row.DataContext as ConsumeRecord;
            if (record != null)
            {
                e.Cancel = true;
                AddOrEditRecord(record);
            }
        }

        /// <summary>
        /// 添加或编辑消费记录
        /// </summary>
        /// <param name="record">消费记录实体对象</param>
        private void AddOrEditRecord(ConsumeRecord record)
        {
            var win = new CreateEditRecordWindow(record);
            win.Closing += delegate
            {
                if (win.DialogResult.HasValue && win.DialogResult.Value)
                {
                    QueryRecords();
                }
            };
            win.Show();
        }

        private void BtnDeleteRecordClick(object sender, RoutedEventArgs e)
        {
            ConsumeRecord record = ((FrameworkElement)sender).DataContext as ConsumeRecord;
            if (record != null)
            {
                TipWindow.Confirm("Are you sure to delete this record?", isConfirm =>
                {
                    if (isConfirm)
                    {
                        this.CurrQueryOperation = ContextFactory.RecordsContext.DeleteConsumeRecord(record.Id, result =>
                        {
                            if (result.Error != null)
                            {
                                ErrorWindow.CreateNew(result.Error);
                            }
                            else if (!result.Value)
                            {
                                TipWindow.Alert("记录删除失败!");
                            }
                            else
                            {
                                QueryRecords();
                            }
                        }, null);
                    }
                });
            }
            else
            {
                TipWindow.Alert("Invalid Action, record is null");
            }
        }
    }
}
