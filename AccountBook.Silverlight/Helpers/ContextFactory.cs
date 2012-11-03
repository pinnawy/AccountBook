using AccountBook.Silverlight.Web;
using AccountBook.Silverlight.Web.Services;

namespace AccountBook.Silverlight.Helpers
{
    public class ContextFactory
    {
        private static RecordsContext _recordsContext;
        private static AccountTypeContext _accountTypeContext;
        private static UserContext _userContext ;

        public static RecordsContext RecordsContext
        {
            get { return _recordsContext ?? (_recordsContext = new RecordsContext()); }
        }

        public static AccountTypeContext ConsumeTypeContext
        {
            get { return _accountTypeContext ?? (_accountTypeContext = new AccountTypeContext()); }
        }

        public static UserContext UserContext
        {
            get { return _userContext ?? (_userContext = new UserContext()); }
        }
    }
}
