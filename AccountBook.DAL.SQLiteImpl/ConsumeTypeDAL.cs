using System.Collections.Generic;
using AccountBook.DAL.Interface;
using AccountBook.Model;

namespace AccountBook.DAL.SQLiteImpl
{
    public class ConsumeTypeDAL : IConsumeTypeDAL
    {
        public List<AccountType> GetAccountTypes(int parentTypeId, AccountCategory category)
        {
            string cmdText = @"SELECT *,(SELECT [TypeName] FROM [AccountType] WHERE [TypeId] = T.[ParentTypeId]) AS ParentTypeName 
                                FROM [AccountType] T WHERE 1=1";
            if (parentTypeId != 0)
            {
                cmdText = string.Format("{0} AND [ParentTypeId]={1}", cmdText, parentTypeId);
            }

            if(category != AccountCategory.Undefined)
            {
                cmdText = string.Format("{0} AND [Category]={1}", cmdText, (int)category);
            }

            var reader = SqliteHelper.ExecuteReader(cmdText);
            return reader.ToConsumeTypeList();
        }

        public List<AccountType> GetAccountSubTypes(AccountCategory category)
        {
            string cmdText = string.Format(@"SELECT *,(SELECT [TypeName] FROM [AccountType] WHERE [TypeId] = T.[ParentTypeId]) AS ParentTypeName 
                                    FROM [AccountType] T WHERE [ParentTypeId] <> 0");

            if (category != AccountCategory.Undefined)
            {
                cmdText = string.Format("{0} AND [Category]={1}", cmdText, (int)category);
            }

            var reader = SqliteHelper.ExecuteReader(cmdText);
            return reader.ToConsumeTypeList();
        }
    }
}
