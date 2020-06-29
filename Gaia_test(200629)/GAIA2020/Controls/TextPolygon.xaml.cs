namespace GAIA2020.Controls
{
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Defines the <see cref="TextPolygon" />.
    /// </summary>
    public partial class TextPolygon : UserControl, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// Defines the id.
        /// </summary>
        private int id;

        /// <summary>
        /// Defines the isSelectable.
        /// </summary>
        private bool isSelectable = true;

        /// <summary>
        /// Defines the isSelected.
        /// </summary>
        private bool? isSelected;

        /// <summary>
        /// Defines the number.
        /// </summary>
        private int number;

        /// <summary>
        /// Defines the pathFill.
        /// </summary>
        private Brush pathFill;

        /// <summary>
        /// Defines the polygonText.
        /// </summary>
        private string polygonText;

        /// <summary>
        /// Defines the textColor.
        /// </summary>
        private Brush textColor = new SolidColorBrush(Colors.Black);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TextPolygon"/> class.
        /// </summary>
        public TextPolygon()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        /// <summary>
        /// Defines the PropertyChanged.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public int ID
        {
            get { return id; }
            set { this.id = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsSelectable.
        /// </summary>
        public bool IsSelectable
        {
            get { return isSelectable; }
            set { isSelectable = value; }
        }

        /// <summary>
        /// Gets or sets the IsSelected.
        /// </summary>
        public bool? IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        /// <summary>
        /// Gets or sets the Number.
        /// </summary>
        public int Number
        {
            get { return number; }
            set { number = value; }
        }

        /// <summary>
        /// Gets or sets the PathData.
        /// </summary>
        public Geometry PathData
        {
            get { return this.path.Data; }
            set { this.path.Data = value; }
        }

        /// <summary>
        /// Gets or sets the PathFill.
        /// </summary>
        public Brush PathFill
        {
            get { return this.pathFill; }
            set
            {
                this.pathFill = value;
                OnPropertyChanged("PathFill");
            }
        }

        /// <summary>
        /// Gets or sets the PolygonText.
        /// </summary>
        public string PolygonText
        {
            get { return polygonText; }
            set
            {
                polygonText = value;
                OnPropertyChanged("PolygonText");
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The OnPropertyChanged.
        /// </summary>
        /// <param name="propertyName">The propertyName<see cref="string"/>.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// The MeasureString.
        /// </summary>
        /// <param name="candidate">The candidate<see cref="string"/>.</param>
        /// <returns>The <see cref="Size"/>.</returns>
        private Size MeasureString(string candidate)
        {
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(this.textBlock.FontFamily, this.textBlock.FontStyle, this.textBlock.FontWeight, this.textBlock.FontStretch),
                this.textBlock.FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                TextFormattingMode.Display);

            return new Size(formattedText.Width, formattedText.Height);
        }

        /// <summary>
        /// The Path_Loaded.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void Path_Loaded(object sender, RoutedEventArgs e)
        {
            Rect rect = VisualTreeHelper.GetDescendantBounds(this.path);

            Size textSize = this.MeasureString(this.polygonText);

            double x = rect.Right - rect.Width / 2 - textSize.Width / 2;
            double y = rect.Bottom - rect.Height / 2 - textSize.Height / 2;

            string text = this.polygonText;
            if (text.Equals("적갈색") || text.Equals("황적색") || text.Equals("녹황색") || text.Equals("청녹색")
                || text.Equals("담청색") || text.Equals("담회색") || text.Equals("암회색"))
            {
                y -= 15;
            }
            else if (text.Equals("갈 색") || text.Equals("적 색") || text.Equals("황 색") || text.Equals("녹 색")
                || text.Equals("청 색") || text.Equals("백 색") || text.Equals("회 색") || text.Equals("흑 색"))
            {
                y += 5;
            }

            this.textBlock.Margin = new Thickness(x, y, 0, 0);
        }

        #endregion
    }
}
