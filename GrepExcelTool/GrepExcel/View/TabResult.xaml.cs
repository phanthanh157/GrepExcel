using GrepExcel.Excel;
using GrepExcel.ViewModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GrepExcel.View
{
    /// <summary>
    /// Interaction logic for TabResult.xaml
    /// </summary>
    public partial class TabResult : UserControl
    {
        private MainViewModel _mainVm = null;
        public TabResult()
        {
            InitializeComponent();
            _mainVm = MainViewModel.Instance;

            tabAction.ItemsSource = _mainVm.Tabs;
            _mainVm.Tabs.CollectionChanged += TabCollectionChanged;
            _mainVm.TabIndexActive += TabIndexActiveChanged;
        }

        private void TabIndexActiveChanged(object sender, int e)
        {
            if (e > -1)
            {
                tabAction.SelectedIndex = e;
            }
        }

        private void TabCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                tabAction.SelectedIndex = _mainVm.Tabs.Count - 1;
                _mainVm.TabActive = _mainVm.Tabs.Count - 1;
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_mainVm.Tabs.Count != 0 && tabAction.SelectedIndex != -1)
            {
                ShowDebug.Msg(F.FLMD(), "Close tab: index = {0}", tabAction.SelectedIndex);
                //update tabactive
                var resultVm = _mainVm.GetActiveSearchResultVm();
                var excelStore = ExcelStoreManager.Instance;

                var searchInfo = excelStore.GetSearchInfoById(resultVm.SearchId);

                if (searchInfo != null)
                {
                    searchInfo.IsTabActive = false;
                    SqlResult sqlResult = excelStore.UpdateSearchInfo(searchInfo);
                    if (SqlResult.UpdateSuccess == sqlResult)
                    {
                        ShowDebug.Msg(F.FLMD(), "Update tabIndex = false success");
                    }
                }

                _mainVm.Tabs.RemoveAt(tabAction.SelectedIndex);
            }
        }

        private void tabAction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _mainVm.TabActive = tabAction.SelectedIndex;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (_mainVm.Tabs.Count != 0 && tabAction.SelectedIndex != -1)
            {
                ShowDebug.Msg(F.FLMD(), "Close tab: index = {0}", tabAction.SelectedIndex);
                //update tabactive
                var resultVm = _mainVm.GetActiveSearchResultVm();
                var excelStore = ExcelStoreManager.Instance;

                var searchInfo = excelStore.GetSearchInfoById(resultVm.SearchId);

                if (searchInfo != null)
                {
                    searchInfo.IsTabActive = false;
                    SqlResult sqlResult = excelStore.UpdateSearchInfo(searchInfo);
                    if (SqlResult.UpdateSuccess == sqlResult)
                    {
                        ShowDebug.Msg(F.FLMD(), "Update tabIndex = false success");
                    }
                }

                _mainVm.Tabs.RemoveAt(tabAction.SelectedIndex);
            }
        }
    }
}
