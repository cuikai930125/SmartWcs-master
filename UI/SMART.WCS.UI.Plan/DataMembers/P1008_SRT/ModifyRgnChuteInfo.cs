using SMART.WCS.Common.Extensions;

namespace SMART.WCS.UI.Plan.DataMembers.P1008_SRT
{
    public class ModifyRgnChuteInfo : PropertyNotifyExtensions
    {



        #region + RGN_CD - 권역코드
        /// <summary>
        /// 권역코드
        /// </summary>
        private string _RGN_CD;

        /// <summary>
        /// 권역코드
        /// </summary>
        public string RGN_CD
        {
            get { return this._RGN_CD; }
            set
            {
                if (this._RGN_CD != value)
                {
                    this._RGN_CD = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + RGN_NM - 권역명
        /// <summary>
        /// 권역명
        /// </summary>
        private string _RGN_NM;

        /// <summary>
        /// 권역명
        /// </summary>
        public string RGN_NM
        {
            get { return this._RGN_NM; }
            set
            {
                if (this._RGN_NM != value)
                {
                    this._RGN_NM = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region + CHUTE_ID - 슈트 ID
        /// <summary>
        /// 슈트 ID
        /// </summary>
        private string _CHUTE_ID;

        /// <summary>
        /// 슈트 ID
        /// </summary>
        public string CHUTE_ID
        {
            get { return this._CHUTE_ID; }
            set
            {
                if (this._CHUTE_ID != value)
                {
                    this._CHUTE_ID = value;
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        /// <summary>
        /// 설비 ID
        /// </summary>
        public string EQP_ID { get; set; }


    }
}
