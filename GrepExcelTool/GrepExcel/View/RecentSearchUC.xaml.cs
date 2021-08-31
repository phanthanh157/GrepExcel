using GrepExcel.Excel;
using GrepExcel.ViewModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace GrepExcel.View
{
    /// <summary>
    /// Interaction logic for RecentSearchUC.xaml
    /// </summary>
    public partial class RecentSearchUC : UserControl
    {
        private RecentSearchVm _recentVm = null;
        public RecentSearchUC()
        {
            InitializeComponent();
            _recentVm = RecentSearchVm.Instance;
            this.DataContext = _recentVm;
            lstRecent.ItemsSource = _recentVm.Recents;
        }

        private void ItemOnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void lstRecent_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var recentSearch = lstRecent.SelectedItem as SearchInfo;

            if (recentSearch == null)
            {
                ShowDebug.MsgErr(F.FLMD(), "Select search result is null");
                return;
            }

            ListSearchVm.Instance.ShowTabSearchResult(recentSearch);
        }
    }
}
