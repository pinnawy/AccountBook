using System;
using System.Web.Security;
using AccountBook.BLL.Interface;
using AccountBook.Library;

namespace AccountBook.Silverlight.Web
{
    public class AccountBookMembershipProvider : MembershipProvider
    {
        private readonly IUserBLL _userBLL = UnityContext.LoadBLLModel<IUserBLL>();

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            status = MembershipCreateStatus.Success;
            return new MembershipUser("AccountBookMembershipProvider",
                                        username, null, email, passwordQuestion, ""
                                         , true, true, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now, DateTime.Now);
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            return true;
        }

        public override string GetPassword(string username, string answer)
        {
            return "";
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            return true;
        }

        public override string ResetPassword(string username, string answer)
        {
            return "";
        }

        public override void UpdateUser(MembershipUser user)
        {

        }

        public override bool ValidateUser(string username, string password)
        {
            return password.MD5Encrypt() == _userBLL.GetPassword(username);
        }

        public override bool UnlockUser(string userName)
        {
            return true;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            return null;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            return null;
        }

        public override string GetUserNameByEmail(string email)
        {
            return null;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            return true;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = 0;
            return null;
        }

        public override int GetNumberOfUsersOnline()
        {
            return 0;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = 0;
            return null;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = 0;
            return null;
        }

        public override bool EnablePasswordRetrieval
        {
            get { return true; }
        }

        public override bool EnablePasswordReset
        {
            get { return true; }
        }

        private bool _requiresQuestionAndAnswer;
        public override bool RequiresQuestionAndAnswer
        {
            get
            {
                return _requiresQuestionAndAnswer;
            }
        }

        private string _applicationName;
        public override string ApplicationName
        {
            get
            {
                return _applicationName;
            }
            set
            {
                _applicationName = value;
            }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return 5; }
        }

        public override int PasswordAttemptWindow
        {
            get { return 5; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Encrypted; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 6; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 2; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return "abc123"; }
        }
    }
}