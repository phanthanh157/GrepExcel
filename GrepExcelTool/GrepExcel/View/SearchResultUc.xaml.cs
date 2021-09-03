using GrepExcel.Excel;
using GrepExcel.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GrepExcel.View
{
    /// <summary>
    /// Interaction logic for SearchResultUc.xaml
    /// </summary>
    public partial class SearchResultUc : UserControl
    {
        private MainViewModel _mainVm = null;
        public SearchResultUc()
        {
            InitializeComponent();
            _mainVm = MainViewModel.Instance;
            _mainVm.TabSelectionChange += TabChange;

        }

        private void TabChange(object sender, object e)
        {
            //Update again selection index
            cobOptionFilter.SelectedIndex = 0;

        }

        private void lvSearchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void lvSearchResultsColumnHeader_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GotoDocument_Click(object sender, RoutedEventArgs e)
        {

        }

        private void txtFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (string.IsNullOrEmpty(txtFilter.Text)) return;
                var infoSearch = new { Search = txtFilter.Text, OptionFilter = cobOptionFilter.SelectedValue };

                var mainVm = MainViewModel.Instance;
                var searchResultVm = mainVm.GetActiveSearchResultVm();
                if (searchResultVm != null)
                {
                    searchResultVm.CommandSearchResult.Execute(infoSearch);
                }
            }
        }

        private void btnDestroyFilter_Click(object sender, RoutedEventArgs e)
        {
            var mainVm = MainViewModel.Instance;
            var searchResultVm = mainVm.GetActiveSearchResultVm();
            if (searchResultVm != null)
            {
                txtFilter.Text = string.Empty;
                searchResultVm.CommandRefresh.Execute(this);
            }
        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            var mainVm = MainViewModel.Instance;
            var searchResultVm = mainVm.GetActiveSearchResultVm();
            if (txtFilter.Text == string.Empty)
            {
                if (searchResultVm != null)
                {
                    txtFilter.Text = string.Empty;
                    searchResultVm.CommandRefresh.Execute(this);
                }
            }
            else
            {
                var infoSearch = new { Search = txtFilter.Text, OptionFilter = cobOptionFilter.SelectedValue };
                if (searchResultVm != null)
                {
                    searchResultVm.CommandSearchResult.Execute(infoSearch);
                }
            }
        }

        private void CopyPath_Click(object sender, RoutedEventArgs e)
        {
            var searchResult = lvSearchResults.SelectedItem as ResultInfo;

            if(searchResult != null)
            {
                Clipboard.SetText(searchResult.FileName);
            }

        }
    }
}
