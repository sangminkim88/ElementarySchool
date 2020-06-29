namespace GaiaDB
{
    using HmDataDocument;
    using HmGeometry;
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// 표준관입시험 (Standard Penetration Test)
    /// </summary>
    public class DBDataSPTG : DBBaseData
    {
        // 심도/타격횟수/관입량(cm)
        public List<Tuple<double, int, double>> SptList;

        public DBDataSPTG()
        {
            SptList = new List<Tuple<double, int, double>>();
            Initialize();
        }

        ~DBDataSPTG()
        {
            Initialize();
        }

        public void Initialize()
        {
            SptList.Clear();
        }

        public DBDataSPTG Clone()
        {
            DBDataSPTG data = new DBDataSPTG();
            foreach (Tuple<double, int, double> item in SptList)
            { data.SptList.Add(new Tuple<double, int, double>(item.Item1, item.Item2, item.Item3)); }
            return data;
        }

        public override DBBaseData BaseClone()
        { return Clone(); }

        public override bool Write_Binary(BinaryWriter binaryRW)
        {
            binaryRW.Write(SptList.Count);
            foreach (Tuple<double, int, double> item in SptList)
            {
                binaryRW.Write(item.Item1);
                binaryRW.Write(item.Item2);
                binaryRW.Write(item.Item3);
            }
            return true;
        }

        public override bool Read_Binary(BinaryReader binaryRW, int nReadHMVer, int nReadVer)
        {
            Initialize();
            int isize = binaryRW.ReadInt32();
            for (int i = 0; i < isize; i++)
            {
                double item1 = binaryRW.ReadDouble();
                int item2 = binaryRW.ReadInt32();
                double item3 = binaryRW.ReadDouble();
                SptList.Add(new Tuple<double, int, double>(item1, item2, item3));
            }
            return true;
        }
    }

    public class DBClassSPTG : DBBaseClass
    {
        #region Constructors

        public DBClassSPTG(HmBaseDoc doc)
        {
            m_strDBKey = "SPTG";
            m_strDBNote = "표준관입시험";
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
        public bool Get_Data(uint nKey, ref DBDataSPTG data, bool bClone = true)
        { // Clone이 아닌경우 자료를 가져와 수정시 Transaction을 통한 관리가 되지 않습니다. 또한 단위계는 내부 공용단위계 값으로 넘겨줍니다. 
          // 따라서 bClone을 false로 쓸때에는 수정하지 않고 공용단위계로 쓸경우에만(View화면갱신 등) 사용하도록 합니다.
            DBBaseData baseData = null;
            if (!Get_BaseData(nKey, ref baseData, bClone)) return false;
            data = (DBDataSPTG)baseData;
            return true;
        }

        /// <summary>
        /// DataPool에서 해당 KeyList의 DataList를 가져옵니다.
        /// </summary>
        public bool Get_DataList(HmKeyList keyList, ref List<DBDataSPTG> dataList, bool bClone = true)
        {
            DBDataSPTG data = new DBDataSPTG();
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
        public uint Add_TR(DBDataSPTG data)
        { return Add_BaseTR(data); }
        /// <summary>
        /// DB에 dataList를 추가합니다.(Transaction발생)
        /// </summary>
        public bool Add_TR(List<DBDataSPTG> dataList)
        {
            List<DBBaseData> baseList = new List<DBBaseData>();
            for (int i = 0; i < dataList.Count; i++) baseList.Add(dataList[i]);
            return Add_BaseTR(baseList);
        }
        /// <summary>
        /// DB에 data를 수정합니다.(Transaction발생)
        /// </summary>
        public bool Modify_TR(uint nKey, DBDataSPTG data)
        { return Modify_BaseTR(nKey, data); }
        /// <summary>
        /// DB에 dataList를 수정합니다.(Transaction발생)
        /// </summary> 
        public bool Modify_TR(HmKeyList keyList, List<DBDataSPTG> dataList)
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
            DBDataSPTG currData = (DBDataSPTG)data;
            //
            return true;
        }
        /// <summary>
        /// 해당 data의 자료를 단위별로 UnitFactor값을 곱하여 넘겨줍니다.
        /// </summary>
        public override void Convert_Unit(DBBaseData data, Dictionary<UNIT_TYPE, double> dicUnitFactor)
        {
            DBDataSPTG currData = (DBDataSPTG)data;
            // 단위계작업이 필요합니다.
        }
        /// <summary>
        /// 해당 data의 자료 변환시 연관관계 처리를 수행합니다.
        /// </summary> 
        protected override bool Association(uint nKey, DBBaseData data, DBBaseData data_Org, TRANSACTION_DATA type)
        {
            DBDataSPTG currData = type == TRANSACTION_DATA.DEL ? null : (DBDataSPTG)data;
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
            return new DBDataSPTG();
        }

        #endregion
    }
}
