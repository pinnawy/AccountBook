using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace AccountBook.Model
{
    public partial class UserInfo
    {
        public UserInfo Clone()
        {
            return new UserInfo
            {
                FriendlyName = FriendlyName,
                UserId = UserId,
                UserName = UserName
            };
        }
    }
}
