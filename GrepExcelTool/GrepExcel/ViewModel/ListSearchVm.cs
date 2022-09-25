using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GrepExcel.Excel;
using GrepExcel.View;

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
        private readonly ExcelStoreManager excelStore_ = ExcelStoreManager.Instance;
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

        public int TabCountLoading { get; set; } = 0;
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
        public void ShowTabExits(ShowInfo showInfo)
        {
            //update tab active
            showInfo.Info.IsTabActive = true;
            excelStore_.UpdateSearchInfo(showInfo.Info); //update tabactive 

            //check tab is open
            int indexTab = -1;
            bool isOpen = mainVm_.IsTabOpen(showInfo.Info, ref indexTab);
            if (isOpen)
            {
                mainVm_.ActiveTabWithIndex(indexTab);
            }
            else
            {
                mainVm_.AddTabControl(new SearchResultVm(
                                                        new SearchResultUc(),
                                                        showInfo.Info.Search,
                                                        showInfo.Info.Id,
                                                        showInfo));
            }
        }

        public async Task ShowTab(ShowInfo showInfo, bool reload)
        {

            //update tab active
            showInfo.Info.IsTabActive = true;
            excelStore_.UpdateSearchInfo(showInfo.Info); //update tabactive 

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
                                                    showInfo.Info.Id,
                                                    showInfo);
                tabIndex = mainVm_.AddTabControl(searchResultVm);
            }
            else
            {
                searchResultVm = mainVm_.GetTabContent(mainVm_.TabActive + 1);
                tabIndex = mainVm_.TabActive + 1;
            }


            //add tab failed.
            if (searchResultVm is null || tabIndex == -1)
                return;

            var tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;

            var grep = new Grep();
            searchResultVm.IsLoading = true;
            grep.EventGrepResult += Grep_EventGrepResult;

            TokenStores.Add(new TokenStore(tabIndex, tokenSource, grep));
            await Task.Run(() =>
            {
                grep.GrepAsync(showInfo.Info, tabIndex, ct, new Action<bool>((stopLoading) =>
               {
                   if (stopLoading)
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
            //int percent = e.CurrentFileIndex * 100 / e.TotalFiles; 
            //mainVm_.ShowPercentSearching(percent, e.CurrentMatch);

            //render result 
            var searchResultVm = mainVm_.GetTabContent(e.TabIndex);

            if (searchResultVm != null)
                searchResultVm.AddResult(e.Result);
        }


        private void LoadData()
        {

            var listInfo = excelStore_.GetSearchInfoAll();

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

            if (ct != null)
            {
                ct.Cancel();

                tokenStore.GrepObj.EventGrepResult -= Grep_EventGrepResult;

                TokenStores.Remove(tokenStore);
            }
        }

        public void UpdateTotalMatch(ShowInfo showInfo)
        {
            int totalMatch = excelStore_.CountResultInfoBySearchId(showInfo.Info.Id);
            bool recentsExits = false;

            for (int idx = 0; idx < SearchInfos.Count; idx++)
            {
                if (SearchInfos[idx].Info == showInfo.Info)
                {
                    var temp = SearchInfos[idx];
                    temp.Total = totalMatch;

                    SearchInfos[idx] = temp;
                    recentsExits = true;
                    break;
                }
            }

            //add new if recent not exits
            if (!recentsExits)
            {
                showInfo.Total = totalMatch;
                SearchInfos.Add(showInfo);
            }

        }


        public void DelSearchResult(ShowInfo showInfo)
        {
            var recent = RecentSearchVm.Instance;

            //remove tab if tab opening
            int indexTab = -1;
            if (mainVm_.IsTabOpen(showInfo.Info, ref indexTab))
            {
                mainVm_.RemoveTabControl(indexTab);
            }

            var res = excelStore_.DeleteBySearchId(showInfo.Info);
            if (SqlResult.DeleteSuccess == res)
            {
                log_.Info("Delete search info success");
            }
            else
            {
                log_.Error("Delete search info false");
            }

            mainVm_.UpdateStatusBar();
            SearchInfos.Remove(showInfo);

            //Update Recent list
            recent.LoadRecents();

        }

        public bool IsExits(ShowInfo showInfo)
        {
            for (int idx = 0; idx < SearchInfos.Count; idx++)
            {
                if (SearchInfos[idx].Info == showInfo.Info)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion //Method
    }
}
