using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using GrepExcel.ViewModel;
using ExcelApp = Microsoft.Office.Interop.Excel;

namespace GrepExcel.Excel
{
    public class GrepInfoArgs : EventArgs
    {
        public int TabIndex { get; set; }
        public string SearchText { get; set; }
        public int TotalFiles { get; set; }
        public string CurrentFile { get; set; }
        public int CurrentFileIndex { get; set; }
        public int CurrentMatch { get; set; }
        public ResultInfo Result { get; set; }
    }


    public class Grep
    {
        private static readonly log4net.ILog log_ = LogHelper.GetLogger();
        private int totalFiles_;
        private int currentMatch_;
        public event EventHandler<GrepInfoArgs> EventGrepResult;
        public Grep()
        {
            totalFiles_ = 0;
            currentMatch_ = 0;
        }

        private void OnEventGrepResult(GrepInfoArgs e)
        {
            EventGrepResult?.Invoke(this, e);
        }


        public async Task OpenFileAsync(ResultInfo resultInfo)
        {
            await Task.Run(() => OpenFileExcel(resultInfo));
        }

        private void OpenFileExcel(ResultInfo resultInfo)
        {

            ExcelApp.Application xlApp = new ExcelApp.Application()
            {
                Visible = true,
                AutomationSecurity = Microsoft.Office.Core.MsoAutomationSecurity.msoAutomationSecurityForceDisable
            };
            ExcelApp.Workbook xlWorkbook;
            object misValue = System.Reflection.Missing.Value;
            ExcelApp.Worksheet xlWorksheet;
            ExcelApp.Range wsFind;

            xlApp.ScreenUpdating = false;
            xlApp.DisplayAlerts = false;

            try
            {
                xlWorkbook = xlApp.Workbooks.Open(resultInfo.FileName, false, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue, misValue);
                xlWorksheet = (ExcelApp.Worksheet)xlWorkbook.Worksheets.get_Item(resultInfo.Sheet);
                wsFind = (ExcelApp.Range)xlWorksheet.get_Range(resultInfo.Cell, resultInfo.Cell);

                xlWorksheet.Activate();
                wsFind.Activate();
                xlApp.ScreenUpdating = true;
                xlApp.DisplayAlerts = true;
            }
            catch (Exception ex)
            {
                log_.Error(ex.Message);
            }
            finally
            {
                //Release memory.
                xlApp.ScreenUpdating = true;
                xlApp.DisplayAlerts = true;

                xlApp.Application.Quit();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }


        }

        public void GrepAsync(SearchInfo searchInfo, int tabIndex, CancellationToken ct, Action<bool> action)
        {
            // await Task.Run(() => GrepSpeedNonTask(searchInfo));
             GrepSpeedNonTask(searchInfo, tabIndex, ct ,action);
        }

        public void GrepSpeedNonTask(SearchInfo searchInfo, int tabIndex, CancellationToken ct, Action<bool> action)
        {
            if (searchInfo == null)
            {
                log_.Error("Search info is NULL");
                return;
            }
           
            // Open appiliction excel
            ExcelApp.Application xlApp = new ExcelApp.Application()
            {
                Visible = false,
                AutomationSecurity = Microsoft.Office.Core.MsoAutomationSecurity.msoAutomationSecurityForceDisable
            };

            if (xlApp == null)
            {
                log_.Warn("Application excel is NULL");
                return;
            }

            xlApp.ScreenUpdating = false;
            xlApp.DisplayAlerts = false;

            //target current choise
            ExcelApp.XlFindLookIn targetCurrent;
            switch (searchInfo.Target)
            {
                case TypeTarget.Comment:
                    targetCurrent = ExcelApp.XlFindLookIn.xlComments;
                    break;
                case TypeTarget.Fomular:
                    targetCurrent = ExcelApp.XlFindLookIn.xlFormulas;
                    break;
                case TypeTarget.Value:
                    targetCurrent = ExcelApp.XlFindLookIn.xlValues;
                    break;
                default:
                    targetCurrent = ExcelApp.XlFindLookIn.xlValues;
                    break;
            }

            //target current choise
            ExcelApp.XlLookAt findExact;
            if (searchInfo.IsMatchCase)
            {
                findExact = ExcelApp.XlLookAt.xlWhole;
            }
            else
            {
                findExact = ExcelApp.XlLookAt.xlPart;
            }

            try
            {
                var files = new FileCollection(searchInfo.Folder, searchInfo.Method);

                int countFile = 0;
                foreach(string file in files)
                {
                    countFile++;
                }

                totalFiles_ = countFile;

                countFile = 1;
                foreach (string file in files)
                {
                    log_.DebugFormat("Open File:  '{0}'.", file);
                    ItemGrep(
                            tabIndex,
                            searchInfo,
                            file,
                            xlApp,
                            findExact,
                            targetCurrent,
                            countFile
                            );
                    countFile++;

                    //cancle token
                    if (ct.IsCancellationRequested)
                    {
                        log_.Debug("Cancle searching ...");
                        ct.ThrowIfCancellationRequested();
                    }
                }

            }
            catch (Exception ex)
            {
                log_.Error(ex.Message);
            }
            finally
            {
                //Release memory.
                xlApp.ScreenUpdating = true;
                xlApp.DisplayAlerts = true;

                xlApp.Application.Quit();
                GC.Collect();
                GC.WaitForPendingFinalizers();

                action(true);
            }

        }

