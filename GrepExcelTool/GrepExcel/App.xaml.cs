using System;
using System.Windows;
using GrepExcel.ViewModel;
using System.IO;
using GrepExcel.Excel;

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

            mainVm.Close += (object cl, EventArgs ev) =>
            {
                window.Close();
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
