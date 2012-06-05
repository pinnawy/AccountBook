using System;

namespace AccountBook.Silverlight.Events
{
    public class LoginStatusChangedEventArgs : EventArgs
    {
        public LoginStatusChangedEventArgs(LoginStatus loginStatus)
        {
            LoginStatus = loginStatus;
        }

        public LoginStatus LoginStatus
        {
            get;
            private set;
        }
    }
}
