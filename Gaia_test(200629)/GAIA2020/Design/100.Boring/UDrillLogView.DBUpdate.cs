namespace GAIA2020.Design
{
    using GAIA2020.Manager;
    using GAIA2020.Models;
    using GaiaDB;
    using GaiaDB.Enums;
    using HmDataDocument;
    using HmGeometry;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="UDrillLogView" />.
    /// </summary>
    public partial class UDrillLogView : IDBUpdate
    {
        #region Methods

        /// <summary>
        /// The DBUpdate.
        /// </summary>
        /// <param name="trData">The trData<see cref="TransactionData"/>.</param>
        /// <param name="bUndo">The bUndo<see cref="bool"/>.</param>
        public void DBUpdate(TransactionData trData, bool bUndo)
        {
            if (trData.state == TRANSACTION_STATE.NEW || trData.state == TRANSACTION_STATE.OPEN)
            {
                // 아래는 삭제해도 정상작동함 확인요망
                DBDoc doc = DBDoc.Get_CurrDoc();
                HmDBKey dbkey = doc.Get_ActiveDrillLog();
                Draw(dbkey);
            }
            else if (trData.state == TRANSACTION_STATE.USER)
            {
                if (trData.strName == "ActiveDrillLog" && trData.objUserData is HmDBKey)
                {
                    this.selectedEntityInfo = null;
                    Draw((HmDBKey)trData.objUserData);
                }
                else if (trData.strName == "ActiveDrillStyle" && trData.objUserData is eDepartment)
                {
                    if (aloading)
                    {
                        eDepartment e = (eDepartment)Enum.Parse(typeof(eDepartment), trData.objUserData.ToString());
                        // 여기는 주상도가 없는 경우에 호출되므로 기본값으로 범례있는 스타일 사용
                        LoadStyle(e, true, true);
                    }
                }
            }
            else if (trData.state == TRANSACTION_STATE.UPDATE)
            {
                if (dlog.Canvas.GetGraphics().HelperDevice.UnmanagedObject.ToInt64() == 0L) return;
                //if (trData.Is_Data("PROJ") || trData.Is_Data("DRLG") || trData.Is_Data("STRA") || trData.Is_Data("DESC") || trData.Is_Data("JSHP") || trData.Is_Data("SAMP") || trData.Is_Data("SPTG"))
                {
                    Draw();
                }
            }
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
        /// The Draw.
        /// </summary>
        /// <param name="bZoomExtents">The bZoomExtents<see cref="bool"/>.</param>
        private void Draw(bool bZoomExtents = false)
        {
            DBDoc doc = DBDoc.Get_CurrDoc();
            HmDBKey actDrlg = doc.Get_ActiveDrillLog(true);
            Draw(actDrlg, bZoomExtents);
        }

        /// <summary>
        /// The Draw.
        /// </summary>
        /// <param name="actDrlg">The actDrlg<see cref="HmDBKey"/>.</param>
        /// <param name="bZoomExtents">The bZoomExtents<see cref="bool"/>.</param>
        private void Draw(HmDBKey actDrlg, bool bZoomExtents = true)
        {
            // 로그 컨트롤이 로딩되어 있지 않으면 통과
            if (!aloading)
                return;
            if (actDrlg == null)
                return;
#if false
            HmDraw.View.Wpf.ViewPanel viewPanelCtrl = dlog.canvas.GetViewPanel();
            if (viewPanelCtrl.Graphics != null)
            {//[bug:GAIA-47] 부모창이 사라지면서 viewPanelCtrl.Graphics.GraphicHandle값이 0으로 초기되었다가 다시 연결되면서 viewPanelCtrl.Graphics.GraphicHandle에 값이 채워지지 않는 현상이 발생함
                if (viewPanelCtrl.Graphics.GraphicHandle == IntPtr.Zero && viewPanelCtrl.HwndSource.Handle != IntPtr.Zero)
                    viewPanelCtrl.Graphics.GraphicHandle = viewPanelCtrl.HwndSource.Handle;
            }
#endif
            // 디비키
            uint ukey = actDrlg.nKey;
            DBDoc doc = DBDoc.Get_CurrDoc();

            // 로그스타일 읽기
            if (ukey == 0)
            {
                eDepartment e = doc.Get_ActiveStyle();
                LoadStyle(e, true);
            }
            else
            // 주상도 읽기
            {
                DBDataDRLG drlgD = null;
                if (!doc.drlg.Get_Data(actDrlg.nKey, ref drlgD)) return;

                // 주상도 스타일
                eDepartment e = doc.drlg.Get_Department(ukey);
                LogStyleManager styleManager = App.GetLogStyleManager();
                Tuple<double, double> height = styleManager.GetStyleHeight(e);
                doc.drlg.NormalDrillLogHeight = height.Item1;
                doc.drlg.LegendDrillLogHeight = height.Item2;
                int pagecount = doc.drlg.Get_PageCount(ukey);
                bool needLegend = actDrlg.nSubID.Equals(pagecount);
                LoadStyle(e, needLegend, bZoomExtents);

                DrillProperty drillProp = new DrillProperty()
                {
                    Drilltype = drlgD.EDepartment,
                    Title = drlgD.DrillPipeNum,
                    Ukey = actDrlg.nKey,
                    Page = actDrlg.nSubID
                };
                Draw(drillProp);
            }
        }
        #endregion
    }
}
