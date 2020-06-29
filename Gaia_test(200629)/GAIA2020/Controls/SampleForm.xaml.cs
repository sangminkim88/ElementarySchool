namespace GAIA2020.Controls
{
    using GaiaDB.Enums;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Defines the <see cref="SampleForm" />.
    /// </summary>
    public partial class SampleForm : UserControl
    {
        #region Fields

        /// <summary>
        /// Defines the SelectedSampleProperty.
        /// </summary>
        public static readonly DependencyProperty SelectedSampleProperty =
           DependencyProperty.Register("SelectedSample", typeof(eSampleType), typeof(SampleForm), new UIPropertyMetadata(eSampleType.None, selectedSampleChanged));

        /// <summary>
        /// Defines the oldCanvas.
        /// </summary>
        private Canvas oldCanvas;

        /// <summary>
        /// Defines the radioButtons.
        /// </summary>
        private List<RadioButton> radioButtons = new List<RadioButton>();

        /// <summary>
        /// Defines the updateSelf.
        /// </summary>
        private bool updateSelf;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleForm"/> class.
        /// </summary>
        public SampleForm()
        {
            InitializeComponent();
            this.radioButtons.Add(this.sample1);
            this.radioButtons.Add(this.sample2);
            this.radioButtons.Add(this.sample3);
            this.radioButtons.Add(this.sample4);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the SelectedSample.
        /// </summary>
        public eSampleType SelectedSample
        {
            get { return (eSampleType)GetValue(SelectedSampleProperty); }
            set { SetValue(SelectedSampleProperty, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The selectedSampleChanged.
        /// </summary>
        /// <param name="d">The d<see cref="DependencyObject"/>.</param>
        /// <param name="e">The e<see cref="DependencyPropertyChangedEventArgs"/>.</param>
        private static void selectedSampleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SampleForm sampleForm = d as SampleForm;
            if (!sampleForm.updateSelf)
            {
                sampleForm.SetSelectedSampleForm((eSampleType)e.NewValue);
            }
        }

        /// <summary>
        /// The GetSelectedSampleForm.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public eSampleType GetSelectedSampleForm()
        {
            switch (this.radioButtons.IndexOf(this.radioButtons.Find(x => x.IsChecked.Equals(true))))
            {
                case 0:
                    return eSampleType.UD;
                case 1:
                    return eSampleType.DS;
                case 2:
                    return eSampleType.NS;
                case 3:
                    return eSampleType.CS;
                default:
                    return eSampleType.None;
            }
        }

        /// <summary>
        /// The SetSelectedSampleForm.
        /// </summary>
        /// <param name="code">The code<see cref="string"/>.</param>
        public void SetSelectedSampleForm(eSampleType code)
        {
            Canvas canvas;
            switch (code)
            {
                case eSampleType.UD:
                    canvas = this.radioButtons[0].Content as Canvas;
                    break;
                case eSampleType.DS:
                    canvas = this.radioButtons[1].Content as Canvas;
                    break;
                case eSampleType.NS:
                    canvas = this.radioButtons[2].Content as Canvas;
                    break;
                case eSampleType.CS:
                    canvas = this.radioButtons[3].Content as Canvas;
                    break;
                case eSampleType.None:
                    canvas = null;
                    break;
                default:
                    return;
            }
            this.Canvas_MouseDown(canvas, null);
        }

        /// <summary>
        /// The Canvas_MouseDown.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.Windows.Input.MouseButtonEventArgs"/>.</param>
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.oldCanvas != null)
            {
                this.oldCanvas.Background = new SolidColorBrush(Colors.Transparent);
            }

            if (sender == null)
            {
                this.radioButtons.ForEach(x => x.IsChecked = false);
            }
            else
            {
                Canvas canvas = sender as Canvas;
                this.oldCanvas = canvas;
                canvas.Background = FindResource("Theme.Color1") as SolidColorBrush;
                RadioButton radioButton = canvas.Parent as RadioButton;
                radioButton.IsChecked = true;
            }
            this.updateSelf = true;
            this.SelectedSample = this.GetSelectedSampleForm();
            this.updateSelf = false;
        }

        /// <summary>
        /// The UserControl_IsEnabledChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DependencyPropertyChangedEventArgs"/>.</param>
        private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                this.border.IsEnabled = true;
                this.border.Opacity = 1;
            }
            else
            {
                this.border.IsEnabled = false;
                this.border.Opacity = .5;
            }
        }

        #endregion
    }
}
