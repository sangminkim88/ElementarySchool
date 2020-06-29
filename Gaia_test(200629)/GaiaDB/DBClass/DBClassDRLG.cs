namespace GaiaDB
{
    using GaiaDB.Enums;
    using HmDataDocument;
    using HmGeometry;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public class DBDataDRLG : DBBaseData
    {
        /// <summary>
        /// 조사일
        /// </summary>
        public string SurveyDay;
        /// <summary>
        /// 시추장비
        /// </summary>
        public string DrillDevice;
        /// <summary>
        /// 해머효율
        /// </summary>

        private double hammerEfficiencyPercent;
        public double HammerEfficiencyPercent {
            get => hammerEfficiencyPercent;
            set {
                if (value >= 0 && value <= 100)
                {
                    hammerEfficiencyPercent = value;
                }
            }
        }
        /// <summary>
        /// 시추방법
        /// </summary>
        public string DrillingMethod;
        /// <summary>
        /// 시추각도
        /// </summary>
        public string DrillingAngleType;
        /// <summary>
        /// 교량명 또는 터널명
        /// </summary>
        public string BridgeOrTunnelName;
        /// <summary>
        /// 시추위치
        /// </summary>
        public string DrillLocation;
        /// <summary>
        /// 교량 또는 터널 위치 / 교대(각)위치
        /// </summary>
        public string BridgeOrTunnelLocation;
        /// <summary>
        /// 시추공경
        /// </summary>
        public string DrillPipe;
        /// <summary>
        /// 좌표
        /// </summary>
        public HmPoint2D Position;
        /// <summary>
        /// 시추공번
        /// </summary>
        public string DrillPipeNum;
        /// <summary>
        /// 시추표고
        /// </summary>
        public double Elevation;
        /// <summary>
        /// 시추심도
        /// </summary>
        public double Depth;
        /// <summary>
        /// 케이싱심도
        /// </summary>
        public double CasingDepth;
        /// <summary>
        /// 지하수의
        /// </summary>
        public double WaterLevel;
        /// <summary>
        /// 부서 종류(ex : 쌓기, 깍기, 교량 등)
        /// </summary>
        public eDepartment EDepartment;
        /// <summary>
        /// 시추주상도 위치 (Position) 좌표체계
        /// </summary>
        public HmGeoInfo geoInfo;
        /// <summary>
        /// 기타사항
        /// </summary>
        public string additionalInfo; // ver:0101에 추가
        /// <summary>
        /// 시추자
        /// </summary>
        public string DrillManName; // ver:0102에 추가
        /// <summary>
        /// 작성자
        /// </summary>
        public string WriteManName; // ver:0102에 추가

        public DBDataDRLG()
        {
            Position = new HmPoint2D();
            geoInfo = new HmGeoInfo();
            Initialize();
        }

        ~DBDataDRLG()
        {
            Initialize();
        }

        public void Initialize()
        {
            SurveyDay = DateTime.Now.ToString("yyyy-MM-dd");
            DrillDevice = "P4000SD";
            HammerEfficiencyPercent = 0.0;
            DrillingMethod = "회전수세식";
            DrillingAngleType = "";
            BridgeOrTunnelName = "";
            DrillLocation = "";
            BridgeOrTunnelLocation = "";
            DrillPipe = "";
            DrillPipeNum = "";
            Elevation = 0.0;
            Depth = 0.0;
            CasingDepth = 0.0;
            WaterLevel = 99999.0;
            EDepartment = eDepartment.None;

            Position.Initialize();
            geoInfo.Initialize();
            additionalInfo = "";
            DrillManName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];
            WriteManName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];
        }

        public DBDataDRLG Clone()
        {
            DBDataDRLG data = new DBDataDRLG();

            data.Position = Position.Clone();

            data.SurveyDay = SurveyDay;
            data.DrillDevice = DrillDevice;
            data.HammerEfficiencyPercent = HammerEfficiencyPercent;
            data.DrillingMethod = DrillingMethod;
            data.DrillingAngleType = DrillingAngleType;
            data.BridgeOrTunnelName = BridgeOrTunnelName;
            data.DrillLocation = DrillLocation;
            data.BridgeOrTunnelLocation = BridgeOrTunnelLocation;
            data.DrillPipe = DrillPipe;
            data.DrillPipeNum = DrillPipeNum;
            data.Elevation = Elevation;
            data.Depth = Depth;
            data.CasingDepth = CasingDepth;
            data.WaterLevel = WaterLevel;
            data.EDepartment = EDepartment;

            data.geoInfo = geoInfo.Clone();
            data.additionalInfo = additionalInfo;
            data.DrillManName = DrillManName;
            data.WriteManName = WriteManName;

            return data;
        }

        public override DBBaseData BaseClone()
        {
            return Clone();
        }

        public override bool Write_Binary(BinaryWriter binaryRW)
        {
            Position.Write_Binary(binaryRW);

            binaryRW.Write(SurveyDay);
            binaryRW.Write(DrillDevice);
            binaryRW.Write(HammerEfficiencyPercent);
            binaryRW.Write(DrillingMethod);
            binaryRW.Write(DrillingAngleType);
            binaryRW.Write(BridgeOrTunnelName);
            binaryRW.Write(DrillLocation);
            binaryRW.Write(BridgeOrTunnelLocation);
            binaryRW.Write(DrillPipe);
            binaryRW.Write(DrillPipeNum);
            binaryRW.Write(Elevation);
            binaryRW.Write(Depth);
            binaryRW.Write(CasingDepth);
            binaryRW.Write(WaterLevel);
            binaryRW.Write((int)EDepartment);

            if (!geoInfo.Write_Binary(binaryRW)) return false;
            binaryRW.Write(additionalInfo);
            binaryRW.Write(DrillManName);
            binaryRW.Write(WriteManName);

            return true;
        }

        public override bool Read_Binary(BinaryReader binaryRW, int nReadHMVer, int nReadVer)
        {
            Initialize();

            Position.Read_Binary(binaryRW, nReadHMVer);

            SurveyDay = binaryRW.ReadString();
            DrillDevice = binaryRW.ReadString();
            HammerEfficiencyPercent = binaryRW.ReadDouble();
            DrillingMethod = binaryRW.ReadString();
            DrillingAngleType = binaryRW.ReadString();
            BridgeOrTunnelName = binaryRW.ReadString();
            DrillLocation = binaryRW.ReadString();
            BridgeOrTunnelLocation = binaryRW.ReadString();
            DrillPipe = binaryRW.ReadString();
            DrillPipeNum = binaryRW.ReadString();
            Elevation = binaryRW.ReadDouble();
            Depth = binaryRW.ReadDouble();
            CasingDepth = binaryRW.ReadDouble();
            WaterLevel = binaryRW.ReadDouble();
            EDepartment = (eDepartment)binaryRW.ReadInt32();

            if (!geoInfo.Read_Binary(binaryRW, nReadHMVer)) return false;
            if (nReadVer < 101) return true;
            additionalInfo = binaryRW.ReadString();
            if (nReadVer < 102) return true;
            DrillManName = binaryRW.ReadString();
            WriteManName = binaryRW.ReadString();

            return true;
        }


        /////////////////////////////////////////////////////////////////////////

