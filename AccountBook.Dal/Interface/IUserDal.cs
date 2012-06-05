using System.Collections.Generic;
using AccountBook.Model;

namespace AccountBook.Dal.Interface
{
    public interface IUserDal
    {
        /// <summary>
        ///  获取用户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>用户实体对象</returns>
        User GetUser(string username);

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns>用户列表</returns>
        List<User> GetUserList();
    }
}
