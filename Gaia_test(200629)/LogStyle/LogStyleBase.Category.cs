using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogStyle
{
    public enum DrillLogStyle
    {
        // BB,SB 쌓기부,교량부
        // TB,CB 터널부,깎기부
        DLOG_STYLE_BB = 0,
        DLOG_STYLE_SB = 1,
        DLOG_STYLE_TB = 2,
        DLOG_STYLE_CB = 3
    }

    public enum DrillLogCategory
    {
        DLOG_CATEGORY_NONE = 0,
        DLOG_CATEGORY_ROI = 1,
        DLOG_CATEGORY_PROJ = 2,
        DLOG_CATEGORY_DRLG = 3,
        DLOG_CATEGORY_SPTG = 4,
        DLOG_CATEGORY_PART_A = 10,
        DLOG_CATEGORY_SAMP = 11,
        DLOG_CATEGORY_TCRRQD = 12,
        DLOG_CATEGORY_PART_B = 20,
        DLOG_CATEGORY_JSHP = 21,
        DLOG_CATEGORY_TCR = 22,
        DLOG_CATEGORY_RQD = 23,
        DLOG_CATEGORY_JMAX = 24,
        DLOG_CATEGORY_JMIN = 25,
        DLOG_CATEGORY_JAVG = 26,
        DLOG_CATEGORY_DSF = 27,
        DLOG_CATEGORY_PART_C = 30,
        DLOG_CATEGORY_PART_D = 40,
        DLOG_CATEGORY_PART_E = 50,
        DLOG_CATEGORY_PART_Y = 98,
        DLOG_CATEGORY_PART_Z = 99,
    }

    partial class LogStyleBase
    {
        public static LogStyleBase GetLogStyle(DrillLogStyle style)
        {
            LogStyleBase logstyle = null;

            switch(style)
            {
                // 현재는 양식이 쌍으로 되어있음
                case DrillLogStyle.DLOG_STYLE_BB:
                case DrillLogStyle.DLOG_STYLE_SB:
                    logstyle = new LogBBStyle();
                    break;
                case DrillLogStyle.DLOG_STYLE_TB:
                case DrillLogStyle.DLOG_STYLE_CB:
                    logstyle = new LogTBStyle();
                    break;
            }

            return logstyle;
        }

        /// <summary>
        /// 로그타입을 알려준다
        /// </summary>
        /// <returns></returns>
        public DrillLogStyle GetLogStyle()
        {
            DrillLogStyle style = DrillLogStyle.DLOG_STYLE_BB;

            Type type = GetLogType();
            if (type == typeof(LogBBStyle))
                style = DrillLogStyle.DLOG_STYLE_BB;
            else if (type == typeof(LogSBStyle))
                style = DrillLogStyle.DLOG_STYLE_SB;
            else if (type == typeof(LogTBStyle))
                style = DrillLogStyle.DLOG_STYLE_TB;
            else if (type == typeof(LogCBStyle))
                style = DrillLogStyle.DLOG_STYLE_CB;

            return style;
        }
    }
}
