using GrepExcel.Commands;
using GrepExcel.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


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
        public ObservableCollection<ResultInfo> ResultInfos { get; set; }
        public ObservableCollection<OptionFilter> OptionFilters { get; set; }

        private ICommand _commandRefresh;
        private ICommand _searchResult;
        private ICommand _goToDocument;
        private ICommand _commandFocusFind;
        private ICommand _copyFullPath;
        private ICommand _copyResult;
        private ICommand _commandCloseTab;
        private ICommand _commandDelete;
        public SearchResultVm()
        {
            ResultInfos = new ObservableCollection<ResultInfo>();
            OptionFilters = new ObservableCollection<OptionFilter>();
            InitClass();
        }

        private void InitClass()
        {
            OptionFilters.Add(new OptionFilter { Color = "Black", Value = "Result" });
            OptionFilters.Add(new OptionFilter { Color = "Black", Value = "FileName" });
            OptionFilters.Add(new OptionFilter { Color = "Black", Value = "Sheet" });
        }

        public void LoadDataFromDatabase()
        {
            var excelStore = ExcelStoreManager.Instance;
            var listResult = excelStore.GetResultInfoBySearchId(SearchId);

            //Clear before load again.
            ResultInfos.Clear();
            foreach (var result in listResult)
            {
                ResultInfos.Add(result);
            }
        }

        public int SearchId { get; set; }

        public ResultInfo SelectedItem { get; set; }





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
            if (sender == null)
            {
                ShowDebug.Msg(F.FLMD(), "sender is null");
                return;
            }

            var mainVm = MainViewModel.Instance;
            var excelStore = ExcelStoreManager.Instance;
            var typeClose = (TypeCloseTab)sender;
            int tabActive = mainVm.TabActive;
            if (mainVm.Tabs.Count != 0 && tabActive != -1)
            {
                switch (typeClose)
                {
                    case TypeCloseTab.Close:
                        {
                            ShowDebug.Msg(F.FLMD(), "Close tab: index = {0}", tabActive);
                            var resultVm = mainVm.GetActiveSearchResultVm();
                            if (resultVm == null) return;
                            var searchInfo = excelStore.GetSearchInfoById(resultVm.SearchId);

                            if (searchInfo != null)
                            {
                                searchInfo.IsTabActive = false;
                                if (SqlResult.UpdateSuccess != excelStore.UpdateSearchInfo(searchInfo))
                                {
                                    ShowDebug.Msg(F.FLMD(), "Update field 'tabIndex' in database is fail");
                                }
                            }
                            mainVm.Tabs.RemoveAt(tabActive);
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
                                var resultVm = mainVm.GetSearchResultVm(idx);
                                if (resultVm == null) return;
                                var searchInfo = excelStore.GetSearchInfoById(resultVm.SearchId);

                                if (searchInfo != null)
                                {
                                    searchInfo.IsTabActive = false;
                                    if (SqlResult.UpdateSuccess != excelStore.UpdateSearchInfo(searchInfo))
                                    {
                                        ShowDebug.Msg(F.FLMD(), "Update field 'tabIndex' in database is fail");
                                    }
                                }
                                mainVm.Tabs.RemoveAt(idx);
                            }

                        }
                        break;
                    case TypeCloseTab.CloseToRight:
                        for (int idx = mainVm.Tabs.Count - 1; idx > tabActive; idx--)
                        {
                            var resultVm = mainVm.GetSearchResultVm(idx);
                            if (resultVm == null) return;
                            var searchInfo = excelStore.GetSearchInfoById(resultVm.SearchId);

                            if (searchInfo != null)
                            {
                                searchInfo.IsTabActive = false;
                                if (SqlResult.UpdateSuccess != excelStore.UpdateSearchInfo(searchInfo))
                                {
                                    ShowDebug.Msg(F.FLMD(), "Update field 'tabIndex' in database is fail");
                                }
                            }
                            mainVm.Tabs.RemoveAt(idx);
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
                    _commandRefresh = new Commands.AsyncRelayCommand((sender) => CommandRefeshHandler(sender));
                }
                return _commandRefresh;
            }
        }

        private async Task CommandRefeshHandler(object sender)
        {
            var mainVm = MainViewModel.Instance;
            var excelStore = ExcelStoreManager.Instance;
            var grep = new Grep();

           
            var searchInfo = excelStore.GetSearchInfoById(SearchId);

            if (searchInfo == null)
            {
                ShowDebug.Msg(F.FLMD(), "search info is null");
                return;
            }

            //Delete result info old
            if (SqlResult.DeleteSuccess != excelStore.DeleteResultInfoBySearchId(searchInfo))
            {
                ShowDebug.Msg(F.FLMD(), "Delete result info fail");
                return;
            }
            mainVm.NotifyTaskRunning(searchInfo.Search);

            grep.GrepEvent += Grep_GrepEvent;

            await grep.GrepAsync(searchInfo);

            grep.GrepEvent -= Grep_GrepEvent;

            LoadDataFromDatabase();

            mainVm.NotifyTaskRunning(searchInfo.Search, false);
        }

        private void Grep_GrepEvent(object sender, GrepInfoArgs e)
        {
            if(e is null)
            {
                return;
            }

            var mainVm = MainViewModel.Instance;

            int percent = e.CurrentFileIndex * 100 / e.TotalFiles;
            mainVm.SearchPercent = percent;
            mainVm.CurrentResults = e.CurrentMatch;
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
                ShowDebug.Msg(F.FLMD(), "sender is null");
                return;
            }
            // var mainVm = MainViewModel.Instance;OptionFilter
            string keySearch = sender.GetType().GetProperty("Search").GetValue(sender, null).ToString();
            string optionFilter = sender.GetType().GetProperty("OptionFilter").GetValue(sender, null).ToString();

            List<ResultInfo> resultInfos = ExcelStoreManager.Instance.GetResultInfoBySearchId(SearchId);

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
                ShowDebug.Msg(F.FLMD(), "sender is null");
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
                ShowDebug.MsgErr(F.FLMD(), "sender is null");
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
                ShowDebug.MsgErr(F.FLMD(), "sender is null");
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
                ShowDebug.MsgErr(F.FLMD(), "sender is null");
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

            listSearchVm.DelSearchResult(new ShowInfo().SetData(searchInfo));


        }


    }
}
