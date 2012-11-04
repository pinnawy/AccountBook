using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using AccountBook.Model;
using System.Linq;

namespace AccountBook.Silverlight
{
    /// <summary>
    /// AccountBook Context
    /// </summary>
    public sealed class AccountBookContext
    {
        private AccountBookContext()
        {
            ModuleCache = new Dictionary<Type, UserControl>();

            _consumerList = new ObservableCollection<UserInfo>();
            _extUserInfoList = new ObservableCollection<UserInfo> { DefaultConsumer };
            _accountTypeList = new ObservableCollection<AccountType>();
            _extAccountTypeList = new ObservableCollection<AccountType> {DefaultAccountType};
            _expenseTypeList = new ObservableCollection<AccountType>();
            _extExpenseTypeList = new ObservableCollection<AccountType> { DefaultAccountType };
            _incomeTypeList = new ObservableCollection<AccountType>();
            _extIncomeTypeList = new ObservableCollection<AccountType> {DefaultAccountType};
        }

        private static AccountBookContext _instance;

        public static AccountBookContext Instance
        {
            get { return _instance ?? (_instance = new AccountBookContext()); }
        }

        /// <summary>
        /// 模块缓存
        /// </summary>
        public Dictionary<Type, UserControl> ModuleCache
        {
            get;
            private set;
        }

        private AccountType _defaultAccountType;
        /// <summary>
        /// 默认消费类别
        /// </summary>
        public AccountType DefaultAccountType
        {
            get
            {
                return _defaultAccountType ?? (_defaultAccountType = new AccountType { TypeId = 0, ParentTypeId = 0, TypeName = "全部" });
            }
        }

        private UserInfo _defaultConsumer;
        /// <summary>
        /// 默认消费人
        /// </summary>
        public UserInfo DefaultConsumer
        {
            get
            {
                return _defaultConsumer ?? (_defaultConsumer = new UserInfo { UserId = 0, FriendlyName = "全部" });
            }
        }

        private readonly ObservableCollection<UserInfo> _consumerList;
        /// <summary>
        /// 消费人列表
        /// </summary>
        public ObservableCollection<UserInfo> ConsumerList
        {
            get
            {
                return _consumerList;
            }
        }

        private readonly ObservableCollection<UserInfo> _extUserInfoList;
        /// <summary>
        /// 扩展的消费人列表
        /// </summary>
        public ObservableCollection<UserInfo> ExtUserInfoList
        {
            get
            {
                return _extUserInfoList;
            }
        }

        /// <summary>
        /// 设置消费人列表
        /// </summary>
        /// <param name="userInfoList">消费人列表</param>
        public void SetConsumerList(IEnumerable<UserInfo> userInfoList)
        {
            if (userInfoList == null)
            {
                return;
            }

            _consumerList.Clear();
            for (int i = _extUserInfoList.Count - 1; i > 0; i--)
            {
                _extUserInfoList.RemoveAt(i);
            }

            foreach (var userInfo in userInfoList)
            {
                _consumerList.Add(userInfo);
                _extUserInfoList.Add(userInfo);
            }
        }

        private ObservableCollection<AccountType> _accountTypeList;
        /// <summary>
        /// 账目类别列表
        /// </summary>
        public ObservableCollection<AccountType> AccountTypeList
        {
            get
            {
                return _accountTypeList;
            }
        }

        private ObservableCollection<AccountType> _extAccountTypeList;
        /// <summary>
        /// 扩展的账目类别列表
        /// </summary>
        public ObservableCollection<AccountType> ExtAccountTypeList
        {
            get
            {
                return _extAccountTypeList;
            }
        }


        private ObservableCollection<AccountType> _expenseTypeList;
        /// <summary>
        /// 消费类别列表
        /// </summary>
        public ObservableCollection<AccountType> ExpenseTypeList
        {
            get
            {
                return _expenseTypeList;
            }
        }

        private ObservableCollection<AccountType> _extExpenseTypeList;
        /// <summary>
        /// 扩展的消费类别列表
        /// </summary>
        public ObservableCollection<AccountType> ExtExpenseTypeList
        {
            get
            {
                return _extExpenseTypeList;
            }
        }

        private ObservableCollection<AccountType> _incomeTypeList;
        /// <summary>
        /// 收入类别列表
        /// </summary>
        public ObservableCollection<AccountType> IncomeTypeList
        {
            get
            {
                return _incomeTypeList;
            }
        }

        private ObservableCollection<AccountType> _extIncomeTypeList;
        /// <summary>
        /// 扩展的收入类别列表
        /// </summary>
        public ObservableCollection<AccountType> ExtIncomeTypeList
        {
            get
            {
                return _extIncomeTypeList;
            }
        }

        /// <summary>
        /// 设置账目类别
        /// </summary>
        /// <param name="accountTypeList">账目类别列表</param>
        public void SetAccountTypeList(IList<AccountType> accountTypeList)
        {
            SetAccountTypeList(accountTypeList, _accountTypeList, _extAccountTypeList);

            var expenseTypeList = accountTypeList.Where(accountType => accountType.Category == AccountCategory.Expense).ToList();
            SetAccountTypeList(expenseTypeList, _expenseTypeList, _extExpenseTypeList);

            var incomeTypeList = accountTypeList.Where(accountType => accountType.Category == AccountCategory.Income).ToList();
            SetAccountTypeList(incomeTypeList, _incomeTypeList, _extIncomeTypeList);
        }

        private void SetAccountTypeList(IList<AccountType> accountTypeList, ObservableCollection<AccountType> nomalTypeList, ObservableCollection<AccountType> extTypeList)
        {
            if(accountTypeList == null)
            {
                return;
            }

            nomalTypeList.Clear();
            for (int i = extTypeList.Count - 1; i > 0; i--)
            {
                extTypeList.RemoveAt(i);
            }

            var parentTypeList = accountTypeList.Where(accountType => accountType.ParentTypeId == 0);
            foreach (AccountType parentType in parentTypeList)
            {
                IEnumerable<AccountType> subTypeList = accountTypeList.Where(accountType => accountType.ParentTypeId == parentType.TypeId);
                nomalTypeList.Add(parentType.Clone());
                extTypeList.Add(parentType.Clone());
                foreach (var consumeType in subTypeList)
                {
                    nomalTypeList.Add(consumeType.Clone());
                    extTypeList.Add(consumeType.Clone());
                }
            }
        }
    }
}
