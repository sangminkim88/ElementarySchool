namespace GaiaDB
{
    using GaiaDB.Enums;
    using HmGeometry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Defines the <see cref="DescriptionUtil" />.
    /// </summary>
    public static class DescriptionUtil
    {
        #region Methods

        /// <summary>
        /// The IsSoil.
        /// </summary>
        /// <param name="strStraType">The strStraType<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool IsSoil(string strStraType)
        {
            return !(strStraType.Equals("연암") || strStraType.Equals("경암"));
        }

        /// <summary>
        /// The SoilNote.
        /// </summary>
        /// <param name="soilType">The soilType<see cref="eSoil"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string SoilNote(eSoil soilType)
        {
            switch (soilType)
            {
                case eSoil.gw: return "입도양호한 자갈";
                case eSoil.gp: return "입도불량한 자갈";
                case eSoil.gm: return "실트질 자갈";
                case eSoil.gwgm: return "실트 섞인 입도양호한 자갈";
                case eSoil.gpgm: return "실트 섞인 입도불량한 자갈";
                case eSoil.gc: return "점토질 자갈";
                case eSoil.gwgc: return "점토 섞인 입도양호한 자갈";
                case eSoil.gpgc: return "점토 섞인 입도불량한 자갈";
                case eSoil.gcgm: return "실트 섞인 점토질 자갈";
                case eSoil.sw: return "입도양호한 모래";
                case eSoil.sp: return "입도불량한 모래";
                case eSoil.sm: return "실트질 모래";
                case eSoil.swsm: return "실트 섞인 입도양호한 모래";
                case eSoil.spsm: return "실트 섞인 입도불량한 모래";
                case eSoil.sc: return "점토질 모래";
                case eSoil.swsc: return "점토 섞인 입도양호한 모래";
                case eSoil.spsc: return "점토 섞인 입도불량한 모래";
                case eSoil.scsm: return "실트 섞인 점토질 모래";
                case eSoil.ml: return "저소성 실트";
                case eSoil.mh: return "고소성 실트";
                case eSoil.cl: return "압축성 작은 점토";
                case eSoil.clml: return "저소성 실트섞인 점토";
                case eSoil.ch: return "압축성 큰 점토";
                case eSoil.ol: return "저소성 유기질 점토";
                case eSoil.oh: return "고소성 유기질 점토";
                case eSoil.pt: return "이탄토";
                case eSoil.boulder: return "바위, 호박돌";
                //풍화암은 예외적으로 UI에도 표현되지 않는 문자열을 지층설명에 넣어야함(상민, 신원태 선임 지시)
                case eSoil.wrock: return "굴진시 실트질 모래로 분해 기반암의 구조 및 조직 잔존";
                case eSoil.srock: return "연암";
                case eSoil.hrock: return "경암";
            }
            return "";
        }

        /// <summary>
        /// The SortDescription.
        /// </summary>
        /// <param name="strakeylist">The strakeylist<see cref="List{uint}"/>.</param>
        /// <param name="BegFinY">The BegFinY<see cref="double"/>.</param>
        /// <param name="textHeight">The textHeight<see cref="double"/>.</param>
        /// <returns>The <see cref="List{Tuple{double, string, uint, uint}}"/>.</returns>
        public static List<Tuple<double, string, uint, uint>> SortDescription(List<uint> strakeylist, double BegFinY = 20.0, double textHeight = 0.35)
        {
            List<Tuple<double, string, uint, uint>> SortDesc = new List<Tuple<double, string, uint, uint>>();

            DBDoc doc = DBDoc.Get_CurrDoc();
            double startDepth = 0.0;
            double By = 0;
            double Fy = BegFinY;

            for (int i = 0; i < strakeylist.Count; i++)
            {
                double endDepth = 0.0; // 시작 및 종료심도
                double nextStarDepth = 0.0;

                // 현재 지층 가져오기
                DBDataSTRA stra = null;
                if (!doc.stra.Get_Data(strakeylist[i], ref stra, true))
                    continue;

                DBDataSTRA nextstra = null;
                // 현재지층의 다음지층을가져옴
                if (strakeylist.Count - 1 != i)
                {
                    if (!doc.stra.Get_Data(strakeylist[i + 1], ref nextstra, true))
                        continue;

                    nextStarDepth = nextstra.Depth;
                }

                if (startDepth == Fy)
                {
                    endDepth = nextStarDepth != 0 ? nextStarDepth : By == 0 ? Fy + Fy : By + Fy;
                }
                else
                {
                    endDepth = stra.Depth;
                }

                double start = 0.0, end = 0.0;
                if (!doc.stra.Get_Depth(strakeylist[i], ref start, ref end)) return null;

                // 해당지층의 지층설명가져오기
                List<DBDataDESC> desclist = new List<DBDataDESC>();
                HmKeyList keylist = new HmKeyList();
                doc.desc.Get_KeyList(keylist, strakeylist[i]);
                if (!doc.desc.Get_DataList(keylist, ref desclist, true))
                    continue;

                // 지층이 추가됬을때 지층설명을 추가하기위해 Line을 그림
                if (desclist.Count == 0)
                {
                    SortDesc.AddRange(SortDescription(ref By, ref Fy, textHeight, ref startDepth, endDepth, "", strakeylist[i], 0));
                }
                for (int j = 0; j < desclist.Count; j++)
                {
                    // 0=stra에 desc이 한개만 존재하는경우, 1=복수경우에 첫번째인경우, 2=복수경우에 두번째이후인경우
                    int nType = (desclist.Count == 1 ? 0 : (j == 0 ? 1 : 2));
                    SortDesc.AddRange(SortDescription(ref By, ref Fy, textHeight, ref startDepth, endDepth, desclist[j].ToStringBuilder(nType), strakeylist[i], keylist.list[j]));
                }
            }
            if (SortDesc.Count > 0)
            {
                SortDesc.Add(new Tuple<double, string, uint, uint>(SortDesc.Last().Item1, "^시추종료", 0, 0));
                SortDesc.Add(new Tuple<double, string, uint, uint>(SortDesc.Last().Item1 + textHeight, "^페이지확인용", 0, 0));
            }

            return SortDesc;
        }

        /// <summary>
        /// Tuple(double, string, uint, uint)        
        /// Tuple(depth, descstr, starkey, desckey).
        /// </summary>
        /// <param name="drlgkey">The drlgkey<see cref="uint"/>.</param>
        /// <param name="BegFinY">페이지높이.</param>
        /// <param name="textHeight">text 1개당 높이.</param>
        /// <returns>.</returns>
        public static List<Tuple<double, string, uint, uint>> SortDescription(uint drlgkey, double BegFinY = 20.0, double textHeight = 0.35)
        {
            // strakeylist를 가져온다
            DBDoc doc = DBDoc.Get_CurrDoc();
            HmKeyList keylist = new HmKeyList();
            doc.stra.Get_KeyList(keylist, drlgkey);

            return SortDescription(keylist.list, BegFinY, textHeight);
        }

        /// <summary>
        /// The SortDescription.
        /// </summary>
        /// <param name="By">The By<see cref="double"/>.</param>
        /// <param name="Fy">The Fy<see cref="double"/>.</param>
        /// <param name="textHeight">The textHeight<see cref="double"/>.</param>
        /// <param name="start">The start<see cref="double"/>.</param>
        /// <param name="end">The end<see cref="double"/>.</param>
        /// <param name="descStr">The descStr<see cref="string"/>.</param>
        /// <param name="strakey">The strakey<see cref="uint"/>.</param>
        /// <param name="desckey">The desckey<see cref="uint"/>.</param>
        /// <returns>The <see cref="List{Tuple{double, string, uint, uint}}"/>.</returns>
        private static List<Tuple<double, string, uint, uint>> SortDescription(ref double By, ref double Fy, double textHeight, ref double start, double end, string descStr, uint strakey, uint desckey)
        {
            List<Tuple<double, string, uint, uint>> SortDesclist = new List<Tuple<double, string, uint, uint>>();
            Tuple<double, string, uint, uint> SortDesc = null;

            StringBuilder descsb = new StringBuilder();
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(descStr))
                descStr += "사용자 입력\r\n";

            int textCnt = descStr.Split('\n').Count(); // Text 갯수            
            double currline = start + 0.1;

            for (int i = 0; i < textCnt; i++)
            {
                string str = descStr.Split('\n')[i];

                // 마지막 그린게 종료심도를 넘지않았을때
                if (currline < end)
                {
                    SortDesc = new Tuple<double, string, uint, uint>(currline, str, strakey, desckey);
                    if (i == textCnt - 1)
                    {
                        start = end;
                    }
                    SortDesclist.Add(SortDesc);
                }
                else
                {
                    if (currline < Fy - 0.25)
                    {
                        SortDesc = new Tuple<double, string, uint, uint>(currline, str, strakey, desckey);
                        if (i == textCnt - 1)
                        {
                            end = currline;
                            start = currline;
                        }
                    }
                    else
                    {
                        double vvv = currline += textHeight;
                        SortDesc = new Tuple<double, string, uint, uint>(vvv, str, strakey, desckey);
                        end = vvv;
                        start = vvv;
                        By = Fy;
                        Fy = By == 0 ? Fy + Fy : By + Fy;
                    }
                    SortDesclist.Add(SortDesc);
                }

                currline += textHeight;
            }

            return SortDesclist;
        }

        #endregion
    }
}
