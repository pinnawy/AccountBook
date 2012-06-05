using AccountBook.Silverlight.Web;
using AccountBook.Silverlight.Web.Services;

namespace AccountBook.Silverlight.Helpers
{
    public class ContextFactory
    {
        private static RecordsContext _recordsContext;
        private static ConsumeTypeContext _consumeTypeContext;
        private static UserContext _userContext ;

        public static RecordsContext RecordsContext
        {
            get { return _recordsContext ?? (_recordsContext = new RecordsContext()); }
        }

        public static ConsumeTypeContext ConsumeTypeContext
        {
            get { return _consumeTypeContext ?? (_consumeTypeContext = new ConsumeTypeContext()); }
        }

        public static UserContext UserContext
        {
            get { return _userContext ?? (_userContext = new UserContext()); }
        }
    }
}
