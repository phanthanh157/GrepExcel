using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExcelApp = Microsoft.Office.Interop.Excel;

namespace GrepExcel.Excel
{
    public class Grep
    {
        public Grep()
        {

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
                ShowDebug.MsgErr(F.FLMD(), ex.Message);
            }
            finally
            {
                //Release memory.
                //xlApp.ScreenUpdating = true;
                //xlApp.DisplayAlerts = true;

                //xlApp.Application.Quit();
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
            }


        }

        public async Task GrepAsync(SearchInfo searchInfo)
        {
            // await Task.Run(() => GrepSpeedNonTask(searchInfo));
            await GrepSpeedNonTask(searchInfo);
        }

        public async Task GrepSpeedNonTask(SearchInfo searchInfo)
        {
            if (searchInfo == null)
            {
                ShowDebug.Msg(F.FLMD(), "Search info is NULL");
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
                ShowDebug.Msg(F.FLMD(), "Application excel is NULL");
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
            if (searchInfo.IsMatchCase == true)
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

                foreach (string file in files)
                {
                    ShowDebug.Msg(F.FLMD(), "Open File:  '{0}'.", file);
                    await Task.Run(() => ItemGrep(searchInfo,
                                         file,
                                         xlApp,
                                         findExact,
                                         targetCurrent
                                         ));
                }



                xlApp.ScreenUpdating = true;
                xlApp.DisplayAlerts = true;
                xlApp.Application.Quit();

                GC.Collect();
                GC.WaitForPendingFinalizers();

            }
            catch (Exception ex)
            {
                ShowDebug.MsgErr(F.FLMD(), ex.Message);
                //MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void ItemGrep(SearchInfo searchInfo,
                                 string file,
                                 ExcelApp.Application xlApp,
                                 ExcelApp.XlLookAt findExact,
                                 ExcelApp.XlFindLookIn targetCurrent
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

                int TotalSheet = xlWorkbook.Worksheets.Count; //totaol sheet

                for (int idx = 0; idx < TotalSheet; idx++)
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

                    if (currentFind == null)
                    {
                        // Console.WriteLine("Current sheet find is not found");
                        continue;
                    }

                    string fisrtAddressFind = currentFind.Address;
                    _noMatches++;

                    //ShowDebug.Msg(F.FLMD(), "search : {0} ; NoMatches: {1}", searchInfo.Search, _noMatches);
                    //so luong toi da tim kiem
                    if (_noMatches > _maxSearch)
                    {
                        ShowDebug.Msg(F.FLMD(), "Maximum result search");
                        xlWorkbook.Close(false, Type.Missing, Type.Missing);
                        return;
                    }
                    //show result
                    DataGrep(searchInfo, currentFind, file, xlWorksheet.Name);

                    for (int jdx = 1; jdx < _maxSearch; jdx++)
                    {

                        currentFind = wsFind.FindNext(currentFind);

                        // If you didn't move to a new range, you are done.
                        if (currentFind == null)
                        {
                            break;
                        }

                        if (currentFind.Address == fisrtAddressFind)
                        {
                            break;
                        }
                        _noMatches++;

                        //so luong toi da tim kiem
                        //ShowDebug.Msg(F.FLMD(), "search : {0} ; NoMatches: {1}", searchInfo.Search, _noMatches);
                        if (_noMatches > _maxSearch)
                        {
                            ShowDebug.Msg(F.FLMD(), "Maximum result search");
                            xlWorkbook.Close(false, Type.Missing, Type.Missing);
                            return;
                        }
                        //show result next
                        this.DataGrep(searchInfo, currentFind, file, xlWorksheet.Name);

                    }
                }
                // ShowDebug.Msg(F.FLMD(), "Close File when end");
                xlWorkbook.Close(false, Type.Missing, Type.Missing);

            }
            catch (Exception ex)
            {
                //MessageBox.Show("File "+ file + " Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ShowDebug.MsgErr(F.FLMD(), ex.Message);
            }
        }

        private void DataGrep(SearchInfo searchInfo, ExcelApp.Range range, string fileName, string sheetName)
        {
            var excelStore = ExcelStoreManager.Instance;
            ResultInfo searchResult = new ResultInfo();
            searchResult.SearchInfoID = searchInfo.Id;
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
                ShowDebug.MsgErr(F.FLMD(), ex.Message);
            }

            //Insert database result search
            ShowDebug.Msg(F.FLMD(), "Insert database - search: {0}", searchInfo.Search);
            excelStore.InsertResultInfo(searchResult);

        }

    }
}



