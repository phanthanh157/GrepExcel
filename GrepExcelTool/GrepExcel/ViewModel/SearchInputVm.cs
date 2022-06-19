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
        private ICommand _commandSearch = null;

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
                if (_commandSearch == null)
                {
                    _commandSearch = new Commands.AsyncRelayCommand(sender => CommandSeachHander(sender));
                }
                return _commandSearch;
            }
        }

        private async Task CommandSeachHander(object sender)
        {
            ShowDebug.Msg(F.FLMD(), "Handler");
            if (sender == null)
            {
                ShowDebug.Msg(F.FLMD(), "Sender is null");
                return;
            }
            var inputInfo = sender as SearchInfo;
            var mainVm = MainViewModel.Instance;
            var excelStore = ExcelStoreManager.Instance;
            var listSearch = ListSearchVm.Instance;
            var listRecent = RecentSearchVm.Instance;

            //check exits database
            int searchIdFirst = -1;
            if (CheckExitsSearchInfo(inputInfo, ref searchIdFirst))
            {
                MessageBox.Show("Search keyword is exits on database", "Searching", MessageBoxButton.OK, MessageBoxImage.Information);
                inputInfo.Id = searchIdFirst;

                listSearch.ShowTabSearchResult(new ShowInfo().SetData(inputInfo));
                ShowDebug.Msg(F.FLMD(), "Search info is exits, searchId= {0}", searchIdFirst);
                return;
            }

            // await Task.Delay(1000);
            mainVm.NotifyTaskRunning(inputInfo.Search);

            //Insert input info to database
            SqlResult sqlResult = excelStore.InsertSearchInfo(inputInfo);
            if (SqlResult.InsertSucess == sqlResult)
            {
                ShowDebug.Msg(F.FLMD(), "Insert Search info success");
                inputInfo.Id = excelStore.LastIndexSearch();// add id 

                //Search process
                var grep = new Grep();
                grep.GrepEvent += Grep_GrepEvent;
                //grep.GrepSpeedNonTask(inputInfo);
                await grep.GrepAsync(inputInfo);

                grep.GrepEvent -= Grep_GrepEvent;

                //Display result
                int tabIndex = -1;
                bool isTabOpen = mainVm.isTabOpen(inputInfo, ref tabIndex);
                if (!isTabOpen)
                {
                    SearchResultVm tabResult = new SearchResultVm();
                    tabResult.Control = new SearchResultUc();
                    tabResult.TabName = inputInfo.Search;
                    tabResult.SearchId = inputInfo.Id;
                    tabResult.LoadDataFromDatabase();
                    mainVm.AddTabControl(tabResult);
                }
                else // Tab is open and load again data
                {
                    if (tabIndex != -1)
                    {
                        var resultVm = mainVm.GetSearchResultVm(tabIndex);
                        if (resultVm != null)
                        {
                            resultVm.LoadDataFromDatabase();
                        }
                    }
                }

                mainVm.NotifyTaskRunning(inputInfo.Search, false);
                //add observer list serach
                listSearch.SearchInfos.Add(new ShowInfo().SetData(inputInfo));
                listRecent.LoadRecents();
            }
        }

        private void Grep_GrepEvent(object sender, GrepInfoArgs e)
        {
            if(e is null)
            {
                return;
            }

            ShowDebug.Msg(F.FLMD(), "Search {0} ,Total File {1} , Current File {2}, Index {3}, Count Result {4} ", e.SearchText,e.TotalFiles,e.CurrentFile,e.CurrentFileIndex ,e.CurrentMatch);
            var mainVm = MainViewModel.Instance;

            int percent = e.CurrentFileIndex * 100 / e.TotalFiles;
            mainVm.SearchPercent = percent;
            mainVm.CurrentResults = e.CurrentMatch;

            ShowDebug.Msg(F.FLMD(), "Search Percent: {0}", percent);
        }

        private bool CheckExitsSearchInfo(SearchInfo searchInfo, ref int searchId)
        {
            var excelStore = ExcelStoreManager.Instance;

            var list = excelStore.GetSearchInfoAll();

            var filter = list.Where(res => res == searchInfo);

            if (filter.Count() > 0)
            {
                searchId = filter.First().Id;
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
