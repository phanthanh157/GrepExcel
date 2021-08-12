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

namespace GrepExcel.View
{
    /// <summary>
    /// Interaction logic for SearchResultUc.xaml
    /// </summary>
    public partial class SearchResultUc : UserControl
    {
        public SearchResultUc()
        {
            InitializeComponent();
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
            if(e.Key == Key.Return)
            {
                MessageBox.Show(txtFilter.Text);
            }
        }
    }
}
