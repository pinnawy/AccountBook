using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

using AccountBook.Model;
using AccountBook.SControls;
using AccountBook.Silverlight;
using AccountBook.Silverlight.Events;
using AccountBook.Silverlight.Helpers;

namespace AccountBook.Records
{
    public partial class MainPage : BasePage
    {
        private readonly List<int> _recordsCounts = new List<int>();
        private int _currPageIndex = -1;
        private DateTime? _begigDate;
        private DateTime? _endDate;
        private UserInfo _consumer;
        private ConsumeType _consumeType;
        private string _sortName = "ConsumeTime";
        private SortDir _sortDir = SortDir.ASC;
        private string _keyword;
        private DataGridColumnHeader _currSortColumnHeader;

        public MainPage()
        {
            InitializeComponent();

            this.DataContext = this;

#if !DEBUG
            _begigDate = QueryCondionPanel.BeginTime = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day);
#endif
            QueryRecords();
        }

        private void QueryPanelQueryConditionChanged(object sender, QueryConditionChangedEventArgs e)
        {
            _begigDate = e.BeginTime;
            _endDate = e.EndTime;
            _consumer = e.Consumer;
            _consumeType = e.ConsumeType;
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

            if (_consumeType == null)
            {
                _consumeType = AccountBookContext.Instance.DefaultConsumeType;
            }

            var option = new ConsumeRecordQueryOption
            {
                ConsumeType = _consumeType.Clone(),
                UserId = _consumer == null ? AccountBookContext.Instance.DefaultConsumer.UserId : _consumer.UserId,
                PageIndex = RecordsPager.PageIndex == -1 ? 0 : RecordsPager.PageIndex,
                PageSize = RecordsPager.PageSize,
                BeginTime = _begigDate.HasValue ? _begigDate.Value : DateTime.MinValue,
                EndTime = _endDate.HasValue ? _endDate.Value : DateTime.MaxValue,
                SortName = _sortName,
                SortDir = _sortDir,
                KeyWord = _keyword
            };

            // 获取服务端消费记录数据
            this.CurrQueryOperation = ContextFactory.RecordsContext.GetConsumeRecordList(option, operation =>
            {
                if (operation.IsCanceled)
                {
                    return;
                }

                this.RecordsGrid.ItemsSource = operation.Value.Records;
                TotalMoney.Text = operation.Value.TotalMoney.ToString();

                if (operation.Value.TotalCount != RecordsPager.ItemCount)
                {
                    _recordsCounts.Clear();
                    for (int count = 0; count < operation.Value.TotalCount; count++)
                    {
                        _recordsCounts.Add(count);
                    }
                    var pcv = new PagedCollectionView(_recordsCounts);
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
            var record = new ConsumeRecord
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
            var record = e.Row.DataContext as ConsumeRecord;
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
            var record = ((FrameworkElement)sender).DataContext as ConsumeRecord;
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
