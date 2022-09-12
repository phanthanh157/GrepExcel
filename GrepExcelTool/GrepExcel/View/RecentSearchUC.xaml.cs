using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using GrepExcel.ViewModel;

namespace GrepExcel.View
{
    /// <summary>
    /// Interaction logic for RecentSearchUC.xaml
    /// </summary>
    public partial class RecentSearchUC : UserControl
    {
        public RecentSearchUC()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            var recentVm = RecentSearchVm.Instance;
            Base.Check(recentVm);

            this.DataContext = recentVm;
            //lstRecent.ItemsSource = recentVm.Recents;
        }

        private void lstRecent_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var showInfo = (ShowInfo)lstRecent.SelectedItem;

            ListSearchVm.Instance.ShowTabExits(showInfo);
        }

     
    }
}
