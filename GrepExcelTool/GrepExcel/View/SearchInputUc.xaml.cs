using System;
using System.Collections.Generic;
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
using GrepExcel.Excel;
using GrepExcel.ViewModel;

namespace GrepExcel.View
{
    /// <summary>
    /// Interaction logic for SearchInputUc.xaml
    /// </summary>
    public partial class SearchInputUc : UserControl
    {
        public SearchInputUc()
        {
            InitializeComponent();
 
        }

        private void btnOpenFolder_click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            var searchInputVm = new SearchInputVm();

            var inputInfo = new SearchInfo()
            {
                Search = "keyword",
                Folder = "D:/acb",
                Method = TypeMethod.Folder,
                Target = TypeTarget.Value,
                IsLowerOrUper = true,
                IsMatchCase = false
            };

            searchInputVm.CommandSearch.Execute(inputInfo);
        }
    
    }
}
