namespace GAIA2020.Controls
{
    using GAIA2020.Design;
    using GAIA2020.Utilities;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Defines the <see cref="RockColumnBase" />.
    /// </summary>
    public class RockColumnBase : UserControl
    {
        #region Fields
        
        /// <summary>
        /// Defines the JointsProperty.
        /// </summary>
        public static readonly DependencyProperty JointsProperty =
           DependencyProperty.Register("Joints", typeof(ObservableCollection<int?>), typeof(RockColumnBase), new UIPropertyMetadata(default(ObservableCollection<int?>)));

        /// <summary>
        /// Defines the PartialJointProperty.
        /// </summary>
        public static readonly DependencyProperty PartialJointProperty =
           DependencyProperty.Register("PartialJoint", typeof(bool), typeof(RockColumnBase), new UIPropertyMetadata(default(bool)));

        /// <summary>
        /// Defines the StiffnessStatusProperty.
        /// </summary>
        public static readonly DependencyProperty StiffnessStatusProperty =
           DependencyProperty.Register("StiffnessStatus", typeof(ObservableCollection<bool>), typeof(RockColumnBase),
               new UIPropertyMetadata(default(ObservableCollection<bool>)));

        /// <summary>
        /// Defines the RockTypeProperty.
        /// </summary>
        public static readonly DependencyProperty RockTypeProperty =
           DependencyProperty.Register("RockType", typeof(string), typeof(RockColumnBase),
               new UIPropertyMetadata(default(string), new PropertyChangedCallback(RockTypeChanged)));

        /// <summary>
        /// Defines the RoughStatusProperty.
        /// </summary>
        public static readonly DependencyProperty RoughStatusProperty =
           DependencyProperty.Register("RoughStatus", typeof(ObservableCollection<bool>), typeof(RockColumnBase),
                new UIPropertyMetadata(default(ObservableCollection<bool>)));

        /// <summary>
        /// Defines the SelectedStratumProperty.
        /// </summary>
        public static readonly DependencyProperty SelectedStratumProperty =
           DependencyProperty.Register("SelectedStratum", typeof(string), typeof(RockColumnBase),
               new UIPropertyMetadata(default(string)));

        /// <summary>
        /// Defines the WeatheredStatusProperty.
        /// </summary>
        public static readonly DependencyProperty WeatheredStatusProperty =
           DependencyProperty.Register("WeatheredStatus", typeof(ObservableCollection<bool>), typeof(RockColumnBase),
               new UIPropertyMetadata(default(ObservableCollection<bool>)));

        /// <summary>
        /// Defines the listOfRockStiffness.
        /// </summary>
        protected List<CheckBox> listOfRockStiffness = new List<CheckBox>();

        /// <summary>
        /// Defines the listOfRough.
        /// </summary>
        protected List<CheckBox> listOfRough = new List<CheckBox>();

        /// <summary>
        /// Defines the listOfWeathered.
        /// </summary>
        protected List<CheckBox> listOfWeathered = new List<CheckBox>();

        /// <summary>
        /// Defines the selected.
        /// </summary>
        protected Grid selected;
        
