using GrepExcel.Excel;
using GrepExcel.View;
using System;
using System.Collections.ObjectModel;

namespace GrepExcel.ViewModel
{
    public class ListSearchVm : BaseModel
    {
        #region Fields
        private static readonly Lazy<ListSearchVm> lazy_ = new Lazy<ListSearchVm>(() => new ListSearchVm());
        private ObservableCollection<ShowInfo> searchInfos_ = new ObservableCollection<ShowInfo>();
        private SettingVm settings_ = null;
        #endregion 

        #region Properties
        public ObservableCollection<ShowInfo> SearchInfos
        {
            get
            {
                return searchInfos_;
            }
            set
            {
                if (value != searchInfos_)
                {
                    searchInfos_ = value;
                }
                OnPropertyChanged();
            }
        }

        public ShowInfo Info { get; set; }

        #endregion //Properties


        private ListSearchVm()
        {
            InitClass();

            LoadData();
        }

        private void InitClass()
        {
            settings_ = SettingVm.Instance;

            settings_.SettingChanged += SettingChange;

        }

        private void SettingChange(object sender, EventArgs e)
        {


        }

        public static ListSearchVm Instance
        {
            get
            {
                return lazy_.Value;
            }
        }


        #region Method

        public void ShowTab(ShowInfo showInfo)
        {
            var mainVm = MainViewModel.Instance;
            var excelStore = ExcelStoreManager.Instance;

            //update tabactive
            showInfo.Info.IsTabActive = true;
            excelStore.UpdateSearchInfo(showInfo.Info); //update tabactive 

            //check tab is open
            int indexTab = -1;
            bool isOpen = mainVm.IsTabOpen(showInfo.Info, ref indexTab);
            if (isOpen)
            {
                //Check data change and load again data if change
                var listResult = excelStore.GetResultInfoBySearchId(showInfo.Info.Id);
                if (listResult != null)
                {
                    var resultVm = mainVm.GetTabContent(indexTab);
                    if (resultVm != null)
                    {
                        if (listResult.Count > resultVm.ResultInfos.Count)
                        {
                            resultVm.LoadDataFromDatabase();
                        }
                    }
                }

                mainVm.ActionTabIndexActive(indexTab);
            }
            else
            {
                //Display result add new tab
                var tabResult = new SearchResultVm(
                    new SearchResultUc(),
                    showInfo.Info.Search,
                    showInfo.Info.Id);

                tabResult.LoadDataFromDatabase(); //load du lieu tu database

                mainVm.AddTabControl(tabResult);
            }
        }

        private void LoadData()
        {
            var storeManager = ExcelStoreManager.Instance;

            var listInfo = storeManager.GetSearchInfoAll();

            if (listInfo == null)
            {
                return;
            }

            foreach (var item in listInfo)
            {
                SearchInfos.Add(ShowInfo.Create(item));
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
            if (mainVm.IsTabOpen(showInfo.Info, ref indexTab))
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
