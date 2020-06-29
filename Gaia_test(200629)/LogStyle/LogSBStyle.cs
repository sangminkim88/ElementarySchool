using HmDraw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogStyle
{

    public class LogSBStyle : LogStyleBase
    {
        /// <summary>
        /// 
        /// </summary>
        public LogTag 교량명;
        public LogTag 교대교각위치;

        /// <summary>
        /// 
        /// </summary>
        public LogTag 심도표고두께;    
        public LogTag 주상도;
        public LogTag 시료형태;
        public LogTag 통일분류;
        public LogTag 색조;
        public LogTag TCRRQD;
        public LogTag 지층설명;
        public LogTag 타격횟수;
        public LogTag NVALUE;

        public LogTag 전체영역;
        public LogTag 심도;
        public LogTag 표고;
        public LogTag 두께;
        /// <summary>
        /// 
        /// </summary>

        public LogSBStyle() : base()
        {
            // 공번 접두어
            prefix = "SB-";
            //
            교량명 = new LogTag(10);
            교대교각위치 = new LogTag(90);

            심도표고두께 = new LogTag() { justify = HmDraw.Entities.AttachmentPoint.BaseLeft };
            주상도 = new LogTag();
            시료형태 = new LogTag();
            통일분류 = new LogTag();
            색조 = new LogTag();
            TCRRQD = new LogTag();
            지층설명 = new LogTag();
            타격횟수 = new LogTag();
            NVALUE = new LogTag();

            전체영역 = new LogTag();
            심도 = new LogTag();
            표고 = new LogTag();
            두께 = new LogTag();
        }

        #region 오버라이드함수
#if false
        /// <summary>
        /// 클래스의 변수를 검색한다.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public override FieldInfo[] GetFields(BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            Type type = this.GetType();// typeof(LogBBStyle);

            //FieldInfo[] f = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo[] f = type.GetFields(flags);

            return f;
        }
#endif
        /// <summary>
        /// 주상도 양식에서 변수의 이름과 영역을 검색한다
        /// </summary>
        /// <param name="strfname">양식도면</param>
        /// <returns></returns>
        public override bool ReadStyle(string strfname)
        {
            //return base.ReadStyle(strfname);
            HmDocument document = new HmDocument();
            document.FileOpen(strfname, true);
            //
            return ReadStyle(document);
        }
        public override bool ReadStyle(HmDocument document)
        {
            FieldInfo[] f = GetFields();
            foreach (var v in f)
            {
                // LogTag 타입의 변수에 대해서만 작동한다

                // 레이어
                //strlyr = string.Format("#{0}", v.Name);
                // 값설정
                LogTag logTag = (v.GetValue(this)) as LogTag;
                if (null == logTag)
                    continue;
                // 카테고리 및 레이어 결정
                var r = GetLogCategory(v.Name);
                logTag.Set(v.Name, r.strlyr, r.category);
                // 위치
                if (!GetBox(document, logTag.strlyr, ref logTag))
                {
                    logTag = null;
                }
                v.SetValue(this, logTag);

            }
            //
            ReadVar(document, "#변수");
            //
            return true;
        }

        /// <summary>
        /// 양식에 적용할 변수를 텍스트엔티티에서 읽는다
        /// </summary>
        /// <param name="document"></param>
        /// <param name="strlyr">변수로 사용된 레이어이름</param>
        /// <returns></returns>
        public override bool ReadVar(HmDocument document, string strlyr)
        {
            base.ReadVar(document, strlyr);
            //
            List<string> slist = GetVar(document, strlyr);
            string valstr;
            valstr = GetValue(slist, "주상도깊이");
            double.TryParse(valstr, out thisDepth);
            valstr = GetValue(slist, "SPT");
            double.TryParse(valstr, out intvSpt);
            valstr = GetValue(slist, "MAXN");
            int.TryParse(valstr, out maxNValue);

            return true;
        }

        /// <summary>
        /// (변수로 지정한)주상도 항목의 레이어명과 종류를 결정한다.
        /// </summary>
        /// <param name="strkey">주상도항목</param>
        /// <returns></returns>
        public override (string strlyr, DrillLogCategory category) GetLogCategory(string strkey)
        {
            string lyrname = "0";
            DrillLogCategory category = DrillLogCategory.DLOG_CATEGORY_NONE;

            switch (strkey)
            {
                case "영역":
                    category = DrillLogCategory.DLOG_CATEGORY_ROI;
                    break;
                case "사업명":
                case "발주처":
                    category = DrillLogCategory.DLOG_CATEGORY_PROJ;
                    break;
                case "시추자":
                case "작성자":
                case "시추공번":
                case "시추장비":
                case "시추위치":
                case "시추표고":
                case "시추심도":
                case "시추방법":
                case "시추공경":
                case "시추각도":
                case "지하수위":
                case "케이싱심도":
                case "좌표":
                case "조사일":
                case "해머효율":
                case "기타사항":
                    category = DrillLogCategory.DLOG_CATEGORY_DRLG;
                    break;

                case "교량명":
                case "교대교각위치":
                    category = DrillLogCategory.DLOG_CATEGORY_DRLG;
                    break;

                case "심도표고두께":
                    category = DrillLogCategory.DLOG_CATEGORY_PART_Z;
                    break;
                case "TCRRQD":                
                    category = DrillLogCategory.DLOG_CATEGORY_TCRRQD;
                    break;
                case "타격횟수":
                    category = DrillLogCategory.DLOG_CATEGORY_SPTG;
                    break;
                case "시료형태":
                    category = DrillLogCategory.DLOG_CATEGORY_SAMP;
                    break;
                case "주상도":                
                case "통일분류":
                case "색조":
                    category = DrillLogCategory.DLOG_CATEGORY_PART_A;
                    break;
                case "NVALUE":
                    category = DrillLogCategory.DLOG_CATEGORY_PART_A;
                    break;

                case "지층설명":
                    category = DrillLogCategory.DLOG_CATEGORY_PART_B;
                    break;
            }

            lyrname = string.Format("#{0}", strkey);
            return (lyrname, category);
        }
#endregion
    }
}
