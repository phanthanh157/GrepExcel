using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GrepExcel.View;
using GrepExcel.Excel;
using System.Collections.ObjectModel;
using GrepExcel.Commands;
using System.Windows.Threading;

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
        private ICommand _commandSearch = null;

        public ObservableCollection<MethodView> Methods { get; set; }
        public ObservableCollection<TargetView> Targets { get; set; }

        public SearchInputVm()
        {
            Methods = new ObservableCollection<MethodView>();
            Targets = new ObservableCollection<TargetView>();
            LoadItem();
        }


        public ICommand CommandSearch
        {
            get
            {
                if(_commandSearch == null)
                {
                    _commandSearch = new AsyncRelayCommand(sender => CommandSeachHander(sender));
                }
                return _commandSearch;
            }
        }

        private async Task CommandSeachHander(object sender)
        {
            ShowDebug.Msg(F.FLMD(), "Handler");
            if(sender == null)
            {
                ShowDebug.Msg(F.FLMD(), "Sender is null");
                return;
            }
            var inputInfo = sender as SearchInfo;
            var mainVm = MainViewModel.Instance;
            var excelStore = ExcelStoreManager.Instance;

            // await Task.Delay(1000);
            mainVm.NotifyTaskRunning(inputInfo.Search);

            //Insert input info to database
            SqlResult sqlResult =  excelStore.InsertSearchInfo(inputInfo);
            if(SqlResult.InsertSucess == sqlResult)
            {
                ShowDebug.Msg(F.FLMD(), "Insert Search info success");
                inputInfo.Id = excelStore.LastIndexSearch();// add id 
                //Search process
                var grep = new Grep();
                //grep.GrepSpeedNonTask(inputInfo);
                await grep.GrepAsync(inputInfo);

                //Display result
                SearchResultVm tabResult = new SearchResultVm();
                tabResult.Control = new SearchResultUc();
                tabResult.TabName = inputInfo.Search;
                tabResult.SearchId = inputInfo.Id;
                tabResult.CommandRefeshHandler(); //load du lieu tu database

                mainVm.AddTabControl(tabResult);

                //mainVm.IsOpenNotify = true;
                //mainVm.NotifyString = inputInfo.Search;
                //DispatcherTimer time = new DispatcherTimer();
                //time.Interval = TimeSpan.FromSeconds(10);
                //time.Start();
                //time.Tick += delegate
                //{
                //    mainVm.IsOpenNotify = false;
                //    time.Stop();
                //};
                mainVm.NotifyTaskRunning(inputInfo.Search,false);
            }
        }

        public void LoadItem()
        {
            Methods.Add(new MethodView() { Icon = "Folder", Method = TypeMethod.Folder,Name = "Folder" });
            Methods.Add(new MethodView() { Icon = "FolderMultiple", Method = TypeMethod.SubFolder, Name = "SubFolder" });

            Targets.Add(new TargetView() { Icon = "CurrencyUsd", Target = TypeTarget.Value, Name = "Value" });
            Targets.Add(new TargetView() { Icon = "Comment", Target = TypeTarget.Comment, Name = "Comment" });
            Targets.Add(new TargetView() { Icon = "Function", Target = TypeTarget.Fomular, Name = "Fomular" });
        }
    }
}
