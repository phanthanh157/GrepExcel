using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfControls.Editors;
using GrepExcel.Excel;

namespace GrepExcel.ViewModel.Search
{
    public class SearchSuggestionProvider : ISuggestionProvider
    {
        public IEnumerable GetSuggestions(string filter)
        {
            var excelStore = ExcelStoreManager.Instance;

            var lstSearchInfo =  excelStore.GetSearchInfoBySearch(filter);

            return lstSearchInfo;
        }
    }
}
