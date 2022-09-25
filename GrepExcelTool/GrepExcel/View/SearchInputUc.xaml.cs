using GrepExcel.Excel;
using GrepExcel.ViewModel;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GrepExcel.View
{
    /// <summary>
    /// Interaction logic for SearchInputUc.xaml
    /// </summary>
    public partial class SearchInputUc : UserControl
    {
        private bool isMatchCase_ = false;
        private bool isLowerOrUper_ = false;
        private string folder_ = string.Empty;
        private string search_ = string.Empty;
        public SearchInputUc()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            cobMethod.SelectedIndex = 1;
            cobTarget.SelectedIndex = 0;

            var lstSearchInfo = ExcelStoreManager.Instance.GetSearchInfoByLimit(10);
            if(lstSearchInfo != null)
            {
                var searchInfo = lstSearchInfo.FirstOrDefault();
                if(searchInfo != null)
                {
                    txtFolder.Text = searchInfo.Folder;
                }
            }
        }

        public bool Validate()
        {
            var itemFolder = txtFolder.SelectedItem as SearchInfo;
            var itemSearch = txtSearch.SelectedItem as SearchInfo;

            folder_ = itemFolder is null ? txtFolder.Editor.Text : itemFolder.Folder;
            search_ = itemSearch is null ? txtSearch.Editor.Text : itemSearch.Search;

            if (string.IsNullOrEmpty(search_))
            {
                //MessageBox.Show("Search input empty", "Input information", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            if (string.IsNullOrEmpty(folder_))
            {
                MessageBox.Show("Folder input empty", "Input information", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

      
            if (!Directory.Exists(folder_))
            {
                MessageBox.Show("Directory input not exits", "Input information", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            txtSearch.Editor.Text = string.Empty;
            return true;
        }

        private void btnOpenFolder_click(object sender, RoutedEventArgs e)
        {
            var browser = new System.Windows.Forms.FolderBrowserDialog();
            if (browser.ShowDialog() == System.Windows.Forms.DialogResult.OK )
                txtFolder.Text = browser.SelectedPath;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;

            var searchInputVm = new SearchInputVm();

            //get folder
     
            var inputInfo = new SearchInfo()
            {
                Search = search_,
                Folder = folder_,
                Method = (TypeMethod)cobMethod.SelectedValue,
                Target = (TypeTarget)cobTarget.SelectedValue,
                IsLowerOrUper = isLowerOrUper_,
                IsMatchCase = isMatchCase_,
                IsTabActive = true
            };

            searchInputVm.CommandSearch.Execute(inputInfo);
        }

        private void btnOptionLowAndUper_Click(object sender, RoutedEventArgs e)
        {
            isLowerOrUper_ = isLowerOrUper_ == true ? false : true;
            btnOptionLowAndUper.Background = isLowerOrUper_ ? Brushes.Yellow : Brushes.Transparent;
        }

        private void btnOptionMatchCase_Click(object sender, RoutedEventArgs e)
        {
            isMatchCase_ = isMatchCase_ == true ? false : true;
            btnOptionMatchCase.Background = isMatchCase_? Brushes.Yellow : Brushes.Transparent;
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                btnSearch.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }

    }
}
