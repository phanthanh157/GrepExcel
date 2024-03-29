﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using GrepExcel.Excel;
using GrepExcel.View;
using GrepExcel.View.Dialog;

namespace GrepExcel.ViewModel
{

    public class MainViewModel : BaseModel
    {
        #region Fileds
        private static readonly log4net.ILog log_ = LogHelper.GetLogger();
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

            MenuShowHideCollumns = new ObservableCollection<MenuItemModel>();
            var columnsStr = Config.ReadSetting("COLUMNS_HIDE");
            foreach (string s in columnsStr.Split(','))
            {
                var item = s.Split(':');

                int column = int.Parse(item[1]);
                bool isShow = (int.Parse(item[2]) == 1) ? true : false;

                MenuItemModel menuItemModel = new MenuItemModel() { Header = item[0], Column = column, IsShow = isShow };
                MenuShowHideCollumns.Add(menuItemModel);
            }

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
        public ObservableCollection<MenuItemModel> MenuShowHideCollumns { get; set; }

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
                if (value != _searchPercent)
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


        private string GetColumnHideToString()
        {
            List<int> columnHide = new List<int>();
            for (int i = 0; i < MenuShowHideCollumns.Count; i++)
            {
                if (MenuShowHideCollumns[i].IsShow == false)
                {
                    columnHide.Add(MenuShowHideCollumns[i].Column);
                }
            }

            return string.Join(",", columnHide);
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
        public int AddTabControl(TabControl tabControl)
        {
            if (tabControl is null)
                return -1;

            log_.InfoFormat("Add new tabcontrol : {0}", tabControl.TabName);

            this.Tabs.Add(tabControl);

            UpdateStatusBar();

            return Tabs.Count;
        }

        public TabControl SearchTabControl(string tabName)
        {
            if (string.IsNullOrEmpty(tabName))
            {
                log_.Warn("name is null");
                return null;
            }

            return Tabs.Where(tab => tab.TabName == tabName).FirstOrDefault();
        }

        public void RemoveTabControl(int tabIndex)
        {
            if (Tabs.Count > 0 && Tabs.Count > tabIndex)
            {
                log_.InfoFormat("remove tabcontrol from list {0}", tabIndex);
                Tabs.RemoveAt(tabIndex);
            }
        }

        public SearchResultVm GetActiveSearchResultVm()
        {
            if (Tabs.Count >= TabActive)
                return (SearchResultVm)Tabs[TabActive];
            return null;
        }

        public SearchResultVm GetTabContent(int tabIndex)
        {
            if (Tabs.Count >= tabIndex && tabIndex > 0)
            {
                return (SearchResultVm)Tabs[tabIndex - 1];
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

                NotifyString = _msgNotify.Count.ToString();

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
                var showInfo = ShowInfo.Create(tabActive);

                var tabControl = new SearchResultVm(
                 new SearchResultUc(),
                 tabActive.Search,
                 tabActive.Id,
                 showInfo);

                tabControl.ColumnNumbers = GetColumnHideToString();

                results.ForEach(x => tabControl.ResultInfos.Add(x));

                Tabs.Add(tabControl);
            }

            OnTabIndexActive(int.Parse(Config.ReadSetting("TAB_CURRENT_ACTIVE")));
        }


        public void ActiveTabWithIndex(int index)
        {
            if (index != -1 && index < Tabs.Count())
                OnTabIndexActive(index);
        }


        public bool isTabOpen(SearchInfo searchInfo)
        {
            if (Tabs.Count == 0)
            {
                log_.Info( "All TabControl close");
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

        public bool IsTabOpen(SearchInfo searchInfo, ref int index)
        {
            if (Tabs.Count == 0)
                return false;

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

        public void ShowPercentSearching(int percent, int match)
        {
            //log_.InfoFormat("grep percent: {0}", percent);

            this.SearchPercent = percent;
            this.CurrentResults = match;
        }

        public void UpdateShowHideColumnSearch()
        {
            foreach (var tab in Tabs)
            {
                if (tab is SearchResultVm)
                {
                    var searchVm = tab as SearchResultVm;

                    searchVm.ColumnNumbers = GetColumnHideToString();
                }
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < MenuShowHideCollumns.Count; i++)
            {
                string header = MenuShowHideCollumns[i].Header;
                int column = MenuShowHideCollumns[i].Column;
                int isShow = MenuShowHideCollumns[i].IsShow ? 1 : 0;

                if (i < MenuShowHideCollumns.Count - 1)
                {
                    sb.Append(header).Append(":").Append(column).Append(":").Append(isShow).Append(",");
                }
                else
                {
                    sb.Append(header).Append(":").Append(column).Append(":").Append(isShow);
                }
            }

            Config.AddUpdateAppSettings("COLUMNS_HIDE", sb.ToString());
        }

        #endregion
    }
}
