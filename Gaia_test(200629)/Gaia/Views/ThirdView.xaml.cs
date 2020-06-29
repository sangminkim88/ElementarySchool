namespace Gaia.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    public partial class ThirdView : Grid
    {
        #region Fields


        private List<CheckBox> gaps = new List<CheckBox>();
        
        private List<CheckBox> listOfRough = new List<CheckBox>();

        private List<CheckBox> listOfStrength = new List<CheckBox>();

        private List<CheckBox> listOfWeathered = new List<CheckBox>();

        private List<RadioButton> rockKinds = new List<RadioButton>();

        private List<Grid> rocks = new List<Grid>();
        
        private Grid selected;


        #endregion

        #region Constructors

        public ThirdView()
        {
            InitializeComponent();            
            this.rockKinds.Add(this.rockKind0);
            this.rockKinds.Add(this.rockKind1);
            this.rocks.Add(this.rock0);
            this.rocks.Add(this.rock1);
            this.rocks.Add(this.rock2);
            this.rocks.Add(this.rock3);
            this.rocks.Add(this.rock4);
            this.rocks.Add(this.rock5);
            this.rocks.Add(this.rock6);
            this.rocks.Add(this.rock7);
            this.rocks.Add(this.rock8);
            this.rocks.Add(this.rock9);
            this.rocks.Add(this.rock10);
            this.rocks.Add(this.rock11);
            this.rocks.Add(this.rock12);
            this.rocks.Add(this.rock13);
            this.rocks.Add(this.rock14);
            this.listOfWeathered.Add(this.weathered0);
            this.listOfWeathered.Add(this.weathered1);
            this.listOfWeathered.Add(this.weathered2);
            this.listOfWeathered.Add(this.weathered3);
            this.listOfWeathered.Add(this.weathered4);
            this.listOfWeathered.Add(this.weathered5);
            this.listOfStrength.Add(this.strength0);
            this.listOfStrength.Add(this.strength1);
            this.listOfStrength.Add(this.strength2);
            this.listOfStrength.Add(this.strength3);
            this.listOfStrength.Add(this.strength4);
            this.gaps.Add(this.gap0);
            this.gaps.Add(this.gap1);
            this.gaps.Add(this.gap2);
            this.gaps.Add(this.gap3);
            this.gaps.Add(this.gap4);
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

        public void ClearCohesiveDensities()
        {
            this.sandColumn.ClearCohesiveDensities();
        }

        public void ClearGap()
        {
            this.gaps.ForEach(x => x.IsChecked = false);
        }

        public void ClearLayer()
        {
            this.sandColumn.ClearLayer();
        }

        public void ClearRock()
        {
            this.grid_PreviewMouseUp(this.selected, null);
        }

        public void ClearRockKinds()
        {
            this.rockKinds.ForEach(x => x.IsChecked = false);
        }

        public void ClearRough()
        {
            this.listOfRough.ForEach(x => x.IsChecked = false);
        }

        public void ClearSandyDensities()
        {
            this.sandColumn.ClearSandyDensities();
        }

        public void ClearStrength()
        {
            this.listOfStrength.ForEach(x => x.IsChecked = false);
        }

        public void ClearWeathered()
        {
            this.listOfWeathered.ForEach(x => x.IsChecked = false);
        }

        public void ClearWets()
        {
            this.sandColumn.ClearWets();
        }

        public List<Tuple<string, string>> GetCohesiveDensities()
        {
            return this.sandColumn.GetCohesiveDensities();
        }

        public List<Tuple<string, string>> GetGaps()
        {
            return this.getCheckedCheckBoxes(this.gaps);
        }

        public string GetSand()
        {
            return this.sandColumn.GetSand();
        }

        public string GetRock()
        {
            Grid selectedGrid = this.rocks.Find(x => x.Background.Equals(FindResource("Theme.SelectedForeground") as SolidColorBrush));
            string returnData = string.Empty;
            if (selectedGrid != null)
            {
                foreach (var item in selectedGrid.Children)
                {
                    if (item is TextBlock) returnData = ((TextBlock)item).Text;
                }
            }
            return returnData;
        }

        public string GetRockKinds()
        {
            return this.rockKinds.Find(x => x.IsChecked.Equals(true))?.Content.ToString();
        }

        public List<Tuple<string, string>> GetRough()
        {
            List<Tuple<string, string>> returnData = new List<Tuple<string, string>>();

            foreach (CheckBox item in this.listOfRough.FindAll(x => x.IsChecked.Equals(true)).ToList())
            {
                string number = item.Name.Replace("rough", string.Empty);
                string type = string.Empty, degree = string.Empty;

                switch (number[0])
                {
                    case '0':
                        type = "계단형";
                        break;
                    case '1':
                        type = "파동형";
                        break;
                    case '2':
                        type = "평면형";
                        break;
                }

                switch (number[1])
                {
                    case '0':
                        degree = "거침";
                        break;
                    case '1':
                        degree = "완만";
                        break;
                    case '2':
                        degree = "경면";
                        break;
                }

                returnData.Add(new Tuple<string, string>(type, degree));
            }
            return returnData;
        }

        public List<Tuple<string, string>> GetSandyDensities()
        {
            return this.sandColumn.GetSandyDensities();
        }

        public List<Tuple<string, string>> GetStrength()
        {
            return this.getCheckedCheckBoxes(this.listOfStrength);
        }

        public List<Tuple<string, string>> GetWeathered()
        {
            return this.getCheckedCheckBoxes(this.listOfWeathered);
        }

        public List<Tuple<string, string>> GetWets()
        {
            return this.sandColumn.GetWets();
        }

        public bool SetCohesiveDensitiesByComment(List<string> targets)
        {
            return this.sandColumn.SetCohesiveDensitiesByComment(targets);
        }

        public bool SetCohesiveDensitiesByTitle(List<string> targets)
        {
            return this.sandColumn.SetCohesiveDensitiesByTitle(targets);
        }

        public void SetCutSectionUI(bool isCutSection)
        {
            if (isCutSection)
            {
                this.column0.Width = new GridLength(11, GridUnitType.Pixel);
                this.column1.Width = new GridLength(238, GridUnitType.Star);
                this.row00.Height = new GridLength(39, GridUnitType.Star);
                this.jointForm.Visibility = Visibility.Visible;
            }
            else
            {
                this.column0.Width = new GridLength(0, GridUnitType.Star);
                this.column1.Width = new GridLength(0, GridUnitType.Star);
                this.row00.Height = new GridLength(0, GridUnitType.Star);
                this.jointForm.Visibility = Visibility.Collapsed;
            }
        }

        public bool SetGapsByComment(List<string> targets)
        {
            return this.setCheckBoxesByComment(this.gaps, targets);
        }

        public bool SetGapsByTitle(List<string> targets)
        {
            return this.setCheckBoxesByTitle(this.gaps, targets);
        }

        public bool SetLayer(string target)
        {
            return this.sandColumn.SetSand(target);
        }

        public bool SetRock(string target)
        {
            foreach (Grid item1 in this.rocks)
            {
                foreach (var item2 in item1.Children)
                {
                    if (item2 is TextBlock)
                    {
                        if (((TextBlock)item2).Text.Equals(target))
                        {
                            this.grid_PreviewMouseUp(item1, null);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool SetRockKinds(string target)
        {
            RadioButton radioButton = this.rockKinds.Find(x => x.Content.Equals(target));
            if (radioButton != null)
            {
                radioButton.IsChecked = true;
                return true;
            }
            return false;
        }

        public bool SetRough(List<Tuple<string, string>> targets)
        {
            bool returnData = true;
            string controlName = "rough";
            foreach (Tuple<string, string> item in targets)
            {
                switch (item.Item1)
                {
                    case "계단형":
                        controlName += "0";
                        break;
                    case "파동형":
                        controlName += "1";
                        break;
                    case "평면형":
                        controlName += "2";
                        break;
                    default:
                        returnData = false;
                        continue;
                }
                switch (item.Item2)
                {
                    case "거침":
                        controlName += "0";
                        break;
                    case "완만":
                        controlName += "1";
                        break;
                    case "경면":
                        controlName += "2";
                        break;
                    default:
                        returnData = false;
                        continue;
                }
                CheckBox target = (CheckBox)FindName(controlName);
                if (target != null)
                {
                    target.IsChecked = true;
                }
            }
            return returnData;
        }

        public bool SetSandyDensitiesByComment(List<string> targets)
        {
            return this.sandColumn.SetSandyDensitiesByComment(targets);
        }

        public bool SetSandyDensitiesByTitle(List<string> targets)
        {
            return this.sandColumn.SetSandyDensitiesByTitle(targets);
        }

        public bool SetStrengthByComment(List<string> targets)
        {
            return this.setCheckBoxesByComment(this.listOfStrength, targets);
        }

        public bool SetStrengthByTitle(List<string> targets)
        {
            return this.setCheckBoxesByTitle(this.listOfStrength, targets);
        }

        public bool SetWeatheredByComment(List<string> targets)
        {
            return this.setCheckBoxesByComment(this.listOfWeathered, targets);
        }

        public bool SetWeatheredByTitle(List<string> targets)
        {
            return this.setCheckBoxesByTitle(this.listOfWeathered, targets);
        }

        public bool SetWetsByComment(List<string> targets)
        {
            return this.sandColumn.SetWetsByComment(targets);
        }

        public bool SetWetsByTitle(List<string> targets)
        {
            return this.sandColumn.SetWetsByTitle(targets);
        }

        private List<Tuple<string, string>> getCheckedCheckBoxes(List<CheckBox> targets)
        {
            List<Tuple<string, string>> returnData = new List<Tuple<string, string>>();

            foreach (CheckBox item in targets.FindAll(x => x.IsChecked.Equals(true)).ToList())
            {
                StackPanel stackPanel = (StackPanel)item.Content;
                string text = ((TextBlock)stackPanel.Children[0]).Text.Replace(" ", "");
                returnData.Add(new Tuple<string, string>(text, ((TextBlock)stackPanel.Children[1]).Text));
            }
            return returnData;
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

                this.selected = null;
            }
            else
            {
                if (this.selected != null)
                {
                    this.selected.Background = FindResource("Theme.SelectedForeground") as SolidColorBrush;
                    foreach (var item in this.selected.Children)
                    {
                        if (item is TextBlock)
                        {
                            ((TextBlock)item).Foreground = FindResource("Theme.DefaultForeground") as SolidColorBrush;
                        }
                    }
                }

                grid.Background = FindResource("Theme.Background") as SolidColorBrush;
                foreach (var item in grid.Children)
                {
                    if (item is TextBlock)
                    {
                        ((TextBlock)item).Foreground = FindResource("Theme.SelectedForeground") as SolidColorBrush;
                    }
                }
                this.selected = grid;
            }
        }

        private bool setCheckBoxesByComment(List<CheckBox> targetCheckBoxes, List<string> targets)
        {
            bool returnData = true;
            targetCheckBoxes.ForEach(x => x.IsChecked = false);
            foreach (string item in targets)
            {
                CheckBox checkBox = this.listOfWeathered.Find(x => ((TextBlock)((StackPanel)x.Content).Children[1]).Text.Equals(item));
                if (checkBox == null)
                {
                    returnData = false;
                }
                else
                {
                    checkBox.IsChecked = true;
                }
            }
            return returnData;
        }

        private bool setCheckBoxesByTitle(List<CheckBox> targetCheckBoxes, List<string> targets)
        {
            bool returnData = true;
            targetCheckBoxes.ForEach(x => x.IsChecked = false);
            foreach (string item in targets)
            {
                CheckBox checkBox = this.listOfWeathered.Find(x => ((TextBlock)((StackPanel)x.Content).Children[0]).Text.Replace(" ", string.Empty).Equals(item));
                if (checkBox == null)
                {
                    returnData = false;
                }
                else
                {
                    checkBox.IsChecked = true;
                }
            }
            return returnData;
        }

        #endregion
    }
}
