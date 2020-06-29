using GaiaDB;
using GaiaDB.Enums;
using HmDataDocument;
using HmGeometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GAIA2020.Menu
{
    //public class DictionaryItemConverter : IMultiValueConverter
    //{
    //    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        if (values != null && values.Length >= 2)
    //        {
    //            var myDict = values[0] as IDictionary;
    //            var myKey = values[1] as string;
    //            if (myDict != null && myKey != null)
    //            {
    //                //the automatic conversion from Uri to string doesn't work
    //                //return myDict[myKey];
    //                return myDict[myKey].ToString();
    //            }
    //        }
    //        return Binding.DoNothing;
    //    }

    //    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new NotSupportedException();
    //    }
    //}

    partial class MenuViewModel : IDBUpdate
    {
        private eDepartment _Department;
        private bool m_lockDataUpdate = false;

        public eDepartment CurrDepartment
        {
            get
            {
                return _Department;
            }
            set
            {
                SetValue(ref _Department, value);
                //this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DrillLog"));
                //NotifyPropertyChanged("CurrDrillLog");
            }
        }



        #region 인터페이스함수
        public bool Is_Exist()
        {
            return true;
        }

        public void DBUpdate(TransactionData trData, bool bUndo)
        {
            if (m_lockDataUpdate) return;
            m_lockDataUpdate = true;

            if (trData.state == TRANSACTION_STATE.NEW || trData.state == TRANSACTION_STATE.OPEN)
            {
                DBClassDRLG dBClassDRLG = DBDoc.Get_CurrDoc().drlg;
                HmKeyList keyList = new HmKeyList();

                foreach (eDepartment item in Enum.GetValues(typeof(eDepartment)))
                {
                    if (item.Equals(eDepartment.None)) continue;

                    keyList.Clear();
                    this.DrillLogCounts[item] = dBClassDRLG.Get_KeyList(keyList, item);
                }
            }
            else if (trData.state == TRANSACTION_STATE.USER)
            {
                if(trData.strName == "ActiveDrillLog" && trData.objUserData is HmDBKey)
                {
                    CurrDepartment = DBDoc.Get_CurrDoc().drlg.Get_Department(((HmDBKey)trData.objUserData).nKey);
                }
                else if (trData.strName == "ActiveDrillStyle" && trData.objUserData is eDepartment)
                {
                    CurrDepartment = (eDepartment)(trData.objUserData);
                }
            }
            else if(trData.state == TRANSACTION_STATE.UPDATE)
            {
                if (trData.Is_Data("DRLG", TRANSACTION_DATA.ADD) || trData.Is_Data("DRLG", TRANSACTION_DATA.DEL))
                { DBUpdate_DRLG(); }
                else if(trData.Is_Data("DRLG", TRANSACTION_DATA.MODIFY))
                {
                    for(int i=0; i<trData.itemList.Count; i++)
                    {
                        if(trData.itemList[i].strDBKey == "DRLG" || trData.itemList[i].type == TRANSACTION_DATA.MODIFY)
                        {
                            if (((DBDataDRLG)(trData.itemList[i].beforeData)).EDepartment != ((DBDataDRLG)(trData.itemList[i].currData)).EDepartment)
                            { DBUpdate_DRLG();  break; }
                        }
                    }
                }
            }
            m_lockDataUpdate = false;
        }
        #endregion

        public void DBUpdate_All()
        {
            DBUpdate_DRLG();
        }

        public void DBUpdate_DRLG()
        {
            DBClassDRLG dBClassDRLG = DBDoc.Get_CurrDoc().drlg;
            HmKeyList keyList = new HmKeyList();

            foreach (eDepartment item in Enum.GetValues(typeof(eDepartment)))
            {
                if (item.Equals(eDepartment.None)) continue;

                keyList.Clear();
                this.DrillLogCounts[item] = dBClassDRLG.Get_KeyList(keyList, item);
            }
        }

        #region 종류별 주상도 개수
        //private int fillsize;
        //public int FillSize
        //{
        //    get { return fillsize; }
        //    set { SetValue(ref fillsize, value); }
        //}
        //public Dictionary<string, int> Bori
        //{
        //    get { return BoriCategory; }
        //    set { SetValue(ref BoriCategory, value); }
        //}

        //private void Upsert(string e, int size)
        //{
        //    if(BoriCategory.ContainsKey(e))
        //    {
        //        BoriCategory[e] = size;

        //    }
        //    else
        //        BoriCategory.Add(e, size);

        //    //
        //    //Bori = BoriCategory;
        //}
        #endregion
    }
}
