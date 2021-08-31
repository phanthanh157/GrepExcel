using GrepExcel.Excel;
using GrepExcel.View;
using System.Collections.ObjectModel;
using System.Linq;

namespace GrepExcel.ViewModel
{
    public class RecentSearchVm : BaseModel
    {
        #region Fields
        private static RecentSearchVm _instance = null;
        private ObservableCollection<SearchInfo> _recents;
        private readonly int _numberOfRecents = 10;

        #endregion 


        public RecentSearchVm()
        {
            _recents = new ObservableCollection<SearchInfo>();

            LoadRecents();
        }


        #region Properties

        public static RecentSearchVm Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RecentSearchVm();
                }
                return _instance;
            }
        }

        public ObservableCollection<SearchInfo> Recents
        {
            get
            {
                return _recents;
            }
            set
            {
                if (value != _recents)
                {
                    _recents = value;
                }
                OnPropertyChanged();
            }
        }

        #endregion //Properties


        #region Method
        public void LoadRecents()
        {
            var storeManager = ExcelStoreManager.Instance;

            var listInfo = storeManager.GetSearchInfoAll();

            if (listInfo == null)
            {
                return;
            }

            listInfo.Reverse();
            var filter = listInfo.Where(x => x.IsTabActive == false)
                                 .Take(_numberOfRecents)
                                 .OrderByDescending(x => x.Id)
                                 .ToList();
            Recents.Clear();
            filter.ForEach(x => Recents.Add((x)));

        }
        #endregion //Method

    }
}
