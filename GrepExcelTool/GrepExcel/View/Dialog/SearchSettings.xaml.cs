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
using System.Windows.Shapes;

namespace GrepExcel.View.Dialog
{
    /// <summary>
    /// Interaction logic for SearchSettings.xaml
    /// </summary>
    public partial class SearchSettings : Window
    {
        public SearchSettings()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void cobMaxSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ComboBoxItem)cobMaxSearch.SelectedItem;

            switch (item.Content.ToString())
            {
                case "500":
                    Config.AddUpdateAppSettings("MAX_SEARCH", "500");
                    break;
                case "1000":
                    Config.AddUpdateAppSettings("MAX_SEARCH", "1000");
                    break;
                case "2000":
                    Config.AddUpdateAppSettings("MAX_SEARCH", "2000");
                    break;
                case "5000":
                    Config.AddUpdateAppSettings("MAX_SEARCH", "5000");
                    break;
                default:
                    break;
            }
        }

        private void cobMaxFolders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ComboBoxItem)cobMaxFolders.SelectedItem;

            switch (item.Content.ToString())
            {
                case "50":
                    Config.AddUpdateAppSettings("MAX_FOLDER", "50");
                    break;
                case "100":
                    Config.AddUpdateAppSettings("MAX_FOLDER", "100");
                    break;
                case "200":
                    Config.AddUpdateAppSettings("MAX_FOLDER", "200");
                    break;
                default:
                    break;
            }
        }

        private void cobMaxFiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = (ComboBoxItem)cobMaxFiles.SelectedItem;

            switch (item.Content.ToString())
            {
                case "50":
                    Config.AddUpdateAppSettings("MAX_FILE", "50");
                    break;
                case "100":
                    Config.AddUpdateAppSettings("MAX_FILE", "100");
                    break;
                case "200":
                    Config.AddUpdateAppSettings("MAX_FILE", "200");
                    break;
                default:
                    break;
            }
        }
    }
}
