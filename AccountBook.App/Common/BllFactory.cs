using AccountBook.Bll.Interface;

namespace AccountBook.App.Common
{
    public class BllFactory
    {
        private static IUserBll _userBll;

        //public static IUserBll GetUserBll()
        //{
        //    return _userBll ?? (_userBll = UnityContext.LoadBllModel<IUserBll>());
        //}
    }
}
