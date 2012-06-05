
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
    public class ConsumeTypeService : DomainService, IConsumeTypeBLL
    {
        private readonly IConsumeTypeBLL _consumeTypeBLL = UnityContext.LoadBLLModel<IConsumeTypeBLL>();

        public List<ConsumeType> GetConsumeTypes(int parentTypeId)
        {
            return _consumeTypeBLL.GetConsumeTypes(parentTypeId);
        }

        public List<ConsumeType> GetConsumeSubTypes()
        {
            return _consumeTypeBLL.GetConsumeSubTypes();
        }
    }
}


