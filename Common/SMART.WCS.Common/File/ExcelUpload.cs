using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SMART.WCS.Common.File
{
    public class ExcelUpload
    {
        #region > 엑셀 데이터를 데이터 테이블로 변환
        /// <summary>
        /// 엑셀 데이터를 데이터 테이블로 변환
        /// </summary>
        /// <param name="_strFileName">Reference (엑셀 업로드 파일명)</param>
        /// <returns></returns>
        public static DataTable ConvertExcelToDataTable(ref string _strFileName)
        {
            DataTable dtRtnValue = null;

            try
            {
                using (System.Windows.Forms.OpenFileDialog dialog = new System.Windows.Forms.OpenFileDialog())
                {
                    dialog.DefaultExt = "*.*";
                    dialog.InitialDirectory = ",";
                    dialog.Filter = "Excel(*.xls; *.xlsx; *)| *.xls; *.xlsx; *";

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string strConnectionString  = string.Empty;
                        _strFileName                = File.GetFileNameExtension(dialog.FileName);

                        if (dialog.FileName.IndexOf(".xlsx") > -1)
                        {
                            strConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data source=" + dialog.FileName + ";Extended Properties=\"Excel 12.0\"";
                        }
                        else
                        {
                            strConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data source=" + dialog.FileName + ";Extended Properties=\"Excel 8.0\"";
                        }

                        using (OleDbConnection conn = new OleDbConnection(strConnectionString))
                        {
                            conn.Open();

                            DataTable dtTemp = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                            using (OleDbDataAdapter adapter = new OleDbDataAdapter(string.Format(" SELECT * FROM [{0}] ", dtTemp.Rows[0]["TABLE_NAME"].ToString()), conn))
                            {
                                if (dtRtnValue == null) { dtRtnValue = new DataTable(); }
                                adapter.Fill(dtRtnValue);
                            }
                        }
                    }
                }

                return dtRtnValue;
            }
            catch { throw; }
        }
        #endregion

        #region > ConvertDataTableToStringWriter - 데이터 테이블을 StringWrite형식으로 변환
        /// <summary>
        /// 데이터 테이블을 StringWrite형식으로 변환
        /// </summary>
        /// <param name="_dtExcelData">엑셀 데이터 (데이터 테이블)</param>
        /// <returns></returns>
        public static StringWriter ConvertDataTableToStringWriter(DataTable _dtExcelData)
        {
            try
            {
                StringWriter sWriter;

                using (sWriter = new StringWriter())
                {
                    _dtExcelData.WriteXml(sWriter, true);
                }

                return sWriter;
            }
            catch { throw; }

        }
        #endregion
    }
}
