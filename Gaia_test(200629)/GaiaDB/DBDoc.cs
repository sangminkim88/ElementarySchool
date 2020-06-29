namespace GaiaDB
{
    using System.Collections.Generic;
    using GaiaDB.Enums;
    using HmDataDocument;
    using HmGeometry;
    using System;

    public class DBDoc : HmBaseDoc
    {
        public DBClassPROJ proj;    //사업 정보
        public DBClassDRLG drlg;    //시추 주상도
        public DBClassSTRA stra;    //지층 레벨
        public DBClassDESC desc;    //지층설명
        public DBClassSAMP samp;    //채취 심도별 시료 형태
        public DBClassSPTG sptg;    //표준관입시험
        public DBClassPOSI posi;    //위치도
        public DBClassJSHP jshp;    //절리형상


        public DBDoc(bool bRegisterDoc = true) : base(bRegisterDoc)
        {
            m_nVersion = DBDoc_Define.nVersion;
            m_strProgramName = DBDoc_Define.strProgramName;
            m_strExt = "gab";

            if(m_TranCtrl != null) m_TranCtrl.Set_MaxTRSize(20);// 최대 Undo갯수를 10->20개로 변경

            // 연관관계 순서로 생성해줍니다.(ex : Node정보는 Elem정보보다 먼저 생성합니다.) 
            // ※ 해당 순서는 읽기쓰기 순서이며, 연관관계만 영향이 없다면 순서가 바뀌어도 읽고 쓰기에 문제가 발생되지는 않습니다.

            proj = new DBClassPROJ(this);
            drlg = new DBClassDRLG(this);
            stra = new DBClassSTRA(this);
            desc = new DBClassDESC(this);
            samp = new DBClassSAMP(this);
            sptg = new DBClassSPTG(this);
            posi = new DBClassPOSI(this);
            jshp = new DBClassJSHP(this);

            Reg_DocPoint(); // 필수 등록
        }

        ~DBDoc()
        {
        }

        public static DBDoc Get_CurrDoc()
        {
            HmBaseDoc hmDoc = Get_CurrHMDoc(DBDoc_Define.strProgramName);
            if (hmDoc == null)
            { return new DBDoc(); }
            return (DBDoc)hmDoc;
        }



        /// <summary>
        /// 해당 파일에서 HINFO정보를 만들어 줍니다.(해당 함수가 재지정되어 있으면, 저장시 자동으로 해당 함수를 호출해서 파일에 Info정보를 저장합니다.)
        /// </summary>
        /// <param name="InfoD">넘겨받을 HINFO정보</param>
        /// <returns>정보가 생성되었는지 여부</returns>
        public override bool Make_InfoData(ref DBDataHINFO infoD)
        {
            if (!hinfo.Get_Data(ref infoD)) infoD.Initialize();

            if (infoD.strNote == "" || infoD.strNote == "프로젝트 이름이 없습니다.")
            {
                infoD.strNote = "프로젝트 이름이 없습니다.";
            }

            if (infoD.dicStr.ContainsKey("FINAL_DATE")) infoD.dicStr["FINAL_DATE"] = DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
            else infoD.dicStr.Add("FINAL_DATE", DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss"));

            IMainFrame iMainFrame = MainFrame_Imp.Get_MainFrame();
            if (iMainFrame != null)
            {
                HmBitmap imageD = new HmBitmap();
                imageD.img = iMainFrame.Get_PreViewImage();
                if (imageD.img != null)
                {
                    if (infoD.dicImage.ContainsKey("PREVIEW")) infoD.dicImage["PREVIEW"] = imageD;
                    else infoD.dicImage.Add("PREVIEW", imageD);
                }
            }

            DBDataPROJ projD = null;
            if (proj.Get_Data(ref projD))
            {
                if (infoD.dicStr.ContainsKey("PROJECT_NAME")) infoD.dicStr["PROJECT_NAME"] = projD.ProjectName;
                else infoD.dicStr.Add("PROJECT_NAME", projD.ProjectName);

                if (infoD.dicStr.ContainsKey("COMPANY_NAME")) infoD.dicStr["COMPANY_NAME"] = projD.CompanyName;
                else infoD.dicStr.Add("COMPANY_NAME", projD.CompanyName);
            }
     
            int count;
            HmKeyList drlgKeyList = new HmKeyList();
            foreach (eDepartment item in Enum.GetValues(typeof(eDepartment)))
            {
                count = drlg.Get_KeyList(drlgKeyList, item);
                string sEnum = item.ToString();

                if (infoD.dicInt.ContainsKey(sEnum))
                {
                    infoD.dicInt[sEnum] = count;
                }
                else
                {
                    infoD.dicInt.Add(sEnum, count);
                }

                drlgKeyList.Clear();
            }

            return true;
        }


        /// <summary>파일 Open시 완료후, Transaction메시지 출력전에 호출됩니다.</summary>
        public override void OpenDocumentEvent(int nReadVersion)
        {
            m_activeStyle = eDepartment.None;
            m_styleActiveDRLG = null;
            m_activeDRLG = null;
            m_activeSTRA = null;

            m_sendStyle = eDepartment.None;
            m_sendDRLG = null;
            m_sendSTRA = null;

            if (nReadVersion < 103)
            {
                HmKeyList keylist = new HmKeyList();
                int nSize = desc.Get_KeyList(keylist);
                for (int i = 0; i < nSize; i++)
                {
                    DBDataDESC descD = null;
                    if (desc.Get_Data(keylist.list[i], ref descD, false))
                    {
                        // Desc에 있던 None(이전Stratum)
                        // 정보를 Dic에 StraKey와 내용을 모으고
                        uint straKey = descD.straKey;
                        if (descD.dicDesc.ContainsKey(eDescriptionKey.None))
                        {
                            string strStraType = descD.dicDesc[eDescriptionKey.None];

                            // Dic로 모은 정보는 Stra에 새로만든 변수에 반영해줍니다.
                            DBDataSTRA straD = null;
                            if (stra.Get_Data(straKey, ref straD, false))
                            {
                                straD.stratumName = strStraType;
                            }

                            // Desc에 있는 None정보는 제거
                            if (descD.dicDesc.Remove(eDescriptionKey.None)) { }
                        }
                    }
                }
                
            }

            return;
        }


        /// <summary>파일 New시 완료후, Transaction메시지 출력전에 호출됩니다.</summary>
        public override void NewDocumentEvent()
        {
            m_activeStyle = eDepartment.None;
            m_styleActiveDRLG = null;
            m_activeDRLG = null;
            m_activeSTRA = null;

            m_sendStyle = eDepartment.None;
            m_sendDRLG = null;
            m_sendSTRA = null;
            return;
        }

        /// <summary>파일 Save시 완료후, Transaction메시지 출력전에 호출됩니다.</summary>
        public override void SaveDocumentEvent()
        {
            return;
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        private eDepartment m_activeStyle = eDepartment.None;
        private Dictionary<eDepartment, HmDBKey> m_styleActiveDRLG = null;
        private HmDBKey m_activeDRLG = null; // 현재 Active된 시추공(DRLG)의 Key(nKey)와 Page Number(nSubID:1부터시작)을 보관합니다.
        private HmDBKey m_activeSTRA = null; // 현재 Active된 지층(STRA)의 Key(nKey)와 하위 설명(nSubID : DESC Key), 시료번호(n3rdID : SAMP Key)를 보관합니다.
        private bool m_activeDRLGLock = false;
        private bool m_activeSTRALock = false;

        private eDepartment m_sendStyle = eDepartment.None;
        private HmDBKey m_sendDRLG = null;
        private HmDBKey m_sendSTRA = null;

        /// <summary>현재 보여주고 있는DrillLog의 Key 정보및 Page정보(nSubID:1부터시작)를 넘겨줍니다.</summary>
        /// <param name="trSendData">TRANSACTION에서 메시지를 보내는 ActiveDrillLog정보를 넘겨받을지 여부(TRANSACTION에서 보내지 않을때는 현재 ActiveDrillLog정보를 넘겨줍니다.)</param>
        /// <returns></returns>
        public HmDBKey Get_ActiveDrillLog(bool trSendData = false)
        {
            if (trSendData && m_sendDRLG != null) return m_sendDRLG;

            eDepartment activeStyle = Get_ActiveStyle(trSendData);
            if (m_activeDRLG != null)
            { if(!trSendData || activeStyle == drlg.Get_Department(m_activeDRLG.nKey)) return m_activeDRLG.Clone(); }

            if (activeStyle != eDepartment.None) return Get_ActiveDrillLog(activeStyle);
            return new HmDBKey(); // 선택된 Drlg없음
        }
        /// <summary>해당 style의 가장 최근에 Active되었던(없으면 첫번째) DrillLog의 Key 정보및 Page정보(nSubID:1부터시작)를 넘겨줍니다.</summary>
        public HmDBKey Get_ActiveDrillLog(eDepartment style)
        {
            if (m_styleActiveDRLG != null)
            {
                if (m_styleActiveDRLG.ContainsKey(style)) return m_styleActiveDRLG[style].Clone();
            }
            HmKeyList drlgKList = new HmKeyList();
            if (drlg.Get_KeyList(drlgKList, style) > 0) return new HmDBKey("DRLG", drlgKList[0], 1);
            return new HmDBKey(); // 선택된 Drlg없음
        }
        /// <summary>현재 보여주고 있는DrillLog의 형식을 넘겨줍니다.</summary>
        /// <param name="trSendData">TRANSACTION에서 메시지를 보내는 ActiveStyle정보를 넘겨받을지 여부(TRANSACTION에서 보내지 않을때는 현재 ActiveStyle정보를 넘겨줍니다.)</param>
        public eDepartment Get_ActiveStyle(bool trSendData = false)
        {
            if (trSendData && m_sendStyle != eDepartment.None) return m_sendStyle;
            return m_activeStyle;
        }
        /// <summary>보여줄 DrillLog의 Key 정보및 Page정보(nSubID:1부터시작)를 입력합니다.</summary>
        public void Set_ActiveDrillLog(HmDBKey drlgKey)
        {
            if (m_TranCtrl == null || m_activeDRLGLock) return;

            // DB에 없는 데이터가 들어오면 active DRLG와 해당 STRA가 null 됩니다.
            if (!drlg.Is_Data(drlgKey.nKey))
            {
                m_activeDRLG = null;
                m_activeSTRA = null;

                if (m_styleActiveDRLG.ContainsKey(m_activeStyle))
                { m_styleActiveDRLG.Remove(m_activeStyle); }

                return;
            }

            if (m_activeDRLG != null)
            {
                if (drlgKey.strDBKey == m_activeDRLG.strDBKey)
                {
                    if (drlgKey.nKey == m_activeDRLG.nKey)
                    { if (drlgKey.nSubID < 0 || drlgKey.nSubID == m_activeDRLG.nSubID) return; }
                }
            }

            HmDBKey hmDBKey = drlgKey.Clone();
            int pageCount = drlg.Get_PageCount(drlgKey.nKey);
            if(pageCount < hmDBKey.nSubID)
            {
                hmDBKey.nSubID = 1;
            }
            m_activeDRLGLock = true;

            m_sendDRLG = hmDBKey;
            m_sendStyle = drlg.Get_Department(drlgKey.nKey);
            if(m_activeDRLG == null) m_activeSTRA = null;
            else if (drlgKey.nKey == 0 || drlgKey.nKey != m_activeDRLG.nKey) m_activeSTRA = null;

            m_TranCtrl.Send_Message(TRANSACTION_STATE.USER, "ActiveDrillLog", hmDBKey);

            m_sendDRLG = null;
            m_sendStyle = eDepartment.None;

            m_activeDRLG = hmDBKey;
            if (drlgKey.nKey != 0)
            {
                if (m_styleActiveDRLG == null) m_styleActiveDRLG = new Dictionary<eDepartment, HmDBKey>();
                m_activeStyle = drlg.Get_Department(drlgKey.nKey);
                if (m_activeStyle != eDepartment.None)
                {
                    if (m_styleActiveDRLG.ContainsKey(m_activeStyle)) m_styleActiveDRLG[m_activeStyle] = hmDBKey;
                    else m_styleActiveDRLG.Add(m_activeStyle, hmDBKey);
                }
            }

            m_activeDRLGLock = false;
        }
        /// <summary>보여줄 DrillLog의  style정보를 설정합니다.(이전에 Active되었거나 첫번째 DrillLog를 Active상태로 만듭니다)</summary>
        public void Set_ActiveStyle(eDepartment style)
        {
            if (m_TranCtrl == null || m_activeDRLGLock) return;

            if (m_activeStyle == style) return;

            if (m_styleActiveDRLG != null)
            {
                if (m_styleActiveDRLG.ContainsKey(style))
                { Set_ActiveDrillLog(m_styleActiveDRLG[style]); return; } // 기존 기록이 있으면 기존 기록정보 ActiveDrillLog로 날립니다.
            }
            HmKeyList currKList = new HmKeyList();
            if (drlg.Get_KeyList(currKList, style) > 0)
            { Set_ActiveDrillLog(new HmDBKey("DRLG", currKList[0], 1)); return; }

            // 해당 Style의 Drlg정보가 없는 경우
            m_activeDRLGLock = true;
            m_sendStyle = style;
            m_activeSTRA = null;
            m_TranCtrl.Send_Message(TRANSACTION_STATE.USER, "ActiveDrillStyle", style);
            m_sendStyle = eDepartment.None;

            m_activeStyle = style;
            m_activeDRLG = null;
            m_activeDRLGLock = false;
        }
        public void Set_FirstActiveStyle()
        {            
            if (m_activeStyle == eDepartment.None)
            {
                eDepartment style = eDepartment.Fill;
                HmKeyList drlgKList = new HmKeyList();
                if (drlg.Get_KeyList(drlgKList, eDepartment.Fill) > 0)
                { style = eDepartment.Fill; }
                else if (drlg.Get_KeyList(drlgKList, eDepartment.Cut) > 0)
                { style = eDepartment.Cut; }
                else if (drlg.Get_KeyList(drlgKList, eDepartment.Bridge) > 0)
                { style = eDepartment.Bridge; }
                else if (drlg.Get_KeyList(drlgKList, eDepartment.Tunnel) > 0)
                { style = eDepartment.Tunnel; }
                else if (drlg.Get_KeyList(drlgKList, eDepartment.BorrowPit) > 0)
                { style = eDepartment.BorrowPit; }
                else if (drlg.Get_KeyList(drlgKList, eDepartment.TrialPit) > 0)
                { style = eDepartment.TrialPit; }
                else if (drlg.Get_KeyList(drlgKList) > 0)
                { style = drlg.Get_Department(drlgKList[0]); }

                Set_ActiveStyle(style);
            }            
        }

        /// <summary>현재 보여주고 있는 지층(Stra)의 Key 정보및 지층설명 정보(nSubID:Desc의 Key값), 시료번호(n3rdID : SAMP Key)를 넘겨줍니다.(연결요망)</summary>
        /// <param name="trSendData">TRANSACTION에서 메시지를 보내는 ActiveStyle정보를 넘겨받을지 여부(TRANSACTION에서 보내지 않을때는 현재 ActiveStyle정보를 넘겨줍니다.)</param>
        public HmDBKey Get_ActiveStratum(bool trSendData = false)
        {
            if (trSendData && m_sendSTRA != null) return m_sendSTRA;

            if (m_activeSTRA != null) return m_activeSTRA.Clone();
            HmDBKey drlgKey = Get_ActiveDrillLog(trSendData);

            if (drlgKey.nKey == 0) return new HmDBKey();

            HmDBKey activeSTRA = new HmDBKey("STRA", 0, 0, 0);
            HmKeyList straKList = new HmKeyList();
            HmKeyList descKList = new HmKeyList();
            HmKeyList sampKList = new HmKeyList();
            if (stra.Get_KeyList(straKList, drlgKey.nKey) > 0)
            {
                activeSTRA.nKey = straKList[0];
                if (desc.Get_KeyList(descKList, activeSTRA.nKey) > 0)
                { activeSTRA.nSubID = (int)(descKList[0]); }
            }
            if (samp.Get_KeyList(sampKList, drlgKey.nKey) > 0)
            { activeSTRA.n3rdID = (int)(sampKList[0]); }

            return activeSTRA;
        }
        /// <summary>보여줄 지층(Stra)의 Key 정보및 지층설명 정보(nSubID:Desc의 Key값), 시료번호(n3rdID : SAMP Key)를 설정합니다.(연결요망)</summary>
        public void Set_ActiveStratum(HmDBKey straKey)
        {
            if (m_TranCtrl == null || m_activeSTRALock) return;

            if (straKey.strDBKey != "") // strDBKey값이 ""인경우 미선택 상태로 만듭니다.
            {
                if (straKey.strDBKey != "STRA")
                {
                    if (straKey.strDBKey == "SAMP")
                    { straKey.Set("STRA", 0, 0, (int)(straKey.nKey)); }
                    else if (straKey.strDBKey == "DESC")
                    { straKey.Set("STRA", 0, (int)(straKey.nKey), 0); }
                    else if (straKey.strDBKey == "")
                    { } // 미선택 상태
                    else return; // 기타 항목에 대해선 미지원합니다.
                }


                if (straKey.n3rdID != 0)
                {
                    if (straKey.nSubID == 0) straKey.nSubID = (int)(samp.Get_DescKey((uint)(straKey.n3rdID)));
                    if (straKey.nKey == 0) straKey.nKey = samp.Get_StraKey((uint)(straKey.n3rdID));
                }
                else if (straKey.nSubID != 0)
                {
                    HmKeyList keyList = new HmKeyList();
                    //if (straKey.n3rdID == 0) 확정이므로 검토하지 않음
                    { if (0 < samp.Get_KeyList_DESC(keyList, (uint)(straKey.nSubID))) straKey.n3rdID = (int)(keyList[0]); }
                    if (straKey.nKey == 0) straKey.nKey = desc.Get_StraKey((uint)(straKey.nSubID));
                }
                else if (straKey.nKey != 0)
                {
                    HmKeyList keyList = new HmKeyList();
                    //if (straKey.n3rdID == 0) 확정이므로 검토하지 않음
                    { if (0 < samp.Get_KeyList_STRA(keyList, straKey.nKey)) straKey.n3rdID = (int)(keyList[0]); }
                    //if (straKey.nSubID == 0) 확정이므로 검토하지 않음
                    { if (0 < desc.Get_KeyList(keyList, straKey.nKey)) straKey.nSubID = (int)(keyList[0]); }
                }
                else return;


                if (m_activeSTRA != null)
                {
                    if (straKey.strDBKey == m_activeSTRA.strDBKey)
                    {
                        if (straKey.nKey == m_activeSTRA.nKey)
                        { if (straKey.nSubID < 1 || straKey.nSubID == m_activeSTRA.nSubID) return; }
                    }
                }
            }

            m_activeSTRALock = true;

            m_sendSTRA = straKey.Clone();

            m_TranCtrl.Send_Message(TRANSACTION_STATE.USER, "ActiveStratum", straKey.Clone());

            m_sendSTRA = null;
            m_activeSTRA = straKey.Clone();

            m_activeSTRALock = false;
        }
    }

    public class DBDoc_Define
    {
        #region Constants

        public const int nVersion = 0103;// 현재 버젼

        public const string strProgramName = "Gaia";// 프로그램 명

        #endregion
    }
}
