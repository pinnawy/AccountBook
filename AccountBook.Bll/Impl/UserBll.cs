using System;
using System.Collections.Generic;
using AccountBook.Bll.Interface;
using AccountBook.Dal.Interface;
using AccountBook.Library;
using AccountBook.Model;

namespace AccountBook.Bll.Impl
{
    public class UserBll : IUserBll
    {
        private readonly IUserDal _userDal = UnityContext.LoadDalModel<IUserDal>();

        public User GetUser(string username)
        {
            return _userDal.GetUser(username);
        }

        public List<User> GetUserList()
        {
            return _userDal.GetUserList();
        }
    }
}
