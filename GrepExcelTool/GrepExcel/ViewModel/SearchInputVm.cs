using GrepExcel.Excel;
using GrepExcel.View;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GrepExcel.ViewModel
{
    public struct MethodView
    {
        public string Icon { get; set; }
        public TypeMethod Method { get; set; }
        public string Name { get; set; }
    }

    public struct TargetView
    {
        public string Icon { get; set; }
        public TypeTarget Target { get; set; }
        public string Name { get; set; }
    }

    public class SearchInputVm
    {
        private static readonly log4net.ILog log_ = LogHelper.GetLogger();
        private readonly MainViewModel mainVm_= MainViewModel.Instance;
        private ICommand cmdSearch_ = null;
        public ObservableCollection<MethodView> Methods { get; set; }
        public ObservableCollection<TargetView> Targets { get; set; }

        public SearchInputVm()
        {
            Methods = new ObservableCollection<MethodView>();
            Targets = new ObservableCollection<TargetView>();
            LoadItem();
        }

        public ICommand CommandSearch
        {
            get
            {
                if (cmdSearch_ == null)
                {
                    cmdSearch_ = new Commands.AsyncRelayCommand(sender => CommandSeachHander(sender));
                }
                return cmdSearch_;
            }
        }

        private async Task CommandSeachHander(object sender)
        {
            if (sender == null)
            {
                log_.Error("Sender is null");
                return;
            }

            var inputInfo = sender as SearchInfo;
            var mainVm = MainViewModel.Instance;
            var excelStore = ExcelStoreManager.Instance;
            var listSearchVm = ListSearchVm.Instance;
            var listRecentVm = RecentSearchVm.Instance;

            //check exits database
            SearchInfo searchInfoFirst = null;
            if (CheckExitsSearchInfo(inputInfo, out searchInfoFirst))
            {
                MessageBox.Show("Search keyword is exits on database", "Searching...", MessageBoxButton.OK, MessageBoxImage.Information);

                listSearchVm.ShowTab(ShowInfo.Create(searchInfoFirst));
                return;
            }

            // await Task.Delay(1000);
            mainVm.NotifyTaskRunning(inputInfo.Search);

            //Insert input info to database
            SqlResult sqlResult = excelStore.InsertSearchInfo(inputInfo);

            if (SqlResult.InsertSucess == sqlResult)
            {
                inputInfo.Id = excelStore.LastIndexSearch();// add id 

                //Search process
                var grep = new Grep();
                grep.GrepEvent += Grep_GrepEvent;
                //grep.GrepSpeedNonTask(inputInfo);
                await grep.GrepAsync(inputInfo);

                grep.GrepEvent -= Grep_GrepEvent;

                //Display result when finish search
                listSearchVm.ShowTab(ShowInfo.Create(inputInfo));
             
                //add observer list serach
                listSearchVm.SearchInfos.Add(ShowInfo.Create(inputInfo));

                //add first list recent
                listRecentVm.Recents.Insert(0, ShowInfo.Create(inputInfo));
            }

            mainVm.NotifyTaskRunning(inputInfo.Search, false);
        }

        private void Grep_GrepEvent(object sender, GrepInfoArgs e)
        {
            if (e is null)
                return;
            int percent = e.CurrentFileIndex * 100 / e.TotalFiles;
            mainVm_.ShowPercentSearching(percent, e.CurrentMatch);

            //log_.DebugFormat("Search Percent: {0}", percent);
        }

        private bool CheckExitsSearchInfo(SearchInfo searchInfo, out SearchInfo outSearchInfo)
        {
            outSearchInfo = null;

            var excelStore = ExcelStoreManager.Instance;

            var dataSearch = excelStore.GetSearchInfoAll();

            if (dataSearch is null)
                return false;

            var filter = dataSearch.Where(res => res == searchInfo);

            if (filter.Count() > 0)
            {
                //searchId = filter.First().Id;
                outSearchInfo = filter.First();
                return true;
            }
            return false;
        }


        public void LoadItem()
        {
            Methods.Add(new MethodView() { Icon = "Folder", Method = TypeMethod.Folder, Name = "Folder" });
            Methods.Add(new MethodView() { Icon = "FolderMultiple", Method = TypeMethod.SubFolder, Name = "SubFolder" });

            Targets.Add(new TargetView() { Icon = "CurrencyUsd", Target = TypeTarget.Value, Name = "Value" });
            Targets.Add(new TargetView() { Icon = "Comment", Target = TypeTarget.Comment, Name = "Comment" });
            Targets.Add(new TargetView() { Icon = "Function", Target = TypeTarget.Fomular, Name = "Fomular" });
        }
    }
}
