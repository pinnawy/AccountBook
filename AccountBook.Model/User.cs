namespace AccountBook.Model
{
    /// <summary>
    /// 用户
    /// </summary>
    public class User
    {
        private long _userId;
        private string _userName;
        private string _fullName;

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

        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; }
        }

        public override string ToString()
        {
            return FullName;
        }
    }
}
