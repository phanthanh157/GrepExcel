using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrepExcel
{
    public class Version
    {
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }

        public Version()
        {
            Major = 1;
            Minor = 0;
            Build = 0;
        }
    }
}
