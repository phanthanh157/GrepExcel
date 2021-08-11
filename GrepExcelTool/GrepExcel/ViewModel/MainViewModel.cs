using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GrepExcel.Excel;
using GrepExcel.View;

namespace GrepExcel.ViewModel
{
    public class MainViewModel : TabControl
    {
        private static MainViewModel _instance = null;
        private ICommand _commandClose;
        public ObservableCollection<TabControl> Tabs { get; set; }
        public event EventHandler<int> TabIndexActive;
        public MainViewModel()
        {
            InitClass();


        }

        public void InitClass()
        {
            Tabs = new ObservableCollection<TabControl>();


        }

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

        public int TabActive { get; set; }

        private void CommandCloseHandler()
        {
            OnClose(EventArgs.Empty);
        }


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

        private void OnTabIndexActive(int index)
        {
            TabIndexActive?.Invoke(this, index);
        }

        public void LoadTabControl()
        {
            var excelStore = ExcelStoreManager.Instance;
            var listTabActive = excelStore.GetTabActive(true);

            foreach(var tabActive in listTabActive)
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
    }
}
