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
        private readonly RecentSearchVm recentVm_ = RecentSearchVm.Instance;
        private readonly ListSearchVm listSearchVm_ = ListSearchVm.Instance;
        public RecentSearchUC()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            Base.Check(recentVm_);
            this.DataContext = recentVm_;
            //lstRecent.ItemsSource = recentVm.Recents;
        }

        private void lstRecent_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var showInfo = (ShowInfo)lstRecent.SelectedItem;

            if(listSearchVm_ != null)
                listSearchVm_.ShowTabExits(showInfo);
        }

     
    }
}
