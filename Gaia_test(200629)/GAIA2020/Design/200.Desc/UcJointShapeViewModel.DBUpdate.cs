using GAIA2020.Utilities;
using GaiaDB;
using HmDataDocument;
using HmGeometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAIA2020.Design
{
    public partial class UcJointShapeViewModel : IDBUpdate
    {
        private uint m_nKey;
        private uint m_nStraKey;

        public void View_Load()
        {
            //TransactionCtrl.Add_DBUpdateWndCtrl(this); // IDBUpdate를 상속받은 경우 필히 연결시켜줍니다. 
            //DBUpdate_All();
        }

        #region 인터페이스함수
        public bool Is_Exist()
        {
            return true;
        }

        public void DBUpdate(TransactionData trData, bool bUndo)
        {
            if (trData.state == TRANSACTION_STATE.NEW || trData.state == TRANSACTION_STATE.OPEN)
            { DBUpdate_All(); }
            else if (trData.state == TRANSACTION_STATE.UPDATE)
            {
                if (!trData.Is_Data("JSHP")) return;

                for (int i = 0; i < trData.itemList.Count; i++)
                {
                    if (trData.itemList[i].strDBKey == "JSHP")
                    { DBUpdate_JSHP(trData.itemList[i], bUndo); }
                }
            }
            else if (trData.state == TRANSACTION_STATE.SAVE)
            { }
            else if (trData.state == TRANSACTION_STATE.USER)
            {
                if (0 == string.Compare(trData.strName, "B2PartDesc"))
                {
                    string valkey;

                    // 디비키 추출
                    valkey = CmdParmParser.GetValuefromKey(trData.strUserList, "key");
                    if (string.IsNullOrEmpty(valkey))
                        return;

                    uint ukey = 0;
                    if (!uint.TryParse(valkey, out ukey))
                        return;

                    // 지층키 추출
                    valkey = CmdParmParser.GetValuefromKey(trData.strUserList, "strakey");
                    if (string.IsNullOrEmpty(valkey))
                        return;

                    uint ukeyStra = 0;
                    if (!uint.TryParse(valkey, out ukeyStra))
                        return;

                    m_nKey = ukey;
                    m_nStraKey = ukeyStra;

                    // 기존 데이터를 UI에 뿌려줍니다.
                    DBDoc doc = DBDoc.Get_CurrDoc();
                    DBDataJSHP jshpD = new DBDataJSHP();
                    if (doc.jshp.Get_Data(ukey, ref jshpD)) // 해당 키의 절리형상 데이터 있으면 DBUpdate
                    {
                        DBUpdate_JSHP(jshpD, ukey);
                    }
                    else // 해당 키의 절리형상 없으면 아무것도 안보인다.
                    {
                        m_jshpImage = null;
                        m_jshpLines?.Clear();
                        UpdateImage();
                    }
                }
            }
        }

        public void DBUpdate_All()
        {
            DBDoc doc = DBDoc.Get_CurrDoc();
            HmDBKey dbKey = doc.Get_ActiveStratum(true);

            uint ukeyStra = dbKey.nKey; // Stra key
            uint ukey = (uint)dbKey.nSubID; // Desc key

            m_nKey = ukey;
            m_nStraKey = ukeyStra;

            // 기존 데이터를 UI에 뿌려줍니다.
            DBDataJSHP jshpD = new DBDataJSHP();
            if (doc.jshp.Get_Data(ukey, ref jshpD)) // 해당 키의 절리형상 데이터 있으면 DBUpdate
            {
                DBUpdate_JSHP(jshpD, ukey);
            }
            else // 해당 키의 절리형상 없으면 아무것도 안보인다.
            {
                m_jshpImage = null;
                m_jshpLines?.Clear();
                UpdateImage();
            }
        }

        public void DBUpdate_JSHP(TransactionItem trItem, bool bUndo)
        {
            if (trItem.type == TRANSACTION_DATA.DEL || trItem.type == TRANSACTION_DATA.MODIFY || trItem.type == TRANSACTION_DATA.REDRAW)
            {
                //picturebox에서 사진을 제거
            }

            if (trItem.type == TRANSACTION_DATA.ADD || trItem.type == TRANSACTION_DATA.MODIFY || trItem.type == TRANSACTION_DATA.REDRAW)
            {
                DBDataJSHP jshpD = null;
                if (trItem.type == TRANSACTION_DATA.REDRAW)
                { if (!DBDoc.Get_CurrDoc().jshp.Get_Data(trItem.nKey, ref jshpD, false)) jshpD = null; }
                else
                { jshpD = (DBDataJSHP)trItem.currData; }

                if (jshpD != null)
                { DBUpdate_JSHP(jshpD, trItem.nKey); }
            }
        }

        public void DBUpdate_JSHP(DBDataJSHP dbData, uint nKey)
        {
            this.HmDBKey = new HmDBKey("JSHP", nKey);

            m_jshpImage = new HmBitmap();
            m_jshpImage.img = new System.Drawing.Bitmap(dbData.img.Image);

            m_jshpLines?.Clear();
            m_jshpLines.AddRange(dbData.lines);

            UpdateImage();

            return;

            // draw line on image
            System.Drawing.Bitmap image = (System.Drawing.Bitmap)dbData.img.Image.Clone();
            System.Drawing.Pen redPen = new System.Drawing.Pen(System.Drawing.Color.Red, 2);

            foreach (var line in dbData.lines)
            {
                using (var graphics = System.Drawing.Graphics.FromImage(image))
                {
                    graphics.DrawLine(redPen, (float)line.StartPoint.X, (float)line.StartPoint.Y, (float)line.EndPoint.X, (float)line.EndPoint.Y);
                }
            }

            this.Img = ImageConverter.BitMapToBitmapImage(image, System.Drawing.Imaging.ImageFormat.Png);


        }
        #endregion
    }
}
