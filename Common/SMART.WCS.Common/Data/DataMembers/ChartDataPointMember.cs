using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMART.WCS.Common.Data.DataMembers
{
    public class ChartDataPointMember
    {
        /// <summary>
        /// 범례
        /// </summary>
        public string Regend { get; private set; }

        /// <summary>
        /// 차트 Argument
        /// </summary>
        public double Argument { get; private set; }

        /// <summary>
        /// 차트 Value
        /// </summary>
        public double Value { get; private set; }

        /// <summary>
        /// String 타입 차트 Argument
        /// </summary>
        public string Argument_str { get; private set; }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_dValue">차트 Value</param>
        /// <param name="_dArgument">차트 Argument</param>
        public ChartDataPointMember(double _dArgument, double _dValue)
        {
            this.Argument = _dArgument;
            this.Value = _dValue;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_strRegend">범례</param>
        /// /// <param name="_dValue">차트 Value</param>
        /// <param name="_dArgument">차트 Argument</param>
        public ChartDataPointMember(string _strRegend, double _dArgument, double _dValue)
        {
            this.Regend = _strRegend;
            this.Argument = _dArgument;
            this.Value = _dValue;
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="_strRegend">범례</param>
        /// <param name="_dArgument">차트 Argument(String type)</param>
        /// <param name="_dValue">차트 Value</param>
        public ChartDataPointMember(string _strRegend, string _dArgument, double _dValue)
        {
            this.Regend = _strRegend;
            this.Argument_str = _dArgument;
            this.Value = _dValue;
        }
    }
}
