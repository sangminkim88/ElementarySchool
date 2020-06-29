namespace GAIA2020.Design
{
    using HMFrameWork.Ancestor;

    public partial class BoringBorrowPitView : AUserControl
    {
        #region Constructors

        public BoringBorrowPitView()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            App.GetViewManager().AddValue(typeof(BoringBorrowPitView), this);
        }

        #endregion
    }
}
