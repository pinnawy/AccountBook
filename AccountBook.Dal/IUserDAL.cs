using System.Collections.Generic;
using AccountBook.Model;

namespace AccountBook.DAL.Interface
{
    public interface IUserDAL
    {
        /// <summary>
        ///  获取用户
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>用户实体对象</returns>
        UserInfo GetUser(string username);

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <returns>用户列表</returns>
        List<UserInfo> GetUserList();

        /// <summary>
        /// 获取用户密码
        /// </summary>
        /// <param name="username">用户名</param>
        /// <returns>用户密码</returns>
        string GetPassword(string username);
    }
}
