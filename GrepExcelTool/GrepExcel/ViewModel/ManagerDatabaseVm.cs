using GrepExcel.Excel;
using GrepExcel.View;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace GrepExcel.ViewModel
{
    public class ManagerDatabaseVm : BaseModel
    {
        private static ManagerDatabaseVm _instance = null;
        private ICommand _commandResetDatabase;
        private string _sizeDb;
        private string _dirDb;

        public ManagerDatabaseVm()
        {
            InitClass();
        }

        public void InitClass()
        {
            string database = Define.Database;
            string dir = Directory.GetCurrentDirectory();

            string pathDb = Path.Combine(dir, database);

            if (File.Exists(pathDb))
            {
                FileInfo fileInfo = new FileInfo(pathDb);
                long filesize = fileInfo.Length / 2024;

                DirDb = pathDb;
                SizeDb = filesize.ToString() + " (Kb)";
            }
        }

        public static ManagerDatabaseVm Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ManagerDatabaseVm();
                }
                return _instance;
            }
        }

        public string DirDb
        {
            get
            {
                return _dirDb;
            }
            set
            {
                if (value != _dirDb)
                {
                    _dirDb = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SizeDb
        {
            get
            {
                return _sizeDb;
            }
            set
            {
                if (value != _sizeDb)
                {
                    _sizeDb = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand CommandResetDatabase
        {
            get
            {
                if (_commandResetDatabase == null)
                {
                    _commandResetDatabase = new RelayCommand(x => CommandResetDatabaseHandler(x));
                }
                return _commandResetDatabase;
            }
        }

        private void CommandResetDatabaseHandler(object sender)
        {
            if (sender == null)
                return;

            var isReset = MessageBox.Show("Do you want to reset database ?", "Reset Database", MessageBoxButton.YesNo);

            if (isReset == MessageBoxResult.No)
            {
                return;
            }

            string pathFile = (string)sender;

            if (File.Exists(pathFile))
            {
                var excelStore = ExcelStoreManager.Instance;
                var listSearch = ListSearchVm.Instance;
                var listRecent = RecentSearchVm.Instance;
                var mainVm = MainViewModel.Instance;

                //if (SqlResult.DeleteTableSuccess != excelStore.DropTable())
                //{
                //    MessageBox.Show("Database reset failed", "Notification", MessageBoxButton.OK, MessageBoxImage.Warning);
                //    return;
                //}
                File.Delete(pathFile);


                if (SqlResult.CreateTableSuccess != excelStore.CreateTable())
                {
                    MessageBox.Show("Database reset failed", "Notification", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                mainVm.Tabs.Clear();
                listSearch.SearchInfos.Clear();
                listRecent.Recents.Clear();

                InitClass();
                MessageBox.Show("Database reset successfull", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

    }
}
