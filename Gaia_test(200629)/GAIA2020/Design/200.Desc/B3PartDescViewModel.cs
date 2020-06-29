namespace GAIA2020.Design
{
    using GAIA2020.Utilities;
    using GaiaDB;
    using GaiaDB.Enums;
    using HmDataDocument;
    using HMFrameWork.Ancestor;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="B3PartDescViewModel" />.
    /// </summary>
    public class B3PartDescViewModel : AViewModel, IDBUpdate
    {
        #region Fields

        /// <summary>
        /// 디비키.
        /// </summary>
        protected uint dbKey = 0;

        /// <summary>
        /// Defines the selectedSoilColumn.
        /// </summary>
        private string selectedSoilColumn;

        /// <summary>
        /// Defines the selectedStratum.
        /// </summary>
        private string selectedStratum;

        /// <summary>
        /// Defines the updateSelf.
        /// </summary>
        private bool updateSelf;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="B3PartDescViewModel"/> class.
        /// </summary>
        public B3PartDescViewModel()
        {
            this.WetStatus.Add(false);
            this.WetStatus.Add(false);
            this.WetStatus.Add(false);
            this.WetStatus.Add(false);
            this.DensityStatus.Add(false);
            this.DensityStatus.Add(false);
            this.DensityStatus.Add(false);
            this.DensityStatus.Add(false);
            this.DensityStatus.Add(false);
            this.DensityStatus.Add(false);
            this.DensityStatus.Add(false);
            this.DensityStatus.Add(false);
            this.DensityStatus.Add(false);
            this.DensityStatus.Add(false);
            this.DensityStatus.Add(false);
            this.SelectedColorIds.CollectionChanged += (s, e) => { propertyChanged("ColorIds", s); };
            this.WetStatus.CollectionChanged += (s, e) => { propertyChanged(eDescriptionKey.Wet, s); };
            this.DensityStatus.CollectionChanged += (s, e) => { propertyChanged(eDescriptionKey.Density, s); };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the DensityStatus.
        /// </summary>
        public ObservableCollection<bool> DensityStatus { get; set; } = new ObservableCollection<bool>();

        /// <summary>
        /// Gets or sets the SelectedColorIds.
        /// </summary>
        public ObservableCollection<int> SelectedColorIds { get; set; } = new ObservableCollection<int>();

        /// <summary>
        /// Gets or sets the SelectedSoilColumn.
        /// </summary>
        public string SelectedSoilColumn
        {
            get { return selectedSoilColumn; }
            set
            {
                SetValue(ref selectedSoilColumn, value);
                this.propertyChanged("SoilColumn", value);
            }
        }

        /// <summary>
        /// Gets or sets the SelectedStratum.
        /// </summary>
        public string SelectedStratum
        {
            get { return selectedStratum; }
            set
            {
                SetValue(ref selectedStratum, value);
                this.propertyChanged(eDescriptionKey.None/*eDescriptionKey.Stratum*/, value);
            }
        }

        /// <summary>
        /// Gets or sets the WetStatus.
        /// </summary>
        public ObservableCollection<bool> WetStatus { get; set; } = new ObservableCollection<bool>();

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
                //    case TRANSACTION_STATE.UPDATE:
                //        if (trData.Is_Data("STRA"))
                //        {
                //            //itemList의 몇번째가 선택됐는지는 어떻게 아는지...
                //            DBDataSTRA dbDataSTRA = trData.itemList.FirstOrDefault()?.currData as DBDataSTRA;
                //            if (dbDataSTRA != null)
                //            {
                //                this.SelectedSoilColumn = dbDataSTRA.Type;
                //                this.SelectedColorIds = new ObservableCollection<int>(dbDataSTRA.ColorList);
                //                //this.SelectedColorIds.Clear();
                //                //foreach (var item in dbDataSTRA.ColorList)
                //                //{
                //                //    this.SelectedColorIds.Add(item);
                //                //}
                //            }
                //        }
                //        break;
                //    case TRANSACTION_STATE.USER:
                //        break;
                //    case TRANSACTION_STATE.NEW:
                //        ClearAll();
                //        break;
                //    case TRANSACTION_STATE.OPEN:
                //        ClearAll();
                //        break;
                //    case TRANSACTION_STATE.SAVE:
                //        break;
                //    default:
                //        break;
            }
        }

        /// <summary>
        /// The EndInit.
        /// </summary>
        public override void EndInit()
        {
            App.GetViewModelManager().AddValue(typeof(B3PartDescViewModel), this);
            TransactionCtrl.Add_DBUpdateWndCtrl(this); // IDBUpdate를 상속받은 경우 필히 연결시켜줍니다. 
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
        /// <param name="key">The v<see cref="string"/>.</param>
        /// <param name="value">The s<see cref="object"/>.</param>
        private void propertyChanged(object key, object value)
        {
            if (!this.updateSelf)
            {
                // ※ 디키키 = 0 이면 추가하고, 디비키 > 0 이면 업데이트할 것, dbKeyStra 사용할 것
                if (dbKey != 0)
                {
                    var doc = DBDoc.Get_CurrDoc();
                    try
                    {
                        if (key is string)
                        {
                            DBDataSTRA dbStraData = null;

                            //추후 선택된 stra의 키값을 가지고 찾도록 변경 필요
                            //현재는 임시로 1로 찾음, 1값이 없다면 새로운 값을 추가함
                            if (!doc.stra.Get_Data(dbKey, ref dbStraData))
                            {
                                dbStraData = new DBDataSTRA();
                                dbStraData.drlgKey = 1;
                                doc.stra.Add_TR(dbStraData);
                            }

                            switch (key)
                            {
                                case "SoilColumn":
                                    {
                                        string type = value.ToString();
                                        if (type.Equals(string.Empty))
                                        {
                                            dbStraData.soilType = eSoil.None;
                                        }
                                        else
                                        {
                                            dbStraData.soilType = (eSoil)Enum.Parse(typeof(eSoil), type);
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
                        else
                        {
                            DBDataDESC dbDataDESC = null;

                            //추후 선택된 stra의 키값을 가지고 찾도록 변경 필요
                            //현재는 임시로 1로 찾음, 1값이 없다면 새로운 값을 추가함
                            if (!doc.desc.Get_Data(1, ref dbDataDESC))
                            {
                                dbDataDESC = new DBDataDESC();
                                dbDataDESC.straKey = 1;
                                doc.desc.Add_TR(dbDataDESC);
                            }

                            string data = string.Empty;
                            switch (key)
                            {
                                case eDescriptionKey.None/*eDescriptionKey.Stratum*/:
                                    {
                                        data = value.ToString();
                                        break;
                                    }
                                case eDescriptionKey.Density:
                                case eDescriptionKey.Wet:
                                    {
                                        ObservableCollection<bool> tmp = value as ObservableCollection<bool>;
                                        for (int i = 0; i < tmp.Count; i++)
                                        {
                                            if (tmp[i].Equals(false)) continue;
                                            data += KeyControlPair.Instance.CheckBoxes.FindAll(x => x.Item1.Equals(key)).Find(x => x.Item2.Equals(i)).Item3 + GaiaConstants.FIRST_DELIMITER;
                                        }
                                        data = data.Length > 0 ? data.Substring(0, data.Length - 1) : data;
                                        break;
                                    }
                            }

                            dbDataDESC.dicDesc[(eDescriptionKey)key] = data;

                            doc.desc.Modify_TR(dbKey, dbDataDESC);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        #endregion
    }
}
