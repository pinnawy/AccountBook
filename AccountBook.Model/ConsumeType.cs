namespace AccountBook.Model
{
    /// <summary>
    /// 消费类别
    /// </summary>
    public class ConsumeType
    {
        private long _typeId;
        private long _parentTypeId;
        private string _typeName;
        private string _parentTypeName;

        public long TypeId
        {
            get { return _typeId; }
            set { _typeId = value; }
        }

        public long ParentTypeId
        {
            get { return _parentTypeId; }
            set { _parentTypeId = value; }
        }

        public string TypeName
        {
            get { return _typeName; }
            set { _typeName = value; }
        }

        public string ParentTypeName
        {
            get { return _parentTypeName; }
            set { _parentTypeName = value; }
        }

        public override string ToString()
        {
            return TypeName;
        }
    }
}
