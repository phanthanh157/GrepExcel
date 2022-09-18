using System.Globalization;
using CsvHelper.Configuration;

namespace GrepExcel.Excel
{
    public class ResultInfo
    {
        public int ResultId { get; set; }
        public int SearchId { get; set; }
        public string Result { get; set; }
        public string FileName { get; set; }
        public string Sheet { get; set; }
        public string Cell { get; set; }
    }

    public sealed class ResultInfoMap : ClassMap<ResultInfo>
    {
        public ResultInfoMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.ResultId).Ignore();
            Map(m => m.SearchId).Ignore();
        }
    }
}
