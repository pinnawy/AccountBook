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

        public long AddConsumeRecord(AccountRecord record)
        {
            return _consumeRecordBLL.AddConsumeRecord(record);
        }

        public bool UpdateConsumeRecord(AccountRecord record)
        {
            return _consumeRecordBLL.UpdateConsumeRecord(record);
        }

        public bool DeleteConsumeRecord(long recordId)
        {
            return _consumeRecordBLL.DeleteConsumeRecord(recordId);
        }

        public Dictionary<string, double> GetConsumeAmountByMonth(AccountRecordQueryOption option)
        {
            return _consumeRecordBLL.GetConsumeAmountByMonth(option);
        }

        public Dictionary<string, double> GetConsumeAmountByYear(AccountRecordQueryOption option)
        {
            return _consumeRecordBLL.GetConsumeAmountByYear(option);
        }

        public AccountRecordsResult GetConsumeRecordList(AccountRecordQueryOption option)
        {
            return _consumeRecordBLL.GetConsumeRecordList(option);
        }
    }
}


