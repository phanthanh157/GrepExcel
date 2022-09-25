using GrepExcel.ViewModel;
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
        private SettingVm _settingVm = null;
        public SearchSettings()
        {
            InitializeComponent();
            _settingVm = SettingVm.Instance;

            Base.Check(_settingVm);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void cobMaxSearch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = cobMaxSearch.SelectedItem as ComboBoxItem;

            switch (item.Content.ToString())
            {
                case "100":
                    Config.AddUpdateAppSettings("MAX_SEARCH", "100");
                    break;
                case "300":
                    Config.AddUpdateAppSettings("MAX_SEARCH", "300");
                    break;
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
                case "30":
                    Config.AddUpdateAppSettings("MAX_FILE", "30");
                    break;
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

        private void txtNumberRecent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(txtNumberRecent.Text.Length > 0)
            {
                int result;
                bool tryConvert = int.TryParse(txtNumberRecent.Text, out result);
                if (tryConvert)
                {
                    SettingArgs settingArgs = new SettingArgs
                    {
                        NumberRecent = result
                    };

                    _settingVm.Notify(settingArgs);
                    Config.AddUpdateAppSettings("NUMBER_RECENTS", result.ToString());
                }
                    
            }
        }
    }
}
