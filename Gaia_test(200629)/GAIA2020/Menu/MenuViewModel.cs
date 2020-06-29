namespace GAIA2020.Menu
{
    using GAIA2020.Utilities;
    using GaiaDB.Enums;
    using HmDataDocument;
    using HMFrameWork.Ancestor;
    using HMFrameWork.Command;
    using System.Collections.Specialized;

    /// <summary>
    /// Defines the <see cref="MenuViewModel" />.
    /// </summary>
    public partial class MenuViewModel : AViewModel
    {
        #region Fields

        /// <summary>
        /// Defines the projectModel.
        /// </summary>
        //private ProjectModel projectModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuViewModel"/> class.
        /// </summary>
        public MenuViewModel()
        {
            CommandCheck = new RelayCommand(check);
            CommandOthers = new RelayCommand(others);
            foreach (var v in System.Enum.GetValues(typeof(eDepartment)))
            {
                DrillLogCounts.Add((eDepartment)v, 0);
            }
            DrillLogCounts.CollectionChanged += new NotifyCollectionChangedEventHandler(SelectedColorIdsCollectionChanged);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the DrillLogCounts.
        /// </summary>
        public ObservableDictionary<eDepartment, int> DrillLogCounts { get; set; } = new ObservableDictionary<eDepartment, int>();
        
        #endregion

        #region Methods

        /// <summary>
        /// The BeginInit.
        /// </summary>
        public override void BeginInit()
        {
        }

        /// <summary>
        /// The EndInit.
        /// </summary>
        public override void EndInit()
        {
            App.GetViewModelManager().AddValue(typeof(MenuViewModel), this);
            TransactionCtrl.Add_DBUpdateWndCtrl(this); // IDBUpdate를 상속받은 경우 필히 연결시켜줍니다.
        }

        /// <summary>
        /// The SelectedColorIdsCollectionChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="NotifyCollectionChangedEventArgs"/>.</param>
        private void SelectedColorIdsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyPropertyChanged("DrillLogCounts");
        }

        #endregion
    }
}
