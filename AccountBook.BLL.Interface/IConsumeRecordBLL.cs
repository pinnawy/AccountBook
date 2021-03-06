﻿using System.Collections.Generic;
using AccountBook.Model;

namespace AccountBook.BLL.Interface
{
    public interface IConsumeRecordBLL
    {
        /// <summary>
        /// 添加消费记录
        /// </summary>
        /// <param name="record">消费记录实体对象</param>
        /// <returns>消费记录ID</returns>
        long AddConsumeRecord(AccountRecord record);

        /// <summary>
        /// 更新消费记录
        /// </summary>
        /// <param name="record">消费记录实体对象</param>
        /// <returns>true:更新成功 false:更新失败</returns>
        bool UpdateConsumeRecord(AccountRecord record);

        /// <summary>
        /// 删除消费记录
        /// </summary>
        /// <param name="recordId">消费记录ID</param>
        /// <returns>true:删除成功 false:删除失败</returns>
        bool DeleteConsumeRecord(long recordId);

        /// <summary>
        /// 获取消费记录列表
        /// </summary>
        /// <param name="option">获取消费记录列表查询参数</param>
        /// <returns>消费记录列表</returns>
        AccountRecordsResult GetConsumeRecordList(AccountRecordQueryOption option);

        /// <summary>
        /// 按月统计消费金额
        /// </summary>
        /// <param name="option">获取消费记录列表查询参数</param>
        /// <returns>每月消费总额信息</returns>
        Dictionary<string, double> GetConsumeAmountByMonth(AccountRecordQueryOption option);

        /// <summary>
        /// 按年统计消费金额
        /// </summary>
        /// <param name="option">获取消费记录列表查询参数</param>
        /// <returns>每年消费总额信息</returns>
        Dictionary<string, double> GetConsumeAmountByYear(AccountRecordQueryOption option);

        /// <summary>
        /// 统计消费类别信息
        /// </summary>
        /// <param name="option">获取消费记录列表查询参数</param>
        /// <param name="typeLevel">记录类别等级</param>
        /// <returns>消费类别信息</returns>
        Dictionary<string, double> GetAccountTypeInfo(AccountRecordQueryOption option, int typeLevel);
    }
}
