namespace GaiaDB
{
    using GaiaDB.Enums;
    using HmDataDocument;
    using HmGeometry;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// 채취 심도별 시료 형태 데이터
    /// 시료형태 UD=0, DS=1, NS=2, CS=3
    /// </summary>
    public class DBDataSAMP : DBBaseData
    {
        /// <summary>
        /// 해당 시추주상도 데이터의 키
        /// </summary>
        public uint drlgKey;

        /// <summary>
        /// 시료형태
        /// </summary>
        public eSampleType SType;

        /// <summary>
        /// 심도
        /// </summary>
        public double Depth;

        public DBDataSAMP()
        {
            Initialize();
        }

        ~DBDataSAMP()
        {
            Initialize();
        }

        public void Initialize()
        {
            drlgKey = 0;
            SType = eSampleType.DS;
            Depth = 0.0;
        }

        public DBDataSAMP Clone()
        {
            DBDataSAMP data = new DBDataSAMP();
            data.drlgKey = drlgKey;
            data.SType = SType;
            data.Depth = Depth;
            return data;
        }

        public override DBBaseData BaseClone()
        {
            return Clone();
        }

        public override bool Write_Binary(BinaryWriter binaryRW)
        {
            binaryRW.Write(drlgKey);
            binaryRW.Write((int)SType);
            binaryRW.Write(Depth);
            return true;
        }

        public override bool Read_Binary(BinaryReader binaryRW, int nReadHMVer, int nReadVer)
        {
            Initialize();
            drlgKey = binaryRW.ReadUInt32();
            SType = (eSampleType)binaryRW.ReadInt32();
            Depth = binaryRW.ReadDouble();
            return true;
        }
    }

    public class DBClassSAMP : DBBaseClass
    {
        #region Constructors

