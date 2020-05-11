using SMART.WCS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SMART.WCS.Modules.Extensions
{
    public static class CommonExtensions
    {   
        /// <summary>
        /// datetime에서 date만 추출하여 yyyymmdd형식으로 출력
        /// </summary>
        /// <param name="dateTime">yyyyMMdd</param>
        /// <returns></returns>
        public static string ToShortDateDBString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMdd");
        }

        /// <summary>
        /// 해당 테이블명과 컬럼명으로 첫번째 데이터 검색
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="TableName">null인 경우 첫번째 테이블</param>
        /// <param name="ColumnName">null인 경우 첫번째 컬럼</param>
        /// <returns></returns>
        public static object TableFirstData(this System.Data.DataSet dataset, string TableName, string ColumnName)
        {
            return dataset.TableFirstData(0, TableName, ColumnName);
        }

        public static string GetLabelDesc(this System.Windows.Controls.Control usercontrol, string LabelCd)
        {
            string _result = "";
            var _menuId = usercontrol.Tag.ToWhiteSpaceOrString();

            if (Application.Current != null)
            {
                var app = (Application.Current as SMART.WCS.Control.BaseApp);

                var _contentLabel = app.g_LangTable.Where(f => f.MENU_ID == _menuId && f.LAN_CD == LabelCd);

                if (_contentLabel.Count() > 0)
                {
                    _result = _contentLabel.First().LAN_DESC;
                }
            }

            

            if (string.IsNullOrWhiteSpace(_result))
            {
                _result = LabelCd;

                System.Diagnostics.Debug.WriteLine("****언어 추가 필요 *****");
                System.Diagnostics.Debug.WriteLine($"menuID = {_menuId}   LabelCd = {LabelCd}");
            }

            return _result;
        }


        public static Control.BaseInfo BaseInfo(this System.Windows.Controls.UserControl usercontrol)
        {
            if (Application.Current != null)
            {
                return (Application.Current as SMART.WCS.Control.BaseApp).BASE_INFO;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 첫번째 테이블의 지정된 컬럼이나 첫번째 컬럼
        /// </summary>
        /// <param name="dataset"></param>
        /// <param name="ColumnName"> Null인 경우 첫번째 커럼</param>
        /// <returns></returns>
        public static object TableFirstData(this System.Data.DataSet dataset, string ColumnName)
        {
            return dataset.TableFirstData(0, null, ColumnName);
        }

        public static object TableFirstData(this System.Data.DataSet dataset, int TableIndex = 0, string TableName = null, string ColumnName = null)
        {
            object _result = null;

            if (dataset?.Tables.Count > 0)
            {
                if (!string.IsNullOrWhiteSpace(TableName))
                {
                    for (int i = 0; i < dataset.Tables.Count; i++)
                    {
                        if (dataset.Tables[i].TableName == TableName)
                        {
                            TableIndex = i;
                            break;
                        }
                    }
                }

                _result = TableFirstData(dataset.Tables[TableIndex], ColumnName);
            }

            return _result;
        }

        public static object TableFirstData(this System.Data.DataTable table, string ColumnName = null)
        {
            object _result = null;

            if (table.Rows.Count > 0)
            {
                if (ColumnName == null)
                {
                    _result = table.Rows[0][0];
                }
                else
                {
                    if (table.Columns.Cast<System.Data.DataColumn>().Count(x => x.ColumnName == ColumnName) > 0)
                    {
                        _result = table.Rows[0][ColumnName];
                    }
                }
            }

            return _result;
        }


        public static bool ExistsTable(this System.Data.DataSet dataset, string TableName)
        {
            bool _result = false;

            if (dataset.Tables.Count > 0)
            {
                if (TableName != null)
                {
                    _result = dataset.Tables[TableName] != null ? true : false ;
                }
            }

            return _result;
        }



        public static string SelectedKey(this DevExpress.Xpf.Editors.ComboBoxEdit combobox)
        {
            string _result = "";

            if (combobox.SelectedItem != null)
            {
                var _item = combobox.SelectedItem;

                if (_item.GetType().GetProperty(combobox.ValueMember) != null)
                {
                    _result = _item.GetType().GetProperty(combobox.ValueMember).GetValue(_item).ToWhiteSpaceOrString();
                }
            }
            return _result;
        }

        public static string SelectedValue(this DevExpress.Xpf.Editors.ComboBoxEdit combobox)
        {
            string _result = "";

            if (combobox.SelectedItemValue != null)
            {
                var _item = combobox.SelectedItemValue;

                if (_item.GetType().GetProperty(combobox.DisplayMember) != null)
                {
                    _result = _item.GetType().GetProperty(combobox.DisplayMember).GetValue(_item).ToWhiteSpaceOrString();
                }
            }
            return _result;
        }

        /// <summary>
        /// string반환하며 obj가 null이면 null이 아닌 빈값으로 반환
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToWhiteSpaceOrString(this object obj)
        {
            if (obj != null)
            {
                return obj.ToString();
            }
            else
            {
                return "";
            }
        }


        /// <summary>
        /// Property의 값을 반환한다
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetPropertyValue(this object obj, string PropertyName)
        {
            object _result = null;

            if (obj != null)
            {
                var _prop = obj.GetType().GetProperty(PropertyName);

                if (_prop != null)
                {
                    _result = _prop.GetValue(obj);
                }
            }

            return _result;
        }


        /// <summary>
        /// datatable의 데이터를 ObservableCollection<T>에 추가 한다
        /// ObservableCollection<T>가 null이면 생성하여 추가
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="datatable"></param>
        public static void AddRange<T>(this System.Collections.ObjectModel.ObservableCollection<T> collection, List<T> itemList)
        {

            if (collection == null)
            {
                collection = new System.Collections.ObjectModel.ObservableCollection<T>();
            }

            for (int i = 0; i < itemList.Count; i++)
            {
                collection.Add(itemList[i]);
            }
        }

        public static void AddRange<T>(this System.Collections.ObjectModel.ObservableCollection<T> collection, IEnumerable<T> itemList)
        {

            if (collection == null)
            {
                collection = new System.Collections.ObjectModel.ObservableCollection<T>();
            }

            foreach (var item in itemList)
            {
                collection.Add(item);
            }
        }


        /// <summary>
        /// upopup으로 호출된 창이 있으면 활성화하고 아니면 새로 뛰운다
        /// </summary>
        /// <param name="content"></param>
        public static bool IsActiveWindow(this Type contentControlType)
        {
            Type _contentType = contentControlType.GetType();
            bool _IsActive = false;

            //foreach (var item in Application.Current.Windows)
            //{
            //    if (item is Control.uPopup)
            //    {
            //        if ((item as Control.uPopup).Content.GetType() == contentControlType)
            //        {
            //            _IsActive = (item as Control.uPopup).Activate();
            //            break;
            //        }
            //    }
            //}

            return _IsActive;
        }

        public static String SubStringByte(String str, int nStart, int nLen)
        {
            try
            {
                if (str != null && str != String.Empty)
                {
                    Encoding encoding = Encoding.GetEncoding("euc-kr");
                    byte[] abyBuf = encoding.GetBytes(str);
                    int nBuf = abyBuf.Length;

                    if (nStart < 0)
                    {
                        nStart = 0;
                    }
                    else if (nStart > nBuf)
                    {
                        nStart = nBuf;
                    }

                    if (nLen < 0)
                    {
                        nLen = 0;
                    }
                    else if (nLen > nBuf - nStart)
                    {
                        nLen = nBuf - nStart;
                    }

                    if (nStart < nBuf)
                    {
                        int nCopyStart = 0;
                        int nCopyLen = 0;

                        // 시작 위치를 결정한다.
                        if (nStart >= 1)
                        {
                            while (true)
                            {
                                if (abyBuf[nCopyStart] >= 0x80)
                                {
                                    nCopyStart++;
                                }

                                nCopyStart++;

                                if (nCopyStart >= nStart)
                                {
                                    if (nCopyStart > nStart)
                                    {
                                        nLen--;
                                    }

                                    break;
                                }
                            }
                        }

                        // 길이를 결정한다.
                        int nI = 0;

                        while (nI < nLen)
                        {
                            if (abyBuf[nCopyStart + nI] >= 0x80)
                            {
                                nI++;
                            }

                            nI++;
                        }

                        nCopyLen = (nI <= nLen) ? nI : nI - 2;

                        if (nCopyLen >= 1)
                        {
                            return encoding.GetString(abyBuf, nCopyStart, nCopyLen);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            return String.Empty;
        }

        public static bool CloseWindow(this Type contentControlType)
        {
            Type _contentType = contentControlType.GetType();
            bool _IsActive = false;

            foreach (var item in Application.Current.Windows)
            {
                //if (item is Control.uPopup)
                //{
                //    if ((item as Control.uPopup).Content?.GetType() == contentControlType)
                //    {
                //        (item as Control.uPopup).Close();
                //        break;
                //    }
                //}
            }

            return _IsActive;
        }

    }
}
