namespace GAIA2020.Design
{
    using HMFrameWork.Ancestor;

    public partial class BoringCutView : AUserControl
    {
        #region Constructors

        public BoringCutView()
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
            App.GetViewManager().AddValue(typeof(BoringCutView), this);
        }

        #endregion
    }
}
