namespace GAIA2020.Design
{
    using HMFrameWork.Ancestor;

    public partial class BoringTunnelView : AUserControl
    {
        #region Constructors

        public BoringTunnelView()
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
            App.GetViewManager().AddValue(typeof(BoringTunnelView), this);
        }

        #endregion
    }
}
