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

        public bool IsTabActive { get; set; }

        //public static bool operator ==(SearchInfo left, SearchInfo right)
        //{
        //    if(right is null)
        //    {
        //        if(left is null)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //    return ((left.Search == right.Search) &&
        //          (left.Folder == right.Folder) &&
        //          (left.Method == right.Method) &&
        //          (left.Target == right.Target) &&
        //          (left.IsMatchCase == right.IsMatchCase) &&
        //          (left.IsLowerOrUper == right.IsLowerOrUper)) ;
        //}

        //public static bool operator !=(SearchInfo left, SearchInfo right)
        //{
        //    return !(left == right);
        //}

    }



}
