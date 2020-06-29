namespace GAIA2020.Design
{
    using GAIA2020.Manager;
    using GAIA2020.Models;
    using GAIA2020.Utilities;
    using GaiaDB;
    using GaiaDB.Enums;
    using HmDraw;
    using HmDraw.Entities;
    using HmGeometry;
    using LogStyle;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Media;
    using Brush = System.Windows.Media.Brush;
    using CmdParmParser = LogStyle.CmdParmParser;
    using Color = System.Drawing.Color;

    public partial class UDrillLogView
    {
        #region 속성                
        /// <summary>
        /// Defines the curentDrillProperty.
        /// </summary>
        private DrillProperty curentDrillProperty;

        /// <summary>
        /// 전체 영역의 LogTag
        /// </summary>
        private LogTag AllLogTag
        {
            get
            {
                return dlogstyle.GetLogTag("전체영역");
            }
        }

        /// <summary>
        /// 로그의 시작깊이
        /// </summary>
        private double BegY
        {
            get
            {
                double Y = dlogstyle.baseDepth + dlogstyle.normalDepth * dlogstyle.currPageNo;
                return Y;
            }
        }
        /// <summary>
        /// 로그의 종료깊이
        /// </summary>
        private double FinY
        {
            get
            {
                double Y = dlogstyle.baseDepth + dlogstyle.normalDepth * dlogstyle.currPageNo;
                Y += dlogstyle.thisDepth;
                return Y;
            }
        }
        #endregion

        #region 계산
        /// <summary>
        /// 심도값에 해당하는 캐드상의 좌표를 계산해줍니다.
        /// </summary>
        /// <param name="depth">심도값</param>
        /// <returns></returns>
        private double CalcDepth2CAD(double depth)
        {
            if (AllLogTag == null) return 0.0;
            double py = AllLogTag.box.leftY + (BegY - depth) * (AllLogTag.box.height / dlogstyle.thisDepth);
            return py;
        }
        /// <summary>
        /// 캐드값에 해당하는 실제 심도를 계산해줍니다
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        private double CalcCAD2Depth(double depth)
        {
            if (AllLogTag == null) return 0.0;
            double py = BegY + ((AllLogTag.box.leftY - depth) / AllLogTag.box.height) * dlogstyle.thisDepth;
            return py;
        }
        /// <summary>
        /// SPT값에 해당하는 캐드상의 좌표를 계산해줍니다.
        /// </summary>
        /// <param name="depth">spt 값</param>
        /// <returns></returns>
        private double CalcSpt(double depth)
        {
            LogTag v = dlogstyle.GetLogTag("NVALUE");
            if (v == null)
                return 0.0;

            double px = v.box.leftX + (depth * (v.box.width / dlogstyle.maxNValue));
            return px;
        }
        #endregion


        #region Methods

        /// <summary>
        /// DB에 저장된 주상도데이터를 그립니다.
        /// </summary>
        /// <param name="log">주상도.</param>
        public void Draw(DrillProperty log)
        {
            this.curentDrillProperty = log;

            if (!aloadflag)
                return;

            // 로그스타일 확인
            if (null == dlogstyle)
                return;

            // 로그 스타일 준비
            //LoadStyle(log.Drilltype);

            // 엔티티그리기            
            DrawPage(log.Ukey, log.Page - 1);
        }

        /// <summary>
        /// 페이지에 주상도데이터를 그립니다
        /// </summary>
        /// <param name="ukey">선택된 주상도의 key</param>
        /// <param name="page">현재 page</param>
        public void DrawPage(uint ukey, int page = 0)
        {
            // 페이지 지정
            dlogstyle.SetCurrPage(page);

            // 프로젝트정보
            DrawPROJ(ukey, page); // 공통정보
            DrawDRLG(ukey, page); // 기본정보

            //심도/표고/두께/주상도/통일분류/색조 그리기(STRA)/절리형상
            DrawSTRA(ukey, page);
            //시료형태 그리기(SAMP)
            DrawSAMP(ukey, page);
            // SPT
            DrawSPTG(ukey, page);

            showHighlight(true);
        }
        #region 상단 프로젝트 정보 (PROJ, DRLG)
        private void DrawPROJ(uint ukey, int page = 0)
        {
            DBDataPROJ d = null;
            DBDoc doc = DBDoc.Get_CurrDoc();
            if (!doc.proj.Get_Data(ref d, true))
                return;

            LogTag v;
            string s;

            // 사업명
            v = dlogstyle.GetLogTag("사업명");
            if (v != null)
            {
                s = d.ProjectName;
                DrawHmText(v, s, ukey, "PROJ");
            }

            // 발주처
            v = dlogstyle.GetLogTag("발주처");
            if (v != null)
            {
                s = d.CompanyName;
                DrawHmText(v, s, ukey, "PROJ");
            }
#if false // 시추자 와 작성자는 DrillLog으로 옮겨졌습니다.
            // 시추자
            v = dlogstyle.GetLogTag("시추자");
            if (v != null)
            {
                s = d.DrillManName;
                DrawHmTextPROJ(v, s, ukey);
            }

            // 작성자
            v = dlogstyle.GetLogTag("작성자");
            if (v == null)
            {
                s = d.WriteManName;
                DrawHmTextPROJ(v, s, ukey);
            }
#endif

            // 현재페이지
            v = dlogstyle.GetLogTag("현재페이지");
            if (v != null)
            {
                DrawHmTextPage(v, (page + 1).ToString());
            }

            // 전체페이지
            v = dlogstyle.GetLogTag("전체페이지");
            if (v != null)
            {
#if true
                int allpage = 1;
                double dCurrDepth = 0.0;
                if (doc.drlg.Get_Depth(ukey, ref dCurrDepth, true))
                {
                    eDepartment e = doc.drlg.Get_Department(ukey);
                    LogStyleManager styleManager = App.GetLogStyleManager();
                    Tuple<double, double> height = styleManager.GetStyleHeight(e);
                    allpage = DBDataDRLG.Get_PageNumber(dCurrDepth, height.Item1, height.Item2);
                }

                DrawHmTextPage(v, allpage.ToString());
#else
                DBDataDRLG drlgD = null;
                if (!doc.drlg.Get_Data(ukey, ref drlgD)) return;
                DrawHmTextPage(v, drlgD.Get_PageCount().ToString());
#endif
            }
        }
        private void DrawDRLG(uint ukey, int page = 0)
        {
            DBDataDRLG d = null;
            DBDoc doc = DBDoc.Get_CurrDoc();
            if (!doc.drlg.Get_Data(ukey, ref d, true))
                return;

            LogTag v;
            string s;
            string nameType = string.Empty;
            string locType = string.Empty;

            // 조사일            
            v = dlogstyle.GetLogTag("조사일");
            if (v != null)
            {
                //string start = "";// string.Format("{0}년{1}월{2}일", d.DateStart.Year, d.DateStart.Month, d.DateStart.Day);
                //string end = "";// string.Format("~{0}월{1}일", d.DateEnd.Month, d.DateStart.Day);
                //s = Path.Combine(start, end).Replace(@"\", "");
                s = d.SurveyDay;
                DrawHmText(v, s, ukey, "DRLG");
            }

            // 기타사항
            //v = dlogstyle.GetLogTag("기타사항");
            //if (v != null)
            //{
            //    s = d.
            //    DrawHmTextDRLG(v, s, ukey);
            //}

            // 시추장비
            v = dlogstyle.GetLogTag("시추장비");
            if (v != null)
            {
                s = d.DrillDevice;
                DrawHmText(v, s, ukey, "DRLG");
            }

            // 해머효율
            v = dlogstyle.GetLogTag("해머효율");
            if (v != null)
            {
                s = d.HammerEfficiencyPercent.ToString() + "%";
                DrawHmText(v, s, ukey, "DRLG");
            }

            // 시추방법
            v = dlogstyle.GetLogTag("시추방법");
            if (v != null)
            {
                s = d.DrillingMethod;
                DrawHmText(v, s, ukey, "DRLG");
            }

            // 시추각도
            v = dlogstyle.GetLogTag("시추각도");
            if (v != null)
            {
                s = d.DrillingAngleType;
                DrawHmText(v, s, ukey, "DRLG");
            }

            switch (CurrLogStyle)
            {
                case eDepartment.Fill:
                case eDepartment.Bridge:
                    nameType = "교량명";
                    locType = "교대교각위치";
                    break;
                case eDepartment.Cut:
                case eDepartment.Tunnel:
                    nameType = "터널명";
                    locType = "터널위치";
                    break;
            }

            // 교량명or터널명
            v = dlogstyle.GetLogTag(nameType);
            if (v != null)
            {
                s = d.BridgeOrTunnelName;
                DrawHmText(v, s, ukey, "DRLG");
            }

            // 교대(각)위치or터널위치            
            v = dlogstyle.GetLogTag(locType);
            if (v != null)
            {
                s = d.BridgeOrTunnelLocation;
                DrawHmText(v, s, ukey, "DRLG");
            }

            // 시추위치
            v = dlogstyle.GetLogTag("시추위치");
            if (v != null)
            {
                s = d.DrillLocation;
                DrawHmText(v, s, ukey, "DRLG");
            }

            // 시추공경
            v = dlogstyle.GetLogTag("시추공경");
            if (v != null)
            {
                s = string.Format("NX θ{0}", d.DrillPipe);
                DrawHmText(v, s, ukey, "DRLG");
            }

            // 좌표
            v = dlogstyle.GetLogTag("좌표");
            if (v != null)
            {
                s = string.Format("{0:F3} / {1:F3}", d.Position.X, d.Position.Y);
                DrawHmText(v, s, ukey, "DRLG");
            }

            // 시추공번
            v = dlogstyle.GetLogTag("시추공번");
            if (v != null)
            {
                s = d.DrillPipeNum;
                DrawHmText(v, s, ukey, "DRLG");
            }

            // 시추표고
            v = dlogstyle.GetLogTag("시추표고");
            if (v != null)
            {
                s = string.Format("EL({0}) {1}m", d.Elevation > 0 ? '+' : '-', d.Elevation > 0 ? d.Elevation : d.Elevation * -1);
                DrawHmText(v, s, ukey, "DRLG");
            }

            // 시추심도
            v = dlogstyle.GetLogTag("시추심도");
            if (v != null)
            {
                s = string.Format("GL(-) {0}m", d.Depth);
                DrawHmText(v, s, ukey, "DRLG");
            }

            // 케이싱심도
            v = dlogstyle.GetLogTag("케이싱심도");
            if (v != null)
            {
                s = string.Format("GL(-) {0}m", d.CasingDepth);
                DrawHmText(v, s, ukey, "DRLG");
            }

            // 지하수위
            v = dlogstyle.GetLogTag("지하수위");
            if (v != null)
            {
                if(d.WaterLevel == 99999.0)
                {
                    s = "-";
                }
                else
                {
                    s = string.Format("GL(-) {0}m", d.WaterLevel);
                }
                
                DrawHmText(v, s, ukey, "DRLG");
                DrawWaterLevel(v, d.WaterLevel);
            }

            // 기타사항
            v = dlogstyle.GetLogTag("기타사항");
            if (v != null)
            {
                s = d.additionalInfo;
                DrawHmText(v, s, ukey, "DRLG");
            }

            // 시추자
            v = dlogstyle.GetLogTag("시추자");
            if (v != null)
            {
                s = d.DrillManName;
                DrawHmText(v, s, ukey, "DRLG");
            }

            // 작성자
            v = dlogstyle.GetLogTag("작성자");
            if (v != null)
            {
                s = d.WriteManName;
                DrawHmText(v, s, ukey, "DRLG");
            }

#if false
            //심도/표고/두께/주상도/통일분류/색조 그리기(STRA)
            DrawSTRA(ukey, d.Elevation, page);
            //시료형태 그리기(SAMP)
            //DrawSAMP(ukey, page);
#endif
        }

        private void DrawHmText(LogTag v, string varstr, uint ukey, string db, bool needInvisibleBox = true)
        {
            AddLayer(v.strlyr);
            v.xdatastr = v.valkey;
            // label setting
            Dictionary<string, string> diclabel = new Dictionary<string, string>
            {
                { "db", db },
                { "var", v.valkey },
                { "key", ukey.ToString() }
            };

            HmText hmText = new HmText(new HmPoint3D(v.box.leftX, v.box.GetBound().Center.Y, 0.0), 2.0, varstr)
            {
                Label = CmdParmParser.GetLabelfromDictionary(diclabel),
                Justify = AttachmentPoint.MiddleLeft,
                Layer = v.strlyr
            };

            DrawHmEntity(hmText, v, v.box.GetBound());
            if (needInvisibleBox)
            {
                addInvisibleBox(new Point(v.box.leftX, v.box.leftY), v.box.width, v.box.height, hmText.Label, hmText.Layer);
            }
        }
        private void DrawHmTextPage(LogTag v, string varstr)
        {
            AddLayer(v.strlyr);

            HmText hmText = new HmText(new HmPoint3D(v.box.leftX, v.box.leftY, 0.0), 2.0, varstr)
            {
                Justify = AttachmentPoint.MiddleCenter,
                Layer = v.strlyr
            };

            DrawHmEntity(hmText, v);
        }
#endregion

#region 심도, 표고, 두께(STRA), 절리형상
        /// <summary>
        /// 심도, 표고, 두께를 한꺼번에 그린다.
        /// </summary>
        /// <param name="ukey">주상도</param>
        /// <param name="page">페이지</param>
        private void DrawSTRA(uint ukey, int page = 0)
        {
            DBDataDRLG d = null;
            DBDoc doc = DBDoc.Get_CurrDoc();
            if (!doc.drlg.Get_Data(ukey, ref d, true))
                return;

            List<DBDataSTRA> stralist = new List<DBDataSTRA>();
            HmKeyList keylist = new HmKeyList();

            //키 리스트를 깊이로 정렬하여 가져옵니다.
            doc.stra.Get_KeyList(keylist, ukey);
            if (!doc.stra.Get_DataList(keylist, ref stralist, true))
                return;

            // 시추표고
            double elevation = d.Elevation;

            if (stralist.Count > 0)
            {
                // 심도, 표고, 두께
                DrawSTRAInfo(stralist, keylist, elevation, page);
                // 지층설명
                DrawDESC(keylist.list, page);
                // 절리형상(JSHP)
                DrawJSHP(keylist.list, page);
                // TCRRQD(쌓기부, 교량부)
                DrawTCR_RQD(keylist.list, page);
                // TCRRQD(깎기부, 터널부) D,S,F,JOINT
                DrawTCR_RQD_D_S_F_Joint(keylist.list, page);
            }
        }
        private void DrawSTRAInfo(List<DBDataSTRA> stralist, HmKeyList keylist, double elevation, int page = 0)
        {
            // 심도, 표고, 두께
            DrawSTRADepthElevationThick(stralist, keylist, elevation);
            // 주상도
            DrawSTRADrill(stralist, keylist);
            // 통일분류
            DrawSTRAType(stralist, keylist);
            // 색조
            DrawSTRAColor(stralist, keylist);
        }
        private void DrawSTRADepthElevationThick(List<DBDataSTRA> stralist, HmKeyList keylist, double elev)
        {
            LogTag v = dlogstyle.GetLogTag("심도표고두께");
            if (v == null)
                return;

            v.xdatastr = v.valkey;
            AddLayer(v.strlyr);

            double pagefr = BegY, pageto = FinY;// 페이지의 시작과 끝깊이
            double zzfr = 0.0, zzto = 0.0;      // 지층의 시작과 끝깊이
            double zbeg, zfin;                  // 페이지에 그리는 지층의 시작과 끝
            double dwgz1, dwgz2;

            for (int i = 0; i < stralist.Count; i++)
            {
                if (stralist[i].Depth == pagefr) continue;

                zzto = stralist[i].Depth;
                if (zzto < pagefr || zzfr > pageto) continue;

                // 화면에 그릴때 사용할 지층의 시작깊이
                zbeg = pagefr > zzfr ? pagefr : zzfr;
                // 화면에 그릴때 사용할 지층의 종료깊이
                zfin = zzto > pageto ? pageto : zzto;

                // 그릴영역계산
                LogTag calcBound = v.Clone();
                dwgz1 = CalcDepth2CAD(zbeg);
                dwgz2 = CalcDepth2CAD(zfin);
                calcBound.box.Set(v.box.leftX, dwgz1, v.box.width, dwgz1 - dwgz2);

                // 심도 계산
                double currDepth = CalcDepth2CAD(stralist[i].Depth);
                // 표고 계산
                double currElevation = elev - stralist[i].Depth;
                // 두께 계산
                double currThick = 0.0;
                if (i == 0)
                { currThick = stralist[i].Depth; }
                else
                { currThick = stralist[i].Depth - stralist[i - 1].Depth; }

                // 심도표고두께 폴리곤
                HmPolyline poly = DrawHmPolygon(calcBound, keylist.list[i], currDepth, Color.Red, stralist[i].Depth);

                // 심도표고두께 Text
                if (zzto <= pageto)
                {
                    DrawHmTextSTRA(v, poly.GetBounds(), stralist[i].Depth, currElevation, currThick, Color.Red, keylist.list[i]);
                }

                // 다음 지층의 시작깊이는 이번층의 종료깊이가 됨
                zzfr = zzto;
            }
        }
        private void DrawSTRADrill(List<DBDataSTRA> stralist, HmKeyList keylist)
        {
            LogTag v = dlogstyle.GetLogTag("주상도");
            if (v == null)
                return;

            v.xdatastr = v.strlyr.Replace("#", "");
            AddLayer(v.strlyr);

            double pagefr = BegY, pageto = FinY;// 페이지의 시작과 끝깊이
            double zzfr = 0.0, zzto = 0.0;      // 지층의 시작과 끝깊이
            double zbeg, zfin;                  // 페이지에 그리는 지층의 시작과 끝
            double dwgz1, dwgz2;

            for (int i = 0; i < stralist.Count; i++)
            {
                if (stralist[i].Depth == pagefr) continue;

                zzto = stralist[i].Depth;
                if (zzto < pagefr || zzfr > pageto) continue;

                // 화면에 그릴때 사용할 지층의 시작깊이
                zbeg = pagefr > zzfr ? pagefr : zzfr;
                // 화면에 그릴때 사용할 지층의 종료깊이
                zfin = zzto > pageto ? pageto : zzto;

                // 그릴영역계산
                LogTag calcBound = v.Clone();
                dwgz1 = CalcDepth2CAD(zbeg);
                dwgz2 = CalcDepth2CAD(zfin);
                calcBound.box.Set(v.box.leftX, dwgz1, v.box.width, dwgz1 - dwgz2);

                double currDepth = CalcDepth2CAD(stralist[i].Depth);
                // 주상도 폴리곤                
                HmPolyline poly = DrawHmPolygon(calcBound, keylist.list[i], currDepth, Color.FromArgb(0, 0, 1), stralist[i].Depth);
                // 패턴 및 색상결정
                List<Tuple<string, Brush>> straColor = ColorUtil.GetGaiaColorDataByID(stralist[i].ColorList);

                System.Windows.Media.Color drillColor = straColor.Count == 0 ?
                    System.Windows.Media.Color.FromArgb(255, 255, 255, 255)
                : ((SolidColorBrush)straColor[0].Item2).Color;

                // 주상도 Hatch
                DrawHmHatchSTRA(v, keylist.list[i], poly, EnumUtil.GetDescription(stralist[i].soilType), drillColor);
                // 다음 지층의 시작깊이는 이번층의 종료깊이가 됨
                zzfr = zzto;
            }
        }
        private void DrawSTRAType(List<DBDataSTRA> stralist, HmKeyList keylist)
        {
            LogTag v = dlogstyle.GetLogTag("통일분류");
            if (v == null)
                return;

            v.xdatastr = v.valkey;
            AddLayer(v.strlyr);

            double pagefr = BegY, pageto = FinY;// 페이지의 시작과 끝깊이
            double zzfr = 0.0, zzto = 0.0;      // 지층의 시작과 끝깊이
            double zbeg, zfin;                  // 페이지에 그리는 지층의 시작과 끝
            double dwgz1, dwgz2;

            for (int i = 0; i < stralist.Count; i++)
            {
                if (stralist[i].Depth == pagefr) continue;

                zzto = stralist[i].Depth;
                if (zzto < pagefr || zzfr > pageto) continue;

                // 화면에 그릴때 사용할 지층의 시작깊이
                zbeg = pagefr > zzfr ? pagefr : zzfr;
                // 화면에 그릴때 사용할 지층의 종료깊이
                zfin = zzto > pageto ? pageto : zzto;

                // 그릴영역계산
                LogTag calcBound = v.Clone();
                dwgz1 = CalcDepth2CAD(zbeg);
                dwgz2 = CalcDepth2CAD(zfin);
                calcBound.box.Set(v.box.leftX, dwgz1, v.box.width, dwgz1 - dwgz2);

                double currDepth = CalcDepth2CAD(stralist[i].Depth);
                // 통일분류 폴리곤
                HmPolyline poly = DrawHmPolygon(calcBound, keylist.list[i], currDepth, Color.Pink, stralist[i].Depth);
                // 통일분류 Text                
                DrawHmTextType(v, keylist.list[i], poly.GetBounds(), EnumUtil.GetDescription(stralist[i].soilType), Color.Pink);
                // 다음 지층의 시작깊이는 이번층의 종료깊이가 됨
                zzfr = zzto;
            }
        }
        private void DrawSTRAColor(List<DBDataSTRA> stralist, HmKeyList keylist)
        {
            LogTag v = dlogstyle.GetLogTag("색조");
            if (v == null)
                return;

            v.xdatastr = v.valkey;
            AddLayer(v.strlyr);

            double pagefr = BegY, pageto = FinY;// 페이지의 시작과 끝깊이
            double zzfr = 0.0, zzto = 0.0;      // 지층의 시작과 끝깊이
            double zbeg, zfin;                  // 페이지에 그리는 지층의 시작과 끝
            double dwgz1, dwgz2;

            for (int i = 0; i < stralist.Count; i++)
            {
                if (stralist[i].Depth == pagefr) continue;

                zzto = stralist[i].Depth;
                if (zzto < pagefr || zzfr > pageto) continue;

                // 화면에 그릴때 사용할 지층의 시작깊이
                zbeg = pagefr > zzfr ? pagefr : zzfr;
                // 화면에 그릴때 사용할 지층의 종료깊이
                zfin = zzto > pageto ? pageto : zzto;

                // 그릴영역계산
                LogTag calcBound = v.Clone();
                dwgz1 = CalcDepth2CAD(zbeg);
                dwgz2 = CalcDepth2CAD(zfin);
                calcBound.box.Set(v.box.leftX, dwgz1, v.box.width, dwgz1 - dwgz2);

                double currDepth = CalcDepth2CAD(stralist[i].Depth);
                // 색조 폴리곤
                HmPolyline poly = DrawHmPolygon(calcBound, keylist.list[i], currDepth, Color.FromArgb(0, 0, 1));
                // 패턴 및 색상결정
                List<Tuple<string, Brush>> straColor = ColorUtil.GetGaiaColorDataByID(stralist[i].ColorList);
                if (straColor == null || straColor.Count == 0)
                {
                    // 다음 지층의 시작깊이는 이번층의 종료깊이가 됨
                    zzfr = zzto;
                    continue;
                }

                // 색조 Hatch
                DrawHmHatchSTRA(v, keylist.list[i], poly, "SOLID", ((SolidColorBrush)straColor[0].Item2).Color);
                // 색조 Text
                DrawHmTextColor(v, keylist.list[i], poly.GetBounds(), straColor, Color.DarkBlue);
                // 다음 지층의 시작깊이는 이번층의 종료깊이가 됨
                zzfr = zzto;
            }
        }
        /// <summary>
        /// 심도,표고,두께에 해당하는 HmText를 그려줍니다.
        /// </summary>
        /// <param name="vv">logtag</param>
        /// <param name="py">draw할 좌표</param>
        /// <param name="value">draw할 값</param>
        /// <param name="color">색상</param>
        /// <returns></returns>
        private void DrawHmTextSTRA(LogTag v, HmBounds2D bound, double v1, double v2, double v3, Color color, uint ukey)
        {
            // Label
            Dictionary<string, string> straLabel = new Dictionary<string, string>
            {
                { "db", "STRA" },
                { "var", v.valkey },
                { "key", ukey.ToString() },
                { "currtext", v1.ToString() }
            };
            LogTag t1 = dlogstyle.GetLogTag("심도");
            LogTag t2 = dlogstyle.GetLogTag("표고");
            LogTag t3 = dlogstyle.GetLogTag("두께");

            t1.box.Set(t1.box.leftX, bound.UpperLeft.Y, t1.box.width, bound.UpperLeft.Y - bound.LowerLeft.Y);
            t2.box.Set(t2.box.leftX, bound.UpperLeft.Y, t2.box.width, bound.UpperLeft.Y - bound.LowerLeft.Y);
            t3.box.Set(t3.box.leftX, bound.UpperLeft.Y, t3.box.width, bound.UpperLeft.Y - bound.LowerLeft.Y);

            // 심도
            HmText hmtext = new HmText(new HmPoint3D((t1.box.leftX + (t1.box.width) / 2) + 1.0, bound.LowerLeft.Y + 1.8), 2.0, string.Format("{0:F2}", v1))
            //HmText hmtext = new HmText(new HmPoint3D(bound.UpperLeft.X + 2.8, bound.LowerLeft.Y + 2.8), 2.0, string.Format("{0:F2}", v1))
            {
                Color = color,
                Label = CmdParmParser.GetLabelfromDictionary(straLabel),
                Layer = v.strlyr,
                Justify = AttachmentPoint.MiddleCenter
            };
            DrawHmEntity(hmtext, v, t1.box.GetBound());

            // 표고
            straLabel.Add("type", "표고");
            hmtext = new HmText(new HmPoint3D(t2.box.leftX + (t2.box.width) / 2, bound.LowerLeft.Y + 1.8), 2.0, string.Format("{0:F2}", v2))
            //hmtext = new HmText(new HmPoint3D(bound.UpperLeft.X + 12.8, bound.LowerLeft.Y + 2.8), 2.0, string.Format("{0:F2}", v2))
            {
                Color = color,
                Label = CmdParmParser.GetLabelfromDictionary(straLabel),
                Layer = v.strlyr,
                Justify = AttachmentPoint.MiddleCenter
            };
            DrawHmEntity(hmtext, v, t2.box.GetBound());

            // 두께            
            straLabel["type"] = "두께";
            hmtext = new HmText(new HmPoint3D((t3.box.leftX + (t3.box.width) / 2) + 1.0, bound.LowerLeft.Y + 1.8), 2.0, string.Format("{0:F2}", v3))
            //hmtext = new HmText(new HmPoint3D(bound.UpperLeft.X + 23.8, bound.LowerLeft.Y + 2.8), 2.0, string.Format("{0:F2}", v3))
            {
                Color = color,
                Label = CmdParmParser.GetLabelfromDictionary(straLabel),
                Layer = v.strlyr,
                Justify = AttachmentPoint.MiddleCenter
            };
            DrawHmEntity(hmtext, v, t3.box.GetBound());
        }
        /// <summary>
        /// 통일분류에 해당하는 HmText를 그려줍니다.
        /// </summary>
        /// <param name="v"></param>
        /// <param name="ukey"></param>
        /// <param name="bound"></param>
        /// <param name="value"></param>
        /// <param name="color"></param>
        private void DrawHmTextType(LogTag v, uint ukey, HmBounds2D bound, string value, Color color)
        {
            if (value == "풍화암" || value == "연암" || value == "경암") return;

            // Label
            Dictionary<string, string> straLabel = new Dictionary<string, string>
            {
                { "db", "STRA" },
                { "var", v.xdatastr },
                { "key", ukey.ToString() }
            };

            if (value.Contains("-"))
            {
                List<string> varstr = value.Split('-').ToList();

                string mstr = string.Format("{0}\n{1}\n{2}", varstr[0], " ~", varstr[1]);
                HmMText mText = new HmMText(new HmPoint3D(bound.Center.X, bound.Center.Y, 0), 2.0, mstr)
                {
                    Layer = v.strlyr,
                    Label = v.xdatastr,
                    Color = color,
                    Attachment = AttachmentPoint.MiddleCenter
                };
                DrawHmEntity(mText, v, bound);
            }
            else
            {
                if (value == "NONE") return;

                HmText hmtext = new HmText(new HmPoint3D(bound.Center.X, bound.Center.Y, 0), 2.0, value)
                {
                    Layer = v.strlyr,
                    Label = CmdParmParser.GetLabelfromDictionary(straLabel),
                    Color = color,
                    Justify = AttachmentPoint.MiddleCenter
                };
                DrawHmEntity(hmtext, v, bound);
            }
        }
        /// <summary>
        /// 색조에 해당하는 HmText를 그려줍니다.
        /// </summary>
        /// <param name="v">logtag</param>
        /// <param name="bound">영역</param>
        /// <param name="value"></param>
        /// /// <param name="color">색상</param>
        private void DrawHmTextColor(LogTag v, uint ukey, HmBounds2D bound, List<Tuple<string, Brush>> straColor, Color color)
        {
            // Label
            Dictionary<string, string> straLabel = new Dictionary<string, string>
            {
                { "db", "STRA" },
                { "var", v.xdatastr },
                { "key", ukey.ToString() }
            };

            if (straColor.Count == 2)
            {
                string mstr = string.Format("{0}\n{1}\n{2}", straColor[0].Item1, " ~", straColor[1].Item1);
                HmMText mText = new HmMText(new HmPoint3D(bound.Center.X, bound.Center.Y, 0), 2.0, mstr)
                {
                    Layer = v.strlyr,
                    Label = v.xdatastr,
                    Color = color,
                    Attachment = AttachmentPoint.MiddleCenter
                };
                DrawHmEntity(mText, v, bound);
            }
            else if (straColor.Count == 1)
            {
                HmText hmtext = new HmText(new HmPoint3D(bound.Center.X, bound.Center.Y, 0), 2.0, straColor[0].Item1)
                {
                    Layer = v.strlyr,
                    Label = CmdParmParser.GetLabelfromDictionary(straLabel),
                    Color = color,
                    Justify = AttachmentPoint.MiddleCenter
                };
                DrawHmEntity(hmtext, v, bound);
            }
        }
        /// <summary>
        /// 주상도, 색조에 해당하는 HmHatch를 만들어줍니다.
        /// </summary>
        /// <param name="v">logtag</param>
        /// <param name="outer">해치폴리곤</param>
        /// <param name="color">색상</param>
        /// <returns></returns>
        private void DrawHmHatchSTRA(LogTag v, uint ukey, HmPolyline outer, string pattern, System.Windows.Media.Color color)
        {
            // Label
            Dictionary<string, string> straLabel = new Dictionary<string, string>
            {
                { "db", "STRA" },
                { "var", v.valkey },
                { "key", ukey.ToString() }
            };

            var hmhatch = new HmHatch
            {
                Origin = new HmPoint2D(outer.GetBounds().UpperLeft.X, outer.GetBounds().UpperLeft.Y),
                HatchObjectType = HatchObjectType.HatchObject,
                HatchStyle = HatchStyle.Normal,
                LoopTypes = HatchLoopTypes.Default,
                Label = CmdParmParser.GetLabelfromDictionary(straLabel),
                PatternScale = 8.0, // scale
                PatternAngle = 0.0, // angle 
                PatternType = HatchPatternType.PreDefined,
                Layer = v.strlyr,
                PatternName = pattern // 패턴이름 .pat에 정의                       
            };

            // 주상도 해치 패턴
            if (v.valkey == "주상도")
            {
                //hmhatch.ColorRgb = new byte[3] { 0, 0, 1 };
                hmhatch.ColorIndex = 7;
                hmhatch.BackgroundColor = new HmColor(new byte[3] { color.R, color.G, color.B }); // backcolor                
            }
            // 색조 해치
            else
            {
                hmhatch.ColorRgb = new byte[3] { color.R, color.G, color.B }; // forecolor
            }
            outer.Closed = true;
            (hmhatch.Geometry as HmGroup).Add(outer.Geometry);
            DrawHmEntity(hmhatch, v);
            dlog.canvas.Entities.ChangeOrder(hmhatch, true);
        }
#endregion

#region 지층설명(DESC)
        private void DrawDESC(List<uint> strakeylist, int page = 0)
        {
            LogTag v = dlogstyle.GetLogTag("지층설명");
            if (v == null)
                return;

            v.xdatastr = v.valkey;
            AddLayer(v.strlyr);
            DBDoc doc = DBDoc.Get_CurrDoc();

            double lastDepth = 0.0;
            if (!doc.stra.Get_LastDepth(strakeylist, ref lastDepth)) return;
#if true
            // 일반주상도의 높이
            LogStyleManager styleManager = App.GetLogStyleManager();
            Tuple<double, double> styleheight = styleManager.GetStyleHeight(curentDrillProperty.Drilltype);           
            double textHeight = (3.5 * styleheight.Item1) / (CalcDepth2CAD(0) - CalcDepth2CAD(styleheight.Item1)); // Text 1개당 높이

            // 더미와 시추종료를 포함한 Desc 리스트
            List<Tuple<double, string, uint, uint>> SortDesc = DescriptionUtil.SortDescription(strakeylist, styleheight.Item1, textHeight);
            // 더미와 시추종료를 제외한 순수 Desc 리스트
            List<Tuple<double, string, uint, uint>> forSortDesc = SortDesc.FindAll(x => x.Item3.Equals(0) == false).ToList();

            double start = 0.0, end = 0.0, py = 0.0;
            HmLine2D edgeline = new HmLine2D();

            for (int i = 0; i < forSortDesc.Count; i++)
            {
                double drawDepth = forSortDesc[i].Item1;
                uint currStrakey = forSortDesc[i].Item3;
                uint ukey = forSortDesc[i].Item4;
                if (!doc.stra.Get_Depth(currStrakey, ref start, ref end)) continue;

                if (drawDepth >= BegY && drawDepth <= FinY)
                {
                    if (forSortDesc.Count - 1 != i)
                    {
                        uint nextStrakey = forSortDesc[i + 1].Item3;
                        if (nextStrakey != currStrakey)
                        {
                            // stra가 바꼈을때 line을 그려줌
                            py = DrawSortDESCLine(v, start, end, drawDepth, currStrakey, ukey, ref edgeline, lastDepth);
                        }
                        else
                        {
                            // stra는 바끼지않았지만 지층이 넘어갓을때 line
                            double nextDepth = forSortDesc[i + 1].Item1;
                            if (nextDepth > FinY)
                            {
                                py = DrawSortDESCLine(v, start, end, drawDepth, currStrakey, ukey, ref edgeline, lastDepth);
                            }
                        }
                    }
                    else
                    {
                        // 마지막 line draw
                        py = DrawSortDESCLine(v, start, end, drawDepth, currStrakey, ukey, ref edgeline, lastDepth);
                    }
                    // text draw
                    DrawSortDESCText(v, forSortDesc[i], currStrakey, ukey, page);
                }
                else
                {
                    if (start < BegY && end > BegY)
                    {
                        // line draw
                        py = DrawSortDESCLine(v, start, end, drawDepth, currStrakey, ukey, ref edgeline, lastDepth);
                    }
                }
            }

            double calY = SortDesc.Find(a => a.Item2.Equals("^시추종료")).Item1;
            if (py != 0.0 && py - calY >= 0)
            {                
                if(lastDepth <= Math.Round(py, 2))
                {
                    DrawEndText(v, py, lastDepth);
                }                
            }
#else

            // 시추종료 심도
            double endDepth = 0.0f;
            // 현재 그릴위치 
            double currline = v.box.leftY;

            // 마지막심도의 이전페이지에 못그린 entity 그리기
            if (preStrlist.Count > 0)
            {
                currline = PrePageTextDraw(v, currline, page, ref endDepth);
            }

            // 시작 descline
            double descsaveline = v.box.leftY;
            // 시작 암질 절리간격line
            double rocksaveline = v.box.leftY;
            // 꺾인 line 저장용
            HmLine2D edgeline = new HmLine2D();
            
            for (int i = 0; i < strakeylist.Count; i++)
            {
                List<DBDataDESC> desclist = new List<DBDataDESC>();
                // 지층정보가져오기
                DBDataSTRA stra = null;
                if (!doc.stra.Get_Data(strakeylist[i], ref stra, true))
                    continue;
                if (stra.Depth == BegY) continue;

                // 심도 셋팅(시추종료)
                endDepth = stra.Depth;
                // 현재 페이지에 맞는 심도인지 확인
                if (BegY > stra.Depth || FinY < stra.Depth) continue;

                // depth 계산
                double currDepth = CalcDepth2CAD(stra.Depth);
                // 해당지층의 지층설명가져오기
                HmKeyList keylist = new HmKeyList();
                doc.desc.Get_KeyList(keylist, strakeylist[i]);
                if (!doc.desc.Get_DataList(keylist, ref desclist, true))
                    continue;

                // 암질, 절리간격 line
                DrawRockJointLine(currDepth, ref rocksaveline);

                for (int j = 0; j < desclist.Count; j++)
                {
                    // 지층설명 그리기
                    currline = DrawDESCInfo(v, keylist.list[j], desclist[j], currline, currDepth, page, (desclist.Count == 1 ? 0 : (j == 0 ? 1 : 2)));
                    // 지층설명이 여러개일때 암질, 절리간격 line
                    DrawRockJointLine(currDepth, ref rocksaveline, desclist[j]);
                    // TCR/RQD/절리간격 그리기
                    DrawTCRRQDJGAP(desclist[j], keylist.list[j], currDepth, ref rocksaveline);
                }
                // 지층설명 line
                uint desckey = keylist.list.Count > 0 ? keylist.list.Last() : 0;
                DrawDESCLine(v, desckey, strakeylist[i], currline, currDepth, ref descsaveline, ref edgeline);
            }
            // 시추종료 Text
            if (BegY <= endDepth && endDepth <= FinY)
            {                
                DrawEndText(v, descsaveline, endDepth);
            }
#endif
        }       
        private void DrawSortDESCText(LogTag v, Tuple<double, string, uint, uint> desc, uint strakey, uint ukey, int page)
        {
            DBDoc doc = DBDoc.Get_CurrDoc();
            double drawDepth = desc.Item1;
            double drawCADDepth = CalcDepth2CAD(drawDepth);

            foreach (string item in desc.Item2.Split('\n'))
            {
                string tmp = item.Trim();

                if (tmp.StartsWith("^"))
                {
                    if (tmp.Contains("페이지확인용")) continue;
                    if (tmp.Contains("시추종료")) continue;
                }

                Dictionary<string, string> descLabel = new Dictionary<string, string>
                {
                    { "db", "DESC" },
                    { "var", v.valkey },
                    { "key", ukey.ToString() },
                    { "strakey", strakey.ToString() }
                };
                
                bool isStartum = tmp.StartsWith("[");
                double position, height;
                string lyr = v.strlyr;
                Color color;
                if (isStartum)
                {
                    position = 2.0;
                    height = 2.0;
                    color = Color.DarkGreen;
                }
                else
                {
                    if (tmp.Equals("사용자 입력"))
                    {
                        position = 12.0;
                        height = 2.0;
                        color = Color.FromArgb(234, 234, 234);
                        descLabel["var"] = "NOTE";
                        lyr = "#NOTE";
                    }
                    else
                    {
                        position = 12.0;
                        height = 2.0;
                        color = Color.DarkBlue;
                    }
                }
                HmText t = new HmText(new HmPoint3D(v.box.leftX + position, drawCADDepth), height, tmp)
                {
                    Layer = lyr,
                    Label = CmdParmParser.GetLabelfromDictionary(descLabel),
                    Color = color,
                };
                DrawHmEntity(t, v);
            }
        }
        private double DrawSortDESCLine(LogTag v, double start, double end, double drawend, uint strakey, uint desckey, ref HmLine2D edgeline, double lastDepth)
        {
            if (FinY < end)
            {
                end = FinY;
            }

            HmPolyline2D poly2D = new HmPolyline2D();

            double startCAD = CalcDepth2CAD(start);
            // 다음페이지로 넘어가서 line그리는경우
            if (startCAD >= v.box.leftY)
            {
                startCAD = v.box.leftY;
            }

            double endCAD = CalcDepth2CAD(end);
            // 다음페이지로 넘어가서 line그리는경우
            if (endCAD >= v.box.leftY)
            {
                endCAD = v.box.leftY;
            }

            if (drawend >= BegY && drawend >= FinY - 0.5)
            {
                drawend = FinY;
            }
            double drawendCAD = CalcDepth2CAD(drawend);

            if (edgeline.StartPoint.X != 0.0 && edgeline.StartPoint.Y != 0.0)
            {
                poly2D.AddVertex(edgeline.StartPoint);
                poly2D.AddVertex(edgeline.EndPoint);
            }
            else
            {
                poly2D.AddVertex(new HmPoint2D(v.box.leftX, startCAD));
            }
            if (drawend > end)
            {
                if (edgeline.EndPoint.Y == 0.0)
                {
                    poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, startCAD));
                }
                else
                {
                    poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, edgeline.EndPoint.Y));
                }
                poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, drawendCAD));
                poly2D.AddVertex(new HmPoint2D(v.box.leftX + 3.0, drawendCAD));
                poly2D.AddVertex(new HmPoint2D(v.box.leftX, endCAD));
                poly2D.Closed = true;

                edgeline.Set(new HmPoint2D(v.box.leftX, endCAD), new HmPoint2D(v.box.leftX + 3.0, drawendCAD));
            }
            else
            {
                if (edgeline.EndPoint.Y == 0.0)
                {
                    poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, startCAD));
                }
                else
                {
                    poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, edgeline.EndPoint.Y));
                }
                poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, endCAD));
                poly2D.AddVertex(new HmPoint2D(v.box.leftX, endCAD));
                poly2D.Closed = true;
                edgeline = new HmLine2D();
            }

            Dictionary<string, string> descLabel = new Dictionary<string, string>
            {
                { "db", "DESC" },
                { "var", v.valkey },
                { "key", desckey.ToString() },
                { "strakey", strakey.ToString() }
            };

            // Add Entity
            var polyline = new HmPolyline(poly2D)
            {
                Label = CmdParmParser.GetLabelfromDictionary(descLabel),
                Layer = v.strlyr,
                Color = Color.DarkGreen
            };
            DrawHmEntity(polyline, v);

            double last = drawend > end ? drawendCAD : endCAD;
            return CalcCAD2Depth(last);
        }
        private void AddPage(List<Tuple<double, string, uint, uint>> SortDesc, int page)
        {
            foreach (Tuple<double, string, uint, uint> data in SortDesc)
            {
                double depth = data.Item1;
                if (FinY <= depth)
                {
                    // 페이지 있는지 확인 없으면 추가
                    if (!((BoringViewModel)DataContext).PageCounts.Any(a => a.Equals(page + 2)))
                    {
                        ((BoringViewModel)DataContext).PageCounts.Add(((BoringViewModel)this.DataContext).PageCounts.Count + 1);
                    }
                }
            }
        }
        /// <summary>
        /// 지층설명을 화면에 그려줍니다
        /// </summary>
        /// <param name="v">logtag</param>
        /// <param name="desc">desc</param>
        /// <param name="currline">그릴 캐드위치</param>
        /// <param name="currDepth">현재 desc에 해당하는 심도캐드좌표</param>
        /// <param name="ukey">desc key</param>
        /// <param name="nType">0=stra에 desc이 한개만 존재하는경우, 1=복수경우에 첫번째인경우, 2=복수경우에 두번째이후인경우</param>
        /// <param name="page">page</param>
        //private double DrawDESCInfo(LogTag v, uint ukey, DBDataDESC desc, double currline, double currDepth, int nType, int page = 0)
        //{
        // 지층설명 Text            
        //double py = DrawDESCText(v, ukey, desc.ToStringBuilder(nType), currline - 2.0, currDepth, page);
        //return py;
        //}
        //private void DrawDESCText(LogTag v, uint ukey, uint strakey, string data, ref double startline, double currDepth, bool isprepageDraw, int page = 0)
        //{
        //    startline -= 2.0;

        //    List<HmText> prestr = new List<HmText>();
        //    double cdepth = CalcCAD2Depth(currDepth);

        //    foreach (var item in data.Split('\n'))
        //    {
        //        // 다음페이지에 그릴 리스트와 중복체크
        //        if (CheckOverlap(strakey, item, startline))
        //        {
        //            startline -= 3.0;
        //            continue;
        //        }

        //        Dictionary<string, string> descLabel = new Dictionary<string, string>
        //        {
        //            { "db", "DESC" },
        //            { "var", v.valkey },
        //            { "key", ukey.ToString() },
        //            { "strakey", strakey.ToString() },
        //            { "page", page.ToString() },
        //            { "title", this.curentDrillProperty.Title },
        //            { "depth", cdepth.ToString() },
        //            { "boolstratum", "true" },
        //            { "currline", startline.ToString() }
        //        };

        //        string tmp = item.Trim();
        //        bool isStartum = tmp.StartsWith("[");
        //        double position;
        //        Color color;
        //        if (isStartum)
        //        {
        //            position = 2.0;
        //            color = Color.DarkGreen;
        //        }
        //        else
        //        {
        //            position = 15.0;
        //            color = Color.DarkBlue;
        //        }

        //        HmText t = new HmText(new HmPoint3D(v.box.leftX + position, startline), 2.0, tmp)
        //        {
        //            Layer = v.strlyr,
        //            Label = CmdParmParser.GetLabelfromDictionary(descLabel),
        //            Color = color,
        //        };

        //        startline -= t.Height + 1.0;  

        //        // 페이지 마지막라인을 넘어가는지 확인
        //        if (startline > AllLogTag.box.leftY - v.box.height)
        //        {
        //            if (isprepageDraw)
        //            {
        //                // Add Entity
        //                DrawHmEntity(t, v);
        //            }
        //        }
        //        // 넘어가면 다음페이지
        //        else
        //        {
        //            if (!isStartum)
        //            {
        //                descLabel["boolstratum"] = "false";
        //            }

        //            // 페이지 있는지 확인 없으면 추가
        //            if(!((BoringViewModel)DataContext).PageCounts.Any(a => a.Equals(page + 2)))
        //            {
        //                ((BoringViewModel)DataContext).PageCounts.Add(((BoringViewModel)this.DataContext).PageCounts.Count + 1);
        //            }

        //            descLabel["page"] = (page + 1).ToString();
        //            t.Label = CmdParmParser.GetLabelfromDictionary(descLabel);
        //            prestr.Add(t);
        //        }
        //    }

        //    startline = MakeNoteWaterMark(v, ukey, strakey, currDepth, startline, ref prestr, isprepageDraw, page);
        //    // 그리지못한 지층설명 리스트 add

        //    Dictionary<uint, List<HmText>> preDic = new Dictionary<uint, List<HmText>>()
        //    {
        //        { strakey, prestr }
        //    };

        //    if (prestr.Count > 0) preStrlist.Add(preDic);

        //    if (!isprepageDraw) startline = v.box.leftY;
        //    else
        //    {
        //        startline = startline > currDepth ? currDepth : startline;
        //    }            
        //}

        ///// <summary>
        ///// 지층설명의 Line을 그려줍니다.
        ///// </summary>
        ///// <param name="v">logtag</param>
        ///// <param name="ukey">desc key</param>
        ///// <param name="currline">현재 지층설명 line y값</param>
        ///// <param name="depth">현재 지층의 심도y값</param>
        //private void DrawDESCLine(LogTag v, uint desckey, uint strakey, ref double currline, double depth, ref double saveline, ref HmLine2D edgeline, bool isprepageDraw, bool isNextline)
        //{
        //    HmPolyline2D poly2D = new HmPolyline2D();
        //    // 페이지가 넘어가면
        //    if (preStrlist.Count > 0 && !isNextline)
        //    {
        //        if (edgeline.StartPoint.X != 0.0 && edgeline.StartPoint.Y != 0.0)
        //        {
        //            poly2D.AddVertex(edgeline.StartPoint);
        //            poly2D.AddVertex(edgeline.EndPoint);
        //            edgeline = new HmLine2D();
        //        }
        //        else
        //        {
        //            poly2D.AddVertex(new HmPoint2D(v.box.leftX, saveline));
        //            poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, saveline));
        //        }
        //        poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, AllLogTag.box.leftY - AllLogTag.box.height));
        //        poly2D.AddVertex(new HmPoint2D(v.box.leftX + 3.0, AllLogTag.box.leftY - AllLogTag.box.height));
        //        poly2D.AddVertex(new HmPoint2D(v.box.leftX, depth));
        //        poly2D.Closed = true;
        //    }
        //    else
        //    {
        //        if(isprepageDraw)
        //        {
        //            // 지층설명 Text line이 심도 line을 넘어서지않을때
        //            if (currline >= depth)
        //            {
        //                if (edgeline.StartPoint.X != 0.0 && edgeline.StartPoint.Y != 0.0)
        //                {
        //                    poly2D.AddVertex(edgeline.StartPoint);
        //                    poly2D.AddVertex(edgeline.EndPoint);
        //                    edgeline = new HmLine2D();
        //                }
        //                else
        //                {
        //                    poly2D.AddVertex(new HmPoint2D(v.box.leftX, saveline));
        //                }
        //                poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, saveline));
        //                poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, depth));
        //                poly2D.AddVertex(new HmPoint2D(v.box.leftX, depth));
        //                poly2D.Closed = true;
        //                // 다음line y값저장
        //                saveline = depth;
        //                currline = depth;
        //            }

        //            // 지층설명 Text line이 심도 line을 넘어설때(꺾인 line)
        //            else
        //            {
        //                if (edgeline.StartPoint.X != 0.0 && edgeline.StartPoint.Y != 0.0)
        //                {
        //                    poly2D.AddVertex(edgeline.StartPoint);
        //                    poly2D.AddVertex(edgeline.EndPoint);
        //                    edgeline = new HmLine2D();
        //                }
        //                else
        //                {
        //                    poly2D.AddVertex(new HmPoint2D(v.box.leftX, saveline));
        //                }
        //                poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, saveline));
        //                poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, currline));
        //                poly2D.AddVertex(new HmPoint2D(v.box.leftX + 3.0, currline));
        //                poly2D.AddVertex(new HmPoint2D(v.box.leftX, depth));
        //                poly2D.Closed = true;
        //                // 다음line y값저장
        //                saveline = currline;
        //                // 꺾인 line부분 저장
        //                edgeline.Set(new HmPoint2D(v.box.leftX, depth), new HmPoint2D(v.box.leftX + 3.0, currline));
        //            }
        //        }                
        //    }

        //    Dictionary<string, string> descLabel = new Dictionary<string, string>
        //    {
        //        { "db", "DESC" },
        //        { "var", v.valkey },
        //        { "key", desckey.ToString() },
        //        { "strakey", strakey.ToString() }
        //    };

        //    // Add Entity
        //    var polyline = new HmPolyline(poly2D)
        //    {
        //        Label = CmdParmParser.GetLabelfromDictionary(descLabel),
        //        Layer = v.strlyr,
        //        Color = Color.DarkGreen
        //    };
        //    DrawHmEntity(polyline, v);
        //}       
        ///// <summary>
        ///// 주상도 지층설명 이전페이지에서 못그린 text를 그려줍니다.
        ///// </summary>
        ///// <param name="v"></param>
        ///// <param name="currline"></param>
        ///// <param name="s">마지막 심도Setting</param>
        //private bool PrePageTextDraw(LogTag v, ref double currline, ref double saveline, ref HmLine2D edgeline, int page, double s)
        //{
        //    currline -= 2.0;

        //    uint ukey = 0;
        //    uint straukey = 0;

        //    bool isDraw = true;
        //    DBDataSTRA stra = new DBDataSTRA();
        //    List<HmText> prestr = new List<HmText>();
        //    double currDepth = 0.0;
        //    DBDoc doc = DBDoc.Get_CurrDoc();

        //    // 이전페이지에서 못그린텍스트 Draw            
        //    for (int i = 0; i < preStrlist.Count; i++)
        //    {
        //        foreach (var tt in preStrlist[i])
        //        {
        //            foreach(var t in tt.Value)
        //            {
        //                // 못그린 텍스트가 있는채로 다른 주상도를 불렀으면 못그린 텍스트는 제거한다                
        //                string titlestr = CmdParmParser.GetValuefromKey(t.Label, "title", '&');
        //                string pagestr = CmdParmParser.GetValuefromKey(t.Label, "page", '&');
        //                string bstratum = CmdParmParser.GetValuefromKey(t.Label, "boolstratum", '&');

        //                if (!bool.TryParse(bstratum, out bool boolStratum)) continue;

        //                // 공번이 바뀌었거나 같은 공번이라도 페이지가 다를땐 preStrlist Clear
        //                if (titlestr != this.curentDrillProperty.Title)
        //                {
        //                    preStrlist.Clear();
        //                    currline += 2.0;
        //                    return false;
        //                }
        //                if (pagestr != page.ToString())
        //                {
        //                    isDraw = false;
        //                    continue;
        //                }

        //                isDraw = true;
        //                // 심도 setting
        //                string depthstr = CmdParmParser.GetValuefromKey(t.Label, "depth", '&');
        //                if (!double.TryParse(depthstr, out s)) continue;

        //                string linestr = CmdParmParser.GetValuefromKey(t.Label, "currline", '&');
        //                if (!double.TryParse(depthstr, out double line)) continue;

        //                // key setting
        //                string key = CmdParmParser.GetValuefromKey(t.Label, "key", '&');
        //                if (!uint.TryParse(key, out ukey)) continue;

        //                // strakey setting
        //                string strakey = CmdParmParser.GetValuefromKey(t.Label, "strakey", '&');
        //                if (!uint.TryParse(key, out straukey)) continue;
        //                if (!doc.stra.Get_Data(straukey, ref stra, true)) return false;
        //                currDepth = stra.Depth;

        //                // Label setting
        //                Dictionary<string, string> descLabel = new Dictionary<string, string>
        //            {
        //                { "db", "DESC" },
        //                { "var", v.valkey },
        //                { "key", key },
        //                { "strakey", strakey },
        //                { "title", titlestr },
        //                { "page", pagestr },
        //                { "depth", s.ToString() },
        //                { "boolstratum", bstratum },
        //                { "currline", linestr }
        //            };
        //                Dictionary<string, string> noteLabel = new Dictionary<string, string>()
        //            {
        //                { "db", "DESC" },
        //                { "var", v.valkey },
        //                { "key", ukey.ToString() },
        //                { "strakey", strakey },
        //                { "depth", s.ToString() },
        //                { "title", this.curentDrillProperty.Title },
        //                { "page", page.ToString() },
        //                { "boolstratum", "false" }
        //            };
        //                // 못그린 text에서 심도가 나오면 새로운desc이므로 띄어줌                    
        //                if (t.TextString.Contains("심도")) currline -= 2.0;

        //                HmText hmtxt = null;
        //                if (t.TextString.Contains("NOTE"))
        //                {
        //                    hmtxt = new HmText(new HmPoint3D(v.box.leftX + 15.0, currline, 0.0), t.Height, t.TextString)
        //                    {
        //                        Label = CmdParmParser.GetLabelfromDictionary(noteLabel),
        //                        Color = Color.LightGray,
        //                        Layer = "#NOTE"
        //                    };
        //                }
        //                else
        //                {
        //                    if (boolStratum)
        //                    {
        //                        hmtxt = new HmText(new HmPoint3D(v.box.leftX + 2.0, currline, 0.0), t.Height, t.TextString)
        //                        {
        //                            Label = CmdParmParser.GetLabelfromDictionary(descLabel),
        //                            Color = Color.DarkGreen,
        //                            Layer = v.strlyr
        //                        };
        //                    }
        //                    else
        //                    {
        //                        hmtxt = new HmText(new HmPoint3D(v.box.leftX + 15.0, currline, 0.0), t.Height, t.TextString)
        //                        {
        //                            Label = CmdParmParser.GetLabelfromDictionary(descLabel),
        //                            Color = Color.DarkBlue,
        //                            Layer = v.strlyr
        //                        };
        //                    }
        //                }
        //                DrawHmEntity(hmtxt, v);
        //                currline -= hmtxt.Height + 1.0;
        //            }                    
        //        }

        //        if(isDraw)
        //        {                    
        //            // Note 작성                    
        //            //currline = MakeNoteWaterMark(v, ukey, currline, ref prestr, page);

        //            // 종료line                
        //            if (BegY >= currDepth || FinY <= currDepth)
        //            {
        //                PrePagelineDraw(v, currline, ref saveline, ref edgeline, ukey, straukey);
        //            }
        //        }
        //    }

        //    currline += 2.0;
        //    return isDraw;
        //}

        ///// <summary>
        ///// 주상도 지층설명 이전페이지에서 못그린 line을 그려줍니다.
        ///// </summary>
        ///// <param name="v"></param>
        ///// <param name="currline"></param>
        ///// <param name="ukey"></param>
        //private void PrePagelineDraw(LogTag v, double currline, ref double saveline, ref HmLine2D edgeline, uint ukey, uint straukey)
        //{
        //    // 종료line
        //    HmPolyline2D poly2D = new HmPolyline2D();

        //    if (edgeline.StartPoint.X != 0.0 && edgeline.StartPoint.Y != 0.0)
        //    {
        //        poly2D.AddVertex(edgeline.StartPoint);
        //        poly2D.AddVertex(edgeline.EndPoint);
        //    }
        //    else
        //    {
        //        poly2D.AddVertex(new HmPoint2D(v.box.leftX, saveline));
        //    }            
        //    poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, saveline));
        //    poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, currline));
        //    poly2D.AddVertex(new HmPoint2D(v.box.leftX + 3.0, currline));
        //    poly2D.Closed = true;

        //    // Label setting
        //    Dictionary<string, string> desclineLabel = new Dictionary<string, string>
        //    {
        //        { "db", "DESC" },
        //        { "var", v.valkey },
        //        { "key", ukey.ToString() },
        //        { "strakey", straukey.ToString() }
        //    };

        //    HmPolyline polyline = new HmPolyline(poly2D)
        //    {
        //        Label = CmdParmParser.GetLabelfromDictionary(desclineLabel),
        //        Layer = v.strlyr,
        //        Color = Color.DarkBlue
        //    };
        //    DrawHmEntity(polyline, v);
        //    // 꺾인 line부분 저장            
        //    if(edgeline.StartPoint.X == 0.0 && edgeline.StartPoint.Y == 0.0)
        //    {
        //        edgeline.Set(new HmPoint2D(v.box.leftX, saveline), new HmPoint2D(v.box.leftX + 3.0, currline));
        //    }
        //    edgeline.EndPoint = new HmPoint2D(v.box.leftX + 3.0, currline);
        //    saveline = currline;            
        //}        
        //private bool CheckOverlap(uint strakey, string item, double startline)
        //{            
        //    foreach (var sametext in preStrlist)
        //    {
        //        for (int pi = 0; pi < sametext.Count; pi++)
        //        {
        //            if (!sametext.ContainsKey(strakey)) continue;

        //            for (int pj = 0; pj < sametext[strakey].Count; pj++)
        //            {                        
        //                if (item.Contains("\r"))
        //                {
        //                    item = item.Split('\r')[0];
        //                }
        //                string seachstr = sametext[strakey][pj].TextString.Trim();
        //                if (!item.StartsWith("["))
        //                {
        //                    string linestr = CmdParmParser.GetValuefromKey(sametext[strakey][pj].Label, "currline", '&');
        //                    if (string.Compare(startline.ToString(), linestr) != 0) continue;
        //                    else
        //                    {
        //                        if (sametext[strakey][pj].TextString.Equals("NOTE 입력")) return true;

        //                        if (string.Compare(item, sametext[strakey][pj].TextString) != 0)
        //                        {
        //                            //sametext[strakey].Add()
        //                            sametext[strakey][pj].TextString = item;
        //                        }
        //                        return true;
        //                    }
        //                }
        //                else
        //                {
        //                    if (string.Compare(seachstr, item) == 0)
        //                        return true;
        //                }
        //            }                                    
        //        }
        //    }

        //    return false;
        //}
        ////private bool CheckCurrentPage(int currentpage)
        ////{
        ////    if (currentpage == 0) return true;

        ////    for (int i = 0; i < preStrlist.Count; i++)
        ////    {
        ////        foreach (var t in preStrlist[i])
        ////        {
        ////            string pagestr = CmdParmParser.GetValuefromKey(t.Label, "page", '&');
        ////            if (string.Compare(currentpage.ToString(), pagestr) == 0) return true;
        ////        }
        ////    }
        ////    return false;
        ////}
        ///// <summary>
        ///// 지층설명의 Note 작성할부분에 watermark text를 그려줍니다.
        ///// </summary>
        ///// <param name="v"></param>
        ///// <param name="currline"></param>
        ///// <param name="page"></param>
        ///// <returns></returns>
        //private double MakeNoteWaterMark(LogTag v, uint ukey, uint straykey, double depth, double currline, ref List<HmText> prestr, bool isprepageDraw, int page)
        //{
        //    AddLayer("#NOTE");

        //    double vv = CalcCAD2Depth(depth);
        //    // 다음페이지에 그릴 리스트와 중복체크
        //    if (CheckOverlap(straykey, "NOTE 입력", currline)) return currline -= 3.5;

        //    Dictionary<string, string> noteLabel = new Dictionary<string, string>()
        //    {
        //        { "db", "DESC" },
        //        { "var", "NOTE" },
        //        { "key", ukey.ToString() },
        //        { "strakey", straykey.ToString() },
        //        { "depth", vv.ToString() },
        //        { "title", this.curentDrillProperty.Title },
        //        { "page", page.ToString() },
        //        { "boolstratum", "false" },
        //        { "currline", currline.ToString() }
        //    };

        //    HmText notetxt = new HmText(new HmPoint3D(v.box.leftX + 15.0, currline), 3.0, "NOTE 입력")
        //    {
        //        Layer = "#NOTE",
        //        Label = CmdParmParser.GetLabelfromDictionary(noteLabel),
        //        Color = Color.LightGray
        //    };
        //    // 페이지 마지막라인을 넘어가는지 확인
        //    if (currline > AllLogTag.box.leftY - v.box.height)
        //    {
        //        if(isprepageDraw)
        //        {
        //            // Add Entity
        //            DrawHmEntity(notetxt, v);
        //        }
        //    }
        //    // 넘어가면 다음페이지
        //    else
        //    {
        //        bool isOverlap = false;
        //        //CmdParmParser.GetValuefromKey(x.Label, "depth", '&');
        //        for(int i = 0; i < preStrlist.Count; i++)
        //        {
        //            if (!preStrlist[i].ContainsKey(straykey)) continue;

        //            HmText note = preStrlist[i][straykey].Find(x => (CmdParmParser.GetValuefromKey(x.Label, "depth", '&') == vv.ToString()).Equals(true));
        //            if(note != null)
        //            {
        //                isOverlap = true;
        //            }
        //        }
        //        if(!isOverlap)
        //        {
        //            noteLabel["page"] = (page + 1).ToString();
        //            notetxt.Label = CmdParmParser.GetLabelfromDictionary(noteLabel);
        //            prestr.Add(notetxt);
        //        }
        //    }

        //    return currline -= notetxt.Height + 0.5;
        //}
