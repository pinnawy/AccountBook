using System.Collections.Generic;
using AccountBook.BLL.Interface;
using AccountBook.DAL.Interface;
using AccountBook.Library;
using AccountBook.Model;

namespace AccountBook.BLL
{
    public class AccountTypeBLL : IAccountTypeBLL
    {
        private readonly IConsumeTypeDAL _consumeTypedDal = UnityContext.LoadDALModel<IConsumeTypeDAL>();

        public List<AccountType> GetAccountTypes(int parentTypeId, AccountCategory categoty)
        {
            return _consumeTypedDal.GetAccountTypes(parentTypeId,categoty);
        }

        public List<AccountType> GetAccountSubTypes(AccountCategory categoty)
        {
            return _consumeTypedDal.GetAccountSubTypes(categoty);
        }
    }
}
