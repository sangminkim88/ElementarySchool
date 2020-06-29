namespace GAIA2020.Design
{
    using GAIA2020.Utilities;
    using GaiaDB;
    using GaiaDB.Enums;
    using HmDataDocument;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using CmdParmParser = LogStyle.CmdParmParser;

    /// <summary>
    /// Defines the <see cref="B2PartDescViewModel" />.
    /// </summary>
    public class B2PartDescViewModel : B1PartDescViewModel, IDBUpdate
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="B2PartDescViewModel"/> class.
        /// </summary>
        public B2PartDescViewModel()
        {
            this.GapStatus.Add(false);
            this.GapStatus.Add(false);
            this.GapStatus.Add(false);
            this.GapStatus.Add(false);
            this.GapStatus.Add(false);
            this.GapStatus.CollectionChanged += (s, e) => { propertyChanged(eDescriptionKey.Gap, s); };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the GapStatus.
        /// </summary>
        public ObservableCollection<bool> GapStatus { get; set; } = new ObservableCollection<bool>();

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
        public new void DBUpdate(TransactionData trData, bool bUndo)
        {
            switch (trData.state)
            {
                case TRANSACTION_STATE.USER:
                    {
                        this.InitData();
                        if (0 == string.Compare(trData.strName, "B2PartDesc"))
                        {
                            string valkey = CmdParmParser.GetValuefromKey(trData.strUserList, "key");
                            if (string.IsNullOrEmpty(valkey))
                                break;
                            // 디비키 추출
                            uint ukey = 0;
                            if (!uint.TryParse(valkey, out ukey))
                                break;
                            // 데이터가져오기
                            DBDataDESC dataDESC = null;
                            DBDoc doc = DBDoc.Get_CurrDoc();
                            if (!doc.desc.Get_Data(ukey, ref dataDESC))
                                break;

                            // 데이터 화면에 업데이트
                            if (dataDESC.dicDesc.ContainsKey(eDescriptionKey.Gap))
                            {
                                this.updateSelf = true;

                                List<int> tmp = new List<int>();
                                foreach (var item in dataDESC.dicDesc[eDescriptionKey.Gap].Split(GaiaConstants.FIRST_DELIMITER))
                                {
                                    if (!item.Equals(string.Empty))
                                    {
                                        tmp.Add(KeyControlPair.Instance.CheckBoxes.FindAll(x => x.Item1.Equals(eDescriptionKey.Gap)).Find(x => x.Item3.Equals(item)).Item2);
                                    }
                                }

                                for (int i = 0; i < this.GapStatus.Count; i++)
                                {
                                    if (tmp.Contains(i))
                                    {
                                        this.GapStatus[i] = true;
                                    }
                                    else
                                    {
                                        this.GapStatus[i] = false;
                                    }
                                }
                                this.updateSelf = false;
                            }

                            trData.strName = "B1PartDesc";
                            base.DBUpdate(trData, bUndo);
                        }
                        else if (trData.strName.Equals("ActiveDrillLog") || trData.strName.Equals("ActiveDrillStyle"))
                        {
                            dbKey = 0;
                            this.IsEnabled = false;
                            this.Opacity = .5;
                        }
                        break;
                    }
            }
        }

        /// <summary>
        /// The EndInit.
        /// </summary>
        public override void EndInit()
        {
            App.GetViewModelManager().AddValue(typeof(B2PartDescViewModel), this);
            TransactionCtrl.Add_DBUpdateWndCtrl(this); // IDBUpdate를 상속받은 경우 필히 연결시켜줍니다. 
        }

        /// <summary>
        /// The initData.
        /// </summary>
        public new void InitData()
        {
            this.updateSelf = true;
            for (int i = 0; i < this.GapStatus.Count; i++)
            {
                this.GapStatus[i] = false;
            }
            this.updateSelf = false;
            base.InitData();
        }

        /// <summary>
        /// The onChanged.
        /// </summary>
        /// <param name="key">The key<see cref="eDescriptionKey"/>.</param>
        /// <param name="value">The value<see cref="string"/>.</param>
        protected new void propertyChanged(eDescriptionKey key, object value)
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

                        if (key.Equals(eDescriptionKey.Gap))
                        {
                            string data = string.Empty;
                            ObservableCollection<bool> tmp = value as ObservableCollection<bool>;
                            for (int i = 0; i < tmp.Count; i++)
                            {
                                if (tmp[i].Equals(false)) continue;
                                data += KeyControlPair.Instance.CheckBoxes.FindAll(x => x.Item1.Equals(key)).Find(x => x.Item2.Equals(i)).Item3 + GaiaConstants.FIRST_DELIMITER;
                            }

                            dbDataDESC.dicDesc[key] = data.Length > 0 ? data.Substring(0, data.Length - 1) : data;

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
