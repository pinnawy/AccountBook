using System;
using System.Collections.Generic;
using AccountBook.Bll.Interface;
using AccountBook.Dal.Interface;
using AccountBook.Library;
using AccountBook.Model;

namespace AccountBook.Bll.Impl
{
    public class ConsumeTypeBll : IConsumeTypeBll
    {
        private readonly IConsumeTypeDal _consumeTypedDal = UnityContext.LoadDalModel<IConsumeTypeDal>();

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
