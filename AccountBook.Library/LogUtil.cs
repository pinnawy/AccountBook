using log4net;

namespace AccountBook.Library
{
    public class LogUtil
    {
        private static readonly ILog _Log = LogManager.GetLogger("AccountBookLog");
        static LogUtil()
        {
            //log4net.Config.DOMConfigurator.Configure();
            log4net.Config.XmlConfigurator.Configure();
        }

        public static ILog Log
        {
            get
            {
                return _Log;
            }
        }
    }
}