#endregion

#region SPT정보(SPTG)
        private void DrawSPTG(uint ukey, int page = 0)
        {
            DBDataSPTG d = null;
            DBDoc doc = DBDoc.Get_CurrDoc();
            if (!doc.sptg.Get_Data(ukey, ref d, true))
                return;

            DrawSPTGInfo(d, ukey, page);
        }
        /// <summary>
        /// SPT 정보를 그려줍니다
        /// </summary>
        /// <param name="sptg"></param>
        private void DrawSPTGInfo(DBDataSPTG sptg, uint ukey, int page = 0)
        {
            // 타격횟수/관입량(표준관입시험)
            DrawSPTGHitCount(sptg, ukey, page);
            // N-VALUE
            DrawSPTGNvalue(sptg, ukey, page);
        }
        /// <summary>
        /// 타격횟수 정보를 그려줍니다.
        /// </summary>
        /// <param name="sptg"></param>
        /// <param name="ukey"></param>
        /// <param name="page"></param>
        private void DrawSPTGHitCount(DBDataSPTG sptg, uint ukey, int page = 0)
        {
            LogTag sptgLogtag = dlogstyle.GetLogTag("타격횟수");
            if (sptgLogtag == null)
                return;

            AddLayer(sptgLogtag.strlyr);
            DrawHmTextSPTG(sptgLogtag, sptg.SptList, ukey, Color.DarkGray);
        }
        /// <summary>
        /// NVALUE 정보를 그려줍니다.
        /// </summary>
        /// <param name="sptg"></param>
        /// <param name="ukey"></param>
        /// <param name="page"></param>
        private void DrawSPTGNvalue(DBDataSPTG sptg, uint ukey, int page = 0)
        {
            LogTag nLogtag = dlogstyle.GetLogTag("NVALUE");
            if (nLogtag == null)
                return;

            AddLayer(nLogtag.strlyr);
            DrawPolyPointSPTG(nLogtag, sptg.SptList, ukey, Color.DarkBlue);
        }
        /// <summary>
        /// SPT의 텍스트를 그려줍니다.
        /// </summary>
        /// <param name="v">logtag</param>
        /// <param name="sptlist">spt 리스트</param>
        /// <param name="color">색상</param>
        private void DrawHmTextSPTG(LogTag v, List<Tuple<double, int, double>> sptlist, uint ukey, Color color)
        {
            double zlevel;
            for (int i = 0; i < sptlist.Count; i++)
            {
                zlevel = sptlist[i].Item1;
                if (BegY > zlevel || FinY < zlevel)
                    continue;

                string value = string.Format("{0}/{1}", sptlist[i].Item2, sptlist[i].Item3);
                double py = CalcDepth2CAD(zlevel);

                Dictionary<string, string> diclabel = new Dictionary<string, string>
                {
                    { "db", "SPTG" },
                    { "var", v.valkey },
                    { "key", ukey.ToString() },
                    { "index", i.ToString() },
                    { "zlv", zlevel.ToString() },
                };

                HmText hmtext = new HmText(new HmPoint3D(v.box.leftX + v.box.width / 2, py), 2.0, string.Format("{0:F2}", value))
                {
                    Justify = AttachmentPoint.MiddleCenter,
                    Color = color,
                    Label = CmdParmParser.GetLabelfromDictionary(diclabel),
                    Layer = v.strlyr
                };
                DrawHmEntity(hmtext, v);

                addInvisibleBox(new Point(v.box.leftX, py + hmtext.Height), v.box.width, hmtext.Height * 2, hmtext.Label, hmtext.Layer);
            }
        }

        private void addInvisibleBox(Point position, double width, double height, string label, string layer)
        {
            HmPolyline2D poly2D = new HmPolyline2D();
            poly2D.AddVertex(new HmPoint2D(position.X, position.Y));
            poly2D.AddVertex(new HmPoint2D(position.X, position.Y - height));
            poly2D.AddVertex(new HmPoint2D(position.X + width, position.Y - height));
            poly2D.AddVertex(new HmPoint2D(position.X + width, position.Y));
            poly2D.Closed = true;

            var polyline = new HmPolyline(poly2D)
            {
                Label = label,
                Layer = layer,
                Transparency = new HmTransparency() { Alpha = 0 },
                Color = Color.DarkGreen
            };
            DrawHmEntity(polyline, null);
        }

        /// <summary>
        /// SPT line과 point를 그려줍니다
        /// </summary>
        /// <param name="v">logtag</param>
        /// <param name="sptlist">spt 리스트</param>
        /// <param name="color">색상</param>
        private void DrawPolyPointSPTG(LogTag v, List<Tuple<double, int, double>> sptlist, uint ukey, Color color)
        {
            HmPolyline2D poly2D = new HmPolyline2D();
            HmPolyline2D secpoly2D = new HmPolyline2D();
            for (int i = 0; i < sptlist.Count; i++)
            {
                if (BegY > sptlist[i].Item1 || FinY < sptlist[i].Item1)
                    continue;
                if (sptlist[i].Item2 > dlogstyle.maxNValue)
                    return;

                if (i + 1 == sptlist[i].Item1)
                {
                    AddPointPolyline(v, sptlist[i].Item2, sptlist[i].Item1, poly2D, color, ukey, i);
                }
                else
                {
                    AddPointPolyline(v, sptlist[i].Item2, sptlist[i].Item1, secpoly2D, color, ukey, i);
                }

            }
            if (poly2D.VertexCount > 0)
            {
                HmPolyline poly = new HmPolyline(poly2D)
                {
                    Layer = v.strlyr,
                    Color = color,
                    Label = string.Format("&db=SPTG&var={0}", v.xdatastr),
                    LineWeight = LineWeight.LineWeight040,
                };
                DrawHmEntity(poly, v);
            }
            if (secpoly2D.VertexCount > 0)
            {
                HmPolyline poly = new HmPolyline(secpoly2D)
                {
                    Layer = v.strlyr,
                    Color = color,
                    Label = string.Format("&db=SPTG&var={0}", v.xdatastr),
                    LineWeight = LineWeight.LineWeight040
                };
                DrawHmEntity(poly, v);
            }
        }
        private void AddPointPolyline(LogTag v, int hitcnt, double depth, HmPolyline2D poly2D, Color color, uint ukey, int i)
        {
            // point
            double px = CalcSpt(hitcnt);
            double py = CalcDepth2CAD(depth);

            Dictionary<string, string> diclabel = new Dictionary<string, string>
            {
                { "db", "SPTG" },
                { "var", v.valkey },
                { "key", ukey.ToString() },
                { "index", i.ToString() }
            };

            HmCircle circle = new HmCircle(new HmPoint3D(px, py, 0.0), 0.3)
            {
                Layer = v.strlyr,
                Color = color,
                Label = CmdParmParser.GetLabelfromDictionary(diclabel)
            };
            DrawHmEntity(circle, v);

            var hmhatch = new HmHatch
            {
                Origin = new HmPoint2D(circle.Center.X, circle.Center.Y),
                HatchObjectType = HatchObjectType.HatchObject,
                HatchStyle = HatchStyle.Normal,
                LoopTypes = HatchLoopTypes.Default,
                Label = CmdParmParser.GetLabelfromDictionary(diclabel),
                PatternScale = 1.0, // scale
                PatternAngle = 0.0, // angle 
                PatternType = HatchPatternType.PreDefined,
                Layer = v.strlyr,
                PatternName = "SOLID", // 패턴이름 .pat에 정의     
                ColorRgb = new byte[3] { color.R, color.G, color.B }, // forecolor                    
            };
            (hmhatch.Geometry as HmGroup).Add(circle.Geometry);            
            DrawHmEntity(hmhatch, v);

            // polyline
            poly2D.AddVertex(new HmPoint2D(px, py));
        }
