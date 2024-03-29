﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GrepExcel.Excel;

namespace GrepExcel.ViewModel
{
    public struct MethodView
    {
        public string Icon { get; set; }
        public TypeMethod Method { get; set; }
        public string Name { get; set; }
    }

    public struct TargetView
    {
        public string Icon { get; set; }
        public TypeTarget Target { get; set; }
        public string Name { get; set; }
    }

    public class SearchInputVm
    {
        private static readonly log4net.ILog log_ = LogHelper.GetLogger();

        private ICommand cmdSearch_ = null;
        private ICommand cmdDeleteAllSerach_ = null;
        public ObservableCollection<MethodView> Methods { get; set; }
        public ObservableCollection<TargetView> Targets { get; set; }

        public SearchInputVm()
        {
            Methods = new ObservableCollection<MethodView>();
            Targets = new ObservableCollection<TargetView>();
            LoadItem();
        }

        public ICommand CommandDeleteAllSearch
        {
            get
            {
                if(cmdDeleteAllSerach_ is null)
                {
                    cmdDeleteAllSerach_ = new RelayCommand((sender) => CommandDeleteAllSearchHandler(sender));
                }
                return cmdDeleteAllSerach_;
            }
        }

        private void CommandDeleteAllSearchHandler(object sender)
        {
            var managerDb = ManagerDatabaseVm.Instance;

            managerDb.CommandResetDatabase.Execute(managerDb.DirDb);

        }

        public ICommand CommandSearch
        {
            get
            {
                if (cmdSearch_ == null)
                {
                    cmdSearch_ = new Commands.AsyncRelayCommand((sender) => CommandSeachHander(sender), ex => SearchException(ex));
                }
                return cmdSearch_;
            }
        }

        private void SearchException(Exception ex)
        {

            log_.DebugFormat("search exception", ex);

        }

        private async Task CommandSeachHander(object sender)
        {
            if (sender == null)
            {
                log_.Error("Sender is null");
                return;
            }

            var inputInfo = sender as SearchInfo;
            var excelStore = ExcelStoreManager.Instance;
            var listSearchVm = ListSearchVm.Instance;
            var listRecentVm = RecentSearchVm.Instance;

            
            listSearchVm.TabCountLoading += 1;

         
            //check exits database
            SearchInfo searchInfoFirst;
            if (CheckExitsSearchInfo(inputInfo, out searchInfoFirst))
            {
                MessageBox.Show("Search keyword is exits on database", "Searching...", MessageBoxButton.OK, MessageBoxImage.Information);
                var showInfoFirst = ShowInfo.Create(searchInfoFirst);

                listSearchVm.ShowTabExits(showInfoFirst);

                listRecentVm.UpdateTotalMatch(showInfoFirst);
                listSearchVm.TabCountLoading -= 1;
                return;
            }

            // await Task.Delay(1000);
            //mainVm.NotifyTaskRunning(inputInfo.Search);

            //Insert input info to database
            SqlResult sqlResult = excelStore.InsertSearchInfo(inputInfo);
            var showInfo = ShowInfo.Create(inputInfo);

            if (SqlResult.InsertSucess != sqlResult)
            {
                listSearchVm.TabCountLoading -= 1;
                return;
            }

            if(listSearchVm.TabCountLoading > Define.MAX_TAB_OPEN_LOADING)
            {
                MessageBox.Show("Tab open is loading greater than " + Define.MAX_TAB_OPEN_LOADING, "Searching",
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
                listSearchVm.TabCountLoading -= 1;
                return;
            }


            inputInfo.Id = excelStore.LastIndexSearch();// add id 
              
            //Display result when finish search
            await listSearchVm.ShowTab(showInfo, false);

            //add observer list serach
            listSearchVm.UpdateTotalMatch(showInfo);

            //add first list recent
            listRecentVm.UpdateTotalMatch(showInfo);

            listSearchVm.TabCountLoading -= 1;
            //mainVm.NotifyTaskRunning(inputInfo.Search, false);
        }


        private bool CheckExitsSearchInfo(SearchInfo searchInfo, out SearchInfo outSearchInfo)
        {
            outSearchInfo = null;

            var excelStore = ExcelStoreManager.Instance;

            var dataSearch = excelStore.GetSearchInfoAll();

            if (dataSearch is null)
                return false;

            var filter = dataSearch.Where(res => res == searchInfo);

            if (filter.Count() > 0)
            {
                //searchId = filter.First().Id;
                outSearchInfo = filter.First();
                return true;
            }
            return false;
        }


        public void LoadItem()
        {
            Methods.Add(new MethodView() { Icon = "Folder", Method = TypeMethod.Folder, Name = "Folder" });
            Methods.Add(new MethodView() { Icon = "FolderMultiple", Method = TypeMethod.SubFolder, Name = "SubFolder" });

            Targets.Add(new TargetView() { Icon = "CurrencyUsd", Target = TypeTarget.Value, Name = "Value" });
            Targets.Add(new TargetView() { Icon = "Comment", Target = TypeTarget.Comment, Name = "Comment" });
            Targets.Add(new TargetView() { Icon = "Function", Target = TypeTarget.Fomular, Name = "Fomular" });
        }
    }
}
