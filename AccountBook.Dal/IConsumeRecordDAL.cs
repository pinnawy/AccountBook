using System.Collections.Generic;
using AccountBook.Model;

namespace AccountBook.DAL.Interface
{
    public interface IConsumeRecordDAL
    {
        /// <summary>
        /// 添加消费记录
        /// </summary>
        /// <param name="record">消费记录实体对象</param>
        /// <returns>消费记录ID</returns>
        long AddConsumeRecord(ConsumeRecord record);

        /// <summary>
        /// 更新消费记录
        /// </summary>
        /// <param name="record">消费记录实体对象</param>
        /// <returns>true:更新成功 false:更新失败</returns>
        bool UpdateConsumeRecord(ConsumeRecord record);

        /// <summary>
        /// 获取消费记录列表
        /// </summary>
        /// <param name="option">获取消费记录列表查询参数</param>
        /// <param name="recordCount">记录总条数</param>
        /// <param name="totalMoney">总消费金额</param>
        /// <returns>消费记录列表</returns>
        List<ConsumeRecord> GetConsumeRecordList(ConsumeRecordQueryOption option, out int recordCount, out decimal totalMoney);

        /// <summary>
        /// 按月统计消费金额
        /// </summary>
        /// <param name="format">时间格式</param>
        /// <param name="option">获取消费记录列表查询参数</param>
        /// <returns>每月消费总额信息</returns>
        List<KeyValuePair<string, double>> GetConsumeAmountList(string format, ConsumeRecordQueryOption option);
    }
}
