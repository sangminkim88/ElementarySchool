namespace GAIA2020.Design
{
    using GAIA2020.Utilities;
    using GaiaDB;
    using GaiaDB.Enums;
    using HmDataDocument;
    using HMFrameWork.Ancestor;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Defines the <see cref="B1PartDescViewModel" />.
    /// </summary>
    public class B1PartDescViewModel : AViewModel, IDBUpdate
    {
        #region Fields

        /// <summary>
        /// 디비키....
        /// </summary>
        protected uint dbKey = 0;

        /// <summary>
        /// 디비키(참조 지층)...
        /// </summary>
        protected uint dbKeyStra = 0;

        /// <summary>
        /// Defines the updateSelf.
        /// </summary>
        protected bool updateSelf;

        /// <summary>
        /// Defines the isEnabled.
        /// </summary>
        private bool isEnabled = false;

        /// <summary>
        /// Defines the isSand.
        /// </summary>
        private bool? isSand;

        /// <summary>
        /// Defines the opacity.
        /// </summary>
        private double opacity = .5;

        /// <summary>
        /// Defines the partialJoint.
        /// </summary>
        private bool partialJoint;

        /// <summary>
        /// Defines the rockType.
        /// </summary>
        private string rockType;

        /// <summary>
        /// Defines the selectedStratum.
        /// </summary>
        private string selectedStratum;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="B1PartDescViewModel"/> class.
        /// </summary>
        public B1PartDescViewModel()
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
            this.WeatheredStatus.Add(false);
            this.WeatheredStatus.Add(false);
            this.WeatheredStatus.Add(false);
            this.WeatheredStatus.Add(false);
            this.WeatheredStatus.Add(false);
            this.WeatheredStatus.Add(false);
            this.StiffnessStatus.Add(false);
            this.StiffnessStatus.Add(false);
            this.StiffnessStatus.Add(false);
            this.StiffnessStatus.Add(false);
            this.StiffnessStatus.Add(false);
            this.Joints.Add(null);
            this.Joints.Add(null);
            this.Joints.Add(null);
            this.Joints.Add(null);
            this.Joints.Add(null);
            this.Joints.Add(null);
            this.RoughStatus.Add(false);
            this.RoughStatus.Add(false);
            this.RoughStatus.Add(false);
            this.RoughStatus.Add(false);
            this.RoughStatus.Add(false);
            this.RoughStatus.Add(false);
            this.RoughStatus.Add(false);
            this.RoughStatus.Add(false);
            this.RoughStatus.Add(false);

            this.WetStatus.CollectionChanged += (s, e) => { propertyChanged(eDescriptionKey.Wet, s); };
            this.DensityStatus.CollectionChanged += (s, e) => { propertyChanged(eDescriptionKey.Density, s); };
            this.WeatheredStatus.CollectionChanged += (s, e) => { propertyChanged(eDescriptionKey.Weathered, s); };
            this.StiffnessStatus.CollectionChanged += (s, e) => { propertyChanged(eDescriptionKey.Stiffness, s); };
            this.Joints.CollectionChanged += (s, e) => { propertyChanged(eDescriptionKey.Joint, s); };
            this.RoughStatus.CollectionChanged += (s, e) => { propertyChanged(eDescriptionKey.Rough, s); };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the DensityStatus.
        /// </summary>
        public ObservableCollection<bool> DensityStatus { get; set; } = new ObservableCollection<bool>();

        /// <summary>
        /// Gets or sets a value indicating whether IsEnabled.
        /// </summary>
        public bool IsEnabled
        {
            get { return isEnabled; }
            set { SetValue(ref isEnabled, value); }
        }

        /// <summary>
        /// Gets or sets the IsSand.
        /// </summary>
        public bool? IsSand
        {
            get { return isSand; }
            set { SetValue(ref isSand, value); }
        }

        /// <summary>
        /// Gets or sets the Joints.
        /// </summary>
        public ObservableCollection<int?> Joints { get; set; } = new ObservableCollection<int?>();

        /// <summary>
        /// Gets or sets the Opacity.
        /// </summary>
        public double Opacity
        {
            get { return opacity; }
            set { SetValue(ref opacity, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether PartialJoint.
        /// </summary>
        public bool PartialJoint
        {
            get { return partialJoint; }
            set
            {
                SetValue(ref partialJoint, value);
                this.propertyChanged(eDescriptionKey.Joint, value);
            }
        }

        /// <summary>
        /// Gets or sets the RockType.
        /// </summary>
        public string RockType
        {
            get { return rockType; }
            set
            {
                SetValue(ref rockType, value);
                this.propertyChanged(eDescriptionKey.RockType, value);
            }
        }

        /// <summary>
        /// Gets or sets the RoughStatus.
        /// </summary>
        public ObservableCollection<bool> RoughStatus { get; set; } = new ObservableCollection<bool>();

        /// <summary>
        /// Gets or sets the SelectedStratum.
        /// </summary>
        public string SelectedStratum
        {
            get { return selectedStratum; }
            set
            {
                SetValue(ref selectedStratum, value);
                Update_STRA();
            }
        }

        /// <summary>
        /// Gets or sets the StiffnessStatus.
        /// </summary>
        public ObservableCollection<bool> StiffnessStatus { get; set; } = new ObservableCollection<bool>();

        /// <summary>
        /// Gets or sets the WeatheredStatus.
        /// </summary>
        public ObservableCollection<bool> WeatheredStatus { get; set; } = new ObservableCollection<bool>();

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
                case TRANSACTION_STATE.USER:
                    {
                        this.InitData();
                        if (0 == string.Compare(trData.strName, "B1PartDesc"))
                        {
                            string valkey;

                            // 디비키 추출
                            valkey = CmdParmParser.GetValuefromKey(trData.strUserList, "key");
                            if (string.IsNullOrEmpty(valkey))
                                break;
                            uint ukey = 0;
                            if (!uint.TryParse(valkey, out ukey))
                                break;

                            // 지층키 추출
                            valkey = CmdParmParser.GetValuefromKey(trData.strUserList, "strakey");
                            if (string.IsNullOrEmpty(valkey))
                                break;
                            uint ukeyStra = 0;
                            if (!uint.TryParse(valkey, out ukeyStra))
                                break;

                            DBDoc doc = DBDoc.Get_CurrDoc();

                            // 데이터가져오기
                            DBDataDESC dataDESC = null;
                            if (!doc.desc.Get_Data(ukey, ref dataDESC))
                                break;

                            // 데이터 화면에 업데이트
                            dbKey = ukey;
                            this.IsEnabled = true;
                            this.Opacity = 1;
                            dbKeyStra = ukeyStra;
                            this.updateSelf = true;

                            // DB에서 지층 type(stratum)를 가져온다.
                            DBDataSTRA straD = null;
                            if (doc.stra.Get_Data(ukeyStra, ref straD))
                            {
                                SelectedStratum = straD.stratumName;
                                if (straD.soilType.Equals(eSoil.None))
                                {
                                    this.IsSand = null;
                                }
                                else if (straD.soilType.Equals(eSoil.srock) || straD.soilType.Equals(eSoil.hrock))
                                {
                                    this.IsSand = false;
                                }
                                else
                                {
                                    this.IsSand = true;
                                }
                            }

                            foreach (var keyValue in dataDESC.dicDesc)
                            {
                                switch (keyValue.Key)
                                {
                                    case eDescriptionKey.None/*eDescriptionKey.Stratum*/:
                                        {
                                            SelectedStratum = dataDESC.dicDesc[keyValue.Key];
                                            break;
                                        }
                                    case eDescriptionKey.Weathered:
                                    case eDescriptionKey.Wet:
                                    case eDescriptionKey.Density:
                                    case eDescriptionKey.Stiffness:
                                        {
                                            List<int> tmp = new List<int>();
                                            foreach (var item in dataDESC.dicDesc[keyValue.Key].Split(GaiaConstants.FIRST_DELIMITER))
                                            {
                                                if (!item.Equals(string.Empty))
                                                {
                                                    tmp.Add(KeyControlPair.Instance.CheckBoxes.FindAll(x => x.Item1.Equals(keyValue.Key)).Find(x => x.Item3.Equals(item)).Item2);
                                                }
                                            }

                                            ObservableCollection<bool> targetList = null;

                                            switch (keyValue.Key)
                                            {
                                                case eDescriptionKey.Weathered:
                                                    {
                                                        targetList = this.WeatheredStatus;
                                                        break;
                                                    }
                                                case eDescriptionKey.Stiffness:
                                                    {
                                                        targetList = this.StiffnessStatus;
                                                        break;
                                                    }
                                                case eDescriptionKey.Wet:
                                                    {
                                                        targetList = this.WetStatus;
                                                        break;
                                                    }
                                                case eDescriptionKey.Density:
                                                    {
                                                        targetList = this.DensityStatus;
                                                        break;
                                                    }
                                            }

                                            for (int i = 0; i < targetList.Count; i++)
                                            {
                                                if (tmp.Contains(i))
                                                {
                                                    targetList[i] = true;
                                                }
                                                else
                                                {
                                                    targetList[i] = false;
                                                }
                                            }
                                            break;
                                        }
                                    case eDescriptionKey.RockType:
                                        {
                                            this.RockType = dataDESC.dicDesc[keyValue.Key];
                                            break;
                                        }
                                    case eDescriptionKey.Joint:
                                        {
                                            string[] tmp1 = dataDESC.dicDesc[keyValue.Key].Split(GaiaConstants.SECOND_DELIMITER);
                                            string[] tmp2 = tmp1[0].Split(GaiaConstants.FIRST_DELIMITER);

                                            this.PartialJoint = bool.Parse(tmp1[1]);

                                            for (int i = 0; i < tmp2.Length; i++)
                                            {
                                                if (tmp2[i].Equals(string.Empty))
                                                {
                                                    this.Joints[i] = null;
                                                }
                                                else
                                                {
                                                    this.Joints[i] = int.Parse(tmp2[i]);
                                                }
                                            }
                                            break;
                                        }
                                    case eDescriptionKey.Rough:
                                        {
                                            List<int> tmp = new List<int>();
                                            foreach (var item in dataDESC.dicDesc[keyValue.Key].Split(GaiaConstants.TILDE))
                                            {
                                                if (!item.Equals(string.Empty))
                                                {
                                                    tmp.Add(KeyControlPair.Instance.CheckBoxes.FindAll(x => x.Item1.Equals(keyValue.Key)).Find(x => x.Item3.Equals(item)).Item2);
                                                }
                                            }

                                            for (int i = 0; i < this.RoughStatus.Count; i++)
                                            {
                                                if (tmp.Contains(i))
                                                {
                                                    this.RoughStatus[i] = true;
                                                }
                                                else
                                                {
                                                    this.RoughStatus[i] = false;
                                                }
                                            }
                                            break;
                                        }
                                }
                            }

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
            }
        }

        /// <summary>
        /// The EndInit.
        /// </summary>
        public override void EndInit()
        {
            App.GetViewModelManager().AddValue(typeof(B1PartDescViewModel), this);
            TransactionCtrl.Add_DBUpdateWndCtrl(this); // IDBUpdate를 상속받은 경우 필히 연결시켜줍니다.
        }

        /// <summary>
        /// The initData.
        /// </summary>
        public void InitData()
        {
            this.updateSelf = true;
            for (int i = 0; i < this.DensityStatus.Count; i++)
            {
                this.DensityStatus[i] = false;
            }
            for (int i = 0; i < this.Joints.Count; i++)
            {
                this.Joints[i] = null;
            }
            for (int i = 0; i < this.RoughStatus.Count; i++)
            {
                this.RoughStatus[i] = false;
            }
            for (int i = 0; i < this.StiffnessStatus.Count; i++)
            {
                this.StiffnessStatus[i] = false;
            }
            for (int i = 0; i < this.WeatheredStatus.Count; i++)
            {
                this.WeatheredStatus[i] = false;
            }
            for (int i = 0; i < this.WetStatus.Count; i++)
            {
                this.WetStatus[i] = false;
            }
            this.SelectedStratum = string.Empty;
            this.PartialJoint = false;
            this.RockType = string.Empty;
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
        /// The onChanged.
        /// </summary>
        /// <param name="key">The key<see cref="eDescriptionKey"/>.</param>
        /// <param name="value">The value<see cref="string"/>.</param>
        protected void propertyChanged(eDescriptionKey key, object value)
        {
            if (!this.updateSelf)
            {
                // ※ 디키키 = 0 이면 추가하고, 디비키 > 0 이면 업데이트할 것, dbKeyStra 사용할 것
                if (dbKey != 0)
                {
                    var doc = DBDoc.Get_CurrDoc();
                    try
                    {
                        DBDataDESC dbDataDESC = null;

                        if (!doc.desc.Get_Data(dbKey, ref dbDataDESC))
                        {
                            dbDataDESC = new DBDataDESC();
                            dbDataDESC.straKey = dbKeyStra;
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
                            case eDescriptionKey.RockType:
                                {
                                    data = value.ToString();
                                    break;
                                }
                            case eDescriptionKey.Joint:
                                {
                                    //절리각이 변경된 경우
                                    if (value is ObservableCollection<int?>)
                                    {
                                        ObservableCollection<int?> tmp = value as ObservableCollection<int?>;
                                        foreach (var item in tmp)
                                        {
                                            data += item + GaiaConstants.FIRST_DELIMITER.ToString();
                                        }
                                        data = data.Remove(data.Length - 1);
                                        data += GaiaConstants.SECOND_DELIMITER + this.PartialJoint.ToString();
                                    }
                                    //부분절리가 변경된 경우
                                    else
                                    {
                                        foreach (var item in this.Joints)
                                        {
                                            data += item + GaiaConstants.FIRST_DELIMITER.ToString();
                                        }
                                        data = data.Remove(data.Length - 1);
                                        data += GaiaConstants.SECOND_DELIMITER + value.ToString();
                                    }
                                    break;
                                }
                            case eDescriptionKey.Rough:
                                {
                                    ObservableCollection<bool> tmp = value as ObservableCollection<bool>;
                                    for (int i = 0; i < tmp.Count; i++)
                                    {
                                        if (tmp[i].Equals(false)) continue;
                                        string stmp = KeyControlPair.Instance.CheckBoxes.FindAll(x => x.Item1.Equals(key)).Find(x => x.Item2.Equals(i)).Item3;

                                        if (string.IsNullOrEmpty(data))
                                        {
                                            data = stmp;
                                        }
                                        else
                                        {
                                            data += GaiaConstants.TILDE + stmp.Split(GaiaConstants.FIRST_DELIMITER)[1];
                                        }
                                    }
                                }
                                break;
                            case eDescriptionKey.Weathered:
                            case eDescriptionKey.Stiffness:
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

                        dbDataDESC.dicDesc[key] = data;

                        doc.desc.Modify_TR(dbKey, dbDataDESC);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        private void Update_STRA()
        {
            if (!this.updateSelf)
            {
                // DB에 지층 type를 저장한다.
                DBDoc doc = DBDoc.Get_CurrDoc();
                DBDataSTRA straD = null;
                if (doc.stra.Get_Data(dbKeyStra, ref straD))
                {
                    straD.stratumName = SelectedStratum;

                    eSoil soil = EnumUtil.GetEnumFromDescription<eSoil>(SelectedStratum);
                    if (soil.Equals(eSoil.wrock) || soil.Equals(eSoil.hrock) || soil.Equals(eSoil.srock))
                    {
                        straD.soilType = soil;
                    }
                    else
                    {
                        if (straD.soilType.Equals(eSoil.wrock) || straD.soilType.Equals(eSoil.srock) || straD.soilType.Equals(eSoil.hrock))
                        {
                            straD.soilType = eSoil.None;
                        }
                    }

                    if (doc.stra.Modify_TR(dbKeyStra, straD))
                    { }
                }
            }
        }

        #endregion
    }
}
