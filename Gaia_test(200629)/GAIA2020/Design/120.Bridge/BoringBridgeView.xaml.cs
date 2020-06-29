namespace GAIA2020.Design
{
    using HMFrameWork.Ancestor;

    public partial class BoringBridgeView : AUserControl
    {
        #region Constructors

        public BoringBridgeView()
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
            App.GetViewManager().AddValue(typeof(BoringBridgeView), this);
        }

        #endregion
    }
}