/// <summary>총 Page수를 넘겨줍니다.</summary>
#if false
        public int Get_PageCount(double dFirstDepth = 20.0, double dSecondDepth = 20.0)
        {
            return Get_PageNumber(Depth, dFirstDepth, dSecondDepth);
        }        
#endif
        /// <summary>해당깊이에서의 Page Number(1번부터 시작)를 넘겨줍니다.</summary>
        static public int Get_PageNumber(double dCurrDepth, double dFirstDepth = 20.0, double dSecondDepth = 20.0)
        {
#if true
            int pageID = 1;
            double zdepth = dCurrDepth;
            zdepth -= dSecondDepth;
            if(zdepth > 0.0)
            {
                pageID += (int)Math.Ceiling(zdepth / dFirstDepth);
            }
            return pageID;
#else
            int pageID = 0;
            if (dCurrDepth > dFirstDepth)
            { pageID = MathUtil.RoundUp((dCurrDepth - dFirstDepth) / dSecondDepth); }

            return pageID + 1;
#endif
        }
    }

    public class DBClassDRLG : DBBaseClass
    {
        private double normalDrillLogHeight = 20;

        public double NormalDrillLogHeight
        {
            get { return normalDrillLogHeight; }
            set { normalDrillLogHeight = value; }
        }

        private double legendDrillLogHeight = 20;

        public double LegendDrillLogHeight
        {
            get { return legendDrillLogHeight; }
            set { legendDrillLogHeight = value; }
        }

        #region Constructors

        public DBClassDRLG(HmBaseDoc doc)
        {
            m_strDBKey = "DRLG";
            m_strDBNote = "시추 주상도";
            m_bSingleData = false; // 복수 자료구조
            Set_Doc(doc);
        }

#endregion

#region Methods

        // 복수 자료구조인경우 ////////////////////////////////////////////////////////////////////////////////////////////
        // 아래 함수형은 복수자료형방식으로 타 복수자료방식과 동일합니다.(Get_Data ~ Del_TR)

        /// <summary>
        /// DataPool에서 해당 Key의 Data를 가져옵니다.
        /// </summary>
        public bool Get_Data(uint nKey, ref DBDataDRLG data, bool bClone = true)
        { // Clone이 아닌경우 자료를 가져와 수정시 Transaction을 통한 관리가 되지 않습니다. 또한 단위계는 내부 공용단위계 값으로 넘겨줍니다. 
          // 따라서 bClone을 false로 쓸때에는 수정하지 않고 공용단위계로 쓸경우에만(View화면갱신 등) 사용하도록 합니다.
            DBBaseData baseData = null;
            if (!Get_BaseData(nKey, ref baseData, bClone)) return false;
            data = (DBDataDRLG)baseData;
            return true;
        }

        /// <summary>
        /// DataPool에서 해당 KeyList의 DataList를 가져옵니다.
        /// </summary>
        public bool Get_DataList(HmKeyList keyList, ref List<DBDataDRLG> dataList, bool bClone = true)
        {
            DBDataDRLG data = new DBDataDRLG();
            for (int i = 0; i < keyList.list.Count; i++)
            {
                if (!Get_Data(keyList.list[i], ref data, bClone))
                { dataList.Clear(); return false; }
                dataList.Add(data);
            }
            return true;
        }

        // Transaction을 발생시키며 Data를 변경하는 경우  ////////////////////////////////// 

        /// <summary>
        /// DB에 data를 추가합니다.(Transaction발생)
        /// </summary>
        /// <param name="data">추가할 Data</param>    
        public uint Add_TR(DBDataDRLG data)
        { return Add_BaseTR(data); }
        /// <summary>
        /// DB에 dataList를 추가합니다.(Transaction발생)
        /// </summary>
        public bool Add_TR(List<DBDataDRLG> dataList)
        {
            List<DBBaseData> baseList = new List<DBBaseData>();
            for (int i = 0; i < dataList.Count; i++) baseList.Add(dataList[i]);
            return Add_BaseTR(baseList);
        }
        /// <summary>
        /// DB에 data를 수정합니다.(Transaction발생)
        /// </summary>
        public bool Modify_TR(uint nKey, DBDataDRLG data)
        { return Modify_BaseTR(nKey, data); }
        /// <summary>
        /// DB에 dataList를 수정합니다.(Transaction발생)
        /// </summary> 
        public bool Modify_TR(HmKeyList keyList, List<DBDataDRLG> dataList)
        {
            List<DBBaseData> baseList = new List<DBBaseData>();
            for (int i = 0; i < dataList.Count; i++) baseList.Add(dataList[i]);
            return Modify_BaseTR(keyList, baseList);
        }
        // DB에 data를 제거합니다.(Transaction발생) - DBBaseClass에 정의 되어있음
        //public bool Del_TR(uint nKey)         
        //public bool Del_TR(HMKeyList keyList) 

        // Transaction없이 Data를 변경하는 경우 (TR발생하지 않으므로 사용시 주의 요망) /////

        // - DBBaseClass에 정의 되어있으며 여러종류의 Data 변경을 하나의 Transaction내에서 변경할경우에 사용합니다.
        // public uint Add_Data(uint nKey, DBBaseData data)
        // public bool Modify_Data(uint nKey, DBBaseData data)
        // public bool Del_Data(uint nKey)



        // 재정의 함수 //////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// 해당 data의 자료의 오류여부를 검토합니다. 오류 발생시에 오류 내용을 strMsg에 채워서 넘겨줍니다.
        /// </summary> 
        public override bool Check(uint nKey, DBBaseData data, ref string strMsg)
        {
            DBDoc doc = (DBDoc)m_Doc;
            DBDataDRLG newData = (DBDataDRLG)data;
            DBDataDRLG oldData = null;
            if (doc.drlg.Get_Data(nKey, ref oldData)) {
                if (!newData.DrillPipeNum.Equals(oldData.DrillPipeNum))
                {
                    HmKeyList keyList = new HmKeyList();
                    int nSize = doc.drlg.Get_KeyList(keyList);
                    DBDataPool dataPool = doc.drlg.Get_DataPool();
                    for (int n = 0; n < nSize; n++)
                    {
                        DBDataDRLG dbData = null;
                        if (doc.drlg.Get_Data(keyList.list[n], ref dbData))
                        {
                            if (dbData.DrillPipeNum.Equals(newData.DrillPipeNum))
                            {
                                strMsg = "동일이름의 시추공이 있습니다.";
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }
        /// <summary>
        /// 해당 data의 자료를 단위별로 UnitFactor값을 곱하여 넘겨줍니다.
        /// </summary>
        public override void Convert_Unit(DBBaseData data, Dictionary<UNIT_TYPE, double> dicUnitFactor)
        {
            DBDataDRLG currData = (DBDataDRLG)data;
            // 단위계작업이 필요합니다.
        }
        /// <summary>
        /// 해당 data의 자료 변환시 연관관계 처리를 수행합니다.
        /// </summary> 
        protected override bool Association(uint nKey, DBBaseData data, DBBaseData data_Org, TRANSACTION_DATA type)
        {
            DBDataDRLG currData = type == TRANSACTION_DATA.DEL ? null : (DBDataDRLG)data;
            DBDoc doc = DBDoc.Get_CurrDoc();
            if (type == TRANSACTION_DATA.ADD)
            { /*Data 추가시 연관관계 처리를 합니다.(일반적으로 Add시에 연관처리가 필요한 경우는 없습니다.)*/ }
            else if (type == TRANSACTION_DATA.MODIFY)
            { /*Data 수정시 연관관계 처리를 합니다.*/ }
            else if (type == TRANSACTION_DATA.DEL)
            {
                /*Data 삭제시 연관관계 처리를 합니다.(Del시에는 data값이 Null로 넘어오므로 주의를 요합니다.)*/

                HmKeyList straKeyList = new HmKeyList();
                doc.stra.Get_KeyList(straKeyList, nKey);
                if (!doc.stra.Del_Data(straKeyList)) return false;

                if (!doc.sptg.Del_Data(nKey)) return false;

                HmKeyList sampKeyList = new HmKeyList();
                doc.samp.Get_KeyList(sampKeyList, nKey);
                if (!doc.samp.Del_Data(sampKeyList)) return false;
            }
            return true;
        }

        // User Binary 방식 저장시 필수 재지정 함수 ////////////////////////////////////////////////////////////////////
        public override DBBaseData New_Data()
        {
            return new DBDataDRLG();
        }



#endregion
        // 추가 함수 ///////////////////////////////////////////////////////////////////////////////////////////////////
        public override string Get_Name(DBBaseData baseData)
        {
            DBDataDRLG data = (DBDataDRLG)baseData;
            return data.DrillPipeNum;
        }

        /// <summary>보유하고 있는 전체 자료중 소속된 DRLG의 EDepartment값이 같은 Key값을 넘겨줍니다.</summary>
        public int Get_KeyList(HmKeyList keyList, eDepartment eDepartment)
        {
            DBDoc doc = DBDoc.Get_CurrDoc();
            if (Get_KeyList(keyList).Equals(0)) return 0;

            for (int i = keyList.Count - 1; i >= 0; i--)
            {
                DBDataDRLG itemD = null;
                Get_Data(keyList.list[i], ref itemD, false);
                if (!itemD.EDepartment.Equals(eDepartment))
                { keyList.list.RemoveAt(i); }
            }

            return keyList.Count;
        }

        /// <summary>보유하고 있는 전체 자료중 소속된 DRLG의 EDepartment list 값이 같은 Key값을 넘겨줍니다.</summary>
        public int Get_KeyList(HmKeyList keyList, List<eDepartment> statusList)
        {
            DBDoc doc = (DBDoc)m_Doc;
            if (Get_KeyList(keyList) == 0) return 0;
            if (statusList.Count < 1) return 0;

            for (int i = keyList.Count - 1; i >= 0; i--)
            {
                DBDataDRLG itemD = null;
                Get_Data(keyList.list[i], ref itemD, false);
                if (!statusList.Contains(itemD.EDepartment))
                { keyList.list.RemoveAt(i); }
            }

            return keyList.Count;
        }


        public eDepartment Get_Department(uint nKey)
        {
            DBDataDRLG itemD = null;
            if (!Get_Data(nKey, ref itemD, false)) return eDepartment.None;
            return itemD.EDepartment;
        }

        /// <summary>해당 시추공의 깊이를 넘겨줍니다.</summary>
        /// <param name="drlgKey">시추공의 Key</param>
        /// <param name="dDepth">시추공깊이를 넘겨받을 변수</param>
        /// <param name="bConsiderStraData">시추공에 포함된 지층의 최종깊이를 고려할지 여부</param>
        public bool Get_Depth(uint drlgKey, ref double dDepth, bool bConsiderStraData = true)
        {
            DBDataDRLG itemD = null;
            if (!Get_Data(drlgKey, ref itemD)) return false;
            if (!bConsiderStraData) { dDepth = itemD.Depth; return true; } 
            
            DBDoc doc = (DBDoc)m_Doc;
            HmKeyList straKList = new HmKeyList();
            List<double> depthList = new List<double>();

            if(doc.stra.Get_KeyList(straKList, drlgKey, true, depthList) == 0) { dDepth = itemD.Depth; return true; }

            dDepth = Math.Max(itemD.Depth, depthList[depthList.Count-1]);
            return true;
        }
        /// <summary>해당 시추공의 Page수를 넘겨줍니다.</summary>
        /// <param name="drlgKey">시추공의 Key</param>
        /// <param name="bConsiderStraData">시추공에 포함된 지층의 최종깊이를 고려할지 여부</param>
        /// <returns>Page수</returns>
        public int Get_PageCount(uint drlgKey, bool bConsiderStraData = true)
        {
            DBDoc doc = DBDoc.Get_CurrDoc();
            HmKeyList straKeys = new HmKeyList();
            doc.stra.Get_KeyList(straKeys, drlgKey);
            List<Tuple<double, string, uint, uint>> SortDesc = DescriptionUtil.SortDescription(drlgKey);


            double dCurrDepth = 0.0;
            if (!Get_Depth(drlgKey, ref dCurrDepth, bConsiderStraData)) return 1;

            int returnData = DBDataDRLG.Get_PageNumber(dCurrDepth, this.normalDrillLogHeight, this.legendDrillLogHeight);
            if (SortDesc.Count > 0)
            {
                double lastDepth = SortDesc.LastOrDefault().Item1;
                int tmpPageCount = (int)(lastDepth / this.normalDrillLogHeight) + 1;
                if ((lastDepth % this.normalDrillLogHeight) > this.legendDrillLogHeight)
                {
                    tmpPageCount++;
                }
                if (returnData < tmpPageCount)
                {
                    returnData = tmpPageCount;
                }
            }
            return returnData;


        }
    }
}
