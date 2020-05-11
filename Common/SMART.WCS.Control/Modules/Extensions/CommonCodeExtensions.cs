using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.Control;
using SMART.WCS.Modules.Behaviors;
using SMART.WCS.Modules.Extensions;
using System;
using System.Collections.Generic;
using System.Windows;

namespace SMART.WCS.Modules.Extensions
{
    public class CommonCodeExtensions : List<SMART.WCS.Common.Data.ComboBoxInfo>
    {
        BaseClass BaseClass     = new BaseClass();
        BaseInfo BaseInfo       = new BaseInfo();

        public static readonly DependencyProperty CommonCodeProperty
            = DependencyProperty.RegisterAttached("CommonCode", typeof(string), typeof(CommonCodeExtensions), null);


        public static string GetcommonCode(UIElement _element)
        {
            return (string)_element.GetValue(CommonCodeProperty);
        }

        public static void SetCommonCode(UIElement _element, string _strCommonCode)
        {
            _element.SetValue(CommonCodeProperty, _strCommonCode);
        }


        //public static readonly DependencyProperty CommonCodeTypeProperty = DependencyProperty.RegisterAttached(
        //                                                       "CommonCodeType",
        //                                                       typeof(CodeType),
        //                                                       typeof(CommonCodeExtensions), null);

        //public static void SetCommonCodeType(UIElement element, CodeType value)
        //{
        //    element.SetValue(CommonCodeTypeProperty, value);
        //}
        //public static CodeType GetCommonCodeType(UIElement element)
        //{
        //    return (CodeType)element.GetValue(CommonCodeTypeProperty);
        //}

        //public static readonly DependencyProperty IsAllTypeCodeAddProperty = DependencyProperty.RegisterAttached(
        //                                                    "IsAllTypeCodeAdd",
        //                                                    typeof(bool),
        //                                                    typeof(CommonCodeExtensions), new PropertyMetadata(false));

        //public static void SetIsAllTypeCodeAdd(UIElement element, CodeType value)
        //{
        //    element.SetValue(IsAllTypeCodeAddProperty, value);
        //}
        //public static bool GetIsAllTypeCodeAdd(UIElement element)
        //{
        //    return (bool)element.GetValue(IsAllTypeCodeAddProperty);
        //}


        //public CodeType CommonCodeType
        //{
        //    get; set;
        //}

        //public bool IsAllTypeCodeAdd
        //{
        //    get; set;
        //}

        public CommonCodeExtensions()
        {
            //GetList();
        }

        //public CommonCodeExtensions(CodeType codeType, bool isAlltypeCodeAdd)
        //{
        //    this.IsAllTypeCodeAdd = isAlltypeCodeAdd;
        //    this.CommonCodeType = codeType;
        //    GetList();
        //}

        //public CommonCodeExtensions(CodeType codeType, bool isAlltypeCodeAdd, string callType)
        //{
        //    this.IsAllTypeCodeAdd = isAlltypeCodeAdd;
        //    this.CommonCodeType = codeType;

        //    GetList(callType, String.Empty, string.Empty, string.Empty, string.Empty);

        //    /*
        //    using (BaseClass baseclass = new BaseClass())
        //    {
        //        this.AddRange(baseclass.GetComboBoxListItem(
        //                                   callType
        //                                 , attr1
        //                                 , attr2
        //                                 , attr3
        //                                 , IsAllTypeCodeAdd
        //                                 , "ALL"
        //                                 , (Application.Current as BaseApp).BASE_INFO.country_cd
        //                                 , _CenterCd
        //                             ));
        //    }
        //    */
        //}

        //public CommonCodeExtensions(CodeType codeType, bool isAlltypeCodeAdd, string callType, string attr1, string attr2, string attr3, string attr4)
        //{
        //    this.IsAllTypeCodeAdd = isAlltypeCodeAdd;
        //    this.CommonCodeType = codeType;

