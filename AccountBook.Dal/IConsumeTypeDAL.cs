using System.Collections.Generic;
using AccountBook.Model;

namespace AccountBook.DAL.Interface
{
    public interface IConsumeTypeDAL
    {
        /// <summary>
        /// 获取账目类别列表
        /// </summary>
        /// <param name="parentTypeId">父类别ID</param>
        /// <param name="category">分类</param>
        /// <returns>消费类别List集合</returns>
        List<AccountType> GetAccountTypes(int parentTypeId, AccountCategory category);

        /// <summary>
        /// 获取账目类别二级分类
        /// </summary>
        /// <returns>消费类别二级分类List集合</returns>
        List<AccountType> GetAccountSubTypes(AccountCategory category);
    }
}
