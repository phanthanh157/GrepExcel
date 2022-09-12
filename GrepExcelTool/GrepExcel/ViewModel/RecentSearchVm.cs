using System;
using System.Collections.ObjectModel;
using System.Linq;
using GrepExcel.Excel;
using GrepExcel.View;

namespace GrepExcel.ViewModel
{
    public struct ShowInfo
    {
        public SearchInfo Info { get; set; }
        public int Total { get; set; }
        public string Type { get; set; }

        public ShowInfo(SearchInfo info, int total, string type)
        {
            Info = info;
            Total = total;
            Type = type;
        }

        public static ShowInfo Create(SearchInfo searchInfo)
        {
            Base.Check(searchInfo);
            return new ShowInfo(
                searchInfo,
                ExcelStoreManager.Instance.CountResultInfoBySearchId(searchInfo.Id),
                SubOption(searchInfo));
        }

        public static string SubOption(SearchInfo searchInfo)
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

        private static string SubOptionMethod(SearchInfo searchInfo)
        {
           return searchInfo.Method == TypeMethod.Folder ? "F" : "S";
        }

        private static string SubOptionTarget(SearchInfo searchInfo)
        {
            if (searchInfo.Target == TypeTarget.Comment)
                return "C";
            else if (searchInfo.Target == TypeTarget.Fomular)
                return "F";
            else
                return "V";
        }

        private static string SubOptionMathCase(SearchInfo searchInfo)
        {
            return searchInfo.IsMatchCase ? "C" : string.Empty;
        }

        private static string SubOptionMathWhole(SearchInfo searchInfo)
        {
            return searchInfo.IsLowerOrUper ? "W" : string.Empty;
        }
    }


    public class RecentSearchVm : BaseModel
    {
        #region Fields
        private static readonly Lazy<RecentSearchVm> lazy_ = new Lazy<RecentSearchVm>(() => new RecentSearchVm());
        private SettingVm settings_ = null;
        private int numberOfRecent_;
        private readonly ExcelStoreManager excelStore_ = ExcelStoreManager.Instance;
        #endregion 


        public RecentSearchVm()
        {
            Recents = new ObservableCollection<ShowInfo>();
            InitClass();
            LoadRecents();
        }

        #region Properties

        public static RecentSearchVm Instance => lazy_.Value;

        public ShowInfo SelectedItem { get; set; }

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
                numberOfRecent_ = int.Parse(Config.ReadSetting("NUMBER_RECENTS"));

                settings_ = SettingVm.Instance;
                settings_.SettingChanged += SettingChange;
            }
            catch
            {
                numberOfRecent_ = 10;
            }
        }

        private void SettingChange(object sender, EventArgs e)
        {
            var settingArgs = e as SettingArgs;
            numberOfRecent_ = settingArgs.NumberRecent;

            LoadRecents();
        }

        public void LoadRecents()
        {
            Base.Check(excelStore_);

            var listInfo = excelStore_.GetSearchInfoAll();
           
            //no data in db
            if (listInfo is null)
                return;
           
            listInfo.Reverse();

            var filter = listInfo.Take(numberOfRecent_)
                                 .OrderByDescending(x => x.Id)
                                 .ToList();
            Recents.Clear();

            foreach (var item in filter)
            {
                Recents.Add(ShowInfo.Create(item));
            }

        }

        public void UpdateTotalMatch(ShowInfo showInfo)
        {
            int totalMatch = excelStore_.CountResultInfoBySearchId(showInfo.Info.Id);
            bool recentsExits = false;

            for(int idx = 0; idx < Recents.Count; idx++)
            {
                if(Recents[idx].Info == showInfo.Info)
                {
                    var temp = Recents[idx];
                    temp.Total = totalMatch;

                    Recents[idx] = temp;
                    recentsExits = true;
                    break;
                }
            }

            //add new if recent not exits
            if (!recentsExits)
            {
                showInfo.Total = totalMatch;
                Recents.Insert(0, showInfo);
            }

        }

        public bool IsExits(ShowInfo showInfo)
        {
            for (int idx = 0; idx < Recents.Count; idx++)
            {
                if (Recents[idx].Info == showInfo.Info)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion //Method

    }
}
