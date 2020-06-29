using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaiaDB
{
    using HmDataDocument;
    using HmGeometry;
    using System.IO;

    /// <summary>
    /// 사진의 영역
    /// </summary>
    public class ImageRect
    {
        /// <summary>
        /// 좌상 Y 값
        /// </summary>
        public int top;
        /// <summary>
        /// 좌상 X 값
        /// </summary>
        public int left;
        /// <summary>
        /// height
        /// </summary>
        public int height;
        /// <summary>
        /// width
        /// </summary>
        public int width;
        /// <summary>
        /// 회전정보
        /// </summary>
        public double rotation;

        public ImageRect()
        {
            Initialize();
        }

        ~ImageRect()
        {
            Initialize();
        }

        public void Initialize()
        {
            top = 0;
            left = 0;
            height = 0;
            width = 0;
            rotation = 0.0;
        }

        public ImageRect Clone()
        {
            ImageRect data = new ImageRect();
            data.top = top;
            data.left = left;
            data.height = height;
            data.width = width;
            data.rotation = rotation;
            return data;
        }

        public bool Write_Binary(BinaryWriter binaryRW)
        {
            binaryRW.Write(top);
            binaryRW.Write(left);
            binaryRW.Write(height);
            binaryRW.Write(width);
            binaryRW.Write(rotation);
            return true;
        }

        public bool Read_Binary(BinaryReader binaryRW, int nReadHMVer)
        {
            Initialize();
            top = binaryRW.ReadInt32();
            left = binaryRW.ReadInt32();
            height = binaryRW.ReadInt32();
            width = binaryRW.ReadInt32();
            rotation = binaryRW.ReadDouble();
            return true;
        }
    }

    /// <summary>
    /// 절리형상 (Joint Shape)
    /// </summary>
    public class DBDataJSHP : DBBaseData
    {
        /// <summary>
        /// 해당 지층 데이터의 키
        /// </summary>
        public uint straKey;
        /// <summary>
        /// 절리형상 사진
        /// </summary>
        public HmImage img;
        /// <summary>
        /// 절리형상 사진 관심영역
        /// </summary>
        public ImageRect cropArea;
        /// <summary>
        /// 셀 해상도, 픽셀단거리, distance in pixel
        /// </summary>
        public double distInPxl;
        /// <summary>
        /// 절리형상 벡터 폴리라인
        /// </summary>
        public List<HmLine2D> lines;
        /// <summary>
        /// 절리형상 심도, 위치정보
        /// </summary>
        public double depth;
        /// <summary>
        /// 절리형상 두께, 위치정보
        /// </summary>
        public double thick;

        public DBDataJSHP()
        {
            img = new HmImage();
            cropArea = new ImageRect();
            lines = new List<HmLine2D>();

            Initialize();
        }

        ~DBDataJSHP()
        {
            Initialize();
        }

        public void Initialize()
        {
            straKey = 0;
            img.Initialize();
            cropArea.Initialize();
            distInPxl = 0.0;
            lines.Clear();
            depth = 0.0;
            thick = 0.0;
        }

        public DBDataJSHP Clone()
        {
            DBDataJSHP data = new DBDataJSHP();
            data.straKey = straKey;
            data.img = img.Clone();
            data.cropArea = cropArea.Clone();
            data.distInPxl = distInPxl;
            foreach (var item in lines)
            { data.lines.Add(item.Clone()); }
            data.depth = depth;
            data.thick = thick;
            return data;
        }

        public override DBBaseData BaseClone()
        {
            return Clone();
        }

        public override bool Write_Binary(BinaryWriter binaryRW)
        {
            binaryRW.Write(straKey);

            if (!img.Write_Binary(binaryRW)) return false;
            if (!cropArea.Write_Binary(binaryRW)) return false;

            binaryRW.Write(distInPxl);

            binaryRW.Write(lines.Count);
            foreach (var item in lines)
            { if (!item.Write_Binary(binaryRW)) return false; }

            binaryRW.Write(depth);
            binaryRW.Write(thick);
            return true;
        }

        public override bool Read_Binary(BinaryReader binaryRW, int nReadHMVer, int nReadVer)
        {
            int i, isize;

            Initialize();

            straKey = binaryRW.ReadUInt32();

            if (!img.Read_Binary(binaryRW, nReadHMVer)) return false;
            if (!cropArea.Read_Binary(binaryRW, nReadHMVer)) return false;

            distInPxl = binaryRW.ReadDouble();

            isize = binaryRW.ReadInt32();
            for (i = 0; i < isize; i++)
            {
                HmLine2D line2D = new HmLine2D();
                if (!line2D.Read_Binary(binaryRW, nReadHMVer)) return false;
                lines.Add(line2D);
            }

            depth = binaryRW.ReadDouble();
            thick = binaryRW.ReadDouble();

            return true;
        }
    }

    public class DBClassJSHP : DBBaseClass
    {
        public DBClassJSHP(HmBaseDoc doc)
        {
            m_strDBKey = "JSHP";
            m_strDBNote = "절리형상";
            m_bSingleData = false; // 복수 자료구조
            Set_Doc(doc);
        }

        ~DBClassJSHP()
        {

        }

        // 복수 자료구조인경우 ////////////////////////////////////////////////////////////////////////////////////////////
        // 아래 함수형은 복수자료형방식으로 타 복수자료방식과 동일합니다.(Get_Data ~ Del_TR)

        /// <summary>
        /// DataPool에서 해당 Key의 Data를 가져옵니다.
        /// </summary>
        public bool Get_Data(uint nKey, ref DBDataJSHP data, bool bClone = true)
        { // Clone이 아닌경우 자료를 가져와 수정시 Transaction을 통한 관리가 되지 않습니다. 또한 단위계는 내부 공용단위계 값으로 넘겨줍니다. 
          // 따라서 bClone을 false로 쓸때에는 수정하지 않고 공용단위계로 쓸경우에만(View화면갱신 등) 사용하도록 합니다.
            DBBaseData baseData = null;
            if (!Get_BaseData(nKey, ref baseData, bClone)) return false;
            data = (DBDataJSHP)baseData;
            return true;
        }

        /// <summary>
        /// DataPool에서 해당 KeyList의 DataList를 가져옵니다.
        /// </summary>
        public bool Get_DataList(HmKeyList keyList, ref List<DBDataJSHP> dataList, bool bClone = true)
        {
            DBDataJSHP data = new DBDataJSHP();
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
        public uint Add_TR(DBDataJSHP data)
        { return Add_BaseTR(data); }
        /// <summary>
        /// DB에 dataList를 추가합니다.(Transaction발생)
        /// </summary>
        public bool Add_TR(List<DBDataJSHP> dataList)
        {
            List<DBBaseData> baseList = new List<DBBaseData>();
            for (int i = 0; i < dataList.Count; i++) baseList.Add(dataList[i]);
            return Add_BaseTR(baseList);
        }
        /// <summary>
        /// DB에 data를 수정합니다.(Transaction발생)
        /// </summary>
        public bool Modify_TR(uint nKey, DBDataJSHP data)
        { return Modify_BaseTR(nKey, data); }
        /// <summary>
        /// DB에 dataList를 수정합니다.(Transaction발생)
        /// </summary> 
        public bool Modify_TR(HmKeyList keyList, List<DBDataJSHP> dataList)
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
            DBDataJSHP currData = (DBDataJSHP)data;
            //
            return true;
        }
        /// <summary>
        /// 해당 data의 자료를 단위별로 UnitFactor값을 곱하여 넘겨줍니다.
        /// </summary>
        public override void Convert_Unit(DBBaseData data, Dictionary<UNIT_TYPE, double> dicUnitFactor)
        {
            DBDataJSHP currData = (DBDataJSHP)data;
            // 단위계작업이 필요합니다.
        }
        /// <summary>
        /// 해당 data의 자료 변환시 연관관계 처리를 수행합니다.
        /// </summary> 
        protected override bool Association(uint nKey, DBBaseData data, DBBaseData data_Org, TRANSACTION_DATA type)
        {
            DBDataJSHP currData = type == TRANSACTION_DATA.DEL ? null : (DBDataJSHP)data;
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
            return new DBDataJSHP();
        }

        /// <summary>
        /// 해당지층의 키를 가지는 모든 절리형상의 키리스트를 넘겨줍니다.
        /// </summary>
        /// <param name="keyList">절리형상의 키리스트</param>
        /// <param name="straKey">해당지층키</param>
        /// <returns></returns>
        public int Get_KeyList(HmKeyList keyList, uint straKey)
        {
            DBDoc doc = DBDoc.Get_CurrDoc();
            if (Get_KeyList(keyList) == 0) return 0;

            for (int i = keyList.Count - 1; i >= 0; i--)
            {
                DBDataJSHP itemD = null;
                Get_Data(keyList.list[i], ref itemD, false);
                if (itemD.straKey != straKey)
                { keyList.list.RemoveAt(i); }
            }

            return keyList.Count;
        }
    }
}
