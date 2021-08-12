using GrepExcel.View;
using System.Windows.Controls;

namespace GrepExcel.ViewModel
{
    public class TabControl : BaseModel
    {
        private string _tabName;
        private UserControl _userControl;

        public UserControl Control { get => _userControl; set => _userControl = value; }
        public string TabName { get => _tabName; set => _tabName = value; }

    }
}
