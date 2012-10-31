using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
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
            _consumerList = new ObservableCollection<UserInfo>();
            _extUserInfoList = new ObservableCollection<UserInfo> { DefaultConsumer };
            _consumeTypeList = new ObservableCollection<ConsumeType>();
            _extConsumeTypeList = new ObservableCollection<ConsumeType> { DefaultConsumeType };
        }

        private static AccountBookContext _instance;

        public static AccountBookContext Instance
        {
            get { return _instance ?? (_instance = new AccountBookContext()); }
        }

        /// <summary>
        /// 管理模块
        /// </summary>
        public UIElement ManageModule
        {
            get;
            set;
        }

        /// <summary>
        /// 统计模块
        /// </summary>
        public UIElement StatisticsModule
        {
            get;
            set;
        }

        private ConsumeType _defaultConsumeType;
        /// <summary>
        /// 默认消费类别
        /// </summary>
        public ConsumeType DefaultConsumeType
        {
            get
            {
                return _defaultConsumeType ?? (_defaultConsumeType = new ConsumeType { TypeId = 0, ParentTypeId = 0, TypeName = "全部" });
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

        private ObservableCollection<ConsumeType> _consumeTypeList;
        /// <summary>
        /// 消费类别列表
        /// </summary>
        public ObservableCollection<ConsumeType> ConsumeTypeList
        {
            get
            {
                return _consumeTypeList;
            }
        }

        private ObservableCollection<ConsumeType> _extConsumeTypeList;
        /// <summary>
        /// 扩展的消费类别列表
        /// </summary>
        public ObservableCollection<ConsumeType> ExtConsumeTypeList
        {
            get
            {
                return _extConsumeTypeList;
            }
        }

        /// <summary>
        /// 设置消费类别列表
        /// </summary>
        /// <param name="consumeTypeList">消费类别列表</param>
        public void SetConsumeTypeList(IEnumerable<ConsumeType> consumeTypeList)
        {
            if (consumeTypeList == null)
            {
                return;
            }

            _consumeTypeList.Clear();
            for (int i = _extConsumeTypeList.Count - 1; i > 0; i--)
            {
                _extConsumeTypeList.RemoveAt(i);
            }

            var parentTypeList = consumeTypeList.Where(consumeType => consumeType.ParentTypeId == 0);
            foreach (ConsumeType parentType in parentTypeList)
            {
                IEnumerable<ConsumeType> subTypeList = consumeTypeList.Where(consumeType => consumeType.ParentTypeId == parentType.TypeId);
                _consumeTypeList.Add(parentType);
                _extConsumeTypeList.Add(parentType);
                foreach (var consumeType in subTypeList)
                {
                    _consumeTypeList.Add(consumeType);
                    _extConsumeTypeList.Add(consumeType);
                }
            }
        }
    }
}
