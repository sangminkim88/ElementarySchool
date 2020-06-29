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

    public class DBDataPOSI : DBBaseData
    {
        /// <summary>
        /// 외부참조여부
        /// </summary>
        public bool bExternalFile;
        /// <summary>
        /// 외부참조파일경로
        /// </summary>
        public string strFileName;
        /// <summary>
        /// 도면
        /// </summary>
        public List<HmGLayer> layerList;
        /// <summary>
        /// 그림객체에 대한 좌표계정보
        /// </summary>
        public HmGeoInfo geoInfo;

        public DBDataPOSI()
        {
            layerList = new List<HmGLayer>();
            geoInfo = new HmGeoInfo();

            Initialize();
        }

        ~DBDataPOSI()
        {
            Initialize();
        }

        public void Initialize()
        {
            bExternalFile = false;
            strFileName = "";
            layerList.Clear();
            geoInfo.Initialize();
        }

        public DBDataPOSI Clone()
        {
            DBDataPOSI data = new DBDataPOSI();
            data.bExternalFile = bExternalFile;
            data.strFileName = strFileName;
            foreach (var item in layerList)
            { data.layerList.Add(item.Clone()); }
            data.geoInfo = geoInfo.Clone();
            return data;
        }

        public override DBBaseData BaseClone()
        {
            return Clone();
        }

        public override bool Write_Binary(BinaryWriter binaryRW)
        {
            binaryRW.Write(bExternalFile);
            binaryRW.Write(strFileName);

            binaryRW.Write(layerList.Count);
            foreach (var item in layerList)
            { if (!item.Write_Binary(binaryRW)) return false; }

            if (!geoInfo.Write_Binary(binaryRW)) return false;
            return true;
        }

        public override bool Read_Binary(BinaryReader binaryRW, int nReadHMVer, int nReadVer)
        {
            int i, isize;

            Initialize();
            bExternalFile = binaryRW.ReadBoolean();
            strFileName = binaryRW.ReadString();

            isize = binaryRW.ReadInt32();
            for (i = 0; i < isize; i++)
            {
                HmGLayer layer = new HmGLayer();
                if (!layer.Read_Binary(binaryRW, nReadHMVer)) return false;
                layerList.Add(layer);
            }

            if (!geoInfo.Read_Binary(binaryRW, nReadHMVer)) return false;

            return true;
        }
    }

    public class DBClassPOSI : DBBaseClass
    {
        public DBClassPOSI(HmBaseDoc doc)
        {
            m_strDBKey = "POSI";
            m_strDBNote = "위치도";
            m_bSingleData = true; // 단일 자료구조
            Set_Doc(doc);
        }

        ~DBClassPOSI()
        {

        }

        // 단일 자료구조인경우 //////////////////////////////////////////////////////////////////////////////////////////// 
        // 아래 함수형은 복수자료형방식으로 타 복수자료방식과 동일합니다.(Get_Data ~ Del_TR)


        /// <summary>
        /// DataPool에서 해당 Data를 가져옵니다.
        /// </summary>
        public bool Get_Data(ref DBDataPOSI data, bool bClone = true)
        { // Clone이 아닌경우 자료를 가져와 수정시 Transaction을 통한 관리가 되지 않습니다. 또한 단위계는 내부 공용단위계 값으로 넘겨줍니다. 
          // 따라서 bClone을 false로 쓸때에는 수정하지 않고 공용단위계로 쓸경우에만(View화면갱신 등) 사용하도록 합니다.
            DBBaseData baseData = null;
            if (!Get_BaseData(1, ref baseData, bClone)) return false;
            data = (DBDataPOSI)baseData;
            return true;
        }

        // Transaction을 발생시키며 Data를 변경하는 경우  //////////////////////////////////

        /// <summary>
        /// DB에 data를 추가 및 수정합니다.(Transaction발생)
        /// </summary>
        public uint Add_TR(DBDataPOSI data)
        { return Add_BaseTR(data); } // 단일 Data형은 Modify시에도 Add를 호출합니다.
                                     // DB에 data를 제거합니다.(Transaction발생)
                                     // public bool Del_TR(uint nKey = 0) // DBBaseClass에 정의 되어있음

        // Transaction없이 Data를 변경하는 경우 (TR발생하지 않으므로 사용시 주의 요망) /////

        // - DBBaseClass에 정의 되어있으며 여러종류의 Data 변경을 하나의 Transaction내에서 변경할경우에 사용합니다.
        // public uint Add_Data(uint nKey, DBBaseData data)
        // public bool Del_Data(uint nKey)


        // 재정의 함수 //////////////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// 해당 data의 자료의 오류여부를 검토합니다. 오류 발생시에 오류 내용을 strMsg에 채워서 넘겨줍니다.
        /// </summary> 
        public override bool Check(uint nKey, DBBaseData data, ref string strMsg)
        {
            DBDataPOSI currData = (DBDataPOSI)data;
            //
            return true;
        }
        /// <summary>
        /// 해당 data의 자료를 단위별로 UnitFactor값을 곱하여 넘겨줍니다.
        /// </summary>
        //public override void Convert_Unit(DBBaseData data, Dictionary<UNIT_TYPE, double> dicUnitFactor)
        //{
        //    DBDataPOSI currData = (DBDataPOSI)data;
        //    // 단위계작업이 필요합니다.
        //}
        /// <summary>
        /// 해당 data의 자료 변환시 연관관계 처리를 수행합니다.
        /// </summary> 
        protected override bool Association(uint nKey, DBBaseData data, DBBaseData data_Org, TRANSACTION_DATA type)
        {
            DBDataPOSI currData = type == TRANSACTION_DATA.DEL ? null : (DBDataPOSI)data;
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
            return new DBDataPOSI();
        }

        // 개별 추가작업 ///////////////////////////////////////////////////////////////////////////////////////////////
        // 필요시 추가함수를 정의합니다.
    }
}