        //    GetList(callType, attr1, attr2, attr3, attr4);

        //    /*
        //    using (BaseClass baseclass = new BaseClass())
        //    {
        //        this.AddRange(baseclass.GetComboBoxListItem(
        //                                   callType
        //                                 , attr1
        //                                 , attr2
        //                                 , attr3
        //                                 , IsAllTypeCodeAdd
        //                                 , "ALL"
        //                                 , (Application.Current as BaseApp).BASE_INFO.country_cd
        //                                 , _CenterCd
        //                             ));
        //    }
        //    */
        //}

        #region CommonCodeExtensions - XAML단 공통코드 설정 - 파라메터에 공통코드만 있는 경우
        /// <summary>
        /// XAML단 공통코드 설정 - 파라메터에 공통코드만 있는 경우
        /// </summary>
        /// <param name="_strCommonCode"></param>
        public CommonCodeExtensions(string _strCommonCode)
        {
            if (_strCommonCode.Length > 0)
            {
                var dtComboBoxInfo  = SMART.WCS.Common.Control.CommonComboBox.GetFirstCommonData(_strCommonCode);

                if (dtComboBoxInfo != null && dtComboBoxInfo.Rows.Count > 0)
                {
                    this.AddRange(ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtComboBoxInfo));
                }
            }
        }
        #endregion

        public CommonCodeExtensions(string _strCommonCode, string[] _arrComboBoxInputParam, bool _isfirstRowEmpty)
        {
            if (_strCommonCode.Length > 0)
            {
                var dtComboBoxInfo = SMART.WCS.Common.Control.CommonComboBox.GetCommonData(_strCommonCode, _arrComboBoxInputParam, _isfirstRowEmpty);

                if (dtComboBoxInfo != null && dtComboBoxInfo.Rows.Count > 0)
                {
                    this.AddRange(ConvertDataTableToList.DataTableToList<ComboBoxInfo>(dtComboBoxInfo));
                }
            }
        }


        //public CommonCodeExtensions(CodeType codeType, bool isAlltypeCodeAdd, string attr1, string attr2, string attr3, string attr4)
        //{
        //    this.IsAllTypeCodeAdd = isAlltypeCodeAdd;
        //    this.CommonCodeType = codeType;
        //    string _CallCodeType = null;

        //    switch (CommonCodeType)
        //    {
        //        // 사용여부
        //        case CodeType.USE_YN:
        //            _CallCodeType = "USE_YN";
        //            break;
        //        // 설비
        //        case CodeType.EQP:
        //            _CallCodeType = "EQP_CD_BY_USER";
        //            break;
        //        case CodeType.CHUTE_USED:
        //            _CallCodeType = "CHUTE_USE_CD";
        //            break;
        //        // 고객사
        //        case CodeType.CST_CD:
        //            _CallCodeType = "CST_CD";
        //            break;
        //        // 거래처
        //        case CodeType.BIZPTNR_CD:
        //            _CallCodeType = "BIZPTNR_CD";
        //            break;

        //        ///WAVE NO
        //        case CodeType.WAVE_NO:
        //            _CallCodeType = "WAVE_NO";
        //            break;
        //        case CodeType.SYSTEM:
        //            _CallCodeType = "SYSTEM_CD";
        //            break;
        //        default:
        //            _CallCodeType = CommonCodeType.ToWhiteSpaceOrString();
        //            break;
        //    }

        //    if (_CallCodeType != null)
        //    {
        //        GetList(_CallCodeType, attr1, attr2, attr3, attr4);
        //    }
        //}


        //public void GetList()
        //{
        //    if (CommonProperties.GetIsDesignTime())
        //    {
        //        return;
        //    }

        //    string _CallCodeType = null;
        //    string strAttribute1 = string.Empty;
        //    string strAttribute2 = string.Empty;
        //    string strAttribute3 = string.Empty;
        //    string strAttribute4 = string.Empty;

