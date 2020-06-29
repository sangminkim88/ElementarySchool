namespace GaiaDB
{
    using GaiaDB.Enums;
    using HmDataDocument;
    using HmGeometry;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class DBDataSTRA : DBBaseData
    {
        /// <summary>
        /// 해당 시추주상도 데이터의 키
        /// </summary>
        public uint drlgKey;

        /// <summary>
        /// 심도
        /// </summary>
        public double Depth;

        /// <summary>
        /// 주상도 / 통일분류
        /// </summary>
        public eSoil soilType;

        /// <summary>
        /// 색조
        /// </summary>
        public List<int> ColorList;

        /// <summary>
        /// 매립층, 전답토, 붕적층, 퇴적층, 충적층, 풍화토, 풍화암, 연암, 경암
        /// </summary>
        public string stratumName;

        /// <summary>
        /// 비교
        /// </summary>
        public string strNote;

        public DBDataSTRA()
        {
            ColorList = new List<int>();
            Initialize();
        }

        ~DBDataSTRA()
        {
            Initialize();
        }

        public void Initialize()
        {
            drlgKey = 0;
            Depth = 0.0;
            soilType = eSoil.None;
            ColorList.Clear();
            stratumName = "";
            strNote = "";
        }

        public DBDataSTRA Clone()
        {
            DBDataSTRA data = new DBDataSTRA();
            data.drlgKey = drlgKey;
            data.Depth = Depth;
            data.soilType = soilType;
            foreach (var item in ColorList)
            {
                data.ColorList.Add(item);
            }
            data.stratumName = stratumName;
            data.strNote = strNote;
            return data;
        }

        public override DBBaseData BaseClone()
        {
            return Clone();
        }

        public override bool Write_Binary(BinaryWriter binaryRW)
        {
            binaryRW.Write(drlgKey);
            binaryRW.Write(Depth);
            binaryRW.Write(soilType.ToString());
            binaryRW.Write(ColorList.Count);
            foreach (var item in ColorList)
            { binaryRW.Write(item); }
            binaryRW.Write(stratumName);
            binaryRW.Write(strNote);
            return true;
        }

        public override bool Read_Binary(BinaryReader binaryRW, int nReadHMVer, int nReadVer)
        {
            Initialize();
            drlgKey = binaryRW.ReadUInt32();
            Depth = binaryRW.ReadDouble();
            string type = binaryRW.ReadString();
            if (type.Equals(string.Empty))
            {
                soilType = eSoil.None;
            }
            else
            {
                soilType = (eSoil)Enum.Parse(typeof(eSoil), type);
            }
            int i, isize = binaryRW.ReadInt32();
            for (i = 0; i < isize; i++)
            { ColorList.Add(binaryRW.ReadInt32()); }
            if (nReadVer < 103) return true;
            stratumName = binaryRW.ReadString();
            strNote = binaryRW.ReadString();
            return true;
        }
    }

    public class DBClassSTRA : DBBaseClass
    {
        public DBClassSTRA(HmBaseDoc doc)
        {
            m_strDBKey = "STRA";
            m_strDBNote = "지층 레벨";
            m_bSingleData = false; // 복수 자료구조
            Set_Doc(doc);
        }

        ~DBClassSTRA()
        {

        }

        // 복수 자료구조인경우 ////////////////////////////////////////////////////////////////////////////////////////////
        // 아래 함수형은 복수자료형방식으로 타 복수자료방식과 동일합니다.(Get_Data ~ Del_TR)

        /// <summary>
        /// DataPool에서 해당 Key의 Data를 가져옵니다.
        /// </summary>
        public bool Get_Data(uint nKey, ref DBDataSTRA data, bool bClone = true)
        { // Clone이 아닌경우 자료를 가져와 수정시 Transaction을 통한 관리가 되지 않습니다. 또한 단위계는 내부 공용단위계 값으로 넘겨줍니다. 
          // 따라서 bClone을 false로 쓸때에는 수정하지 않고 공용단위계로 쓸경우에만(View화면갱신 등) 사용하도록 합니다.
            DBBaseData baseData = null;
            if (!Get_BaseData(nKey, ref baseData, bClone)) return false;
            data = (DBDataSTRA)baseData;
            return true;
        }

        /// <summary>
        /// DataPool에서 해당 KeyList의 DataList를 가져옵니다.
        /// </summary>
        public bool Get_DataList(HmKeyList keyList, ref List<DBDataSTRA> dataList, bool bClone = true)
        {
            DBDataSTRA data = new DBDataSTRA();
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
        public uint Add_TR(DBDataSTRA data)
        { return Add_BaseTR(data); }
        /// <summary>
        /// DB에 dataList를 추가합니다.(Transaction발생)
        /// </summary>
        public bool Add_TR(List<DBDataSTRA> dataList)
        {
            List<DBBaseData> baseList = new List<DBBaseData>();
            for (int i = 0; i < dataList.Count; i++) baseList.Add(dataList[i]);
            return Add_BaseTR(baseList);
        }
        /// <summary>
        /// DB에 data를 수정합니다.(Transaction발생)
        /// </summary>
        public bool Modify_TR(uint nKey, DBDataSTRA data)
        { return Modify_BaseTR(nKey, data); }
        /// <summary>
        /// DB에 dataList를 수정합니다.(Transaction발생)
        /// </summary> 
        public bool Modify_TR(HmKeyList keyList, List<DBDataSTRA> dataList)
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
            DBDataSTRA currData = (DBDataSTRA)data;
            //
            return true;
        }
        /// <summary>
        /// 해당 data의 자료를 단위별로 UnitFactor값을 곱하여 넘겨줍니다.
        /// </summary>
        public override void Convert_Unit(DBBaseData data, Dictionary<UNIT_TYPE, double> dicUnitFactor)
        {
            DBDataSTRA currData = (DBDataSTRA)data;
            // 단위계작업이 필요합니다.
        }
        /// <summary>
        /// 해당 data의 자료 변환시 연관관계 처리를 수행합니다.
        /// </summary> 
        protected override bool Association(uint nKey, DBBaseData data, DBBaseData data_Org, TRANSACTION_DATA type)
        {
            DBDataSTRA currData = type == TRANSACTION_DATA.DEL ? null : (DBDataSTRA)data;
            DBDoc doc = DBDoc.Get_CurrDoc();
            if (type == TRANSACTION_DATA.ADD)
            { /*Data 추가시 연관관계 처리를 합니다.(일반적으로 Add시에 연관처리가 필요한 경우는 없습니다.)*/ }
            else if (type == TRANSACTION_DATA.MODIFY)
            { /*Data 수정시 연관관계 처리를 합니다.*/ }
            else if (type == TRANSACTION_DATA.DEL)
            { /*Data 삭제시 연관관계 처리를 합니다.(Del시에는 data값이 Null로 넘어오므로 주의를 요합니다.)*/

                HmKeyList descKeyList = new HmKeyList();
                doc.desc.Get_KeyList(descKeyList, nKey);
                if (!doc.desc.Del_Data(descKeyList)) return false;

                HmKeyList jshpKeyList = new HmKeyList();
                doc.jshp.Get_KeyList(jshpKeyList, nKey);
                if (!doc.jshp.Del_Data(jshpKeyList)) return false;
            }
            return true;
        }

        // User Binary 방식 저장시 필수 재지정 함수 ////////////////////////////////////////////////////////////////////
        public override DBBaseData New_Data()
        {
            return new DBDataSTRA();
        }

        /// <summary>
        /// 해당수치주상도의 키를 가지는 모든 지층의 키리스트를 넘겨줍니다.
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
                DBDataSTRA itemD = null;
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

        public uint Get_DrlgKey(uint straKey)
        {
            DBDataSTRA straD = null;
            if (!Get_Data(straKey, ref straD, false)) return 0;
            return straD.drlgKey;
        }
        
        public bool Get_Depth(uint straKey, ref double dTop, ref double dBottom)
        {
            HmKeyList allKeyList = new HmKeyList();
            List<double> depthList = new List<double>();
            if (0 == Get_KeyList(allKeyList, Get_DrlgKey(straKey), true, depthList)) return false;

            for (int i=0; i< allKeyList.Count; i++)
            {
                if(allKeyList[i] == straKey)
                {
                    dTop = i == 0 ? 0.0 : depthList[i - 1];
                    dBottom = depthList[i];
                    return true;
                }
            }
            return false;
        }
        public bool Get_LastDepth(List<uint> straKey, ref double dLast)
        {
            HmKeyList allKeyList = new HmKeyList();
            List<double> depthList = new List<double>();
            if(straKey.Count > 0)
            {
                if (0 == Get_KeyList(allKeyList, Get_DrlgKey(straKey[0]), true, depthList)) return false;

                if (depthList.Count > 0)
                {
                    dLast = depthList[depthList.Count - 1];
                    return true;
                }
            }
            return false;
        }

        public string ToStringBuilder(uint straKey)
        {
            DBDataSTRA straD = null;
            if (!Get_Data(straKey, ref straD)) return "";

            bool bSoil = DescriptionUtil.IsSoil(straD.stratumName);
            double dTop = 0.0, dBot = 0.0;
            Get_Depth(straKey, ref dTop, ref dBot);

            string strBuilder = string.Format("[{0}] 심도 {1} ~ {2}m", straD.stratumName, dTop, straD.Depth);

            if (bSoil)
            {
                strBuilder += ("\n" + DescriptionUtil.SoilNote(straD.soilType));
            }

            if (straD.ColorList.Count > 0)
            {
                if (straD.ColorList.Count == 1) strBuilder += ("\n" + ColorUtil.Get_SoilColorNameByID(straD.ColorList[0]));
                else strBuilder += ("\n" + ColorUtil.Get_SoilColorNameByID(straD.ColorList[0]) + "~" + ColorUtil.Get_SoilColorNameByID(straD.ColorList[straD.ColorList.Count - 1]));
            }

            DBDoc doc = (DBDoc)m_Doc;
            HmKeyList descKList = new HmKeyList();
            if (doc.desc.Get_KeyList(descKList, straKey) > 1)
            {
                // 복수인 경우 하위 desc에 중요항목만 모아서 요약정보를 표시합니다.
                List<string> rockList = new List<string>();

                DBDataDESC descD = null;
                for (int i = 0; i < descKList.Count; i++)
                {
                    if (!doc.desc.Get_Data(descKList[i], ref descD)) continue;

                    if (bSoil)
                    {

                    }
                    else
                    {
                        if (descD.dicDesc.ContainsKey(eDescriptionKey.RockType))
                        { if (!rockList.Contains(descD.dicDesc[eDescriptionKey.RockType])) rockList.Add(descD.dicDesc[eDescriptionKey.RockType]); }
                    }
                }

                if (rockList.Count > 0) strBuilder += ("\n" + TextUtil.Get_String(rockList));
            }


            return strBuilder;
        }
    }
}
