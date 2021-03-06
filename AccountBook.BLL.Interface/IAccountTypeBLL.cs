﻿using System.Collections.Generic;
using AccountBook.Model;

namespace AccountBook.BLL.Interface
{
    public interface IAccountTypeBLL
    {
        /// <summary>
        /// 获取消费类别列表
        /// </summary>
        /// <param name="parentTypeId">父类别ID(父类别ID为0时，查询所有消费类别)</param>
        /// <param name="categoty"></param>
        /// <returns>消费类别List集合</returns>
        List<AccountType> GetAccountTypes(int parentTypeId, AccountCategory categoty);

        /// <summary>
        /// 获取消费类别二级分类
        /// </summary>
        /// <returns>消费类别二级分类List集合</returns>
        List<AccountType> GetAccountSubTypes(AccountCategory categoty);
    }
}
