using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using AccountBook.DAL.Interface;
using AccountBook.Model;

namespace AccountBook.DAL.SQLiteImpl
{
    public class AccountRecordDAL : IAccountRecordDAL
    {
        private const string ESCAPER = @"\";

        public long AddAccountRecord(AccountRecord record)
        {
            string cmdText = @"INSERT INTO [AccountRecord] (
                                    [TypeId],[Money],[ConsumeTime],[RecordTime],[Memo],[UserId],[IsAccessorial]
                                )
                                VALUES (
                                    @typeId, @money, @consumeTime, @recordTime, @memo, @userId, @isAccessorial
                                )";
            var parameters = new[]
                {
                    new SQLiteParameter("@typeId", record.Type.TypeId),
                    new SQLiteParameter("@money", record.Money),
                    new SQLiteParameter("@consumeTime", record.ConsumeTime),
                    new SQLiteParameter("@recordTime", DateTime.Now),
                    new SQLiteParameter("@memo", record.Memo),
                    new SQLiteParameter("@userId", record.Consumer.UserId),
                    new SQLiteParameter("@isAccessorial", record.IsAccessorial)
                };
            SqliteHelper.ExecuteNonQuery(cmdText, parameters);

            //查询刚插入数据的历史记录ID
            cmdText = @"SELECT last_insert_rowid()";
            return (long)SqliteHelper.ExecuteScalar(cmdText);
        }

        public bool UpdateAccountRecord(AccountRecord record)
        {
            const string cmdText = @"UPDATE [AccountRecord]
                                SET [TypeId] = @typeId,
                                    [Money]=@money,
                                    [ConsumeTime]=@consumeTime,
                                    [RecordTime]=@recordTime,
                                    [Memo]= @memo,
                                    [UserId]= @userId,
                                    [IsAccessorial] = @isAccessorial
                                WHERE [Id]=@recordId";
            var parameters = new[]
                {
                    new SQLiteParameter("@typeId", record.Type.TypeId),
                    new SQLiteParameter("@money", record.Money),
                    new SQLiteParameter("@consumeTime", record.ConsumeTime),
                    new SQLiteParameter("@recordTime", DateTime.Now),
                    new SQLiteParameter("@memo", record.Memo),
                    new SQLiteParameter("@userId", record.Consumer.UserId),
                    new SQLiteParameter("@isAccessorial", record.IsAccessorial),
                    new SQLiteParameter("@recordId", record.Id)
                };
            var rowCount = SqliteHelper.ExecuteNonQuery(cmdText, parameters);

            return rowCount == 1;
        }

        public bool DeleteAccountRecord(long recordId)
        {
            const string cmdText = @"DELETE FROM [AccountRecord]
                                     WHERE [Id]=@recordId";
            var parameters = new[]
            {
                new SQLiteParameter("@recordId", recordId)
            };
            var rowCount = SqliteHelper.ExecuteNonQuery(cmdText, parameters);
            return rowCount == 1;
        }

        public List<AccountRecord> GetAccountRecordList(AccountRecordQueryOption option, out int recordCount, out decimal totalMoney)
        {
            string cmdText = @"SELECT R.[Id] As RecordId, R.[ConsumeTime], R.[Money], R.[Memo], R.[RecordTime], R.[IsAccessorial],
                                      U.[UserId], U.[UserName], U.[FriendlyName], 
                                      T.[TypeId], T.[ParentTypeId], T.[TypeName], T.[Category] 
                                FROM [AccountRecord] R 
                                     JOIN [USER] U ON R.[UserId] = U.[UserId]       
                                     JOIN [AccountType] T ON T.[TypeId] = R.[TypeId]
                                WHERE 1=1";

            cmdText = string.Format("{0} AND R.[ConsumeTime] >= '{1}'", cmdText, option.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"));
            cmdText = string.Format("{0} AND R.[ConsumeTime] < '{1}'", cmdText, option.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));

            if (!option.ShowAccessorial)
            {
                cmdText = string.Format("{0} AND R.[IsAccessorial] = 0", cmdText);
            }

            if (option.AccountType.TypeId != 0)
            {
                if (option.AccountType.ParentTypeId != 0)
                {
                    cmdText = string.Format("{0} AND R.[TypeId] = {1}", cmdText, option.AccountType.TypeId);
                }
                else
                {
                    cmdText = string.Format("{0} AND T.[ParentTypeId] = {1}", cmdText, option.AccountType.TypeId);
                }
            }

            if (option.UserId != 0)
            {
                cmdText = string.Format("{0} AND U.[UserId] = {1}", cmdText, option.UserId);
            }

            if(option.AccountCategory != AccountCategory.Undefined)
            {
                cmdText = string.Format("{0} AND T.[Category] = {1}", cmdText, (int)option.AccountCategory);
            }

            // Keyword
            if (!string.IsNullOrWhiteSpace(option.KeyWord))
            {
                cmdText = string.Format(@"{0} AND R.[Memo] like '%{1}%' escape '{2}'", cmdText, GetFinalKeyword(option.KeyWord), ESCAPER);
            }

            // Sort
            if (!string.IsNullOrWhiteSpace(option.SortName))
            {
                string sortName = option.SortName;
                switch (option.SortName)
                {
                    case "Memo":
                        sortName = "R.[Memo]";
                        break;
                    case "ConsumeTime":
                        sortName = "R.[ConsumeTime]";
                        break;
                    case "Money":
                        sortName = "R.[Money]";
                        break;
                    case "RecordTime":
                        sortName = "R.[RecordTime]";
                        break;
                    case "UserId":
                        sortName = "U.[UserId]";
                        break;
                    case "UserName":
                        sortName = "U.[UserName]";
                        break;
                    case "User.FriendlyName":
                        sortName = "U.[FriendlyName]";
                        break;
                    case "TypeId":
                        sortName = "T.[TypeId]";
                        break;
                    case "ParentTypeId":
                        sortName = "T.[ParentTypeId]";
                        break;
                    case "Type.TypeName":
                        sortName = "T.[TypeName]";
                        break;
                    case "Accessorial":
                        sortName = "R.[IsAccessorial]";
                        break;
                }
                cmdText = string.Format("{0} ORDER BY {1} {2}", cmdText, sortName, option.SortDir);
            }

            Debug.WriteLine(cmdText);

            var reader = SqliteHelper.ExecutePagerReader(out recordCount, option.PageIndex, option.PageSize, cmdText);

            cmdText = string.Format("SELECT SUM([Money]) FROM ({0})", cmdText);
            decimal.TryParse(SqliteHelper.ExecuteScalar(cmdText).ToString(), out totalMoney);

            return reader.ToConsumeRecordList();
        }

