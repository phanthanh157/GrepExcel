using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrepExcel.Excel
{
    public class ResultStore : SqlLiteImp, ISqlLiteImp
    {
        private bool _dispose = false;
        public ResultStore(string databaseName="", SqliteOpenMode sqliteOpenMode = SqliteOpenMode.ReadWriteCreate, SqliteCacheMode sqliteCacheMode = SqliteCacheMode.Shared)
            : base(databaseName, sqliteOpenMode, sqliteCacheMode)
        {

        }

        public SqlResult CreateTable()
        {
            SqlResult res = SqlResult.CreateTableFailed;

            if (_sqlConnection == null)
            {
                ShowDebug.Msg(F.FLMD(), "sql connection faile = null");
                return res;
            }

            try
            {
                using (var command = _sqlConnection.CreateCommand())
                {
                    command.CommandText = @"CREATE TABLE IF NOT EXISTS pct_tblResult (
                                        result_id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        result TEXT,
                                        filename TEXT,
                                        sheet VARCHAR(255),
                                        cell VARCHAR(25),
                                        search_id INT,
                                        FOREIGN KEY (search_id) REFERENCES pct_tblSearch(search_id)
                                        )";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE INDEX IF NOT EXISTS index_result_id ON pct_tblResult(result_id)";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE INDEX IF NOT EXISTS index_search_id ON pct_tblSearch(search_id)";
                    command.ExecuteNonQuery();

                    res = SqlResult.CreateTableSuccess;
                }
            }
            catch (SqliteException ex)
            {
                ShowDebug.Msg(F.FLMD(), ex.Message);
            }

            return res;
        }

        public SqlResult Delete(object data)
        {
            SqlResult res = SqlResult.DeleteFailed;

            if (_sqlConnection == null)
            {
                ShowDebug.Msg(F.FLMD(), "sql connection faile = null");
                return res;
            }

            if (data == null)
            {
                ShowDebug.Msg(F.FLMD(), "data = null");
                return res;
            }

            try
            {

                using (var transaction = _sqlConnection.BeginTransaction())
                {
                    using (var command = _sqlConnection.CreateCommand())
                    {
                        var resultInfo = data as ResultInfo;
                        command.CommandText = "DELETE FROM pct_tblResult WHERE result_id = $result_id ";
                        command.Parameters.AddWithValue("$result_id", resultInfo.ResultID);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    res = SqlResult.DeleteSuccess;
                }
            }
            catch (SqliteException ex)
            {
                ShowDebug.Msg(F.FLMD(), ex.Message);
            }

            return res;

        }

        public SqlResult DropTable()
        {
            SqlResult res = SqlResult.DeleteTableFailed;

            if (_sqlConnection == null)
            {
                ShowDebug.Msg(F.FLMD(), "sql connection faile = null");
                return res;
            }

            try
            {
                using (var command = _sqlConnection.CreateCommand())
                {
                    command.CommandText = @"DROP TABLE IF EXISTS pct_tblResult";
                    command.ExecuteNonQuery();
                    res = SqlResult.DeleteTableSuccess;
                }
            }
            catch (SqliteException ex)
            {
                ShowDebug.Msg(F.FLMD(), ex.Message);
            }

            return res;
        }

        public SqlResult Insert(object data)
        {
            SqlResult res = SqlResult.InsertFailed;

            if (_sqlConnection == null)
            {
                ShowDebug.Msg(F.FLMD(), "sql connection faile = null");
                return res;
            }

            if (data == null)
            {
                ShowDebug.Msg(F.FLMD(), "data = null");
                return res;
            }

            try
            {

                using (var transaction = _sqlConnection.BeginTransaction())
                {
                    using (var command = _sqlConnection.CreateCommand())
                    {
                        command.CommandText = @"CREATE TABLE IF NOT EXISTS pct_tblResult (
                                        result_id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        search_id INT NOT NULL,
                                        result TEXT,
                                        filename TEXT,
                                        sheet VARCHAR(255),
                                        cell VARCHAR(25)
                                        )";
                        var resultInfo = data as ResultInfo;
                        command.CommandText = "INSERT INTO pct_tblResult (search_id,result,filename,sheet,cell)" +
                                              " VALUES($search_id,$result,$filename,$sheet,$cell)";
                        command.Parameters.AddWithValue("$search_id", resultInfo.SearchInfoID);
                        command.Parameters.AddWithValue("$result", resultInfo.Result);
                        command.Parameters.AddWithValue("$filename", resultInfo.FileName);
                        command.Parameters.AddWithValue("$sheet", resultInfo.Sheet);
                        command.Parameters.AddWithValue("$cell", resultInfo.Cell);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    res = SqlResult.InsertSucess;
                }
            }
            catch (SqliteException ex)
            {
                ShowDebug.Msg(F.FLMD(), ex.Message);
            }
            return res;
        }

        public int LastIndex()
        {
            int index = -1;
            if (base._sqlConnection == null)
            {
                ShowDebug.Msg(F.FLMD(), "Sql connection faile = null");
                return index;
            }

            try
            {

                using (var transaction = _sqlConnection.BeginTransaction())
                {
                    using (var command = _sqlConnection.CreateCommand())
                    {
                        command.CommandText = "SELECT SEQ FROM sqlite_sequence WHERE name = 'pct_tblResult'";

                        index = Convert.ToInt32(command.ExecuteScalar());

                    }
                }
            }
            catch (SqliteException ex)
            {
                ShowDebug.Msg(F.FLMD(), ex.Message);
                throw;
            }

            return index;
        }

        public SqlResult Update(object data)
        {
            SqlResult res = SqlResult.InsertFailed;

            if (_sqlConnection == null)
            {
                ShowDebug.Msg(F.FLMD(), "sql connection faile = null");
                return res;
            }

            if (data == null)
            {
                ShowDebug.Msg(F.FLMD(), "data = null");
                return res;
            }

            try
            {

                using (var transaction = _sqlConnection.BeginTransaction())
                {
                    using (var command = _sqlConnection.CreateCommand())
                    {
                        var searchInfo = data as SearchInfo;
                        command.CommandText = "UPDATE pct_tblResult SET  " +
                                              "search = $message," +
                                              "folder = $type," +
                                              "method = $request_item," +
                                              "target = $doc," +
                                              "match_case = $sheet," +
                                              "lower_uper = $cell," +
                                              "WHERE search_id = $message_id";
                        command.Parameters.AddWithValue("$search_id", searchInfo.Search);
                        command.Parameters.AddWithValue("$search", searchInfo.Search);
                        command.Parameters.AddWithValue("$folder", searchInfo.Folder);
                        command.Parameters.AddWithValue("$method", searchInfo.Method);
                        command.Parameters.AddWithValue("$target", searchInfo.Target);
                        command.Parameters.AddWithValue("$match_case", searchInfo.IsMatchCase);
                        command.Parameters.AddWithValue("$lower_uper", searchInfo.IsLowerOrUper);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    res = SqlResult.InsertSucess;
                }
            }
            catch (SqliteException ex)
            {
                ShowDebug.Msg(F.FLMD(), ex.Message);
            }
            return res;
        }

        /// <summary>
        /// Giai phong bo nho.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (_dispose)
            {
                return;
            }

            if (disposing)
            {
                //Hande
            }

            _dispose = true;
            base.Dispose(disposing);

        }
    }
}
