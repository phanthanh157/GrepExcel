using Microsoft.Data.Sqlite;
using System;


namespace GrepExcel.Excel
{

    public class SqlLiteImp : IDisposable
    {
        private static readonly log4net.ILog log_ = LogHelper.GetLogger();
        private readonly string dbName_ = Define.Database;
        private string _databaseName;
        protected SqliteConnection sqlConn_ = null;
        private bool dispose_ = false;
        public SqlLiteImp()
        {
            
        }
        ~SqlLiteImp() => Dispose(false);

        protected SqlLiteImp(string databaseName, SqliteOpenMode sqliteOpenMode = SqliteOpenMode.ReadWriteCreate, SqliteCacheMode sqliteCacheMode = SqliteCacheMode.Shared)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                _databaseName = dbName_;
            }
            else
            {
                _databaseName = databaseName;
            }
            Connection(sqliteOpenMode, sqliteCacheMode);
        }


        /// <summary>
        /// Connection database
        /// </summary>
        /// <param name="sqliteOpenMode"></param>
        /// <param name="password"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        private bool Connection(SqliteOpenMode sqliteOpenMode, SqliteCacheMode cache)
        {
            bool res;
            try
            {
                //connect string Data Source=spec.db|Mode|Password|Cache.
                string dataBase = "Data Source =" + _databaseName;
                var connectString = new SqliteConnectionStringBuilder(dataBase)
                {
                    Mode = sqliteOpenMode,
                    Cache = cache
                };

                //Khong su dung using de thoat connect- giai phong dispose doi tuong.
                sqlConn_ = new SqliteConnection(connectString.ToString());
                sqlConn_.Open();
                res = true;
            }
            catch (SqliteException ex)
            {
                log_.Error(ex.Message);
                res = false;
            }
            return res;
        }

        /// <summary>
        /// Check table exits.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public bool IsTableExits(string table)
        {
            bool res = false;

            if (string.IsNullOrEmpty(table))
                return false;

            if (sqlConn_ is null)
                return false;

            try
            {
                using (var command = sqlConn_.CreateCommand())
                {
                    command.CommandText = "SELECT name FROM sqlite_master WHERE name=$table";
                    command.Parameters.AddWithValue("$table", table);
                    var name = command.ExecuteScalar();

                    if (name != null && name.ToString() == table)
                        res = true;
                }
            }
            catch (SqliteException ex)
            {
                log_.Error(ex.Message);
                res = false;
            }
            return res;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Override Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (dispose_)
                return;

            if (disposing)
                sqlConn_.Dispose();

            dispose_ = true;
        }
    }




}
