using GrepExcel.Excel;
using GrepExcel.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GrepExcel.View
{
    /// <summary>
    /// Interaction logic for ListSeachUC.xaml
    /// </summary>
    public partial class ListSeachUC : UserControl
    {
        private ListSearchVm _lstSearchVm = null;
        public ListSeachUC()
        {
            InitializeComponent();
            _lstSearchVm = ListSearchVm.Instance;
            this.DataContext = _lstSearchVm;
            lstSearch.ItemsSource = _lstSearchVm.SearchInfos;
        }

        private void lstSearch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var searchResult = (ShowInfo) lstSearch.SelectedItem;

            //if (searchResult == null)
            //{
            //    ShowDebug.MsgErr(F.FLMD(), "Select search result is null");
            //    return;
            //}

            _lstSearchVm.ShowTabSearchResult(searchResult);
        }

        private void btnDelSerachResult_Click(object sender, RoutedEventArgs e)
        {
            var showInfo = (ShowInfo) lstSearch.SelectedItem ;

            ShowDebug.MsgErr(F.FLMD(), "Delete id = {0}", showInfo.Info.Id);

            _lstSearchVm.DelSearchResult(showInfo);

            lstSearch.UpdateLayout();
        }

        private void ItemOnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((ListBoxItem)sender).IsSelected = true;
        }
    }
}
