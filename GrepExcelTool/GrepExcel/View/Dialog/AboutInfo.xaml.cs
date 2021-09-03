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
    /// Interaction logic for AboutInfo.xaml
    /// </summary>
    public partial class AboutInfo : Window
    {
        public AboutInfo()
        {
            InitializeComponent();

            txtContent.Text = "" +
               "Tool Grep Excel is free and open-source, help search many file excel.\n\n" +
               "Link: https://github.com/phanthanh157/GrepExcel \n\n" +

               "Potable distribution.";
          
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
