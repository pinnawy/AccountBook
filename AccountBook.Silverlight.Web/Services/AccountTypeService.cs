
using AccountBook.BLL.Interface;
using AccountBook.Library;
using AccountBook.Model;

namespace AccountBook.Silverlight.Web.Services
{
    using System.Collections.Generic;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;


    // TODO: Create methods containing your application logic.
    [EnableClientAccess()]
    public class AccountTypeService : DomainService, IAccountTypeBLL
    {
        private readonly IAccountTypeBLL _accountTypeBLL = UnityContext.LoadBLLModel<IAccountTypeBLL>();

        public List<AccountType> GetAccountTypes(int parentTypeId, AccountCategory category)
        {
            return _accountTypeBLL.GetAccountTypes(parentTypeId, category);
        }

        public List<AccountType> GetAccountSubTypes(AccountCategory category)
        {
            return _accountTypeBLL.GetAccountSubTypes(category);
        }
    }
}


