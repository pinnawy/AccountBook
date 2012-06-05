using System.Collections.Generic;
using AccountBook.BLL.Interface;
using AccountBook.DAL.Interface;
using AccountBook.Library;
using AccountBook.Model;

namespace AccountBook.BLL
{
    public class ConsumeTypeBLL : IConsumeTypeBLL
    {
        private readonly IConsumeTypeDAL _consumeTypedDal = UnityContext.LoadDALModel<IConsumeTypeDAL>();

        public List<ConsumeType> GetConsumeTypes(int parentTypeId)
        {
            return _consumeTypedDal.GetConsumeTypes(parentTypeId);
        }

        public List<ConsumeType> GetConsumeSubTypes()
        {
            return _consumeTypedDal.GetConsumeSubTypes();
        }
    }
}
