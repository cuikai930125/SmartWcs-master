using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Common_Etc
{
    public class BaseEnumClass
    {
        #region SystemCode - WCS 프로그램 시스템 코드
        /// <summary>
        /// WCS 프로그램 시스템 코드
        /// </summary>
        public enum SystemCode
        {
            /// <summary>
            /// 컨베이어
            /// </summary>
            CNV = 0,
            /// <summary>
            /// DAS
            /// </summary>
            DAS = 1,
            /// <summary>
            /// EMS
            /// </summary>
            EMS = 2,
            /// <summary>
            /// 미니로드
            /// </summary>
            MLD = 3,
            /// <summary>
            /// QPS
            /// </summary>
            QPS = 4,
            /// <summary>
            /// 소트
            /// </summary>
            SMS = 5,
            /// <summary>
            /// WCS
            /// </summary>
            WCS = 6
        };
        #endregion

        #region SelectedDatabaseKind - 현재 접속한 (동일세션) Database 종류
        public enum SelectedDatabaseKind : int
        {
            /// <summary>
            /// 오라클
            /// </summary>
            ORACLE = 0,
            /// <summary>
            /// MS-SQL
            /// </summary>
            MS_SQL = 1,
            /// <summary>
            /// MariaDB or MY-SQL
            /// </summary>
            MARIA_DB = 2,
            /// <summary>
            /// 타입 없음
            /// </summary>
            NONE = 3
        };
        #endregion

        #region DatabaseConnectionType - Database 접속 타입
        /// <summary>
        /// DataBase 접속 타입
        /// </summary>
        public enum DatabaseConnectionType
        {
            /// <summary>
            /// 개발 DB서버 접속
            /// </summary>
            DEV     = 0,
            /// <summary>
            /// 운영 DB서버 접속
            /// </summary>
            REAL    = 1
        };
        #endregion

        #region TransactionState_ORACLE - 트랜잭션 상태 (오라클)
        /// <summary>
        /// 트랜잭션 상태 (오라클)
        /// </summary>
        public enum TransactionState_ORACLE : int
        {
            None                = 0,    // 트랜잭선 없음
            TransactionStarted  = 1     // 트랜잭션
        };
        #endregion

        #region Transactionstate_MSSQL (MS-SQL) - 트랜잭션 상태 (MS-SQL)
        /// <summary>
        /// Transactionstate_MSSQL (MS-SQL) - 트랜잭션 상태 (MS-SQL)
        /// </summary>
        public enum TransactionState_MSSQL : int
        {
            None                = 0,    // 트랜잭선 없음
            TransactionStarted  = 1     // 트랜잭션
        }
        #endregion

        #region TransactionState_MariaDB (Maria DB) - 트랜잭션 상태 (Maria DB)
        /// <summary>
        /// TransactionState_MariaDB (Maria DB) - 트랜잭션 상태 (Maria DB)
        /// </summary>
        public enum TransactionState_MariaDB : int
        {
            None                = 0,    // 트랜잭선 없음
            TransactionStarted  = 1     // 트랜잭션
        }
        #endregion

        #region MSSqlOutputDataType - MS-SQL Output Parameter 데이터 타입
        /// <summary>
        /// MS-SQL Output Parameter 데이터 타입
        /// </summary>
        public enum MSSqlOutputDataType
        {
            INT16       = 0,
            INT32       = 1,
            INT64       = 2,
            DECIMAL     = 3,
            NUMERIC     = 4,
            VARCHAR     = 5,
            DATETIME    = 6,
            NVARCHAR    = 10,
        }
        #endregion

        #region ButtonKind - UserControl 버튼 종류 (Simple Button)
        /// <summary>
        /// UserControl 버튼 종류 (Simple Button)
        /// </summary>
        public enum ButtonKind : int
        {
            SEARCH          = 0,        // 조회
            SAVE            = 1,        // 저장
            DELETE          = 2,        // 삭제
            EXCEL_DOWNLOAD  = 3,        // 엑셀 다운로드
            EXCEL_UPLOAD    = 4,        // 엑셀 업로드

            BTCH_CREATE     = 10,       // 배치 생성
            BTCH_END        = 11,       // 배치 종료
            BTCH_CLEAR      = 12,       // 배치 삭제
            CHUTE_ASSIGN    = 13,       // 슈트 배정
            
            ROW_ADD         = 20,       // Row 추가
            ROW_DELETE      = 21,       // Row 삭제

            ALL             = 99
        }
        #endregion

        public enum ResourceType : int
        {
            NORMAL  = 0,        // 기본
            MESSAGE = 1         // 메세지
        };

        public enum CodeMessage : int
        {
            CODE        = 0,    // 코드
            MESSAGE     = 1     // 메세지
        }

        #region MenuCommand - Menu 상단 버튼 타입
        /// <summary>
        /// Menu 상단 버튼 타입
        /// </summary>
        public enum MenuCommand : uint
        {
            SC_CLOSE    = 0xF060,
            SC_MINIMIZE = 0xF020,
            SC_MAXIMIZE = 0xF030,
        }
        #endregion

        #region Control Type - 컨트롤 타입
        /// <summary>
        /// 컨트롤 타입
        /// </summary>
        public enum ControlType : int
        {
            BUTTON  = 0,
            LABEL   = 1
        }
        #endregion

        #region ErrorType - 오류 타입
        /// <summary>
        /// 오류 타입
        /// </summary>
        public enum ErrorConnectType
        {
            CONNECTION_STRINGS_VALUE_IS_EMPTY   = 0,        // 연결문자열 값이 공백인 경우
            APP_SETTINGS_VALUE_IS_EMPTY         = 1,        // AppSettings 값 중 TripleDES 값이 공백인 경우
            ENCRYPTION_VALUE_IS_EMPTY           = 2,        // 암호화 결과값이 공백인 경우
            DESCRYPTION_VALUE_IS_EMPTY          = 3         // 복호호 결과값이 공백인 경우
        }
        #endregion

        #region MessageBoxType - 메세지박스 타입
        /// <summary>
        /// 메세지박스 타입
        /// </summary>
        public enum MessageBoxType
        {
            /// <summary>
            /// 정보
            /// </summary>
            Info        = 0,
            /// <summary>
            /// 경고
            /// </summary>
            Warning     = 1,
            /// <summary>
            /// 오류
            /// </summary>
            Question    = 2
        };
        #endregion

        #region CreateTableSchemaKind - 데이터테이블 신규 스키마 생성 시 종류
        /// <summary>
        /// 데이터테이블 신규 스키마 생성 시 종류
        /// </summary>
        public enum CreateTableSchemaKind : int
        {
            COMMON_CODE                 = 0,
            SERVER_DEPLOY_FILE_LIST     = 1,
            LOCAL_DEPLOY_FILE_LIST      = 2,
            DEPLOY_FILE_LIST            = 3
        }
        #endregion

        #region FileCompareResult - 파일 비교 결과
        /// <summary>
        /// 파일 비교 결과
        /// </summary>
        public enum FileCompareResult : int
        {
            EQUAL       = 0,        // 파일 수정일시 일치
            NEW         = 1,        // 신규 등록파일
            UPDATE      = 2         // 수정된 파일 (배포대상)
        }
        #endregion

        #region ECS 화면 통신 - ECS => 화면
        /// <summary>
        /// ECS 화면 통신
        /// </summary>
        public enum EnumToUIEvent
        {
            // connect
            Connected,
            Disconnected,
            CheckReply,
            SendResult,      // int result  0보다 작으면 error 
            KioskSetInductionMode,
            SendResultClear,

            kioskRead = 1000,
            Inducted,
            IPSRead,
            Discharged,
            SortedConfirm,
            LineStop,

            ValueRcv = 2000,
            Retrieve,
            ItemStart,
        };
        #endregion

        #region KIOSK Mode 
        /// <summary>
        /// KIOSK Mode 
        /// </summary>
        public enum EnumModeType
        {
            Empty               = -1, //Overflow
            IPS                 = 0,
            KioskAll            = 1,
            KioskRepresentative = 2,
        }
        #endregion

        #region KIOSK ErrorMessage 
        /// <summary>
        /// KIOSK ErrorMessage 
        /// </summary>
        public enum enumClientErrorMessage
        {
            OK,
            NEWCODEOK,
            NEEDGROUPCODE,
            NOTEXISTPARCELCODE,
            NOTEXISTPOSTALCODE,
            NOTEXISTDESTINATION,
            DUPLICATIONCODE,
            INPUTLENTHCHECK,
            SHOULDFILLLESSONE,
            NORESPONSE,
            // for auto. 
            REGISTED,
            NEEDMOREINFO,
            NEWCODEREGISTED,
            REGISTEDERROR,
            LENTHERROR,
            DBERROR,
            NOCHUTENO,
            IPSMODECHECK,
        }
        #endregion

        #region SystemEquipLocation - 시스템 장비 위치
        /// <summary>
        /// 시스템 장비 위치
        /// </summary>
        public enum SystemEquipLocation
        {
            /// <summary>
            /// 3층 왼쪽 컨베이어
            /// </summary>
            LEFT_3F_CNV = 47,

            /// <summary>
            /// 오른쪽 컨베이어
            /// </summary>
            RIGHT_CNV = 100,

            /// <summary>
            /// 3층 통합 DAS
            /// </summary>
            TOT_3F_DAS = 101,

            /// <summary>
            /// 4층 통합 DAS
            /// </summary>
            TOT_4F_DAS = 103,

            /// <summary>
            /// 4층 왼쪽 컨베이어
            /// </summary>
            LEFT_4F_CNV = 104,

            /// <summary>
            /// 4층 오른쪽 컨베이어
            /// </summary>
            RIGHT_4F_CNV = 105,
        }
        #endregion

        #region SearchPopupType - 검색 팝업 타입
        /// <summary>
        /// 검색 팝업 타입
        /// </summary>
        public enum SearchPopupType : int
        {
            CST_SearchPopup = 0,
            BIZPTNR_SearchPopup = 1,
            SKU_SearchPopup = 2,
            SKU_BARCODE_SearchPopup = 3,
            WORK_STATION_SearchPopup = 4,
            CELL_MST_SearchPopup = 5
        }
        #endregion

        #region ExcelUploadKind - 엑셀 업로드 종류
        /// <summary>
        /// 엑셀 업로드 종류
        /// </summary>
        public enum ExcelUploadKind : int
        {
            OPT_ORD_UPLOAD      = 0     // 최적화 오더 업로드
            //CST_MST         = 1,    // 고객사 마스터
            //BIZPTNR_MST     = 2,    // 거래처 마스터
            //SKU_MST         = 3,    // SKU 마스터
            //GI_ORD_MPNG     = 4,    // 출고 오더 매핑
            //GI_ORD          = 5,    // 출고 오더 업로드
            //CLAS_MST        = 6,    // 분류코드 마스터
            //CHUTE_CLAS      = 7,    // 슈트 분류코드 별 차수
            //TRAIL_INFO      = 8     // 차량정보 (동탄)
        }
        #endregion

        #region LabelPrintKind - 라벨 출력 종류
        /// <summary>
        /// 라벨 출력 종류
        /// </summary>
        public enum LabelPrintKind
        {
            /// <summary>
            /// 입고라벨발행
            /// </summary>
            CNV_TALLY_GR_LABEL = 0,

            /// <summary>
            /// 반품
            /// </summary>
            CNV_SMS_TALLY_GI_LABEL = 1,

            /// <summary>
            /// 반품오류라벨출력
            /// </summary>
            CNV_SMS_TALLY_RTN_IRR_LABEL = 2,

            /// <summary>
            /// 설비를 타지 않는 SKU 박스에 대한 라벨
            /// </summary>
            SP_CNV_TALLY_GR_NEQP_LABEL = 3,

            /// <summary>
            /// 송장 라벨
            /// </summary>
            CNV_SMS_TALLY_GI_S_LABEL = 4,

            /// <summary>
            /// 송장재출력
            /// </summary>
            CNV_INV_RE_PRT_LABEL = 5,

            /// <summary>
            /// 공박스 라벨
            /// </summary>
            EMPTY_BOX_LBL = 6,

            /// <summary>
            /// 반품오류라벨출력 (PC 재출력)
            /// </summary>
            SMS_TALLY_RTN_IRR_LABEL_PC = 7
        };
        #endregion

        #region SerialPort 연결 장비 구분
        /// <summary>
        /// SerialPort 연결 장비 구분
        /// </summary>
        public enum EquipSprBySerialPort
        {
            /// <summary>
            /// 핸드스캐너
            /// </summary>
            HAND_SCANNER = 3,
            /// <summary>
            /// 저울
            /// </summary>
            SCALE = 4,
            /// <summary>
            /// 체적기
            /// </summary>
            CBM = 5
        };
        #endregion
    }
}
