namespace Gaia.Controls
{
    using Gaia.Views;
    using System;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    public partial class SoilColumn : Grid
    {
        #region Fields

        private Grid selected;

        private Grid selectedGroup;
        private Action<Tuple<string, string>> SetA1;
        private FirstViewModel prop;

        public void SetAction(Action<Tuple<string, string>> setA1)
        {
            this.SetA1 = setA1;
        }
        public void SetProp(FirstViewModel prop)
        {
            this.prop = prop;
        }

        #endregion

        #region Constructors

        public SoilColumn()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        public Tuple<string, string> GetSelected()
        {
            if(this.selected == null)
            {
                return new Tuple<string, string>(string.Empty, string.Empty);
            }
            switch (this.selected.Name)
            {
                case "gw": return new Tuple<string, string>("Images/a-1_01.jpg", "GW");
                case "gp": return new Tuple<string, string>("Images/a-1_02.jpg", "GP");
                case "gm": return new Tuple<string, string>("Images/a-1_03.jpg", "GM");
                case "gwgm": return new Tuple<string, string>("Images/a-1_03.jpg", "GW-GM");
                case "gpgm": return new Tuple<string, string>("Images/a-1_03.jpg", "GP-GM");
                case "gc": return new Tuple<string, string>("Images/a-1_04.jpg", "GC");
                case "gwgc": return new Tuple<string, string>("Images/a-1_04.jpg", "GW-GC");
                case "gpgc": return new Tuple<string, string>("Images/a-1_04.jpg", "GP-GC");
                case "gcgm": return new Tuple<string, string>("Images/a-1_04.jpg", "GC-GM");
                case "sw": return new Tuple<string, string>("Images/a-1_05.jpg", "SW");
                case "sp": return new Tuple<string, string>("Images/a-1_06.jpg", "SP");
                case "sm": return new Tuple<string, string>("Images/a-1_07.jpg", "SM");
                case "swsm": return new Tuple<string, string>("Images/a-1_07.jpg", "SW-SM");
                case "spsm": return new Tuple<string, string>("Images/a-1_07.jpg", "SP-SM");
                case "sc": return new Tuple<string, string>("Images/a-1_08.jpg", "SC");
                case "swsc": return new Tuple<string, string>("Images/a-1_08.jpg", "SW-SC");
                case "spsc": return new Tuple<string, string>("Images/a-1_08.jpg", "SP-SC");
                case "scsm": return new Tuple<string, string>("Images/a-1_08.jpg", "SC-SM");
                case "ml": return new Tuple<string, string>("Images/a-1_09.jpg", "ML");
                case "mh": return new Tuple<string, string>("Images/a-1_10.jpg", "MH");
                case "cl": return new Tuple<string, string>("Images/a-1_11.jpg", "CL");
                case "clml": return new Tuple<string, string>("Images/a-1_11.jpg", "CL-ML");
                case "ch": return new Tuple<string, string>("Images/a-1_12.jpg", "CH");
                case "ol": return new Tuple<string, string>("Images/a-1_13.jpg", "OL");
                case "oh": return new Tuple<string, string>("Images/a-1_14.jpg", "OH");
                case "pt": return new Tuple<string, string>("Images/a-1_15.jpg", "PT");
                case "boulder": return new Tuple<string, string>("Images/a-1_16.jpg", "Boulder");
                case "풍화암": return new Tuple<string, string>("Images/a-1_17.jpg", "풍화암");
                case "연암": return new Tuple<string, string>("Images/a-1_18.jpg", "연암");
                case "경암": return new Tuple<string, string>("Images/a-1_19.jpg", "경암");
                default: return null;
            }
        }

        public void SetSelectedByCode(string code)
        {
            Grid target = null;
            code = code.Replace("-", "");
            switch (code.ToLower())
            {
                case "gw": target = this.gw; break;
                case "gp": target = this.gp; break;
                case "gm": target = this.gm; break;
                case "gwgm": target = this.gwgm; break;
                case "gpgm": target = this.gpgm; break;
                case "gc": target = this.gc; break;
                case "gwgc": target = this.gwgc; break;
                case "gpgc": target = this.gpgc; break;
                case "gcgm": target = this.gcgm; break;
                case "sw": target = this.sw; break;
                case "sp": target = this.sp; break;
                case "sm": target = this.sm; break;
                case "swsm": target = this.swsm; break;
                case "spsm": target = this.spsm; break;
                case "sc": target = this.sc; break;
                case "swsc": target = this.swsc; break;
                case "spsc": target = this.spsc; break;
                case "scsm": target = this.scsm; break;
                case "ml": target = this.ml; break;
                case "mh": target = this.mh; break;
                case "cl": target = this.cl; break;
                case "clml": target = this.clml; break;
                case "ch": target = this.ch; break;
                case "ol": target = this.ol; break;
                case "oh": target = this.oh; break;
                case "pt": target = this.pt; break;
                case "boulder": target = this.boulder; break;
                case "풍화암": target = this.풍화암; break;
                case "연암": target = this.연암; break;
                case "경암": target = this.경암; break;
            }
            this.grid_PreviewMouseUp(target, null);
        }

        private void grid_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            if (this.selected != null && !this.selected.Equals(grid))
            {
                grid.Background = FindResource("Theme.FocusBackground") as SolidColorBrush;
            }
        }

        private void grid_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            if (this.selected != null && !this.selected.Equals(grid))
            {
                grid.Background = FindResource("Theme.SelectedForeground") as SolidColorBrush;
            }
        }

        private void grid_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Space) || e.Key.Equals(Key.Enter))
            {
                this.grid_PreviewMouseUp(sender, null);
            }
        }

        private void grid_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender == null) return;

            Grid grid = sender as Grid;
            grid.Focus();

            if (this.selected == grid)
            {
                grid.Background = FindResource("Theme.SelectedForeground") as SolidColorBrush;
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

                    this.selectedGroup.Background = FindResource("Theme.SelectedForeground") as SolidColorBrush;

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
                    this.selected.Background = FindResource("Theme.SelectedForeground") as SolidColorBrush;
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

                        this.selectedGroup.Background = FindResource("Theme.SelectedForeground") as SolidColorBrush;

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
                    ((Border)grid.Parent).BorderBrush = FindResource("Theme.SelectedForeground") as SolidColorBrush;
                    Grid group = ((Grid)((Border)grid.Parent).Parent).Children[0] as Grid;
                    group.Background = FindResource("Theme.Background") as SolidColorBrush;

                    foreach (var item in group.Children)
                    {
                        if (item is TextBlock)
                        {
                            ((TextBlock)item).Foreground = FindResource("Theme.SelectedForeground") as SolidColorBrush;
                        }
                    }
                    this.selectedGroup = group;
                }

                grid.Background = FindResource("Theme.Background") as SolidColorBrush;
                foreach (var item in grid.Children)
                {
                    if (item is Border)
                    {
                        ((Border)item).BorderBrush = FindResource("Theme.SelectedForeground") as SolidColorBrush;
                    }
                    else if (item is TextBlock)
                    {
                        ((TextBlock)item).Foreground = FindResource("Theme.SelectedForeground") as SolidColorBrush;
                    }
                }
                this.selected = grid;
            }

            this.prop.SoilColumn = this.GetSelected();

            //if (this.SetA1 != null)
            //{
            //    this.SetA1(this.GetSelected());
            //}
        }

        #endregion
    }
}
