using GrepExcel.Excel;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Data.Sqlite;
using GrepExcel.View;
using System;

namespace GrepExcel.ViewModel
{
    public class ListSearchVm : BaseModel
    {
        #region Fields
        private static ListSearchVm _instance = null;
        private ObservableCollection<SearchInfo> _searchInfos;

        #endregion 

        #region Properties
        public ObservableCollection<SearchInfo> SearchInfos {
            get
            {
                return _searchInfos;
            }
            set
            {
                if(value != _searchInfos)
                {
                    _searchInfos = value;
                }
                OnPropertyChanged();
            }
        }

        public SearchInfo Info { get; set; }

        #endregion //Properties


        public ListSearchVm()
        {
            SearchInfos = new ObservableCollection<SearchInfo>();

            LoadData();
        }

        public static ListSearchVm Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new ListSearchVm();
                }
                return _instance;
            }
        }


        #region Method

        public void ShowTabSearchResult(SearchInfo searchInfo)
        {
            if(searchInfo == null)
            {
                ShowDebug.MsgErr(F.FLMD(), "Search info is null");
                return;
            }
            var mainVm = MainViewModel.Instance;
            var excelStore = ExcelStoreManager.Instance;

            //update tabactive
            searchInfo.IsTabActive = true;
            excelStore.UpdateSearchInfo(searchInfo);

            //check tab is open
            int indexTab = -1;
            bool isTabOpen = mainVm.isTabOpen(searchInfo,ref indexTab);
            if(isTabOpen == true)
            {
                mainVm.ActionTabIndexActive(indexTab);
                return;
            }

            //Display result add new tab
            SearchResultVm tabResult = new SearchResultVm();
            tabResult.Control = new SearchResultUc();
            tabResult.TabName = searchInfo.Search;
            tabResult.SearchId = searchInfo.Id;
            tabResult.CommandRefeshHandler(); //load du lieu tu database

            mainVm.AddTabControl(tabResult);

        }

        private void LoadData()
        {
            var storeManager = ExcelStoreManager.Instance;

            var listInfo = storeManager.GetSearchInfoAll();

            if (listInfo == null)
            {
                return;
            }

            // var filter = listInfo.Where(x => x.IsTabActive == false).ToList();
            listInfo.ForEach(x => SearchInfos.Add((x)));
        }


        public void DelSearchResult(SearchInfo searchInfo)
        {
            if (searchInfo == null)
            {
                ShowDebug.MsgErr(F.FLMD(), "Search info is null");
                return;
            }
            var mainVm = MainViewModel.Instance;
            var excelStore = ExcelStoreManager.Instance;

            //remove tab if tab opening
            int indexTab = -1;
            if (mainVm.isTabOpen(searchInfo,ref indexTab))
            {
                mainVm.RemoveTabControl(indexTab);
            }

            var res =  excelStore.DeleteBySearchId(searchInfo);
            if(SqlResult.DeleteSuccess == res)
            {
                ShowDebug.MsgErr(F.FLMD(), "Delete search info success");
            }
            else
            {
                ShowDebug.MsgErr(F.FLMD(), "Delete search info false");
            }

            //update list collection
            // RemoveList(searchInfo.Id);
            SearchInfos.Remove(searchInfo);
   
        }


        private void RemoveList(int id)
        {
            int cnt = 0;
            int idxDelete = 0;
            foreach(var item in SearchInfos)
            {
                if(item.Id == id)
                {
                    idxDelete = cnt;
                    break;
                }
                cnt++;
            }
            ShowDebug.MsgErr(F.FLMD(), "Collection RemoveAt = {0}, CountBefore = {1}", idxDelete, SearchInfos.Count);

            SearchInfos.RemoveAt(idxDelete);
            ShowDebug.MsgErr(F.FLMD(), "Collection RemoveAt = {0}, CountAfter = {1}", idxDelete, SearchInfos.Count);
        }
        #endregion //Method
    }
}
