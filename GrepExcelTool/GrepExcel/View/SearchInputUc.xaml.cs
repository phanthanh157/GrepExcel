using GrepExcel.Excel;
using GrepExcel.ViewModel;
using System.IO;
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
        private bool _isMatchCase = false;
        private bool _isLowerOrUper = false;
        public SearchInputUc()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            cobMethod.SelectedIndex = 1;
            cobTarget.SelectedIndex = 0;
            //txtSearch.Text = "thanh";
            //txtFolder.Text = @"D:\VBA-Excel";//test tam
        }

        public bool Validate()
        {
            if (string.IsNullOrEmpty(txtSearch.Text))
            {
                MessageBox.Show("Search input empty", "Input information", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            if (string.IsNullOrEmpty(txtFolder.Text))
            {
                MessageBox.Show("Folder input empty", "Input information", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            if (!Directory.Exists(txtFolder.Text))
            {
                MessageBox.Show("Directory input not exits", "Input information", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }


            return true;
        }

        private void btnOpenFolder_click(object sender, RoutedEventArgs e)
        {
            var browser = new System.Windows.Forms.FolderBrowserDialog();
            var result = browser.ShowDialog();

            if (System.Windows.Forms.DialogResult.OK == result)
            {
                txtFolder.Text = browser.SelectedPath;
            }

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;

            var searchInputVm = new SearchInputVm();

            var inputInfo = new SearchInfo()
            {
                Search = txtSearch.Text,
                Folder = txtFolder.Text,
                Method = (TypeMethod)cobMethod.SelectedValue,
                Target = (TypeTarget)cobTarget.SelectedValue,
                IsLowerOrUper = _isLowerOrUper,
                IsMatchCase = _isMatchCase,
                IsTabActive = true
            };

            searchInputVm.CommandSearch.Execute(inputInfo);
        }

        private void btnOptionLowAndUper_Click(object sender, RoutedEventArgs e)
        {
            _isLowerOrUper = _isLowerOrUper == true ? false : true;

            if (_isLowerOrUper)
            {
                btnOptionLowAndUper.Background = Brushes.Yellow;
            }
            else
            {
                btnOptionLowAndUper.Background = Brushes.Transparent;
            }
        }

        private void btnOptionMatchCase_Click(object sender, RoutedEventArgs e)
        {
            _isMatchCase = _isMatchCase == true ? false : true;

            if (_isMatchCase)
            {
                btnOptionMatchCase.Background = Brushes.Yellow;
            }
            else
            {
                btnOptionMatchCase.Background = Brushes.Transparent;
            }
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                //TODO: Dang bi loi khong textchanged khong thay doi
                //if (!Validate()) return;

                //var searchInputVm = new SearchInputVm();

                //var inputInfo = new SearchInfo()
                //{
                //    Search = txtSearch.Text,
                //    Folder = txtFolder.Text,
                //    Method = (TypeMethod)cobMethod.SelectedValue,
                //    Target = (TypeTarget)cobTarget.SelectedValue,
                //    IsLowerOrUper = _isLowerOrUper,
                //    IsMatchCase = _isMatchCase,
                //    IsTabActive = true
                //};

                //searchInputVm.CommandSearch.Execute(inputInfo);
            }
        }
    }
}
