using GrepExcel.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfControls.Editors;

namespace GrepExcel.ViewModel.Search
{
    public class FolderSuggestionProvider : ISuggestionProvider
    {
        public IEnumerable GetSuggestions(string filter)
        {
            var excelStore = ExcelStoreManager.Instance;

            var lstSearchInfo = excelStore.GetSearchInfoBySearch(filter,2);

            return lstSearchInfo;
        }
    }
}
