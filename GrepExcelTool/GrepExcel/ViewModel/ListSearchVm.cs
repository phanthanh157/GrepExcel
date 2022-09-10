using GrepExcel.Excel;
using GrepExcel.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GrepExcel.ViewModel
{
    public struct TokenStore
    {
        public int TabIndex { get; set; }
        public CancellationTokenSource TokenSource { get; set; }
        public Grep GrepObj { get; set; }
        public TokenStore(int tabIndex, CancellationTokenSource token, Grep grep)
        {
            TabIndex = tabIndex;
            TokenSource = token;
            GrepObj = grep;
        }
    }
    public class ListSearchVm : BaseModel
    {
        #region Fields
        private static readonly log4net.ILog log_ = LogHelper.GetLogger();
        private static readonly Lazy<ListSearchVm> lazy_ = new Lazy<ListSearchVm>(() => new ListSearchVm());
        private ObservableCollection<ShowInfo> searchInfos_ = new ObservableCollection<ShowInfo>();
        private readonly MainViewModel mainVm_ = MainViewModel.Instance;
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
        public List<TokenStore> TokenStores { get; private set; }

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

            TokenStores = new List<TokenStore>();
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
        public void ShowTabSync(ShowInfo showInfo)
        {
            var mainVm = MainViewModel.Instance;
            var excelStore = ExcelStoreManager.Instance;

            //update tab active
            showInfo.Info.IsTabActive = true;
            excelStore.UpdateSearchInfo(showInfo.Info); //update tabactive 

            //check tab is open
            int indexTab = -1;
            bool isOpen = mainVm.IsTabOpen(showInfo.Info, ref indexTab);
            if (isOpen)
            {
                mainVm.ActiveTabWithIndex(indexTab);
            }
            else
            {
                mainVm.AddTabControl(new SearchResultVm(
                                                        new SearchResultUc(),
                                                        showInfo.Info.Search,
                                                        showInfo.Info.Id));
            }
        }

        public async Task ShowTab(ShowInfo showInfo, bool reload)
        {
            var excelStore = ExcelStoreManager.Instance;

            //update tab active
            showInfo.Info.IsTabActive = true;
            excelStore.UpdateSearchInfo(showInfo.Info); //update tabactive 

            //check tab is open
            int indexTab = -1;
            bool isOpen = mainVm_.IsTabOpen(showInfo.Info, ref indexTab);
            if (isOpen && !reload)
            {
                mainVm_.ActiveTabWithIndex(indexTab);
                return;
            }

            SearchResultVm searchResultVm;
            int tabIndex;

            if (!reload)
            {
                searchResultVm = new SearchResultVm(
                                                    new SearchResultUc(),
                                                    showInfo.Info.Search,
                                                    showInfo.Info.Id);
                tabIndex = mainVm_.AddTabControl(searchResultVm);
            }
            else
            {
                searchResultVm = mainVm_.GetTabContent(mainVm_.TabActive + 1);
                tabIndex = mainVm_.TabActive + 1;
            }
                                                
                                           
            //add tab failed.
            if (tabIndex == -1)
                return;

            var tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;

            var grep = new Grep();
            searchResultVm.IsLoading = true;
            grep.EventGrepResult += Grep_EventGrepResult;

            TokenStores.Add(new TokenStore(tabIndex, tokenSource, grep));
            await Task.Run(() =>
            {              
                grep.GrepAsync(showInfo.Info, tabIndex, ct, new Action<bool> ((stopLoading) =>
                {
                    if(stopLoading)
                        searchResultVm.IsLoading = false;
                }));
            });

            grep.EventGrepResult -= Grep_EventGrepResult;
        }

   
        private void Grep_EventGrepResult(object sender, GrepInfoArgs e)
        {
            if (e is null)
                return;
            //update status percent and match count
            int percent = e.CurrentFileIndex * 100 / e.TotalFiles; 
            mainVm_.ShowPercentSearching(percent, e.CurrentMatch);

            //render result 
            var searchResultVm = mainVm_.GetTabContent(e.TabIndex);

            if(searchResultVm != null)
                searchResultVm.AddResult(e.Result);
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

        public void StopSearching(int tabIndex)
        {
            TokenStore tokenStore = TokenStores.Where(x => x.TabIndex == tabIndex).FirstOrDefault();

            CancellationTokenSource ct = tokenStore.TokenSource;

            if(ct != null)
            {
                ct.Cancel();

                tokenStore.GrepObj.EventGrepResult -= Grep_EventGrepResult;

                TokenStores.Remove(tokenStore);

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
