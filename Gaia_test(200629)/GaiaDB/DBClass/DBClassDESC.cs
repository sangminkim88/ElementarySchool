namespace GaiaDB
{
    using GaiaDB.Enums;
    using HmDataDocument;
    using HmGeometry;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Defines the <see cref="DBDataDESC" />.
    /// </summary>
    public class DBDataDESC : DBBaseData
    {
        /// <summary>
        /// 지층 설명 데이터.
        /// </summary>
        public Dictionary<eDescriptionKey, string> dicDesc;

        /// <summary>
        /// 해당 지층 데이터의 키.
        /// </summary>
        public uint straKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="DBDataDESC"/> class.
        /// </summary>
        public DBDataDESC()
        {
            dicDesc = new Dictionary<eDescriptionKey, string>();
            Initialize();
        }

        ~DBDataDESC()
        {
            Initialize();
        }

        /// <summary>
        /// The Initialize.
        /// </summary>
        public void Initialize()
        {
            straKey = 0;
            dicDesc.Clear();
        }

        /// <summary>
        /// The Clone.
        /// </summary>
        /// <returns>The <see cref="DBBaseData"/>.</returns>
        public DBBaseData Clone()
        {
            DBDataDESC data = new DBDataDESC();
            data.straKey = straKey;
            foreach (KeyValuePair<eDescriptionKey, string> item in dicDesc)
            { data.dicDesc.Add(item.Key, item.Value); }
            return data;
        }

        /// <summary>
        /// The BaseClone.
        /// </summary>
        /// <returns>The <see cref="DBBaseData"/>.</returns>
        public override DBBaseData BaseClone()
        {
            return Clone();
        }

        /// <summary>
        /// The Write_Binary.
        /// </summary>
        /// <param name="binaryRW">The binaryRW<see cref="BinaryWriter"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public override bool Write_Binary(BinaryWriter binaryRW)
        {
            binaryRW.Write(straKey);
            binaryRW.Write(dicDesc.Count);
            foreach (KeyValuePair<eDescriptionKey, string> item in dicDesc)
            {
                binaryRW.Write((int)item.Key);
                binaryRW.Write(item.Value);
            }

            return true;
        }

        /// <summary>
        /// The Read_Binary.
        /// </summary>
        /// <param name="binaryRW">The binaryRW<see cref="BinaryReader"/>.</param>
        /// <param name="nReadHMVer">The nReadHMVer<see cref="int"/>.</param>
        /// <param name="nReadVer">The nReadVer<see cref="int"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public override bool Read_Binary(BinaryReader binaryRW, int nReadHMVer, int nReadVer)
        {
            int i, isize;
            Initialize();

            straKey = binaryRW.ReadUInt32();
            isize = binaryRW.ReadInt32();
            for (i = 0; i < isize; i++)
            {
                eDescriptionKey key = (eDescriptionKey)binaryRW.ReadInt32();
                string value = binaryRW.ReadString();
                dicDesc.Add((eDescriptionKey)key, value);
            }
            return true;
        }



        /// <summary>
        /// The ToString.
        /// </summary>
        /// <param name="nType">0=stra에 desc이 한개만 존재하는경우, 1=복수경우에 첫번째인경우, 2=복수경우에 두번째이후인경우</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string ToStringBuilder(int nType)
        {
            StringBuilder returnSb = new StringBuilder();

            bool? isSand = null;

            DBDoc doc = DBDoc.Get_CurrDoc();

            //토사? 암? 확인
            DBDataSTRA straD = null;
            if (doc.stra.Get_Data(this.straKey, ref straD))
            {
                if (straD.stratumName.Equals("연암") || straD.stratumName.Equals("경암"))
                {
                    isSand = false;
                }
                else
                {
                    isSand = true;
                }
            }

            string strTitle = "";
            if(nType == 2) // stra와 desc가 1:n 경우에, desc에서 심도와 두깨 값을 추출하고 넘겨줍니다. (암반일때만 stra와 desc가 1:n 있을 수 있으며 desc의 depth과 thick를 시료현태 입력시에 정한다.)
            {
                strTitle = "*심도 XXX ~ XXX";
            }
            else // stra와 desc가 1:1 경우에, stra에서 심도와 두깨 값을 추출하고 넘겨줍니다. (토사일때 stra와 desc가 1:1, 암반일때 1:1 또는 1:n)
            {
                strTitle = doc.stra.ToStringBuilder(straKey);
            }

            return strTitle + "\r\n" +ToStringBuilder(dicDesc, nType, isSand);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicDesc"></param>
        /// <param name="nType"></param>
        /// <param name="isSand"></param>
        /// <returns></returns>
        public static string ToStringBuilder(Dictionary<eDescriptionKey, string> dicDesc, int nType, bool? isSand)
        {
            StringBuilder returnSb = new StringBuilder();

            foreach (var item in dicDesc.OrderBy(x => x.Key))
            {
                if (item.Value.Equals(string.Empty)) continue;
                switch (item.Key)
                {
                    //case eDescriptionKey.Stratum:
                    //    {
                    //        if (item.Value != null)
                    //        {
                    //            string data = item.Value;
                    //            returnSb.AppendLine("[" + data + "]");
                    //            if (data.Equals("연암") || data.Equals("경암"))
                    //            {
                    //                isSand = false;
                    //            }
                    //            else
                    //            {
                    //                isSand = true;
                    //            }
                    //        }
                    //        break;
                    //    }
                    case eDescriptionKey.Depth:
                        {
                            if (item.Value != null)
                            {
                                double dTop = 0.0, dBottom = 0.0;
                                Get_Depth(item.Value, ref dTop, ref dBottom);
                                returnSb.AppendLine("심도 " + dTop + " ~ " + dBottom);
                                //상민 : 심도 데이터가 천지인으로 도출한 샘플인 이유로 포맷이 다름
                                //var tmp = item.Value.Split('/').Cast<double>();
                                //returnSb.AppendLine("심도 " + tmp.First() + " ~ " + tmp.Last());
                            }
                            break;
                        }
                    case eDescriptionKey.SoilColumn:
                        {
                            if (isSand != true) break;
                            if (item.Value != null) returnSb.AppendLine("-" + item.Value);
                            break;
                        }
                    case eDescriptionKey.Color:
                        {
                            if (item.Value != null)
                            {
                                //상민 : 색상 데이터가 천지인으로 도출한 샘플인 이유로 포맷이 다름
                                //List<Tuple<string, Brush>> tmp = ColorUtil.GetGaiaColorDataByID(item.Value.Split('/').Cast<int>().ToList());
                                //string tmpString = string.Empty;
                                //for (int i = 0; i < tmp.Count; i++)
                                //{
                                //    if (i.Equals(1))
                                //    {
                                //        tmpString += "~";
                                //    }
                                //    tmpString += tmp[i];
                                //}
                                //returnSb.AppendLine("-" + tmpString);
                            }
                            break;
                        }
                    case eDescriptionKey.RockType:
                        {
                            if (isSand != false) break;
                            if (item.Value != null) returnSb.AppendLine("-" + item.Value);
                            break;
                        }
                    case eDescriptionKey.Joint:
                        {
                            if (isSand != false) break;
                            if (item.Value != null)
                            {
                                var tmp1 = item.Value.Split(',');
                                var tmp2 = tmp1[0].Split('/');
                                string tmpString = string.Empty;
                                for (int i = 0; i < tmp2.Count(); i++)
                                {
                                    if (!string.IsNullOrEmpty(tmp2[i]))
                                    {
                                        if (!i.Equals(0))
                                        {
                                            if ((i % 2).Equals(1))
                                            {
                                                tmpString += "~";
                                            }
                                        }
                                        tmpString += tmp2.ElementAt(i);
                                    }
                                    if (!i.Equals(0) && (i % 2).Equals(1))
                                    {
                                        tmpString += ", ";
                                    }
                                }
                                while (true)
                                {
                                    if (tmpString.Length > 1 && tmpString.Substring(tmpString.Length - 2).Equals(", "))
                                    {
                                        tmpString = tmpString.Substring(0, tmpString.Length - 2);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                int jointCount = tmpString.Count(x => x.Equals(',')) > 0 ? tmpString.Count(x => x.Equals(',')) + 1 : 0;
                                if (tmp1[1].Equals("True"))
                                {
                                    returnSb.AppendLine("-절리군 : " + jointCount + "절리군 + 부분절리");
                                }
                                else
                                {
                                    returnSb.AppendLine("-절리군 : " + jointCount + "절리군");
                                }
                                if (!string.IsNullOrEmpty(tmpString))
                                {
                                    returnSb.AppendLine("-절리각 : " + tmpString);
                                }
                            }
                            break;
                        }
                    case eDescriptionKey.Rough:
                        {
                            if (isSand != false) break;
                            if (item.Value != null)
                            {
                                var tmp1 = item.Value.Split(',');
                                string tmpType = string.Empty;
                                string tmpString = string.Empty;
                                for (int i = 0; i < tmp1.Length; i++)
                                {
                                    var tmp2 = tmp1[i].Split('/');
                                    if (i.Equals(1))
                                    {
                                        tmpString += "~";
                                    }
                                    tmpType = tmp2[0];
                                    tmpString += tmp2[1];
                                }

                                //var tmp2 = tmp[1].Split('/');
                                returnSb.AppendLine("-절리면 : " + tmpType + "/" + tmpString);
                            }
                            break;
                        }
                    case eDescriptionKey.Note:
                        {
                            if (item.Value != null) returnSb.AppendLine("-" + item.Value);
                            break;
                        }
                    case eDescriptionKey.Wet:
                    case eDescriptionKey.Density:
                        {
                            if (isSand != true) break;
                            if (item.Value != null)
                            {
                                string tmpString = getData(item);
                                returnSb.AppendLine("-" + tmpString);
                            }
                            break;
                        }
                    case eDescriptionKey.Weathered:
                    case eDescriptionKey.Stiffness:
                    case eDescriptionKey.Gap:
                        {
                            if (isSand != false) break;
                            if (item.Value != null)
                            {
                                string tmpString = getData(item);
                                returnSb.AppendLine("-" + tmpString);
                            }
                            break;
                        }
                }
            }

            return returnSb.ToString();
        }        

        /// <summary>
        /// The getData.
        /// </summary>
        /// <param name="item">The item<see cref="KeyValuePair{eDescriptionKey, string}"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private static string getData(KeyValuePair<eDescriptionKey, string> item)
        {
            var tmp = item.Value.Split('/');
            string tmpString = string.Empty;
            for (int i = 0; i < tmp.Count(); i++)
            {
                if (i.Equals(1))
                {
                    tmpString += "~";
                }
                tmpString += tmp.ElementAt(i);
            }

            return tmpString;
        }

        public static bool Get_Depth(string strDepth, ref double dTop, ref double dBottom)
        {
            if (strDepth == null) return false;
            List<double> valueList = new List<double>();
            if (0 == TextUtil.Get_ValueList(strDepth, ref valueList, -1)) return false;
            valueList.Sort();
            dTop = valueList.First(); dBottom = valueList.Last();
            return true;
        }
        public bool Get_Depth(ref double dTop, ref double dBottom)
        {
            if (!dicDesc.ContainsKey(eDescriptionKey.Depth)) return false;
            string strValue = dicDesc[eDescriptionKey.Depth];
            if (strValue == null) return false;
            return Get_Depth(strValue, ref dTop, ref dBottom);
        }
    }

    /// <summary>
    /// Defines the <see cref="DBClassDESC" />.
    /// </summary>
    public class DBClassDESC : DBBaseClass
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DBClassDESC"/> class.
        /// </summary>
        /// <param name="doc">The doc<see cref="HmBaseDoc"/>.</param>
        public DBClassDESC(HmBaseDoc doc)
        {
            m_strDBKey = "DESC";
            m_strDBNote = "지층설명";
            m_bSingleData = false;
            Set_Doc(doc);
        }

        #endregion

        #region Methods

        /// <summary>
        /// DB에 data를 추가합니다.(Transaction발생).
        /// </summary>
        /// <param name="data">추가할 Data.</param>
        /// <returns>The <see cref="uint"/>.</returns>
        public uint Add_TR(DBDataDESC data)
        {
            return Add_BaseTR(data);
        }

        /// <summary>
        /// DB에 dataList를 추가합니다.(Transaction발생).
        /// </summary>
        /// <param name="dataList">The dataList<see cref="List{DBDataDESC}"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Add_TR(List<DBDataDESC> dataList)
        {
            List<DBBaseData> baseList = new List<DBBaseData>();
            for (int i = 0; i < dataList.Count; i++) baseList.Add(dataList[i]);
            return Add_BaseTR(baseList);
        }

        /// <summary>
        /// 해당 data의 자료의 오류여부를 검토합니다. 오류 발생시에 오류 내용을 strMsg에 채워서 넘겨줍니다.
        /// </summary>
        /// <param name="nKey">The nKey<see cref="uint"/>.</param>
        /// <param name="data">The data<see cref="DBBaseData"/>.</param>
        /// <param name="strMsg">The strMsg<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public override bool Check(uint nKey, DBBaseData data, ref string strMsg)
        {
            DBDataDESC currData = (DBDataDESC)data;
            //
            return true;
        }

        /// <summary>
        /// 해당 data의 자료를 단위별로 UnitFactor값을 곱하여 넘겨줍니다.
        /// </summary>
        /// <param name="data">The data<see cref="DBBaseData"/>.</param>
        /// <param name="dicUnitFactor">The dicUnitFactor<see cref="Dictionary{UNIT_TYPE, double}"/>.</param>
        public override void Convert_Unit(DBBaseData data, Dictionary<UNIT_TYPE, double> dicUnitFactor)
        {
            DBDataDESC currData = (DBDataDESC)data;
        }

        /// <summary>
        /// DataPool에서 해당 Key의 Data를 가져옵니다.
        /// </summary>
        /// <param name="nKey">The nKey<see cref="uint"/>.</param>
        /// <param name="data">The data<see cref="DBDataDESC"/>.</param>
        /// <param name="bClone">The bClone<see cref="bool"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Get_Data(uint nKey, ref DBDataDESC data, bool bClone = true)
        {
            // 따라서 bClone을 false로 쓸때에는 수정하지 않고 공용단위계로 쓸경우에만(View화면갱신 등) 사용하도록 합니다.
            DBBaseData baseData = null;
            if (!Get_BaseData(nKey, ref baseData, bClone)) return false;
            data = (DBDataDESC)baseData;
            return true;
        }

        /// <summary>
        /// DataPool에서 해당 KeyList의 DataList를 가져옵니다.
        /// </summary>
        /// <param name="keyList">The keyList<see cref="HmKeyList"/>.</param>
        /// <param name="dataList">The dataList<see cref="List{DBDataDESC}"/>.</param>
        /// <param name="bClone">The bClone<see cref="bool"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Get_DataList(HmKeyList keyList, ref List<DBDataDESC> dataList, bool bClone = true)
        {
            DBDataDESC data = new DBDataDESC();
            for (int i = 0; i < keyList.list.Count; i++)
            {
                if (!Get_Data(keyList.list[i], ref data, bClone))
                { dataList.Clear(); return false; }
                dataList.Add(data);
            }
            return true;
        }
        
        /// <summary>
        /// DB에 dataList를 수정합니다.(Transaction발생).
        /// </summary>
        /// <param name="keyList">The keyList<see cref="HmKeyList"/>.</param>
        /// <param name="dataList">The dataList<see cref="List{DBDataDESC}"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Modify_TR(HmKeyList keyList, List<DBDataDESC> dataList)
        {
            List<DBBaseData> baseList = new List<DBBaseData>();
            for (int i = 0; i < dataList.Count; i++) baseList.Add(dataList[i]);
            return Modify_BaseTR(keyList, baseList);
        }

        /// <summary>
        /// DB에 data를 수정합니다.(Transaction발생).
        /// </summary>
        /// <param name="nKey">The nKey<see cref="uint"/>.</param>
        /// <param name="data">The data<see cref="DBDataDESC"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Modify_TR(uint nKey, DBDataDESC data)
        {
            return Modify_BaseTR(nKey, data);
        }

        /// <summary>
        /// The New_Data.
        /// </summary>
        /// <returns>The <see cref="DBBaseData"/>.</returns>
        public override DBBaseData New_Data()
        {
            return new DBDataDESC();
        }

        /// <summary>
        /// 해당 data의 자료 변환시 연관관계 처리를 수행합니다.
        /// </summary>
        /// <param name="nKey">The nKey<see cref="uint"/>.</param>
        /// <param name="data">The data<see cref="DBBaseData"/>.</param>
        /// <param name="data_Org">The data_Org<see cref="DBBaseData"/>.</param>
        /// <param name="type">The type<see cref="TRANSACTION_DATA"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        protected override bool Association(uint nKey, DBBaseData data, DBBaseData data_Org, TRANSACTION_DATA type)
        {
            DBDataDESC currData = type == TRANSACTION_DATA.DEL ? null : (DBDataDESC)data;
            DBDoc doc = DBDoc.Get_CurrDoc();
            if (type == TRANSACTION_DATA.ADD)
            { /*Data 추가시 연관관계 처리를 합니다.(일반적으로 Add시에 연관처리가 필요한 경우는 없습니다.)*/ }
            else if (type == TRANSACTION_DATA.MODIFY)
            { /*Data 수정시 연관관계 처리를 합니다.*/ }
            else if (type == TRANSACTION_DATA.DEL)
            { /*Data 삭제시 연관관계 처리를 합니다.(Del시에는 data값이 Null로 넘어오므로 주의를 요합니다.)*/ }
            return true;
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// 해당지층의 키를 가지는 모든 지층설명의 키리스트를 넘겨줍니다.
        /// </summary>
        /// <param name="keyList">.</param>
        /// <param name="straKey">.</param>
        /// <param name="bSort">.</param>
        /// <returns>.</returns>
        public int Get_KeyList(HmKeyList keyList, uint straKey, bool bSort = true, List<double> depthList = null)
        {
            keyList.Clear();
            DBDoc doc = DBDoc.Get_CurrDoc();
            HmKeyList allKeyList = new HmKeyList();
            if (Get_KeyList(allKeyList) == 0) return 0;

            if (depthList == null) depthList = new List<double>();
            else depthList.Clear();

            for (int i = allKeyList.Count - 1; i >= 0; i--)
            {
                DBDataDESC itemD = null;
                Get_Data(allKeyList.list[i], ref itemD, false);
                if (itemD.straKey == straKey)
                {
                    double currTop = 0.0, currBot = 0.0;
                    itemD.Get_Depth(ref currTop, ref currBot);
                    keyList.Add(allKeyList.list[i]);
                    depthList.Add(currTop);
                }
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
        public uint Get_StraKey(uint descKey)
        {
            DBDataDESC descD = null;
            if (!Get_Data(descKey, ref descD, false)) return 0;
            return descD.straKey;
        }
        public uint Get_DrlgKey(uint descKey)
        {
            return ((DBDoc)m_Doc).stra.Get_DrlgKey(Get_StraKey(descKey));
        }

        public bool Get_Depth(uint descKey, ref double dTop, ref double dBottom)
        {
            DBDataDESC descD = null;
            if (!Get_Data(descKey, ref descD)) return false;
            return descD.Get_Depth(ref dTop, ref dBottom);
        }
    }
}
