namespace GAIA2020.Controls
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Defines the <see cref="RockColumn2" />.
    /// </summary>
    public partial class RockColumn2 : RockColumnBase
    {
        #region Fields

        /// <summary>
        /// Defines the GapStatusProperty.
        /// </summary>
        public static readonly DependencyProperty GapStatusProperty =
           DependencyProperty.Register("GapStatus", typeof(ObservableCollection<bool>), typeof(RockColumn2), new UIPropertyMetadata(default(ObservableCollection<bool>)));

        /// <summary>
        /// Defines the listOfGap.
        /// </summary>
        private List<CheckBox> listOfGap = new List<CheckBox>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RockColumn2"/> class.
        /// </summary>
        public RockColumn2()
        {
            InitializeComponent();
            this.listOfWeathered.Add(this.weathered0);
            this.listOfWeathered.Add(this.weathered1);
            this.listOfWeathered.Add(this.weathered2);
            this.listOfWeathered.Add(this.weathered3);
            this.listOfWeathered.Add(this.weathered4);
            this.listOfWeathered.Add(this.weathered5);
            this.listOfRockStiffness.Add(this.rockStiffness0);
            this.listOfRockStiffness.Add(this.rockStiffness1);
            this.listOfRockStiffness.Add(this.rockStiffness2);
            this.listOfRockStiffness.Add(this.rockStiffness3);
            this.listOfRockStiffness.Add(this.rockStiffness4);
            this.listOfRough.Add(this.rough00);
            this.listOfRough.Add(this.rough01);
            this.listOfRough.Add(this.rough02);
            this.listOfRough.Add(this.rough10);
            this.listOfRough.Add(this.rough11);
            this.listOfRough.Add(this.rough12);
            this.listOfRough.Add(this.rough20);
            this.listOfRough.Add(this.rough21);
            this.listOfRough.Add(this.rough22);
            this.listOfGap.Add(this.gap0);
            this.listOfGap.Add(this.gap1);
            this.listOfGap.Add(this.gap2);
            this.listOfGap.Add(this.gap3);
            this.listOfGap.Add(this.gap4);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the GapStatus.
        /// </summary>
        public ObservableCollection<bool> GapStatus
        {
            get { return (ObservableCollection<bool>)GetValue(GapStatusProperty); }
            set { SetValue(GapStatusProperty, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The GapClick.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void GapCheckChanged(object sender, RoutedEventArgs e)
        {
            enableByCount(this.listOfGap, 2);
        }

        #endregion

        private void RockColumnBase_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
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
