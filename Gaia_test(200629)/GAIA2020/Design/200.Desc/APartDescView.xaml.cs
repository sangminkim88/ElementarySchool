namespace GAIA2020.Design
{
    using HMFrameWork.Ancestor;

    public partial class APartDescView : AUserControl
    {
        #region Constructors

        public APartDescView()
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
            App.GetViewManager().AddValue(typeof(APartDescView), this);
        }

        #endregion
    }
}
