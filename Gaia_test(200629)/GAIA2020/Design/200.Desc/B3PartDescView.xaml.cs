namespace GAIA2020.Design
{
    using HMFrameWork.Ancestor;

    public partial class B3PartDescView : AUserControl
    {
        #region Constructors

        public B3PartDescView()
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
            App.GetViewManager().AddValue(typeof(B3PartDescView), this);
        }

        #endregion
    }
}
