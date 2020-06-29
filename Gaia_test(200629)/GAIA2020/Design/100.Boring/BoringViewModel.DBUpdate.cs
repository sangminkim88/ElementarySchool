namespace GAIA2020.Design
{
    using GAIA2020.Manager;
    using GAIA2020.Models;
    using GAIA2020.Utilities;
    using GaiaDB;
    using GaiaDB.Enums;
    using HmDataDocument;
    using HmGeometry;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="BoringViewModel" />.
    /// </summary>
    public partial class BoringViewModel : IDBUpdate
    {
        #region Fields

        /// <summary>
        /// Defines the m_lockDataUpdate.
        /// </summary>
        private bool m_lockDataUpdate = false;

        /// <summary>
        /// 화면뷰어..
        /// </summary>
        private UDrillLogView m_ucModelView;

        #endregion

        #region Methods

        /// <summary>
        /// The DBUpdate.
        /// </summary>
        /// <param name="trData">The trData<see cref="TransactionData"/>.</param>
        /// <param name="bUndo">The bUndo<see cref="bool"/>.</param>
        public void DBUpdate(TransactionData trData, bool bUndo)
        {
            if (m_lockDataUpdate) return;
            m_lockDataUpdate = true;
            if (trData.state == TRANSACTION_STATE.NEW || trData.state == TRANSACTION_STATE.OPEN)
            {
                DBUpdate_All();
            }
            else if (trData.state == TRANSACTION_STATE.USER)
            {
                if (trData.strName == "ActiveDrillLog" && trData.objUserData is HmDBKey)
                {
                    Update_ActDRLG((HmDBKey)(trData.objUserData));
                    this.AddDrillLogVisibility = System.Windows.Visibility.Collapsed;
                    this.AddButtonContent = "+";
                }
                else if (trData.strName == "ActiveDrillStyle" && trData.objUserData is eDepartment)
                {
                    Update_ActDRLG((eDepartment)(trData.objUserData));
                    this.AddDrillLogVisibility = System.Windows.Visibility.Collapsed;
                    this.AddButtonContent = "+";
                }
            }
            else if (trData.state == TRANSACTION_STATE.UPDATE)
            {
                bool bUpdateDrlgList = false;
                for (int i = 0; i < trData.itemList.Count; i++)
                {
                    if (trData.itemList[i].strDBKey == "DRLG") DBUpdate_DRLG(trData.itemList[i], bUndo, ref bUpdateDrlgList);
                }

                if (bUpdateDrlgList)
                {
                    DBDoc doc = DBDoc.Get_CurrDoc();
                    HmDBKey actDrlg = doc.Get_ActiveDrillLog(true);

                    Update_DRLGList();
                    Update_PageList();

                    Update_ActDRLG(actDrlg);
                }
            }

            m_lockDataUpdate = false;
        }

        /// <summary>
        /// The DBUpdate_All.
        /// </summary>
        public void DBUpdate_All()
        {
            Update_DRLGList();
            Update_PageList();

            Update_ActDRLG(DBDoc.Get_CurrDoc().Get_ActiveDrillLog(true));
        }

        /// <summary>
        /// The DBUpdate_DRLG.
        /// </summary>
        /// <param name="trItem">The trItem<see cref="TransactionItem"/>.</param>
        /// <param name="bUndo">The bUndo<see cref="bool"/>.</param>
        /// <param name="bUpdateDrlgList">The bUpdateDrlgList<see cref="bool"/>.</param>
        public void DBUpdate_DRLG(TransactionItem trItem, bool bUndo, ref bool bUpdateDrlgList)
        {
            DBDoc doc = DBDoc.Get_CurrDoc();
            eDepartment currDepartment = doc.Get_ActiveStyle();

            if (trItem.type == TRANSACTION_DATA.DEL)
            { if (((DBDataDRLG)(trItem.beforeData)).EDepartment != currDepartment) return; }
            else
            { if (((DBDataDRLG)(trItem.currData)).EDepartment != currDepartment) return; }


            if (trItem.type != TRANSACTION_DATA.MODIFY) { bUpdateDrlgList = true; }
            else
            {
                if (((DBDataDRLG)(trItem.beforeData)).DrillPipeNum != ((DBDataDRLG)(trItem.currData)).DrillPipeNum)
                { bUpdateDrlgList = true; } // 이름 바뀐 경우 갱신
                if (((DBDataDRLG)(trItem.beforeData)).Depth != ((DBDataDRLG)(trItem.currData)).Depth)
                { bUpdateDrlgList = true; } // 깊이 바뀐 경우 갱신(Page 갯수)
            }
            return;
        }

        /// <summary>
        /// The Is_Exist.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Is_Exist()
        {
            return true;
        }

        /// <summary>
        /// The Set_ViewCtrl.
        /// </summary>
        /// <param name="modelview">The modelview<see cref="UDrillLogView"/>.</param>
        public void Set_ViewCtrl(UDrillLogView modelview)
        {
            m_ucModelView = modelview;
        }

        /// <summary>
        /// The Update_ActDRLG.
        /// </summary>
        /// <param name="style">The style<see cref="eDepartment"/>.</param>
        public void Update_ActDRLG(eDepartment style)
        {
            HmDBKey actDrlg = DBDoc.Get_CurrDoc().Get_ActiveDrillLog(style);
            if (actDrlg.nKey > 0) Update_ActDRLG(actDrlg);
            else
            {
                Update_DRLGList(style);
                Update_PageList(0);
            }
        }

        /// <summary>
        /// The Update_ActDRLG.
        /// </summary>
        /// <param name="drlgKey">The drlgKey<see cref="HmDBKey"/>.</param>
        public void Update_ActDRLG(HmDBKey drlgKey)
        {
            if (!drlgKey.nKey.Equals(0))
            {
                DBDoc doc = DBDoc.Get_CurrDoc();
                HmDBKey actDrlg = doc.Get_ActiveDrillLog(false);
                if (doc.drlg.Get_Department(actDrlg.nKey) != doc.drlg.Get_Department(drlgKey.nKey)) Update_DRLGList(drlgKey.nKey);
                if (actDrlg.nKey != drlgKey.nKey) Update_PageList(drlgKey.nKey);

                this.DrillLogCountSelectedIndex = this.DrillLogCounts.IndexOf(this.DrillLogCounts.First(x => x.Ukey.Equals(drlgKey.nKey)));
                this.PageCountSelectedIndex = drlgKey.nSubID - 1;
                //int pageIndex = drlgKey.nSubID - 1;
                //if(this.pageCounts.Count <= pageIndex)
                //{
                //    drlgKey.nSubID = 0;
                //    DBDoc.Get_CurrDoc().Set_ActiveDrillLog(drlgKey);
                //}
            }
        }

        /// <summary>
        /// The Update_DRLGList.
        /// </summary>
        /// <param name="style">The style<see cref="eDepartment"/>.</param>
        public void Update_DRLGList(eDepartment style)
        {
            // drill log combobox item를 제거
            drillLogCounts.Clear();

            // 지금 선택된 style에 해당 되는 drlg를 가져오고
            DBDoc doc = DBDoc.Get_CurrDoc();
            HmKeyList drlgKeyList = new HmKeyList();
            doc.drlg.Get_KeyList(drlgKeyList, style);

            // 여기에 채워 준다.
            List<DrillProperty> drillLogPropertyList = new List<DrillProperty>();

            foreach (var nKey in drlgKeyList.list)
            {
                DBDataDRLG drlgD = null;
                if (doc.drlg.Get_Data(nKey, ref drlgD))
                {
                    //int nCurrentPageNumber = PageCountSelectedIndex + 1; // 현재 선택되어 있는 page number
                    //int nPageCount = PageCounts.Count; // 총 page의 개수
                    //drillLogPropertyList.Add(new DrillProperty()
                    //{
                    //    DepartmentType = drlgD.EDepartment,
                    //    DrillPipeNum = drlgD.DrillPipeNum,
                    //    HmDBKey = new HmDBKey("DRLG", nKey, nCurrentPageNumber, nPageCount),
                    //});

                    drillLogPropertyList.Add(new DrillProperty()
                    {
                        Drilltype = drlgD.EDepartment,
                        Title = drlgD.DrillPipeNum,
                        Ukey = nKey,
                        Page = 1, // 이것은 현재 선택 되어 있는 page의 index. Active style 또는 Active drlg 변경되도 자기 page index를 기억해야함. 그리고 우상에 있는 page 선택의 property과 binding 되어야 함.
                    });
                }
            }
            
            List<DrillProperty> unformatted = drillLogPropertyList.FindAll(x => x.Title == null || !x.Title.Contains("-"));
            List<DrillProperty> formatted = drillLogPropertyList.Except(unformatted).OrderBy(x => StringUtil.GetFirstIntFromString(x.Title.Split('-')[1])).ToList();

            unformatted.ForEach(x => drillLogCounts.Add(x));
            formatted.ForEach(x => drillLogCounts.Add(x));
        }

        /// <summary>
        /// The Update_DRLGList.
        /// </summary>
        /// <param name="nKey">The nKey<see cref="uint"/>.</param>
        public void Update_DRLGList(uint nKey = 0)
        {
            DBDoc doc = DBDoc.Get_CurrDoc();
            if (nKey == 0) nKey = doc.Get_ActiveDrillLog(true).nKey;

            Update_DRLGList(doc.drlg.Get_Department(nKey));
        }

        /// <summary>
        /// The Update_PageList.
        /// </summary>
        /// <param name="nKey">The nKey<see cref="uint"/>.</param>
        public void Update_PageList(uint nKey = 0)
        {
            pageCounts.Clear();

            DBDoc doc = DBDoc.Get_CurrDoc();
            if (nKey == 0) nKey = doc.Get_ActiveDrillLog(true).nKey;
            DBDataDRLG drlgD = null;
            doc.drlg.Get_Data(nKey, ref drlgD);
            if (drlgD != null)
            {
                eDepartment e = doc.drlg.Get_Department(nKey);
                LogStyleManager styleManager = App.GetLogStyleManager();
                Tuple<double, double> height = styleManager.GetStyleHeight(e);
                doc.drlg.NormalDrillLogHeight = height.Item1;
                doc.drlg.LegendDrillLogHeight = height.Item2;
                int pagecount = doc.drlg.Get_PageCount(nKey);

                for (int i = 1; i <= pagecount; i++)
                {
                    pageCounts.Add(i);
                }
            }
        }

        #endregion
    }
}