        //    var _CenterCd = (Application.Current as BaseApp).BASE_INFO.CENTER_CD;

        //    switch (CommonCodeType)
        //    {
        //        // 사용여부
        //        case CodeType.USE_YN:
        //            _CallCodeType = "USE_YN";
        //            break;
        //        // 설비
        //        case CodeType.EQP:
        //            _CallCodeType = "EQP_CD_BY_USER";

        //            strAttribute3 = (Application.Current as BaseApp).BASE_INFO.USER_ID;
        //            break;
        //        case CodeType.CHUTE_USED:
        //            _CallCodeType = "CHUTE_USE_CD";
        //            break;
        //        // 고객사
        //        case CodeType.CST_CD:
        //            _CallCodeType = "CST_CD";
        //            strAttribute1 = _CenterCd.ToString();
        //            break;
        //        // 거래처
        //        case CodeType.BIZPTNR_CD:
        //            _CallCodeType = "BIZPTNR_CD";
        //            strAttribute1 = _CenterCd.ToString();
        //            break;
        //        case CodeType.WORK_STAT_CD:
        //            _CallCodeType = "WORK_STAT_CD";
        //            strAttribute1 = _CenterCd.ToString();
        //            break;
        //        case CodeType.WAVE_NO:
        //            throw new Exception("Attribute1에는 센터코드 Attribute2에는 조회 날짜를 입력");
        //        case CodeType.SYSTEM:
        //            _CallCodeType = "SYSTEM_CD";
        //            break;
        //        default:
        //            _CallCodeType = CommonCodeType.ToString();
        //            break;
        //    }

        //    if (_CallCodeType != null)
        //    {
        //        //if (_CallCodeType == "EQP_CD_BY_USER" && strAttribute1 == "SMS")
        //        //{
        //        //    strAttribute1 = "SRT";
        //        //}





        //        using (BaseDataAccess da = new BaseDataAccess())
        //        {
        //            //this.AddRange(da.GetCombo)
        //        }

        //        //// 2019-09-04
        //        //// 추성호
        //        //// DB 종류에 따른 접속 구문 변경해야함.
        //        //using (DataAccess da = new DataAccess())
        //        //{
        //        //   this.AddRange(da.GetComboBoxListItem(
        //        //                                _CallCodeType
        //        //                            , strAttribute1
        //        //                            , strAttribute2
        //        //                            , strAttribute3
        //        //                            , IsAllTypeCodeAdd
        //        //                            , "ALL"
        //        //                            , (Application.Current as BaseApp).BASE_INFO.country_cd
        //        //                            , _CenterCd
        //        //                        ));
        //        //}
        //    }
        //}

        //public void GetList(string callType, string attr1, string attr2, string attr3, string attr4)
        //{
        //    if (CommonProperties.GetIsDesignTime())
        //    {
        //        return;
        //    }

        //    string _CallCodeType = callType;
        //    string strAttribute1 = attr1;
        //    string strAttribute2 = attr2;
        //    string strAttribute3 = attr3;
        //    string strAttribute4 = attr4;
        //    var _CenterCd = (Application.Current as BaseApp).BASE_INFO.CENTER_CD;

        //    if (_CallCodeType != null)
        //    {
        //        // CHOO
        //        // 2019-09-04
        //        // DB 종류에 따라 구분하여 DB 접속 코딩해야함.
        //        ////using (DataAccess da = new DataAccess())
        //        ////{
        //        ////    this.BaseInfo = ((SMART.WCS.Control.BaseApp)System.Windows.Application.Current).BASE_INFO;

        //        ////    // 2019.05.28
        //        ////    // 추성호
        //        ////    // 센터별 테그값 설정
        //        ////    string strCenterCD      = this.BaseInfo.CENTER_CD;

        //        ////    string strConnectType   = string.Empty;
        //        ////    string strConnectTag    = string.Empty;

