namespace GAIA2020.Design
{
    using GAIA2020.Enums;
    using GAIA2020.Utilities;
    using GaiaDB;
    using GaiaDB.Enums;
    using HmDataDocument;
    using HMFrameWork.Ancestor;
    using HmGeometry;
    using LogStyle;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using CmdParmParser = LogStyle.CmdParmParser;

    /// <summary>
    /// Defines the <see cref="APartDescViewModel" />.
    /// </summary>
    public class APartDescViewModel : AViewModel, IDBUpdate
    {
        #region Fields

        /// <summary>
        /// 디비키...
        /// </summary>
        private uint dbKey;

        /// <summary>
        /// 편집대상 DB클래스...
        /// </summary>
        private eDescKind descKind = eDescKind.None;

        /// <summary>
        /// Defines the isEnabled.
        /// </summary>
        private bool isEnabled = false;

        /// <summary>
        /// Defines the opacity.
        /// </summary>
        private double opacity = .5;

        /// <summary>
        /// Defines the selectedSample.
        /// </summary>
        private eSampleType selectedSample = eSampleType.None;

        /// <summary>
        /// Defines the selectedSoil.
        /// </summary>
        private eSoil selectedSoil;

        /// <summary>
        /// Defines the updateSelf.
        /// </summary>
        private bool updateSelf;

