using System;
using System.ComponentModel.DataAnnotations;

namespace AccountBook.Model
{
    /// <summary>
    /// 消费记录实体类
    /// </summary>
    public class AccountRecord
    {
        private long _id;
        private AccountType _type;
        private decimal _money;
        private DateTime _consumeTime;
        private DateTime _recordTime;
        private string _memo;
        private UserInfo _consumer;
        private bool _isAccessorial;

        /// <summary>
        /// 消费记录ID
        /// </summary>
        [Display(AutoGenerateField = false)]
        public long Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// 消费类别
        /// </summary>
        [Display(Order = 0)]
        [Required(ErrorMessage = "消费类别不能为空")]
        public AccountType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        /// <summary>
        /// 消费金额
        /// </summary>
        [Display(Order = 1, Description = "消费金额不能小于0")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "消费金额不能为空")]
        public decimal Money
        {
            get { return _money; }
            set { _money = value; }
        }

        /// <summary>
        /// 消费时间
        /// </summary>
        [Display(Order = 2, Description = "消费时间不能大于当前时间")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "消费时间不能为空")]
        [DataType(DataType.DateTime)]
        public DateTime ConsumeTime
        {
            get { return _consumeTime; }
            set { _consumeTime = value; }
        }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Order = 4, Description = "填写消费缘由")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "消费备注不能为空")]
        public string Memo
        {
            get { return _memo; }
            set { _memo = value; }
        }

        /// <summary>
        /// 消费支出人
        /// </summary>
        [Display(Order = 3, Description = "选择消费支出人")]
        public UserInfo Consumer
        {
            get { return _consumer; }
            set { _consumer = value; }
        }

        /// <summary>
        /// 记录时间
        /// </summary>
        [Display(AutoGenerateField = false)]
        public DateTime RecordTime
        {
            get { return _recordTime; }
            set { _recordTime = value; }
        }

        /// <summary>
        /// 消费支出人
        /// </summary>
        [Display(Order = 5, Description = "附属记录", Name = "Accessorial")]
        public bool IsAccessorial
        {
            get { return _isAccessorial; }
            set { _isAccessorial = value; }
        }
    }
}
