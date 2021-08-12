using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GrepExcel.Excel;
using GrepExcel.Commands;


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

        public int SearchId { get; set; }

        public ResultInfo SelectedItem { get; set; }

        public ICommand CommandRefresh
        {
            get
            {
                if (_commandRefresh == null)
                {
                    _commandRefresh = new RelayCommand((sender) => CommandRefeshHandler());
                }
                return _commandRefresh;
            }
        }

        public void CommandRefeshHandler()
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
            if(sender == null)
            {
                ShowDebug.Msg(F.FLMD(), "sender is null");
                return;
            }
            // var mainVm = MainViewModel.Instance;OptionFilter
            string keySearch = sender.GetType().GetProperty("Search").GetValue(sender, null).ToString();
            string optionFilter = sender.GetType().GetProperty("OptionFilter").GetValue(sender, null).ToString();

            List<ResultInfo> resultInfos = ExcelStoreManager.Instance.GetResultInfoBySearchId(SearchId);

            if(resultInfos != null)
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
                foreach(var item in filter)
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
                    _goToDocument = new AsyncRelayCommand ((sender) => CommandGotoDocumentHander(sender));
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
    }
}
