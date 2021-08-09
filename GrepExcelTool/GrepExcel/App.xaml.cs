using System.Windows;

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




            window.Show();
        }

    }
}
