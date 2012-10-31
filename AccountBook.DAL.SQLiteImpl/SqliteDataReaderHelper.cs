using System;
using System.Collections.Generic;
using System.Data.SQLite;
using AccountBook.Model;

namespace AccountBook.DAL.SQLiteImpl
{
    /// <summary>
    /// SqliteDataReader帮助类
    /// Author: yuwang 2011-3-9
    /// </summary>
    public static class SqliteDataReaderHelper
    {
        public static UserInfo ToUser(this SQLiteDataReader reader)
        {
            UserInfo userInfo = null;
            if (reader.Read())
            {
                userInfo = new UserInfo();
                userInfo.UserId = (int)reader.SafeRead<long>("UserId");
                userInfo.UserName = reader.SafeRead<string>("UserName");
                userInfo.FriendlyName = reader.SafeRead<string>("FriendlyName");
            }

            return userInfo;
        }

        public static List<UserInfo> ToUserList(this SQLiteDataReader reader)
        {
            List<UserInfo> userList = new List<UserInfo>();
            while (reader.Read())
            {
                UserInfo userInfo = new UserInfo();
                userInfo.UserId = (int)reader.SafeRead<long>("UserId");
                userInfo.UserName = reader.SafeRead<string>("UserName");
                userInfo.FriendlyName = reader.SafeRead<string>("FriendlyName");
                userList.Add(userInfo);
            }

            return userList;
        }

        public static List<ConsumeRecord> ToConsumeRecordList(this SQLiteDataReader reader)
        {
            List<ConsumeRecord> recordList = new List<ConsumeRecord>();

            while (reader.Read())
            {
                ConsumeRecord record = new ConsumeRecord();
                record.Id = reader.SafeRead<long>("RecordId");
                record.Money = reader.SafeRead<decimal>("Money");
                record.ConsumeTime = reader.SafeRead<DateTime>("ConsumeTime");
                record.RecordTime = reader.SafeRead<DateTime>("RecordTime");
                record.Memo = reader.SafeRead<string>("Memo");

                UserInfo consumer = new UserInfo();
                consumer.UserId = reader.SafeRead<long>("UserId");
                consumer.UserName = reader.SafeRead<string>("UserName");
                consumer.FriendlyName = reader.SafeRead<string>("FriendlyName");
                record.Consumer = consumer;

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

        public static Dictionary<string, double> ToConsumeAmountDictionary(this SQLiteDataReader reader)
        {
            Dictionary<string, double> list = new Dictionary<string, double>();
            while (reader.Read())
            {
                string time = reader.SafeRead<string>("Time");
                double money = reader.SafeRead<double>("Money");
                list.Add(time, money);
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