        public Dictionary<string, double> GetAccountAmountInfo(string format, AccountRecordQueryOption option)
        {
            string cmdText = string.Format(@"SELECT strftime('{0}', R.ConsumeTime) as 'Time', CAST(Sum(R.Money) as DOUBLE) as 'Money'
                                FROM [AccountRecord] R 
                                     JOIN [USER] U ON R.[UserId] = U.[UserId]       
                                     JOIN [AccountType] T ON T.[TypeId] = R.[TypeId]
                                WHERE 1=1", format);

            cmdText = string.Format("{0} AND R.[ConsumeTime] >= '{1}'", cmdText, option.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"));
            cmdText = string.Format("{0} AND R.[ConsumeTime] < '{1}'", cmdText, option.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
            if (!option.ShowAccessorial)
            {
                cmdText = string.Format("{0} AND R.[IsAccessorial] = 0", cmdText);
            }
            if (option.AccountType.TypeId != 0)
            {
                if (option.AccountType.ParentTypeId != 0)
                {
                    cmdText = string.Format("{0} AND R.[TypeId] = {1}", cmdText, option.AccountType.TypeId);
                }
                else
                {
                    cmdText = string.Format("{0} AND T.[ParentTypeId] = {1}", cmdText, option.AccountType.TypeId);
                }
            }

            if (option.UserId != 0)
            {
                cmdText = string.Format("{0} AND U.[UserId] = {1}", cmdText, option.UserId);
            }

            if (option.AccountCategory != AccountCategory.Undefined)
            {
                cmdText = string.Format("{0} AND T.[Category] = {1}", cmdText, (int)option.AccountCategory);
            }

            // Keyword
            if (!string.IsNullOrWhiteSpace(option.KeyWord))
            {
                cmdText = string.Format(@"{0} AND R.[Memo] like '%{1}%' escape '{2}'", cmdText, GetFinalKeyword(option.KeyWord), ESCAPER);
            }

            cmdText = string.Format("{0} GROUP BY  strftime('{1}', R.ConsumeTime)", cmdText, format);

            Debug.WriteLine(cmdText);

            var reader = SqliteHelper.ExecuteReader(cmdText);

            return reader.ToConsumeAmountDictionary();
        }

        public Dictionary<string, double> GetAccountTypeInfo(AccountRecordQueryOption option, int typeLevel)
        {
            string cmdText = string.Format(@"SELECT T.TypeId, T.TypeName, CAST(Sum(R.Money) as DOUBLE) as 'Money'
                                FROM [AccountRecord] R 
                                     JOIN [USER] U ON R.[UserId] = U.[UserId]       
                                     JOIN [AccountType] T ON T.[TypeId] = R.[TypeId]
                                WHERE 1=1 AND T.Level={0}", typeLevel);

            cmdText = string.Format("{0} AND R.[ConsumeTime] >= '{1}'", cmdText, option.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"));
            cmdText = string.Format("{0} AND R.[ConsumeTime] < '{1}'", cmdText, option.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
            if (!option.ShowAccessorial)
            {
                cmdText = string.Format("{0} AND R.[IsAccessorial] = 0", cmdText);
            }
            if (option.UserId != 0)
            {
                cmdText = string.Format("{0} AND U.[UserId] = {1}", cmdText, option.UserId);
            }

            if (option.AccountCategory != AccountCategory.Undefined)
            {
                cmdText = string.Format("{0} AND T.[Category] = {1}", cmdText, (int)option.AccountCategory);
            }

            // Keyword
            if (!string.IsNullOrWhiteSpace(option.KeyWord))
            {
                cmdText = string.Format(@"{0} AND R.[Memo] like '%{1}%' escape '{2}'", cmdText, GetFinalKeyword(option.KeyWord), ESCAPER);
            }

            cmdText = string.Format("{0} GROUP BY T.TypeId", cmdText);

            Debug.WriteLine(cmdText);

            var reader = SqliteHelper.ExecuteReader(cmdText);

            return reader.ToConsumeTypeInfoDictionary();
        }

        private string GetFinalKeyword(string originalKeyword)
        {
            return originalKeyword.Replace(ESCAPER, ESCAPER + ESCAPER).Replace("_", ESCAPER + "_").Replace("%", ESCAPER + "%");
        }
    }
}