        public DBClassSAMP(HmBaseDoc doc)
        {
            m_strDBKey = "SAMP";
            m_strDBNote = "채취 심도별 시료 형태";
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
        public bool Get_Data(uint nKey, ref DBDataSAMP data, bool bClone = true)
        { // Clone이 아닌경우 자료를 가져와 수정시 Transaction을 통한 관리가 되지 않습니다. 또한 단위계는 내부 공용단위계 값으로 넘겨줍니다. 
          // 따라서 bClone을 false로 쓸때에는 수정하지 않고 공용단위계로 쓸경우에만(View화면갱신 등) 사용하도록 합니다.
            DBBaseData baseData = null;
            if (!Get_BaseData(nKey, ref baseData, bClone)) return false;
            data = (DBDataSAMP)baseData;
            return true;
        }

        /// <summary>
        /// DataPool에서 해당 KeyList의 DataList를 가져옵니다.
        /// </summary>
        public bool Get_DataList(HmKeyList keyList, ref List<DBDataSAMP> dataList, bool bClone = true)
        {
            DBDataSAMP data = new DBDataSAMP();
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
        public uint Add_TR(DBDataSAMP data)
        { return Add_BaseTR(data); }
        /// <summary>
        /// DB에 dataList를 추가합니다.(Transaction발생)
        /// </summary>
        public bool Add_TR(List<DBDataSAMP> dataList)
        {
            List<DBBaseData> baseList = new List<DBBaseData>();
            for (int i = 0; i < dataList.Count; i++) baseList.Add(dataList[i]);
            return Add_BaseTR(baseList);
        }
        /// <summary>
        /// DB에 data를 수정합니다.(Transaction발생)
        /// </summary>
        public bool Modify_TR(uint nKey, DBDataSAMP data)
        { return Modify_BaseTR(nKey, data); }
        /// <summary>
        /// DB에 dataList를 수정합니다.(Transaction발생)
        /// </summary> 
        public bool Modify_TR(HmKeyList keyList, List<DBDataSAMP> dataList)
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
            DBDataSAMP currData = (DBDataSAMP)data;
            //
            return true;
        }
        /// <summary>
        /// 해당 data의 자료를 단위별로 UnitFactor값을 곱하여 넘겨줍니다.
        /// </summary>
        public override void Convert_Unit(DBBaseData data, Dictionary<UNIT_TYPE, double> dicUnitFactor)
        {
            DBDataSAMP currData = (DBDataSAMP)data;
            // 단위계작업이 필요합니다.
        }
        /// <summary>
        /// 해당 data의 자료 변환시 연관관계 처리를 수행합니다.
        /// </summary> 
        protected override bool Association(uint nKey, DBBaseData data, DBBaseData data_Org, TRANSACTION_DATA type)
        {
            DBDataSAMP currData = type == TRANSACTION_DATA.DEL ? null : (DBDataSAMP)data;
            DBDoc doc = DBDoc.Get_CurrDoc();
            if (type == TRANSACTION_DATA.ADD)
            { /*Data 추가시 연관관계 처리를 합니다.(일반적으로 Add시에 연관처리가 필요한 경우는 없습니다.)*/ }
            else if (type == TRANSACTION_DATA.MODIFY)
            { /*Data 수정시 연관관계 처리를 합니다.*/ }
            else if (type == TRANSACTION_DATA.DEL)
            { /*Data 삭제시 연관관계 처리를 합니다.(Del시에는 data값이 Null로 넘어오므로 주의를 요합니다.)*/ }
            return true;
        }

        // User Binary 방식 저장시 필수 재지정 함수 ////////////////////////////////////////////////////////////////////
        public override DBBaseData New_Data()
        {
            return new DBDataSAMP();
        }

        /// <summary>
        /// 해당수치주상도의 키를 가지는 모든 시료현태의 키리스트를 넘겨줍니다.
        /// </summary>
        /// <param name="keyList"></param>
        /// <param name="drlgKey"></param>
        /// <param name="bSort"></param>
        /// <returns></returns>
        public int Get_KeyList(HmKeyList keyList, uint drlgKey, bool bSort = true, List<double> depthList = null)
        {
            keyList.Clear();
            DBDoc doc = DBDoc.Get_CurrDoc();
            HmKeyList allKeyList = new HmKeyList();
            if (Get_KeyList(allKeyList) == 0) return 0;

            if (depthList == null) depthList = new List<double>();
            else depthList.Clear();

            for (int i = allKeyList.Count - 1; i >= 0; i--)
            {
                DBDataSAMP itemD = null;
                Get_Data(allKeyList.list[i], ref itemD, false);
                if (itemD.drlgKey == drlgKey)
                { keyList.Add(allKeyList.list[i]); depthList.Add(itemD.Depth); }
            }

            // depth순으로 정렬합니다.
            if (bSort)
            {
                for (int i = 0; i < keyList.Count - 1; i++)
                {
                    for (int n = i + 1; n < keyList.Count; n++)
                    {
                        if (depthList[i] > depthList[n])
                        {
                            MathUtil.Swap(keyList.list, i, n);
                            MathUtil.Swap(depthList, i, n);
                        }
                    }
                }
            }

            return keyList.Count;
        }
        public int Get_KeyList_STRA(HmKeyList keyList, uint straKey, bool bSort = true, List<double> depthList = null)
        {
            keyList.Clear();
            if (depthList == null) depthList = new List<double>();
            else depthList.Clear();

            DBDoc doc = (DBDoc)m_Doc;
            double dTop = 0.0, dBot = 0.0;
            if (!doc.stra.Get_Depth(straKey, ref dTop, ref dBot)) return 0;
            uint drlgKey = doc.stra.Get_DrlgKey(straKey);

            HmKeyList allKeyList = new HmKeyList();
            List<double> allDepthList = new List<double>();
            if (0 == Get_KeyList(allKeyList, drlgKey, true, allDepthList)) return 0;

            if (dTop >= dBot) dBot = dTop + Constants.Tolerance_Length;

            for (int i = 0; i < allKeyList.Count; i++)
            {
                if (dTop <= allDepthList[i] && dBot > allDepthList[i])
                { keyList.Add(allKeyList[i]); depthList.Add(allDepthList[i]); }
            }
            return keyList.Count;
        }
        public int Get_KeyList_DESC(HmKeyList keyList, uint descKey, bool bSort = true, List<double> depthList = null)
        {
            keyList.Clear();
            if (depthList == null) depthList = new List<double>();
            else depthList.Clear();

            DBDoc doc = (DBDoc)m_Doc;
            DBDataDESC descData = null;
            if (!doc.desc.Get_Data(descKey, ref descData)) return 0;

            double dTop = 0.0, dBot = 0.0;
            if (!descData.Get_Depth(ref dTop, ref dBot)) return 0;


            uint drlgKey = doc.desc.Get_DrlgKey(descKey);
            HmKeyList allKeyList = new HmKeyList();
            List<double> allDepthList = new List<double>();
            if (0 == Get_KeyList(allKeyList, drlgKey, true, allDepthList)) return 0;

            if (dTop >= dBot) dBot = dTop + Constants.Tolerance_Length;

            for (int i = 0; i < allKeyList.Count; i++)
            {
                if (dTop <= allDepthList[i] && dBot > allDepthList[i])
                { keyList.Add(allKeyList[i]); depthList.Add(allDepthList[i]); }
            }
            return keyList.Count;
        }
        public uint Get_StraKey(uint sampKey)
        {
            DBDataSAMP itemD = null;
            if (!Get_Data(sampKey, ref itemD)) return 0;

            HmKeyList straKList = new HmKeyList();
            List<double> depthList = new List<double>(); // 지층 하부 깊이

            if(0==((DBDoc)m_Doc).stra.Get_KeyList(straKList, itemD.drlgKey, true, depthList)) return 0;
            for(int i=0; i< depthList.Count; i++)
            { if (itemD.Depth > depthList[i]) return straKList[i]; }

            return straKList.list[straKList.Count - 1];
        }
        public uint Get_DescKey(uint sampKey)
        {
            uint nStraK = Get_StraKey(sampKey);
            if (nStraK == 0) return 0;

            DBDataSAMP itemD = null;
            if (!Get_Data(sampKey, ref itemD)) return 0;

            HmKeyList descKList = new HmKeyList();
            List<double> depthList = new List<double>(); // 지층 상부 깊이

            if (0 == ((DBDoc)m_Doc).desc.Get_KeyList(descKList, nStraK, true, depthList)) return 0;
            for (int i = depthList.Count-1; i >= 0 ; i--)
            { if (itemD.Depth <= depthList[i]) return descKList[i]; }

            return descKList.list[0];
        }

        #endregion
    }
}
