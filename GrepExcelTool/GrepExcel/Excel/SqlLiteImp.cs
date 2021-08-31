using Microsoft.Data.Sqlite;
using System;


namespace GrepExcel.Excel
{

    public class SqlLiteImp : IDisposable
    {
        private readonly string DatabaseName = Define.Database;
        private string _databaseName;
        protected SqliteConnection _sqlConnection = null;
        private bool _dispose = false;
        public SqlLiteImp()
        {

        }

        ~SqlLiteImp() => Dispose(false);

        protected SqlLiteImp(string databaseName = "", SqliteOpenMode sqliteOpenMode = SqliteOpenMode.ReadWriteCreate, SqliteCacheMode sqliteCacheMode = SqliteCacheMode.Shared)
        {
            if (string.IsNullOrEmpty(databaseName))
            {
                _databaseName = DatabaseName;
            }
            else
            {
                _databaseName = databaseName;
            }
            ConnectionSpec(sqliteOpenMode, sqliteCacheMode);
        }


        /// <summary>
        /// Check connection.
        /// </summary>
        /// <param name="sqliteOpenMode"></param>
        /// <param name="password"></param>
        /// <param name="cache"></param>
        /// <returns></returns>
        private bool ConnectionSpec(SqliteOpenMode sqliteOpenMode, SqliteCacheMode cache, string password = "12345678")
        {
            bool res = false;

            try
            {
                //connect string Data Source=spec.db|Mode|Password|Cache.
                string dataBase = "Data Source =" + _databaseName;
                var connectString = new SqliteConnectionStringBuilder(dataBase)
                {
                    Mode = sqliteOpenMode,
                    //Password = password,
                    Cache = cache
                };

                //Khong su dung using de thoat connect- giai phong dispose doi tuong.
                _sqlConnection = new SqliteConnection(connectString.ToString());
                _sqlConnection.Open();
                res = true;

            }
            catch (SqliteException ex)
            {
                ShowDebug.MsgErr(F.FLMD(), ex.Message);
                throw;
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
            {
                ShowDebug.MsgErr(F.FLMD(), "sql connection faile = null");
                return false;
            }

            if (_sqlConnection == null)
            {
                ShowDebug.MsgErr(F.FLMD(), "sql connection faile = null");
                return false;
            }

            try
            {
                using (var command = _sqlConnection.CreateCommand())
                {
                    command.CommandText = "SELECT name FROM sqlite_master WHERE name=$table";
                    command.Parameters.AddWithValue("$table", table);
                    var name = command.ExecuteScalar();

                    if (name != null && name.ToString() == table)
                    {
                        res = true;
                    }
                }
            }
            catch (SqliteException ex)
            {
                ShowDebug.MsgErr(F.FLMD(), ex.Message);
                throw;
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
            if (_dispose)
            {
                return;
            }

            if (disposing)
            {
                //handle manager tai nguyen.
                ShowDebug.Msg(F.FLMD(), "release memory database");
                _sqlConnection.Dispose();
            }

            _dispose = true;
        }
    }




}