#endregion

#region 시료형태(SAMP)
        private void DrawSAMP(uint ukey, int page = 0)
        {
            List<DBDataSAMP> samplist = new List<DBDataSAMP>();
            DBDoc doc = DBDoc.Get_CurrDoc();
            HmKeyList keylist = new HmKeyList();
            doc.samp.Get_KeyList(keylist, ukey);

            if (!doc.samp.Get_DataList(keylist, ref samplist, true))
                return;

            DrawSAMPInfo(samplist, keylist, page);
        }
        private void DrawSAMPInfo(List<DBDataSAMP> samplist, HmKeyList keyList, int page = 0)
        {
            LogTag v = dlogstyle.GetLogTag("시료형태");
            if (v == null)
                return;

            double pagefr = BegY, pageto = FinY;// 페이지의 시작과 끝깊이
            double zzfr = 0.0, zzto = 0.0;      // 지층의 시작과 끝깊이
            double zbeg, zfin;                  // 페이지에 그리는 지층의 시작과 끝
            double dwgz1, dwgz2;

            v.xdatastr = v.strlyr.Replace("#", "");
            AddLayer(v.strlyr);
            for (int i = 0; i < keyList.Count; i++)
            {
                zzto = samplist[i].Depth;
                if (zzto < pagefr || zzfr > pageto) return;

                // 화면에 그릴때 사용할 지층의 시작깊이
                zbeg = pagefr > zzfr ? pagefr : zzfr;
                // 화면에 그릴때 사용할 지층의 종료깊이
                zfin = zzto > pageto ? pageto : zzto;

                // 그릴영역계산
                LogTag calcBound = v.Clone();
                dwgz1 = CalcDepth2CAD(zbeg);
                dwgz2 = CalcDepth2CAD(zfin);
                calcBound.box.Set(v.box.leftX, dwgz1, v.box.width, dwgz1 - dwgz2);

                // 그리기
                DrawHmBlock(calcBound, samplist[i], keyList.list[i], 0.0, 0.0);
                // 다음 지층의 시작깊이는 이번층의 종료깊이가 됨
                zzfr = zzto;
            }
        }
        private void DrawHmBlock(LogTag v, DBDataSAMP samp, uint ukey, double scale, double rotation)
        {
            // samp label
            Dictionary<string, string> sampLabel = new Dictionary<string, string>
            {
                { "db", "SAMP" },
                { "var", v.valkey },
                { "key", ukey.ToString() },
                { "zlv", samp.Depth.ToString() },
            };

            HmBlockReference blkRef = new HmBlockReference(samp.SType.ToString())
            {
                Position = new HmPoint3D(v.box.leftX + 5.0, v.box.leftY - v.box.height, 0.0),
                Rotation = rotation,
                Layer = v.strlyr,
                Label = CmdParmParser.GetLabelfromDictionary(sampLabel),
                Color = Color.DarkBlue
            };
            blkRef.Scale(scale, blkRef.Position);

            if (blkRef != null)
            {
                DrawHmEntity(blkRef, v);
                addInvisibleBox(new Point(v.box.leftX, blkRef.Bounds.UpperLeft.Y + 1), v.box.width, blkRef.Bounds.Height + 2.0, blkRef.Label, blkRef.Layer);
            }
        }
