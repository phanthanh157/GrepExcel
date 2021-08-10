using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrepExcel.Excel
{
    public class ResultInfo
    {
        public int ResultID { get; set; }
        public int SearchInfoID { get; set; }
        public string Result { get; set; }

        public string FileName { get; set; }
        public string Sheet { get; set; }
        public string Cell { get; set; }
    }
}
