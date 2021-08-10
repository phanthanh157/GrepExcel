using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GrepExcel.View;
using GrepExcel.Excel;
using System.Collections.ObjectModel;

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
                    _commandSearch = new RelayCommand(sender => CommandSeachHander(sender));
                }
                return _commandSearch;
            }
        }

        private void CommandSeachHander(object sender)
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

            //Insert input info to database
            SqlResult sqlResult =  excelStore.InsertSearchInfo(inputInfo);
            if(SqlResult.InsertSucess == sqlResult)
            {
                ShowDebug.Msg(F.FLMD(), "Insert Search info success");
                inputInfo.Id = excelStore.LastIndexSearch();// add id 
                //Search process
                var grep = new Grep();
                //grep.GrepSpeedNonTask(inputInfo);
                grep.GrepAsync(inputInfo);

                //Display result
                TabControl tabResult = new SearchResultVm();
                tabResult.TabName = inputInfo.Search;

                mainVm.AddTabControl(tabResult);
           
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
