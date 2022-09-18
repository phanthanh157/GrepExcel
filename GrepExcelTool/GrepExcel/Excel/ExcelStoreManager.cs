using System;
using System.Collections.Generic;
using System.IO;

namespace GrepExcel.Excel
{
    public class ExcelStoreManager
    {
        private static readonly Lazy<ExcelStoreManager> lazy_ = new Lazy<ExcelStoreManager>(() => new ExcelStoreManager());
        private ExcelStoreManager()
        {
            if (!File.Exists(Define.Database))
            {
                CreateTable();
            }
            // DropTable();
        }
        public static ExcelStoreManager Instance => lazy_.Value;


        /// <summary>
        /// Create table 
        /// </summary>
        public SqlResult CreateTable()
        {
            using (var searchInfo = new SearchStore())
            {
                if (SqlResult.CreateTableFailed == searchInfo.CreateTable())
                {
                    return SqlResult.CreateTableFailed;
                }
            }

            using (var resultInfo = new ResultStore())
            {
                if (SqlResult.CreateTableFailed == resultInfo.CreateTable())
                {
                    return SqlResult.CreateTableFailed;
                }
            }
            return SqlResult.CreateTableSuccess;
        }

        /// <summary>
        /// Droptable
        /// </summary>
        public SqlResult DropTable()
        {

            using (var resultInfo = new ResultStore())
            {
                if (SqlResult.DeleteTableFailed == resultInfo.DropTable())
                {
                    return SqlResult.DeleteTableFailed;
                }
            }

            using (var searchInfo = new SearchStore())
            {
                if (SqlResult.DeleteTableFailed == searchInfo.DropTable())
                {
                    return SqlResult.DeleteTableFailed;
                }
            }
            return SqlResult.DeleteTableSuccess;
        }



        public SqlResult InsertSearchInfo(object data)
        {
            using (var searchInfo = new SearchStore())
            {
                return searchInfo.Insert(data);
            }
        }

        public int LastIndexSearch()
        {
            using (var searchInfo = new SearchStore())
            {
                return searchInfo.LastIndex();
            }
        }


        public SqlResult InsertResultInfo(object data)
        {
            using (var resultInfo = new ResultStore())
            {
                return resultInfo.Insert(data);
            }
        }


        public int LastIndexResult()
        {
            using (var resultInfo = new ResultStore())
            {
                return resultInfo.LastIndex();
            }
        }


        public SqlResult Delete(object data)
        {
            using (var resultInfo = new ResultStore())
            {
                if (SqlResult.DeleteSuccess == resultInfo.Delete(data))
                {
                    using (var searchInfo = new SearchStore())
                    {
                        return searchInfo.Delete(data);
                    }
                }
            }
            return SqlResult.DeleteFailed;
        }

        public List<ResultInfo> GetResultInfoBySearchId(int searchID)
        {
            using (var resultInfo = new ResultStore())
            {
                return resultInfo.GetResultInfoBySearchId(searchID);
            }
        }

        public List<ResultInfo> GetResultInfoAll()
        {
            using (var resultInfo = new ResultStore())
            {
                return resultInfo.GetResultInfoAll();
            }
        }


        public SqlResult UpdateSearchInfo(object data)
        {
            using (var searchInfo = new SearchStore())
            {
                return searchInfo.Update(data);
            }
        }

        public List<SearchInfo> GetTabActive(bool tabActive)
        {
            using (var searchInfo = new SearchStore())
            {
                return searchInfo.GetTabActive(tabActive);
            }
        }

        public SearchInfo GetSearchInfoById(int searchId)
        {
            using (var searchInfo = new SearchStore())
            {
                return searchInfo.GetSearchInfoById(searchId);
            }
        }


        public List<SearchInfo> GetSearchInfoByLimit(int limit)
        {
            using (var searchInfo = new SearchStore())
            {
                return searchInfo.GetSearchInfoByLimit(limit);
            }
        }

        public List<SearchInfo> GetSearchInfoBySearch(string filter, int option = 1)
        {
            using (var searchInfo = new SearchStore())
            {
                return searchInfo.GetSearchInfoBySearch(filter, option);
            }
        }


        public List<SearchInfo> GetSearchInfoAll()
        {
            using (var resultInfo = new SearchStore())
            {
                return resultInfo.GetSearchInfoAll();
            }
        }


        public SqlResult DeleteBySearchId(SearchInfo data)
        {
            using (var searchInfo = new SearchStore())
            {
                return searchInfo.DeleteBySearchId(data);
            }
        }

        public SqlResult DeleteResultInfoBySearchId(SearchInfo data)
        {
            using (var resultInfo = new ResultStore())
            {
                return resultInfo.DeleteBySearchId(data);
            }
        }

        /// <summary>
        /// Total rows search info in database
        /// </summary>
        /// <returns></returns>
        public int CountSearchInfo()
        {
            using (var searchInfo = new SearchStore())
            {
                return searchInfo.Count();
            }
        }

        /// <summary>
        /// Total rows result info in database
        /// </summary>
        /// <returns></returns>
        public int CountResultInfo()
        {
            using (var resultInfo = new ResultStore())
            {
                return resultInfo.CountResultInfo();
            }
        }

        /// <summary>
        /// Total rows result info in database
        /// </summary>
        /// <returns></returns>
        public int CountResultInfoBySearchId(int searchId)
        {
            using (var resultInfo = new ResultStore())
            {
                return resultInfo.CountWithBySerachId(searchId);
            }
        }


    }
}
