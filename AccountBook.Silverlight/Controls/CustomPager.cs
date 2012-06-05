using System.Windows;
using System.Windows.Controls;

namespace AccountBook.Silverlight.Controls
{
    /// <summary>
    /// 自定义数据分页控件
    /// </summary>
    [TemplatePart(Name = PageSizeListName, Type = typeof(ComboBox))]
    public class CustomPager : DataPager
    {
        #region DependencyProperty

        public static readonly DependencyProperty PageSizeListProperty;

        static CustomPager()
        {
            PageSizeListProperty = DependencyProperty.Register(
                "PageSizeList",
                typeof(int[]),
                typeof(CustomPager),
                new PropertyMetadata(new[] { 5, 10, 15, 20, 25 }));
        }

        #endregion

        /// <summary>
        /// PageSize选择改变事件
        /// </summary>
        public event SelectionChangedEventHandler PageSizeSelectionChanged;

        private const string PageSizeListName = "cmbPageSizeList";

        private ComboBox _cmbPageSizeList;
        /// <summary>
        /// 页码选择CombBox
        /// </summary>
        internal ComboBox CmbPageSizeList
        {
            get { return _cmbPageSizeList; }
            set
            {
                if (_cmbPageSizeList != null)
                {
                    _cmbPageSizeList.SelectionChanged -= PageSizeListSelectionChanged;
                }
                _cmbPageSizeList = value;

                if (_cmbPageSizeList != null)
                {
                    _cmbPageSizeList.SelectionChanged += PageSizeListSelectionChanged;
                }
            }
        }

        public CustomPager()
        {
            this.DefaultStyleKey = typeof(CustomPager);
            this.DataContext = this;
        }

        public override void OnApplyTemplate()
        {
            CmbPageSizeList = GetTemplateChild(PageSizeListName) as ComboBox;
            base.OnApplyTemplate();
        }

        /// <summary>
        /// PageSize列表
        /// </summary>
        public int[] PageSizeList
        {
            get { return (int[])GetValue(PageSizeListProperty); }
            set { SetValue(PageSizeListProperty, value); }
        }

        /// <summary>
        /// PageSize选择改变事件
        /// </summary>
        private void PageSizeListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PageSizeSelectionChanged != null)
            {
                PageSizeSelectionChanged(this, e);
            }
        }
    }
}