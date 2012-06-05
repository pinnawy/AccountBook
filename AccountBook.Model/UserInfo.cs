namespace AccountBook.Model
{
    /// <summary>
    /// 用户
    /// </summary>
    public class UserInfo
    {
        private long _userId;
        private string _userName;
        private string _friendlyName;

        public long UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public string UserName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string FriendlyName
        {
            get { return _friendlyName; }
            set { _friendlyName = value; }
        }

        public override string ToString()
        {
            return FriendlyName;
        }
    }
}
