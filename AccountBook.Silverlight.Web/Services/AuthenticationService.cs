using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server.ApplicationServices;

namespace AccountBook.Silverlight.Web
{
    // TODO: Switch to a secure endpoint when deploying the application.
    //       The user's name and password should only be passed using https.
    //       To do this, set the RequiresSecureEndpoint property on EnableClientAccessAttribute to true.
    //   
    //       [EnableClientAccess(RequiresSecureEndpoint = true)]
    //
    //       More information on using https with a Domain Service can be found on MSDN.

    /// <summary>
    /// Domain Service responsible for authenticating users when they log on to the application.
    ///
    /// Most of the functionality is already provided by the AuthenticationBase class.
    /// </summary>
    [EnableClientAccess]
    public class AuthenticationService : AuthenticationBase<User>
    {
        protected override bool AuthorizeChangeSet()
        {
            return base.AuthorizeChangeSet();
        }

        protected override void UpdateUserCore(User user)
        {
            base.UpdateUserCore(user);
        }

        protected override void ClearAuthenticationToken()
        {
            base.ClearAuthenticationToken();
        }

        protected override bool ValidateChangeSet()
        {
            return base.ValidateChangeSet();
        }

    }
}
