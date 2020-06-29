namespace GAIA2020.Design
{
    using GAIA2020.Models;
    using GAIA2020.Utilities;
    using GaiaDB;
    using HmDataDocument;
    using HMFrameWork.Command;
    using HmGeometry;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;

    /// <summary>
    /// Defines the <see cref="BoringViewModel" />.
    /// </summary>
    public partial class BoringViewModel
    {
        #region Fields

        /// <summary>
        /// Defines the commandAdd.
        /// </summary>
        private ICommand commandAdd;

        /// <summary>
        /// Defines the commandAddPage.
        /// </summary>
        private ICommand commandAddPage;

        /// <summary>
        /// Defines the commandRemove.
        /// </summary>
        private ICommand commandRemove;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the CommandAdd.
        /// </summary>
        public ICommand CommandAdd
        {
            get
            {
                return commandAdd ?? (commandAdd = new RelayCommand(addExecute));
            }
        }

        /// <summary>
        /// Gets the CommandAddPage.
        /// </summary>
        public ICommand CommandAddPage
        {
            get
            {
                return commandAddPage ?? (commandAddPage = new RelayCommand(addPageExecute));
            }
        }

        /// <summary>
        /// Gets the CommandRemove.
        /// </summary>
        public ICommand CommandRemove
        {
            get
            {
                return commandRemove ?? (commandRemove = new RelayCommand(CommandRemoveExecute));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The AddExecute.
        /// 주상도 추가 할 때 현재 active 되어 있는 주상도의 해더 정보를 복사해서 생성한다.
        /// </summary>
        /// <param name="obj">The obj<see cref="object"/>.</param>
        private void addExecute(object obj)
        {
            DBDoc doc = DBDoc.Get_CurrDoc();

            if (DrillLogCounts.ToList().Find(x => x.Title.Equals(obj.ToString())) != null)
            {
                NotifyHelper.Instance.Show(NotifyHelper.NotiType.Error, "동명이 존재합니다.");
                return;
            }

            try
            {
                uint nDrlgKey = 0;
                GaiaDB.Enums.eDepartment activeStyle = doc.Get_ActiveStyle();
                HmDBKey activeDrlgKey = doc.Get_ActiveDrillLog();

                doc.Get_TranCtrl().TransactionOpen(TRANSACTION_DATA.ADD, "주상도");

                DBDataDRLG activeDrlgD = null;
                if (doc.drlg.Get_Data(activeDrlgKey.nKey, ref activeDrlgD)) // 현재 선택된 (active) 주상도 데이터(헤더)를 복사하고 신규 주상도 생성
                {
                    // drlg
                    DBDataDRLG drlgD = activeDrlgD.Clone();
                    drlgD.DrillPipeNum = obj.ToString(); // 공번 세팅
                    drlgD.Depth = 0.0; // 주상도 추가시 시추심도는 항상 0.0 됨
                    nDrlgKey = doc.drlg.Add_Data(0, drlgD);

                    //// stra
                    //HmKeyList straKeyList = new HmKeyList();
                    //doc.stra.Get_KeyList(straKeyList, activeDrlgKey.nKey);

                    //foreach (var oldStraKey in straKeyList.list)
                    //{
                    //    DBDataSTRA straD = null;
                    //    if (doc.stra.Get_Data(oldStraKey, ref straD))
                    //    {
                    //        straD.drlgKey = nDrlgKey;
                    //        uint nStraKey = doc.stra.Add_Data(0, straD);

                    //        // desc
                    //        HmKeyList descKeyList = new HmKeyList();
                    //        doc.desc.Get_KeyList(descKeyList, oldStraKey);

                    //        foreach (var oldDescKey in descKeyList.list)
                    //        {
                    //            DBDataDESC descD = null;
                    //            if (doc.desc.Get_Data(oldDescKey, ref descD))
                    //            {
                    //                descD.straKey = nStraKey;
                    //                uint nDescKey = doc.desc.Add_Data(0, descD);
                    //            }

                    //            DBDataJSHP jshpD = null;
                    //            if(doc.jshp.Get_Data(oldDescKey, ref jshpD))
                    //            {
                    //                jshpD.straKey = nStraKey;
                    //                uint nJshpKey = doc.desc.Add_Data(0, jshpD);
                    //            }
                    //        }
                    //    }
                    //}

                    //// samp
                    //HmKeyList sampKeyList = new HmKeyList();
                    //doc.samp.Get_KeyList(sampKeyList, activeDrlgKey.nKey);

                    //foreach (var oldSampKey in sampKeyList.list)
                    //{
                    //    DBDataSAMP sampD = null;
                    //    if (doc.samp.Get_Data(oldSampKey, ref sampD))
                    //    {
                    //        sampD.drlgKey = nDrlgKey;
                    //        uint nSampKey = doc.samp.Add_Data(0, sampD);
                    //    }
                    //}

                    //// sptg
                    //DBDataSPTG sptgD = null;
                    //if (doc.sptg.Get_Data(activeDrlgKey.nKey, ref sptgD))
                    //{
                    //    doc.sptg.Add_Data(nDrlgKey, sptgD);
                    //}
                }
                else // 신규 주상도 생성
                {
                    DBDataDRLG drlgD = new DBDataDRLG();
                    drlgD.EDepartment = activeStyle; // 부 세팅
                    drlgD.DrillPipeNum = obj.ToString(); // 공번 세팅
                    nDrlgKey = doc.drlg.Add_Data(0, drlgD);
                }

                doc.Get_TranCtrl().TransactionClose(true, true);

                doc.Set_ActiveDrillLog(new HmDBKey("DRLG", nDrlgKey, 1 /*pageNum*/));
            }
            catch (Exception ex)
            {
                if (doc.Get_TranCtrl().Is_TransactionOpen())
                { doc.Get_TranCtrl().TransactionClose(false, true); }
                System.Windows.MessageBox.Show(ex.Message.ToString());
            }
            finally { }
        }

        /// <summary>
        /// The addPageExecute.
        /// </summary>
        /// <param name="obj">The obj<see cref="object"/>.</param>
        private void addPageExecute(object obj)
        {
            if (PageCounts.Count < 1)
            {
                if (this.DrillLogCounts.Count > 0)
                {
                    this.PageCounts.Add(1);
                }
            }
            else
            {
                this.PageCounts.Add(this.PageCounts.Count + 1);
                this.PageCountSelectedIndex = this.PageCounts.Count - 1;
            }
        }

        /// <summary>
        /// The CommandRemoveExecute.
        /// </summary>
        /// <param name="parameter">The parameter<see cref="object"/>.</param>
        private void CommandRemoveExecute(object parameter)
        {
            var drillLogProperty = parameter as DrillProperty;
            if (drillLogProperty == null)
                return;

            DBDoc doc = DBDoc.Get_CurrDoc();

            try
            {
                // 들어온 drill log property가 combobox list에 있는지 확인하자
                var drillLogProperty2 = DrillLogCounts.FirstOrDefault(i => i.Ukey == drillLogProperty.Ukey);
                if (drillLogProperty2 != null)
                {
                    doc.Get_TranCtrl().TransactionOpen(TRANSACTION_DATA.DEL, "주상도");

                    // Rmove item from DB
                    doc.drlg.Del_Data(drillLogProperty.Ukey);

                    // Remove item from UI list
                    drillLogCounts.Remove(drillLogProperty2);

                    // Change active drlg
                    if (DrillLogCounts.Count > 0)
                    {
                        // 첫번째 drill log property를 선택
                        var drillLogProperty3 = DrillLogCounts.First();
                        HmDBKey dbKey = new HmDBKey("DRLG", drillLogProperty3.Ukey, drillLogProperty3.Page);
                        doc.Set_ActiveDrillLog(dbKey);
                    }
                    else
                    {
                        doc.Set_ActiveDrillLog(new HmDBKey());
                    }

                    doc.Get_TranCtrl().TransactionClose(true, true);
                }
            }
            catch (Exception ex)
            {
                if (doc.Get_TranCtrl().Is_TransactionOpen())
                { doc.Get_TranCtrl().TransactionClose(false, true); }
                System.Windows.MessageBox.Show(ex.Message.ToString());
            }
            finally { }
        }

        ///// <summary>
        ///// 열기/가져오기/내보내기 기능
        ///// 테스트 단계에서 활용하였으나 현재는 UI부터 제거됨
        ///// 추후 천지인 파일 관련돼 활용 가능성 있음
        ///// 카자브 선임
        ///// </summary>
        ///// <param name = "parameter" ></ param >
        //private void FileExecute(object parameter)
        //{
        //    try
        //    {
        //        string valstr, strkey = parameter.ToString();
        //        // 명령창 종류
        //        valstr = CmdParmParser.GetValuefromKey(strkey, "cmd");

        //        DBDoc doc = DBDoc.Get_CurrDoc();

        //        if (valstr == "Import")
        //        {
        //            OpenFileDialog openDig = new OpenFileDialog();
        //            openDig.Multiselect = true;
        //            openDig.InitialDirectory = "";
        //            openDig.Title = "도면선택";
        //            openDig.Filter = "Vector파일(*.BORI)|*.bori";

        //            if (openDig.ShowDialog() == true)
        //            {
        //                int i, isize = openDig.FileNames.Length;
        //                uint nKey;
        //                if (isize > 0)
        //                {
        //                    for (i = 0; i < isize; i++)
        //                    {
        //                        DBDataGiBORI dataD = new DBDataGiBORI();
        //                        DBBaseClassExternal.Get_ExternalData(dataD, openDig.FileNames[i]);

        //                        // DBDataPROJ는 단일 데이터이며 한번만 추가됩니다.
        //                        if (!doc.proj.Is_ExistData())
        //                        {
        //                            DBDataPROJ projD = new DBDataPROJ();
        //                            projD.ProjectName = dataD.boring.businessName != "" ? dataD.boring.businessName : "Sample";
        //                            projD.CompanyName = dataD.boring.orderPlace != "" ? dataD.boring.orderPlace : "Sample";
        //                            //projD.DrillManName = dataD.boring.driller;
        //                            //projD.WriteManName = dataD.boring.writer;
        //                            doc.proj.Add_TR(projD);
        //                        }

        //                        //시추주상도 데이터
        //                        DBDataDRLG drlgD = new DBDataDRLG();

        //                        //조사일
        //                        drlgD.SurveyDay = dataD.boring.surveyDay;
        //                        //시추장비
        //                        drlgD.DrillDevice = dataD.boring.drillEquipment;

        //                        //해머효율
        //                        double dHammerEfficiency;
        //                        if (double.TryParse(dataD.boring.hammerEfficiency, out dHammerEfficiency))
        //                        { drlgD.HammerEfficiencyPercent = dHammerEfficiency; }

        //                        //시추방법
        //                        drlgD.DrillingMethod = dataD.boring.drillingMethod;

        //                        //시추각도
        //                        drlgD.DrillingAngleType = dataD.boring.drillingAngle;

        //                        //교량명 또는 터널명
        //                        drlgD.BridgeOrTunnelName = dataD.boring.bridgeName;

        //                        //시추위치
        //                        drlgD.DrillLocation = dataD.boring.surveyLocation;

        //                        //교대(각)위치
        //                        drlgD.BridgeOrTunnelLocation = dataD.boring.anglePosition;

        //                        //시추공경
        //                        drlgD.DrillPipe = dataD.boring.drillingLandscape;

        //                        //시추공번
        //                        drlgD.Position = new HmGeometry.HmPoint2D(dataD.boring.x, dataD.boring.y);

        //                        //시추공경
        //                        drlgD.DrillPipeNum = dataD.boring.title;

        //                        //시추표고
        //                        drlgD.Elevation = dataD.boring.z;

        //                        //시추심도
        //                        drlgD.Depth = dataD.boring.drillingDepth;

        //                        //케이싱심도
        //                        drlgD.CasingDepth = dataD.boring.casingDepth;

        //                        //지하수의
        //                        drlgD.WaterLevel = dataD.boring.waterLv;

        //                        //부서 종류
        //                        if (dataD.boring.title.Contains("SB")) // 쌓기부
        //                        { drlgD.EDepartment = GaiaDB.Enums.eDepartment.Fill; }
        //                        else if (dataD.boring.title.Contains("BB")) // 교량부
        //                        { drlgD.EDepartment = GaiaDB.Enums.eDepartment.Bridge; }
        //                        else if (dataD.boring.title.Contains("CB")) // 깎기부
        //                        { drlgD.EDepartment = GaiaDB.Enums.eDepartment.Cut; }
        //                        else if (dataD.boring.title.Contains("TB")) // 터널부
        //                        { drlgD.EDepartment = GaiaDB.Enums.eDepartment.Tunnel; }
        //                        else if (dataD.boring.title.Contains("BP")) // 토취장
        //                        { drlgD.EDepartment = GaiaDB.Enums.eDepartment.BorrowPit; }
        //                        else if (dataD.boring.title.Contains("TP")) // 시험굴
        //                        { drlgD.EDepartment = GaiaDB.Enums.eDepartment.TrialPit; }
        //                        else if (dataD.boring.title.Contains("HAB")) // 핸드오거
        //                        { drlgD.EDepartment = GaiaDB.Enums.eDepartment.HandAuger; }

        //                        drlgD.DrillManName = dataD.boring.driller;
        //                        drlgD.WriteManName = dataD.boring.writer;

        //                        uint drlgKey = doc.drlg.Add_TR(drlgD); // 시추주사도의 디비키

        //                        foreach (HmGeoLayer item in dataD.boring.listStory)
        //                        {
        //                            //지층 레벨 데이터
        //                            DBDataSTRA straD = new DBDataSTRA();
        //                            straD.drlgKey = drlgKey;
        //                            straD.Depth = item.zbot;
        //                            string type = item.sciencename;
        //                            if (type.Equals(string.Empty))
        //                            {
        //                                straD.soilType = eSoil.None;
        //                            }
        //                            else
        //                            {
        //                                straD.soilType = (eSoil)Enum.Parse(typeof(eSoil), type);
        //                            }
        //                            // string color를 GAIA color index으로 변경 필요
        //                            uint straKey = doc.stra.Add_TR(straD);

        //                            //지층 설명 데이터
        //                            DBDataDESC descD = new DBDataDESC();
        //                            descD.straKey = straKey;
        //                            //uint descKey = doc.desc.Add_Data(0, descD);
        //                        }

        //                        //표준관입시험 데이터
        //                        DBDataSPTG sptgD = new DBDataSPTG();
        //                        foreach (HmDrillSpt item in dataD.boring.listSpt)
        //                        { sptgD.SptList.Add(new System.Tuple<double, int, double>(item.depth, item.spt_value, item.spt_depth)); }
        //                        doc.sptg.Add_TR(sptgD);
        //                    }
        //                }
        //            }
        //        }
        //        else if (valstr == "Export")
        //        {
        //            bool flag = doc.Save_Document("", false);
        //            if (flag)
        //            {
        //            }
        //        }
        //        else if (valstr == "Open")
        //        {
        //            bool flag = doc.Open_Document("");
        //            if (flag)
        //            {
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        NotifyHelper.Instance.Show(NotifyHelper.NotiType.Error, ex.Message.ToString());
        //    }
        //    finally
        //    { }
        //}

        #endregion
    }
}
