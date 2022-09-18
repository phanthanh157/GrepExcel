using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;

namespace GrepExcel.Excel
{
    public class CsvManager
    {
        public static void WriteDataToCsv<T>(string fullName, List<T> data)
        {
            if (data == null || data.Count == 0)
                return;

            FileStream fs = null;

            try
            {
                fs = new FileStream(fullName, FileMode.Create);
                using (var writer = new StreamWriter(fs, Encoding.UTF8))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<ResultInfoMap>();
                    csv.WriteRecords(data);
                }
                fs.Close();
            }
            catch
            {

            }
            finally
            {
                if (fs != null)
                    fs.Dispose();
            }
        }
    }
}
