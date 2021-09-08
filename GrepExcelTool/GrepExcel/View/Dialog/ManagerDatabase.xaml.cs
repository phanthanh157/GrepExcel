using GrepExcel.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GrepExcel.View.Dialog
{
    /// <summary>
    /// Interaction logic for ManagerDatabase.xaml
    /// </summary>
    public partial class ManagerDatabase : Window
    {
        private ManagerDatabaseVm _mDb = null;
        public ManagerDatabase()
        {
            InitializeComponent();
            _mDb = ManagerDatabaseVm.Instance;
            this.DataContext = _mDb;
        }



    }
}
