using GrepExcel.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GrepExcel.ViewModel.Search
{
    public class SearchTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SearchTempalte { get; set; }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is SearchInfo)
                return SearchTempalte;
            return base.SelectTemplate(item, container);
        }
    }
}
