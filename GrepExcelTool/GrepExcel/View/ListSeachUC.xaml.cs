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
        private ListSearchVm listSearchVm_ = null;
        public ListSeachUC()
        {
            InitializeComponent();
            listSearchVm_ = ListSearchVm.Instance;

            Base.Check(listSearchVm_);

            this.DataContext = listSearchVm_;
            lstSearch.ItemsSource = listSearchVm_.SearchInfos;
        }

        private void lstSearch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var searchResult = (ShowInfo) lstSearch.SelectedItem;

            listSearchVm_.ShowTab(searchResult);
        }

        private void btnDelSerachResult_Click(object sender, RoutedEventArgs e)
        {
            var showInfo = (ShowInfo) lstSearch.SelectedItem ;
            listSearchVm_.DelSearchResult(showInfo);

            lstSearch.UpdateLayout();
        }

        private void ItemOnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((ListBoxItem)sender).IsSelected = true;
        }
    }
}
