namespace GAIA2020.Controls
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Defines the <see cref="SandColumn" />.
    /// </summary>
    public partial class SandColumn : UserControl
    {
        #region Fields

        /// <summary>
        /// Defines the DensityStatusProperty.
        /// </summary>
        public static readonly DependencyProperty DensityStatusProperty =
           DependencyProperty.Register("DensityStatus", typeof(ObservableCollection<bool>), typeof(SandColumn),
               new UIPropertyMetadata(default(ObservableCollection<bool>)));

        /// <summary>
        /// Defines the IsTrialPitOrHandAugerProperty.
        /// </summary>
        public static readonly DependencyProperty IsTrialPitOrHandAugerProperty =
           DependencyProperty.Register("IsTrialPitOrHandAuger", typeof(bool), typeof(SandColumn), new UIPropertyMetadata(default(bool), isTrialPitOrHandAugerChanged));

        /// <summary>
        /// Defines the SelectedStratumProperty.
        /// </summary>
        public static readonly DependencyProperty SelectedStratumProperty =
           DependencyProperty.Register("SelectedStratum", typeof(string), typeof(SandColumn),
               new UIPropertyMetadata(default(string)));

        /// <summary>
        /// Defines the WetStatusProperty.
        /// </summary>
        public static readonly DependencyProperty WetStatusProperty =
           DependencyProperty.Register("WetStatus", typeof(ObservableCollection<bool>), typeof(SandColumn),
               new UIPropertyMetadata(default(ObservableCollection<bool>)));

        /// <summary>
        /// Defines the sandyDensities.
        /// </summary>
        private List<CheckBox> sandyDensities = new List<CheckBox>();

        /// <summary>
        /// Defines the updateSelf.
        /// </summary>
        private bool updateSelf;

        /// <summary>
        /// Defines the viscosityDensities.
        /// </summary>
        private List<CheckBox> viscosityDensities = new List<CheckBox>();

        /// <summary>
        /// Defines the wets.
        /// </summary>
        private List<CheckBox> wets = new List<CheckBox>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SandColumn"/> class.
        /// </summary>
        public SandColumn()
        {
            InitializeComponent();
            this.wets.Add(this.wet0);
            this.wets.Add(this.wet1);
            this.wets.Add(this.wet2);
            this.wets.Add(this.wet3);
            this.sandyDensities.Add(this.sandyDensity0);
            this.sandyDensities.Add(this.sandyDensity1);
            this.sandyDensities.Add(this.sandyDensity2);
            this.sandyDensities.Add(this.sandyDensity3);
            this.sandyDensities.Add(this.sandyDensity4);
            this.viscosityDensities.Add(this.viscosityDensity0);
            this.viscosityDensities.Add(this.viscosityDensity1);
            this.viscosityDensities.Add(this.viscosityDensity2);
            this.viscosityDensities.Add(this.viscosityDensity3);
            this.viscosityDensities.Add(this.viscosityDensity4);
            this.viscosityDensities.Add(this.viscosityDensity5);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the DensityStatus.
        /// </summary>
        public ObservableCollection<bool> DensityStatus
        {
            get { return (ObservableCollection<bool>)GetValue(DensityStatusProperty); }
            set { SetValue(DensityStatusProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsTrialPitOrHandAuger.
        /// </summary>
        public bool IsTrialPitOrHandAuger
        {
            get { return (bool)GetValue(IsTrialPitOrHandAugerProperty); }
            set { SetValue(IsTrialPitOrHandAugerProperty, value); }
        }

        /// <summary>
        /// Gets or sets the SelectedStratum.
        /// </summary>
        public string SelectedStratum
        {
            get { return (string)GetValue(SelectedStratumProperty); }
            set { SetValue(SelectedStratumProperty, value); }
        }

        /// <summary>
        /// Gets or sets the WetStatus.
        /// </summary>
        public ObservableCollection<bool> WetStatus
        {
            get { return (ObservableCollection<bool>)GetValue(WetStatusProperty); }
            set { SetValue(WetStatusProperty, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The isTrialPitOrHandAugerChanged.
        /// </summary>
        /// <param name="d">The d<see cref="DependencyObject"/>.</param>
        /// <param name="e">The e<see cref="DependencyPropertyChangedEventArgs"/>.</param>
        private static void isTrialPitOrHandAugerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SandColumn thisObject = d as SandColumn;
            if ((bool)e.NewValue)
            {
                thisObject.viscosityDensityText0.Text = "손으로 누르면 손가락\n사이로 흙이 빠져나옴";
                thisObject.viscosityDensityText1.Text = "엄지손가락이 쉽게\n관입";
                thisObject.viscosityDensityText2.Text = "엄지손가락이 힘들게\n관입";
                thisObject.viscosityDensityText3.Text = "엄지손가락이 매우\n힘들게 관입";
                thisObject.viscosityDensityText4.Text = "엄지손가락의 손톱으로\n쉽게 자국이 남";
                thisObject.viscosityDensityText5.Text = "엄지손가락의 손톱으로\n힘들게 자국이 남";
                thisObject.viscosityDensityText0.FontSize = 7;
                thisObject.viscosityDensityText1.FontSize = 7;
                thisObject.viscosityDensityText2.FontSize = 7;
                thisObject.viscosityDensityText3.FontSize = 7;
                thisObject.viscosityDensityText4.FontSize = 7;
                thisObject.viscosityDensityText5.FontSize = 7;
            }
        }

        /// <summary>
        /// The enableByCount.
        /// </summary>
        /// <param name="target">The target<see cref="List{CheckBox}"/>.</param>
        /// <param name="count">The count<see cref="int"/>.</param>
        private void enableByCount(List<CheckBox> target, int count)
        {
            if (target.FindAll(x => x.IsChecked == true).Count.Equals(count))
            {
                foreach (var item in target.FindAll(x => x.IsChecked == false))
                {
                    item.IsEnabled = false;
                }
            }
            else
            {
                foreach (var item in target)
                {
                    item.IsEnabled = true;
                }
            }
        }

        /// <summary>
        /// The Radio_Checked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void Radio_Checked(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// The SandyDensityCheck.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.Windows.RoutedEventArgs"/>.</param>
        private void SandyDensityCheckChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!updateSelf)
            {
                updateSelf = true;
                for (int i = 5; i < this.DensityStatus.Count; i++)
                {
                    if (this.DensityStatus[i])
                    {
                        this.DensityStatus[i] = false;
                    }
                }
                updateSelf = false;
            }
            enableByCount(this.sandyDensities, 2);
        }

        /// <summary>
        /// The ViscosityDensityCheck.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.Windows.RoutedEventArgs"/>.</param>
        private void ViscosityDensityCheckChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!updateSelf)
            {
                updateSelf = true;
                for (int i = 0; i < 5; i++)
                {
                    if (this.DensityStatus[i])
                    {
                        this.DensityStatus[i] = false;
                    }
                }
                updateSelf = false;
            }
            enableByCount(this.viscosityDensities, 2);
        }

        /// <summary>
        /// The WetClicked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.Windows.RoutedEventArgs"/>.</param>
        private void WetCheckChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            enableByCount(this.wets, 2);
        }

        #endregion

        private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                this.grid.Opacity = 1;
                this.grid.IsEnabled = true;
            }
            else
            {
                this.grid.Opacity = .5;
                this.grid.IsEnabled = false;
            }
        }
    }
}
