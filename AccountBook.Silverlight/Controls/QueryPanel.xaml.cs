using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using AccountBook.Model;
using AccountBook.Silverlight.Events;

namespace AccountBook.Silverlight.Controls
{
    public partial class QueryPanel : UserControl
    {
        public event EventHandler<QueryConditionChangedEventArgs> QueryConditionChanged;

        public static readonly DependencyProperty ConsumerListProperty = DependencyProperty.Register(
            "ConsumerList", typeof(ObservableCollection<UserInfo>), typeof(QueryPanel),
            new PropertyMetadata(new ObservableCollection<UserInfo> { AccountBookContext.Instance.DefaultConsumer }));

        public static readonly DependencyProperty ConsumeTypeListProperty = DependencyProperty.Register(
            "ConsumeTypeList", typeof(ObservableCollection<ConsumeType>), typeof(QueryPanel),
            new PropertyMetadata(new ObservableCollection<ConsumeType> { AccountBookContext.Instance.DefaultConsumeType }));

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<ConsumeType> ConsumeTypeList
        {
            get { return (ObservableCollection<ConsumeType>)GetValue(ConsumeTypeListProperty); }
            set { SetValue(ConsumeTypeListProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<UserInfo> ConsumerList
        {
            get { return (ObservableCollection<UserInfo>)GetValue(ConsumerListProperty); }
            set { SetValue(ConsumerListProperty, value); }
        }

        public QueryPanel()
        {
            InitializeComponent();
        }

        private void CtlQueryConditionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (QueryConditionChanged != null)
            {
                var args = new QueryConditionChangedEventArgs
                {
                    BeginTime = DpBeginDate.SelectedDate,
                    EndTime = DpEndDate.SelectedDate,
                    Consumer = CmbConsumeUser.SelectedValue as UserInfo,
                    ConsumeType = CmbConsumeType.SelectedValue as ConsumeType
                };

                QueryConditionChanged(this, args);
            }
        }
    }
}
