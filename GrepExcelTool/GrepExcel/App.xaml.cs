using System;
using System.Windows;
using GrepExcel.ViewModel;

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

            var mainVm = MainViewModel.Instance;
            window.DataContext = mainVm;

            mainVm.Close += (object cl, EventArgs ev) =>
            {
                window.Close();
            };
           

            window.Show();
        }

    }
}
