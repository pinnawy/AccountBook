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
        private bool _consumerTypeInitialized;
        private bool _consumerInitialized;
        private string _keyword;

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
                return _consumerTypeInitialized && _consumerInitialized;
            }
        }

        public QueryPanel()
        {
            InitializeComponent();
            this.DataContext = this;

            var style = Application.Current.Resources["ConsumerTypeItemPanelStyle"] as Style;
            if (style != null)
            {
                CmbConsumeType.ItemContainerStyle = style;
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
            ContextFactory.ConsumeTypeContext.GetAccountTypes(0, AccountCategory.Expense, result =>
            {
                AccountBookContext.Instance.SetExpenseTypeList(result.Value);
                CmbConsumeType.ItemsSource = AccountBookContext.Instance.ExtExpenseTypeList;
                _consumerTypeInitialized = true;

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
                var consumerType = CmbConsumeType.SelectedItem as AccountType;
                var args = new QueryConditionChangedEventArgs
                {
                    BeginTime = DpBeginDate.SelectedDate,
                    EndTime = DpEndDate.SelectedDate,
                    Consumer = CmbConsumeUser.SelectedItem as UserInfo,
                    AccountType = consumerType,
                    Keyword = _keyword
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
    }
}