#endregion

#region 절리형상(JSHP) 
#if true
        private void DrawJSHP(List<uint> strakeylist, int page = 0)
        {
            LogTag v = dlogstyle.GetLogTag("절리형상");
            if (v == null)
                return;
            AddLayer(v.strlyr);

            DBDoc doc = DBDoc.Get_CurrDoc();
            DBDataSTRA straD = null;
            DBDataJSHP vvvvD = null;
            List<DBDataJSHP> vvvvlist = new List<DBDataJSHP>();

            double pagefr = BegY, pageto = FinY;// 페이지의 시작과 끝깊이
            double zzfr = 0.0, zzto = 0.0;      // 지층의 시작과 끝깊이
            double zbeg, zfin;                  // 페이지에 그리는 지층의 시작과 끝
            double dwgz1, dwgz2;
            string msgstr = string.Empty;

            int j, jsize, i, isize = strakeylist.Count;
            for (i = 0; i < isize; i++)
            {
                // 지층정보가져오기
                if (!doc.stra.Get_Data(strakeylist[i], ref straD, true))
                    continue;
                if (straD.Depth == 0) return;

                // 리스트 청소
                vvvvlist.Clear();

                // 해당지층의 지층설명 가져오기
                HmKeyList keylist = new HmKeyList();
                doc.desc.Get_KeyList(keylist, strakeylist[i]);
                if (doc.jshp.Get_DataList(keylist, ref vvvvlist, true))
                {
                    jsize = vvvvlist.Count;
                }
                else
                    jsize = 0;

                // 1. 지층전체 박스 그리기

                // 이번 층의 지층종료깊이 계산
                zzto = straD.Depth;

                // 현지층이 페이지 사이에 있지 않으면 통과
                //       
                //     ┌────────────┓ zzfr
                //     |            |     제외(X)
                //     └────────────┘ zzto
                //         
                //       ┌────────┓ 페이지시작(pagefr)
                //       |        |
                //     ┌─┼────────┼─┐  zzfr
                //     | |        | |     고려(O)
                //     └─┼────────┼─┘  zzto
                //       |        |
                //       └────────┘ 페이지종료(pageto)
                //
                //     ┌────────────┓ zzfr
                //     |            |     제외(X)
                //     └────────────┘ zzto
                //
                if (zzto < pagefr || zzfr > pageto)
                    continue;

                //
                //       ┌────────┓ 페이지시작(pagefr)
                //       |        |
                //     ┌─┼────────┼─┐  zbeg
                //     | |        | |
                //     └─┼────────┼─┘  zfin
                //       |        |
                //       └────────┘ 페이지종료(pageto)

                // 화면에 그릴때 사용할 지층의 시작깊이
                if (pagefr > zzfr)
                    zbeg = pagefr;
                else
                    zbeg = zzfr;
                // 화면에 그릴때 사용할 지층의 종료깊이
                if (zzto > pageto)
                    zfin = pageto;
                else
                    zfin = zzto;

                // 그릴영역계산
                LogTag vv = v.Clone();
                dwgz1 = CalcDepth2CAD(zbeg);
                dwgz2 = CalcDepth2CAD(zfin);
                vv.box.Set(v.box.leftX, dwgz1, v.box.width, dwgz1 - dwgz2);

                // 데이터그리기(여기서는 빈박스만 그리고 아래에서 형상을 그린다)
                DrawJSHP(vv, 0, strakeylist[i], null, zbeg, zfin);

                // 2. 세부지질형상 그리기
                {
                    double zfr = zzfr, zto, zthick;
                    // 지층종료깊이 계산
                    zfr = zzfr;
                    for (j = 0; j < vvvvlist.Count; j++)
                    {
                        vvvvD = vvvvlist[j];
                        //vvvvD.thick = 3.0;// 임시

                        // 시작위치
                        zfr = vvvvD.depth;
                        zthick = vvvvD.thick;

                        // 종료위치
                        zto = zfr + zthick;

                        // 현지층이 페이지 사이에 있지 않으면 통과
                        if (zto < pagefr || zfr > pageto)
                            continue;

                        // 화면에 그릴때 사용할 지층의 시작깊이
                        if (pagefr > zfr)
                            zbeg = pagefr;
                        else
                            zbeg = zfr;
                        // 화면에 그릴때 사용할 지층의 종료깊이
                        if (zto > pageto)
                            zfin = pageto;
                        else
                            zfin = zto;

                        // 그릴영역계산
                        vv = v.Clone();
                        dwgz1 = CalcDepth2CAD(zbeg);
                        dwgz2 = CalcDepth2CAD(zfin);
                        vv.box.Set(v.box.leftX, dwgz1, v.box.width, dwgz1 - dwgz2);

                        // 데이터그리기(지질형상 포함)
                        DrawJSHP(vv, keylist[j], strakeylist[i], vvvvD, zbeg, zfin, msgstr);

                        // 다음 지층의 시작깊이는 이번층의 종료깊이가 됨
                        zfr = zto;
                    }
                }

                // 다음 지층의 시작깊이는 이번층의 종료깊이가 됨
                zzfr = zzto;
            }
        }
        private void DrawJSHP(LogTag v, uint ukey, uint strakey, DBDataJSHP vvvvD, double zbeg, double zfin, string msgstr = "")
        {
            // 0. XDATA 준비
            Dictionary<string, string> entLabel = new Dictionary<string, string>
            {
                { "db", "JSHP" },
                { "var", v.valkey },
                { "key", ukey.ToString() },
                { "strakey", strakey.ToString() }
            };

            // 1. 박스 그리기
            HmPolyline2D poly2D = new HmPolyline2D();
            poly2D.AddVertex(new HmPoint2D(v.box.leftX, v.box.leftY));
            poly2D.AddVertex(new HmPoint2D(v.box.leftX, v.box.leftY - v.box.height));
            poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, v.box.leftY - v.box.height));
            poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, v.box.leftY));
            poly2D.Closed = true;
            // Add Entity
            var polyline = new HmPolyline(poly2D)
            {
                Label = CmdParmParser.GetLabelfromDictionary(entLabel),
                Layer = v.strlyr,
                Color = Color.DarkGreen
            };
            DrawHmEntity(polyline, v);

            // 2. 박스안에 절리형상 출력
            if (null != vvvvD && null != vvvvD.img)
            {
                //
                //  (0,0)
                //  ┌────────────┓                ┌────────────┓ z1  (절리 시작)        ───┬─── 
                //  |            |                |            |                         
                //  |            |   v.box.leftY  |            |                           
                //  ├────────────┼────────────────┼────────────┤ zbeg(그리기 시작)        
                //  | 이미지기준  |                |            |                           
                //  |  HmLine2D  |   v.box.Height |  그릴영역   |                        thick    
                //  |            |                |            |                         
                //  ├────────────┼────────────────┼────────────┤ zfin(그리기 끝)          
                //  |            |                |            |                         
                //  |            |                |            |                         
                //  └────────────┘                └────────────┘ z2  (절리 끝)          ───┴───
                //               (iw,ih)       
                //                                 v.box.Width
                //
                //
                double thick = vvvvD.thick;
                double z1 = vvvvD.depth;
                double z2 = z1 + thick;


                double ox = v.box.leftX;
                double oy = v.box.leftY;
                double ix, iy;
                double sx, sy, zz1;
                double ex, ey, zz2;

                // 절리형상 크기
                double iw = vvvvD.img.Image.Width;
                double ih = vvvvD.img.Image.Height;

                double nw = v.box.width;
                //double nh = 3.0;// jshp.thick;

                foreach (HmLine2D pline in vvvvD.lines)
                {
                    // 1) 시점좌표뱐환
                    // 이미지 기준 시점
                    ix = pline.StartPoint.X;
                    iy = pline.StartPoint.Y;
                    // 이미지기준 -> 도면
                    sx = ox + (nw / iw) * ix;
                    //sy = oy - (nh / ih) * yy;
                    // 도면 -> 실제
                    zz1 = z1 + (iy / ih) * thick;
                    sy = CalcDepth2CAD(zz1);

                    // 2) 종점좌표뱐환
                    // 이미지 기준 종점
                    ix = pline.EndPoint.X;
                    iy = pline.EndPoint.Y;
                    // 이미지기준 -> 도면
                    ex = ox + (nw / iw) * ix;
                    //ey = oy - (nh / ih) * yy;
                    // 도면 -> 실제
                    zz2 = z1 + (iy / ih) * thick;
                    ey = CalcDepth2CAD(zz2);

                    if (zz1 >= zbeg && zz1 <= zfin)
                    {
                        if (zz2 >= zbeg && zz2 <= zfin)
                        {
                            // 시점종점 모두 표시 범위안에 들어오면
                            HmLine newline = new HmLine(new HmPoint3D(sx, sy), new HmPoint3D(ex, ey))
                            {
                                Label = CmdParmParser.GetLabelfromDictionary(entLabel),
                                Layer = v.strlyr,
                                Color = Color.DarkBlue
                            };
                            DrawHmEntity(newline, v);
                        }
                    }
                }
            }
        }
