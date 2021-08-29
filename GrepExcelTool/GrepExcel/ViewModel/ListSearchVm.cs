using GrepExcel.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrepExcel.ViewModel
{
    public class ListSearchVm
    {


        #region Properties
        public ObservableCollection<SearchInfo> SearchInfos { get; set; }

        public SearchInfo Info { get; set; }

        #endregion
        public ListSearchVm()
        {
            SearchInfos = new ObservableCollection<SearchInfo>();

            LoadData();
        }


        private void LoadData()
        {
            var storeManager = ExcelStoreManager.Instance;

            var listInfo = storeManager.GetSearchInfoAll();

            if(listInfo == null )
            {
                return;
            }

            var filter = listInfo.Where(x => x.IsTabActive == false).ToList();
            filter.ForEach(x => SearchInfos.Add((x)));
        }

    }
}