        /// <summary>
        /// 샘플인 경우 깊이.
        /// </summary>
        private double zDepth = 0.0;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="APartDescViewModel"/> class.
        /// </summary>
        public APartDescViewModel()
        {
            this.SelectedColorIds.CollectionChanged += (s, e) => { propertyChanged("ColorIds", s); };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the DescKind
        /// Gets or sets a value indicating whether IsStra....
        /// </summary>
        public eDescKind DescKind
        {
            get { return descKind; }
            set
            {
                SetValue(ref descKind, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsEnabled.
        /// </summary>
        public bool IsEnabled
        {
            get { return isEnabled; }
            set { SetValue(ref isEnabled, value); }
        }

        /// <summary>
        /// Gets or sets the Opacity.
        /// </summary>
        public double Opacity
        {
            get { return opacity; }
            set { SetValue(ref opacity, value); }
        }

        /// <summary>
        /// Gets or sets the SelectedColorIds.
        /// </summary>
        public ObservableCollection<int> SelectedColorIds { get; set; } = new ObservableCollection<int>();

        /// <summary>
        /// Gets or sets the SelectedSample.
        /// </summary>
        public eSampleType SelectedSample
        {
            get { return selectedSample; }
            set
            {
                SetValue(ref selectedSample, value);
                this.propertyChanged("SelectedSample", value);
            }
        }

        /// <summary>
        /// Gets or sets the SelectedSoil.
        /// </summary>
        public eSoil SelectedSoil
        {
            get { return selectedSoil; }
            set
            {
                SetValue(ref selectedSoil, value);
                this.propertyChanged("SelectedSoil", value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The BeginInit.
        /// </summary>
        public override void BeginInit()
        {
        }

        /// <summary>
        /// The DBUpdate.
        /// </summary>
        /// <param name="trData">The trData<see cref="TransactionData"/>.</param>
        /// <param name="bUndo">The bUndo<see cref="bool"/>.</param>
        public void DBUpdate(TransactionData trData, bool bUndo)
        {
            switch (trData.state)
            {
                //case TRANSACTION_STATE.UPDATE:
                //    {
                //        if (trData.Is_Data("STRA"))
                //        {
                //            //itemList의 몇번째가 선택됐는지는 어떻게 아는지...
                //            DBDataSTRA dbDataSTRA = trData.itemList.FirstOrDefault()?.currData as DBDataSTRA;
                //            if (dbDataSTRA != null)
                //            {
                //                this.SelectedSoil = EnumUtil.GetEnumFromDescription<eSoil>(dbDataSTRA.stratumName);
                //            }
                //        }
                //        //if (trData.Is_Data("SAMP"))
                //        //{
                //        //    //itemList의 몇번째가 선택됐는지는 어떻게 아는지...
                //        //    DBDataSAMP dbDataSAMP = trData.itemList.FirstOrDefault()?.currData as DBDataSAMP;
                //        //    if (dbDataSAMP != null)
                //        //    {
                //        //        this.SelectedSampleItem = dbDataSAMP.SType.ToString();
                //        //    }
                //        //}
                //        break;
                //    }
                case TRANSACTION_STATE.USER:
                    {
                        this.InitData();
                        if (trData.strName.Equals("STRA"))
                        {
#if true
                            string valkey = CmdParmParser.GetValuefromKey(trData.strUserList, "key");
                            if (string.IsNullOrEmpty(valkey))
                                break;
                            // 디비키 추출
                            uint ukey = 0;
                            if (!uint.TryParse(valkey, out ukey))
                                break;
                            // 데이터가져오기
                            DBDataSTRA d = null;
                            DBDoc doc = DBDoc.Get_CurrDoc();
                            if (!doc.stra.Get_Data(ukey, ref d))
                                break;

                            // 데이터 화면에 업데이트
                            DescKind = eDescKind.Stra;
                            dbKey = ukey;
                            this.IsEnabled = true;
                            this.Opacity = 1;

                            this.updateSelf = true;

                            this.SelectedSoil = d.soilType;

                            List<int> removeTarget = this.SelectedColorIds.Except(d.ColorList).ToList();
                            List<int> addTarget = d.ColorList.Except(this.SelectedColorIds).ToList();
                            foreach (var item in removeTarget)
                            {
                                this.SelectedColorIds.Remove(item);
                            }

                            foreach (var item in addTarget)
                            {
                                this.SelectedColorIds.Add(item);
                            }

                            this.updateSelf = false;

#else
                            if (trData.objUserData is HmDataFramework.HmFileSystem.DataDirectoryKey)
                            {
                                //DDK = (HmDataFramework.HmFileSystem.DataDirectoryKey)(trData.objUserData);
                            }
#endif
                        }
                        else if (trData.strName.Equals("SAMP"))
                        {
                            string valkey;

                            valkey = CmdParmParser.GetValuefromKey(trData.strUserList, "key");
                            if (string.IsNullOrEmpty(valkey))
                                break;
                            // 디비키 추출
                            uint ukey = 0;
                            if (!uint.TryParse(valkey, out ukey))
                                break;
                            // 깊이
                            double zlv = 0.0;
                            valkey = CmdParmParser.GetValuefromKey(trData.strUserList, "zlv");
                            if (!double.TryParse(valkey, out zlv))
                                break;

                            // 데이터가져오기
                            eSampleType eSType = eSampleType.None;
                            DBDataSAMP d = null;
                            DBDoc doc = DBDoc.Get_CurrDoc();
                            if (doc.samp.Get_Data(ukey, ref d))
                            {
                                eSType = d.SType;
                            }
                            // 데이터 화면에 업데이트
                            DescKind = eDescKind.Samp;
                            dbKey = ukey;
                            this.IsEnabled = true;
                            this.Opacity = 1;
                            zDepth = zlv;

                            this.updateSelf = true;
                            this.SelectedSample = eSType;// d.SType;
                            this.updateSelf = false;
                        }
                        else if (trData.strName.Equals("ActiveDrillLog") || trData.strName.Equals("ActiveDrillStyle"))
                        {
                            dbKey = 0;
                            this.IsEnabled = false;
                            this.Opacity = .5;
                        }
                    }
                    break;
                    //case TRANSACTION_STATE.NEW:
                    //    ClearAll();
                    //    break;
                    //case TRANSACTION_STATE.OPEN:
                    //    ClearAll();
                    //    break;
                    //case TRANSACTION_STATE.SAVE:
                    //    break;
                    //default:
                    //    break;
            }
        }

        /// <summary>
        /// The EndInit.
        /// </summary>
        public override void EndInit()
        {
            App.GetViewModelManager().AddValue(typeof(APartDescViewModel), this);
            TransactionCtrl.Add_DBUpdateWndCtrl(this); // IDBUpdate를 상속받은 경우 필히 연결시켜줍니다. 
        }

        /// <summary>
        /// The initData.
        /// </summary>
        public void InitData()
        {
            this.updateSelf = true;
            this.DescKind = eDescKind.None;
            this.SelectedSoil = eSoil.None;
            this.SelectedColorIds.Clear();
            this.SelectedSample = eSampleType.None;
            this.updateSelf = false;
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
        /// The propertyChanged.
        /// </summary>
        /// <param name="kind">The kind<see cref="string"/>.</param>
        /// <param name="value">The value<see cref="object"/>.</param>
        private void propertyChanged(string kind, object value)
        {
            if (!this.updateSelf)
            {
                try
                {
                    if (kind.Equals("SelectedSample") && descKind == eDescKind.Samp)
                    {
                        // 시료형태는 디비키가 없는 경우에는 추가, 있으면 수정
                        var doc = DBDoc.Get_CurrDoc();
                        DBDataSAMP dbSampData = null;
#if true
                        if (dbKey == 0)
                        {
                            // 추가
                            dbSampData = new DBDataSAMP()
                            {
                                SType = (eSampleType)Enum.Parse(typeof(eSampleType), value.ToString()),
                                Depth = zDepth,
                                drlgKey = doc.Get_ActiveDrillLog(true).nKey,
                            };
                            dbKey = doc.samp.Add_TR(dbSampData); // 추가하여 디비키가 생성되었으므로 다음부터는 업데이트로 
                        }
                        else
                        {
                            // 수정
                            if (doc.samp.Get_Data(dbKey, ref dbSampData))
                            {
                                dbSampData.SType = (eSampleType)Enum.Parse(typeof(eSampleType), value.ToString());
                                doc.samp.Modify_TR(dbKey, dbSampData);
                            }
                        }
#else
                        //추후 선택된 stra의 키값을 가지고 찾도록 변경 필요
                        //현재는 임시로 1로 찾음, 1값이 없다면 새로운 값을 추가함
                        if (!doc.samp.Get_Data(1, ref dbSampData))
                        {
                            dbSampData = new DBDataSAMP();
                            dbSampData.drlgKey = 1;
                            doc.samp.Add_Data(doc.samp.New_Key(), dbSampData);
                        }
                        dbSampData.SType = (eSampleType)Enum.Parse(typeof(eSampleType), value.ToString());
                        doc.samp.Modify_Data(1, dbSampData);
#endif
                    }
                    else if (descKind == eDescKind.Stra && dbKey != 0)
                    {
                        // 지층은 디비키가 꼭 있어야함 즉, 수정만 가능
                        var doc = DBDoc.Get_CurrDoc();
                        DBDataSTRA dbStraData = null;
#if true

                        if (doc.stra.Get_Data(dbKey, ref dbStraData))
                        {
                            switch (kind)
                            {
                                case "SelectedSoil":
                                    {
                                        string type = value.ToString();
                                        if (type.Equals(string.Empty))
                                        {
                                            dbStraData.soilType = eSoil.None;
                                        }
                                        else
                                        {
                                            eSoil soil = (eSoil)Enum.Parse(typeof(eSoil), type);
                                            dbStraData.soilType = soil;

                                            if (soil.Equals(eSoil.wrock) || soil.Equals(eSoil.hrock) || soil.Equals(eSoil.srock))
                                            {                                                
                                                dbStraData.stratumName = EnumUtil.GetDescription(soil);
                                            }
                                            else
                                            {
                                                if(dbStraData.stratumName.Equals("풍화암") || dbStraData.stratumName.Equals("연암")|| dbStraData.stratumName.Equals("경암"))
                                                {
                                                    dbStraData.stratumName = string.Empty;
                                                }
                                            }
                                        }
                                        break;
                                    }
                                case "ColorIds":
                                    {
                                        dbStraData.ColorList = (value as ObservableCollection<int>).ToList();
                                        break;
                                    }
                            }
                            doc.stra.Modify_TR(dbKey, dbStraData);
                        }

#else
                        //추후 선택된 stra의 키값을 가지고 찾도록 변경 필요
                        //현재는 임시로 1로 찾음, 1값이 없다면 새로운 값을 추가함
                        if (!doc.stra.Get_Data(1, ref dbStraData))
                        {
                            dbStraData = new DBDataSTRA();
                            dbStraData.drlgKey = 1;
                            doc.stra.Add_Data(doc.stra.New_Key(), dbStraData);
                        }

                        switch (kind)
                        {
                            case "SelectedSoil":
                                {
                                    string type = value.ToString();
                                    if (type.Equals(string.Empty))
                                    {
                                        dbStraData.Type = eSoil.None;
                                    }
                                    else
                                    {
                                        dbStraData.Type = (eSoil)Enum.Parse(typeof(eSoil), type);
                                    }
                                    break;
                                }
                            case "ColorIds":
                                {
                                    dbStraData.ColorList = (value as ObservableCollection<int>).ToList();
                                    break;
                                }
                        }
                        doc.stra.Modify_Data(1, dbStraData);
#endif

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }

        #endregion
    }
}
