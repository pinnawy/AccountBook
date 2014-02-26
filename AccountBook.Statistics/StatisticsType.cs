using System.ComponentModel;

namespace AccountBook.Statistics
{
    /// <summary>
    /// 统计类别
    /// </summary>
    public enum StatisticsType
    {
        [Description("记录总金额")]
        AmountInfo,

        [Description("记录类别")]
        AccountType
    }
}
