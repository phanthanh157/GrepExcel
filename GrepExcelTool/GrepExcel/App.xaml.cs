using System;
using System.Windows;
using GrepExcel.ViewModel;
using System.IO;
using GrepExcel.Excel;
using System.Windows.Controls;

namespace GrepExcel
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            MainWindow window = new MainWindow();
            //load config.
            Config config = new Config();
            config.Load();
            //Load instance and set datacontext
            var mainVm = MainViewModel.Instance;
            window.DataContext = mainVm;
            mainVm.LoadTabControl();

            mainVm.Close += (object cl, EventArgs ev) =>
            {
                Config.AddUpdateAppSettings("TAB_CURRENT_ACTIVE", mainVm.TabActive.ToString());
                window.Close();
            };

            window.Closed += (object wcl, EventArgs ev1) => {
                Config.AddUpdateAppSettings("TAB_CURRENT_ACTIVE", mainVm.TabActive.ToString());
            };

            //Create table excel store.
            if (!File.Exists(Define.Database))
            {
                ExcelStoreManager.Instance.CreateTable();
            }



            window.Show();
        }

    
    }
}
