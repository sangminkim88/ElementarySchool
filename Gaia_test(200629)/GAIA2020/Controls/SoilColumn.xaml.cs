namespace GAIA2020.Controls
{
    using GaiaDB.Enums;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using GaiaDB;
    using HmGeometry;

    /// <summary>
    /// Defines the <see cref="SoilColumn" />.
    /// </summary>
    public partial class SoilColumn : UserControl
    {
        #region Fields

        /// <summary>
        /// Defines the SelectedSoilProperty.
        /// </summary>
        public static readonly DependencyProperty SelectedSoilProperty =
           DependencyProperty.Register("SelectedSoil", typeof(eSoil), typeof(SoilColumn), new UIPropertyMetadata(eSoil.None, selectedSoilChanged));

        /// <summary>
        /// Defines the selected.
        /// </summary>
        private Grid selected;

        /// <summary>
        /// Defines the selectedGroup.
        /// </summary>
        private Grid selectedGroup;

        /// <summary>
        /// Defines the updateSelf.
        /// </summary>
        private bool updateSelf;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SoilColumn"/> class.
        /// </summary>
        public SoilColumn()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the SelectedSoil.
        /// </summary>
        public eSoil SelectedSoil
        {
            get { return (eSoil)GetValue(SelectedSoilProperty); }
            set { SetValue(SelectedSoilProperty, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The selectedSoilChanged.
        /// </summary>
        /// <param name="d">The d<see cref="DependencyObject"/>.</param>
        /// <param name="e">The e<see cref="DependencyPropertyChangedEventArgs"/>.</param>
        private static void selectedSoilChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SoilColumn soilColumn = d as SoilColumn;
            if (!soilColumn.updateSelf)
            {
                soilColumn.SetSelectedByCode((eSoil)e.NewValue);
            }
        }

        /// <summary>
        /// The GetSelected.
        /// </summary>
        /// <returns>The <see cref="Tuple{string, string}"/>.</returns>
        public Tuple<string, eSoil> GetSelected()
        {
            if (this.selected == null)
            {
                return new Tuple<string, eSoil>(string.Empty, eSoil.None);
            }
            eSoil soil = (eSoil)Enum.Parse(typeof(eSoil), this.selected.Name);
            string path;
            switch (soil)
            {
                case eSoil.None: path = string.Empty; break;
                case eSoil.gw: path = "Images/a-1_01.jpg"; break;
                case eSoil.gp: path = "Images/a-1_02.jpg"; break;
                case eSoil.gm: path = "Images/a-1_03.jpg"; break;
                case eSoil.gwgm: path = "Images/a-1_03.jpg"; break;
                case eSoil.gpgm: path = "Images/a-1_03.jpg"; break;
                case eSoil.gc: path = "Images/a-1_04.jpg"; break;
                case eSoil.gwgc: path = "Images/a-1_04.jpg"; break;
                case eSoil.gpgc: path = "Images/a-1_04.jpg"; break;
                case eSoil.gcgm: path = "Images/a-1_04.jpg"; break;
                case eSoil.sw: path = "Images/a-1_05.jpg"; break;
                case eSoil.sp: path = "Images/a-1_06.jpg"; break;
                case eSoil.sm: path = "Images/a-1_07.jpg"; break;
                case eSoil.swsm: path = "Images/a-1_07.jpg"; break;
                case eSoil.spsm: path = "Images/a-1_07.jpg"; break;
                case eSoil.sc: path = "Images/a-1_08.jpg"; break;
                case eSoil.swsc: path = "Images/a-1_08.jpg"; break;
                case eSoil.spsc: path = "Images/a-1_08.jpg"; break;
                case eSoil.scsm: path = "Images/a-1_08.jpg"; break;
                case eSoil.ml: path = "Images/a-1_09.jpg"; break;
                case eSoil.mh: path = "Images/a-1_10.jpg"; break;
                case eSoil.cl: path = "Images/a-1_11.jpg"; break;
                case eSoil.clml: path = "Images/a-1_11.jpg"; break;
                case eSoil.ch: path = "Images/a-1_12.jpg"; break;
                case eSoil.ol: path = "Images/a-1_13.jpg"; break;
                case eSoil.oh: path = "Images/a-1_14.jpg"; break;
                case eSoil.pt: path = "Images/a-1_15.jpg"; break;
                case eSoil.boulder: path = "Images/a-1_16.jpg"; break;
                case eSoil.wrock: path = "Images/a-1_17.jpg"; break;
                case eSoil.srock: path = "Images/a-1_18.jpg"; break;
                case eSoil.hrock: path = "Images/a-1_19.jpg"; break;
                default: path = string.Empty; break;
            }
            return new Tuple<string, eSoil>(path, soil);
        }

        /// <summary>
        /// The GetSelectedCode.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public eSoil GetSelectedCode()
        {
            if (this.selected == null)
            {
                return eSoil.None;
            }
            switch (this.selected.Name)
            {
                case "gw": return eSoil.gw;
                case "gp": return eSoil.gp;
                case "gm": return eSoil.gm;
                case "gwgm": return eSoil.gwgm;
                case "gpgm": return eSoil.gpgm;
                case "gc": return eSoil.gc;
                case "gwgc": return eSoil.gwgc;
                case "gpgc": return eSoil.gpgc;
                case "gcgm": return eSoil.gcgm;
                case "sw": return eSoil.sw;
                case "sp": return eSoil.sp;
                case "sm": return eSoil.sm;
                case "swsm": return eSoil.swsm;
                case "spsm": return eSoil.spsm;
                case "sc": return eSoil.sc;
                case "swsc": return eSoil.swsc;
                case "spsc": return eSoil.spsc;
                case "scsm": return eSoil.scsm;
                case "ml": return eSoil.ml;
                case "mh": return eSoil.mh;
                case "cl": return eSoil.cl;
                case "clml": return eSoil.clml;
                case "ch": return eSoil.ch;
                case "ol": return eSoil.ol;
                case "oh": return eSoil.oh;
                case "pt": return eSoil.pt;
                case "boulder": return eSoil.boulder;
                case "풍화암": return eSoil.wrock;
                case "연암": return eSoil.srock;
                case "경암": return eSoil.hrock;
                default: return eSoil.None;
            }
        }

        /// <summary>
        /// The grid_GotFocus.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.Windows.RoutedEventArgs"/>.</param>
        private void grid_GotFocus(object sender, System.Windows.RoutedEventArgs e)
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
        private void grid_LostFocus(object sender, System.Windows.RoutedEventArgs e)
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
        private void grid_PreviewKeyUp(object sender, KeyEventArgs e)
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
        private void grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == null)
            {
                if (this.selected != null)
                {
                    grid_PreviewMouseDown(this.selected, null);                    
                }
                return;
            }

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

                if (this.selectedGroup != null)
                {
                    ((Border)this.selected.Parent).BorderBrush = FindResource("Theme.DefaultForeground") as SolidColorBrush;

                    this.selectedGroup.Background = FindResource("Theme.Color4") as SolidColorBrush;

                    foreach (var item in this.selectedGroup.Children)
                    {
                        if (item is TextBlock)
                        {
                            ((TextBlock)item).Foreground = FindResource("Theme.DefaultForeground") as SolidColorBrush;
                        }
                    }
                }

                this.selected = null;
                this.selectedGroup = null;
            }
            else
            {
                if (this.selected != null)
                {
                    this.selected.Background = FindResource("Theme.Color4") as SolidColorBrush;
                    foreach (var item in this.selected.Children)
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

                    if (this.selectedGroup != null)
                    {
                        ((Border)this.selected.Parent).BorderBrush = FindResource("Theme.DefaultForeground") as SolidColorBrush;

                        this.selectedGroup.Background = FindResource("Theme.Color4") as SolidColorBrush;

                        foreach (var item in this.selectedGroup.Children)
                        {
                            if (item is TextBlock)
                            {
                                ((TextBlock)item).Foreground = FindResource("Theme.DefaultForeground") as SolidColorBrush;
                            }
                        }
                    }
                }

                if (grid.Tag != null && grid.Tag.Equals("group"))
                {
                    ((Border)grid.Parent).BorderBrush = FindResource("Theme.Color4") as SolidColorBrush;
                    Grid group = ((Grid)((Border)grid.Parent).Parent).Children[0] as Grid;
                    group.Background = FindResource("Theme.Color1") as SolidColorBrush;

                    foreach (var item in group.Children)
                    {
                        if (item is TextBlock)
                        {
                            ((TextBlock)item).Foreground = FindResource("Theme.Color4") as SolidColorBrush;
                        }
                    }
                    this.selectedGroup = group;
                }

                grid.Background = FindResource("Theme.Color1") as SolidColorBrush;
                foreach (var item in grid.Children)
                {
                    if (item is Border)
                    {
                        ((Border)item).BorderBrush = FindResource("Theme.Color4") as SolidColorBrush;
                    }
                    else if (item is TextBlock)
                    {
                        ((TextBlock)item).Foreground = FindResource("Theme.Color4") as SolidColorBrush;
                    }
                }

                //[케이싱심도]는 토사층(풍화암 포함)까지 표현되는 심도임
                DBDoc doc = DBDoc.Get_CurrDoc();
                HmDBKey activeKey = doc.Get_ActiveStratum(false);

                DBDataSTRA straD = new DBDataSTRA();
                if (doc.stra.Get_Data(activeKey.nKey, ref straD))
                {
                    DBDataDRLG drlgD = new DBDataDRLG();
                    if (doc.drlg.Get_Data(straD.drlgKey, ref drlgD))
                    {
                        this.selected = grid;
                        eSoil tmp = this.GetSelectedCode();

                        if (drlgD.CasingDepth > straD.Depth && // 토사
                            (tmp == eSoil.srock || tmp == eSoil.hrock))
                        {
                            this.updateSelf = true;
                            this.SetSelectedByCode(eSoil.None);
                            this.updateSelf = false;
                        }
                        else
                        {
                            this.updateSelf = true;
                            this.SelectedSoil = this.GetSelectedCode();
                            this.updateSelf = false;
                        }
                    }
                }                
            }
        }

        /// <summary>
        /// The SetSelectedByCode.
        /// </summary>
        /// <param name="code">The code<see cref="string"/>.</param>
        private void SetSelectedByCode(eSoil code)
        {
            Grid target = null;
            switch (code)
            {
                case eSoil.None:
                    target = null; break;
                case eSoil.gw: target = this.gw; break;
                case eSoil.gp: target = this.gp; break;
                case eSoil.gm: target = this.gm; break;
                case eSoil.gwgm: target = this.gwgm; break;
                case eSoil.gpgm: target = this.gpgm; break;
                case eSoil.gc: target = this.gc; break;
                case eSoil.gwgc: target = this.gwgc; break;
                case eSoil.gpgc: target = this.gpgc; break;
                case eSoil.gcgm: target = this.gcgm; break;
                case eSoil.sw: target = this.sw; break;
                case eSoil.sp: target = this.sp; break;
                case eSoil.sm: target = this.sm; break;
                case eSoil.swsm: target = this.swsm; break;
                case eSoil.spsm: target = this.spsm; break;
                case eSoil.sc: target = this.sc; break;
                case eSoil.swsc: target = this.swsc; break;
                case eSoil.spsc: target = this.spsc; break;
                case eSoil.scsm: target = this.scsm; break;
                case eSoil.ml: target = this.ml; break;
                case eSoil.mh: target = this.mh; break;
                case eSoil.cl: target = this.cl; break;
                case eSoil.clml: target = this.clml; break;
                case eSoil.ch: target = this.ch; break;
                case eSoil.ol: target = this.ol; break;
                case eSoil.oh: target = this.oh; break;
                case eSoil.pt: target = this.pt; break;
                case eSoil.boulder: target = this.boulder; break;
                case eSoil.wrock: target = this.풍화암; break;
                case eSoil.srock: target = this.연암; break;
                case eSoil.hrock: target = this.경암; break;
                default: target = null; break;
            }
            this.grid_PreviewMouseDown(target, null);
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