#else
        private void DrawJSHP(List<uint> strakeylist, int page = 0)
        {
            LogTag v = dlogstyle.GetLogTag("절리형상");
            if (v == null)
                return;

            v.xdatastr = v.valkey;
            AddLayer(v.strlyr);
            DBDoc doc = DBDoc.Get_CurrDoc();
            
            // jshpline 저장용
            double jshpsaveline = v.box.leftY;

            for (int i = 0; i < strakeylist.Count; i++)
            {
                List<DBDataJSHP> jshplist = new List<DBDataJSHP>();

                // 지층정보가져오기
                DBDataSTRA stra = null;
                if (!doc.stra.Get_Data(strakeylist[i], ref stra, true))
                    continue;

                // 현재 페이지에 맞는 심도인지 확인
                if (BegY > stra.Depth || FinY < stra.Depth)
                    continue;
                // depth 계산
                double currDepth = CalcDepth2CAD(stra.Depth);

                // 해당지층의 절리형상가져오기
                HmKeyList jshpkeylist = new HmKeyList();
                doc.jshp.Get_KeyList(jshpkeylist, strakeylist[i]);
                if (!doc.jshp.Get_DataList(jshpkeylist, ref jshplist, true))
                    continue;

                for (int j = 0; j < jshplist.Count; j++)
                {                    
                    // 절리형상 그리기
                    DrawJSHPInfo(v, jshpkeylist.list[j], jshplist[j], ref jshpsaveline, page);               
                }
                // 절리형상 line
                uint jshpkey = jshpkeylist.Count > 0 ? jshpkeylist.list.Last() : 0;
                DrawJSHPLine(v, jshpkey, strakeylist[i], currDepth, ref jshpsaveline);
            }
        }
        /// <summary>
        /// 절리형상을 화면에 그려줍니다
        /// </summary>
        /// <param name="v">logtag</param>
        /// <param name="desc">desc</param>
        /// <param name="currline">그릴 캐드위치</param>
        /// <param name="currDepth">현재 desc에 해당하는 심도캐드좌표</param>
        /// <param name="ukey">desc key</param>
        /// <param name="page">page</param>
        private void DrawJSHPInfo(LogTag v, uint ukey, DBDataJSHP jshp, ref double currline, int page = 0)
        {
            if (null == jshp.img)
                return;

            Dictionary<string, string> jshpLabel = new Dictionary<string, string>
            {
                { "db", "JSHP" },
                { "var", v.valkey },
                { "key", ukey.ToString() }
            };

            double ox = v.box.leftX;
            double oy = currline;
            double xx, yy;
            double sx, sy;
            double ex, ey;

            double iw = jshp.img.Image.Width;
            double ih = jshp.img.Image.Height;

            double nw = v.box.width;
            double nh = 3.0;// jshp.thick;

            foreach (HmLine2D pline in jshp.lines)
            {
                xx = pline.StartPoint.X;
                yy = pline.StartPoint.Y;

                sx = ox + (nw / iw) * xx;
                sy = oy - (nh / ih) * yy;

                xx = pline.EndPoint.X;
                yy = pline.EndPoint.Y;

                ex = ox + (nw / iw) * xx;
                ey = oy - (nh / ih) * yy;

                HmLine newline = new HmLine(new HmPoint3D(sx, sy), new HmPoint3D(ex, ey))
                {
                    Label = CmdParmParser.GetLabelfromDictionary(jshpLabel),
                    Layer = v.strlyr,
                    Color = Color.DarkBlue
                };
                DrawHmEntity(newline, v);
            }
        }

