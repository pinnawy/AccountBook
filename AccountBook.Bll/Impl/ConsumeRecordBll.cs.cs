using System.Collections.Generic;
using AccountBook.Bll.Interface;
using AccountBook.Dal.Interface;
using AccountBook.Library;
using AccountBook.Model;

namespace AccountBook.Bll.Impl
{
    public class ConsumeRecordBll : IConsumeRecordBll
    {
        private readonly IConsumeRecordDal _consumeRecordDal = UnityContext.LoadDalModel<IConsumeRecordDal>();

        public long AddConsumeRecord(ConsumeRecord record)
        {
            return _consumeRecordDal.AddConsumeRecord(record);
        }

        public List<ConsumeRecord> GetConsumeRecordList(ConsumeRecortQueryOption option, out int recordCount, out decimal totalMoney)
        {
            return _consumeRecordDal.GetConsumeRecordList(option, out recordCount, out totalMoney);
        }

        public List<KeyValuePair<string, double>> GetConsumeAmountByMonth(ConsumeRecortQueryOption option)
        {
            return _consumeRecordDal.GetConsumeAmountList("%Y-%m", option);
        }

        public List<KeyValuePair<string, double>> GetConsumeAmountByYear(ConsumeRecortQueryOption option)
        {
            return _consumeRecordDal.GetConsumeAmountList("%m", option);
        }
    }
}
