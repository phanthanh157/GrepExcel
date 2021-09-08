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
        public SearchInfo()
        {
            Id = -1;
            Search = string.Empty;
            Folder = string.Empty;
            Method = TypeMethod.SubFolder;
            Target = TypeTarget.Value;
            IsMatchCase = false;
            IsLowerOrUper = false;
            IsTabActive = false;
        }
        public int Id { get; set; }
        public string Search { get; set; }
        public string Folder { get; set; }

        public TypeMethod Method { get; set; }

        public TypeTarget Target { get; set; }

        public bool IsMatchCase { get; set; }

        public bool IsLowerOrUper { get; set; }

        public bool IsTabActive { get; set; }

        public static bool operator ==(SearchInfo left, SearchInfo right)
        {
            if (right is null)
            {
                if (left is null)
                {
                    return true;
                }
                return false;
            }
            return ((left.Search == right.Search) &&
                  (left.Folder == right.Folder) &&
                  (left.Method == right.Method) &&
                  (left.Target == right.Target) &&
                  (left.IsMatchCase == right.IsMatchCase) &&
                  (left.IsLowerOrUper == right.IsLowerOrUper));
        }

        public static bool operator !=(SearchInfo left, SearchInfo right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var right = (SearchInfo)obj;
            return ((this.Search == right.Search) &&
                     (this.Folder == right.Folder) &&
                     (this.Method == right.Method) &&
                     (this.Target == right.Target) &&
                     (this.IsMatchCase == right.IsMatchCase) &&
                     (this.IsLowerOrUper == right.IsLowerOrUper));
        }

        public override int GetHashCode()
        {

            return Search.GetHashCode() ^ Method.GetHashCode() ^ Target.GetHashCode() ^ IsMatchCase.GetHashCode() ^ IsLowerOrUper.GetHashCode();

        }

    }



}
