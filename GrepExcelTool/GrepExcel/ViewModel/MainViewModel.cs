using GrepExcel.Excel;
using GrepExcel.View;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace GrepExcel.ViewModel
{
    public class MainViewModel : TabControl
    {
        #region Fileds
        private static MainViewModel _instance = null;
        private string _notifyString;
        private bool _isOpenNotify = false;
        private int _tabActive;
        private Queue _msgNotify;
        private ICommand _commandClose;

        #endregion


        public MainViewModel()
        {
            InitClass();
          
        }

        public void InitClass()
        {
            Tabs = new ObservableCollection<TabControl>();
            _msgNotify = new Queue();
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

        #endregion

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
                return (SearchResultVm) Tabs[TabActive];
            return null;
        }

        public void NotifyTaskRunning(string taskName, bool isAdd = true)
        {
      
            if(_msgNotify.Count == 0)
            {
                IsOpenNotify = true;
            }

            if (isAdd)
            {
                _msgNotify.Enqueue(taskName);

                 NotifyString = "Task running (" + _msgNotify.Count + " tasks in queue)";

            }
            else
            {
                if(_msgNotify.Count > 0)
                {
                    _msgNotify.Dequeue();
                    NotifyString = string.Empty;

                    NotifyString = "Task running (" + _msgNotify.Count + " tasks in queue)";

                    if (_msgNotify.Count == 0)
                    {
                        IsOpenNotify = false;
                    }
                }
                else
                {
                    IsOpenNotify = false;
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
            if(index != -1 && index < Tabs.Count())
                OnTabIndexActive(index);
        }


        public bool isTabOpen(SearchInfo searchInfo, ref int index)
        {
            if(Tabs.Count == 0)
            {
                ShowDebug.Msg(F.FLMD(), "All TabControl close");
                return false;
            }

            int cnt = 0;
            foreach(var tab in Tabs)
            {
                if(tab is SearchResultVm)
                {
                    var searchVm = tab as SearchResultVm;

                    if(searchInfo.Id == searchVm.SearchId)
                    {
                        index = cnt;
                        return true;
                    }

                }
                cnt++;
            }

            return false;
        }

        #endregion
    }
}
