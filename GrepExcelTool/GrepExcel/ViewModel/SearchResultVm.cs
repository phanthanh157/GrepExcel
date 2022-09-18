using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GrepExcel.Commands;
using GrepExcel.Excel;
using Microsoft.Win32;

namespace GrepExcel.ViewModel
{
    public enum TypeCloseTab
    {
        Close,
        CloseAllButThis,
        CloseToRight,
        CloseToLeft
    }
    public struct OptionFilter
    {
        public string Value { get; set; }
        public string Color { get; set; }
    }

    public class SearchResultVm : TabControl
    {
        private static readonly log4net.ILog log_ = LogHelper.GetLogger();
        private readonly ExcelStoreManager excelStore_ = ExcelStoreManager.Instance;
        private ObservableCollection<ResultInfo> resultInfos_;
        private readonly object resultLock_ = new object();
        private bool isLoading_ = false;
        private string columnNumbers_ = string.Empty;


        public ObservableCollection<ResultInfo> ResultInfos
        {
            get { return resultInfos_; }
            private set
            {
                if (resultInfos_ != value)
                {
                    resultInfos_ = value;
                    BindingOperations.EnableCollectionSynchronization(resultInfos_, resultLock_);
                }
            }
        }

        public ObservableCollection<OptionFilter> OptionFilters { get; set; }

        private ICommand _commandRefresh;
        private ICommand _commandStopLoading;
        private ICommand _searchResult;
        private ICommand _goToDocument;
        private ICommand _commandFocusFind;
        private ICommand _copyFullPath;
        private ICommand _copyResult;
        private ICommand _commandCloseTab;
        private ICommand _commandDelete;
        private ICommand _commandExport;
        public SearchResultVm(UserControl userControl,
                              string tabName, 
                              int searchId, 
                              ShowInfo showInfo) 
            : base(userControl, tabName)
        {
            SearchId = searchId;
            ResultInfos = new ObservableCollection<ResultInfo>();
            OptionFilters = new ObservableCollection<OptionFilter>();
            SearchInfo = showInfo;
            InitClass();
        }

        private void InitClass()
        {
            OptionFilters.Add(new OptionFilter { Color = "Black", Value = "Result" });
            OptionFilters.Add(new OptionFilter { Color = "Black", Value = "FileName" });
            OptionFilters.Add(new OptionFilter { Color = "Black", Value = "Sheet" });

            LoadDataFromDatabase();
        }

        public void LoadDataFromDatabase()
        {
            var listResult = excelStore_.GetResultInfoBySearchId(SearchId);

            if (listResult is null)
                return;

            //Clear before load again.
            ResultInfos.Clear();
            listResult.ForEach(x => ResultInfos.Add(x));
        }

        public ShowInfo SearchInfo { get; set; }

        public int SearchId { get; set; }

        public ResultInfo SelectedItem { get; set; }