        //        ////    ////if (this.BaseInfo.db_connect_type != null)
        //        ////    ////{
        //        ////    ////    strConnectType = this.BaseInfo.db_connect_type;
        //        ////    ////}

        //        ////    ////this.BaseClass.Info("strConnectType : " + strConnectType);

        //        ////    ////if (strConnectType.ToUpper().Equals("DEV") == true || strConnectType.ToUpper().Equals("TEST") == true)
        //        ////    ////{
        //        ////    ////    if (strCenterCD.ToUpper().Equals("OY") == true)
        //        ////    ////    {
        //        ////    ////        strConnectTag = string.Format("CJFC_{0}_DEV", strCenterCD);
        //        ////    ////    }
        //        ////    ////    else
        //        ////    ////    {
        //        ////    ////        strConnectTag = string.Format("CJFC_{0}_TEST", strCenterCD);
        //        ////    ////    }
        //        ////    ////}
        //        ////    ////else
        //        ////    ////{
        //        ////    ////    strConnectTag = string.Format("CJFC_{0}_REAL", strCenterCD);
        //        ////    ////}

        //        ////    ////// 2019.05.28
        //        ////    ////// 추성호
        //        ////    ////// Parameter 추가 (strConnectTag)
        //        ////    ////// 현재 화면 (XAML)에서 콤보박스 설정 시 개발, 운영 접속할 수 없기 때문에
        //        ////    ////// 접속 테그를 Parameter로 넘기는 구문 추가
        //        ////    ////this.AddRange(da.GetComboBoxListItem(
        //        ////    ////                             _CallCodeType
        //        ////    ////                         ,  strAttribute1
        //        ////    ////                         ,  strAttribute2
        //        ////    ////                         ,  strAttribute3
        //        ////    ////                         ,  IsAllTypeCodeAdd
        //        ////    ////                         ,  "ALL"
        //        ////    ////                         ,  (Application.Current as BaseApp).BASE_INFO.country_cd
        //        ////    ////                         ,  _CenterCd
        //        ////    ////                         , strConnectTag
        //        ////    ////                     ));
        //        ////}
        //    }
        //}


    }


    //public enum CodeType
    //{
    //    /// <summary>
    //    /// 사용여부
    //    /// </summary>
    //    USE_YN,

    //    /// <summary>
    //    /// 설비
    //    /// </summary>
    //    EQP,

    //    /// <summary>
    //    /// SYSTEM리스트 (CNV,QPS,SMS,WCS)
    //    /// </summary>
    //    SYSTEM,
    //    /// <summary>
    //    /// 슈트 사용용도
    //    /// </summary>
    //    CHUTE_USED,

    //    /// <summary>
    //    /// 공통 코드 사용
    //    /// </summary>
    //    COMM_CD,

    //    /// <summary>
    //    /// 고객사 코드
    //    /// </summary>
    //    CST_CD,

    //    /// <summary>
    //    /// 거래처 코드
    //    /// </summary>
    //    BIZPTNR_CD,

    //    /// <summary>
    //    /// 작업 상태 코드
    //    /// </summary>
    //    WORK_STAT_CD,

    //    /// <summary>
    //    /// 박스 타입 코드
    //    /// ATT1은 박스그룹타입
    //    /// </summary>
    //    BOX_TYPE_CD,

    //    /// <summary>
    //    /// 권한 코드
    //    /// ATT1은 센터코드
    //    /// </summary>
    //    ROLE_CD,

    //    /// <summary>
    //    /// WAVE NO (올리브영)
    //    /// </summary>
    //    WAVE_NO,

    //    /// <summary>
    //    /// 업무공통옵션 (올리브영)
    //    /// 호출시 모두 기본값으로
    //    /// </summary>
    //    HDR_OPTION_CD,


    //    /// <summary>
    //    /// 피킹유형(올리브영)
    //    /// </summary>
    //    PICK_TYPE_CD
    //}
}
