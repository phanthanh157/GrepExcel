using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrepExcel.Excel
{
    public enum SqlResult
    {
        Normal,
        Error,
        Timeout,
        Null,
        CreateTableSuccess,
        CreateTableFailed,
        DeleteTableSuccess,
        DeleteTableFailed,
        UpdateSuccess,
        UpdateFailed,
        DeleteSuccess,
        DeleteFailed,
        InsertSucess,
        InsertFailed
    }
    public interface ISqlLiteImp
    {
        SqlResult CreateTable();
        SqlResult DropTable();

        SqlResult Insert(object data);

        SqlResult Update(object data);

        SqlResult Delete(object data);

        int LastIndex();

    }
}