        private void ItemGrep(   int tabIndex,
                                 SearchInfo searchInfo,
                                 string file,
                                 ExcelApp.Application xlApp,
                                 ExcelApp.XlLookAt findExact,
                                 ExcelApp.XlFindLookIn targetCurrent,
                                 int countFile
                                 )
        {
            ExcelApp.Workbook xlWorkbook;
            object misValue = System.Reflection.Missing.Value;
            ExcelApp.Worksheet xlWorksheet;
            ExcelApp.Range wsFind;
            ExcelApp.Range currentFind;
            int _noMatches = 0;


            try
            {
                int _maxSearch = int.Parse(Config.ReadSetting("MAX_SEARCH"));

                xlWorkbook = xlApp.Workbooks.Open(file, false, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 1);

                for (int idx = 0; idx < xlWorkbook.Worksheets.Count; idx++)
                {
                    xlWorksheet = (ExcelApp.Worksheet)xlWorkbook.Worksheets.get_Item(idx + 1);

                    wsFind = xlWorksheet.Cells;

                    currentFind = wsFind.Find(searchInfo.Search, misValue,
                                             targetCurrent, // xlFormulas, xlValues, xlComments or xlCommentsThreaded
                                             findExact, // find exact cells
                                             ExcelApp.XlSearchOrder.xlByRows, ExcelApp.XlSearchDirection.xlNext,
                                             searchInfo.IsLowerOrUper, //match case lower/upper
                                             false, // ky tu byte kep
                                             misValue);//format

                    if (currentFind is null)
                    {
                        // Console.WriteLine("Current sheet find is not found");
                        continue;
                    }

                    string fisrtAddressFind = currentFind.Address;
                    _noMatches++;
                    currentMatch_++;

                    //ShowDebug.Msg(F.FLMD(), "search : {0} ; NoMatches: {1}", searchInfo.Search, _noMatches);
                    //so luong toi da tim kiem
                    if (_noMatches > _maxSearch)
                    {
                        log_.Debug("Reach maximum result search");
                        xlWorkbook.Close(false, Type.Missing, Type.Missing);
                        return;
                    }
                    //show result
                    ResultInfo result = DataGrep(searchInfo, currentFind, file, xlWorksheet.Name);

                    //notify result
                    GrepInfoArgs grepInfo = new GrepInfoArgs
                    {
                        TabIndex = tabIndex,
                        SearchText = searchInfo.Search,
                        TotalFiles = totalFiles_,
                        CurrentFile = file,
                        CurrentMatch = currentMatch_,
                        CurrentFileIndex = countFile,
                        Result = result
                    };

                    OnEventGrepResult(grepInfo);


                    for (int jdx = 1; jdx < _maxSearch; jdx++)
                    {

                        currentFind = wsFind.FindNext(currentFind);

                        // If you didn't move to a new range, you are done.
                        if (currentFind == null)
                            break;

                        if (currentFind.Address == fisrtAddressFind)
                            break;

                        _noMatches++;
                        currentMatch_++;

                        //so luong toi da tim kiem
                        if (_noMatches > _maxSearch)
                        {
                            log_.Debug("Maximum result search");
                            xlWorkbook.Close(false, Type.Missing, Type.Missing);
                            return;
                        }
                        //show result next
                        result = DataGrep(searchInfo, currentFind, file, xlWorksheet.Name);

                        //notify result
                        grepInfo = new GrepInfoArgs
                        {
                            TabIndex = tabIndex,
                            SearchText = searchInfo.Search,
                            TotalFiles = totalFiles_,
                            CurrentFile = file,
                            CurrentMatch = currentMatch_,
                            CurrentFileIndex = countFile,
                            Result = result
                        };

                        OnEventGrepResult(grepInfo);
                    }
                }
                xlWorkbook.Close(false, Type.Missing, Type.Missing);

            }
            catch (Exception ex)
            {
                log_.Error(ex.Message);
            }
        }

        private ResultInfo DataGrep(SearchInfo searchInfo, ExcelApp.Range range, string fileName, string sheetName)
        {
            var excelStore = ExcelStoreManager.Instance;
            ResultInfo searchResult = new ResultInfo();
            searchResult.SearchId = searchInfo.Id;
            searchResult.FileName = fileName;
            searchResult.Sheet = sheetName;
            searchResult.Cell = Regex.Replace(range.Address, @"[$]", string.Empty);

            try
            {
                switch (searchInfo.Target)
                {
                    case TypeTarget.Comment:
                        searchResult.Result = range.Comment.Shape.AlternativeText;
                        break;
                    case TypeTarget.Fomular:
                        searchResult.Result = range.Formula;
                        break;
                    case TypeTarget.Value:
                        if (range.Value is double)
                        {
                            double v = range.Value;
                            searchResult.Result = v.ToString();
                        }
                        else if (range.Value is int)
                        {
                            int v = range.Value;
                            searchResult.Result = v.ToString();
                        }
                        else if (range.Value is DateTime)
                        {
                            DateTime v = range.Value;
                            searchResult.Result = v.ToString();
                        }
                        else if (range.Value is bool)
                        {
                            bool v = range.Value;
                            searchResult.Result = v.ToString();
                        }
                        else
                        {
                            searchResult.Result = range.Value;
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                log_.Error(ex.Message);
            }

            //Insert database result search
            //log_.DebugFormat("Insert database - search: {0}", searchInfo.Search);
            excelStore.InsertResultInfo(searchResult);

            return searchResult;
        }

    }
}



