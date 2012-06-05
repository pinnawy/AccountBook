using System;
using System.Collections.Generic;
using System.Data.SQLite;
using AccountBook.Model;

namespace AccountBook.Dal.Impl
{
    /// <summary>
    /// SqliteDataReader帮助类
    /// Author: yuwang 2011-3-9
    /// </summary>
    public static class SqliteDataReaderHelper
    {
        public static User ToUser(this SQLiteDataReader reader)
        {
            User user = null;
            if (reader.Read())
            {
                user = new User();
                user.UserId = (int)reader.SafeRead<long>("UserId");
                user.UserName = reader.SafeRead<string>("UserName");
                user.FullName = reader.SafeRead<string>("FullName");
            }

            return user;
        }

        public static List<User> ToUserList(this SQLiteDataReader reader)
        {
            List<User> userList = new List<User>();
            while (reader.Read())
            {
                User user = new User();
                user.UserId = (int)reader.SafeRead<long>("UserId");
                user.UserName = reader.SafeRead<string>("UserName");
                user.FullName = reader.SafeRead<string>("FullName");
                userList.Add(user);
            }

            return userList;
        }

        public static List<ConsumeRecord> ToConsumeRecordList(this SQLiteDataReader reader)
        {
            List<ConsumeRecord> recordList = new List<ConsumeRecord>();

            while(reader.Read())
            {
                ConsumeRecord record = new ConsumeRecord();
                record.Id = reader.SafeRead<long>("RecordId");
                record.Money = reader.SafeRead<decimal>("Money");
                record.ConsumeTime = reader.SafeRead<DateTime>("ConsumeTime");
                record.RecordTime = reader.SafeRead<DateTime>("RecordTime");
                record.Memo = reader.SafeRead<string>("Memo");

                User user = new User();
                user.UserId = reader.SafeRead<long>("UserId");
                user.UserName = reader.SafeRead<string>("UserName");
                user.FullName = reader.SafeRead<string>("FullName");
                record.User = user;

                ConsumeType type = new ConsumeType();
                type.TypeId = reader.SafeRead<long>("TypeId");
                type.ParentTypeId = reader.SafeRead<long>("ParentTypeId");
                type.TypeName = reader.SafeRead<string>("TypeName");
                record.Type = type;

                recordList.Add(record);
            }

            return recordList;
        }

        public static List<ConsumeType> ToConsumeTypeList(this SQLiteDataReader reader)
        {
            List<ConsumeType> typeList = new List<ConsumeType>();

            while (reader.Read())
            {
                ConsumeType type = new ConsumeType();
                type.TypeId = reader.SafeRead<long>("TypeId");
                type.ParentTypeId = reader.SafeRead<long>("ParentTypeId");
                type.TypeName = reader.SafeRead<string>("TypeName");
                type.ParentTypeName = reader.SafeRead<string>("ParentTypeName");
                typeList.Add(type);
            }

            return typeList;
        }

        public static List<KeyValuePair<string, double>> ToConsumeAmountList(this SQLiteDataReader reader)
        {
            List<KeyValuePair<string, double>> list = new List<KeyValuePair<string, double>>();
            while(reader.Read())
            {
                string time = reader.SafeRead<string>("Time");
                double money = reader.SafeRead<double>("Money");
                KeyValuePair<string, double> amount = new KeyValuePair<string, double>(time, money);
                list.Add(amount);
            }
            return list;
        }

        /// <summary>
        /// 将Sqlite读取的数据转换成相应类型的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="reader">SqliteDataReader</param>
        /// <param name="fieldName">数据库字段名称</param>
        /// <param name="defaultValue">转换失败的返回的默认值</param>
        public static T SafeRead<T>(this SQLiteDataReader reader, string fieldName, T defaultValue)
        {
            try
            {
                var ordinal = reader.GetOrdinal(fieldName);
                if (reader.IsDBNull(ordinal))//if column "fieldName" doesn't exist, will throw an exception
                    return defaultValue;

                return (T)Convert.ChangeType(reader[ordinal], defaultValue.GetType());
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 将Sqlite读取的数据转换成相应类型的数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="reader">SqliteDataReader</param>
        /// <param name="fieldName">数据库字段名称</param>
        public static T SafeRead<T>(this SQLiteDataReader reader, string fieldName)
        {
            var obj = reader[fieldName];
            if (obj == DBNull.Value)
                return default(T);
            return (T)obj;
        }
    }
}
