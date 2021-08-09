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
using GrepExcel.ViewModel;

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
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_mainVm.Tabs.Count != 0 && tabAction.SelectedIndex != -1)
            {
                _mainVm.Tabs.RemoveAt(tabAction.SelectedIndex);
            }
        }

        private void tabAction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
