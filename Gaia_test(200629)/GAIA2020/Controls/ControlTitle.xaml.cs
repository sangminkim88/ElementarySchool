namespace GAIA2020.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Defines the <see cref="ControlTitle" />.
    /// </summary>
    public partial class ControlTitle : UserControl
    {
        #region Fields

        /// <summary>
        /// Defines the CodeProperty.
        /// </summary>
        public static readonly DependencyProperty CodeProperty =
           DependencyProperty.Register("Code", typeof(string), typeof(ControlTitle), new UIPropertyMetadata(default(string)));

        /// <summary>
        /// Defines the TitleProperty.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
           DependencyProperty.Register("Title", typeof(string), typeof(ControlTitle), new UIPropertyMetadata(default(string)));

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlTitle"/> class.
        /// </summary>
        public ControlTitle()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Code.
        /// </summary>
        public string Code
        {
            get { return (string)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        #endregion
    }
}
