using System.Collections.Generic;
using AccountBook.BLL.Interface;
using AccountBook.DAL.Interface;
using AccountBook.Library;
using AccountBook.Model;

namespace AccountBook.BLL
{
    public class UserBLL : IUserBLL
    {
        private readonly IUserDAL _userDal = UnityContext.LoadDALModel<IUserDAL>();

        public UserInfo GetUser(string username)
        {
            return _userDal.GetUser(username);
        }

        public List<UserInfo> GetUserList()
        {
            return _userDal.GetUserList();
        }

        public string GetPassword(string username)
        {
            return _userDal.GetPassword(username);
        }
    }
}
