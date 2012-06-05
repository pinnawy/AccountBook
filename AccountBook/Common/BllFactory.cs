using AccountBook.Bll.Interface;
using AccountBook.Library;

namespace AccountBookWin.Common
{
    public class BllFactory
    {
        private static IUserBll _userBll;
        private static IConsumeRecordBll _consumeRecordBll;
        private static IConsumeTypeBll _consumeTypeBll;

        public static IUserBll GetUserBll()
        {
            return _userBll ?? (_userBll = UnityContext.LoadBllModel<IUserBll>());
        }

        public static IConsumeRecordBll GetConsumeRecordBll()
        {
            return _consumeRecordBll ?? (_consumeRecordBll = UnityContext.LoadBllModel<IConsumeRecordBll>());
        }

        public static IConsumeTypeBll GetConsumeTypeBll()
        {
            return _consumeTypeBll ?? (_consumeTypeBll = UnityContext.LoadBllModel<IConsumeTypeBll>());
        }
    }
}
