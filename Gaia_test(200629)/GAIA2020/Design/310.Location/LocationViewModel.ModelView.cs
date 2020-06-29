namespace GAIA2020.Design
{
    using GAIA2020.Utilities;
    using GaiaDB;
    using HmDataDocument;
    using HmGeometry;
    using HmViewWpf.MultiView;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Input;

    /// <summary>
    /// Defines the <see cref="LocationViewModel" />.
    /// </summary>
    public partial class LocationViewModel : IDBUpdate, IMapViewParents
    {
        #region Fields

        /// <summary>
        /// Defines the m_LockZoomFit.
        /// </summary>
        internal bool m_LockZoomFit = false;

        /// <summary>
        /// Defines the m_ucModelView.
        /// </summary>
        internal IMultiView m_ucModelView;

        #endregion

        #region Methods

        /// <summary>
        /// The DBUpdate.
        /// </summary>
        /// <param name="trData">The trData<see cref="TransactionData"/>.</param>
        /// <param name="bUndo">The bUndo<see cref="bool"/>.</param>
        public void DBUpdate(TransactionData trData, bool bUndo)
        {
            if (m_ucModelView == null) return; //해당 View가 한번도 Visible상태가 된적이 없다면 DBUpdate를 건너뜁니다.

            if (trData.state == TRANSACTION_STATE.NEW || trData.state == TRANSACTION_STATE.OPEN)
            {
                DBUpdate_All();
            }
            else if (trData.state == TRANSACTION_STATE.USER)
            {
            }
            else if (trData.state == TRANSACTION_STATE.UPDATE)
            {
                bool bZoomFit = false;
                if (trData.Is_Data("POSI"))
                { DBUpdate_All(); return; } // 모두 지우고 다시 그립니다.(DRLG만빼고 나머지 Layer만 선별적으로 지우고 만드는거와 새로 만드는것의 차이가 거의 없을것으로 판단됨)

                for (int i = 0; i < trData.itemList.Count; i++)
                {
                    if (trData.itemList[i].strDBKey == "DRLG")
                    {
                        DBUpdate_DRLG(trData.itemList[i], bUndo);
                        bZoomFit = true;
                    }
                }

                if (!m_LockZoomFit && bZoomFit) m_ucModelView.ZoomFitView();
            }
            // 그리기 설정 취소
            m_ucModelView.Set_DrawCommand(HMMAPDRAW_TYPE.NONE);
            // ZoomFit Lock 해제
            m_LockZoomFit = false;
            return;
        }

        /// <summary>
        /// The DBUpdate_All.
        /// </summary>
        public void DBUpdate_All()
        {
            if (m_ucModelView == null) return;
            m_ucModelView.Clear_Entities(true);

            DBUpdate_POSI();
            DBUpdate_DRLG();

            m_ucModelView.ZoomFitView();
        }

        /// <summary>
        /// The DBUpdate_DRLG.
        /// </summary>
        public void DBUpdate_DRLG()
        {            
            DBDoc doc = DBDoc.Get_CurrDoc();
            HmKeyList keyList = new HmKeyList();
            doc.drlg.Get_KeyList(keyList);
            for (int i = 0; i < keyList.Count; i++)
            {
                DBDataDRLG dbData = null;
                if (!doc.drlg.Get_Data(keyList.list[i], ref dbData)) continue;
                DBUpdate_DRLG(dbData, keyList.list[i]);
            }
        }

        /// <summary>
        /// The DBUpdate_DRLG.
        /// </summary>
        /// <param name="dbData">The dbData<see cref="DBDataDRLG"/>.</param>
        /// <param name="nkey">The nkey<see cref="uint"/>.</param>
        public void DBUpdate_DRLG(DBDataDRLG dbData, uint nkey)
        {
            if (!m_ucModelView.Is_Layer("DrillLog"))
            { m_ucModelView.Add_Layer("DrillLog", System.Drawing.Color.Red); }

            AddBlockLayer();

            HmPoint2D point = dbData.Position;

            GeodeticUtil.TransPosi(ref point, dbData.geoInfo, m_ucModelView.Get_GeoInfo());            
            List<HmGEntity> entityList = new List<HmGEntity>();

            // 주상도 심볼Block
            string blockName = string.Format("Location_{0}", dbData.DrillPipeNum.Split('-')[0]);
            HmGBlockRef blk = new HmGBlockRef(new HmPoint3D(point.Y, point.X, 0.0), blockName);            
            m_ucModelView.UpdateEntity(blk, false, blockName, new HmDBKey("DRLG", nkey));

            HmGText textEntity = new HmGText();
            textEntity.Position.Set(point.Y + 2, point.X + 2, 0.0);
            textEntity.Text = dbData.DrillPipeNum;
            textEntity.Height = 5;
            entityList.Add(textEntity);

            m_ucModelView.UpdateEntity(entityList, false, blockName, new HmDBKey("DRLG", nkey));
        }

        /// <summary>
        /// The DBUpdate_DRLG.
        /// </summary>
        /// <param name="trItem">The trItem<see cref="TransactionItem"/>.</param>
        /// <param name="bUndo">The bUndo<see cref="bool"/>.</param>
        public void DBUpdate_DRLG(TransactionItem trItem, bool bUndo)
        {
            if (trItem.type == TRANSACTION_DATA.DEL || trItem.type == TRANSACTION_DATA.MODIFY)
            {
                m_ucModelView.Remove_Entity(new HmDBKey("DRLG", trItem.nKey, -1));
            }
            if (trItem.type == TRANSACTION_DATA.ADD || trItem.type == TRANSACTION_DATA.MODIFY)
            {
                DBUpdate_DRLG((DBDataDRLG)trItem.currData, trItem.nKey);
            }
        }

        /// <summary>
        /// The DBUpdate_POSI.
        /// </summary>
        public void DBUpdate_POSI()
        {
            DBDoc dbDoc = DBDoc.Get_CurrDoc();
            DBDataPOSI posi = null;
            if (!dbDoc.posi.Get_Data(ref posi))
            {
                m_ucModelView.Set_GeoInfo(new HmGeoInfo(5186));
                return;
            }

            m_ucModelView.Set_GeoInfo(posi.geoInfo);

            if (posi.bExternalFile)
            {
                if (File.Exists(posi.strFileName))
                {
                    m_ucModelView.OpenFile(posi.strFileName, true);
                }
                else
                {
                    NotifyHelper.Instance.Show(NotifyHelper.NotiType.Error, posi.strFileName + "이 존재하지 않습니다.");
                }
            }
            else
            {
                foreach (HmGLayer hmGLayer in posi.layerList)
                {// nDBKeyIncrease : 복수의 HmGEntity를 입력할때 Tag에 들어가는 DBKey의 값을 증가시켜줄지 여부(-1:입력그대로 입력, 0:SubID값 증가, 1:3rdID값 증가
                    int nLayerID = m_ucModelView.Add_Layer(hmGLayer.Name, hmGLayer.Display);
                    m_ucModelView.UpdateEntity(hmGLayer.List, false, hmGLayer.Name, new HmDBKey("POSI", 1, nLayerID), false, true, null, 1);
                }
            }
        }

        /// <summary>
        /// The EndCurrentDrawing.
        /// </summary>
        /// <param name="nViewID">The nViewID<see cref="uint"/>.</param>
        /// <param name="drawType">The drawType<see cref="HMMAPDRAW_TYPE"/>.</param>
        /// <param name="pointList">The pointList<see cref="HmPolyline3D"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool EndCurrentDrawing(uint nViewID, HMMAPDRAW_TYPE drawType, HmPolyline3D pointList)
        {
            return true;
        }

        /// <summary>
        /// The EndLoaded.
        /// </summary>
        /// <param name="nViewID">The nViewID<see cref="uint"/>.</param>
        public void EndLoaded(uint nViewID)
        {
            m_ucModelView.Set_2DView(true);
            //m_ucModelView.Set_WebImage(true); WebImage사용시 HmDraw에서 ChangeOrder함수에서 오류가 발생함에 따라 막아둡니다.
            m_ucModelView.Set_BackGround(HMMAPDRAW_BACKGROUND.BLACK);

            DBUpdate_All();
        }

        //, devDept.Graphics.RenderContextBase renderContext);
        /// <summary>
        /// The Is_Exist.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool Is_Exist()
        {
            return true;
        }

        /// <summary>
        /// The RelayCurrentDrawing.
        /// </summary>
        /// <param name="nViewID">The nViewID<see cref="uint"/>.</param>
        /// <param name="drawType">The drawType<see cref="HMMAPDRAW_TYPE"/>.</param>
        /// <param name="pointList">The pointList<see cref="HmPolyline3D"/>.</param>
        public void RelayCurrentDrawing(uint nViewID, HMMAPDRAW_TYPE drawType, HmPolyline3D pointList)
        {
        }

        /// <summary>
        /// The Set_ViewCtrl.
        /// </summary>
        /// <param name="modelView">The modelView<see cref="IMultiView"/>.</param>
        public void Set_ViewCtrl(IMultiView modelView)
        {
            TransactionCtrl.Add_DBUpdateWndCtrl(this); // IDBUpdate를 상속받은 경우 필히 연결시켜줍니다. 

            m_ucModelView = modelView;
            m_ucModelView.Set_Parents(this);
        }

        /// <summary>
        /// The UpdateCameraLocation.
        /// </summary>
        /// <param name="nViewID">The nViewID<see cref="uint"/>.</param>
        /// <param name="x">The x<see cref="double"/>.</param>
        /// <param name="y">The y<see cref="double"/>.</param>
        /// <param name="z">The z<see cref="double"/>.</param>
        public void UpdateCameraLocation(uint nViewID, double x, double y, double z)
        {
        }

        /// <summary>
        /// The UpdateDrawOverlay.
        /// </summary>
        /// <param name="nViewID">The nViewID<see cref="uint"/>.</param>
        /// <param name="drawType">The drawType<see cref="HMMAPDRAW_TYPE"/>.</param>
        /// <param name="pointList">The pointList<see cref="HmPolyline3D"/>.</param>
        /// <param name="currentPoint">The currentPoint<see cref="HmPoint3D"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool UpdateDrawOverlay(uint nViewID, HMMAPDRAW_TYPE drawType, HmPolyline3D pointList, HmPoint3D currentPoint)
        {
            return false;
        }

        /// <summary>
        /// The UpdateKeyDown.
        /// </summary>
        /// <param name="nViewID">The nViewID<see cref="uint"/>.</param>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="KeyEventArgs"/>.</param>
        public void UpdateKeyDown(uint nViewID, object sender, KeyEventArgs e)
        {
        }

        /// <summary>
        /// The UpdateKeyUp.
        /// </summary>
        /// <param name="nViewID">The nViewID<see cref="uint"/>.</param>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="KeyEventArgs"/>.</param>
        public void UpdateKeyUp(uint nViewID, object sender, KeyEventArgs e)
        {
        }

        /// <summary>
        /// The UpdateMouseClick.
        /// </summary>
        /// <param name="nViewID">The nViewID<see cref="uint"/>.</param>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="MouseButtonEventArgs"/>.</param>
        public void UpdateMouseClick(uint nViewID, object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// The UpdateMouseDoubleClick.
        /// </summary>
        /// <param name="nViewID">The nViewID<see cref="uint"/>.</param>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="MouseButtonEventArgs"/>.</param>
        public void UpdateMouseDoubleClick(uint nViewID, object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// The UpdateMouseDown.
        /// </summary>
        /// <param name="nViewID">The nViewID<see cref="uint"/>.</param>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="MouseButtonEventArgs"/>.</param>
        public void UpdateMouseDown(uint nViewID, object sender, MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// The UpdateMouseMove.
        /// </summary>
        /// <param name="nViewID">The nViewID<see cref="uint"/>.</param>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="MouseEventArgs"/>.</param>
        public void UpdateMouseMove(uint nViewID, object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// The UpdateMouseWheel.
        /// </summary>
        /// <param name="nViewID">The nViewID<see cref="uint"/>.</param>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="MouseWheelEventArgs"/>.</param>
        public void UpdateMouseWheel(uint nViewID, object sender, MouseWheelEventArgs e)
        {
        }

        /// <summary>
        /// The UpdateObjectLocation.
        /// </summary>
        /// <param name="nViewID">The nViewID<see cref="uint"/>.</param>
        /// <param name="x">The x<see cref="double"/>.</param>
        /// <param name="y">The y<see cref="double"/>.</param>
        /// <param name="z">The z<see cref="double"/>.</param>
        public void UpdateObjectLocation(uint nViewID, double x, double y, double z)
        {
        }

        private void AddBlockLayer()
        {
            if (!m_ucModelView.Is_Layer("Location_BB"))
            {
                m_ucModelView.Add_Layer("Location_BB", System.Drawing.Color.Cyan);
            }
            if (!m_ucModelView.Is_Layer("Location_SB"))
            {
                m_ucModelView.Add_Layer("Location_SB", System.Drawing.Color.LightGreen);
            }
            if (!m_ucModelView.Is_Layer("Location_TB"))
            {
                m_ucModelView.Add_Layer("Location_TB", System.Drawing.Color.DarkBlue);
            }
            if (!m_ucModelView.Is_Layer("Location_CB"))
            {
                m_ucModelView.Add_Layer("Location_CB", System.Drawing.Color.Magenta);
            }            
        }
        #endregion
    }
}
