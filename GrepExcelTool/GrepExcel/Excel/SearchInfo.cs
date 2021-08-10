using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrepExcel.Excel
{
    public enum TypeMethod
    {
        Folder,
        SubFolder
    }

    public enum TypeTarget
    {
        Value,
        Comment,
        Fomular
    }

    public class SearchInfo
    {
        public int Id { get; set; }
        public string Search { get; set; }
        public string Folder { get; set; }

        public TypeMethod Method { get; set; }

        public TypeTarget Target { get; set; }

        public bool IsMatchCase { get; set; }

        public bool IsLowerOrUper { get; set; }

    }



}
