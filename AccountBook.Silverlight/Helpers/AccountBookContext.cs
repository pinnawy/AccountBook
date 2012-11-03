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
            _expenseTypeList = new ObservableCollection<AccountType>();
            _extExpenseTypeList = new ObservableCollection<AccountType> { DefaultAccountType };
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

        /// <summary>
        /// 设置消费类别列表
        /// </summary>
        /// <param name="expenseTypeList">消费类别列表</param>
        public void SetExpenseTypeList(IEnumerable<AccountType> expenseTypeList)
        {
            if (expenseTypeList == null)
            {
                return;
            }

            _expenseTypeList.Clear();
            for (int i = _extExpenseTypeList.Count - 1; i > 0; i--)
            {
                _extExpenseTypeList.RemoveAt(i);
            }

            var parentTypeList = expenseTypeList.Where(consumeType => consumeType.ParentTypeId == 0);
            foreach (AccountType parentType in parentTypeList)
            {
                IEnumerable<AccountType> subTypeList = expenseTypeList.Where(consumeType => consumeType.ParentTypeId == parentType.TypeId);
                _expenseTypeList.Add(parentType);
                _extExpenseTypeList.Add(parentType);
                foreach (var consumeType in subTypeList)
                {
                    _expenseTypeList.Add(consumeType);
                    _extExpenseTypeList.Add(consumeType);
                }
            }
        }
    }
}
