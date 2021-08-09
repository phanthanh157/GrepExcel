using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GrepExcel.View;
using GrepExcel.Excel;

namespace GrepExcel.ViewModel
{
    public class SearchInputVm
    {
        private ICommand _commandSearch = null;

        public SearchInputVm()
        {

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
            var inputInfo = sender as SearchInfomation;

            var mainVm = MainViewModel.Instance;



            TabControl tabResult = new SearchResultVm();
            tabResult.TabName = inputInfo.Search;


            mainVm.AddTabControl(tabResult);

        }
    }
}
