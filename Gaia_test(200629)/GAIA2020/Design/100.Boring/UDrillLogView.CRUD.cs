using GaiaDB;
using GaiaDB.Enums;
using HmGeometry;
using LogStyle;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace GAIA2020.Design
{
    public partial class UDrillLogView
    {
        #region (지층) 추가/수정/삭제
        private void UpdateSTRA(LogTag logTag, object value)
        {
            uint ukey = 0;
            string valstr;
            valstr = CmdParmParser.GetValuefromKey(logTag.xdatastr, "db", '&');
            if (valstr == "STRA")
            {
                valstr = CmdParmParser.GetValuefromKey(logTag.xdatastr, "key", '&');
                if (!uint.TryParse(valstr, out ukey))
                    ukey = 0; // 지층추가
                valstr = CmdParmParser.GetValuefromKey(logTag.xdatastr, "var", '&');
                if (string.IsNullOrEmpty(valstr))
                    return;
            }
            else
                return;

            uint drlgkey = DBDoc.Get_CurrDoc().Get_ActiveDrillLog(true).nKey;
            double z = 0;
            double.TryParse(value.ToString(), out z);
            if (z == 0) return;
            UpdateSTRA(ukey, z, drlgkey);
        }
        /// <summary>
        /// 지층 추가 또는 수정
        /// </summary>
        /// <param name="ukey">지층키</param>
        /// <param name="z">심도</param>
        /// <param name="drlgkey">주상도키</param>
        /// <returns></returns>
        private void UpdateSTRA(uint ukey, double z, uint drlgkey)
        {
            DBDoc doc = DBDoc.Get_CurrDoc();
            DBDataSTRA d = null;
            bool update = doc.stra.Get_Data(ukey, ref d);

            // 수정작업인 경우
            if (update)
            {
                d.Depth = z;
                doc.stra.Modify_TR(ukey, d);
            }
            // 추가작업인 경우
            else
            {
                //AddStra(z, drlgkey);
                DrillLogStyle style = dlogstyle.GetLogStyle();
                switch (style)
                {
                    case DrillLogStyle.DLOG_STYLE_BB:
                    case DrillLogStyle.DLOG_STYLE_SB:
                    case DrillLogStyle.DLOG_STYLE_TB:
                    case DrillLogStyle.DLOG_STYLE_CB:
                    default:
                        {
                            // 지층 추가
                            d = new DBDataSTRA()
                            {
                                drlgKey = drlgkey,
                                Depth = z,
                            };
                            ukey = doc.stra.Add_TR(d);
                        }
                        break;
                }
            }

            //주상도 상세에서 지층 변경 시 주상도의 심도와 비교하여 더 큰 값을 주상도의 심도로 변경
            DBDataDRLG dbDataDRLG = null;
            if(doc.drlg.Get_Data(drlgkey, ref dbDataDRLG))
            {
                List<DBDataSTRA> stralist = new List<DBDataSTRA>();
                HmKeyList keylist = new HmKeyList();

                //키 리스트를 깊이로 정렬하여 가져옵니다.
                doc.stra.Get_KeyList(keylist, drlgkey);
                if (!doc.stra.Get_DataList(keylist, ref stralist, true))
                    return;

                double maxDepth = stralist.FindAll(x => x.drlgKey.Equals(drlgkey)).Max(x => x.Depth);
                if (dbDataDRLG.Depth < maxDepth)
                {
                    dbDataDRLG.Depth = maxDepth;
                    doc.drlg.Modify_TR(drlgkey, dbDataDRLG);
                }
            }
            
        }
        /// <summary>
        /// 지층 삭제
        /// </summary>
        /// <param name="ukey"></param>
        /// <returns></returns>
        private bool DeleteSTRA(uint ukey)
        {
            DBDoc doc = DBDoc.Get_CurrDoc();
            //
            doc.stra.Del_TR(ukey);
            return true;
        }

        private bool AddStra(double z, uint drlgkey)
        {
            DBDoc doc = DBDoc.Get_CurrDoc();
            //
            //
            DrillLogStyle style = dlogstyle.GetLogStyle();
            switch (style)
            {
                case DrillLogStyle.DLOG_STYLE_BB:
                case DrillLogStyle.DLOG_STYLE_SB:
                case DrillLogStyle.DLOG_STYLE_TB:
                case DrillLogStyle.DLOG_STYLE_CB:
                default:
                    {
                        DBDataSTRA d = new DBDataSTRA()
                        {
                            drlgKey = 1,
                            Depth = z,
                        };
                        doc.stra.Add_TR(d);
                    }
                    break;
            }
            //
            return true;
        }
        #endregion

        #region 지층설명
        private void UpdateDESC(LogTag logTag, object value)
        {
            uint ukey = 0;
            string valstr;
            valstr = CmdParmParser.GetValuefromKey(logTag.xdatastr, "db", '&');
            if (valstr == "DESC")
            {
                valstr = CmdParmParser.GetValuefromKey(logTag.xdatastr, "key", '&');
                if (!uint.TryParse(valstr, out ukey))
                    ukey = 0; // 지층추가
            }
            else
                return;

            UpdateDESC(ukey, value.ToString(), DBDoc.Get_CurrDoc().Get_ActiveStratum().nKey);
        }

        private void UpdateDESC(uint ukey, string data, uint straKey)
        {
            DBDoc doc = DBDoc.Get_CurrDoc();
            DBDataDESC d = null;

            // 수정작업인 경우
            if (doc.desc.Get_Data(ukey, ref d))
            {
                d.dicDesc[eDescriptionKey.Note] = data;
                doc.desc.Modify_TR(ukey, d);
            }
            // 추가작업인 경우
            else
            {             
                d = new DBDataDESC()
                {
                    straKey = straKey,
                };
                d.dicDesc[eDescriptionKey.Note] = data;
                doc.desc.Add_TR(d);
            }
        }

        private bool DeleteDESC(uint ukey)
        {
            return DBDoc.Get_CurrDoc().desc.Del_TR(ukey);
        }

        private bool AddDESC(double z, uint strakey)
        {
            DBDataDESC d = new DBDataDESC()
            {
                straKey = strakey,
            };
            DBDoc.Get_CurrDoc().desc.Add_TR(d);
            return true;
        }

        #endregion

        #region 프로젝트정보
        private void UpdatePROJ(LogTag logTag, object value)
        {
            string valstr;
            valstr = CmdParmParser.GetValuefromKey(logTag.xdatastr, "db", '&');
            if (valstr == "PROJ")
            {
                valstr = CmdParmParser.GetValuefromKey(logTag.xdatastr, "var", '&');
                if (string.IsNullOrEmpty(valstr))
                    return;
            }
            else
                return;

            DBDoc doc = DBDoc.Get_CurrDoc();
            DBDataPROJ d = null;
            if (!doc.proj.Get_Data(ref d))
                return;

            bool isUpdated = false;
            string data = value.ToString();
            // 수정
            switch (valstr)
            {
                case "사업명":
                    if (d.ProjectName != data)
                    {
                        d.ProjectName = data;
                        isUpdated = true;
                    }
                    break;
                case "발주처":
                    if (d.CompanyName != data)
                    {
                        d.CompanyName = data;
                        isUpdated = true;
                    }
                    break;
#if false // 시추자 와 작성자는 DrillLog으로 옮겨졌습니다.
                case "시추자":
                    d.DrillManName = value.ToString();
                    break;
                case "작성자":
                    d.WriteManName = value.ToString();
                    break;
#endif
            }

            // 업데이트
            if (isUpdated)
            {
                doc.proj.Add_TR(d);
            }
        }
#endregion

        #region 주상도정보
        private void UpdateDRLG(LogTag logTag, object value)
        {
            uint ukey = 0;
            string valstr;
            valstr = CmdParmParser.GetValuefromKey(logTag.xdatastr, "db", '&');
            if (valstr == "DRLG")
            {
                valstr = CmdParmParser.GetValuefromKey(logTag.xdatastr, "key", '&');
                if (!uint.TryParse(valstr, out ukey))
                    return;
                valstr = CmdParmParser.GetValuefromKey(logTag.xdatastr, "var", '&');
                if (string.IsNullOrEmpty(valstr))
                    return;
            }
            else
                return;

            //
            DBDoc doc = DBDoc.Get_CurrDoc();
            DBDataDRLG d = null;
            if (!doc.drlg.Get_Data(ukey, ref d))
                return;

            string data = value.ToString();

            // 문자의 정수 혹은 실수만 추출
            Regex r = new Regex(@"-?[0-9]*\.*[0-9]+");
            Match m = r.Match(data);
            bool isUpdated = false;
            // 수정
            switch (valstr)
            {
                case "교량명":
                case "터널명":
                    {
                        if (d.BridgeOrTunnelName != data)
                        {
                            d.BridgeOrTunnelName = data;
                            isUpdated = true;
                        }
                        break;
                    }
                case "교대교각위치":
                case "터널위치":
                    {
                        if (d.BridgeOrTunnelLocation != data)
                        {
                            d.BridgeOrTunnelLocation = data;
                            isUpdated = true;
                        }
                        break;
                    }
                case "시추공번":
                    {
                        if (d.DrillPipeNum != data)
                        {
                            d.DrillPipeNum = data;
                            isUpdated = true;
                        }
                        break;
                    }
                case "시추장비":
                    {
                        if (d.DrillDevice != data)
                        {
                            d.DrillDevice = data;
                            isUpdated = true;
                        }
                        break;
                    }
                case "시추위치":
                    {
                        if (d.DrillLocation != data)
                        {
                            d.DrillLocation = data;
                            isUpdated = true;
                        }
                        break;
                    }
                case "시추표고":
                    {
                        if (double.TryParse(m.Value, out double elevation))
                        {
                            double tmpElevation = Math.Round(elevation, 2);
                            if (d.Elevation != tmpElevation)
                            {
                                d.Elevation = tmpElevation;
                                isUpdated = true;
                            }
                        }
                        break;
                    }
                case "시추자":
                    {
                        if (d.DrillManName != data)
                        {
                            d.DrillManName = data;
                            isUpdated = true;
                        }
                        break;
                    }
                case "해머효율":
                    {
                        if (double.TryParse(m.Value, out double hammer))
                        {
                            if (d.HammerEfficiencyPercent != hammer)
                            {
                                d.HammerEfficiencyPercent = hammer;
                                isUpdated = true;
                            }
                        }
                        break;
                    }
                case "시추심도":
                    //상민 : (200623 작성)
                    //200605 김규범님이 작업한 내용입니다.
                    //UX팀에서 헤더부분 시추심도가 입력되면 최대값임을 전제로 하여 지층을 자동 추가하였는데
                    //강수석님의 의견으로는 시추심도를 입력하는 것보다 상세에서 지층에 기반해 도출하는것이 데이터 무결성을 보장할 수 있다고 판단됩니다.
                    //추후 UX팀과 논의 후 변경할 예정입니다.
                    //if (double.TryParse(m.Value, out double depth))
                    //{
                    //    depth = Math.Round(depth, 2);
                    //    d.Depth = depth > 0 ? depth : -depth;

                    //    // STRA add
                    //    UpdateSTRA(0, depth, ukey);
                    //}
                    break;
                case "작성자":
                    {
                        if (d.WriteManName != data)
                        {
                            d.WriteManName = data;
                            isUpdated = true;
                        }
                        break;
                    }
                case "시추방법":
                    {
                        if (d.DrillingMethod != data)
                        {
                            d.DrillingMethod = data;
                            isUpdated = true;
                        }
                        break;
                    }
                case "시추공경":
                    {
                        if (d.DrillPipe != data)
                        {
                            d.DrillPipe = data;
                            isUpdated = true;
                        }
                        break;
                    }
                case "케이싱심도":
                    {
                        if (!double.TryParse(m.Value, out double casing)) return;
                        casing = Math.Round(casing, 2);
                        casing = casing > 0 ? casing : -casing;
                        if (d.CasingDepth != casing)
                        {
                            d.CasingDepth = casing;
                            isUpdated = true;
                        }
                        break;
                    }
                case "조사일":
                    {
                        if (d.SurveyDay != data)
                        {
                            d.SurveyDay = data;
                            isUpdated = true;
                        }
                        break;
                    }
                case "시추각도":
                    {
                        if (d.DrillingAngleType != data)
                        {
                            d.DrillingAngleType = data;
                            isUpdated = true;
                        }
                        break;
                    }
                case "좌표":
                    {
                        if (!data.Contains('/')) { System.Windows.MessageBox.Show("좌표를 \" / \" 으로 구분하여 주세요."); return; }
                        string strx = data.Split('/')[0];
                        string stry = data.Split('/')[1];
                        if (!double.TryParse(strx, out double x)) { System.Windows.MessageBox.Show("좌표를 \" / \" 으로 구분하여 주세요."); return; }
                        if (!double.TryParse(stry, out double y)) { System.Windows.MessageBox.Show("좌표를 \" / \" 으로 구분하여 주세요."); return; }
                        x = Math.Round(x, 3);
                        y = Math.Round(y, 3);
                        HmPoint2D hmPoint2D = new HmGeometry.HmPoint2D(x, y);
                        if (d.Position.X != hmPoint2D.X || d.Position.Y != hmPoint2D.Y)
                        {
                            d.Position = hmPoint2D;
                            isUpdated = true;
                        }
                        break;
                    }
                case "지하수위":
                    {
                        if (!double.TryParse(m.Value, out double waterlv)) return;
                        waterlv = Math.Round(waterlv, 2);
                        waterlv = waterlv > 0 ? waterlv : -waterlv;
                        if (d.WaterLevel != waterlv)
                        {
                            d.WaterLevel = waterlv;
                            isUpdated = true;
                        }
                        break;
                    }
                case "기타사항":
                    {
                        if (d.additionalInfo != data)
                        {
                            d.additionalInfo = data;
                            isUpdated = true;
                        }
                        break;
                    }
            }

            // 업데이트
            if (isUpdated)
            {
                doc.drlg.Modify_TR(ukey, d);
            }
        }
#endregion

        #region 표준관입시험(SPT)
        private void UpdateSPTG(LogTag logTag, object value)
        {
            if(value is string valuestr)
            {
                int hitcnt, intrusion;
                double zlevel;
                string valstr;

                valstr = CmdParmParser.GetValuefromKey(logTag.xdatastr, "db", '&');
                if (valstr != "SPTG") return;

                valstr = CmdParmParser.GetValuefromKey(logTag.xdatastr, "key", '&');
                if (!uint.TryParse(valstr, out uint ukey)) return;

                valstr = CmdParmParser.GetValuefromKey(logTag.xdatastr, "var", '&');
                if (string.IsNullOrEmpty(valstr)) return;

                valstr = CmdParmParser.GetValuefromKey(logTag.xdatastr, "index", '&');
                if (!int.TryParse(valstr, out int idx)) return;

                valstr = CmdParmParser.GetValuefromKey(logTag.xdatastr, "zlv", '&');
                if (!double.TryParse(valstr, out zlevel)) return;

                DBDoc doc = DBDoc.Get_CurrDoc();
                DBDataSPTG d = null;
                // 해당키의 데이터가 없으면 추가(처음 입력하는 경우에는 없을 수 있음)
                if (!doc.sptg.Get_Data(ukey, ref d))
                    d = new DBDataSPTG();

                if (!valuestr.Contains("/")) return;
                else
                {
                    // 타격횟수
                    if (!int.TryParse(valuestr.Split('/')[0], out hitcnt)) return;
                    // 관입량
                    if (!int.TryParse(valuestr.Split('/')[1], out intrusion)) return;
                }
#if true
                if (logTag.update && idx >= 0)
                {
                    // 기존값 수정
                    d.SptList[idx] = new Tuple<double, int, double>(zlevel, hitcnt, intrusion);
                }
                else
                {
                    // 새로값 추가
                    d.SptList.Add(new Tuple<double, int, double>(zlevel, hitcnt, intrusion));
                    d.SptList.Sort((a, b) => a.Item1.CompareTo(b.Item1)); // 깊이값 오름차순 정렬
                }
#else
                // 수정
                List<Tuple<double, int, double>> sptlist = new List<Tuple<double, int, double>>();                
                for (int i = 0; i < d.SptList.Count; i++)
                {
                    if (i == idx)
                    {
                        sptlist.Add(new Tuple<double, int, double>(d.SptList[i].Item1, hitcnt, intrusion));
                    }
                    else
                    {
                        sptlist.Add(new Tuple<double, int, double>(d.SptList[i].Item1, d.SptList[i].Item2, d.SptList[i].Item3));
                    }
                }
                d.SptList.Clear();
                d.SptList.AddRange(sptlist);
#endif
                // 업데이트
                doc.sptg.Modify_TR(ukey, d);
            }
        }
        #endregion

        #region TCR/RQG (쌓기부, 교량부)
        private void UpdateTCRRQD(LogTag logTag, object value)
        {
            string[] data = value.ToString().Split('/');
            if (data.Length.Equals(2))
            {
                string valstr;
                valstr = CmdParmParser.GetValuefromKey(logTag.xdatastr, "db", '&');
                if (valstr != "DESC") return;

                uint ukey = 0;
                valstr = CmdParmParser.GetValuefromKey(logTag.xdatastr, "key", '&');
                if (!uint.TryParse(valstr, out ukey)) return;

                valstr = CmdParmParser.GetValuefromKey(logTag.xdatastr, "var", '&');
                if (string.IsNullOrEmpty(valstr))
                    return;

                DBDoc doc = DBDoc.Get_CurrDoc();
                DBDataDESC d = null;

                if (doc.desc.Get_Data(ukey, ref d))
                {
                    d.dicDesc[eDescriptionKey.TCR] = data.FirstOrDefault();
                    d.dicDesc[eDescriptionKey.RQD] = data.LastOrDefault();
                    doc.desc.Modify_TR(ukey, d);
                }
            }
        }
        #endregion

        #region TCR/RQD
        private void UpdateTCRRQDJGAP(LogTag logTag, object value)
        {
            uint ukey = 0;
            string valstr;
            valstr = CmdParmParser.GetValuefromKey(logTag.xdatastr, "db", '&');
            if (valstr == "DESC")
            {
                valstr = CmdParmParser.GetValuefromKey(logTag.xdatastr, "key", '&');
                if (!uint.TryParse(valstr, out ukey)) return;

                valstr = CmdParmParser.GetValuefromKey(logTag.xdatastr, "var", '&');
                if (string.IsNullOrEmpty(valstr))
                    return;
            }
            else
                return;

            DBDoc doc = DBDoc.Get_CurrDoc();
            DBDataDESC d = null;
            if (!doc.desc.Get_Data(ukey, ref d))
                return;

            // 수정
            switch (valstr)
            {
                case "TCR":
                    d.dicDesc[GaiaDB.Enums.eDescriptionKey.TCR] = value.ToString();
                    break;
                case "RQD":
                    d.dicDesc[GaiaDB.Enums.eDescriptionKey.RQD] = value.ToString();
                    break;
                case "D":
                    d.dicDesc[GaiaDB.Enums.eDescriptionKey.Density] = value.ToString();
                    break;
                case "S":
                    d.dicDesc[GaiaDB.Enums.eDescriptionKey.Weathered] = value.ToString();
                    break;
                case "F":
                    d.dicDesc[GaiaDB.Enums.eDescriptionKey.Stiffness] = value.ToString();
                    break;
                case "최대":
                    d.dicDesc[GaiaDB.Enums.eDescriptionKey.JointGapMax] = value.ToString();
                    break;
                case "최소":
                    d.dicDesc[GaiaDB.Enums.eDescriptionKey.JointGapMin] = value.ToString();
                    break;
                case "평균":
                    d.dicDesc[GaiaDB.Enums.eDescriptionKey.JointGapAvg] = value.ToString();
                    break;
            }
            // 업데이트
            doc.desc.Modify_TR(ukey, d);
        }
        #endregion
    }
}
