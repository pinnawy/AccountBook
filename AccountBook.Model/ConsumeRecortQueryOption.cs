using System;
namespace AccountBook.Model
{
    public class ConsumeRecortQueryOption : QueryOptionBase
    {
        public ConsumeRecortQueryOption()
        {
            ConsumeType = new ConsumeType{ParentTypeId = 0, TypeId = 0};
            UserId = 0;
            PageIndex = 1;
            PageSize = 10;
            SortName = string.Empty;
            SortDir = SortDir.DESC;
        }

        private DateTime _beginTime;
        private DateTime _endTime;
        private ConsumeType _type;
        private long _userId;

        public DateTime BeginTime
        {
            get { return _beginTime; }
            set { _beginTime = value; }
        }

        public DateTime EndTime
        {
            get { return _endTime; }
            set { _endTime = value; }
        }

        public ConsumeType ConsumeType
        {
            get { return _type; }
            set { _type = value; }
        }

        public long UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }
    }
}
