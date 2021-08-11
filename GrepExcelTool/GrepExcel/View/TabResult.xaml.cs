using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GrepExcel.ViewModel;
using GrepExcel.Excel;

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
                var resultVm = _mainVm.Tabs[tabAction.SelectedIndex];
                var excelStore = ExcelStoreManager.Instance;

                var searchInfo = excelStore.GetSearchInfoById(resultVm.SearchId);

                if(searchInfo != null)
                {
                    searchInfo.IsTabActive = false;
                    SqlResult sqlResult = excelStore.UpdateSearchInfo(searchInfo);
                    if(SqlResult.UpdateSuccess == sqlResult)
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
    }
}
