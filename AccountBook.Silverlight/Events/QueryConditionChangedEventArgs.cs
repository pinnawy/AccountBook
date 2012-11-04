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