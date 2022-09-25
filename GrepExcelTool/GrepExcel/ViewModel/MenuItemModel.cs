using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GrepExcel.View;
using GrepExcel.ViewModel;

namespace GrepExcel.ViewModel
{
    public class MenuItemModel : BaseModel
    {
        private bool isShow_;
        public string Header { get; set; }
        public int Column { get; set; }
        public bool IsShow {
            get { return isShow_; }
            set
            {
                if(isShow_ != value)
                {
                    isShow_ = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<MenuItemModel> MenuItems { get; set; }

        private ICommand command_;
        public MenuItemModel()
        {

        }
        public ICommand Command
        {
            get
            {
                if(command_ is null)
                {
                    command_ = new RelayCommand((sender) => CommandHandler(sender));
                }
                return command_;
            }
        }

        private void CommandHandler(object sender)
        {
            IsShow = IsShow ? false : true;

            var mainVm = MainViewModel.Instance;
            mainVm.UpdateShowHideColumnSearch();
        }
    }
}
