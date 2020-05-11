using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using SMART.WCS.Common;
using SMART.WCS.Common.Data;
using SMART.WCS.Common.DataBase;
using SMART.WCS.UI.Plan.DataMembers.P2001_ANV;
using SMART.WCS.UI.Plan.DataMembers.P2001_ANV_01P;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SMART.WCS.UI.Plan.Views.ANL_VOLUME
{
    /// <summary>
    /// P2001_ANV_01P.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class P2001_ANV_01P : Window, IDisposable
    {
        #region ▩ 전역변수
        /// <summary>
        /// Base클래스 선언
        /// </summary>
        BaseClass BaseClass = new BaseClass();

        #endregion

        #region ▩ 데이터 바인딩 용 객체 선언 및 속성 정의

        #region >> ViewTitle - 팝업 타이틀
        public static readonly DependencyProperty _ViewTitle
            = DependencyProperty.Register("ViewTitle", typeof(string), typeof(P2001_ANV_01P)
                , new PropertyMetadata(string.Empty));

        /// <summary>
        /// 팝업 타이틀
        /// </summary>
        public string ViewTitle
        {
            get { return (string)GetValue(_ViewTitle); }
            set { SetValue(_ViewTitle, value); }
        }
        #endregion

        #region 추출 항목 리스트

        #region >> BaseAnAggTypeList - 기본 추출 항목 리스트
        public static readonly DependencyProperty _BaseAnAggTypeList
            = DependencyProperty.Register("BaseAnAggTypeList", typeof(ObservableCollection<AnAggTypeInfo>), typeof(P2001_ANV_01P)
                , new PropertyMetadata(new ObservableCollection<AnAggTypeInfo>()));

        /// <summary>
        /// 추출 항목 리스트
        /// </summary>
        public ObservableCollection<AnAggTypeInfo> BaseAnAggTypeList
        {
            get { return (ObservableCollection<AnAggTypeInfo>)GetValue(_BaseAnAggTypeList); }
            set { SetValue(_BaseAnAggTypeList, value); }
        }
        #endregion

        #region >> SaveAnAggTypeList - 저장된 추출 항목 리스트
        public static readonly DependencyProperty _SaveAnAggTypeList
            = DependencyProperty.Register("SaveAnAggTypeList", typeof(ObservableCollection<AnAggTypeInfo>), typeof(P2001_ANV_01P)
                , new PropertyMetadata(new ObservableCollection<AnAggTypeInfo>()));

        /// <summary>
        /// 저장된(선택된) 추출항목 리스트
        /// </summary>
        public ObservableCollection<AnAggTypeInfo> SaveAnAggTypeList
        {
            get { return (ObservableCollection<AnAggTypeInfo>)GetValue(_SaveAnAggTypeList); }
            set { SetValue(_SaveAnAggTypeList, value); }
        }
        #endregion

        #region >> AggTypeAbcList - 상세 추출 항목 리스트 - ABC
        public static readonly DependencyProperty _AggTypeAbcList
            = DependencyProperty.Register("AggTypeAbcList", typeof(ObservableCollection<AnAbcDtlInfo>), typeof(P2001_ANV_01P)
                , new PropertyMetadata(new ObservableCollection<AnAbcDtlInfo>()));

        public ObservableCollection<AnAbcDtlInfo> AggTypeAbcList
        {
            get { return (ObservableCollection<AnAbcDtlInfo>)GetValue(_AggTypeAbcList); }
            set { SetValue(_AggTypeAbcList, value); }
        }
        #endregion

        #endregion 추출 항목 리스트

        #region 추출 리스트
        #region >> ObExtCapaList - CAPA 추출 리스트
        public static readonly DependencyProperty _ObExtCapaList
            = DependencyProperty.Register("ObExtCapaList", typeof(ObservableCollection<ObExtCapaInfo>), typeof(P2001_ANV_01P)
                , new PropertyMetadata(new ObservableCollection<ObExtCapaInfo>()));

        public ObservableCollection<ObExtCapaInfo> ObExtCapaList
        {
            get { return (ObservableCollection<ObExtCapaInfo>)GetValue(_ObExtCapaList); }
            set { SetValue(_ObExtCapaList, value); }
        }
        #endregion

        #region >> ObExtEiqList - EIQ 추출 리스트
        public static readonly DependencyProperty _ObExtEiqList
            = DependencyProperty.Register("ObExtEiqList", typeof(ObservableCollection<ObExtEiqInfo>), typeof(P2001_ANV_01P)
                , new PropertyMetadata(new ObservableCollection<ObExtEiqInfo>()));

        public ObservableCollection<ObExtEiqInfo> ObExtEiqList
        {
            get { return (ObservableCollection<ObExtEiqInfo>)GetValue(_ObExtEiqList); }
            set { SetValue(_ObExtEiqList, value); }
        }
        #endregion

        #region >> ObExtCapaList - ABC 추출 리스트
        public static readonly DependencyProperty _ObExtAbcList
            = DependencyProperty.Register("ObExtAbcList", typeof(ObservableCollection<ObExtAbcInfo>), typeof(P2001_ANV_01P)
                , new PropertyMetadata(new ObservableCollection<ObExtAbcInfo>()));

        public ObservableCollection<ObExtAbcInfo> ObExtAbcList
        {
            get { return (ObservableCollection<ObExtAbcInfo>)GetValue(_ObExtAbcList); }
            set { SetValue(_ObExtAbcList, value); }
        }
        #endregion

        #region >> ObExtCapaList - ABC 상세 리스트
        public static readonly DependencyProperty _ObExtAbcDetailList
            = DependencyProperty.Register("ObExtAbcDetailList", typeof(ObservableCollection<ObExtAbcDetailInfo>), typeof(P2001_ANV_01P)
                , new PropertyMetadata(new ObservableCollection<ObExtAbcDetailInfo>()));

        public ObservableCollection<ObExtAbcDetailInfo> ObExtAbcDetailList
        {
            get { return (ObservableCollection<ObExtAbcDetailInfo>)GetValue(_ObExtAbcDetailList); }
            set { SetValue(_ObExtAbcDetailList, value); }
        }
        #endregion

        #endregion 추출 리스트

        #region >> DateMaskText - 
        public static readonly DependencyProperty _DateMaskText
            = DependencyProperty.Register("DateMaskText", typeof(string), typeof(P2001_ANV_01P)
                , new PropertyMetadata(string.Empty));

        /// <summary>
        /// 추출일자 DisplayFormatString
        /// </summary>
        public string DateMaskText
        {
            get { return (string)GetValue(_DateMaskText); }
            set { SetValue(_DateMaskText, value); }
        }
        #endregion

        #region >> ExtITEM - 추출항목
        public static readonly DependencyProperty _ExtITEM
            = DependencyProperty.Register("ExtITEM", typeof(AnRlstInfo), typeof(P2001_ANV_01P)
                , new PropertyMetadata(new AnRlstInfo()));

        /// <summary>
        /// 추출항목
        /// </summary>
        public AnRlstInfo ExtITEM
        {
            get { return (AnRlstInfo)GetValue(_ExtITEM); }
            set { SetValue(_ExtITEM, value); }
        }
        #endregion

        // 신규 추출 여부
        bool IsNewExt = false;

        // 추출 작업 여부 (데이터 추출 완료되었습니다. 메세지 표시 여부)
        bool IsDataExtract = false;

        string cCompanyCd = string.Empty;

        #endregion

        #region ▩ 생성자
        /// <summary>
        /// 생성자 (기본) -신규
        /// </summary>
        public P2001_ANV_01P(string pCoCd, string pCoNm)
        {
            InitializeComponent();

            cCompanyCd = pCoCd;

            if(string.IsNullOrEmpty(cCompanyCd))
            {
                this.BaseClass.MsgError("회사 코드 오류입니다.");
            }

            this.ExtITEM = new AnRlstInfo();

            //this.IsNewExt = true;
            //this.ExtITEM.EXT_ID = 1100;
            GetExtIdSearch();

            this.ExtITEM.CO_CD = cCompanyCd;

            ViewTitle = string.Format("물동량 분석결과 ({0} - {1})", pCoNm, this.ExtITEM.EXT_ID);

            // 컨트롤 관련 초기화
            this.InitControl();

            InitEvent();
        }

        /// <summary>
        /// 생성자 (AnRlstInfo) - 추출결과 확인
        /// </summary>
        /// <param name="pInfo"></param>
        public P2001_ANV_01P(AnRlstInfo pInfo)
        {
            InitializeComponent();

            this.ExtITEM = new AnRlstInfo();

            this.ExtITEM = pInfo;

            this.cCompanyCd = pInfo.CO_CD;

            ViewTitle = string.Format("물동량 분석결과 ({0} - {1})", pInfo.CO_NM, this.ExtITEM.EXT_ID);

            // 컨트롤 관련 초기화
            this.InitControl();

            InitEvent();
        }
        #endregion ▩ 생성자

        #region ▩ 함수

        #region 초기화 부분
        #region InitControl - 컨트롤 초기화한다.
        private void InitControl()
        {
            AggTypeAbcList = new ObservableCollection<AnAbcDtlInfo>();
            BaseAnAggTypeList = new ObservableCollection<AnAggTypeInfo>();
            SaveAnAggTypeList = new ObservableCollection<AnAggTypeInfo>();

            if (!IsNewExt)
            {
                this.listExtTarCd.IsReadOnly = true;
                this.listExtTypeCd.IsReadOnly = true;
            }

            if (this.ExtITEM.EXT_COMP_YN.Equals("Y"))
            {
                this.btnExtract.IsEnabled = false;
            }
            else if (this.IsNewExt)
            {
                this.btnExtract.IsEnabled = false;
            }
            else
            {
                this.btnExtract.IsEnabled = true;
            }

            IsDataExtract = false;
        }

        #endregion

        #region InitEvent - 이벤트 초기화한다.
        /// <summary>
        /// 이벤트 초기화한다.
        /// </summary>
        private void InitEvent()
        {
            try
            {
                //#region 폼 이벤트
                this.Loaded += P2001_ANV_01P_Loaded; ;

                #region 버튼 관련 이벤트
                //화면 상단 X버튼 클릭 이벤트
                this.btnFormClose.Click += BtnFormClose_Click;

                // 엑셀 다운로드 버튼
                this.btnExcelDownload.PreviewMouseLeftButtonUp += BtnExcelDownload_PreviewMouseLeftButtonUp;

                // 저장
                this.btnSave.PreviewMouseLeftButtonUp += BtnSave_PreviewMouseLeftButtonUp;

                // 추출
                this.btnExtract.PreviewMouseLeftButtonUp += BtnExtract_PreviewMouseLeftButtonUp;
                #endregion

                #region 그리드 이동 버튼 관련 이벤트
                btnLeftToRight.PreviewMouseLeftButtonUp += BtnLeftToRight_PreviewMouseLeftButtonUp;
                btnRightToLeft.PreviewMouseLeftButtonUp += BtnRightToLeft_PreviewMouseLeftButtonUp;
                btnBottomToTop.PreviewMouseLeftButtonUp += BtnBottomToTop_PreviewMouseLeftButtonUp;
                btnTopToBottom.PreviewMouseLeftButtonUp += BtnTopToBottom_PreviewMouseLeftButtonUp;
                #endregion

                this.cboABCCnt.SelectedIndexChanged += CboABCCnt_SelectedIndexChanged;

                this.KeyDown += P2001_ANV_01P_KeyDown;

            }
            catch { throw; }
        }

        #endregion
        #endregion

        #region 이벤트 메서드
        private void P2001_ANV_01P_Loaded(object sender, RoutedEventArgs e)
        {
            #region 설명
            //if (!string.IsNullOrEmpty(this.ExtITEM.COMMENTS))
            //{
            //    this.txtComment.Text = this.ExtITEM.COMMENTS;    // 설명
            //}
            #endregion

            #region 구분
            if (this.ExtITEM.EXT_TAR_CD.Equals("01"))
            {
                this.listExtTarCd.SelectedIndex = 0;    // 출고
            }
            else if (this.ExtITEM.EXT_TAR_CD.Equals("02"))
            {
                this.listExtTarCd.SelectedIndex = 1;    // 입고
            }
            else if (this.ExtITEM.EXT_TAR_CD.Equals("03"))
            {
                this.listExtTarCd.SelectedIndex = 2;    // 재고
            }
            else
            {
                this.listExtTarCd.SelectedIndex = 0;    // 출고
            }
            #endregion

            #region 추출타입
            if (this.ExtITEM.EXT_TYPE_CD.Equals("01"))
            {
                this.listExtTypeCd.SelectedIndex = 0;    // CAPA
            }
            else if (this.ExtITEM.EXT_TYPE_CD.Equals("02"))
            {
                this.listExtTypeCd.SelectedIndex = 1;    // EIQ
            }
            else if (this.ExtITEM.EXT_TYPE_CD.Equals("03"))
            {
                this.listExtTypeCd.SelectedIndex = 2;    // ABC
            }
            else
            {
                this.listExtTypeCd.SelectedIndex = 0;    // CAPA
            }
            #endregion

            #region 추출일자
            this.cboExtTimeFr.Visibility = Visibility.Collapsed;
            this.cboExtTimeTo.Visibility = Visibility.Collapsed;

            if (this.ExtITEM.EXT_DATE_TYPE_CD.Equals("ALL"))
            {
                this.listExtDateTypeCd.SelectedIndex = 0;    // ALL
                this.DateMaskText = "yyyy/MM/dd";
            }
            else if (this.ExtITEM.EXT_DATE_TYPE_CD.Equals("YYYY"))
            {
                this.listExtDateTypeCd.SelectedIndex = 1;    // 년
                this.DateMaskText = "yyyy";
            }
            else if (this.ExtITEM.EXT_DATE_TYPE_CD.Equals("YYYYMM"))
            {
                this.listExtDateTypeCd.SelectedIndex = 2;    // 월
                this.DateMaskText = "yyyy/MM";
            }
            else if (this.ExtITEM.EXT_DATE_TYPE_CD.Equals("YYYYMMDD"))
            {
                this.listExtDateTypeCd.SelectedIndex = 3;    // 일
                this.DateMaskText = "yyyy/MM/dd";
            }
            else if (this.ExtITEM.EXT_DATE_TYPE_CD.Equals("YYYYMMDDHH"))
            {
                this.listExtDateTypeCd.SelectedIndex = 4;    // 시간
                this.DateMaskText = "yyyy/MM/dd";
            }
            else
            {
                this.listExtDateTypeCd.SelectedIndex = 2;    // 월
                this.DateMaskText = "yyyy/MM";
            }

            if (!IsNewExt)
            {
                deExtFr.DateTime = GetFromDateTime(this.ExtITEM.EXT_FROM);
                deExtTo.DateTime = GetToDateTime(this.ExtITEM.EXT_TO);
            }

            List<ComboBoxInfo> ComboBoxTimeInfo = new List<ComboBoxInfo>();

            for (int i = 0; i < 24; i++)
            {
                ComboBoxTimeInfo.Add(new ComboBoxInfo { CODE = i.ToString("D2"), NAME = i.ToString("D2") });
            }

            cboExtTimeFr.ItemsSource = ComboBoxTimeInfo;
            cboExtTimeTo.ItemsSource = ComboBoxTimeInfo;

            cboExtTimeFr.SelectedIndex = 0;
            cboExtTimeTo.SelectedIndex = 23;

            if (this.ExtITEM.EXT_DATE_TYPE_CD.Equals("YYYYMMDDHH"))
            {
                if (this.ExtITEM.EXT_FROM.Length == 10)
                {
                    cboExtTimeFr.SelectedIndex = int.Parse(this.ExtITEM.EXT_FROM.Substring(8, 2));
                }

                if (this.ExtITEM.EXT_TO.Length == 10)
                {
                    cboExtTimeTo.SelectedIndex = int.Parse(this.ExtITEM.EXT_TO.Substring(8, 2));
                }
            }

            #endregion

            #region 추출항목

            #region Base

            #endregion

            #region ABC
            if (this.ExtITEM.ABC_TYPE_CD.Equals("01"))
            {
                this.listAbcTypeCd.SelectedIndex = 0;   // 빈도
            }
            else if (this.ExtITEM.ABC_TYPE_CD.Equals("02"))
            {
                this.listAbcTypeCd.SelectedIndex = 1;   // 수량
            }
            else
            {
                this.listAbcTypeCd.SelectedIndex = 0;   // 빈도
            }

            List<ComboBoxInfo> ComboBoxABCCntInfo = new List<ComboBoxInfo>();

            for (int i = 2; i < 5; i++)
            {
                ComboBoxABCCntInfo.Add(new ComboBoxInfo { CODE = i.ToString(), NAME = i.ToString() });
            }

            cboABCCnt.ItemsSource = ComboBoxABCCntInfo;

            if (this.ExtITEM.ABC_CODE_CNT.Equals(2))
            {
                this.cboABCCnt.SelectedIndex = 0;
            }
            else if (this.ExtITEM.ABC_CODE_CNT.Equals(3))
            {
                this.cboABCCnt.SelectedIndex = 1;
            }
            else if (this.ExtITEM.ABC_CODE_CNT.Equals(4))
            {
                this.cboABCCnt.SelectedIndex = 2;
            }
            else
            {
                this.cboABCCnt.SelectedIndex = 1;
            }

            #endregion

            #endregion

            AnAggTypeSearch();
            AnDtlRlstSearch();       
        }

        #region +  엑셀 다운로드 버튼 클릭 이벤트
        /// <summary>
        ///  엑셀 다운로드 버튼 클릭 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExcelDownload_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var strMessage = this.BaseClass.GetResourceValue("ASK_EXCEL_DOWNLOAD", BaseEnumClass.ResourceType.MESSAGE);

                this.BaseClass.MsgQuestion(strMessage, BaseEnumClass.CodeMessage.MESSAGE);
                if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                {
                    // 상태바 (아이콘) 실행
                    this.loadingScreen.IsSplashScreenShown = true;

                    List<TableView> tv = new List<TableView>();

                    switch (listExtTypeCd.SelectedIndex)
                    {
                        case 0:
                            tv.Add(this.tvObExtCapa);
                            break;
                        case 1:
                            tv.Add(this.tvObExtEiq);
                            break;
                        case 2:
                            tv.Add(this.tvObExtAbc);
                            tv.Add(this.tvObExtAbcDetail);
                            break;
                        default:
                            break;
                    }

                    this.BaseClass.GetExcelDownload(tv);
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                // 상태바 (아이콘) 제거
                this.loadingScreen.IsSplashScreenShown = false;
            }
        }
        #endregion

        /// <summary>
        /// 추출버튼 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExtract_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            IsNewExt = false;
            IsDataExtract = true;
            this.ExtITEM.EXT_COMP_YN = "Y"; // 추출처리

            MakeExtract();
        }

        private void listExtTarCd_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            this.ExtITEM.EXT_TAR_CD = (this.listExtTarCd.SelectedIndex + 1).ToString("D2");
        }

        private void listExtTypeCd_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            switch (listExtTypeCd.SelectedIndex)
            {
                case 0:
                    this.ExtITEM.EXT_TYPE_CD = "01";
                    ObExtCapaSearch();
                    break;
                case 1:
                    this.ExtITEM.EXT_TYPE_CD = "02";
                    ObExtEiqSearch();
                    break;
                case 2:
                    this.ExtITEM.EXT_TYPE_CD = "03";
                    ObExtAbcSearch();
                    break;
                default:
                    break;
            }
        }

        private void listExtDateTypeCd_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            this.cboExtTimeFr.Visibility = Visibility.Collapsed;
            this.cboExtTimeTo.Visibility = Visibility.Collapsed;

            switch (listExtDateTypeCd.SelectedIndex)
            {
                case 0:
                    this.DateMaskText = "yyyy/MM/dd";    // ALL
                    this.ExtITEM.EXT_DATE_TYPE_CD = "ALL";
                    this.cboExtTimeFr.Visibility = Visibility.Visible;
                    this.cboExtTimeTo.Visibility = Visibility.Visible;
                    break;
                case 1:
                    this.DateMaskText = "yyyy";     // 년
                    this.ExtITEM.EXT_DATE_TYPE_CD = "YYYY";
                    break;
                case 2:
                    this.DateMaskText = "yyyy/MM";  // 월
                    this.ExtITEM.EXT_DATE_TYPE_CD = "YYYYMM";
                    break;
                case 3:
                    this.DateMaskText = "yyyy/MM/dd";   // 일
                    this.ExtITEM.EXT_DATE_TYPE_CD = "YYYYMMDD";
                    break;
                case 4:
                    this.DateMaskText = "yyyy/MM/dd";    // 시간
                    this.ExtITEM.EXT_DATE_TYPE_CD = "YYYYMMDDHH";
                    this.cboExtTimeFr.Visibility = Visibility.Visible;
                    this.cboExtTimeTo.Visibility = Visibility.Visible;
                    break;
                default:
                    this.DateMaskText = string.Empty;
                    break;
            }
        }

        /// <summary>
        /// ABC수 콤보박스 변경 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CboABCCnt_SelectedIndexChanged(object sender, RoutedEventArgs e)
        {
            // 번경적용할지 물어보는 팝업 창
            {
                if (cboABCCnt.SelectedIndex + 2 == AggTypeAbcList.Count)
                {
                    return;
                }
                else
                {
                    bool SetAbcList = true;

                    if (AggTypeAbcList.Count > 0)
                    {
                        var strMessage = "변경시 초기화 됩니다.|계속 변경하시겠습니까?";

                        this.BaseClass.MsgQuestion(strMessage, BaseEnumClass.CodeMessage.MESSAGE);

                        if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                        {
                            SetAbcList = true;
                        }
                        else
                        {
                            SetAbcList = false;
                        }
                    }

                    if (SetAbcList)
                    {
                        AggTypeAbcList = new ObservableCollection<AnAbcDtlInfo>();

                        AggTypeAbcList.Add(new AnAbcDtlInfo { ABC_CODE = "A", FROM_PER_RANGE = 0, TO_PER_RANGE = 0 });

                        switch (cboABCCnt.SelectedIndex)
                        {
                            case 0:
                                AggTypeAbcList.Add(new AnAbcDtlInfo { ABC_CODE = "B", FROM_PER_RANGE = 0, TO_PER_RANGE = 100 });
                                break;
                            case 1:
                                AggTypeAbcList.Add(new AnAbcDtlInfo { ABC_CODE = "B", FROM_PER_RANGE = 0, TO_PER_RANGE = 0 });
                                AggTypeAbcList.Add(new AnAbcDtlInfo { ABC_CODE = "C", FROM_PER_RANGE = 0, TO_PER_RANGE = 100 });
                                break;
                            case 2:
                                AggTypeAbcList.Add(new AnAbcDtlInfo { ABC_CODE = "B", FROM_PER_RANGE = 0, TO_PER_RANGE = 0 });
                                AggTypeAbcList.Add(new AnAbcDtlInfo { ABC_CODE = "C", FROM_PER_RANGE = 0, TO_PER_RANGE = 0 });
                                AggTypeAbcList.Add(new AnAbcDtlInfo { ABC_CODE = "D", FROM_PER_RANGE = 0, TO_PER_RANGE = 100 });
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        // 선택 취소 (원복)
                        cboABCCnt.SelectedIndex = AggTypeAbcList.Count - 2;
                    }
                }
            }
        }

        private void tvDetailABCGrid_CellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            // FROM_PER_RANGE
            // TO_PER_RANGE
            if (e.Column.FieldName.Equals("TO_PER_RANGE"))
            {
                AnAbcDtlInfo item = e.Row as AnAbcDtlInfo;

                int AbcTypeCount = AggTypeAbcList.Count();
                if (AbcTypeCount == 2)
                {
                    if (item.ABC_CODE.Equals("A"))
                    {
                        (AggTypeAbcList.Where(w => w.ABC_CODE.Equals("B")).FirstOrDefault()).FROM_PER_RANGE = item.TO_PER_RANGE;
                    }
                    else
                    {
                        return;
                    }
                }
                else if (AbcTypeCount == 3)
                {
                    if (item.ABC_CODE.Equals("A"))
                    {
                        (AggTypeAbcList.Where(w => w.ABC_CODE.Equals("B")).FirstOrDefault()).FROM_PER_RANGE = item.TO_PER_RANGE;
                    }
                    else if (item.ABC_CODE.Equals("B"))
                    {
                        (AggTypeAbcList.Where(w => w.ABC_CODE.Equals("C")).FirstOrDefault()).FROM_PER_RANGE = item.TO_PER_RANGE;
                    }
                    else
                    {
                        return;
                    }

                }
                else if (AbcTypeCount == 4)
                {
                    if (item.ABC_CODE.Equals("A"))
                    {
                        (AggTypeAbcList.Where(w => w.ABC_CODE.Equals("B")).FirstOrDefault()).FROM_PER_RANGE = item.TO_PER_RANGE;
                    }
                    else if (item.ABC_CODE.Equals("B"))
                    {
                        (AggTypeAbcList.Where(w => w.ABC_CODE.Equals("C")).FirstOrDefault()).FROM_PER_RANGE = item.TO_PER_RANGE;
                    }
                    else if (item.ABC_CODE.Equals("C"))
                    {
                        (AggTypeAbcList.Where(w => w.ABC_CODE.Equals("D")).FirstOrDefault()).FROM_PER_RANGE = item.TO_PER_RANGE;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }
        
        #region 그리드 이동 버튼 관련 이벤트
        private void BtnRightToLeft_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.SaveAnAggTypeList.Count == 0 || this.gridDetail1.SelectedItem == null)
            {
                return;
            }

            var moveItem = this.gridDetail1.SelectedItem as AnAggTypeInfo;

            SaveAnAggTypeList.Remove(moveItem);
            BaseAnAggTypeList.Add(moveItem);
        }

        private void BtnLeftToRight_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (this.BaseAnAggTypeList.Count == 0 || this.gridDetail.SelectedItem == null)
            {
                return;
            }

            var moveItem = this.gridDetail.SelectedItem as AnAggTypeInfo;

            BaseAnAggTypeList.Remove(moveItem);
            SaveAnAggTypeList.Add(moveItem);
        }

        private void BtnTopToBottom_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.gridDetail1.Focus();

            int index = this.gridDetail1.View.FocusedRowHandle;
            if (index >= this.SaveAnAggTypeList.Count - 1 || index < 0)
            {
                return;
            }

            var moveItem = SaveAnAggTypeList[index];

            this.gridDetail1.SelectedItem = moveItem;

            SaveAnAggTypeList.Remove(moveItem);
            SaveAnAggTypeList.Insert(index + 1, moveItem);

            this.gridDetail1.View.FocusedRowHandle = index + 1;

            this.gridDetail1.SelectedItem = moveItem;
        }

        private void BtnBottomToTop_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.gridDetail1.Focus();

            int index = this.gridDetail1.View.FocusedRowHandle;
            if (index <= 0) return;

            var moveItem = SaveAnAggTypeList[index];

            this.gridDetail1.SelectedItem = moveItem;

            SaveAnAggTypeList.Remove(moveItem);
            SaveAnAggTypeList.Insert(index - 1, moveItem);

            this.gridDetail1.View.FocusedRowHandle = index - 1;

            this.gridDetail1.SelectedItem = moveItem;
        }
        #endregion

        #region OnDragRecordOver
        private void OnDragRecordOver(object sender, DragRecordOverEventArgs e)
        {
            if (e.IsFromOutside && typeof(AnAggTypeInfo).IsAssignableFrom(e.GetRecordType()))
            {
                e.Effects = DragDropEffects.Move;
                e.Handled = true;
            }
        }
        #endregion

        #region BtnSave_PreviewMouseLeftButtonUp
        private void BtnSave_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                bool isRtnValue = false;

                this.BaseClass.MsgQuestion("ASK_CHANGE_DATA_SAVE");
                if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                {
                    using (BaseDataAccess da = new BaseDataAccess())
                    {
                        try
                        {
                            // 상태바 (아이콘) 실행
                            this.loadingScreen.IsSplashScreenShown = true;

                            da.BeginTransaction();

                            isRtnValue = this.SetSP_AN_EXT_COND_SAVE(da);

                            if (isRtnValue == true)
                            {
                                // 저장된 경우
                                da.CommitTransaction();

                                // 상태바 (아이콘) 제거
                                this.loadingScreen.IsSplashScreenShown = false;

                                var strMessage = this.BaseClass.GetResourceValue("ASK_DATA_EXTRACT", BaseEnumClass.ResourceType.MESSAGE);

                                this.BaseClass.MsgQuestion(strMessage, BaseEnumClass.CodeMessage.MESSAGE);

                                if (this.BaseClass.BUTTON_CONFIRM_YN == true)
                                {
                                    IsNewExt = false;
                                    IsDataExtract = true;
                                    this.ExtITEM.EXT_COMP_YN = "Y"; // 추출처리
                                    MakeExtract();
                                }
                                else
                                {
                                    // 저장되었습니다.
                                    BaseClass.MsgInfo("CMPT_SAVE");

                                    this.btnExtract.IsEnabled = true;
                                    this.ExtITEM.EXT_COMP_YN = "N"; // 추출처리
                                }
                            }
                            else
                            {
                                // 오류 발생하여 저장 실패한 경우
                                da.RollbackTransaction();
                            }
                        }
                        catch
                        {
                            if (da.TransactionState_Oracle == BaseEnumClass.TransactionState_ORACLE.TransactionStarted)
                            {
                                da.RollbackTransaction();
                            }

                            BaseClass.MsgError("ERR_SAVE");
                            throw;
                        }
                        finally
                        {
                            // 상태바 (아이콘) 제거
                            this.loadingScreen.IsSplashScreenShown = false;

                            // 재조회

                        }
                    }
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
        }

        #endregion

        private void P2001_ANV_01P_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.LeftCtrl))
            {
                this.btnExtract.IsEnabled = true;
                this.listExtTarCd.IsReadOnly = false;
                this.listExtTypeCd.IsReadOnly = false;
            }
        }

        #region Close 이벤트
        private void BtnFormClose_Click(object sender, RoutedEventArgs e)
        {
            this.CloseProcess();
        }
        #endregion

        #endregion

        #region 데이터 관련 메서드

        #region GetExtIdSearch
        /// <summary>
        /// 신규 ExtId 생성
        /// </summary>
        private void GetExtIdSearch()
        {
            this.IsNewExt = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_P2001_ANV.SP_AN_EXT_ID_CRT";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            //var strCoCd = this.BaseClass.CompanyCode;         // 회사 코드
            var strCoCd = this.cCompanyCd;         // 회사 코드

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCoCd);      // 회사 코드
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                dtRtnValue = dataAccess.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);
            }
            #endregion

            if (dtRtnValue != null && dtRtnValue.Rows.Count > 0 && dtRtnValue.Rows[0]["EXT_ID"].ToString().Length > 0)
            {
                this.ExtITEM.EXT_ID = int.Parse(dtRtnValue.Rows[0]["EXT_ID"].ToString());
            }
            else
            {
                this.ExtITEM.EXT_ID = 0;
            }
        }
        #endregion GetExtIdSearch

        #region GetSP_AN_DTL_RLST_SEARCH
        /// <summary>
        /// GetSP_AN_DTL_RLST_SEARCH
        /// </summary>
        /// <returns></returns>
        private DataSet GetSP_AN_DTL_RLST_SEARCH()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_P2001_ANV.SP_AN_DTL_RLST_SEARCH";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            //var strCoCd = this.BaseClass.CompanyCode;         // 회사 코드
            var strCoCd = this.cCompanyCd;         // 회사 코드
            var strExtId = this.ExtITEM.EXT_ID;        // 구분 코드
            var strExtTypeCd = this.ExtITEM.EXT_TYPE_CD;    // 추출타입 코드

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCoCd);                  // 회사 코드
            dicInputParam.Add("P_EXT_ID", strExtId);
            dicInputParam.Add("P_EXT_TYPE_CD", strExtTypeCd);
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
            }
            #endregion

            return dsRtnValue;
        }
        #endregion GetSP_AN_DTL_RLST_SEARCH

        #region GetSP_AN_AGG_TYPE_SEARCH
        /// <summary>
        /// GetSP_AN_AGG_TYPE_SEARCH
        /// </summary>
        /// <returns></returns>
        private DataSet GetSP_AN_AGG_TYPE_SEARCH()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_P2001_ANV.SP_AN_AGG_TYPE_SEARCH";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "O_LIST", "OUT_RESULT" };

            //var strCoCd = this.BaseClass.CompanyCode;         // 회사 코드
            var strCoCd = this.cCompanyCd;         // 회사 코드
            var strExtTarCd = ((ListBoxItem)listExtTarCd.SelectedItem).Tag.ToString();  // 코드

            var strErrCode = string.Empty;          // 오류 코드
            var strErrMsg = string.Empty;           // 오류 메세지
            #endregion

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCoCd);      // 회사 코드
            dicInputParam.Add("P_EXT_TAR_CD", strExtTarCd);      // 회사 코드
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
            }
            #endregion

            return dsRtnValue;
        }
        #endregion SP_AN_AGG_TYPE_SEARCH

        #region GetSP_OB_EXT_CAPA
        /// <summary>
        /// GetSP_OB_EXT_CAPA
        /// </summary>
        /// <returns></returns>
        private DataSet GetSP_OB_EXT_CAPA()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_P2001_ANV.SP_OB_EXT_CAPA";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "OUR_CUR" };
            #endregion

            //var strCoCd = this.BaseClass.CompanyCode;         // 회사 코드
            var strCoCd = this.cCompanyCd;         // 회사 코드

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCoCd);
            dicInputParam.Add("P_EXT_ID", this.ExtITEM.EXT_ID);
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                //await System.Threading.Tasks.Task.Run(() =>
                //{
                //    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                //}).ConfigureAwait(true);

                dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
            }
            #endregion

            return dsRtnValue;
        }
        #endregion GetSP_OB_EXT_CAPA

        #region GetSP_OB_EXT_EIQ
        /// <summary>
        /// GetSP_OB_EXT_EIQ
        /// </summary>
        /// <returns></returns>
        private DataSet GetSP_OB_EXT_EIQ()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_P2001_ANV.SP_OB_EXT_EIQ";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "OUR_CUR" };
            #endregion

            //var strCoCd = this.BaseClass.CompanyCode;         // 회사 코드
            var strCoCd = this.cCompanyCd;         // 회사 코드

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCoCd);
            dicInputParam.Add("P_EXT_ID", this.ExtITEM.EXT_ID);
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                //await System.Threading.Tasks.Task.Run(() =>
                //{
                //    dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
                //}).ConfigureAwait(true);

                dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
            }
            #endregion

            return dsRtnValue;
        }
        #endregion GetSP_OB_EXT_EIQ

        #region GetSP_OB_EXT_ABCAsync
        /// <summary>
        /// GetSP_OB_EXT_ABCAsync
        /// </summary>
        /// <returns></returns>
        private DataSet GetSP_OB_EXT_ABC()
        {
            #region 파라메터 변수 선언 및 값 할당
            DataSet dsRtnValue = null;
            var strProcedureName = "PK_P2001_ANV.SP_OB_EXT_ABC";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "OUR_CUR", "OUR_CUR1" };
            #endregion

            //var strCoCd = this.BaseClass.CompanyCode;         // 회사 코드
            var strCoCd = this.cCompanyCd;         // 회사 코드

            #region Input 파라메터
            dicInputParam.Add("P_CO_CD", strCoCd);
            dicInputParam.Add("P_EXT_ID", this.ExtITEM.EXT_ID);
            #endregion

            #region 데이터 조회
            using (BaseDataAccess dataAccess = new BaseDataAccess())
            {
                dsRtnValue = dataAccess.GetSpDataSet(strProcedureName, dicInputParam, arrOutputParam);
            }
            #endregion

            return dsRtnValue;
        }
        #endregion GetSP_OB_EXT_ABC

        #region SetSP_AN_EXT_COND_SAVE
        private bool SetSP_AN_EXT_COND_SAVE(BaseDataAccess da)
        {
            bool isRtnValue = true;

            #region 파라메터 변수 선언 및 값 할당
            DataTable dtRtnValue = null;
            var strProcedureName = "PK_P2001_ANV.SP_AN_EXT_COND_SAVE";
            Dictionary<string, object> dicInputParam = new Dictionary<string, object>();
            string[] arrOutputParam = { "OUT_RESULT" };

            //var strCoCd = this.BaseClass.CompanyCode;         // 회사 코드
            var strCoCd = this.cCompanyCd;         // 회사 코드
            var strExtId = this.ExtITEM.EXT_ID;
            var strExtTarCd = this.ExtITEM.EXT_TAR_CD;
            var strExtTypeCd = this.ExtITEM.EXT_TYPE_CD;
            var strExtDateTypeCd = this.ExtITEM.EXT_DATE_TYPE_CD;
            var strExtFrom = GetExtDate(this.BaseClass.GetCalendarValue(this.deExtFr) + this.BaseClass.ComboBoxSelectedKeyValue(this.cboExtTimeFr));
            var strExtTo = GetExtDate(this.BaseClass.GetCalendarValue(this.deExtTo) + this.BaseClass.ComboBoxSelectedKeyValue(this.cboExtTimeTo));
            var strAbcTypeCd = this.ExtITEM.EXT_TYPE_CD.Equals("03") ? (listAbcTypeCd.SelectedIndex + 1).ToString("D2") : string.Empty; //this.AbcTypeCd;
            int intAbcCnt = this.ExtITEM.EXT_TYPE_CD.Equals("03") ? int.Parse(this.BaseClass.ComboBoxSelectedKeyValue(this.cboABCCnt)) : 0;    //this.AbcCnt;
            var strAggTypeCd = MakeAggTypeCd();  // /*AGG_TYPE1,AGG_TYPE2*/
            var strAbcCd = MakeAbcCd();  /*A,0,10^B,10,90^C,90,100*/
            var strUserId = this.BaseClass.UserID;          // 사용자 ID
            var strComments = this.ExtITEM.COMMENTS;

            var strErrCode = string.Empty;                         // 오류 코드
            var strErrMsg = string.Empty;                          // 오류 메세지
            #endregion

            #region Input 파라메터     
            dicInputParam.Add("P_CO_CD", strCoCd);  // 회사 코드
            dicInputParam.Add("P_EXT_ID", strExtId);
            dicInputParam.Add("P_EXT_TAR_CD", strExtTarCd);
            dicInputParam.Add("P_EXT_TYPE_CD", strExtTypeCd);
            dicInputParam.Add("P_EXT_DATE_TYPE_CD", strExtDateTypeCd);
            dicInputParam.Add("P_EXT_FROM", strExtFrom);
            dicInputParam.Add("P_EXT_TO", strExtTo);
            dicInputParam.Add("P_ABC_TYPE_CD", strAbcTypeCd);
            dicInputParam.Add("P_ABC_CNT", intAbcCnt);
            dicInputParam.Add("P_AGG_TYPE_CD", strAggTypeCd);
            dicInputParam.Add("P_ABC_CD", strAbcCd);
            dicInputParam.Add("P_USER_ID", strUserId);
            dicInputParam.Add("P_COMMENTS", strComments);
            
            #endregion

            dtRtnValue = da.GetSpDataTable(strProcedureName, dicInputParam, arrOutputParam);

            if (dtRtnValue != null)
            {
                if (dtRtnValue.Rows.Count > 0)
                {
                    if (dtRtnValue.Rows[0]["CODE"].ToString().Equals("0") == false)
                    {
                        BaseClass.MsgInfo(dtRtnValue.Rows[0]["MSG"].ToString(), BaseEnumClass.CodeMessage.MESSAGE);
                        isRtnValue = false;
                    }
                }
                else
                {
                    BaseClass.MsgError("ERR_SAVE");
                    isRtnValue = false;
                }
            }

            return isRtnValue;
        }

        #endregion

        #endregion

        #region 기타 메서드
        /// <summary>
        /// 추출기능 메서드 (EXT_TYPE_CD 에 따라 다른 메서드 호출)
        /// </summary>
        /// <returns></returns>
        private void MakeExtract()
        {
            switch (this.ExtITEM.EXT_TYPE_CD)
            {
                case "01":
                    ObExtCapaSearch();
                    break;
                case "02":
                    ObExtEiqSearch();
                    break;
                case "03":
                    ObExtAbcSearch();
                    break;
                default:
                    break;
            }
        }
        
        #region 추출항목 리스트 조회
        /// <summary>
        /// 추출항목 리스트 조회 (CAPA)
        /// </summary>        
        private void AnAggTypeSearch()
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                this.BaseAnAggTypeList = new ObservableCollection<AnAggTypeInfo>();     // 추출 항목 리스트

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_AN_AGG_TYPE_SEARCH();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    // 정상 처리된 경우
                    this.BaseAnAggTypeList.ToObservableCollection(dsRtnValue.Tables[0]);
                }
                else
                {
                    // 오류가 발생한 경우
                    this.BaseAnAggTypeList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                // 상태바 (아이콘) 제거
                this.loadingScreen.IsSplashScreenShown = false;
            }
        }

        /// <summary>
        /// 추출항목 리스트 조회 (ABC)
        /// </summary>
        private void AnDtlRlstSearch()
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_AN_DTL_RLST_SEARCH();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                if (this.BaseClass.CheckResultDataProcess(dsRtnValue, ref strErrCode, ref strErrMsg) == true)
                {
                    if (this.ExtITEM.EXT_TYPE_CD.Equals("03"))  // ABC
                    {
                        this.AggTypeAbcList.ToObservableCollection(dsRtnValue.Tables[0]);

                        if (AggTypeAbcList != null && AggTypeAbcList.Count > 1)
                        {
                            cboABCCnt.SelectedIndex = this.AggTypeAbcList.Count() - 2;
                        }
                    }
                    else
                    {
                        this.SaveAnAggTypeList.ToObservableCollection(dsRtnValue.Tables[0]);

                        foreach (var item in SaveAnAggTypeList)
                        {
                            var delItem = BaseAnAggTypeList.Where(x => x.COM_HDR_CD.Equals(item.COM_HDR_CD)).FirstOrDefault();
                            BaseAnAggTypeList.Remove(delItem);
                        }
                    }
                }
                else
                {
                    // 오류가 발생한 경우
                    this.SaveAnAggTypeList.ToObservableCollection(null);
                    BaseClass.MsgError(strErrMsg, BaseEnumClass.CodeMessage.MESSAGE);
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
            }
            finally
            {
                // 상태바 (아이콘) 제거
                this.loadingScreen.IsSplashScreenShown = false;
            }
        }
        #endregion
        
        private string MakeAggTypeCd()
        {
            /*AGG_TYPE1, AGG_TYPE2*/
            string rtnAggTypeCd = string.Empty;

            if (!this.ExtITEM.EXT_TYPE_CD.Equals("03"))
            {
                foreach (var item in SaveAnAggTypeList)
                {
                    rtnAggTypeCd += item.COM_HDR_CD + ",";
                }

                if (rtnAggTypeCd.Length > 0)
                {
                    rtnAggTypeCd = rtnAggTypeCd.TrimEnd(',');
                }
            }

            return rtnAggTypeCd;
        }

        private object MakeAbcCd()
        {
            /*A,0,10^B,10,90^C,90,100*/
            string rtnAbcCd = string.Empty;

            if (this.ExtITEM.EXT_TYPE_CD.Equals("03"))
            {
                foreach (var item in AggTypeAbcList)
                {
                    rtnAbcCd += item.ABC_CODE + "," + item.FROM_PER_RANGE.ToString() + "," + item.TO_PER_RANGE.ToString() + "^";
                }

                if (rtnAbcCd.Length > 0)
                {
                    rtnAbcCd = rtnAbcCd.TrimEnd('^');
                }
            }

            return rtnAbcCd;
        }

        /// <summary>
        /// CAPA 추출 타입 리스트 조회
        /// </summary>
        private void ObExtCapaSearch()
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                this.gridExtCapa.Visibility = Visibility.Visible;
                this.gridExtEiq.Visibility = Visibility.Collapsed;
                this.gridExtAbc.Visibility = Visibility.Collapsed;

                this.gridAggTypeBase.Visibility = Visibility.Visible;
                this.gridAggTypeAbc.Visibility = Visibility.Collapsed;

                this.ObExtCapaList = new ObservableCollection<ObExtCapaInfo>();   // 추출 항목 리스트

                // 신규이거나 추출하지 않았으면 조회하지 않음.
                if (IsNewExt || !this.ExtITEM.EXT_COMP_YN.Equals("Y"))  
                {
                    return;
                }

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_OB_EXT_CAPA();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                // 정상 처리된 경우
                this.ObExtCapaList.ToObservableCollection(dsRtnValue.Tables[0]);

                //isNewExt = false;

                if (this.IsDataExtract)
                {
                    // 데이터 추출 완료되었습니다.
                    BaseClass.MsgInfo("CMPT_DATA_EXTRACT");
                }

            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
                this.ObExtCapaList.ToObservableCollection(null);

                if (this.IsDataExtract)
                {
                    // 데이터 추출중 오류가 발생했습니다.
                    BaseClass.MsgInfo("ERR_DATA_EXTRACT");
                }
            }
            finally
            {
                

                // 상태바 (아이콘) 제거
                this.loadingScreen.IsSplashScreenShown = false;
            }
        }

        /// <summary>
        /// EIQ 추출 타입 리스트 조회
        /// </summary>
        private void ObExtEiqSearch()
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                this.gridExtCapa.Visibility = Visibility.Collapsed;
                this.gridExtEiq.Visibility = Visibility.Visible;
                this.gridExtAbc.Visibility = Visibility.Collapsed;

                this.gridAggTypeBase.Visibility = Visibility.Visible;
                this.gridAggTypeAbc.Visibility = Visibility.Collapsed;

                this.ObExtEiqList = new ObservableCollection<ObExtEiqInfo>();   // 추출 항목 리스트

                // 신규이거나 추출하지 않았으면 조회하지 않음.
                if (IsNewExt || !this.ExtITEM.EXT_COMP_YN.Equals("Y"))
                {
                    return;
                }

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_OB_EXT_EIQ();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                // 정상 처리된 경우
                this.ObExtEiqList.ToObservableCollection(dsRtnValue.Tables[0]);

                //isNewExt = false;

                if (this.IsDataExtract)
                {
                    // 데이터 추출 완료되었습니다.
                    BaseClass.MsgInfo("CMPT_DATA_EXTRACT");
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
                this.ObExtEiqList.ToObservableCollection(null);

                if (this.IsDataExtract)
                {
                    // 데이터 추출중 오류가 발생했습니다.
                    BaseClass.MsgInfo("ERR_DATA_EXTRACT");
                }
            }
            finally
            {
                // 상태바 (아이콘) 제거
                this.loadingScreen.IsSplashScreenShown = false;
            }
        }

        /// <summary>
        /// ABC 추출 타입 리스트 조회
        /// </summary>
        private void ObExtAbcSearch()
        {
            try
            {
                // 상태바 (아이콘) 실행
                this.loadingScreen.IsSplashScreenShown = true;

                this.gridExtCapa.Visibility = Visibility.Collapsed;
                this.gridExtEiq.Visibility = Visibility.Collapsed;
                this.gridExtAbc.Visibility = Visibility.Visible;

                this.gridAggTypeBase.Visibility = Visibility.Collapsed;
                this.gridAggTypeAbc.Visibility = Visibility.Visible;

                this.ObExtAbcList = new ObservableCollection<ObExtAbcInfo>();   // 추출 항목 리스트
                this.ObExtAbcDetailList = new ObservableCollection<ObExtAbcDetailInfo>();

                // 신규이거나 추출하지 않았으면 조회하지 않음.
                if (IsNewExt || !this.ExtITEM.EXT_COMP_YN.Equals("Y"))
                {
                    return;
                }

                // 셀 유형관리 데이터 조회
                DataSet dsRtnValue = this.GetSP_OB_EXT_ABC();

                if (dsRtnValue == null) { return; }

                var strErrCode = string.Empty;
                var strErrMsg = string.Empty;

                // 정상 처리된 경우
                this.ObExtAbcList.ToObservableCollection(dsRtnValue.Tables[1]);
                this.ObExtAbcDetailList.ToObservableCollection(dsRtnValue.Tables[0]);

                //isNewExt = false;

                if (this.IsDataExtract)
                {
                    // 데이터 추출 완료되었습니다.
                    BaseClass.MsgInfo("CMPT_DATA_EXTRACT");
                }
            }
            catch (Exception err)
            {
                this.BaseClass.Error(err);
                this.ObExtAbcList.ToObservableCollection(null);
                this.ObExtAbcDetailList.ToObservableCollection(null);

                if (this.IsDataExtract)
                {
                    // 데이터 추출중 오류가 발생했습니다.
                    BaseClass.MsgInfo("ERR_DATA_EXTRACT");
                }
            }
            finally
            {
                // 상태바 (아이콘) 제거
                this.loadingScreen.IsSplashScreenShown = false;
            }
        }

        private DateTime GetToDateTime(string ToDate)
        {
            DateTime rtnDateTime = DateTime.Now;

            switch (this.ExtITEM.EXT_DATE_TYPE_CD)
            {
                case "YYYY":
                    if (ToDate.Length >= 4)
                    {
                        rtnDateTime = new DateTime(int.Parse(ToDate.Substring(0, 4)), 12, 31);
                    }
                    break;
                case "YYYYMM":
                    if (ToDate.Length >= 6)
                    {
                        var _Year = int.Parse(ToDate.Substring(0, 4));
                        var _Month = int.Parse(ToDate.Substring(4, 2));
                        rtnDateTime = new DateTime(_Year, _Month, DateTime.DaysInMonth(_Year, _Month));
                    }
                    break;
                case "YYYYMMDD":
                case "YYYYMMDDHH":
                    if (ToDate.Length >= 8)
                    {
                        rtnDateTime = new DateTime(int.Parse(ToDate.Substring(0, 4)), int.Parse(ToDate.Substring(4, 2)), int.Parse(ToDate.Substring(6, 2)));
                    }
                    break;
                default:
                    rtnDateTime = DateTime.Now;
                    break;
            }

            return rtnDateTime;
        }

        private DateTime GetFromDateTime(string FromDate)
        {
            DateTime rtnDateTime = DateTime.Now;

            switch (this.ExtITEM.EXT_DATE_TYPE_CD)
            {
                case "YYYY":
                    if (FromDate.Length >= 4)
                    {
                        rtnDateTime = new DateTime(int.Parse(FromDate.Substring(0, 4)), 1, 1);
                    }
                    break;
                case "YYYYMM":
                    if (FromDate.Length >= 6)
                    {
                        rtnDateTime = new DateTime(int.Parse(FromDate.Substring(0, 4)), int.Parse(FromDate.Substring(4, 2)), 1);
                    }
                    break;
                case "YYYYMMDD":
                case "YYYYMMDDHH":
                    if (FromDate.Length >= 8)
                    {
                        rtnDateTime = new DateTime(int.Parse(FromDate.Substring(0, 4)), int.Parse(FromDate.Substring(4, 2)), int.Parse(FromDate.Substring(6, 2)));
                    }
                    break;
                default:
                    rtnDateTime = DateTime.Now;
                    break;
            }

            return rtnDateTime;
        }

        /// <summary>
        /// 추출일자 ('-' 제거 & Substring)
        /// </summary>
        /// <param name="pFullDataString"></param>
        /// <returns></returns>
        private string GetExtDate(string pFullDataString)
        {
            string rtnExtDate = string.Empty;

            switch (this.ExtITEM.EXT_DATE_TYPE_CD)
            {
                case "YYYY":
                    rtnExtDate = pFullDataString.Replace("-", "").Substring(0, 4);
                    break;
                case "YYYYMM":
                    rtnExtDate = pFullDataString.Replace("-", "").Substring(0, 6);
                    break;
                case "YYYYMMDD":
                    rtnExtDate = pFullDataString.Replace("-", "").Substring(0, 8);
                    break;
                case "ALL":
                case "YYYYMMDDHH":
                    rtnExtDate = pFullDataString.Replace("-", "").Substring(0, 10);
                    break;
                default:
                    break;
            }

            return rtnExtDate;
        }

        /// <summary>
        /// 팝업 닫기 전 확인 메세지
        /// </summary>
        private void CloseProcess()
        {
            this.Close();
        }
        #endregion

        #endregion ▩ 함수

        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        // ~SWCS101_01P()
        // {
        //   // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
        //   Dispose(false);
        // }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
