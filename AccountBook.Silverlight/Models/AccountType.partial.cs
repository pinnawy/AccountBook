namespace AccountBook.Model
{
    public partial class AccountType
    {
        public AccountType Clone()
        {
            return new AccountType
            {
                TypeId = TypeId,
                ParentTypeId = ParentTypeId,
                TypeName = TypeName,
                ParentTypeName = ParentTypeName
            };
        }
    }
}