#endif
#endregion

#region 기타
        private void AddLayer(string strlyr)
        {
            HmLayer hmLayer = new HmLayer(strlyr)
            {
                Label = strlyr
            };
            dlog.canvas.Layers.Add(hmLayer);
        }
        /// <summary>
        /// 주상도에 영역에 HmPolyline을 그려줍니다.
        /// </summary>
        /// <param name="v">logtag</param>
        /// <param name="py">y값</param>
        /// <param name="topline">시작line</param>
        /// <param name="color">색상</param>
        private HmPolyline DrawHmPolygon(LogTag v, uint ukey, double py, Color color, double currtext = 0.0)
        {
            HmPolyline2D poly2D = new HmPolyline2D();

            poly2D.AddVertex(new HmPoint2D(v.box.leftX, v.box.leftY));
            poly2D.AddVertex(new HmPoint2D(v.box.leftX, v.box.leftY - v.box.height));
            poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, v.box.leftY - v.box.height));
            poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, v.box.leftY));
            poly2D.Closed = true;

            // stra label
            Dictionary<string, string> straLabel = new Dictionary<string, string>
            {
                { "db", "STRA" },
                { "var", v.valkey },
                { "key", ukey.ToString() },
                { "currtext", currtext.ToString() }
            };

            HmPolyline polyline = new HmPolyline(poly2D)
            {
                Label = CmdParmParser.GetLabelfromDictionary(straLabel),
                Layer = v.strlyr,
                Color = color,
            };
            DrawHmEntity(polyline, v);

            return polyline;
        }

        private void DrawHmEntity(HmEntity entity, LogTag logtag, HmBounds2D outsideline = null)
        {
            switch (entity)
            {
                case HmText text:
                    {
                        if (outsideline == null) break;
                        while (true)
                        {
                            if (text.Height < 0)
                            {
                                text.Height = 1.0;
                                break;
                            }
                            if (outsideline.Include(text.Geometry.Get_Bounds2D()) != 1)
                            {
                                text.Height -= 0.05;
                                break;
                            }
                            else
                            {
                                text.Height -= 0.1;
                            }
                        }
                    }
                    break;
                case HmMText mtext:
                    {
                        if (outsideline == null) break;
                        while (true)
                        {
                            if (mtext.TextHeight < 0)
                            {
                                mtext.TextHeight = 1.0;
                                break;
                            }
                            if (outsideline.Include(mtext.Geometry.Get_Bounds2D()) != 1)
                            {
                                mtext.TextHeight -= 0.05;
                                break;
                            }
                            else
                            {
                                mtext.TextHeight -= 0.1;
                            }
                        }
                    }
                    break;
                case HmPolyline polyline:
                case HmHatch hatch:
                case HmBlockReference block:
                case HmCircle circle:
                    break;
            }

            dlog.canvas.Entities.Add(entity);
        }
        /// <summary>
        /// 시료형태와 타격횟수에 entity가 추가되면 양식바운더리를 제거하는 함수
        /// </summary>
        /// <param name="entityBound">추가한 엔티티의 바운드</param>
        /// <param name="layer"></param>
        private void RemoveBound(HmBounds2D entityBound, string layer)
        {
            if (entityBound == null) return;

            IEnumerable<HmEntity> hmEntities = from e in dlog.canvas.Entities.Cast<HmEntity>()
                                               where e.Layer == layer
                                               select e;

            foreach (var v in hmEntities)
            {
                if (v is HmPolyline)
                {
                    HmPolyline bound = v as HmPolyline;
                    if (bound.GetBounds().Include(entityBound) == 2)
                    {
                        dlog.canvas.Entities.Remove(v);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// 시추종료 텍스트를 그려줍니다.
        /// </summary>
        /// <param name="py">그릴 위치</param>
        /// <param name="valstr">마지막 심도</param>
        private void DrawEndText(LogTag v, double py, double endstr)
        {
            double calcline = endstr == FinY ? 3.0 : -3.0;
            double calctext = endstr == FinY ? 2.3 : -0.5;
            py = CalcDepth2CAD(py);

            // 바운더리 setting
            HmPolyline outer = new HmPolyline();
            outer.Vertices.Add(new HmVertex2D(new HmPoint2D(v.box.leftX, py)));
            outer.Vertices.Add(new HmVertex2D(new HmPoint2D(v.box.leftX + v.box.width, py)));
            outer.Vertices.Add(new HmVertex2D(new HmPoint2D(v.box.leftX + v.box.width, py + calcline)));
            outer.Vertices.Add(new HmVertex2D(new HmPoint2D(v.box.leftX, py + calcline)));
            outer.Closed = true;

            // hatch setting
            var hmhatch = new HmHatch
            {
                Origin = new HmPoint2D(outer.GetBounds().UpperLeft.X, outer.GetBounds().UpperLeft.Y),
                HatchObjectType = HatchObjectType.HatchObject,
                HatchStyle = HatchStyle.Normal,
                LoopTypes = HatchLoopTypes.Default,
                PatternScale = 1,
                PatternAngle = 0.0,
                PatternType = HatchPatternType.PreDefined,                
                ColorIndex = 7,
                PatternName = "SOLID",
                Layer = v.strlyr,
            };
            (hmhatch.Geometry as HmGroup).Add(outer.Geometry);
            DrawHmEntity(hmhatch, v);

            // text setting            
            string varstr = dlogstyle.GetMessageBoringTerminate(endstr);
            var hmtext = new HmText(new HmPoint3D(v.box.leftX + 12.0, py + calctext, 0), 2.0, varstr)
            {
                Color = Color.Yellow,
            };
            DrawHmEntity(hmtext, v);
        }
        private void DrawAddRemoveBlock(LogTag v, uint ukey, double py, string blockname)
        {
            Dictionary<string, string> addLabel = new Dictionary<string, string>
            {
                { "db", "DESC" },
                { "var", v.valkey },
                { "ukey", ukey.ToString() }
            };

            HmBlockReference blkRef = new HmBlockReference(blockname)
            {
                Position = new HmPoint3D((v.box.leftX + v.box.width) - 2.0, py - 2.0, 0.0),
                Layer = v.strlyr,
                Label = CmdParmParser.GetLabelfromDictionary(addLabel),
            };
            blkRef.Scale(1, blkRef.Position);
            DrawHmEntity(blkRef, v);
        }
        /// <summary>
        /// 지하수위 심볼 및 line을 그려줍니다.
        /// </summary>
        /// <param name="logTag"></param>
        /// <param name="wlevel"></param>
        public void DrawWaterLevel(LogTag logTag, double wlevel)
        {
            string xdatastr = "&지하수위";// string.Empty;

            // 기존 지하수위 삭제
            List<HmLine> elist = dlog.GetEntityByLayer<HmLine>(logTag.strlyr, xdatastr);
            List<HmText> tlist = dlog.GetEntityByLayer<HmText>(logTag.strlyr, xdatastr);
            List<HmEntity> vlist = elist.Select(x => x as HmEntity).ToList();
            vlist.AddRange(tlist.Select(x => x as HmEntity).ToList());
            dlog.Canvas.Entities.Remove(vlist);

            // 화면을 벗어나면 그리지 않는다            
            if (BegY > wlevel || FinY < wlevel)
                return;

            // 새로 지하수위 입력            
            LogTag v = dlogstyle.GetLogTag("심도");
            double z = CalcDepth2CAD(wlevel);

            // 지하수위 심볼
            HmText text = new HmText(new HmPoint3D(v.box.leftX + v.box.width, z + 0.8, 0), 2.0, "▼")
            {
                Layer = logTag.strlyr,
                Justify = AttachmentPoint.MiddleCenter,
                Label = xdatastr,
                Color = Color.Blue
            };
            DrawHmEntity(text, logTag);

            // 지하수위 라인
            int j = 0;
            for (int i = 5; i >= 0; i--)
            {
                HmLine enty = new HmLine(new HmPoint3D((v.box.leftX + 10.0) - i, z - j, 0.0), new HmPoint3D((v.box.leftX + 10.0) + i, z - j, 0.0))
                {
                    Layer = logTag.strlyr,
                    Label = xdatastr,
                    Color = Color.Blue
                };
                DrawHmEntity(enty, logTag);
                j++;
            }
        }

        /// <summary>
        /// The drawHighlight.
        /// </summary>
        /// <param name="logTag">The logTag<see cref="LogTag"/>.</param>
        /// <param name="px">The px<see cref="double"/>.</param>
        /// <param name="py">The py<see cref="double"/>.</param>
        /// <param name="hpoly">The py<see cref="double"/>.</param>
        private void drawHighlight(HmPolyline2D hpoly = null)
        {
            if (null == hpoly)
            {
                if (this.selectedEntityInfo != null)
                {
                    HmEntity selectedEntity = this.getSelectedEntity();

                    if (selectedEntity != null)
                    {
                        HmHatch hmHatch = new HmHatch();
                        hmHatch.Layer = "#하이라이트";
                        hmHatch.PatternType = HatchPatternType.PreDefined;
                        hmHatch.PatternName = "SOLID";
                        hmHatch.HatchStyle = HatchStyle.Normal;
                        hmHatch.ColorIndex = 5;
                        hmHatch.Transparency = new HmTransparency(30);

                        if (selectedEntity.Geometry is HmPolyline2D poly)
                        {
                            List<HmVertex2D> verts = new List<HmVertex2D>();
                            poly.Vertices.ForEach(x => verts.Add(new HmVertex2D(new HmPoint2D(x.X, x.Y), 0)));
                            HmPolyline2D polytmp = new HmPolyline2D();
                            polytmp.Closed = true;
                            polytmp.Vertices = verts;
                            (hmHatch.Geometry as HmGroup).Add(polytmp);
                        }
                        else
                        {
                            HmBounds2D bounds = selectedEntity.GetBounds();
                            HmPolyline2D polytmp = new HmPolyline2D();
                            polytmp.AddVertex(new HmPoint2D(bounds.xMin, bounds.yMin));
                            polytmp.AddVertex(new HmPoint2D(bounds.xMax, bounds.yMin));
                            polytmp.AddVertex(new HmPoint2D(bounds.xMax, bounds.yMax));
                            polytmp.AddVertex(new HmPoint2D(bounds.xMin, bounds.yMax));
                            polytmp.Close();
                            (hmHatch.Geometry as HmGroup).Add(polytmp);
                        }

                        dlog.canvas.Entities.Add(hmHatch);
                    }
                }
            }
            else
            {
                // 강제로 주어진 폴리라인으로 하이라이트하려고 할 때 사용하고자 한다
                HmHatch hmHatch = new HmHatch();
                hmHatch.Layer = "#하이라이트";
                hmHatch.PatternType = HatchPatternType.PreDefined;
                hmHatch.PatternName = "SOLID";
                hmHatch.HatchStyle = HatchStyle.Normal;
                hmHatch.ColorIndex = 5;
                hmHatch.Transparency = new HmTransparency(30);

                HmBounds2D bounds = hpoly.Get_Bounds2D();
                HmPolyline2D polytmp = new HmPolyline2D();
                polytmp.AddVertex(new HmPoint2D(bounds.xMin, bounds.yMin));
                polytmp.AddVertex(new HmPoint2D(bounds.xMax, bounds.yMin));
                polytmp.AddVertex(new HmPoint2D(bounds.xMax, bounds.yMax));
                polytmp.AddVertex(new HmPoint2D(bounds.xMin, bounds.yMax));
                polytmp.Close();
                (hmHatch.Geometry as HmGroup).Add(polytmp);

                dlog.canvas.Entities.Add(hmHatch);
            }
        }

        /// <summary>
        /// 멤버변수 selectedEntityInfo를 활용해 선택된 엔티티를 구함
        /// </summary>
        /// <returns></returns>
        private HmEntity getSelectedEntity()
        {
            string label = this.selectedEntityInfo.Item2;

            string db = CmdParmParser.GetValuefromKey(label, "db", '&');
            //레이블에서 각 엔티티의 ID로 활용할 수 있는 영역을 나눔
            //SPTG만 키를 공유하면서 인덱스로 구분하는 구조
            int index;
            switch (db)
            {
                case "SPTG":
                    index = 4;
                    break;
                default:
                    index = 3;
                    break;
            }

            string targetId = StringUtil.GetSubstringDividedNIndex(this.selectedEntityInfo.Item2, '&', index, true);
            return dlog.GetEntityByLayer<HmPolyline>(this.selectedEntityInfo.Item1)
        .Find(x => StringUtil.GetSubstringDividedNIndex(x.Label, '&', index, true).Equals(targetId));

        }

        /// <summary>
        /// The removeEntitiesByLayer.
        /// </summary>
        private void removeEntitiesByLayer(string layer)
        {
            foreach (var item in dlog.canvas.Entities.ToList().FindAll(x => x.Layer != null && x.Layer.Equals(layer)))
            {
                dlog.canvas.Entities.Remove(item);
            }
        }

        private void showHighlight(bool clearBefore = true, HmPolyline2D hpoly = null)
        {
            if (clearBefore)
            {
                this.removeEntitiesByLayer("#하이라이트");
            }
            drawHighlight(hpoly);
        }
#endregion

#region TCR/RQD(쌓기부, 교량부)
        private void DrawTCR_RQD(List<uint> strakeylist, int page = 0)
        {
            LogTag logTag = dlogstyle.GetLogTag("TCRRQD");
            if (logTag == null)
                return;
            AddLayer(logTag.strlyr);

            double pageStart = BegY, pageEnd = FinY;// 페이지의 시작과 끝깊이
            double finalStraStart, finalStraEnd;                  // 페이지에 그리는 지층의 시작과 끝
            double straStart = 0.0, straEnd = 0.0;      // 지층의 시작과 끝깊이

            foreach (uint straKey in strakeylist)
            {
                DBDoc doc = DBDoc.Get_CurrDoc();
                DBDataSTRA straD = null;
                // 지층정보가져오기
                if (!doc.stra.Get_Data(straKey, ref straD, true))
                    continue;
                if (straD.Depth == 0) return;

                straEnd = straD.Depth;
                if (straEnd < pageStart || straStart > pageEnd)
                    continue;

                // 해당지층의 지층설명 가져오기
                HmKeyList descKeys = new HmKeyList();
                doc.desc.Get_KeyList(descKeys, straKey);
                List<DBDataDESC> desclist = new List<DBDataDESC>();
                doc.desc.Get_DataList(descKeys, ref desclist, true);

                // 화면에 그릴때 사용할 지층의 시작깊이
                finalStraStart = pageStart > straStart ? pageStart : straStart;

                // 화면에 그릴때 사용할 지층의 종료깊이
                finalStraEnd = pageEnd < straEnd ? pageEnd : straEnd;

                // 그릴영역계산
                LogTag tmpLogTag = logTag.Clone();
                double calculatedStart = CalcDepth2CAD(finalStraStart);
                tmpLogTag.box.Set(logTag.box.leftX, calculatedStart, logTag.box.width, calculatedStart - CalcDepth2CAD(finalStraEnd));

                // 데이터가 없다면 자리에 그리기만
                if (desclist.Count.Equals(0))
                {
                    DrawTCR_RQD(tmpLogTag, 0, straKey, GaiaConstants.DASH.ToString() + GaiaConstants.SLASH + GaiaConstants.DASH);
                }
                else
                {
                    // 2. 세부지층 그리기
                    double descStart = straStart, descEnd, descThickness;
                    double dDescDepthStart = 0.0, dDescDepthEnd = 0.0;

                    foreach (DBDataDESC desc in desclist)
                    {
                        // 시작위치
                        bool hasDepth = true;
                        string descDepth = string.Empty, descDepthStart = string.Empty, descDepthEnd = string.Empty;

                        if (desc.dicDesc.TryGetValue(eDescriptionKey.Depth, out descDepth))
                        {
                            descDepthStart = descDepth.Split('~')[0];
                            descDepthStart = Regex.Replace(descDepthStart, @"\D", "");
                            descDepthEnd = descDepth.Split('~')[1];
                            descDepthEnd = Regex.Replace(descDepthEnd, @"\D", "");
                            //상기 정규식은 소수점을 포함하여 숫자가 아닌 모든 캐릭터를 제거합니다.
                            //ex>> 10 => 10, 10.1 => 101, 0.50 => 050
                            if (!double.TryParse(descDepthStart, out dDescDepthStart))
                                hasDepth = false;//continue;
                            if (!double.TryParse(descDepthEnd, out dDescDepthEnd))
                                hasDepth = false;//continue;                            
                        }
                        else
                        {
                            hasDepth = false;// continue;
                        }

                        if (hasDepth)
                        {
                            descThickness = dDescDepthEnd - dDescDepthStart;
                        }
                        else
                        {
                            dDescDepthStart = straStart;
                            descThickness = straEnd - straStart;
                        }

                        descStart = dDescDepthStart;

                        // 종료위치
                        descEnd = descStart + descThickness;

                        // 화면에 그릴때 사용할 지층의 시작깊이
                        finalStraStart = pageStart > descStart ? pageStart : descStart;

                        // 화면에 그릴때 사용할 지층의 종료깊이
                        finalStraEnd = pageEnd < descEnd ? pageEnd : descEnd;

                        // 그릴영역계산
                        tmpLogTag = logTag.Clone();
                        calculatedStart = CalcDepth2CAD(finalStraStart);
                        tmpLogTag.box.Set(logTag.box.leftX, calculatedStart, logTag.box.width, calculatedStart - CalcDepth2CAD(finalStraEnd));

                        string tcr = desc.dicDesc.ContainsKey(eDescriptionKey.TCR) ? desc.dicDesc[eDescriptionKey.TCR] : GaiaConstants.DASH.ToString();
                        string rqd = desc.dicDesc.ContainsKey(eDescriptionKey.RQD) ? desc.dicDesc[eDescriptionKey.RQD] : GaiaConstants.DASH.ToString();
                        DrawTCR_RQD(tmpLogTag, descKeys[desclist.IndexOf(desc)], straKey, string.Format("{0}/{1}", tcr, rqd));

                        // 다음 지층의 시작깊이는 이번층의 종료깊이가 됨
                        descStart = descEnd;
                    }
                }
                // 다음 지층의 시작깊이는 이번층의 종료깊이가 됨
                straStart = straEnd;
            }
        }
        private void DrawTCR_RQD(LogTag v, uint ukey, uint strakey, string msgstr = "")
        {
            // 0. XDATA 준비
            Dictionary<string, string> entLabel = new Dictionary<string, string>
            {
                { "db", "DESC" },
                { "var", v.valkey },
                { "key", ukey.ToString() },
                { "strakey", strakey.ToString() }
            };

            // 1. 박스 그리기
            HmPolyline2D poly2D = new HmPolyline2D();
            poly2D.AddVertex(new HmPoint2D(v.box.leftX, v.box.leftY));
            poly2D.AddVertex(new HmPoint2D(v.box.leftX, v.box.leftY - v.box.height));
            poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, v.box.leftY - v.box.height));
            poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, v.box.leftY));
            poly2D.Closed = true;
            // Add Entity
            var polyline = new HmPolyline(poly2D)
            {
                Label = CmdParmParser.GetLabelfromDictionary(entLabel),
                Layer = v.strlyr,
                Color = Color.DarkGreen
            };
            DrawHmEntity(polyline, v);

            // 2. 박스안에 텍스트 출력
            if (!string.IsNullOrEmpty(msgstr))
            {
                string varstr = string.Format("{0}", msgstr);
                double xx = v.box.leftX + 3.0, yy = v.box.leftY - v.box.height / 2.0;
                var hmtext = new HmText(new HmPoint3D(poly2D.Get_Bounds2D().Center.X, poly2D.Get_Bounds2D().Center.Y, 0), 2.0, varstr)
                {
                    Layer = v.strlyr,
                    Label = CmdParmParser.GetLabelfromDictionary(entLabel),
                    Color = Color.Black,
                    Justify = AttachmentPoint.MiddleCenter,
                };
                DrawHmEntity(hmtext, v);
            }
        }
