using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using AccountBook.DAL.Interface;
using AccountBook.Model;

namespace AccountBook.DAL.SQLiteImpl
{
    public class ConsumeRecordDAL : IConsumeRecordDAL
    {
        public long AddConsumeRecord(ConsumeRecord record)
        {
            string cmdText = @"INSERT INTO [ConsumeRecord] (
                                    [TypeId],[Money],[ConsumeTime],[RecordTime],[Memo],[UserId]
                                )
                                VALUES (
                                    @typeId, @money, @consumeTime, @recordTime, @memo, @userId
                                )";
            var parameters = new[]
                {
                    new SQLiteParameter("@typeId", record.Type.TypeId),
                    new SQLiteParameter("@money", record.Money),
                    new SQLiteParameter("@consumeTime", record.ConsumeTime),
                    new SQLiteParameter("@recordTime", DateTime.Now),
                    new SQLiteParameter("@memo", record.Memo),
                    new SQLiteParameter("@userId", record.Consumer.UserId)
                };
            SqliteHelper.ExecuteNonQuery(cmdText, parameters);

            //查询刚插入数据的历史记录ID
            cmdText = @"SELECT last_insert_rowid()";
            return (long)SqliteHelper.ExecuteScalar(cmdText);
        }

        public bool UpdateConsumeRecord(ConsumeRecord record)
        {
            string cmdText = @"UPDATE [ConsumeRecord]
                                SET [TypeId] = @typeId,
                                    [Money]=@money,
                                    [ConsumeTime]=@consumeTime,
                                    [RecordTime]=@recordTime,
                                    [Memo]= @memo,
                                    [UserId]= @userId
                                WHERE [Id]=@recordId";
            var parameters = new[]
                {
                    new SQLiteParameter("@typeId", record.Type.TypeId),
                    new SQLiteParameter("@money", record.Money),
                    new SQLiteParameter("@consumeTime", record.ConsumeTime),
                    new SQLiteParameter("@recordTime", DateTime.Now),
                    new SQLiteParameter("@memo", record.Memo),
                    new SQLiteParameter("@userId", record.Consumer.UserId),
                    new SQLiteParameter("@recordId", record.Id)
                };
            var rowCount = SqliteHelper.ExecuteNonQuery(cmdText, parameters);

            return rowCount == 1;
        }

        public List<ConsumeRecord> GetConsumeRecordList(ConsumeRecordQueryOption option, out int recordCount, out decimal totalMoney)
        {
            string cmdText = @"SELECT R.[Id] As RecordId, R.[ConsumeTime], R.[Money], R.[Memo], R.[RecordTime], 
                                      U.[UserId], U.[UserName], U.[FriendlyName], 
                                      T.[TypeId], T.[ParentTypeId], T.[TypeName] 
                                FROM [ConsumeRecord] R 
                                     JOIN [USER] U ON R.[UserId] = U.[UserId]       
                                     JOIN [ConsumeType] T ON T.[TypeId] = R.[TypeId]
                                WHERE 1=1";

            cmdText = string.Format("{0} AND R.[ConsumeTime] >= '{1}'", cmdText, option.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"));
            cmdText = string.Format("{0} AND R.[ConsumeTime] < '{1}'", cmdText, option.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));

            if (option.ConsumeType.TypeId != 0)
            {
                if (option.ConsumeType.ParentTypeId != 0)
                {
                    cmdText = string.Format("{0} AND R.[TypeId] = {1}", cmdText, option.ConsumeType.TypeId);
                }
                else
                {
                    cmdText = string.Format("{0} AND T.[ParentTypeId] = {1}", cmdText, option.ConsumeType.TypeId);
                }
            }

            if (option.UserId != 0)
            {
                cmdText = string.Format("{0} AND U.[UserId] = {1}", cmdText, option.UserId);
            }

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
                        sortName = "T.[UserId]";
                        break;
                    case "ParentTypeId":
                        sortName = "T.[ParentTypeId]";
                        break;
                    case "Type.TypeName":
                        sortName = "T.[TypeName]";
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

        public List<KeyValuePair<string, double>> GetConsumeAmountList(string format, ConsumeRecordQueryOption option)
        {
            string cmdText = string.Format(@"SELECT strftime('{0}', R.ConsumeTime) as 'Time', Sum(R.Money) as 'Money'
                                FROM [ConsumeRecord] R 
                                     JOIN [USER] U ON R.[UserId] = U.[UserId]       
                                     JOIN [ConsumeType] T ON T.[TypeId] = R.[TypeId]
                                WHERE 1=1", format);

            cmdText = string.Format("{0} AND R.[ConsumeTime] >= '{1}'", cmdText, option.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"));
            cmdText = string.Format("{0} AND R.[ConsumeTime] < '{1}'", cmdText, option.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));

            if (option.ConsumeType.TypeId != 0)
            {
                if (option.ConsumeType.ParentTypeId != 0)
                {
                    cmdText = string.Format("{0} AND R.[TypeId] = {1}", cmdText, option.ConsumeType.TypeId);
                }
                else
                {
                    cmdText = string.Format("{0} AND T.[ParentTypeId] = {1}", cmdText, option.ConsumeType.TypeId);
                }
            }

            if (option.UserId != 0)
            {
                cmdText = string.Format("{0} AND U.[UserId] = {1}", cmdText, option.UserId);
            }

            cmdText = string.Format("{0} GROUP BY  strftime('{1}', R.ConsumeTime)", cmdText, format);


            Debug.WriteLine(cmdText);

            var reader = SqliteHelper.ExecuteReader(cmdText);

            return reader.ToConsumeAmountList();
        }
    }
}