        public bool IsLoading
        {
            get { return isLoading_; }
            set
            {
                if (isLoading_ != value)
                {
                    isLoading_ = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ColumnNumbers
        {
            get { return columnNumbers_; }
            set
            {
                if (columnNumbers_ != value)
                {
                    columnNumbers_ = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand CommandCloseTab
        {
            get
            {
                if (_commandCloseTab == null)
                {
                    _commandCloseTab = new RelayCommand(x => { CommandCloseTabHandler(x); });
                }
                return _commandCloseTab;
            }

        }


        private void CommandCloseTabHandler(object sender)
        {
            Base.Check(sender);

            var mainVm = MainViewModel.Instance;
            var typeClose = (TypeCloseTab)sender;
            int tabActive = mainVm.TabActive;
            if (mainVm.Tabs.Count != 0 && tabActive != -1)
            {
                switch (typeClose)
                {
                    case TypeCloseTab.Close:
                        {
                            log_.DebugFormat("Close tab: index = {0}", tabActive);
                            var resultVm = mainVm.GetActiveSearchResultVm();
                            if (resultVm == null) return;
                            var searchInfo = excelStore_.GetSearchInfoById(resultVm.SearchId);

                            if (searchInfo != null)
                            {
                                searchInfo.IsTabActive = false;
                                if (SqlResult.UpdateSuccess != excelStore_.UpdateSearchInfo(searchInfo))
                                {
                                    log_.Error("Update field 'tabIndex' in database is fail");
                                }
                            }
                            mainVm.Tabs.RemoveAt(tabActive);
                            CommandStopLoadingHandler(tabActive);
                        }
                        break;
                    case TypeCloseTab.CloseAllButThis:
                        {
                            CommandCloseTabHandler(TypeCloseTab.CloseToRight);
                            CommandCloseTabHandler(TypeCloseTab.CloseToLeft);
                        }
                        break;
                    case TypeCloseTab.CloseToLeft:
                        {
                            for (int idx = tabActive - 1; idx > -1; idx--)
                            {
                                var resultVm = mainVm.GetTabContent(idx);
                                if (resultVm == null) return;
                                var searchInfo = excelStore_.GetSearchInfoById(resultVm.SearchId);

                                if (searchInfo != null)
                                {
                                    searchInfo.IsTabActive = false;
                                    if (SqlResult.UpdateSuccess != excelStore_.UpdateSearchInfo(searchInfo))
                                    {
                                        log_.Error("Update field 'tabIndex' in database is fail");
                                    }
                                }
                                mainVm.Tabs.RemoveAt(idx);
                                CommandStopLoadingHandler(idx);
                            }

                        }
                        break;
                    case TypeCloseTab.CloseToRight:
                        for (int idx = mainVm.Tabs.Count - 1; idx > tabActive; idx--)
                        {
                            var resultVm = mainVm.GetTabContent(idx);
                            if (resultVm == null) return;
                            var searchInfo = excelStore_.GetSearchInfoById(resultVm.SearchId);

                            if (searchInfo != null)
                            {
                                searchInfo.IsTabActive = false;
                                if (SqlResult.UpdateSuccess != excelStore_.UpdateSearchInfo(searchInfo))
                                {
                                    log_.Error("Update field 'tabIndex' in database is fail");
                                }
                            }
                            mainVm.Tabs.RemoveAt(idx);
                            CommandStopLoadingHandler(idx);
                        }
                        break;
                    default:
                        break;
                }
            }
        }


        public ICommand CommandRefresh
        {
            get
            {
                if (_commandRefresh == null)
                {
                    _commandRefresh = new AsyncRelayCommand((sender) => CommandRefeshHandler(sender));
                }
                return _commandRefresh;
            }
        }

        private async Task CommandRefeshHandler(object sender)
        {
            var searchInfo = excelStore_.GetSearchInfoById(SearchId);

            if (searchInfo == null)
            {
                log_.Error("search info is null");
                return;
            }

            //Delete result info old
            if (SqlResult.DeleteSuccess != excelStore_.DeleteResultInfoBySearchId(searchInfo))
            {
                log_.Error("Delete result info fail");
                return;
            }

            ResultInfos.Clear();

            await ListSearchVm.Instance.ShowTab(ShowInfo.Create(searchInfo), true);

        }

        public ICommand CommandStopLoading
        {
            get
            {
                if (_commandStopLoading == null)
                {
                    _commandStopLoading = new RelayCommand((sender) => CommandStopLoadingHandler(-2));
                }
                return _commandStopLoading;
            }
        }

        private void CommandStopLoadingHandler(int index)
        {
            var mainVm = MainViewModel.Instance;
            var listSearchResult = ListSearchVm.Instance;
            var recentSearchVm = RecentSearchVm.Instance;
            int tabIndex;

            if (IsLoading)
            {
                if (index == -2)
                    tabIndex = mainVm.TabActive + 1;
                else
                    tabIndex = index + 1;

                listSearchResult.StopSearching(tabIndex);

                recentSearchVm.UpdateTotalMatch(SearchInfo);
                listSearchResult.UpdateTotalMatch(SearchInfo);
              
                IsLoading = false;
            }
        }

        public ICommand CommandSearchResult
        {
            get
            {
                if (_searchResult == null)
                {
                    _searchResult = new RelayCommand((sender) => CommandSearchResultHander(sender));
                }
                return _searchResult;
            }
        }

        private void CommandSearchResultHander(object sender)
        {
            if (sender == null)
            {
                log_.Warn("sender is null");
                return;
            }
            // var mainVm = MainViewModel.Instance;OptionFilter
            string keySearch = sender.GetType().GetProperty("Search").GetValue(sender, null).ToString();
            string optionFilter = sender.GetType().GetProperty("OptionFilter").GetValue(sender, null).ToString();

            List<ResultInfo> resultInfos = excelStore_.GetResultInfoBySearchId(SearchId);

            if (resultInfos != null)
            {
                IEnumerable<ResultInfo> filter = null;
                switch (optionFilter)
                {
                    case "Result":
                        filter = resultInfos.Where(x => x.Result.IndexOf(keySearch, StringComparison.OrdinalIgnoreCase) != -1);
                        break;
                    case "FileName":
                        filter = resultInfos.Where(x => x.FileName.IndexOf(keySearch, StringComparison.OrdinalIgnoreCase) != -1);
                        break;
                    case "Sheet":
                        filter = resultInfos.Where(x => x.Sheet.IndexOf(keySearch, StringComparison.OrdinalIgnoreCase) != -1);
                        break;
                    default:
                        break;

                }

                ResultInfos.Clear();//Delete old result
                foreach (var item in filter)
                {
                    ResultInfos.Add(item);
                }
            }
        }


        public ICommand CommandGotoDocument
        {
            get
            {
                if (_goToDocument == null)
                {
                    _goToDocument = new AsyncRelayCommand((sender) => CommandGotoDocumentHander(sender));
                }
                return _goToDocument;
            }
        }

        private async Task CommandGotoDocumentHander(object sender)
        {
            if (sender == null)
            {
                log_.Error("sender is null");
                MessageBox.Show("You have not selected any items yet?\nPlease select one item.", "Go to document", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            var resultInfo = sender as ResultInfo;
            var grep = new Grep();

            await grep.OpenFileAsync(resultInfo);

        }



        public ICommand CommandFocusFind
        {
            get
            {
                if (_commandFocusFind == null)
                {
                    _commandFocusFind = new RelayCommand(x => CommandFocusFindHandler(x));

                }
                return _commandFocusFind;
            }
        }

        private void CommandFocusFindHandler(object sender)
        {
            if (sender is null)
            {
                log_.Error("sender is null");
                return;
            }

            var txtFilter = sender as TextBox;

            txtFilter.Focusable = true;
            txtFilter.Focus();

        }


        public ICommand CopyFullPath
        {
            get
            {
                if (_copyFullPath == null)
                {
                    _copyFullPath = new RelayCommand(x => CopyFullPathHandler(x));
                }
                return _copyFullPath;
            }
        }

        private void CopyFullPathHandler(object sender)
        {
            if (sender is null)
            {
                log_.Error("sender is null");
                return;
            }

            var searchResult = sender as ResultInfo;

            Clipboard.SetText(searchResult.FileName);
        }

        public ICommand CopyResult
        {
            get
            {
                if (_copyResult == null)
                {
                    _copyResult = new RelayCommand(x => CopyResultHandler(x));
                }
                return _copyResult;
            }
        }

        private void CopyResultHandler(object sender)
        {
            if (sender is null)
            {
                log_.Error("sender is null");
                return;
            }

            var searchResult = sender as ResultInfo;

            Clipboard.SetText(searchResult.Result);
        }


        public ICommand CommandDelete
        {
            get
            {
                if (_commandDelete == null)
                {
                    _commandDelete = new RelayCommand(x => CommandDeleteHandler());
                }
                return _commandDelete;
            }
        }

        private void CommandDeleteHandler()
        {
            var mainVm = MainViewModel.Instance;
            var excelStore = ExcelStoreManager.Instance;
            var listSearchVm = ListSearchVm.Instance;

            var resultVm = mainVm.GetActiveSearchResultVm();
            if (resultVm == null) return;
            var searchInfo = excelStore.GetSearchInfoById(resultVm.SearchId);

            listSearchVm.DelSearchResult(ShowInfo.Create(searchInfo));


        }

        public ICommand CommandExport
        {
            get
            {
                if (_commandExport == null)
                {
                    _commandExport = new RelayCommand(x => CommandExportHandler());
                }
                return _commandExport;
            }
        }

        private void CommandExportHandler()
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "files (*.csv)|*.csv";
            saveFileDialog.Title = "Export file search result";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.FileName = SearchInfo.Info.Search;
            bool? openDialog = saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                if (openDialog != null && openDialog == true)
                {
                    string directorySave = saveFileDialog.FileName;

                    ////write file.
                    CsvManager.WriteDataToCsv<ResultInfo>(directorySave, ResultInfos.ToList());

                }


            }


        }

        public void AddResult(ResultInfo resultInfo)
        {
            lock (resultLock_)
            {
                ResultInfos.Add(resultInfo);
            }
        }
    }
}
