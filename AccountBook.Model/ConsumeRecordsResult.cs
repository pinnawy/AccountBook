using System.Collections.Generic;

namespace AccountBook.Model
{
    public class ConsumeRecordsResult
    {
        /// <summary>
        /// 消费记录总条数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 消费总金额(单位：元)
        /// </summary>
        public decimal TotalMoney { get; set; }

        /// <summary>
        /// 平均消费金额(单位: 元/天)
        /// </summary>
        public decimal AverageMoney { get; set; }

        /// <summary>
        /// 消费记录
        /// </summary>
        public List<ConsumeRecord> Records { get; set; }
    }
}
