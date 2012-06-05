using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AccountBook.Model;
using AccountBookWin.Common;
using MessageBox = System.Windows.Forms.MessageBox;

namespace AccountBookWin
{
    /// <summary>
    /// Interaction logic for ConsumeRecordWin.xaml
    /// </summary>
    public partial class ConsumeRecordWin : Window
    {
        public DateTime DateTimeNow
        {
            get { return DateTime.Now; }
            set { Debug.WriteLine(value); } 
        }

        public ConsumeRecordWin()
        {
            InitializeComponent();

            CmbConsumeType.SelectedValuePath = "TypeId";
            var types = BllFactory.GetConsumeTypeBll().GetConsumeSubTypes();
            CmbConsumeType.ItemsSource = types;

            CmbUser.SelectedValuePath = "UserId";
            var users = BllFactory.GetUserBll().GetUserList();
            CmbUser.ItemsSource = users;

            CollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(CmbConsumeType.ItemsSource);
            PropertyGroupDescription groupDescription = new PropertyGroupDescription("ParentTypeName");
            if (cv.GroupDescriptions != null)
            {
                cv.GroupDescriptions.Add(groupDescription);
            }

            ConsumeTime.SelectedDate = DateTimeNow;
        }

        public ConsumeRecordWin(ConsumeRecordWinMode mode)
        {
            InitializeComponent();
        }

        private void BtnSaveClick(object sender, RoutedEventArgs e)
        {
            DialogResult = AddConsumeRecord();
        }

        private bool AddConsumeRecord()
        {
            if(RecordDataIsValid())
            {
                ConsumeRecord record = new ConsumeRecord
                {
                    User = CmbUser.SelectionBoxItem as User,
                    Type = CmbConsumeType.SelectedItem as ConsumeType,
                    Money = decimal.Parse(TxtMoney.Text),
                    ConsumeTime = ConsumeTime.SelectedDate.GetValueOrDefault(),
                    Memo = TxtMemo.Text
                };

                var recordId = BllFactory.GetConsumeRecordBll().AddConsumeRecord(record);
                return recordId > 0;
            }

            return false;
        }

        private void BtnCloseClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnSaveAddClick(object sender, RoutedEventArgs e)
        {
            if (AddConsumeRecord())
            {
                TxtMemo.Text = string.Empty;
                TxtMoney.Text = "0";
            }
        }

        private bool RecordDataIsValid()
        {
            // 检查消费金额是否合法
            if(Validation.GetHasError(TxtMoney))
            {
                MessageBox.Show(Validation.GetErrors(TxtMoney)[0].ErrorContent.ToString());
                TxtMoney.Focus();
                return false;
            }

            // 检查消费记录备注是否合法
            if (Validation.GetHasError(TxtMemo))
            {
                MessageBox.Show(Validation.GetErrors(TxtMemo)[0].ErrorContent.ToString());
                TxtMemo.Focus();
                return false;
            }

            return true;
        }
    }

    public enum ConsumeRecordWinMode
    {
        Add,

        Show
    }
}
