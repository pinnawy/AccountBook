using System.Collections.Generic;
using AccountBook.DAL.Interface;
using AccountBook.Model;

namespace AccountBook.DAL.SQLiteImpl
{
    public class UserDAL : IUserDAL
    {
        public UserInfo GetUser(string username)
        {
            var sql = string.Format("SELECT * FROM [User] WHERE [UserName]='{0}'", username);
            var obj = SqliteHelper.ExecuteReader(sql);
            var userInfo = obj.ToUser();
            return userInfo;
        }

        public List<UserInfo> GetUserList()
        {
            const string cmdText = "SELECT * FROM [User]";
            var obj = SqliteHelper.ExecuteReader(cmdText);
            return obj.ToUserList();
        }

        public string GetPassword(string username)
        {
            string cmdText = string.Format("SELECT [Password] FROM [User] WHERE [UserName]='{0}'", username);
            var obj = SqliteHelper.ExecuteReader(cmdText);
            return obj.SafeRead<string>("Password");
        }

        //        public long AddNewUser(Consumer userInfo)
        //        {
        //            //判断当前用户是否在数据库中已经存在
        //            string cmdText = string.Format("SELECT [userId] FROM [SimUser] WHERE [userName]='{0}'", userInfo.UserName);
        //            object newUserId = SqliteHelper.ExecuteScalar(cmdText);

        //            if (newUserId == null)
        //            {
        //                cmdText = @"INSERT INTO [SimUser] (
        //                                    [userName],[fullName],[className],[password],[roleType],[status],[userImage],[gender],[EMail],[regTime],[reserved1],[reserved2],[reserved3]
        //                                )
        //                                VALUES (
        //                                    @userName, @fullName, @className, @password, @roleType, @status, @userImage, @gender, @EMail, @regTime, @reserved1, @reserved2, @reserved3
        //                                )";
        //                var parameters = new[]
        //                {
        //                    new SQLiteParameter("@userName", userInfo.UserName),
        //                    new SQLiteParameter("@fullName", userInfo.FriendlyName),
        //                    new SQLiteParameter("@className", userInfo.ClassName),
        //                    new SQLiteParameter("@password", userInfo.Password),
        //                    new SQLiteParameter("@roleType", userInfo.RoleType),
        //                    new SQLiteParameter("@status", userInfo.Status),
        //                    new SQLiteParameter("@userImage", userInfo.UserImage),
        //                    new SQLiteParameter("@gender", userInfo.Gender),
        //                    new SQLiteParameter("@EMail", userInfo.Email),
        //                    new SQLiteParameter("@regTime", userInfo.RegisterTime),
        //                    new SQLiteParameter("@reserved1", userInfo.Reserved1),
        //                    new SQLiteParameter("@reserved2", userInfo.Reserved2),
        //                    new SQLiteParameter("@reserved3", userInfo.Reserved3)
        //                };
        //                SqliteHelper.ExecuteNonQuery(cmdText, parameters);

        //                //查询刚插入数据的历史记录ID
        //                cmdText = @"SELECT last_insert_rowid()";
        //                newUserId = SqliteHelper.ExecuteScalar(cmdText);
        //            }

        //            return (long)newUserId;
        //        }

        public int DeleteUser(int userId)
        {
            var cmdText = string.Format("DELETE FROM [User] WHERE [userID]={0}", userId);
            return SqliteHelper.ExecuteNonQuery(cmdText);
        }
    }
}
