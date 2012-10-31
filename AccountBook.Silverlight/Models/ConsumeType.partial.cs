namespace AccountBook.Model
{
    public partial class ConsumeType
    {
        public ConsumeType Clone()
        {
            return new ConsumeType
            {
                TypeId = TypeId,
                ParentTypeId = ParentTypeId,
                TypeName = TypeName,
                ParentTypeName = ParentTypeName
            };
        }
    }
}
