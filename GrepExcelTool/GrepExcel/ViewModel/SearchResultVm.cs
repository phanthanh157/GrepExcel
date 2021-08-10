using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GrepExcel.Excel;

namespace GrepExcel.ViewModel
{
  

    public class SearchResultVm : TabControl
    {
        public ObservableCollection<ResultInfo> ResultInfos { get; set; }
   
        private ICommand _commandRefresh;
        public SearchResultVm()
        {
            ResultInfos = new ObservableCollection<ResultInfo>();
        }


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

        private void CommandRefeshHandler()
        {

            var excelStore = ExcelStoreManager.Instance;
            var listResult = excelStore.GetResultInfoBySearchId(base.SearchId);

            //Clear before load again.
            ResultInfos.Clear();
            foreach (var result in listResult)
            {
                ResultInfos.Add(result);
            }
        }
    }
}
