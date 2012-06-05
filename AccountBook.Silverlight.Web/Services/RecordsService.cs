using System.Threading;
using AccountBook.BLL.Interface;
using AccountBook.Library;
using AccountBook.Model;

namespace AccountBook.Silverlight.Web
{
    using System.Collections.Generic;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;

    // TODO: Create methods containing your application logic.
    [EnableClientAccess()]
    public class RecordsService : DomainService, IConsumeRecordBLL
    {
        private readonly IConsumeRecordBLL _consumeRecordBLL = UnityContext.LoadBLLModel<IConsumeRecordBLL>();

        public long AddConsumeRecord(ConsumeRecord record)
        {
            return _consumeRecordBLL.AddConsumeRecord(record);
        }

        public bool UpdateConsumeRecord(ConsumeRecord record)
        {
            return _consumeRecordBLL.UpdateConsumeRecord(record);
        }

        public List<KeyValuePair<string, double>> GetConsumeAmountByMonth(ConsumeRecordQueryOption option)
        {
            return _consumeRecordBLL.GetConsumeAmountByMonth(option);
        }

        public List<KeyValuePair<string, double>> GetConsumeAmountByYear(ConsumeRecordQueryOption option)
        {
            return _consumeRecordBLL.GetConsumeAmountByYear(option);
        }

        public ConsumeRecordsResult GetConsumeRecordList(ConsumeRecordQueryOption option)
        {
            return _consumeRecordBLL.GetConsumeRecordList(option);
        }
    }
}


