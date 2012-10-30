using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using AccountBook.Model;
using AccountBook.Silverlight.Controls;
using AccountBook.Silverlight.Events;
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
        private DateTime? _begigDate;
        private DateTime? _endDate;
        private UserInfo _consumer;
        private ConsumeType _consumeType;
        private string _sortName = "ConsumeTime";
        private SortDir _sortDir = SortDir.ASC;
        private DataGridColumnHeader _currSortColumnHeader;

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
        }

        private void QueryPanelQueryConditionChanged(object sender, QueryConditionChangedEventArgs e)
        {
            _begigDate = e.BeginTime;
            _endDate = e.EndTime;
            _consumer = e.Consumer;
            _consumeType = e.ConsumeType;
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

            var consumeType = _consumeType;
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
                UserId = _consumer == null ? AccountBookContext.Instance.DefaultConsumer.UserId : _consumer.UserId,
                PageIndex = RecordsPager.PageIndex == -1 ? 0 : RecordsPager.PageIndex,
                PageSize = RecordsPager.PageSize,
                BeginTime = _begigDate.HasValue ? _begigDate.Value : DateTime.MinValue,
                EndTime = _endDate.HasValue ? _endDate.Value : DateTime.MaxValue,
                SortName = _sortName,
                SortDir = _sortDir
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

        private void PageSizeChanged(object sender, SelectionChangedEventArgs e)
        {
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

        private void ColumnHeaderMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var header = ((FrameworkElement)sender).Tag as DataGridColumnHeader;
            if (header != null)
            {
                // 通过HeaderName找到对应的Column
                string headerName = header.Content.ToString();
                var column = RecordsGrid.Columns.Where(col => col.Header.ToString() == headerName).Single();

                // 只对可以排序的Column进行处理
                if (column.CanUserSort)
                {
                    var newSortName = string.IsNullOrWhiteSpace(column.SortMemberPath) ? headerName : column.SortMemberPath.Trim();
                    if (newSortName == _sortName)
                    {
                        _sortDir = _sortDir == SortDir.ASC ? SortDir.DESC : SortDir.ASC;
                    }
                    else
                    {
                        _sortName = newSortName;
                        _sortDir = SortDir.ASC;
                    }

                    // 设置箭头状态
                    string sortState = _sortDir == SortDir.ASC ? "SortAsc" : "SortDesc";
                    VisualStateManager.GoToState(header, sortState, true);
                    if (_currSortColumnHeader != null && _currSortColumnHeader != header)
                    {
                        VisualStateManager.GoToState(_currSortColumnHeader, "Unsort", false);
                    }
                    _currSortColumnHeader = header;

                    // 查询
                    QueryRecords();
                }
            }

            e.Handled = true;
        }
    }
}
