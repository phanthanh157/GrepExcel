using GrepExcel.Excel;
using GrepExcel.View;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GrepExcel.ViewModel
{
    public struct ShowInfo
    {
        public SearchInfo Info { get; set; }
        public int Total { get; set; }

        public string Type { get; set; }

        public ShowInfo SetData(SearchInfo searchInfo)
        {
            ShowInfo showInfo = new ShowInfo();

            if (searchInfo == null) return showInfo;
            showInfo.Info = searchInfo;
            showInfo.Total = ExcelStoreManager.Instance.CountResultInfoBySearchId(searchInfo.Id);
            showInfo.Type = SubOption(searchInfo);
            return showInfo;
        }

        public string SubOption(SearchInfo searchInfo)
        {
            string res = string.Empty;
            // Mehod/Target/MatchCase/MatchWhole
            res = SubOptionMethod(searchInfo)
                   + '/' + SubOptionTarget(searchInfo);
            if (!string.IsNullOrEmpty(SubOptionMathCase(searchInfo)))
            {
                res += '/' + SubOptionMathCase(searchInfo);
            }
            if (!string.IsNullOrEmpty(SubOptionMathWhole(searchInfo)))
            {
                res += '/' + SubOptionMathWhole(searchInfo);
            }

            return res;
        }

        private string SubOptionMethod(SearchInfo searchInfo)
        {
            string res = string.Empty;
            if (searchInfo.Method == TypeMethod.Folder)
            {
                res = "F";
            }
            else
            {
                res = "S";
            }
            return res;
        }

        private string SubOptionTarget(SearchInfo searchInfo)
        {
            string res = string.Empty;
            if (searchInfo.Target == TypeTarget.Comment)
            {
                res = "C";
            }
            else if (searchInfo.Target == TypeTarget.Fomular)
            {
                res = "F";
            }
            else
            {
                res = "V";
            }
            return res;
        }

        private string SubOptionMathCase(SearchInfo searchInfo)
        {
            string res = string.Empty;
            if (searchInfo.IsMatchCase == true)
            {
                res = "C";
            }
            return res;
        }

        private string SubOptionMathWhole(SearchInfo searchInfo)
        {
            string res = string.Empty;
            if (searchInfo.IsLowerOrUper == true)
            {
                res = "W";
            }
            return res;
        }


    }


    public class RecentSearchVm : BaseModel
    {
        #region Fields
        private static RecentSearchVm _instance = null;
        // private ObservableCollection<ShowInfo> _recents;
        private SettingVm _settings = null;
        private int _numberOfRecents;

        #endregion 


        public RecentSearchVm()
        {
            Recents = new ObservableCollection<ShowInfo>();
            InitClass();
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

        public ObservableCollection<ShowInfo> Recents
        {
            get; set;
        }

        #endregion //Properties


        #region Method

        public void InitClass()
        {
            try
            {
                _numberOfRecents = int.Parse(Config.ReadSetting("NUMBER_RECENTS"));

                _settings = SettingVm.Instance;
                _settings.SettingChanged += SettingChange;
            }
            catch
            {
                _numberOfRecents = 10;
            }
        }

        private void SettingChange(object sender, EventArgs e)
        {
            var settingArgs = e as SettingArgs;
            _numberOfRecents = settingArgs.NumberRecent;

            LoadRecents();
        }

        public void LoadRecents()
        {
            var storeManager = ExcelStoreManager.Instance;

            var listInfo = storeManager.GetSearchInfoAll();

            if (listInfo == null)
            {
                return;
            }

            listInfo.Reverse();
            var filter = listInfo.Take(_numberOfRecents)
                                 .OrderByDescending(x => x.Id)
                                 .ToList();
            Recents.Clear();

            foreach (var item in filter)
            {
                Recents.Add(new ShowInfo().SetData(item));
            }

        }
        #endregion //Method

    }
}
