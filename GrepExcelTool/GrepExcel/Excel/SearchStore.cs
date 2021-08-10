using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrepExcel.Excel
{
    public class SearchStore : SqlLiteImp, ISqlLiteImp
    {
        private bool _dispose = false;
        public SearchStore(string databaseName="", SqliteOpenMode sqliteOpenMode = SqliteOpenMode.ReadWriteCreate, SqliteCacheMode sqliteCacheMode = SqliteCacheMode.Shared) 
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
                    command.CommandText = @"CREATE TABLE IF NOT EXISTS pct_tblSearch (
                                        search_id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        search TEXT NOT NULL,
                                        folder TEXT NOT NULL,
                                        method INT NOT NULL,
                                        target INT NOT NULL,
                                        match_case INT NOT NULL,
                                        lower_uper INT NOT NULL
                                        )";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE INDEX IF NOT EXISTS index_search_id ON pct_tblSearch(search_id)";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE INDEX IF NOT EXISTS index_search ON pct_tblSearch(search)";
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

            if(data == null)
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
                        command.CommandText = "DELETE FROM pct_tblSearch WHERE search_id = $search_id ";
                        if (typeof(SearchInfo) == data.GetType())
                        {
                            var searchInfo = data as SearchInfo;
                            command.Parameters.AddWithValue("$search_id", searchInfo.Id);
                        }
                        else if(typeof(ResultInfo) == data.GetType())
                        {
                            var resultInfo = data as ResultInfo;
                            command.Parameters.AddWithValue("$search_id", resultInfo.SearchInfoID);
                        }
                     
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
                    command.CommandText = @"DROP TABLE IF EXISTS pct_tblSearch";
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
                        var searchInfo = data as SearchInfo;
                        command.CommandText = "INSERT INTO pct_tblSearch (search,folder,method,target,match_case,lower_uper)" +
                                              " VALUES($search,$folder,$method,$target,$match_case,$lower_uper)";
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
                        command.CommandText = "SELECT SEQ FROM sqlite_sequence WHERE name = 'pct_tblSearch'";

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
                        command.CommandText = "UPDATE pct_tblSearch SET  " +
                                              "search = $search," +
                                              "folder = $folder," +
                                              "method = $method," +
                                              "target = $target," +
                                              "match_case = $match_case," +
                                              "lower_uper = $lower_uper," +
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
