using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GrepExcel.ViewModel
{
    public class MainViewModel : TabControl
    {
        private static MainViewModel _instance = null;
        private ICommand _commandClose;
        private ObservableCollection<TabControl> Tabs { get; set; }

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

        private void CommandCloseHandler()
        {
            OnClose(EventArgs.Empty);
        }


        public void AddTabControl(string name, UserControl userControl)
        {
            if(string.IsNullOrEmpty(name))
            {
                ShowDebug.Msg(F.FLMD(), "name is null");
                return;
            }

            if (userControl == null)
            {
                ShowDebug.Msg(F.FLMD(), "usercontrol is null");
                return;
            }

            TabControl tabControl = new TabControl()
            {
                TabName = name,
                Control = userControl
            };

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
            if(Tabs.Count >0 && Tabs.Count > tabIndex)
            {
                ShowDebug.Msg(F.FLMD(), "remove tabcontrol from list {0}",tabIndex);
                Tabs.RemoveAt(tabIndex);
            }
        }


    }
}
