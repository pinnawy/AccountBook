namespace AccountBook.Model
{
    public class QueryOptionBase
    {
        private int _pageIndex;
        private int _pageSize;
        private SortDir _sortDir;
        private string _sortName;

        public int PageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value; }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        public SortDir SortDir
        {
            get { return _sortDir; }
            set { _sortDir = value; }
        }

        public string SortName
        {
            get { return _sortName; }
            set { _sortName = value; }
        }
    }

    public enum SortDir
    {
        ASC,

        DESC
    }
}
