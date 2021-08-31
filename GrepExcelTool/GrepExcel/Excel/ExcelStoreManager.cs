﻿using System.Collections.Generic;

namespace GrepExcel.Excel
{
    public class ExcelStoreManager
    {
        private static ExcelStoreManager _instance = null;
        public ExcelStoreManager()
        {
            CreateTable();
            // DropTable();
        }


        public static ExcelStoreManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ExcelStoreManager();
                return _instance;
            }
        }


        /// <summary>
        /// Create table 
        /// </summary>
        public void CreateTable()
        {
            using (var searchInfo = new SearchStore())
            {
                SqlResult sqlResult = searchInfo.CreateTable();
                if (sqlResult == SqlResult.DeleteTableSuccess)
                {
                    ShowDebug.Msg(F.FLMD(), "table pct_tblResult -- delete success ");
                }
            }

            using (var resultInfo = new ResultStore())
            {
                SqlResult sqlResult = resultInfo.CreateTable();
                if (sqlResult == SqlResult.DeleteTableSuccess)
                {
                    ShowDebug.Msg(F.FLMD(), "table pct_tblSearch -- delete success ");
                }
            }
        }

        /// <summary>
        /// Droptable
        /// </summary>
        public void DropTable()
        {

            using (var resultInfo = new ResultStore())
            {
                resultInfo.DropTable();
            }

            using (var searchInfo = new SearchStore())
            {
                searchInfo.DropTable();
            }
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

    }
}
