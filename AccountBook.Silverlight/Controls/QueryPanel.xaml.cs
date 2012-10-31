using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AccountBook.Model;
using AccountBook.Silverlight.Events;
using AccountBook.Silverlight.Helpers;

namespace AccountBook.Silverlight.Controls
{
    public partial class QueryPanel : UserControl
    {
        private bool _consumerTypeInitialized;
        private bool _consumerInitialized;


        public event EventHandler<QueryConditionChangedEventArgs> QueryConditionChanged;

        public static readonly DependencyProperty ConsumerListProperty = DependencyProperty.Register(
            "ConsumerList", typeof(ObservableCollection<UserInfo>), typeof(QueryPanel),
            new PropertyMetadata(new ObservableCollection<UserInfo> { AccountBookContext.Instance.DefaultConsumer }));

        public static readonly DependencyProperty ConsumeTypeListProperty = DependencyProperty.Register(
            "ConsumeTypeList", typeof(ObservableCollection<ConsumeType>), typeof(QueryPanel),
            new PropertyMetadata(new ObservableCollection<ConsumeType> { AccountBookContext.Instance.DefaultConsumeType }));

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

#if !DEBUG
            DpBeginDate.SelectedDate = DateTime.Now.Date.AddDays(1 - DateTime.Now.Day);
#endif
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
            ContextFactory.ConsumeTypeContext.GetConsumeTypes(0, result =>
            {
                AccountBookContext.Instance.SetConsumeTypeList(result.Value);
                CmbConsumeType.ItemsSource = AccountBookContext.Instance.ExtConsumeTypeList;
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

        private void CtlQueryConditionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QueryConditionChanged != null)
            {
                var consumerType = CmbConsumeType.SelectedItem as ConsumeType;
                var args = new QueryConditionChangedEventArgs
                {
                    BeginTime = DpBeginDate.SelectedDate,
                    EndTime = DpEndDate.SelectedDate,
                    Consumer = CmbConsumeUser.SelectedItem as UserInfo,
                    ConsumeType = consumerType
                };

                QueryConditionChanged(this, args);
            }
        }
    }
}
