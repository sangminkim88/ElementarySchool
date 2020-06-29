namespace GAIA2020.Design
{
    using GaiaDB;
    using GaiaDB.Enums;
    using HmDraw;
    using HmDraw.Entities;
    using HMFrameWork.Ancestor;
    using HmGeometry;
    using LogStyle;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Teigha.Geometry;
    using StringUtil = Utilities.StringUtil;
    using NotifyHelper = Utilities.NotifyHelper;

    public partial class UDrillLogView :AUserControl, IDrillLog
    {
        //선택된 엔티티의 레이어명, 레이블
        private Tuple<string, string> selectedEntityInfo;

        #region Constructors

        public UDrillLogView()
        {
            InitializeComponent();
            myInit();
            HmDataDocument.TransactionCtrl.Add_DBUpdateWndCtrl(this); // IDBUpdate를 상속받은 경우 필히 연결시켜줍니다.
        }

        #endregion

        #region 로그스타일에 맞춰 스타일 및 파일생성
        public eDepartment CurrLogStyle
        {
            get
            {
                return DBDoc.Get_CurrDoc().Get_ActiveStyle();
            }
        }
        #endregion

        #region Methods

        public void HideControl()
        {
            (this.Resources["dimmer"] as UTranslucency).HideControl();
        }

        /// <summary>
        /// 입력용 에디트박스의 OK버튼을 클릭하면 호출된다
        /// </summary>
        /// <param name="logTag"></param>
        /// <param name="value"></param>
#if true
        public void iEditorEnter(LogTag logTag, object value, bool isTap)
        {
            switch (logTag.category)
            {
                case DrillLogCategory.DLOG_CATEGORY_PROJ:
                    UpdatePROJ(logTag, value);
                    if (isTap)
                    {
                        List<DrillLogCategory> categories = new List<DrillLogCategory>();
                        categories.Add(logTag.category);
                        categories.Add(DrillLogCategory.DLOG_CATEGORY_DRLG);
                        LogTag nextLogTag = dlogstyle.GetNextLogTag(categories, logTag);
                        double x = nextLogTag.box.leftX;
                        double y = nextLogTag.box.leftY;
                        Point3d p3d = Aux.WorldToEye(this.dlog.canvas.GetViewPanel().Graphics.HelperDevice, x, y);
                        this.ShowEditor(nextLogTag, p3d.X, p3d.Y, x, y, p3d.Z);
                    }
                    break;
                case DrillLogCategory.DLOG_CATEGORY_DRLG:
                    UpdateDRLG(logTag, value);
                    if (isTap)
                    {
                        List<DrillLogCategory> categories = new List<DrillLogCategory>();
                        categories.Add(logTag.category);
                        categories.Add(DrillLogCategory.DLOG_CATEGORY_PROJ);
                        LogTag nextLogTag = dlogstyle.GetNextLogTag(categories, logTag);
                        double x = nextLogTag.box.leftX;
                        double y = nextLogTag.box.leftY;
                        Point3d p3d = Aux.WorldToEye(this.dlog.canvas.GetViewPanel().Graphics.HelperDevice, x, y);
                        this.ShowEditor(nextLogTag, p3d.X, p3d.Y, x, y, p3d.Z);
                    }
                    break;
                case DrillLogCategory.DLOG_CATEGORY_SPTG:
                    UpdateSPTG(logTag, value);
                    break;
                case DrillLogCategory.DLOG_CATEGORY_PART_B:
                    UpdateDESC(logTag, value);
                    break;
                case DrillLogCategory.DLOG_CATEGORY_PART_Z:                    
                    UpdateSTRA(logTag, value);
                    break;
                case DrillLogCategory.DLOG_CATEGORY_TCR:
                case DrillLogCategory.DLOG_CATEGORY_RQD:
                case DrillLogCategory.DLOG_CATEGORY_JMAX:
                case DrillLogCategory.DLOG_CATEGORY_JMIN:
                case DrillLogCategory.DLOG_CATEGORY_JAVG:
                case DrillLogCategory.DLOG_CATEGORY_DSF:
                    UpdateTCRRQDJGAP(logTag, value);
                    break;
                case DrillLogCategory.DLOG_CATEGORY_TCRRQD:
                    UpdateTCRRQD(logTag, value);
                    break;
            }
        }
#else
        public void iEditorEnter(LogTag logTag, object value)
        {
            LogTag NvalueLogTag = dlogstyle.GetLogTag("NVALUE");

            if (logTag != null && value != null && AllLogTag != null && NvalueLogTag != null)
            {
                if (value is string valstr)
                {
                    //심도 추가 시
                    if (logTag.valkey.Equals("심도표고두께"))
                    {
                        var stra = DBDoc.Get_CurrDoc().stra;
                        DBDataSTRA newStra = stra.New_Data() as DBDataSTRA;
                        newStra.Depth = double.Parse(valstr);
                        newStra.drlgKey = this.curentDrillProperty.Ukey;
                        uint key = stra.New_Key();
                        stra.Add_Data(key, newStra);

                        var desc = DBDoc.Get_CurrDoc().desc;
                        DBDataDESC newDesc = desc.New_Data() as DBDataDESC;
                        newDesc.straKey = key;
                        desc.Add_Data(desc.New_Key(), newDesc);
                    }
                    this.Draw(this.curentDrillProperty);

                    if (0 == string.Compare(logTag.valkey, "지하수위"))
                    {
                        valstr = Regex.Replace(valstr, @"\D", ""); // 문자중 숫자만 추출
                        double.TryParse(valstr, out double wlevel);
                        DrawWaterLevel(logTag, wlevel);
                    }
                }
            }
        }
#endif
        public void iEditorHide()
        {
            HideControl();
        }

        public void iEditorShow(LogTag logTag, string value, double lx, double ly, double w, double h)
        {
            ShowControl(logTag, value, lx, ly, w, h);
        }

        public void iMouseDown(double mx, double my, double px, double py, double pz)
        {
            HideControl();
        }

        public void iMouseUp(double mx, double my, double px, double py, double pz)
        {
            // 주상도가 선택이 되어있지 않으면 통과
            if (dlogstyle == null || this.curentDrillProperty == null)
                return;
            Tuple<string, LogTag> v = dlogstyle.GetBox(px, py);
            if (v == null)
                return;
            
            //상민 : (200623 작성)
            //강수석님의 의견으로는 시추심도를 입력하는 것보다 상세에서 지층에 기반해 도출하는것이 데이터 무결성을 보장할 수 있다고 판단됩니다.
            //추후 UX팀과 논의 후 변경할 예정입니다.
            if (v.Item2.strlyr.Equals("#시추심도"))
            {
                return;
            }
            ShowEditor(v.Item2, mx, my, px, py, pz);
        }

        public void iSelectEntity(string stylyr, HmEntity enty)
        {
        }

        public void myInit()
        {
            dlog.SetInterface(this as IDrillLog);
            (this.Resources["dimmer"] as UTranslucency).SetOkAction(this.iEditorEnter);
        }

        public void SetIntercace(ILoadComplete i)
        {
            dlog.SetInterface(i);
        }

        public void ShowControl(LogTag logTag, string value, double leftx, double lefty, double w, double h)
        {
            (this.Resources["dimmer"] as UTranslucency).ShowControl(logTag, value, leftx, lefty, w, h);
        }

        public void ShowControl(LogTag v, string value)
        {
            LogBox box = v.box;

            HmPoint3D p3lt = dlog.World2Screen(box.leftX, box.leftY - box.height);
            HmPoint3D p3rb = dlog.World2Screen(box.leftX + box.width, box.leftY + box.height);
            ShowControl(v, value, p3lt.X, p3lt.Y, Math.Abs(p3rb.X - p3lt.X), Math.Abs(p3rb.Y - p3lt.Y));
        }

        public void ShowEditor(LogTag logTag, double mx, double my, double px, double py, double pz)
        {
            DBDoc doc = DBDoc.Get_CurrDoc();
            uint drlgKey = doc.Get_ActiveDrillLog().nKey;
            // 주상도가 없는 상태에서 작업하는 경우는 리턴
            if (0 == drlgKey)
            {
                NotifyHelper.Instance.Show(NotifyHelper.NotiType.Error, "주상도를 먼저 생성하세요");
                return;
            }
            //
            switch (logTag.category)
            {
                case DrillLogCategory.DLOG_CATEGORY_PROJ:
                    ShowEditorforPROJ(logTag, mx, my, px, py, pz);
                    break;
                case DrillLogCategory.DLOG_CATEGORY_DRLG:
                    ShowEditorforDRLG(logTag, mx, my, px, py, pz);
                    break;
                case DrillLogCategory.DLOG_CATEGORY_SPTG:
                    ShowEditorforSPTG(logTag, mx, my, px, ref py, pz);
                    break;
                case DrillLogCategory.DLOG_CATEGORY_TCRRQD:
                    ShowEditorforTCRRQD(logTag, mx, my, px, py, pz);
                    break;
                case DrillLogCategory.DLOG_CATEGORY_PART_A:
                    ShowEditorforPartA(logTag, mx, my, px, py, pz);
                    break;
                case DrillLogCategory.DLOG_CATEGORY_SAMP:
                    ShowEditorforSAMP(logTag, mx, my, px, ref py, pz);
                    break;
                case DrillLogCategory.DLOG_CATEGORY_PART_B:
                    ShowEditorforPartB(logTag, mx, my, px, py, pz);
                    break;
                case DrillLogCategory.DLOG_CATEGORY_PART_C:
                    ShowEditorforPartC(logTag, mx, my, px, py, pz);
                    break;
                case DrillLogCategory.DLOG_CATEGORY_PART_D:
                    ShowEditorforPartD(logTag, mx, my, px, py, pz);
                    break;
                case DrillLogCategory.DLOG_CATEGORY_PART_Y:
                    ShowEditorforPartY(logTag, mx, my, px, py, pz);
                    break;
                case DrillLogCategory.DLOG_CATEGORY_PART_Z:
                    ShowEditorforPartZ(logTag, mx, my, px, py, pz);
                    break;
                case DrillLogCategory.DLOG_CATEGORY_JSHP:
                    ShowEditorforJSHP(logTag, mx, my, px, py, pz);
                    break;
#if true
                case DrillLogCategory.DLOG_CATEGORY_TCR:
                case DrillLogCategory.DLOG_CATEGORY_RQD:                
                case DrillLogCategory.DLOG_CATEGORY_JMAX:
                case DrillLogCategory.DLOG_CATEGORY_JMIN:
                case DrillLogCategory.DLOG_CATEGORY_JAVG:
                    ShowEditorforTCR_RQD_DSF_Joint(logTag, mx, my, px, py, pz);
                    break;
                case DrillLogCategory.DLOG_CATEGORY_DSF:
                    ShowEditorforDSF(logTag, mx, my, px, py, pz);
                    break;
#else
                case DrillLogCategory.DLOG_CATEGORY_TCR:
                    ShowEditorforTCR(logTag, mx, my, px, py, pz);
                    break;
                case DrillLogCategory.DLOG_CATEGORY_RQD:
                    ShowEditorforRQD(logTag, mx, my, px, py, pz);
                    break;
                case DrillLogCategory.DLOG_CATEGORY_JMAX:
                    ShowEditorforJmax(logTag, mx, my, px, py, pz);
                    break;
                case DrillLogCategory.DLOG_CATEGORY_JMIN:
                    ShowEditorforJmin(logTag, mx, my, px, py, pz);
                    break;
                case DrillLogCategory.DLOG_CATEGORY_JAVG:
                    ShowEditorforJavg(logTag, mx, my, px, py, pz);
                    break;
#endif
            }
            List<HmPolyline> polylines = dlog.GetEntityByLayer<HmPolyline>(logTag.strlyr);
            if (polylines.Count > 0)
            {
                HmPolyline target = polylines.Find(x => IsPointInsideEntity(x, px, py).Equals(true));
                if (target != null) {
                    this.selectedEntityInfo = new Tuple<string, string>(logTag.strlyr, target.Label);

                    showHighlight(true);
                }
            }
        }

        /// <summary>
        /// 주어진 위치가 엔티티 내부에 위치하는지 확인한다
        /// </summary>
        /// <param name="enty">대상엔티티</param>
        /// <param name="px">X</param>
        /// <param name="py">Y</param>
        /// <returns></returns>
        public bool IsPointInsideEntity(HmEntity enty, double px, double py)
        {
            if (enty.GetType() == typeof(HmBlockReference))
            {
                HmBlockReference brenty = enty as HmBlockReference;
                HmBounds3D b = brenty.Bounds;
                int v = b.Include(px, py, 0.0);
                return (v >= 1);
            }
            else
            {
                HmPoint2D p2d = new HmPoint2D(px, py);
                int v = enty.Geometry.IsInsideEntity(p2d);
                return (v >= 0);
            }
        }
        public bool IsPointInsideEntity(HmEntity enty, HmPolyline line)
        {
            HmPoint2D p2d = new HmPoint2D(enty.Geometry.Get_Bounds2D().UpperLeft.X, enty.Geometry.Get_Bounds2D().UpperLeft.Y);
            int v = line.Geometry.IsInsideEntity(p2d);
            return (v >= 0);
        }
        /// <summary>
        /// 주어진 위치에 엔티티가 있는지 확인한다.
        /// </summary>
        /// <param name="enty"></param>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <returns></returns>
        public bool IsPointInsideEntityList(List<HmEntity> entylist, double px, double py)
        {
            if (entylist.Count == 0) return false;

            HmPoint2D p2d = new HmPoint2D(px, py);
            foreach(HmEntity enty in entylist)
            {
                int v = enty.Geometry.IsInsideEntity(p2d);
                if (v == 1 || v == 0) return false;
            }

            return true;
        }

        public void ShowEditorforPROJ(LogTag v, double mx, double my, double px, double py, double pz)
        {
            LogTag w = v.Clone();
            w.update = false;
            string valstr = v.valkey;

            List<HmText> vlist = dlog.GetEntityByLayer<HmText>(v.strlyr);
            foreach (var e in vlist)
            {
                valstr = e.TextString;
                w.xdatastr = e.Label;   // 실제XData
                w.update = true;
                break;
            }

            // 추가하는 경우
            if (!w.update)
            {
                DBDoc doc = DBDoc.Get_CurrDoc();
                HmDBKey dbkey = doc.Get_ActiveDrillLog();
                // XData 업데이트
                Dictionary<string, string> infoLabel = new Dictionary<string, string>()
                {
                    { "db", "PROJ"},
                    { "var", v.valkey },
                    { "key", (dbkey != null) ? dbkey.nKey.ToString() : "0"},
                };
                w.xdatastr = CmdParmParser.GetLabelfromDictionary(infoLabel);
            }

            ShowControl(w, valstr);
        }

        public void ShowEditorforDRLG(LogTag v, double mx, double my, double px, double py, double pz)
        {
            LogTag w = v.Clone();
            w.update = false;
            string valstr = v.valkey;
            List<HmText> vlist = dlog.GetEntityByLayer<HmText>(v.strlyr);
            foreach (var e in vlist)
            {
                valstr = e.TextString;
                w.xdatastr = e.Label;   // 실제XData
                w.update = true;
                break;
            }

            // 추가하는 경우
            if (!w.update)
            {
                DBDoc doc = DBDoc.Get_CurrDoc();
                HmDBKey dbkey = doc.Get_ActiveDrillLog();
                // XData 업데이트
                Dictionary<string, string> infoLabel = new Dictionary<string, string>()
                {
                    { "db", "DRLG"},
                    { "var", v.valkey },
                    { "key", (dbkey != null) ? dbkey.nKey.ToString() : "0"},
                };
                w.xdatastr = CmdParmParser.GetLabelfromDictionary(infoLabel);
            }

            if (v.strlyr.Equals("#해머효율"))
            {
                valstr = valstr.Replace("%", "");
            }

            ShowControl(w, valstr);
        }
        public void ShowEditorforSPTG(LogTag v, double mx, double my, double px, ref double py, double pz)
        {
#if true
            LogTag w = v.Clone();
            w.update = false; // 초기는 추가
                        
            string valstr = v.valkey;

            List<HmEntity> vlist = dlog.GetEntityByLayer<HmEntity>(v.strlyr);
            // 클릭한 위치에 엔티티가 있는지 확인
            foreach (var e in vlist)
            {
                if (!IsPointInsideEntity(e, px, py))
                    continue;
                if (string.IsNullOrEmpty(e.Label))
                    continue;

                if (e is HmText txt)
                {
                    valstr = txt.TextString;
                    w.xdatastr = txt.Label;
                    w.update = true;
                    // 기존값 수정으로 설정
                    break;
                }
            }

            // 기존값이 아니어서 새로 추가하는 경우
            if(!w.update)
            {
                int zindex = -1;

                // 클릭한 위치 인덱스
                double zper1m = w.box.height / dlogstyle.thisDepth; // 1m당 캐드높이
                double zintv = zper1m * dlogstyle.intvSpt;          // SPT 표시 간격당 캐드높이
#if true
                double ztemp = (w.box.leftY - py) / zintv + 0.5;    // 0.5 간격은 반올림
                double zlevel = (int)ztemp * dlogstyle.intvSpt;
#else
                double zlevel = (int)((w.box.leftY - py) / zintv) * dlogstyle.intvSpt;
#endif
                // zlevel에 해당하는 캐드좌표를 계산해서 업데이트한다
                py = w.box.leftY - (zlevel / dlogstyle.intvSpt) * zintv;
                // 페이지를 고려한 높이
                zlevel += BegY;

                // 심도는 항상 0 이상일 것( BegY < SPT <= FinY )
                if (zlevel <= BegY || zlevel > FinY)
                //if (zlevel <= 0.0)
                    return;

                // 현재 작업중이 주상도 디비키
                uint ukey = 0;
                DBDoc doc = DBDoc.Get_CurrDoc();
                HmDBKey akey = doc.Get_ActiveDrillLog();
                if (null != akey)
                    ukey = akey.nKey;
#if false
                // 주상도가 없는 상태에서 작업하는 경우는 리턴
                if(0 == ukey)
                    return;
#endif
                DBDataSPTG sptg = null;
                if (doc.sptg.Get_Data(ukey, ref sptg, true))
                {
                    // 중복되는 경우 기존키로 업데이트                
                    int idx = sptg.SptList.FindIndex(x => x.Item1 == zlevel);
                    if (idx >= 0)
                    {
                        zindex = idx;
                        valstr = string.Format("{0}/{1}", sptg.SptList[idx].Item2, sptg.SptList[idx].Item3);
                        w.update = true; // 기존값 수정
                    }
                    else
                    {
                        valstr = "0/30";
                    }
                }
                else
                {
                    valstr = "0/30";
                }
#if true
                // Label setting
                Dictionary<string, string> sptgLabel = new Dictionary<string, string>()
                {
                    { "db", "SPTG"},
                    { "var", v.valkey },
                    { "key", ukey.ToString() },
                    { "index", zindex.ToString() },
                    { "zlv", zlevel.ToString() }
                };
#else
               // Label setting
                Dictionary<string, string> sptgLabel = new Dictionary<string, string>()
                {
                    { "db", "SPTG"},
                    { "var", v.valkey },
                    { "key", ukey.ToString() },
                    { "index", zindex.ToString() },
                    { "zlv", zlevel.ToString() }
                };

                // 중복되는 경우 기존키로 업데이트                
                int idx = sptg.SptList.FindIndex(x => x.Item1 == zlevel);
                if (idx >= 0)
                {
                    foreach (var e in vlist)
                    {
                        if(e is HmText txt)
                        {
                            string zlvstr = CmdParmParser.GetValuefromKey(e.Label, "zlv", '&');                            
                            if (!double.TryParse(zlvstr, out double zlv)) return;

                            if (zlv == zlevel)
                            {
                                string indexstr = CmdParmParser.GetValuefromKey(e.Label, "index", '&');
                                sptgLabel["index"] = indexstr;
                                valstr = txt.TextString;
                                w.update = true;
                                break;
                            }
                        }
                    }
                }
                else
                {                 
                    valstr = "0/30";
                }
#endif
                w.xdatastr = CmdParmParser.GetLabelfromDictionary(sptgLabel);
#if true
                // 추가할 위치에 임시로 하이라이트 폴리라인 추가
                addInvisibleBox(new System.Windows.Point(w.box.leftX, py + 2.0), w.box.width, 2.0 * 2, "", v.strlyr);
#else
                HmPolyline2D hpoly = new HmPolyline2D();
                hpoly.AddVertex(v.box.leftX, py + 2.0, 0.0);
                hpoly.AddVertex(v.box.leftX, py - 2.0, 0.0);
                hpoly.AddVertex(v.box.leftX + v.box.width, py - 2.0, 0.0);
                hpoly.AddVertex(v.box.leftX + v.box.width, py + 2.0, 0.0);
                hpoly.Closed = true;
                showHighlight(true, hpoly);
#endif
            }

            // 위치조정
            w.box = new LogBox() { leftX = w.box.leftX, leftY = py, width = w.box.width, height = 2.0 };

            ShowControl(w, valstr);
#else
                LogTag w = v.Clone();
            w.update = false;
            string valstr = v.valkey;

            // 클릭한위치 인덱스
            double zper1m = w.box.height / dlogstyle.thisDepth;
            double zintv = zper1m * dlogstyle.intvSpt;
            double zlevel = (int)((w.box.leftY - py) / zintv) * dlogstyle.intvSpt;
            zlevel += BegY;
            double yindex = (w.box.leftY - py) / (w.box.height / 20);

            // Label setting
            Dictionary<string, string> sptgLabel = new Dictionary<string, string>()
            {
                { "db", "SPTG"},
                { "var", v.valkey },
                { "key", "0" },
                { "index", yindex.ToString() },
                { "zlv", zlevel.ToString() }
            };
            w.xdatastr = CmdParmParser.GetLabelfromDictionary(sptgLabel);
            w.box = new LogBox() { leftX = w.box.leftX, leftY = py, width = w.box.width, height = 2.0 };

            List<HmEntity> vlist = dlog.GetEntityByLayer<HmEntity>(v.strlyr);
            // 클릭한 위치에 엔티티가 있는지 확인
            if(!IsPointInsideEntityList(vlist, px, py))
            {
                foreach (var e in vlist)
                {
                    if (!IsPointInsideEntity(e, px, py))
                        continue;
                    if (string.IsNullOrEmpty(e.Label))
                        continue;

                    if (e is HmText txt)
                    {
                        valstr = txt.TextString;
                        w.xdatastr = txt.Label;
                        w.update = true;
                        break;
                    }
                }
            }
            else
            {
                // 클릭한 위치에 엔티티가없으면 가장 근접한 엔티티를 찾아줍니다.
                foreach (var e in vlist)
                {
                    string idxstr = CmdParmParser.GetValuefromKey(e.Label, "index", '&');
                    if (!int.TryParse(idxstr, out int idx)) continue;

                    idx += 1;
                    if(idx + 0.5 > yindex && idx - 0.5 < yindex) // +- 0.5까지
                    {
                        if (e is HmText txt)
                        {
                            valstr = txt.TextString;
                            w.xdatastr = txt.Label;
                            w.update = true;
                            break;
                        }
                    }
                }                    
            }

            ShowControl(w, valstr);
#endif
            }

        public void ShowEditorforTCRRQD(LogTag v, double mx, double my, double px, double py, double pz)
        {
            string valstr = v.valkey;
            string keystr;
            uint ukey = 0, ukeyStra = 0;

            HmText hmText = dlog.GetHmTextByLayerPoint(v.strlyr, px, py);

            if(hmText == null)
            {
                NotifyHelper.Instance.Show(NotifyHelper.NotiType.Error, "지층을 먼저 생성하세요");
                return;
            }

            valstr = CmdParmParser.GetValuefromKey(hmText.Label, "db", '&');
            if (!valstr.Equals("DESC")) return;

            keystr = CmdParmParser.GetValuefromKey(hmText.Label, "var", '&');
            if (!keystr.Equals(v.valkey)) return;

            keystr = CmdParmParser.GetValuefromKey(hmText.Label, "key", '&');
            if (!uint.TryParse(keystr, out ukey)) return;

            keystr = CmdParmParser.GetValuefromKey(hmText.Label, "strakey", '&');
            if (!uint.TryParse(keystr, out ukeyStra)) return;

            DBDoc doc = DBDoc.Get_CurrDoc();
            // 없으면 추가
            if (ukey.Equals(0))
            {
                DBDataDESC d = new DBDataDESC();
                d.straKey = ukeyStra;
                ukey = doc.desc.Add_TR(d);
            }

            if (ukey > 0)
            {
                DBDoc.Get_CurrDoc().Set_ActiveStratum(new HmDBKey("STRA", ukeyStra, (int)ukey));
            }
                       
            LogTag logTag = v.Clone();

            HmDBKey dbkey = doc.Get_ActiveDrillLog();
            Dictionary<string, string> infoLabel = new Dictionary<string, string>()
            {
                { "db", "DESC"},
                { "var", v.valkey },
                { "key", ukey.ToString()},
            };
            logTag.xdatastr = CmdParmParser.GetLabelfromDictionary(infoLabel);
            
            DBDataDESC dBDataDESC = null;
            doc.desc.Get_Data(ukey, ref dBDataDESC);
            string tcr = dBDataDESC.dicDesc.ContainsKey(eDescriptionKey.TCR) ? dBDataDESC.dicDesc[eDescriptionKey.TCR] : GaiaConstants.DASH.ToString();
            string rqd = dBDataDESC.dicDesc.ContainsKey(eDescriptionKey.RQD) ? dBDataDESC.dicDesc[eDescriptionKey.RQD] : GaiaConstants.DASH.ToString();
            valstr = string.Format("{0}/{1}", tcr, rqd);

            logTag.box = new LogBox() { leftX = logTag.box.leftX, leftY = py, width = logTag.box.width, height = 2.0 };
            ShowControl(logTag, valstr);
        }

        public void ShowEditorforSAMP(LogTag v, double mx, double my, double px, ref double py, double pz)
        {
            // 1. 입력화면 전면에 표시
            var mngr = App.GetViewModelManager();
            InputUIViewModel vm = mngr.GetValue(typeof(InputUIViewModel), false) as InputUIViewModel;
            vm.CurrentVM = mngr.GetValue(typeof(APartDescViewModel)) as APartDescViewModel;

            // SAMP 레이어에 해당하는 엔티티 검색
            string keystr, valstr = string.Empty;
            uint ukey = 0;
            double zlevel = 0.0;
            List<HmEntity> vlist = dlog.GetEntityByLayer<HmEntity>(v.strlyr);
            foreach (var e in vlist)
            {
                //현재 위치(px, py, pz)에 존재하는 엔티티 검색
                if (!IsPointInsideEntity(e, px, py) || null == e.Label)
                    continue;

                // SAMP
                valstr = CmdParmParser.GetValuefromKey(e.Label, "db", '&');
                if (!valstr.Equals("SAMP")) continue;

                // 시료형태
                keystr = CmdParmParser.GetValuefromKey(e.Label, "var", '&');
                if (!keystr.Equals(v.valkey)) continue;

                // 깊이
                keystr = CmdParmParser.GetValuefromKey(e.Label, "zlv", '&');
                if (!double.TryParse(keystr, out zlevel))
                    continue;

                // 디비키
                keystr = CmdParmParser.GetValuefromKey(e.Label, "key", '&');
                if (!uint.TryParse(keystr, out ukey))
                    continue;

                break;
            }

            if(ukey == 0)
            {
                // 디비키가 없는 경우는 추가하기 위한 깊이를 계산
                valstr = "SAMP";
                // 클릭한 위치 인덱스
                double zper1m = v.box.height / dlogstyle.thisDepth; // 1m당 캐드높이
                double zintv = zper1m * dlogstyle.intvSpt;          // SPT 표시 간격당 캐드높이
#if true
                double ztemp = (v.box.leftY - py) / zintv + 0.5;    // 0.5 간격은 반올림
                zlevel = (int)ztemp * dlogstyle.intvSpt;
#else
                zlevel = (int)((v.box.leftY - py) / zintv) * dlogstyle.intvSpt;
#endif
                // zlevel에 해당하는 캐드좌표를 계산해서 업데이트한다
                py = v.box.leftY - (zlevel / dlogstyle.intvSpt) * zintv;
                // 페이지를 고려한 높이
                zlevel += BegY;

                // 심도는 항상 0 이상일 것( BegY <= SAMP < FinY )
                if (zlevel < BegY || zlevel >= FinY)
                //if (zlevel <= 0.0)
                    return;

                // 해당깊이에 이미 샘플이 있는 경우는 수정상태로 변경
                DBDoc doc = DBDoc.Get_CurrDoc();
                uint drlgKey = doc.Get_ActiveDrillLog().nKey;
#if false
               // 주상도가 없는 상태에서 작업하는 경우는 리턴
                if (0 == drlgKey)
                    return;
#endif
                HmKeyList allKeyList = new HmKeyList();
                List<double> allDepthList = new List<double>();
                
                doc.samp.Get_KeyList(allKeyList, drlgKey, true, allDepthList);

                // 중복되는 경우 기존키로 업데이트
                int idx = allDepthList.FindIndex(x => x == zlevel);
                if (idx >= 0)
                    ukey = allKeyList.list[idx];
#if true
                // 추가할 위치에 임시로 하이라이트 폴리라인 추가
                addInvisibleBox(new System.Windows.Point(v.box.leftX, py + 1.0), v.box.width, 5.5, "", v.strlyr);
#endif
            }

            // 3. 편집할 데이터키를 사용자 메시지로 전달
            if (ukey >= 0 && zlevel > 0.0)
            {
                // 2. 입력화면에 표시할 데이터키 찾기
                List<string> vmsg = new List<string>();
                vmsg.Add(string.Format("key={0}", ukey));
                vmsg.Add(string.Format("zlv={0}", zlevel));

                // 사용자정의 메시지
                // View에는 사용자정의 DBUpdate 메시지를 보냅니다.
                DBDoc.Get_CurrDoc().Get_TranCtrl().Send_Message(HmDataDocument.TRANSACTION_STATE.USER, string.Format("{0}", valstr), vmsg);
            }
        }

        public void ShowEditorforPartA(LogTag v, double mx, double my, double px, double py, double pz)
        {
            // 1. 입력화면 전면에 표시
            var mngr = App.GetViewModelManager();
            InputUIViewModel vm = mngr.GetValue(typeof(InputUIViewModel), false) as InputUIViewModel;
            vm.CurrentVM = mngr.GetValue(typeof(APartDescViewModel)) as APartDescViewModel;

            // STRA 또는 SAMP 레이어에 해당하는 엔티티 검색
            string keystr, valstr = string.Empty;
            uint ukey = 0;
            
            // 현재 클릭한 영역의 폐합된 polyline을 가져옴
            HmPolyline currPolygon = dlog.GetEntityByLayerPoint(v.strlyr, px, py);
            if (currPolygon == null) return;

            //List<HmEntity> vlist = dlog.GetEntityByLayer<HmEntity>(v.strlyr);
            //foreach (var e in vlist)
            //{
            //    // 현재 클릭한 영역에 있는 폐합된 폴리라인의 내부에 존재하는 엔티티 검색
            //    if (!IsPointInsideEntity(e, currPolygon) || null == e.Label)
            //        continue;

            // STRA
            valstr = CmdParmParser.GetValuefromKey(currPolygon.Label, "db", '&');
            //if (!(valstr.Equals("STRA") || valstr.Equals("SAMP"))) continue;
            if (!valstr.Equals("STRA")) return;

            // 주상도, 색조, 통일분류, 시료형태(X)
            keystr = CmdParmParser.GetValuefromKey(currPolygon.Label, "var", '&');
            if (!keystr.Equals(v.valkey)) return;

            // 디비키
            keystr = CmdParmParser.GetValuefromKey(currPolygon.Label, "key", '&');
            if (!uint.TryParse(keystr, out ukey))
                return;

            //break;
            //}

            //HmText hmText = dlog.GetHmTextByLayerPoint(v.strlyr, px, py);
            //if(hmText != null)
            //{
            //    // STRA
            //    valstr = CmdParmParser.GetValuefromKey(hmText.Label, "db", '&');
            //    if (!valstr.Equals("STRA")) return;

            //    // 주상도, 색조, 통일분류, 시료형태(X)
            //    keystr = CmdParmParser.GetValuefromKey(hmText.Label, "var", '&');
            //    if (!keystr.Equals(v.valkey)) return;

            //    // 디비키
            //    keystr = CmdParmParser.GetValuefromKey(hmText.Label, "key", '&');
            //    if (!uint.TryParse(keystr, out ukey)) return;
            //}

            // 3. 편집할 데이터키를 사용자 메시지로 전달
            if (0 != ukey)
            {
                DBDoc.Get_CurrDoc().Set_ActiveStratum(new HmDBKey(valstr, ukey));
                // 2. 입력화면에 표시할 데이터키 찾기
                List<string> vmsg = new List<string>();
                vmsg.Add(string.Format("key={0}", ukey));

                // 사용자정의 메시지
                // View에는 사용자정의 DBUpdate 메시지를 보냅니다.
                DBDoc.Get_CurrDoc().Get_TranCtrl().Send_Message(HmDataDocument.TRANSACTION_STATE.USER, string.Format("{0}", valstr), vmsg);
            }
        }

        public void ShowEditorforPartB(LogTag v, double mx, double my, double px, double py, double pz)
        {
            // 1. 입력화면 전면에 표시
            var mngr = App.GetViewModelManager();
            InputUIViewModel vm = mngr.GetValue(typeof(InputUIViewModel), false) as InputUIViewModel;
            if (CurrLogStyle == eDepartment.Cut || CurrLogStyle == eDepartment.Tunnel)
            {
                vm.CurrentVM = mngr.GetValue(typeof(B2PartDescViewModel)) as B2PartDescViewModel;
            }
            else
            {
                vm.CurrentVM = mngr.GetValue(typeof(B1PartDescViewModel)) as B1PartDescViewModel;
            }

#if true
            // 2. 입력화면에 표시할 데이터키 찾기
            List<string> vmsg = new List<string>();
            // DESC 레이어에 해당하는 엔티티 검색
            string strlyr = v.strlyr;
            string keystr, valstr = string.Empty;

            //// 현재 클릭한 영역의 폐합된 polyline을 가져옴
            //HmPolyline currPolygon = dlog.GetEntityByLayerPoint(v.strlyr, px, py);
            //if (currPolygon == null) return;

            List<HmEntity> vlist = dlog.GetEntityByLayer<HmEntity>(strlyr);
            uint ukey = 0, ukeyStra = 0;
            string xdatastr = string.Empty;
            foreach (var e in vlist)
            {
                //현재 위치(px, py, pz)에 존재하는 엔티티 검색
                if (!IsPointInsideEntity(e, px, py))
                    continue;
                if (null == e.Label)
                    continue;
                valstr = CmdParmParser.GetValuefromKey(e.Label, "db", '&');
                // DESC
                if (0 != string.Compare("DESC", valstr))
                    continue;
                keystr = CmdParmParser.GetValuefromKey(e.Label, "var", '&');
                // 지층설명
                if (0 != string.Compare(v.valkey, keystr))
                    continue;
                keystr = CmdParmParser.GetValuefromKey(e.Label, "key", '&');
                if (!uint.TryParse(keystr, out ukey))
                    continue;
                // 연관지층키
                keystr = CmdParmParser.GetValuefromKey(e.Label, "strakey", '&');
                if (!uint.TryParse(keystr, out ukeyStra))
                    continue;
                xdatastr = e.Label;
                break;
            }
            if (ukeyStra == 0)
            {
                Utilities.NotifyHelper.Instance.Show(Utilities.NotifyHelper.NotiType.Error, "지층을 먼저 생성하세요");
                return;
            }

            // 2-1. 없으면 편집할수있도록 추가 후 진행
            if (ukey == 0)
            {
#if false
                // stra에서 심도 데이터를 받아오고 desc에 추가
                // 토사일때 stra 심도 와 desc 심도가 같다.
                // 암일때 stra가 여러 desc를 가질 수 있으며 desc 심도를 사용자가 세팅한다.
                // Ex) 암반일때 사용자의 정의한 desc 심도가 3이고, stra 암반 심도 25~35m 이면 desc는 25~28, 28~31, 31~34, 34~35 총 4개 생성.
                //DBDoc doc = DBDoc.Get_CurrDoc();
                //DBDataSTRA straD = null;
                //if (doc.stra.Get_Data(ukeyStra, ref straD))
                //{
                //    switch (straD.soilType)
                //    {
                //        case eSoil.None:
                //        case eSoil.gw:
                //        case eSoil.gp:
                //        case eSoil.gm:
                //        case eSoil.gwgm:
                //        case eSoil.gpgm:
                //        case eSoil.gc:
                //        case eSoil.gwgc:
                //        case eSoil.gpgc:
                //        case eSoil.gcgm:
                //        case eSoil.sw:
                //        case eSoil.sp:
                //        case eSoil.sm:
                //        case eSoil.swsm:
                //        case eSoil.spsm:
                //        case eSoil.sc:
                //        case eSoil.swsc:
                //        case eSoil.spsc:
                //        case eSoil.scsm:
                //        case eSoil.ml:
                //        case eSoil.mh:
                //        case eSoil.cl:
                //        case eSoil.clml:
                //        case eSoil.ch:
                //        case eSoil.ol:
                //        case eSoil.oh:
                //        case eSoil.pt:
                //        case eSoil.boulder:
                //        case eSoil.풍화암:
                //            {
                //                double dTop = 0.0, dBot = 0.0;
                //                if (doc.stra.Get_Depth(ukeyStra, ref dTop, ref dBot))
                //                {
                //                    string strDepth = string.Format("{0}~{1}", dTop, dBot);

                //                    // 기본 추가 추가
                //                    DBDataDESC descD = new DBDataDESC();
                //                    descD.straKey = ukeyStra;

                //                    if (descD.dicDesc.ContainsKey(eDescriptionKey.Depth))
                //                    { descD.dicDesc[eDescriptionKey.Depth] = strDepth; }
                //                    else
                //                    { descD.dicDesc.Add(eDescriptionKey.Depth, strDepth); }

                //                    ukey = doc.desc.Add_TR(descD);
                //                }
                //            }
                //            break;
                //        case eSoil.연암:
                //        case eSoil.경암:
                //            {
                //                double dTop = 0.0, dBot = 0.0;
                //                if (doc.stra.Get_Depth(ukeyStra, ref dTop, ref dBot))
                //                {
                //                    string strDepth = string.Format("{0}~{1}", dTop, dBot);

                //                    // 기본 추가 추가
                //                    DBDataDESC descD = new DBDataDESC();
                //                    descD.straKey = ukeyStra;

                //                    if (descD.dicDesc.ContainsKey(eDescriptionKey.Depth))
                //                    { descD.dicDesc[eDescriptionKey.Depth] = strDepth; }
                //                    else
                //                    { descD.dicDesc.Add(eDescriptionKey.Depth, strDepth); }

                //                    ukey = doc.desc.Add_TR(descD);
                //                }
                //            }
                //            break;
                //        default:
                //            break;
                //    }
                //}
#endif
                // 기본 추가 추가
                DBDataDESC d = new DBDataDESC();
                d.straKey = ukeyStra;
                DBDoc doc = DBDoc.Get_CurrDoc();
                ukey = doc.desc.Add_TR(d);
            }

            // 3. 편집할 데이터키를 사용자 메시지로 전달
            if (ukey > 0)
            {
                DBDoc.Get_CurrDoc().Set_ActiveStratum(new HmDBKey("STRA", ukeyStra, (int)ukey));
                // 디키키 = 0 이면 추가하고, 디비키 > 0 이면 업데이트할 것
                string msgstr = string.Empty;
                if (CurrLogStyle == eDepartment.Cut || CurrLogStyle == eDepartment.Tunnel)
                    msgstr = string.Format("B2PartDesc", valstr);
                else
                    msgstr = string.Format("B1PartDesc", valstr);
                vmsg.Add(string.Format("key={0}", ukey));
                vmsg.Add(string.Format("strakey={0}", ukeyStra));
                // 사용자정의 메시지
                DBDoc.Get_CurrDoc().Get_TranCtrl().Send_Message(HmDataDocument.TRANSACTION_STATE.USER, msgstr, vmsg); // View에는 사용자정의 DBUpdate 메시지를 보냅니다.

                HmText hmText = dlog.GetEntityByLayer<HmText>("#NOTE").Find(x => IsPointInsideEntity(x, px, py).Equals(true));
                if (hmText != null)
                {
                    DBDoc doc = DBDoc.Get_CurrDoc();
                    DBDataDESC dataDESC = null;
                    doc.desc.Get_Data(ukey, ref dataDESC);
                    string data = dataDESC.dicDesc.ContainsKey(eDescriptionKey.Note) ? dataDESC.dicDesc[eDescriptionKey.Note] : string.Empty;
                    LogTag logTag = v.Clone();
                    logTag.type = typeof(decimal);
                    logTag.xdatastr = xdatastr;
                    logTag.box.leftX = hmText.Position.X;
                    logTag.box.leftY = hmText.Position.Y;
                    logTag.box.height = hmText.Height;
                    ShowControl(logTag, data);
                }
            }
#endif
        }

        public void ShowEditorforJSHP(LogTag v, double mx, double my, double px, double py, double pz)
        {
            // 1. 입력화면 전면에 표시
            var mngr = App.GetViewModelManager();
            InputUIViewModel vm = mngr.GetValue(typeof(InputUIViewModel), false) as InputUIViewModel;
            vm.CurrentVM = mngr.GetValue(typeof(B2PartDescViewModel)) as B2PartDescViewModel;

#if true
            // 2. 입력화면에 표시할 데이터키 찾기
            List<string> vmsg = new List<string>();
            // DESC 레이어에 해당하는 엔티티 검색
            string strlyr = v.strlyr;
            string keystr, valstr = string.Empty;
            List<HmEntity> vlist = dlog.GetEntityByLayer<HmEntity>(strlyr);
            uint ukey = 0, ukeyStra = 0;
            foreach (var e in vlist)
            {
                //현재 위치(px, py, pz)에 존재하는 엔티티 검색
                if (!IsPointInsideEntity(e, px, py))
                    continue;
                if (null == e.Label)
                    continue;
                //valstr = e.TextString;
                valstr = CmdParmParser.GetValuefromKey(e.Label, "db", '&');
                // DESC 또는 JSHP
                if (0 != string.Compare("JSHP", valstr))
                    continue;
                keystr = CmdParmParser.GetValuefromKey(e.Label, "var", '&');
                // 지층설명
                if (0 != string.Compare(v.valkey, keystr))
                    continue;
                keystr = CmdParmParser.GetValuefromKey(e.Label, "key", '&');
                if (!uint.TryParse(keystr, out ukey))
                    continue;
                keystr = CmdParmParser.GetValuefromKey(e.Label, "strakey", '&');
                if (!uint.TryParse(keystr, out ukeyStra))
                    continue;

                break;
            }
            if (ukeyStra == 0)
            {
                Utilities.NotifyHelper.Instance.Show(Utilities.NotifyHelper.NotiType.Error, "지층을 먼저 생성하세요");
                return;
            }

            // 2-1. 없으면 편집할 수 있도록 추가 후 진행
            if (ukey == 0)
            {
                // 기본 추가 추가
                DBDataDESC d = new DBDataDESC();
                d.straKey = ukeyStra;
                DBDoc doc = DBDoc.Get_CurrDoc();
                ukey = doc.desc.Add_TR(d);
            }

            // 3. 편집할 데이터키를 사용자 메시지로 전달
            if (ukey > 0)
            {
                DBDoc.Get_CurrDoc().Set_ActiveStratum(new HmDBKey("STRA", ukeyStra, (int)ukey));
                // 디키키 = 0 이면 추가하고, 디비키 > 0 이면 업데이트할 것
                string msgstr = string.Empty;
                msgstr = string.Format("B2PartDesc", valstr);
                vmsg.Add(string.Format("key={0}", ukey));
                vmsg.Add(string.Format("strakey={0}", ukeyStra));
                // 사용자정의 메시지
                DBDoc.Get_CurrDoc().Get_TranCtrl().Send_Message(HmDataDocument.TRANSACTION_STATE.USER, msgstr, vmsg); // View에는 사용자정의 DBUpdate 메시지를 보냅니다.
            }
#endif
        }

        public void ShowEditorforTCR_RQD_DSF_Joint(LogTag v, double mx, double my, double px, double py, double pz)
        {
            LogTag w = v.Clone();
            w.update = false;
            string valstr = v.valkey;
            string keystr = string.Empty;

            // 현재 클릭한 영역의 폐합된 polyline을 가져옴
            HmPolyline currPolygon = dlog.GetEntityByLayerPoint(v.strlyr, px, py);
            if (currPolygon == null) return;

            valstr = CmdParmParser.GetValuefromKey(currPolygon.Label, "db", '&');
            // DESC
            if (0 != string.Compare("DESC", valstr))
                return;
            keystr = CmdParmParser.GetValuefromKey(currPolygon.Label, "var", '&');
            // 지층설명
            if (0 != string.Compare(v.valkey, keystr))
                return;
            keystr = CmdParmParser.GetValuefromKey(currPolygon.Label, "key", '&');
            if (!uint.TryParse(keystr, out uint ukey))
                return;
            // 연관지층키
            keystr = CmdParmParser.GetValuefromKey(currPolygon.Label, "strakey", '&');
            if (!uint.TryParse(keystr, out uint ukeyStra))
                return;

            List<HmText> txtlist = dlog.GetEntityByLayer<HmText>(v.strlyr);
            foreach (var e in txtlist)
            {
                // 현재 클릭한 영역에 있는 폐합된 폴리라인의 내부에 존재하는 엔티티 검색
                if (!IsPointInsideEntity(e, currPolygon))
                    continue;

                w.box = new LogBox() { leftX = e.Geometry.Get_Bounds2D().UpperLeft.X, leftY = e.Geometry.Get_Bounds2D().UpperLeft.Y, height = e.Height };
                valstr = e.TextString;
                w.xdatastr = e.Label;   // 실제XData
                w.update = true;
                break;
            }

            // 추가하는 경우
            if (!w.update)
            {
                DBDoc doc = DBDoc.Get_CurrDoc();
                // 2-1. 없으면 편집할수있도록 추가 후 진행
                if (ukey == 0)
                {
                    // 기본 추가 추가
                    DBDataDESC d = new DBDataDESC();
                    d.straKey = ukeyStra;
                    ukey = doc.desc.Add_TR(d);
                }
                
                HmDBKey dbkey = doc.Get_ActiveDrillLog();
                // XData 업데이트
                Dictionary<string, string> infoLabel = new Dictionary<string, string>()
                {
                    { "db", "DESC"},
                    { "var", v.valkey },
                    { "key", (dbkey != null) ? dbkey.nKey.ToString() : "0"},
                };
                valstr = v.valkey;
                w.xdatastr = CmdParmParser.GetLabelfromDictionary(infoLabel);
                w.box = new LogBox() { leftX = currPolygon.Geometry.Get_Bounds2D().UpperLeft.X, leftY = currPolygon.Geometry.Get_Bounds2D().UpperLeft.Y, height = currPolygon.Geometry.Get_Bounds2D().UpperLeft.Y - currPolygon.Geometry.Get_Bounds2D().LowerLeft.Y };
            }
            if (string.IsNullOrEmpty(valstr)) valstr = v.valkey;

            ShowControl(w, valstr);         
        }
        public void ShowEditorforDSF(LogTag v, double mx, double my, double px, double py, double pz)
        {
            // 1. 입력화면 전면에 표시
            var mngr = App.GetViewModelManager();
            InputUIViewModel vm = mngr.GetValue(typeof(InputUIViewModel), false) as InputUIViewModel;
            if (CurrLogStyle == eDepartment.Cut || CurrLogStyle == eDepartment.Tunnel)
            {
                vm.CurrentVM = mngr.GetValue(typeof(B2PartDescViewModel)) as B2PartDescViewModel;
            }
            else
            {
                vm.CurrentVM = mngr.GetValue(typeof(B1PartDescViewModel)) as B1PartDescViewModel;
            }

            // 2. 입력화면에 표시할 데이터키 찾기
            List<string> vmsg = new List<string>();
            // DESC 레이어에 해당하는 엔티티 검색
            string strlyr = v.strlyr;
            string keystr, valstr = string.Empty;
            List<HmEntity> vlist = dlog.GetEntityByLayer<HmEntity>(strlyr);
            uint ukey = 0, ukeyStra = 0;
            foreach (var e in vlist)
            {
                //현재 위치(px, py, pz)에 존재하는 엔티티 검색
                if (!IsPointInsideEntity(e, px, py))
                    continue;
                if (null == e.Label)
                    continue;
                valstr = CmdParmParser.GetValuefromKey(e.Label, "db", '&');
                // DESC
                if (0 != string.Compare("DESC", valstr))
                    continue;
                keystr = CmdParmParser.GetValuefromKey(e.Label, "var", '&');
                // 지층설명
                if (0 != string.Compare(v.valkey, keystr))
                    continue;
                keystr = CmdParmParser.GetValuefromKey(e.Label, "key", '&');
                if (!uint.TryParse(keystr, out ukey))
                    continue;
                // 연관지층키
                keystr = CmdParmParser.GetValuefromKey(e.Label, "strakey", '&');
                if (!uint.TryParse(keystr, out ukeyStra))
                    continue;
                
                // 지층이 있는 경우 
                if(0 != ukeyStra)
                    break;
            }
            if (ukeyStra == 0)
            {
                Utilities.NotifyHelper.Instance.Show(Utilities.NotifyHelper.NotiType.Error, "지층을 먼저 생성하세요");
                return;
            }

            // 2-1. 없으면 편집할수있도록 추가 후 진행
            if (ukey == 0)
            {
                // 기본 추가 추가
                DBDataDESC d = new DBDataDESC();
                d.straKey = ukeyStra;
                DBDoc doc = DBDoc.Get_CurrDoc();
                ukey = doc.desc.Add_TR(d);
            }

            // 3. 편집할 데이터키를 사용자 메시지로 전달
            if (ukey > 0)
            {
                DBDoc.Get_CurrDoc().Set_ActiveStratum(new HmDBKey("STRA", ukeyStra, (int)ukey));
                // 디키키 = 0 이면 추가하고, 디비키 > 0 이면 업데이트할 것
                string msgstr = string.Empty;
                if (CurrLogStyle == eDepartment.Cut || CurrLogStyle == eDepartment.Tunnel)
                    msgstr = string.Format("B2PartDesc", valstr);
                else
                    msgstr = string.Format("B1PartDesc", valstr);
                vmsg.Add(string.Format("key={0}", ukey));
                vmsg.Add(string.Format("strakey={0}", ukeyStra));
                // 사용자정의 메시지
                DBDoc.Get_CurrDoc().Get_TranCtrl().Send_Message(HmDataDocument.TRANSACTION_STATE.USER, msgstr, vmsg); // View에는 사용자정의 DBUpdate 메시지를 보냅니다.
            }
        }

        public void ShowEditorforPartC(LogTag v, double mx, double my, double px, double py, double pz)
        {
        }

        public void ShowEditorforPartD(LogTag v, double mx, double my, double px, double py, double pz)
        {
        }

        public void ShowEditorforPartE(LogTag v, double mx, double my, double px, double py, double pz)
        {
        }

        public void ShowEditorforPartY(LogTag v, double mx, double my, double px, double py, double pz)
        {

        }

        public void ShowEditorforPartZ(LogTag v, double mx, double my, double px, double py, double pz)
        {
            LogTag w = v.Clone();
            w.update = false;
            string valstr = w.valkey;

            // 현재 클릭한 심도검색
            string xdatastr = "심도표고두께";

            // 깊이를 결정
            //valstr = string.Format("{0:F2}", dlogstyle.baseDepth + ((v.box.leftY - py) / v.box.height) * dlogstyle.thisDepth);
            valstr = string.Format("{0:F2}", CalcCAD2Depth(py));

            // 추가를 위한 기본위치설정
            w.box.leftX = v.box.leftX;
            w.box.leftY = py;
            w.box.width = v.box.width;
            w.box.height = 2.0;

            List<HmText> vlist = dlog.GetEntityByLayer<HmText>(v.strlyr, xdatastr);
            foreach (var e in vlist)
            {
                HmBounds2D b = e.Geometry.Get_Bounds2D();
                if (2 == b.Include(px, py))
                {
                    // 기존 심도 변경을 위한 위치                    
                    double yy = CalcDepth2CAD(Convert.ToDouble(e.TextString));
                    w.box = new LogBox() { leftX = w.box.leftX, leftY = b.UpperLeft.Y, width = w.box.width, height = b.UpperLeft.Y - yy };
                    w.update = true;
                    valstr = e.TextString;
                    xdatastr = e.Label;
                    break;
                }
                else
                {
                    // 추가를 위한 기본위치설정
                    w.box.leftX = v.box.leftX;
                    w.box.leftY = py;
                    w.box.width = v.box.width;
                    w.box.height = 2.0;
                }
            }
            if (!w.update)
            {
                xdatastr = string.Format("&db=STRA&var={0}&currtext={1}", "심도표고두께", valstr);
            }
            // XData 업데이트
            w.xdatastr = xdatastr;
            //
            ShowControl(w, valstr);
        }

        private void Dlog_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            HideControl();
        }

        private void Handler_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.Key == System.Windows.Input.Key.Escape)
            {
                //ilog?.iEditorHide();
            }
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (aloadflag)
                return;
            aloadflag = true;

            SetIntercace(this);            
        }

#endregion

        //private uint GetKey(string xdata, string xformat, string strkey)
        //{
        //    string valstr = string.Empty;
        //    valstr = CmdParmParser.GetValuefromKey(xdata, "db", '&');
        //    // STRA 또는 SAMP
        //    if (0 != string.Compare("DESC", valstr))
        //        continue;
        //    keystr = CmdParmParser.GetValuefromKey(e.Label, "var", '&');
        //    // 지층설명
        //    if (0 != string.Compare(v.valkey, keystr))
        //        continue;
        //    keystr = CmdParmParser.GetValuefromKey(e.Label, "key", '&');
        //    if (!uint.TryParse(keystr, out ukey))
        //        continue;

        //}        
    }
}
