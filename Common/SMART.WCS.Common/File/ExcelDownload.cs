using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Printing;
using DevExpress.XtraPrinting;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Common.File
{
    public class ExcelDownload
    {
        /// <summary>
        /// DevExpress 엑셀 다운로드
        /// </summary>
        /// <param name="_tableView"></param>
        /// <param name="_strFileName"></param>
        public static void GetExcelDownload(TableView _tableView, string _strFileName)
        {
            try
            {
                _tableView.ExportToXls(_strFileName, new XlsExportOptionsEx()
                { ExportType = DevExpress.Export.ExportType.WYSIWYG });

                Process.Start(string.Format(_strFileName));
            }
            catch { throw; }
        }

       

        public static void ExportToXlsx(IPrintableControl printableControlLink, string _strFileName)
        {
            using (DevExpress.Xpf.Printing.PrintableControlLink link = new DevExpress.Xpf.Printing.PrintableControlLink(printableControlLink))
            {
                link.ExportToXlsx(_strFileName, new XlsxExportOptions() { ExportMode = XlsxExportMode.SingleFile, ShowGridLines = true });
                // 엑셀 다운로드 대상 컬럼 타입을 String으로 설정
                link.ExportToXlsx(_strFileName, new XlsxExportOptions() { TextExportMode = TextExportMode.Text });
            }
        }


        public static void MergeXlsxFiles(string _strDestXlsxFileName, params string[] _arrSourceXlsxFileNames)
        {
            Microsoft.Office.Interop.Excel.Application excelApp = null;
            Microsoft.Office.Interop.Excel.Workbook destWorkBook = null;

            try
            {
                excelApp = new Microsoft.Office.Interop.Excel.Application() { DisplayAlerts = false };
                excelApp.SheetsInNewWorkbook = 1;
                destWorkBook = excelApp.Workbooks.Add();

                foreach (var sourceXlsxFile in _arrSourceXlsxFileNames)
                {
                    Microsoft.Office.Interop.Excel.Workbook sourceWorkBook = excelApp.Workbooks.Open(sourceXlsxFile);
                    foreach (Microsoft.Office.Interop.Excel.Worksheet sheet in sourceWorkBook.Sheets)
                    {
                        sheet.Columns.NumberFormat = "@";
                        sheet.Copy(Type.Missing, destWorkBook.Sheets[destWorkBook.Sheets.Count]);
                        destWorkBook.Sheets[destWorkBook.Sheets.Count].Name = "Sheet " + (destWorkBook.Sheets.Count - 1);
                    }
                    sourceWorkBook.Close(XlSaveAction.xlDoNotSaveChanges);
                }
                destWorkBook.Sheets[1].Delete();
                destWorkBook.SaveAs(_strDestXlsxFileName);
            }
            finally
            {
                if (destWorkBook != null)
                {
                    destWorkBook.Close(XlSaveAction.xlSaveChanges);
                }

                if (excelApp != null)
                {
                    excelApp.Quit();
                }
            }
        }

        public static void GetExcelDownloadTreeList(TreeListView _tableView, string _strFileName)
        {
            try
            {
                _tableView.ExportToXls(_strFileName, new XlsExportOptionsEx()
                {
                    ExportType = DevExpress.Export.ExportType.WYSIWYG
                });

                Process.Start(string.Format(_strFileName));
            }
            catch { throw; }
        }

        public static void DeleteFileIfExist(string _strFileName)
        {
            if (System.IO.File.Exists(_strFileName))
            {
                System.IO.File.Delete(_strFileName);
            }
        }
    }
}
