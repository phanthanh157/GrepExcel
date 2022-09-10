using GrepExcel.Excel;
using GrepExcel.View;
using System;
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
                    cmdSearch_ = new Commands.AsyncRelayCommand((sender) => CommandSeachHander(sender),ex => SearchException(ex));
                }
                return cmdSearch_;
            }
        }

        private void SearchException(Exception ex)
        {

            log_.DebugFormat("search exception", ex);

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
            SearchInfo searchInfoFirst;
            if (CheckExitsSearchInfo(inputInfo, out searchInfoFirst))
            {
                MessageBox.Show("Search keyword is exits on database", "Searching...", MessageBoxButton.OK, MessageBoxImage.Information);

                await listSearchVm.ShowTab(ShowInfo.Create(searchInfoFirst), false);
                return;
            }

            // await Task.Delay(1000);
            mainVm.NotifyTaskRunning(inputInfo.Search);

            //Insert input info to database
            SqlResult sqlResult = excelStore.InsertSearchInfo(inputInfo);

            if (SqlResult.InsertSucess == sqlResult)
            {
                inputInfo.Id = excelStore.LastIndexSearch();// add id 

                //Display result when finish search
                await listSearchVm.ShowTab(ShowInfo.Create(inputInfo), false);
             
                //add observer list serach
                listSearchVm.SearchInfos.Add(ShowInfo.Create(inputInfo));

                //add first list recent
                listRecentVm.Recents.Insert(0, ShowInfo.Create(inputInfo));
            }

            mainVm.NotifyTaskRunning(inputInfo.Search, false);
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
