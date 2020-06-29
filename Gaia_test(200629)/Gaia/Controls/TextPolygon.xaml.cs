namespace Gaia.Controls
{
    using System.ComponentModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    public partial class TextPolygon : Viewbox, INotifyPropertyChanged
    {
        #region Fields

        private bool isSelectable = true;

        private bool? isSelected;

        private Brush pathFill;

        private string polygonText;

        private Brush textColor = new SolidColorBrush(Colors.Black);

        #endregion

        #region Constructors

        public TextPolygon()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public bool IsSelectable
        {
            get { return isSelectable; }
            set { isSelectable = value; }
        }

        public bool? IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public Geometry PathData
        {
            get { return this.path.Data; }
            set { this.path.Data = value; }
        }

        public Brush PathFill
        {
            get { return this.pathFill; }
            set
            {
                this.pathFill = value;
                OnPropertyChanged("PathFill");
            }
        }

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

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Grid_Loaded(object sender, System.Windows.RoutedEventArgs e)
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

        private void Shape_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (this.isSelectable)
            {
                Path path = sender as Path;
                if (this.isSelected == true)
                {
                    if (this.PolygonText.Equals("흑 색"))
                    {
                        this.IsSelected = null;
                    }
                    else
                    {
                        this.IsSelected = false;
                    }
                }
                else
                {
                    this.IsSelected = true;
                }
            }
        }

        #endregion
    }
}
