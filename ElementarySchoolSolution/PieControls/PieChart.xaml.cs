namespace PieControls
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public partial class PieChart : UserControl
    {
        #region Fields

        public ObservableCollection<PieSegment> values;

        #endregion

        #region Constructors

        public PieChart()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        public ObservableCollection<PieSegment> Data
        {
            get { return values; }
            set
            {
                values = value;
                Pie.Data = value;
                foreach (var v in values)
                {
                    v.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(PieSegment_PropertyChanged);
                }
                Dispatcher.Invoke(new Action(() => { InvalidateVisual(); }));
            }
        }

        public double PieHeight
        {
            get { return Pie.Height; }
            set { Pie.Height = value; }
        }

        public double PieWidth
        {
            get { return Pie.Width; }
            set { Pie.Width = value; }
        }

        public Brush PopupBrush
        {
            get { return Pie.PopupBrush; }
            set { Pie.PopupBrush = value; }
        }

        #endregion

        #region Methods

        public FormattedText GetFormattedText(string textToFormat, double fontSize, Brush brush)
        {
            Typeface typeface = new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            return new FormattedText(textToFormat, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, typeface, fontSize, brush);
        }

        internal void PieSegment_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Dispatcher.Invoke(new Action(() => { InvalidateVisual(); }));
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (values != null)
            {
                double height = values.Count * 20;
                double top = (Height - height) / 2;
                foreach (PieSegment ps in values)
                {
                    dc.DrawRectangle(ps.SolidBrush, null, new Rect(Pie.Width + 10, top, 8, 8));
                    dc.DrawText(GetFormattedText(ps.Name + " (" + ps.Value + ")", 12, Brushes.Black), new Point(Pie.Width + 20, top));
                    top += 20;
                }
            }
        }

        #endregion
    }
}
