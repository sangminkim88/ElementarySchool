namespace GAIA2020.Design
{
    using HMFrameWork.Ancestor;

    /// <summary>
    /// Defines the <see cref="B1PartDescView" />.
    /// </summary>
    public partial class B1PartDescView : AUserControl
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="B1PartDescView"/> class.
        /// </summary>
        public B1PartDescView()
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
            App.GetViewManager().AddValue(typeof(B1PartDescView), this);
        }

        #endregion
    }
}
