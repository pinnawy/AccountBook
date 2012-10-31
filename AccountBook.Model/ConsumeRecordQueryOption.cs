using System;

namespace AccountBook.Model
{
    public class ConsumeRecordQueryOption : QueryOptionBase
    {
        public ConsumeRecordQueryOption()
        {
            ConsumeType = new ConsumeType { ParentTypeId = 0, TypeId = 0 };
            UserId = 0;
            PageIndex = 1;
            PageSize = 10;
            BeginTime = DateTime.MinValue;
            EndTime = DateTime.MaxValue;
            SortName = string.Empty;
            SortDir = SortDir.DESC;
        }

        private DateTime _beginTime;
        private DateTime _endTime;
        private ConsumeType _type;
        private long _userId;
        private string _keyword;

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime BeginTime
        {
            get { return _beginTime; }
            set { _beginTime = value; }
        }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }

        /// <summary>
        /// 消费类别
        /// </summary>
        public ConsumeType ConsumeType
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        /// <summary>
        /// 消费备注关键字
        /// </summary>
        public string KeyWord
        {
            get { return _keyword; }
            set { _keyword = value; }
        }
    }
}
