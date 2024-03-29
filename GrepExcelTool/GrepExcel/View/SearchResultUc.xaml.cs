﻿using GrepExcel.Excel;
using GrepExcel.ViewModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GrepExcel.View.Converters;
using System.Windows.Documents;
using System.ComponentModel;

namespace GrepExcel.View
{
    /// <summary>
    /// Interaction logic for SearchResultUc.xaml
    /// </summary>
    public partial class SearchResultUc : UserControl
    {
        private MainViewModel _mainVm = null;
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
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
           // lvSearchResults.UnselectAll();
        }

        private void lvSearchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void lvSearchResultsColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                lvSearchResults.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            lvSearchResults.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
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
                searchResultVm.LoadDataFromDatabase();
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
                    searchResultVm.LoadDataFromDatabase();
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

        /// <summary>
        /// Never use
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyPath_Click(object sender, RoutedEventArgs e)
        {
            var searchResult = lvSearchResults.SelectedItem as ResultInfo;

            if(searchResult != null)
            {
                Clipboard.SetText(searchResult.FileName);
            }
        }

        private void CopyResult_Click(object sender, RoutedEventArgs e)
        {
            var searchResult = lvSearchResults.SelectedItem as ResultInfo;

            if (searchResult != null)
            {
                Clipboard.SetText(searchResult.Result);
            }
        }
    }
}
