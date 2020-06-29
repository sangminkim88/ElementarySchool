namespace GAIA2020.Models
{
    using GaiaDB.Enums;
    using HMFrameWork.Ancestor;

    /// <summary>
    /// Defines the <see cref="DrillProperty" />.
    /// </summary>
    public class DrillProperty : ANotifyProperty
    {
        #region Fields

        /// <summary>
        /// Defines the drilltype.
        /// </summary>
        private eDepartment drilltype;

        /// <summary>
        /// Defines the page.
        /// </summary>
        private int page;

        /// <summary>
        /// Defines the title.
        /// </summary>
        private string title;

        /// <summary>
        /// Defines the ukey.
        /// </summary>
        private uint ukey;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DrillProperty"/> class.
        /// </summary>
        public DrillProperty()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Drilltype.
        /// </summary>
        public eDepartment Drilltype
        {
            get { return drilltype; }
            set { SetValue(ref drilltype, value); }
        }

        /// <summary>
        /// Gets or sets the Page.
        /// </summary>
        public int Page
        {
            get { return page; }
            set { SetValue(ref page, value); }
        }

        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public string Title
        {
            get { return title; }
            set { SetValue(ref title, value); }
        }

        /// <summary>
        /// Gets or sets the Ukey.
        /// </summary>
        public uint Ukey
        {
            get { return ukey; }
            set { SetValue(ref ukey, value); }
        }

        #endregion
    }
}
