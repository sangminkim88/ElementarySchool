namespace PieControls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;

    public partial class PieControl : UserControl
    {
        #region Fields

        public static readonly DependencyProperty PopupBrushProperty = DependencyProperty.Register("PopupBrush", typeof(Brush), typeof(PieControl));

        internal Dictionary<Path, PieSegment> pathDictionary = new Dictionary<Path, PieSegment>();

        internal ObservableCollection<PieSegment> values;

        #endregion

        #region Constructors

        public PieControl()
        {
            DataContext = this;
            PopupBrush = Brushes.Orange;
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
                values.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(values_CollectionChanged);
                foreach (var v in values)
                {
                    v.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(pieSegment_PropertyChanged);
                }
                ResetPie();
            }
        }

        public Brush PopupBrush
        {
            get
            {
                return (Brush)GetValue(PopupBrushProperty);
            }
            set
            {
                SetValue(PopupBrushProperty, value);
            }
        }

        #endregion

        #region Methods

        internal void AddPathToDictionary(Path path, PieSegment ps)
        {
            pathDictionary.Add(path, ps);
            path.MouseEnter += new MouseEventHandler(Path_MouseEnter);
            path.MouseMove += new MouseEventHandler(Path_MouseMove);
        }

        internal void ClearPathDictionary()
        {
            foreach (Path path in pathDictionary.Keys)
            {
                path.MouseEnter -= Path_MouseEnter;
                path.MouseMove -= Path_MouseMove;
            }
            pathDictionary.Clear();
        }

        internal void CreatePiePathAndGeometries()
        {
            ClearPathDictionary();
            drawingCanvas.Children.Clear();
            drawingCanvas.Children.Add(piePopup);
            if (Data != null)
            {
                double total = Data.Sum(x => x.Value);
                if (total > 0)
                {
                    double angle = -Math.PI / 2;
                    foreach (PieSegment ps in Data)
                    {
                        Geometry geometry;
                        Path path = new Path();
                        if (ps.Value == total)
                        {
                            geometry = new EllipseGeometry(new Point(this.Width / 2, this.Height / 2), this.Width / 2, this.Height / 2);
                        }
                        else
                        {
                            geometry = new PathGeometry();
                            double x = Math.Cos(angle) * Width / 2 + Width / 2;
                            double y = Math.Sin(angle) * Height / 2 + Height / 2;
                            LineSegment lingeSeg = new LineSegment(new Point(x, y), true);
                            double angleShare = (ps.Value / total) * 360;
                            angle += DegreeToRadian(angleShare);
                            x = Math.Cos(angle) * Width / 2 + Width / 2;
                            y = Math.Sin(angle) * Height / 2 + Height / 2;
                            ArcSegment arcSeg = new ArcSegment(new Point(x, y), new Size(Width / 2, Height / 2), angleShare, angleShare > 180, SweepDirection.Clockwise, false);
                            LineSegment lingeSeg2 = new LineSegment(new Point(Width / 2, Height / 2), true);
                            PathFigure fig = new PathFigure(new Point(Width / 2, Height / 2), new PathSegment[] { lingeSeg, arcSeg, lingeSeg2 }, false);
                            ((PathGeometry)geometry).Figures.Add(fig);
                        }
                        path.Fill = ps.GradientBrush;
                        path.Data = geometry;
                        AddPathToDictionary(path, ps);
                        drawingCanvas.Children.Add(path);
                    }
                }
            }
        }

        internal void Path_MouseEnter(object sender, MouseEventArgs e)
        {
            piePopup.Visibility = System.Windows.Visibility.Visible;
            PieSegment seg = pathDictionary[sender as Path];
            popupData.Text = seg.Name + " : " + ((seg.Value / Data.Sum(x => x.Value)) * 100).ToString("N2") + "%";
            Point point = Mouse.GetPosition(this);
            piePopup.Margin = new Thickness(point.X - piePopup.ActualWidth / 4, point.Y - (18 + piePopup.ActualHeight), 0, 0);
        }

        internal void Path_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = Mouse.GetPosition(this);
            piePopup.Margin = new Thickness(point.X - piePopup.ActualWidth / 4, point.Y - (18 + piePopup.ActualHeight), 0, 0);
        }

        internal void pieSegment_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            ResetPie();
        }

        internal void ResetPie()
        {
            Dispatcher.Invoke(new Action(CreatePiePathAndGeometries));
        }

        internal void values_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ResetPie();
        }

        private double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private void PieControl_MouseLeave(object sender, MouseEventArgs e)
        {
            piePopup.Visibility = System.Windows.Visibility.Collapsed;
        }

        private double RadianToDegree(double angle)
        {
            return angle * (180.0 / Math.PI);
        }

        #endregion
    }
}
