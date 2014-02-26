using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using AccountBook.Model;
using AccountBook.SControls;
using AccountBook.Silverlight;
using AccountBook.Silverlight.Helpers;

namespace AccountBook.Records
{
    public partial class CreateEditRecordWindow : ChildWindow
    {
        private readonly bool _isUpdateRecord;      // 是否是更新记录
        private int _addRecordCount;                // 添加记录数
        private AccountCategory _category;

        public CreateEditRecordWindow(AccountRecord record, AccountCategory category)
        {
            InitializeComponent();
            _category = category;
            _isUpdateRecord = record.Id > 0;

            if (_isUpdateRecord)
            {
                Title = string.Format("Update {0} Record", category);
                BtnSave.Content = "Update";
                BtnSaveAdd.Visibility = Visibility.Collapsed;
            }
            else
            {
                Title = string.Format("Create {0} Record", category);
            }
            this.DataContext = record;

            this.Closing += new EventHandler<System.ComponentModel.CancelEventArgs>(CreateEditRecordWindowClosing);
        }

        private void CreateEditRecordWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.DialogResult = _addRecordCount > 0;
        }

        private void RecordDataFormAutoGeneratingField(object sender, DataFormAutoGeneratingFieldEventArgs e)
        {
            e.Field.Mode = DataFieldMode.Edit;

            if (e.PropertyName == "Type")
            {
                var accountTypeList = _category == AccountCategory.Expense ? AccountBookContext.Instance.ExpenseTypeList : AccountBookContext.Instance.IncomeTypeList;
                var cmbAccountType = new ComboBox();
                cmbAccountType.DisplayMemberPath = "TypeName";
                cmbAccountType.Name = "CmbAccountType";
                cmbAccountType.ItemsSource = accountTypeList;
                cmbAccountType.SelectedItem = accountTypeList[1];
                cmbAccountType.ItemContainerStyle = Application.Current.Resources["ExAccountTypeItemPanelStyle"] as Style;
                e.Field.ReplaceTextBox(cmbAccountType, Selector.SelectedIndexProperty, binding =>
                {
                    binding.Mode = BindingMode.TwoWay;
                    binding.Converter = new AccountTypeConverter(_category == AccountCategory.Expense ? AccountBookContext.Instance.ExpenseTypeList : AccountBookContext.Instance.IncomeTypeList);
                });
            }
            else if (e.PropertyName == "Consumer")
            {
                var cmbConsumer = new ComboBox();
                cmbConsumer.Name = "CmbConsumer";
                cmbConsumer.DisplayMemberPath = "FriendlyName";
                cmbConsumer.ItemsSource = AccountBookContext.Instance.ConsumerList;
                e.Field.ReplaceTextBox(cmbConsumer, Selector.SelectedIndexProperty, binding =>
                {
                    binding.Mode = BindingMode.TwoWay;
                    binding.Converter = new ConsumeUserConverter();
                });
            }
            else if (e.PropertyName == "Memo")
            {
                var memoTextBox = e.Field.Content as TextBox;
                if (memoTextBox != null)
                {
                    memoTextBox.Height = 120;
                    memoTextBox.Width = 180;
                    memoTextBox.TextWrapping = TextWrapping.Wrap;
                }
            }
        }

        private void BtnSaveClick(object sender, RoutedEventArgs e)
        {
            AddOrUpdateConsumeRecord(_isUpdateRecord, record =>
            {
                this.DialogResult = true;
            });
        }

        private void BtnSaveAddClick(object sender, RoutedEventArgs e)
        {
            RecordDataForm.CommitEdit();
            AddOrUpdateConsumeRecord(_isUpdateRecord, record =>
            {
                _addRecordCount++;
                TipPanel.Visibility = Visibility.Visible;
                // RecordDataForm.ClearValue(DataForm.CurrentItemProperty);
                this.DataContext = new AccountRecord
                {
                    Consumer = new UserInfo
                    {
                        UserId = record.Consumer.UserId,
                        FriendlyName = record.Consumer.FriendlyName,
                        UserName = record.Consumer.UserName,
                    },
                    ConsumeTime = record.ConsumeTime
                };
            });
        }

        private void CancelButtonClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = _addRecordCount > 0;
        }

        /// <summary>
        /// 新建或修改消费记录
        /// </summary>
        /// <param name="isUpdate">更新记录</param>
        /// <param name="callback">操作成功回调</param>
        private void AddOrUpdateConsumeRecord(bool isUpdate, Action<AccountRecord> callback)
        {
            var record = this.DataContext as AccountRecord;
            if (record != null)
            {
                if (record.CurrentOperation != null && !record.CurrentOperation.IsComplete)
                {
                    return;
                }

                if (RecordDataForm.ValidateItem())
                {
                    // 更新记录
                    if (isUpdate)
                    {
                        record.CurrentOperation = ContextFactory.RecordsContext.UpdateConsumeRecord(record, result =>
                        {
                            if (!result.HasError && result.Value)
                            {
                                if (callback != null)
                                {
                                    callback(record);
                                }
                            }
                            else
                            {
                                TipWindow.Alert("消费记录更新失败...");
                            }
                        }, null);
                    }
                    // 添加记录
                    else
                    {
                        record.CurrentOperation = ContextFactory.RecordsContext.AddConsumeRecord(record, result =>
                        {
                            if (!result.HasError && result.Value > 0)
                            {
                                if (callback != null)
                                {
                                    callback(record);
                                }
                            }
                            else
                            {
                                TipWindow.Alert("消费记录添加失败...");
                            }
                        }, null);
                    }
                }
            }
        }

        private void RecordDataFormContentLoaded(object sender, DataFormContentLoadEventArgs e)
        {
            var cmbConsumer = RecordDataForm.FindNameInContent("CmbConsumer") as ComboBox;
            if (cmbConsumer != null)
            {
                cmbConsumer.IsEnabled = e.Mode != DataFormMode.ReadOnly;
            }

            var cmbAccountType = RecordDataForm.FindNameInContent("CmbAccountType") as ComboBox;
            if (cmbAccountType != null)
            {
                cmbAccountType.IsEnabled = e.Mode != DataFormMode.ReadOnly;
            }
        }

        private void RecordDataFormBeginningEdit(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TipPanel.Visibility = Visibility.Collapsed;
        }

        private void RecordDataFormValidatingItem(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}

