using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace GrepExcel.Excel
{
    public class SearchStore : SqlLiteImp, ISqlLiteImp
    {
        private static readonly log4net.ILog log_ = LogHelper.GetLogger();
        private bool dispose_ = false;
        public SearchStore(string databaseName = null, SqliteOpenMode sqliteOpenMode = SqliteOpenMode.ReadWriteCreate, SqliteCacheMode sqliteCacheMode = SqliteCacheMode.Shared)
            : base(databaseName, sqliteOpenMode, sqliteCacheMode)
        {

        }

        public SqlResult CreateTable()
        {
            SqlResult res = SqlResult.CreateTableFailed;
            Base.Check(sqlConn_);

            try
            {
                using (var command = sqlConn_.CreateCommand())
                {
                    command.CommandText = @"CREATE TABLE IF NOT EXISTS pct_tblSearch (
                                        search_id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        search TEXT NOT NULL,
                                        folder TEXT NOT NULL,
                                        method INT NOT NULL,
                                        target INT NOT NULL,
                                        match_case INT NOT NULL,
                                        lower_uper INT NOT NULL,
                                        tab_active INT NOT NULL
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
                log_.Error(ex.Message);
            }

            return res;
        }

        public SqlResult Delete(object data)
        {
            SqlResult res = SqlResult.DeleteFailed;
            Base.Check(sqlConn_);
            if (data is null) return res;

            try
            {

                using (var transaction = sqlConn_.BeginTransaction())
                {
                    using (var command = sqlConn_.CreateCommand())
                    {
                        command.CommandText = "DELETE FROM pct_tblSearch WHERE search_id = $search_id ";
                        if (typeof(SearchInfo) == data.GetType())
                        {
                            var searchInfo = data as SearchInfo;
                            command.Parameters.AddWithValue("$search_id", searchInfo.Id);
                        }
                        else if (typeof(ResultInfo) == data.GetType())
                        {
                            var resultInfo = data as ResultInfo;
                            command.Parameters.AddWithValue("$search_id", resultInfo.SearchId);
                        }

                        command.ExecuteNonQuery();

                    }
                    transaction.Commit();
                    res = SqlResult.DeleteSuccess;
                }
            }
            catch (SqliteException ex)
            {
                log_.Error(ex.Message);
            }

            return res;

        }


        public SqlResult DeleteBySearchId(SearchInfo data)
        {
            SqlResult res = SqlResult.DeleteFailed;
            Base.Check(sqlConn_);
            if (data is null) return res;

            try
            {

                using (var transaction = sqlConn_.BeginTransaction())
                {
                    using (var command = sqlConn_.CreateCommand())
                    {
                        //Del form table result
                        command.CommandText = "DELETE FROM pct_tblResult WHERE search_id = $result";
                        command.Parameters.AddWithValue("result", data.Id);
                        command.ExecuteNonQuery();

                        //Del from table search
                        command.CommandText = "DELETE FROM pct_tblSearch WHERE search_id = $search_id ";
                        command.Parameters.AddWithValue("$search_id", data.Id);
                        command.ExecuteNonQuery();

                    }
                    transaction.Commit();
                    res = SqlResult.DeleteSuccess;
                }
            }
            catch (SqliteException ex)
            {
                log_.Error(ex.Message);
            }

            return res;

        }


        public SqlResult DropTable()
        {
            SqlResult res = SqlResult.DeleteTableFailed;
            Base.Check(sqlConn_);

            try
            {
                using (var command = sqlConn_.CreateCommand())
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
            Base.Check(sqlConn_);
            if (data is null) return res;

            try
            {

                using (var transaction = sqlConn_.BeginTransaction())
                {
                    using (var command = sqlConn_.CreateCommand())
                    {
                        var searchInfo = data as SearchInfo;
                        command.CommandText = "INSERT INTO pct_tblSearch (search,folder,method,target,match_case,lower_uper,tab_active)" +
                                              " VALUES($search,$folder,$method,$target,$match_case,$lower_uper,$tab_active)";
                        command.Parameters.AddWithValue("$search", searchInfo.Search);
                        command.Parameters.AddWithValue("$folder", searchInfo.Folder);
                        command.Parameters.AddWithValue("$method", searchInfo.Method);
                        command.Parameters.AddWithValue("$target", searchInfo.Target);
                        command.Parameters.AddWithValue("$match_case", searchInfo.IsMatchCase);
                        command.Parameters.AddWithValue("$lower_uper", searchInfo.IsLowerOrUper);
                        command.Parameters.AddWithValue("$tab_active", searchInfo.IsTabActive);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    res = SqlResult.InsertSucess;
                }
            }
            catch (SqliteException ex)
            {
                log_.Error(ex.Message);
            }
            return res;
        }

        public int LastIndex()
        {
            int index = -1;
            Base.Check(sqlConn_);

            try
            {

                using (var transaction = sqlConn_.BeginTransaction())
                {
                    using (var command = sqlConn_.CreateCommand())
                    {
                        command.CommandText = "SELECT SEQ FROM sqlite_sequence WHERE name = 'pct_tblSearch'";

                        index = Convert.ToInt32(command.ExecuteScalar());

                    }
                }
            }
            catch (SqliteException ex)
            {
                log_.Error(ex.Message);
            }

            return index;
        }

        public SqlResult Update(object data)
        {
            SqlResult res = SqlResult.UpdateFailed;
            Base.Check(sqlConn_);
            if (data is null) return res;

            try
            {

                using (var transaction = sqlConn_.BeginTransaction())
                {
                    using (var command = sqlConn_.CreateCommand())
                    {
                        var searchInfo = data as SearchInfo;
                        command.CommandText = "UPDATE pct_tblSearch SET  " +
                                              "search = $search," +
                                              "folder = $folder," +
                                              "method = $method," +
                                              "target = $target," +
                                              "match_case = $match_case," +
                                              "lower_uper = $lower_uper," +
                                              "tab_active = $tab_active" +
                                              " WHERE search_id = $search_id";
                        command.Parameters.AddWithValue("$search_id", searchInfo.Id);
                        command.Parameters.AddWithValue("$search", searchInfo.Search);
                        command.Parameters.AddWithValue("$folder", searchInfo.Folder);
                        command.Parameters.AddWithValue("$method", searchInfo.Method);
                        command.Parameters.AddWithValue("$target", searchInfo.Target);
                        command.Parameters.AddWithValue("$match_case", searchInfo.IsMatchCase);
                        command.Parameters.AddWithValue("$lower_uper", searchInfo.IsLowerOrUper);
                        command.Parameters.AddWithValue("$tab_active", searchInfo.IsTabActive);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                    res = SqlResult.UpdateSuccess;
                }
            }
            catch (SqliteException ex)
            {
                log_.Error(ex.Message);
            }
            return res;
        }

        /// <summary>
        /// Giai phong bo nho.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (dispose_)
            {
                return;
            }

            if (disposing)
            {
                //Hande
            }

            dispose_ = true;
            base.Dispose(disposing);

        }

        public List<SearchInfo> GetTabActive(bool isTabActive)
        {
            Base.Check(sqlConn_);
            List<SearchInfo> lst = new List<SearchInfo>();

            try
            {

                // create command text.
                using (var command = sqlConn_.CreateCommand())
                {
                    var sqlString = "SELECT * FROM pct_tblSearch WHERE tab_active = $tab_active";
                    command.CommandText = sqlString;
                    command.Parameters.AddWithValue("$tab_active", isTabActive);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // them vao doi tuong.
                            SearchInfo searchInfo = new SearchInfo();
                            searchInfo.Id = reader.GetInt32(0);
                            searchInfo.Search = reader.GetString(1);
                            searchInfo.Folder = reader.GetString(2);
                            searchInfo.Method = (TypeMethod)reader.GetInt32(3);
                            searchInfo.Target = (TypeTarget)reader.GetInt32(4);
                            searchInfo.IsMatchCase = reader.GetBoolean(5);
                            searchInfo.IsLowerOrUper = reader.GetBoolean(6);
                            searchInfo.IsTabActive = reader.GetBoolean(7);

                            lst.Add(searchInfo);
                        }
                    }
                }
            }
            catch (SqliteException ex)
            {
                log_.Error(ex.Message);
            }

            return lst;

        }

        public SearchInfo GetSearchInfoById(int searchId)
        {
            Base.Check(sqlConn_);
            if (searchId <= 0)
                return null;

            List<SearchInfo> lst = new List<SearchInfo>();

            try
            {
                // create command text.
                using (var command = sqlConn_.CreateCommand())
                {
                    var sqlString = "SELECT * FROM pct_tblSearch WHERE search_id = $search_id";
                    command.CommandText = sqlString;
                    command.Parameters.AddWithValue("$search_id", searchId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // them vao doi tuong.
                            SearchInfo searchInfo = new SearchInfo
                            {
                                Id = reader.GetInt32(0),
                                Search = reader.GetString(1),
                                Folder = reader.GetString(2),
                                Method = (TypeMethod)reader.GetInt32(3),
                                Target = (TypeTarget)reader.GetInt32(4),
                                IsMatchCase = reader.GetBoolean(5),
                                IsLowerOrUper = reader.GetBoolean(6),
                                IsTabActive = reader.GetBoolean(7)
                            };

                            lst.Add(searchInfo);
                        }
                    }
                }
            }
            catch (SqliteException ex)
            {
                log_.Error(ex.Message);
            }

            if (lst.Count > 0)
            {
                return lst[0];
            }

            return null;
        }


        public List<SearchInfo> GetSearchInfoByLimit(int limit)
        {
            Base.Check(sqlConn_);
            List<SearchInfo> lst = new List<SearchInfo>();
            try
            {
                // create command text.
                using (var command = sqlConn_.CreateCommand())
                {
                    var sqlString = "SELECT * FROM pct_tblSearch ORDER BY search_id DESC LIMIT $limit";
                    command.CommandText = sqlString;
                    command.Parameters.AddWithValue("$limit", limit);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // them vao doi tuong.
                            SearchInfo searchInfo = new SearchInfo
                            {
                                Id = reader.GetInt32(0),
                                Search = reader.GetString(1),
                                Folder = reader.GetString(2),
                                Method = (TypeMethod)reader.GetInt32(3),
                                Target = (TypeTarget)reader.GetInt32(4),
                                IsMatchCase = reader.GetBoolean(5),
                                IsLowerOrUper = reader.GetBoolean(6),
                                IsTabActive = reader.GetBoolean(7)
                            };
                            lst.Add(searchInfo);
                        }
                    }
                }
            }
            catch (SqliteException ex)
            {
                log_.Error(ex.Message);
            }

            return lst;
        }



        public List<SearchInfo> GetSearchInfoBySearch(string filter, int option = 1)
        {
            Base.Check(sqlConn_);
            List<SearchInfo> lst = new List<SearchInfo>();

            try
            {

                // create command text.
                using (var command = sqlConn_.CreateCommand())
                {
                    string sqlString = string.Empty;
                    if (option == 1) //search
                    {
                        sqlString += "SELECT DISTINCT search FROM pct_tblSearch WHERE search LIKE '%" + filter + "%'";
                    }
                    else if (option == 2)//folder
                    {
                        sqlString += "SELECT DISTINCT folder FROM pct_tblSearch WHERE folder LIKE '%" + filter + "%'";
                    }

                    command.CommandText = sqlString;
                    // command.Parameters.AddWithValue("$search", search);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // them vao doi tuong.
                            SearchInfo searchInfo = new SearchInfo();
                            if (option == 1)
                            {
                                searchInfo.Search = reader.GetString(0);
                            }
                            else if (option == 2)
                            {
                                searchInfo.Folder = reader.GetString(0);
                            }

                            lst.Add(searchInfo);
                        }
                    }
                }
            }
            catch (SqliteException ex)
            {
                log_.Error(ex.Message);
            }

            return lst;
        }


        public List<SearchInfo> GetSearchInfoAll()
        {
            Base.Check(sqlConn_);

            List<SearchInfo> lst = new List<SearchInfo>();

            try
            {
                // create command text.
                using (var command = sqlConn_.CreateCommand())
                {
                    var sqlString = "SELECT * FROM pct_tblSearch";
                    command.CommandText = sqlString;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // them vao doi tuong.
                            SearchInfo searchInfo = new SearchInfo();
                            searchInfo.Id = reader.GetInt32(0);
                            searchInfo.Search = reader.GetString(1);
                            searchInfo.Folder = reader.GetString(2);
                            searchInfo.Method = (TypeMethod)reader.GetInt32(3);
                            searchInfo.Target = (TypeTarget)reader.GetInt32(4);
                            searchInfo.IsMatchCase = reader.GetBoolean(5);
                            searchInfo.IsLowerOrUper = reader.GetBoolean(6);
                            searchInfo.IsTabActive = reader.GetBoolean(7);

                            lst.Add(searchInfo);
                        }
                    }
                }
            }
            catch (SqliteException ex)
            {
                log_.Error(ex.Message);
            }

            return lst;

        }


        public int Count()
        {
            Base.Check(sqlConn_);
            try
            {
                // create command text.
                using (var command = sqlConn_.CreateCommand())
                {
                    var sqlString = "SELECT COUNT(*) FROM pct_tblSearch";
                    command.CommandText = sqlString;

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    return count;
                }
            }
            catch (SqliteException ex)
            {
                log_.Error(ex.Message);
            }

            return -1;
        }
    }
}
