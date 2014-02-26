using System;
using AccountBook.Model;

namespace AccountBook.Silverlight.Events
{
    public class QueryConditionChangedEventArgs : EventArgs
    {
        public DateTime? BeginTime
        {
            get;
            set;
        }

        public DateTime? EndTime
        {
            get;
            set;
        }

        public UserInfo Consumer
        {
            get;
            set;
        }

        public AccountType AccountType
        {
            get;
            set;
        }

        /// <summary>
        /// 显示附加记录
        /// </summary>
        public bool ShowAccessorial
        {
            get;
            set;
        }

        /// <summary>
        /// 关键字
        /// </summary>
        public string Keyword
        {
            get;
            set;
        }

        public AccountCategory AccountCategory
        {
            get;
            set;
        }
    }
}