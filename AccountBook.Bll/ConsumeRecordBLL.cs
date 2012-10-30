using System.Collections.Generic;
using System.Linq;
using AccountBook.BLL.Interface;
using AccountBook.DAL.Interface;
using AccountBook.Library;
using AccountBook.Model;

namespace AccountBook.BLL
{
    public class ConsumeRecordBLL : IConsumeRecordBLL
    {
        private readonly IConsumeRecordDAL _consumeRecordDal = UnityContext.LoadDALModel<IConsumeRecordDAL>();

        public long AddConsumeRecord(ConsumeRecord record)
        {
            return _consumeRecordDal.AddConsumeRecord(record);
        }

        public bool UpdateConsumeRecord(ConsumeRecord record)
        {
            return _consumeRecordDal.UpdateConsumeRecord(record);
        }

        public bool DeleteConsumeRecord(long recordId)
        {
            return _consumeRecordDal.DeleteConsumeRecord(recordId);
        }

        public ConsumeRecordsResult GetConsumeRecordList(ConsumeRecordQueryOption option)
        {
            int recordCount;
            decimal totalMoney;

            List<ConsumeRecord> records = _consumeRecordDal.GetConsumeRecordList(option, out recordCount, out totalMoney);

            ConsumeRecordsResult result = new ConsumeRecordsResult
            {
                Records = records,
                TotalCount = recordCount,
                TotalMoney = totalMoney
            };
            return result;
        }

        public List<string> GetConsumeAmountByMonth(ConsumeRecordQueryOption option)
        {
            List<KeyValuePair<string, double>> tempList = _consumeRecordDal.GetConsumeAmountList("%Y-%m", option);

            return tempList.Select(item => item.Key + "_" + item.Value).ToList();
        }

        public List<KeyValuePair<string, double>> GetConsumeAmountByYear(ConsumeRecordQueryOption option)
        {
            return _consumeRecordDal.GetConsumeAmountList("%m", option);
        }
    }
}
