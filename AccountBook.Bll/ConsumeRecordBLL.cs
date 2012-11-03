using System.Collections.Generic;
using AccountBook.BLL.Interface;
using AccountBook.DAL.Interface;
using AccountBook.Library;
using AccountBook.Model;

namespace AccountBook.BLL
{
    public class ConsumeRecordBLL : IConsumeRecordBLL
    {
        private readonly IAccountRecordDAL _accountRecordDAL = UnityContext.LoadDALModel<IAccountRecordDAL>();

        public long AddConsumeRecord(AccountRecord record)
        {
            return _accountRecordDAL.AddAccountRecord(record);
        }

        public bool UpdateConsumeRecord(AccountRecord record)
        {
            return _accountRecordDAL.UpdateAccountRecord(record);
        }

        public bool DeleteConsumeRecord(long recordId)
        {
            return _accountRecordDAL.DeleteAccountRecord(recordId);
        }

        public AccountRecordsResult GetConsumeRecordList(AccountRecordQueryOption option)
        {
            int recordCount;
            decimal totalMoney;

            List<AccountRecord> records = _accountRecordDAL.GetAccountRecordList(option, out recordCount, out totalMoney);

            AccountRecordsResult result = new AccountRecordsResult
            {
                Records = records,
                TotalCount = recordCount,
                TotalMoney = totalMoney
            };
            return result;
        }

        public Dictionary<string, double> GetConsumeAmountByMonth(AccountRecordQueryOption option)
        {
            return _accountRecordDAL.GetAccountAmountInfo("%Y年%m月", option);
        }

        public Dictionary<string, double> GetConsumeAmountByYear(AccountRecordQueryOption option)
        {
            return _accountRecordDAL.GetAccountAmountInfo("%Y年", option);
        }
    }
}
