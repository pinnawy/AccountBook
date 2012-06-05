using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AccountBookWin.Control
{
    /// <summary>
    /// 分页控件切换页码事件委托
    /// </summary>
    public delegate void PagerPageIndexChangedEventHandler(object sender, PagerPageIndexChangedEventArgs e);

    /// <summary>
    /// 分页控件切换页码事件参数
    /// </summary>
    public class PagerPageIndexChangedEventArgs : EventArgs
    {
        public PagerPageIndexChangedEventArgs(int newPageIndex, int oldPageIndex)
        {
            NewPageIndex = newPageIndex;
            OldPageIndex = oldPageIndex;
        }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int OldPageIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// 新页码
        /// </summary>
        public int NewPageIndex
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// Interaction logic for Pager.xaml
    /// </summary>
    public partial class Pager : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty TotalCountProperty;
        public static readonly DependencyProperty PageIndexProperty;
        public static readonly DependencyProperty PageSizeProperty;
        public static readonly DependencyProperty PageSizeListProperty;
        public static readonly DependencyProperty PageIndexShowCountProperty;

        static Pager()
        {
            TotalCountProperty = DependencyProperty.Register(
                "TotalCount",
                typeof(int),
                typeof(Pager),
                new PropertyMetadata(0, TotalCountChanged));

            PageIndexProperty = DependencyProperty.Register(
               "PageIndex",
               typeof(int),
               typeof(Pager),
               new PropertyMetadata(0, PageIndexChanged));

            PageSizeProperty = DependencyProperty.Register(
               "PageSize",
               typeof(int),
               typeof(Pager),
               new PropertyMetadata(10, PageSizeChanged));

            PageIndexShowCountProperty = DependencyProperty.Register(
               "PageIndexShowCount",
               typeof(int),
               typeof(Pager),
               new PropertyMetadata(5, PageIndexShowCountChanged));

            PageSizeListProperty = DependencyProperty.Register(
               "PageSizeList",
               typeof(int[]),
               typeof(Pager),
               new PropertyMetadata(new[] { 10, 20, 30 }, PageSizeListChanged));
        }

        private static void TotalCountChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Pager pager = (Pager)sender;
            int totalCount = (int)e.NewValue;
            pager.LastPageIndex = GetLastPageIndex(totalCount, pager.PageSize);
            pager.PageIndex = GetFixedPageIndex(pager.PageIndex, totalCount, pager.PageSize, pager.PageSize);
            pager.SetPageIndexItem(pager.PageIndex, pager.PageIndexShowCount, pager.LastPageIndex);
            CommandManager.InvalidateRequerySuggested();
        }

        private static void PageIndexChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Pager pager = (Pager)sender;
            int newPageIndex = (int)e.NewValue;
            int oldPageIndex = (int)e.OldValue;
            if (newPageIndex < 1 || newPageIndex > pager.LastPageIndex)
            {
                if (oldPageIndex > 0 && oldPageIndex < pager.LastPageIndex + 1)
                {
                    pager.PageIndex = (int)e.OldValue;
                }
            }
            else
            {
                if (pager.PagerPageIndexChanged != null)
                {
                    if (oldPageIndex > -1 && oldPageIndex < pager.LastPageIndex + 1)
                    {
                        pager.SetPageIndexItem(newPageIndex, pager.PageIndexShowCount, pager.LastPageIndex);
                        pager.PagerPageIndexChanged(pager, new PagerPageIndexChangedEventArgs(newPageIndex, oldPageIndex));
                    }
                }
            }

            CommandManager.InvalidateRequerySuggested();
        }

        private static void PageSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Pager pager = (Pager)sender;
            int pageSize = (int)e.NewValue;
            pager.LastPageIndex = GetLastPageIndex(pager.TotalCount, pageSize);
            pager.PageIndex = GetFixedPageIndex(pager.PageIndex, pager.TotalCount, (int)e.OldValue, pageSize);

            CommandManager.InvalidateRequerySuggested();
        }

        private static void PageIndexShowCountChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private static void PageSizeListChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }

        private static int GetLastPageIndex(int totalCount, int pageSize)
        {
            if (totalCount == 0)
                return 0;

            return totalCount % pageSize == 0 ? totalCount / pageSize : totalCount / pageSize + 1;
        }

        private static int GetFixedPageIndex(int currPageIndex, int newTotalCount, int currPageSize, int newPageSize)
        {
            if (newTotalCount <= newPageSize)
                return 1;


            int currItemIndex = currPageIndex * currPageSize + 1;
            int newPageIndex = currItemIndex % newPageSize == 0 ? currItemIndex / newPageSize : currItemIndex / newPageSize + 1;

            return newPageIndex;
        }

        private void SetPageIndexItem(int pageIndex, int pageIndexShowCount, int lastPageIndex)
        {
            int beginIndex;
            int endIndex;

            if (lastPageIndex > pageIndexShowCount)
            {
                beginIndex = pageIndex - pageIndexShowCount / 2;
                if (beginIndex < 1)
                    beginIndex = 1;

                endIndex = beginIndex + pageIndexShowCount - 1;
                if (endIndex > lastPageIndex)
                {
                    endIndex = lastPageIndex;
                    beginIndex = lastPageIndex - pageIndexShowCount + 1;
                }
            }
            else
            {
                beginIndex = 1;
                endIndex = lastPageIndex;
            }


            int currShowCount = PageIndexPanel.Children.Count;
            int needShowCount = endIndex - beginIndex + 1;

            while (currShowCount > needShowCount)
            {
                PageIndexPanel.Children.RemoveAt(0);
                currShowCount--;
            }

            while (needShowCount > currShowCount)
            {
                Button pageIndexItem = new Button();
                pageIndexItem.Template = (ControlTemplate)FindResource("PageIndexItemTemplate");
                PageIndexPanel.Children.Add(pageIndexItem);
                currShowCount++;
            }

            for (int i = beginIndex; i <= endIndex; i++)
            {
                Button pageIndexItem = (Button)PageIndexPanel.Children[i - beginIndex];
                pageIndexItem.Content = i;
                pageIndexItem.Command = SwitchPage;
                pageIndexItem.CommandParameter = i;
            }
        }

        public static RoutedCommand SwitchPage = new RoutedCommand();
        public static RoutedCommand Next = new RoutedCommand();
        public static RoutedCommand Previous = new RoutedCommand();
        public static RoutedCommand SwitchToFirstPage = new RoutedCommand();
        public static RoutedCommand SwitchToLastPage = new RoutedCommand();

        public event PagerPageIndexChangedEventHandler PagerPageIndexChanged;

        private int lastPageIndex;

        /// <summary>
        /// 记录总条数
        /// </summary>
        public int TotalCount
        {
            get { return (int)GetValue(TotalCountProperty); }
            set { SetValue(TotalCountProperty, value); }
        }

        /// <summary>
        /// 分页控件页码
        /// <remarks>从1开始</remarks>
        /// </summary>
        public int PageIndex
        {
            get { return (int)GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }

        /// <summary>
        /// 分页控件页面Size
        /// </summary>
        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }

        /// <summary>
        /// 页码显示个数
        /// </summary>
        public int PageIndexShowCount
        {
            get { return (int)GetValue(PageIndexShowCountProperty); }
            set { SetValue(PageIndexShowCountProperty, value); }
        }

        /// <summary>
        /// 分页控件页面Size集合
        /// </summary>
        public int[] PageSizeList
        {
            get { return (int[])GetValue(PageSizeListProperty); }
            set { SetValue(PageSizeListProperty, value); }
        }

        /// <summary>
        /// 分页控件最后一页索引
        /// </summary>
        public int LastPageIndex
        {
            get
            {
                return this.lastPageIndex;
            }
            private set
            {
                if (this.lastPageIndex != value)
                {
                    this.lastPageIndex = value;
                    OnPropertyChanged("LastPageIndex");
                }
            }
        }

        public Pager()
        {
            InitializeComponent();
            BindingCommands();
        }

        private void BindingCommands()
        {
            this.CommandBindings.Add(new CommandBinding(SwitchPage, this.SwitchPageExecute, this.SwitchPageEnable));
            this.CommandBindings.Add(new CommandBinding(Next, this.NextExecute, this.NextEnable));
            this.CommandBindings.Add(new CommandBinding(Previous, this.PreviousExecute, this.PreviousEnable));

            this.CommandBindings.Add(new CommandBinding(SwitchToFirstPage, this.SwitchToFirstPageExecute, this.SwitchToFirstPageEnable));
            this.CommandBindings.Add(new CommandBinding(SwitchToLastPage, this.SwitchToLastPageExecute, this.SwitchToLastPageEnable));
        }

        private void SwitchPageExecute(object sender, ExecutedRoutedEventArgs e)
        {
            PageIndex = (int)e.Parameter;
        }

        private void SwitchPageEnable(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }


        private void PreviousExecute(object sender, ExecutedRoutedEventArgs e)
        {
            PageIndex--;
        }

        private void PreviousEnable(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.PageIndex > 1;
        }

        private void NextExecute(object sender, ExecutedRoutedEventArgs e)
        {
            PageIndex++;
        }

        private void NextEnable(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PageIndex < LastPageIndex;
        }

        private void SwitchToFirstPageExecute(object sender, ExecutedRoutedEventArgs e)
        {
            PageIndex = 1;
        }

        private void SwitchToFirstPageEnable(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PageIndex != 1;
        }

        private void SwitchToLastPageExecute(object sender, ExecutedRoutedEventArgs e)
        {
            PageIndex = LastPageIndex;
        }

        private void SwitchToLastPageEnable(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = PageIndex != LastPageIndex;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
