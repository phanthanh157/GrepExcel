using System;
using System.IO;
using System.Windows;
using GrepExcel.Excel;
using GrepExcel.ViewModel;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace GrepExcel
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly log4net.ILog log_ = LogHelper.GetLogger();
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            log_.Info("EXCEL APPLICATION START");

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

            window.Closed += (object wcl, EventArgs ev1) =>
            {
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
