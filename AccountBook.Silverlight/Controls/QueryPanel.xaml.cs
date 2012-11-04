using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AccountBook.Model;
using AccountBook.Silverlight.Events;
using AccountBook.Silverlight.Helpers;

namespace AccountBook.Silverlight.Controls
{
    public partial class QueryPanel : UserControl
    {
        private bool _accountTypeInitialized;
        private bool _consumerInitialized;
        private string _keyword;
        private AccountCategory _accountCategory = AccountCategory.Expense;

        public event EventHandler<QueryConditionChangedEventArgs> QueryConditionChanged;

        public static readonly DependencyProperty BeginTimeProperty = DependencyProperty.Register(
        "BeginTime",
        typeof(DateTime?)
        , typeof(QueryPanel),
        new PropertyMetadata(null));

        /// <summary>
        /// 
        /// </summary>
        public DateTime? BeginTime
        {
            get { return (DateTime?)GetValue(BeginTimeProperty); }
            set { SetValue(BeginTimeProperty, value); }
        }

        /// <summary>
        /// 数据初始化完成
        /// </summary>
        private bool DataInitialized
        {
            get
            {
                return _accountTypeInitialized && _consumerInitialized;
            }
        }

        public QueryPanel()
        {
            InitializeComponent();
            this.DataContext = this;

            var style = Application.Current.Resources["AccountTypeItemPanelStyle"] as Style;
            if (style != null)
            {
                CmbAccountType.ItemContainerStyle = style;
            }

            if (!DesignerProperties.IsInDesignTool)
            {
                InitConsumeTypes();
                InitConsumeUsers();
            }
        }

        /// <summary>
        /// 初始化消费类别下拉框
        /// </summary>
        private void InitConsumeTypes()
        {
            ContextFactory.ConsumeTypeContext.GetAccountTypes(0, AccountCategory.Undefined, result =>
            {
                AccountBookContext.Instance.SetAccountTypeList(result.Value);
                //CmbAccountType.ItemsSource = AccountBookContext.Instance.ExtAccountTypeList;
                CmbAccountType.ItemsSource = AccountBookContext.Instance.ExtExpenseTypeList;
                _accountTypeInitialized = true;

                if (DataInitialized)
                {
                    CtlQueryConditionChanged(this, null);
                }
            }, null);
        }

        /// <summary>
        /// 初始化消费用户下拉框
        /// </summary>
        private void InitConsumeUsers()
        {
            ContextFactory.UserContext.GetUserList(result =>
            {
                AccountBookContext.Instance.SetConsumerList(result.Value);
                CmbConsumeUser.ItemsSource = AccountBookContext.Instance.ExtUserInfoList;
                _consumerInitialized = true;

                if (DataInitialized)
                {
                    CtlQueryConditionChanged(this, null);
                }
            }, null);
        }

        private void RiseQueryConditionChangedEvent()
        {
            if (QueryConditionChanged != null)
            {
                var consumerType = CmbAccountType.SelectedItem as AccountType;
                var args = new QueryConditionChangedEventArgs
                {
                    BeginTime = DpBeginDate.SelectedDate,
                    EndTime = DpEndDate.SelectedDate,
                    Consumer = CmbConsumeUser.SelectedItem as UserInfo,
                    AccountType = consumerType,
                    Keyword = _keyword,
                    AccountCategory = _accountCategory
                };

                QueryConditionChanged(this, args);
            }
        }

        private void CtlQueryConditionChanged(object sender, SelectionChangedEventArgs e)
        {
            RiseQueryConditionChangedEvent();
        }

        private void TxtKeywordLostFocus(object sender, RoutedEventArgs e)
        {
            if (_keyword != TxtKeyword.Text.Trim())
            {
                _keyword = TxtKeyword.Text.Trim();
                RiseQueryConditionChangedEvent();
            }
        }

        private void TxtKeywordKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && _keyword != TxtKeyword.Text.Trim())
            {
                _keyword = TxtKeyword.Text.Trim();
                RiseQueryConditionChangedEvent();
            }
        }

        private void CmbCategorySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(CmbAccountType != null && e.AddedItems.Count>0)
            {
                switch(e.AddedItems[0].ToString())
                {
                    case "支出":
                        _accountCategory = AccountCategory.Expense;
                        CmbAccountType.ItemsSource = AccountBookContext.Instance.ExtExpenseTypeList;
                        break;
                    case "收入":
                        _accountCategory = AccountCategory.Income;
                        CmbAccountType.ItemsSource = AccountBookContext.Instance.ExtIncomeTypeList;
                        break;
                    default:
                        _accountCategory = AccountCategory.Undefined;
                        CmbAccountType.ItemsSource = AccountBookContext.Instance.ExtAccountTypeList;
                        break;
                }

                CmbAccountType.SelectedIndex = 0;
            }
        }
    }
}
