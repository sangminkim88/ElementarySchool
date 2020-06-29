namespace GAIA2020.Controls
{
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Defines the <see cref="RockColumn1" />.
    /// </summary>
    public partial class RockColumn1 : RockColumnBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RockColumn1"/> class.
        /// </summary>
        public RockColumn1()
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
        }

        #endregion

        #region Methods
        
        #endregion

        private void RockColumnBase_IsEnabledChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
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
