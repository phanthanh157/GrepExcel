using GrepExcel.Excel;
using GrepExcel.View;
using System;
using System.Collections.ObjectModel;

namespace GrepExcel.ViewModel
{
    public class ListSearchVm : BaseModel
    {
        #region Fields
        private static ListSearchVm _instance = null;
        private ObservableCollection<ShowInfo> _searchInfos;
        private SettingVm _settings = null;

        #endregion 

        #region Properties
        public ObservableCollection<ShowInfo> SearchInfos
        {
            get
            {
                return _searchInfos;
            }
            set
            {
                if (value != _searchInfos)
                {
                    _searchInfos = value;
                }
                OnPropertyChanged();
            }
        }

        public ShowInfo Info { get; set; }

        #endregion //Properties


        public ListSearchVm()
        {
            SearchInfos = new ObservableCollection<ShowInfo>();

            InitClass();

            LoadData();
        }

        private void InitClass()
        {
            _settings = SettingVm.Instance;

            _settings.SettingChanged += SettingChange;

        }

        private void SettingChange(object sender, EventArgs e)
        {
           

        }

        public static ListSearchVm Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ListSearchVm();
                }
                return _instance;
            }
        }


        #region Method

        public void ShowTabSearchResult(ShowInfo showInfo)
        {
            //if (showInfo == null)
            //{
            //    ShowDebug.MsgErr(F.FLMD(), "Search info is null");
            //    return;
            //}
            var mainVm = MainViewModel.Instance;
            var excelStore = ExcelStoreManager.Instance;

            //update tabactive
            showInfo.Info.IsTabActive = true;
            excelStore.UpdateSearchInfo(showInfo.Info);

            excelStore.GetResultInfoBySearchId(showInfo.Info.Id);

            //check tab is open
            int indexTab = -1;
            bool isTabOpen = mainVm.isTabOpen(showInfo.Info, ref indexTab);
            if (isTabOpen == true)
            {
                //Check data change and load again data if change
                var listResult = excelStore.GetResultInfoBySearchId(showInfo.Info.Id);
                if(listResult != null)
                {
                    var resultVm = mainVm.GetSearchResultVm(indexTab);
                    if (resultVm != null)
                    {
                        if (listResult.Count > resultVm.ResultInfos.Count)
                        {
                            resultVm.LoadDataFromDatabase(); 
                        }
                    }
                }

                mainVm.ActionTabIndexActive(indexTab);
                return;
            }

            //Display result add new tab
            SearchResultVm tabResult = new SearchResultVm();
            tabResult.Control = new SearchResultUc();
            tabResult.TabName = showInfo.Info.Search;
            tabResult.SearchId = showInfo.Info.Id;
            tabResult.LoadDataFromDatabase(); //load du lieu tu database

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

            foreach(var item in listInfo)
            {
                SearchInfos.Add(new ShowInfo().SetData(item));
            }
        }


        public void DelSearchResult(ShowInfo showInfo)
        {
            //if (showInfo == null)
            //{
            //    ShowDebug.MsgErr(F.FLMD(), "Search info is null");
            //    return;
            //}
            var mainVm = MainViewModel.Instance;
            var excelStore = ExcelStoreManager.Instance;
            var recent = RecentSearchVm.Instance;

            //remove tab if tab opening
            int indexTab = -1;
            if (mainVm.isTabOpen(showInfo.Info, ref indexTab))
            {
                mainVm.RemoveTabControl(indexTab);
            }

            var res = excelStore.DeleteBySearchId(showInfo.Info);
            if (SqlResult.DeleteSuccess == res)
            {
                ShowDebug.MsgErr(F.FLMD(), "Delete search info success");
            }
            else
            {
                ShowDebug.MsgErr(F.FLMD(), "Delete search info false");
            }

            //update list collection
            // RemoveList(searchInfo.Id);
            mainVm.UpdateStatusBar();
            SearchInfos.Remove(showInfo);

            //Update Recent list
            recent.LoadRecents();

        }


        private void RemoveList(int id)
        {
            int cnt = 0;
            int idxDelete = 0;
            foreach (var item in SearchInfos)
            {
                if (item.Info.Id == id)
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