        /// <summary>
        /// Defines the updateSelf.
        /// </summary>
        private bool updateSelf;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Joints.
        /// </summary>
        public ObservableCollection<int?> Joints
        {
            get { return (ObservableCollection<int?>)GetValue(JointsProperty); }
            set { SetValue(JointsProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether PartialJoint.
        /// </summary>
        public bool PartialJoint
        {
            get { return (bool)GetValue(PartialJointProperty); }
            set { SetValue(PartialJointProperty, value); }
        }

        /// <summary>
        /// Gets or sets the StiffnessStatus.
        /// </summary>
        public ObservableCollection<bool> StiffnessStatus
        {
            get { return (ObservableCollection<bool>)GetValue(StiffnessStatusProperty); }
            set { SetValue(StiffnessStatusProperty, value); }
        }

        /// <summary>
        /// Gets or sets the RockType.
        /// </summary>
        public string RockType
        {
            get { return (string)GetValue(RockTypeProperty); }
            set { SetValue(RockTypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the RoughStatus.
        /// </summary>
        public ObservableCollection<bool> RoughStatus
        {
            get { return (ObservableCollection<bool>)GetValue(RoughStatusProperty); }
            set { SetValue(RoughStatusProperty, value); }
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
        /// Gets or sets the WeatheredStatus.
        /// </summary>
        public ObservableCollection<bool> WeatheredStatus
        {
            get { return (ObservableCollection<bool>)GetValue(WeatheredStatusProperty); }
            set { SetValue(WeatheredStatusProperty, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The RockTypeChanged.
        /// </summary>
        /// <param name="d">The d<see cref="DependencyObject"/>.</param>
        /// <param name="e">The e<see cref="DependencyPropertyChangedEventArgs"/>.</param>
        private static void RockTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RockColumnBase rockColumnBase = d as RockColumnBase;
            if (!rockColumnBase.updateSelf) {
                string data = e.NewValue.ToString();
                if (!string.IsNullOrEmpty(data))
                {
                    Grid target = (Grid)rockColumnBase.FindName(KeyControlPair.Instance.Rock.Find(x => x.Item1.Equals(e.NewValue)).Item2);
                    if (target != null)
                    {
                        rockColumnBase.grid_PreviewMouseDown(target, null);
                    }
                }
                else
                {
                    rockColumnBase.grid_PreviewMouseDown(rockColumnBase.selected, null);
                }
            }
        }

        /// <summary>
        /// The grid_GotFocus.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.Windows.RoutedEventArgs"/>.</param>
        protected void grid_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            if (this.selected != null && !this.selected.Equals(grid))
            {
                grid.Background = FindResource("Theme.FocusBackground") as SolidColorBrush;
            }
        }

        /// <summary>
        /// The grid_LostFocus.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.Windows.RoutedEventArgs"/>.</param>
        protected void grid_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            if (this.selected != null && !this.selected.Equals(grid))
            {
                grid.Background = FindResource("Theme.Color4") as SolidColorBrush;
            }
        }

        /// <summary>
        /// The grid_PreviewKeyUp.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="KeyEventArgs"/>.</param>
        protected void grid_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Space) || e.Key.Equals(Key.Enter))
            {
                this.grid_PreviewMouseDown(sender, null);
            }
        }

        /// <summary>
        /// The grid_PreviewMouseDown.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="MouseButtonEventArgs"/>.</param>
        protected void grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == null) return;

            Grid grid = sender as Grid;
            grid.Focus();

            if (this.selected == grid)
            {
                grid.Background = FindResource("Theme.Color4") as SolidColorBrush;
                foreach (var item in grid.Children)
                {
                    if (item is Border)
                    {
                        ((Border)item).BorderBrush = FindResource("Theme.DefaultForeground") as SolidColorBrush;
                    }
                    else if (item is TextBlock)
                    {
                        ((TextBlock)item).Foreground = FindResource("Theme.DefaultForeground") as SolidColorBrush;
                    }
                }

                this.selected = null;
            }
            else
            {
                if (this.selected != null)
                {
                    this.selected.Background = FindResource("Theme.Color4") as SolidColorBrush;
                    foreach (var item in this.selected.Children)
                    {
                        if (item is TextBlock)
                        {
                            ((TextBlock)item).Foreground = FindResource("Theme.DefaultForeground") as SolidColorBrush;
                        }
                    }
                }

                grid.Background = FindResource("Theme.Color1") as SolidColorBrush;
                string selectedData = string.Empty;
                foreach (var item in grid.Children)
                {
                    if (item is TextBlock)
                    {
                        ((TextBlock)item).Foreground = FindResource("Theme.Color4") as SolidColorBrush;
                        selectedData = ((TextBlock)item).Text.Replace(" ", "");
                    }
                }
                this.selected = grid;
                this.updateSelf = true;
                this.RockType = selectedData;
                this.updateSelf = false;
            }
        }

        /// <summary>
        /// The RockStiffnessCheck.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        protected void RockStiffnessCheckChanged(object sender, RoutedEventArgs e)
        {
            enableByCount(this.listOfRockStiffness, 2);
        }

        /// <summary>
        /// The RoughCheck.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        protected void RoughCheckChanged(object sender, RoutedEventArgs e)
        {
            string tag = (sender as CheckBox).Tag.ToString();
            if (!this.updateSelf)
            {
                this.updateSelf = true;
                this.listOfRough.FindAll(x => !x.Tag.Equals(tag)).FindAll(x => x.IsChecked.Equals(true)).ForEach(x => x.IsChecked = false);
                this.updateSelf = false;
            }
            enableByCount(this.listOfRough.FindAll(x => x.Tag.Equals(tag)), 2);
        }

        /// <summary>
        /// The WeatheredCheck.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        protected void WeatheredCheckChanged(object sender, RoutedEventArgs e)
        {
            enableByCount(this.listOfWeathered, 2);
        }

        /// <summary>
        /// The enableByCount.
        /// </summary>
        /// <param name="target">The target<see cref="List{CheckBox}"/>.</param>
        /// <param name="count">The count<see cref="int"/>.</param>
        protected void enableByCount(List<CheckBox> target, int count)
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

        #endregion
    }
}
