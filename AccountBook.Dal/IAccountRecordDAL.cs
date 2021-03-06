﻿using System.Collections.Generic;
using AccountBook.Model;

namespace AccountBook.DAL.Interface
{
    public interface IAccountRecordDAL
    {
        /// <summary>
        /// 添加消费记录
        /// </summary>
        /// <param name="record">消费记录实体对象</param>
        /// <returns>消费记录ID</returns>
        long AddAccountRecord(AccountRecord record);

        /// <summary>
        /// 更新消费记录
        /// </summary>
        /// <param name="record">消费记录实体对象</param>
        /// <returns>true:更新成功 false:更新失败</returns>
        bool UpdateAccountRecord(AccountRecord record);

        /// <summary>
        /// 删除消费记录
        /// </summary>
        /// <param name="recordId">消费记录ID</param>
        /// <returns>true:删除成功 false:删除失败</returns>
        bool DeleteAccountRecord(long recordId);

        /// <summary>
        /// 获取消费记录列表
        /// </summary>
        /// <param name="option">获取消费记录列表查询参数</param>
        /// <param name="recordCount">记录总条数</param>
        /// <param name="totalMoney">总消费金额</param>
        /// <returns>消费记录列表</returns>
        List<AccountRecord> GetAccountRecordList(AccountRecordQueryOption option, out int recordCount, out decimal totalMoney);

        /// <summary>
        /// 统计消费总额信息
        /// </summary>
        /// <param name="format">时间格式</param>
        /// <param name="option">获取消费记录列表查询参数</param>
        /// <returns>消费总额信息</returns>
        Dictionary<string, double> GetAccountAmountInfo(string format, AccountRecordQueryOption option);

        /// <summary>
        /// 统计消费类别信息
        /// </summary>
        /// <param name="option">获取消费记录列表查询参数</param>
        /// <param name="typeLevel">记录类别等级</param>
        /// <returns>消费类别信息</returns>
        Dictionary<string, double> GetAccountTypeInfo(AccountRecordQueryOption option, int typeLevel);
    }
}