#endregion

#region TCR/RQD(깎기부, 터널부), D,S,F, JOINT
        private void DrawTCR_RQD_D_S_F_Joint(List<uint> strakeylist, int page = 0)
        {
            eDescriptionKey[] edescKey = { eDescriptionKey.TCR, eDescriptionKey.RQD, eDescriptionKey.Weathered, eDescriptionKey.Stiffness, eDescriptionKey.Gap, eDescriptionKey.JointGapMax, eDescriptionKey.JointGapMin, eDescriptionKey.JointGapAvg };
            string[] category = { "TCR", "RQD", "D", "S", "F", "최대", "최소", "평균" };
            LogTag v;
            int i, isize = edescKey.Length;
            for (i = 0; i < isize; i++)
            {
                v = dlogstyle.GetLogTag(category[i]);
                if (null == v)
                    continue;
                AddLayer(v.strlyr);

                DrawTCR_RQD_D_S_F_Joint(v, edescKey[i], strakeylist, page);
            }
        }
        private void DrawTCR_RQD_D_S_F_Joint(LogTag v, eDescriptionKey edesckey, List<uint> strakeylist, int page = 0)
        {
            //LogTag v = dlogstyle.GetLogTag("절리형상");
            //if (v == null)
            //    return;
            //AddLayer(v.strlyr);

            DBDoc doc = DBDoc.Get_CurrDoc();
            DBDataSTRA straD = null;
            DBDataDESC vvvvD = null;
            List<DBDataDESC> vvvvlist = new List<DBDataDESC>();

            double pagefr = BegY, pageto = FinY;// 페이지의 시작과 끝깊이
            double zzfr = 0.0, zzto = 0.0;      // 지층의 시작과 끝깊이
            double zbeg, zfin;                  // 페이지에 그리는 지층의 시작과 끝
            double dwgz1, dwgz2;
            string msgstr = string.Empty;

            int j, jsize, i, isize = strakeylist.Count;
            for (i = 0; i < isize; i++)
            {
                // 지층정보가져오기
                if (!doc.stra.Get_Data(strakeylist[i], ref straD, true))
                    continue;
                if (straD.Depth == 0) return;

                // 리스트 청소
                vvvvlist.Clear();

                // 해당지층의 지층설명 가져오기
                HmKeyList keylist = new HmKeyList();
                doc.desc.Get_KeyList(keylist, strakeylist[i]);
                if (doc.desc.Get_DataList(keylist, ref vvvvlist, true))
                {
                    jsize = vvvvlist.Count;
                }
                else
                    jsize = 0;

                // 1. 지층전체 박스 그리기

                // 이번 층의 지층종료깊이 계산
                zzto = straD.Depth;

                // 현지층이 페이지 사이에 있지 않으면 통과
                //       
                //     ┌────────────┓ zzfr
                //     |            |     제외(X)
                //     └────────────┘ zzto
                //         
                //       ┌────────┓ 페이지시작(pagefr)
                //       |        |
                //     ┌─┼────────┼─┐  zzfr
                //     | |        | |     고려(O)
                //     └─┼────────┼─┘  zzto
                //       |        |
                //       └────────┘ 페이지종료(pageto)
                //
                //     ┌────────────┓ zzfr
                //     |            |     제외(X)
                //     └────────────┘ zzto
                //
                if (zzto < pagefr || zzfr > pageto)
                    continue;

                //
                //       ┌────────┓ 페이지시작(pagefr)
                //       |        |
                //     ┌─┼────────┼─┐  zbeg
                //     | |        | |
                //     └─┼────────┼─┘  zfin
                //       |        |
                //       └────────┘ 페이지종료(pageto)

                // 화면에 그릴때 사용할 지층의 시작깊이
                if (pagefr > zzfr)
                    zbeg = pagefr;
                else
                    zbeg = zzfr;
                // 화면에 그릴때 사용할 지층의 종료깊이
                if (zzto > pageto)
                    zfin = pageto;
                else
                    zfin = zzto;

                LogTag vv;
                if (jsize == 0)
                {
                    // 그릴영역계산
                    vv = v.Clone();
                    dwgz1 = CalcDepth2CAD(zbeg);
                    dwgz2 = CalcDepth2CAD(zfin);
                    vv.box.Set(v.box.leftX, dwgz1, v.box.width, dwgz1 - dwgz2);

                    // 데이터그리기(여기서는 빈박스만 그리고 아래에서 형상을 그린다)
                    DrawTCR_RQD_D_S_F_Joint(vv, 0, strakeylist[i], null, zbeg, zfin);
                }

                // 2. 세부지질형상 그리기
                {
                    string tmpstr = string.Empty, begstr = string.Empty, finstr = string.Empty;
                    double zfr = zzfr, zto, zthick;
                    double begzzz = 0.0, finzzz = 0.0, zzzthick;
                    bool flag;
                    // 지층종료깊이 계산
                    zfr = zzfr;
                    for (j = 0; j < jsize; j++)
                    {
                        vvvvD = vvvvlist[j];
                        if (!vvvvD.dicDesc.TryGetValue(edesckey, out msgstr))
                        {
                            msgstr = "-";
                        }                      
                        else if(edesckey.Equals(eDescriptionKey.Weathered) || edesckey.Equals(eDescriptionKey.Stiffness) 
                            || edesckey.Equals(eDescriptionKey.Gap))
                        {
                            if (msgstr.Equals(string.Empty))
                            {
                                msgstr = GaiaConstants.DASH.ToString();
                            }
                            else
                            {
                                string[] data = msgstr.Split(GaiaConstants.FIRST_DELIMITER);
                                msgstr = string.Empty;
                                for (int k = 0; k < data.Length; k++)
                                {
                                    if (k.Equals(1))
                                    {
                                        msgstr += "\n" + GaiaConstants.TILDE + "\n";
                                    }
                                    msgstr += KeyControlPair.Instance.CheckBoxes.FindAll(x => x.Item1.Equals(edesckey)).Find(x => x.Item3.Equals(data[k])).Item4;
                                }
                            }
                        }

                        // 시작위치
                        flag = true;
                        if (vvvvD.dicDesc.TryGetValue(eDescriptionKey.Depth, out tmpstr))
                        {
                            begstr = tmpstr.Split('~')[0];
                            begstr = Regex.Replace(begstr, @"\D", "");
                            finstr = tmpstr.Split('~')[1];
                            finstr = Regex.Replace(finstr, @"\D", "");
                            if (!double.TryParse(begstr, out begzzz))
                                flag = false;//continue;
                            if (!double.TryParse(finstr, out finzzz))
                                flag = false;//continue;                            
                        }
                        else
                        {
                            flag = false;// continue;
                        }

                        if (flag)
                        {
                            zzzthick = finzzz - begzzz;
                        }
                        else
                        {
                            begzzz = zzfr;
                            zzzthick = zzto - zzfr;
                        }

                        zfr = begzzz;
                        zthick = zzzthick;

                        // 종료위치
                        zto = zfr + zthick;

                        // 현지층이 페이지 사이에 있지 않으면 통과
                        if (zto < pagefr || zfr > pageto)
                            continue;

                        // 화면에 그릴때 사용할 지층의 시작깊이
                        if (pagefr > zfr)
                            zbeg = pagefr;
                        else
                            zbeg = zfr;
                        // 화면에 그릴때 사용할 지층의 종료깊이
                        if (zto > pageto)
                            zfin = pageto;
                        else
                            zfin = zto;

                        // 그릴영역계산
                        vv = v.Clone();
                        dwgz1 = CalcDepth2CAD(zbeg);
                        dwgz2 = CalcDepth2CAD(zfin);
                        vv.box.Set(v.box.leftX, dwgz1, v.box.width, dwgz1 - dwgz2);

                        // 데이터그리기
                        DrawTCR_RQD_D_S_F_Joint(vv, keylist[j], strakeylist[i], vvvvD, zbeg, zfin, msgstr);

                        // 다음 지층의 시작깊이는 이번층의 종료깊이가 됨
                        zfr = zto;
                    }
                }

                // 다음 지층의 시작깊이는 이번층의 종료깊이가 됨
                zzfr = zzto;
            }
        }
        private void DrawTCR_RQD_D_S_F_Joint(LogTag v, uint ukey, uint strakey, DBDataDESC vvvvD, double zbeg, double zfin, string msgstr = "")
        {
            // 0. XDATA 준비
            Dictionary<string, string> entLabel = new Dictionary<string, string>
            {
                { "db", "DESC" },
                { "var", v.valkey },
                { "key", ukey.ToString() },
                { "strakey", strakey.ToString() }
            };

            // 1. 박스 그리기
            HmPolyline2D poly2D = new HmPolyline2D();
            poly2D.AddVertex(new HmPoint2D(v.box.leftX, v.box.leftY));
            poly2D.AddVertex(new HmPoint2D(v.box.leftX, v.box.leftY - v.box.height));
            poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, v.box.leftY - v.box.height));
            poly2D.AddVertex(new HmPoint2D(v.box.leftX + v.box.width, v.box.leftY));
            poly2D.Closed = true;
            // Add Entity
            var polyline = new HmPolyline(poly2D)
            {
                Label = CmdParmParser.GetLabelfromDictionary(entLabel),
                Layer = v.strlyr,
                Color = Color.DarkGreen
            };
            DrawHmEntity(polyline, v);

            // 2. 박스안에 텍스트 출력
            if (null != vvvvD)
            {                
                DrawSimpleText(v.box.leftX, v.box.leftY, v.box.width, v.box.height, msgstr, v.strlyr, CmdParmParser.GetLabelfromDictionary(entLabel));
            }
        }
        private void DrawSimpleText(double lefttopx, double lefttopy, double w, double h, string value, string strlyr, string xdata)
        {
            //AddLayer(strlyr);
            //if (string.IsNullOrEmpty(value)) return;

            string[] data = value.Split('\n');
            double textHeight = 2;
            double startY = lefttopy - h / 2;
            if (data.Length.Equals(3))
            {
                startY += textHeight + 1;
            }
            foreach (string item in data)
            {
                HmText hmText = new HmText(new HmPoint3D(lefttopx + w / 2, startY, 0.0), textHeight, item)
                {
                    Label = xdata,
                    Justify = AttachmentPoint.MiddleCenter,
                    Layer = strlyr,
                    Color = Color.DarkBlue
                };

                DrawHmEntity(hmText, null);
                startY -= textHeight + 1;
            }
        }
#endregion
#endregion
    }
}
