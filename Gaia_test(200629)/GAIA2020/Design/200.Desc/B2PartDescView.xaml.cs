namespace GAIA2020.Design
{
    using HMFrameWork.Ancestor;

    /// <summary>
    /// Defines the <see cref="B2PartDescView" />.
    /// </summary>
    public partial class B2PartDescView : AUserControl
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="B2PartDescView"/> class.
        /// </summary>
        public B2PartDescView()
        {
            InitializeComponent();
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
            App.GetViewManager().AddValue(typeof(B2PartDescView), this);
        }

        #endregion
    }
}
