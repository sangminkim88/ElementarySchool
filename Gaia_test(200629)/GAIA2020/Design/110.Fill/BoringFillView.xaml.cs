namespace GAIA2020.Design
{
    using HMFrameWork.Ancestor;

    public partial class BoringFillView : AUserControl
    {
        #region Constructors

        public BoringFillView()
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
            App.GetViewManager().AddValue(typeof(BoringFillView), this);
        }

        #endregion
    }
}
