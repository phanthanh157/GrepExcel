using GrepExcel.Excel;
using GrepExcel.View;
using GrepExcel.View.Dialog;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace GrepExcel.ViewModel
{

    public class MainViewModel : TabControl
    {
        #region Fileds
        private static MainViewModel _instance = null;
        private ExcelStoreManager _excelStore = null;
        private string _notifyString;
        private bool _isOpenNotify = false;
        private string _isShowStatus = "Collapsed";
        private int _tabActive;
        private Queue _msgNotify;
        private int _totalKeySearch;
        private int _totalResultSearch;
        private int _searchPercent;
        private int _currentResults;
        private ICommand _commandClose;
        private ICommand _commandAboutInfoOpen;
        private ICommand _commandSearchSettings;
        private ICommand _commandTabNext;
        private ICommand _commandTabPrev;
        private ICommand _commandManagerDatabaseOpen;

        #endregion


        public MainViewModel()
        {
            InitClass();
            UpdateStatusBar();

        }

        public void InitClass()
        {
            Tabs = new ObservableCollection<TabControl>();
            _excelStore = ExcelStoreManager.Instance;
            _msgNotify = new Queue();
            _totalKeySearch = 0;
            _totalKeySearch = 0;
        }


        #region Event
        public event EventHandler<int> TabIndexActive;

        private void OnTabIndexActive(int index)
        {
            TabIndexActive?.Invoke(this, index);
        }

        public event EventHandler<object> TabSelectionChange;

        private void OnTabSelectionChange(object o)
        {
            TabSelectionChange?.Invoke(this, o);
        }

        #endregion


        #region Property
        public ObservableCollection<TabControl> Tabs { get; set; }

        public static MainViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MainViewModel();
                }
                return _instance;
            }
        }

        public static MainWindow InstanceMwd
        {
            get
            {
                return (MainWindow)Application.Current.MainWindow;
            }
        }

        public int TabActive
        {
            get
            {
                return _tabActive;
            }
            set
            {
                if (value != _tabActive)
                {
                    _tabActive = value;
                    OnPropertyChanged();
                    OnTabSelectionChange(this);
                }
            }
        }
        public string NotifyString
        {
            get
            {
                return _notifyString;
            }
            set
            {
                if (value != _notifyString)
                {
                    _notifyString = value;
                    OnPropertyChanged();
                }
            }
        }

        public string IsShowStatus
        {
            get
            {
                return _isShowStatus;
            }
            set
            {
                if (value != _isShowStatus)
                {
                    _isShowStatus = value;
                    OnPropertyChanged();
                }
            }
        }
        public bool IsOpenNotify
        {
            get
            {
                return _isOpenNotify;
            }
            set
            {
                if (value != _isOpenNotify)
                {
                    _isOpenNotify = value;
                    OnPropertyChanged();
                }
            }
        }

        public int TotalKeySearch
        {
            get
            {
                return _totalKeySearch;
            }
            set
            {
                if (value != _totalKeySearch)
                {
                    _totalKeySearch = value;
                    OnPropertyChanged();
                }
            }
        }

        public int TotalResultSearch
        {
            get
            {
                return _totalResultSearch;
            }
            set
            {
                if (value != _totalResultSearch)
                {
                    _totalResultSearch = value;
                    OnPropertyChanged();
                }
            }
        }

        public int SearchPercent
        {
            get { return _searchPercent; }
            set
            {
                if(value != _searchPercent)
                {
                    _searchPercent = value;
                    OnPropertyChanged();
                }
            }
        }

        public int CurrentResults
        {
            get { return _currentResults; }
            set
            {
                if (value != _currentResults)
                {
                    _currentResults = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion //Properties

        #region Command
        public ICommand CommandClose
        {
            get
            {
                if (_commandClose == null)
                {
                    _commandClose = new RelayCommand(x => { CommandCloseHandler(); });
                }
                return _commandClose;
            }
        }
        private void CommandCloseHandler()
        {
            OnClose(EventArgs.Empty);
        }


        public ICommand CommandAboutOpen
        {
            get
            {
                if (_commandAboutInfoOpen == null)
                {
                    _commandAboutInfoOpen = new RelayCommand(x => { CommandAboutOpenHandler(); });
                }
                return _commandAboutInfoOpen;
            }
        }

        private void CommandAboutOpenHandler()
        {

            AboutInfo aboutInfo = new AboutInfo();

            aboutInfo.ShowDialog();

        }


        public ICommand CommandSearchSettings
        {
            get
            {
                if (_commandSearchSettings == null)
                {
                    _commandSearchSettings = new RelayCommand(x => { CommandSearchSettingsHandler(); });
                }

                return _commandSearchSettings;
            }
        }

        private void CommandSearchSettingsHandler()
        {

            SearchSettings settings = new SearchSettings();

            settings.cobMaxFiles.Text = Config.ReadSetting("MAX_FILE");
            settings.cobMaxFolders.Text = Config.ReadSetting("MAX_FOLDER");
            settings.cobMaxSearch.Text = Config.ReadSetting("MAX_SEARCH");
            settings.txtNumberRecent.Text = Config.ReadSetting("NUMBER_RECENTS");

            settings.ShowDialog();

        }

        public ICommand CommandTabNext
        {
            get
            {
                if (_commandTabNext == null)
                {
                    _commandTabNext = new RelayCommand(x => CommandTabNextHandler());
                }
                return _commandTabNext;
            }
        }

        private void CommandTabNextHandler()
        {
            if (TabActive < Tabs.Count - 1)
            {
                TabActive = TabActive + 1;
                OnTabIndexActive(TabActive);
            }

        }

        public ICommand CommandTabPrev
        {
            get
            {
                if (_commandTabPrev == null)
                {
                    _commandTabPrev = new RelayCommand(x => CommandTabPrevHandler());
                }
                return _commandTabPrev;
            }
        }

        private void CommandTabPrevHandler()
        {
            if (TabActive > 0)
            {
                TabActive = TabActive - 1;
                OnTabIndexActive(TabActive);
            }
        }


        public ICommand CommandManagerDatabaseOpen
        {
            get
            {
                if (_commandManagerDatabaseOpen == null)
                {
                    _commandManagerDatabaseOpen = new RelayCommand(x => CommandManagerDatabaseOpenHandler());
                }
                return _commandManagerDatabaseOpen;
            }
        }

        private void CommandManagerDatabaseOpenHandler()
        {
            var managerDatabase = new ManagerDatabase();
            var mDbVm = ManagerDatabaseVm.Instance;

            string database = Define.Database;
            string dir = Directory.GetCurrentDirectory();
            string pathDb = Path.Combine(dir, database);

            if (File.Exists(pathDb))
            {
                mDbVm.InitClass();
                managerDatabase.ShowDialog();
            }
            else
            {
                MessageBox.Show("Database not found", "Notification", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }



        #endregion


        #region Method
        public void AddTabControl(TabControl tabControl)
        {
            if (tabControl == null)
            {
                ShowDebug.Msg(F.FLMD(), "tabcontrol is null");
                return;
            }
            ShowDebug.Msg(F.FLMD(), "add new tabcontrol : {0}", tabControl.TabName);

            this.Tabs.Add(tabControl);

            UpdateStatusBar();
        }

        public TabControl SearchTabControl(string tabName)
        {
            if (string.IsNullOrEmpty(tabName))
            {
                ShowDebug.Msg(F.FLMD(), "name is null");
                return null;
            }

            return Tabs.Where(tab => tab.TabName == tabName).FirstOrDefault();
        }

        public void RemoveTabControl(int tabIndex)
        {
            if (Tabs.Count > 0 && Tabs.Count > tabIndex)
            {
                ShowDebug.Msg(F.FLMD(), "remove tabcontrol from list {0}", tabIndex);
                Tabs.RemoveAt(tabIndex);
            }
        }

        public SearchResultVm GetActiveSearchResultVm()
        {
            if (Tabs.Count >= TabActive)
                return (SearchResultVm)Tabs[TabActive];
            return null;
        }

        public SearchResultVm GetSearchResultVm(int tabIndex)
        {
            if (Tabs.Count >= tabIndex)
            {
                return (SearchResultVm)Tabs[tabIndex];
            }
            return null;
        }

        public void NotifyTaskRunning(string taskName, bool isAdd = true)
        {

            if (_msgNotify.Count == 0)
            {
                IsOpenNotify = true;
                IsShowStatus = "Visible";
            }

            if (isAdd)
            {
                _msgNotify.Enqueue(taskName);

                NotifyString =  _msgNotify.Count.ToString();

            }
            else
            {
                if (_msgNotify.Count > 0)
                {
                    _msgNotify.Dequeue();
                    NotifyString = string.Empty;

                    NotifyString = _msgNotify.Count.ToString();

                    if (_msgNotify.Count == 0)
                    {
                        IsOpenNotify = false;
                        IsShowStatus = "Collapsed";
                    }
                }
                else
                {
                    IsOpenNotify = false;
                    IsShowStatus = "Collapsed";
                }
            }

        }

        public void LoadTabControl()
        {
            var excelStore = ExcelStoreManager.Instance;
            var listTabActive = excelStore.GetTabActive(true);

            foreach (var tabActive in listTabActive)
            {
                var results = excelStore.GetResultInfoBySearchId(tabActive.Id);

                SearchResultVm tabControl = new SearchResultVm();
                tabControl.Control = new SearchResultUc();
                tabControl.TabName = tabActive.Search;
                tabControl.SearchId = tabActive.Id;

                results.ForEach(x => tabControl.ResultInfos.Add(x));

                Tabs.Add(tabControl);
            }

            OnTabIndexActive(int.Parse(Config.ReadSetting("TAB_CURRENT_ACTIVE")));
        }


        public void ActionTabIndexActive(int index)
        {
            if (index != -1 && index < Tabs.Count())
                OnTabIndexActive(index);
        }


        public bool isTabOpen(SearchInfo searchInfo)
        {
            if (Tabs.Count == 0)
            {
                ShowDebug.Msg(F.FLMD(), "All TabControl close");
                return false;
            }

            int cnt = 0;
            foreach (var tab in Tabs)
            {
                if (tab is SearchResultVm)
                {
                    var searchVm = tab as SearchResultVm;

                    if (searchInfo.Id == searchVm.SearchId)
                    {
                        return true;
                    }

                }
                cnt++;
            }

            return false;
        }

        public bool isTabOpen(SearchInfo searchInfo, ref int index)
        {
            if (Tabs.Count == 0)
            {
                ShowDebug.Msg(F.FLMD(), "All TabControl close");
                return false;
            }

            int cnt = 0;
            foreach (var tab in Tabs)
            {
                if (tab is SearchResultVm)
                {
                    var searchVm = tab as SearchResultVm;

                    if (searchInfo.Id == searchVm.SearchId)
                    {
                        index = cnt;
                        return true;
                    }

                }
                cnt++;
            }

            return false;
        }




        public void UpdateStatusBar()
        {
            TotalKeySearch = _excelStore.CountSearchInfo();
            TotalResultSearch = _excelStore.CountResultInfo();
        }

        #endregion
    }
}
