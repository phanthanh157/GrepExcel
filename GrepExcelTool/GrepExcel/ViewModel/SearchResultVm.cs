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
        public SearchResultVm()
        {
            ResultInfos = new ObservableCollection<ResultInfo>();
            OptionFilters = new ObservableCollection<OptionFilter>();
            InitClass();
        }

        private void InitClass()
        {
            OptionFilters.Add(new OptionFilter { Color = "Green", Value = "Result" });
            OptionFilters.Add(new OptionFilter { Color = "Red", Value = "FileName" });
            OptionFilters.Add(new OptionFilter { Color = "Blue", Value = "Sheet" });
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

            if(searchInfo == null)
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

            await grep.GrepAsync(searchInfo);

            LoadDataFromDatabase();

            mainVm.NotifyTaskRunning(searchInfo.Search,false);
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
                if(_commandFocusFind == null)
                {
                    _commandFocusFind = new RelayCommand(x => CommandFocusFindHandler(x));

                }
                return _commandFocusFind;
            }
        }

        private void CommandFocusFindHandler(object sender)
        {
            if(sender is null)
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
                if(_copyFullPath == null)
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
    }
}
