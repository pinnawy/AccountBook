namespace AccountBook.Model
{
    /// <summary>
    /// 账目类别
    /// </summary>
    public class AccountType
    {
        private long _typeId;
        private long _parentTypeId;
        private string _typeName;
        private string _parentTypeName;
        private AccountCategory _category;

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

        public AccountCategory Category
        {
            get { return _category; }
            set { _category = value; }
        }

        public override string ToString()
        {
            return TypeName;
        }
    }
}
