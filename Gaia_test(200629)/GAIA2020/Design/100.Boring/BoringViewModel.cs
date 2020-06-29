namespace GAIA2020.Design
{
    using GAIA2020.Models;
    using GaiaDB;
    using HmDataDocument;
    using HMFrameWork.Ancestor;
    using System.Collections.ObjectModel;
    using System.Windows;

    /// <summary>
    /// Defines the <see cref="BoringViewModel" />.
    /// </summary>
    public partial class BoringViewModel : AViewModel
    {
        #region Fields

        /// <summary>
        /// Defines the addButtonContent.
        /// </summary>
        private string addButtonContent;

        /// <summary>
        /// Defines the addDrillLogVisibility.
        /// </summary>
        private Visibility addDrillLogVisibility;

        /// <summary>
        /// Defines the drillLogCounts.
        /// </summary>
        private ObservableCollection<DrillProperty> drillLogCounts = new ObservableCollection<DrillProperty>();

        /// <summary>
        /// Defines the drillLogCountSelectedIndex.
        /// </summary>
        private int drillLogCountSelectedIndex;

        /// <summary>
        /// Defines the pageCounts.
        /// </summary>
        private ObservableCollection<int> pageCounts = new ObservableCollection<int>();

        /// <summary>
        /// Defines the pageCountSelectedIndex.
        /// </summary>
        private int pageCountSelectedIndex;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BoringViewModel"/> class.
        /// </summary>
        public BoringViewModel()
        {
            AddDrillLogVisibility = Visibility.Visible;
            AddButtonContent = "+";
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the AddButtonContent.
        /// </summary>
        public string AddButtonContent
        {
            get { return addButtonContent; }
            set { SetValue(ref addButtonContent, value); }
        }

        /// <summary>
        /// Gets or sets the AddDrillLogVisibility.
        /// </summary>
        public Visibility AddDrillLogVisibility
        {
            get { return addDrillLogVisibility; }
            set { SetValue(ref addDrillLogVisibility, value); }
        }

        /// <summary>
        /// Gets the DrillLogCounts
        /// Gets or sets the DrillLogCounts...
        /// </summary>
        public ObservableCollection<DrillProperty> DrillLogCounts { get => drillLogCounts; private set => drillLogCounts = value; }

        /// <summary>
        /// Gets or sets the DrillLogCountSelectedIndex.
        /// </summary>
        public int DrillLogCountSelectedIndex
        {
            get { return drillLogCountSelectedIndex; }
            set
            {
                SetValue(ref drillLogCountSelectedIndex, value);
                if (value > -1)
                {
                    DBDoc.Get_CurrDoc().Set_ActiveDrillLog(new HmGeometry.HmDBKey("DRLG", DrillLogCounts[value].Ukey, DrillLogCounts[value].Page));
                }
            }
        }

        /// <summary>
        /// Gets the PageCounts
        /// Gets or sets the PageCounts...
        /// </summary>
        public ObservableCollection<int> PageCounts { get => pageCounts; private set => pageCounts = value; }

        /// <summary>
        /// Gets or sets the PageCountSelectedIndex.
        /// </summary>
        public int PageCountSelectedIndex
        {
            get { return pageCountSelectedIndex; }
            set
            {
                SetValue(ref pageCountSelectedIndex, value);
                if (value > -1 && DrillLogCountSelectedIndex > -1)
                {
                    DBDoc.Get_CurrDoc().Set_ActiveDrillLog(new HmGeometry.HmDBKey("DRLG", DrillLogCounts[DrillLogCountSelectedIndex].Ukey, value + 1));
                }
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
        /// The EndInit.
        /// </summary>
        public override void EndInit()
        {
            App.GetViewModelManager().AddValue(typeof(BoringViewModel), this);
            TransactionCtrl.Add_DBUpdateWndCtrl(this); // IDBUpdate를 상속받은 경우 필히 연결시켜줍니다.
        }

        #endregion
    }
}
