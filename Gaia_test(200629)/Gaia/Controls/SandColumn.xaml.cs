namespace Gaia.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;

    public partial class SandColumn : Grid
    {
        #region Fields

        private List<CheckBox> cohesiveDensities = new List<CheckBox>();

        private List<RadioButton> sands = new List<RadioButton>();

        private List<CheckBox> sandyDensities = new List<CheckBox>();

        private Action<string> SetB1;

        private Action<List<Tuple<string, string>>> SetB2;

        private Action<List<Tuple<string, string>>> SetB3;

        private Action<List<Tuple<string, string>>> SetB4;

        private List<CheckBox> wets = new List<CheckBox>();

        #endregion

        #region Constructors

        public SandColumn()
        {
            InitializeComponent();
            this.sands.Add(this.sand0);
            this.sands.Add(this.sand1);
            this.sands.Add(this.sand2);
            this.sands.Add(this.sand3);
            this.sands.Add(this.sand4);
            this.sands.Add(this.sand5);
            this.sands.Add(this.sand6);
            this.wets.Add(this.wet0);
            this.wets.Add(this.wet1);
            this.wets.Add(this.wet2);
            this.wets.Add(this.wet3);
            this.sandyDensities.Add(this.sandyDensity0);
            this.sandyDensities.Add(this.sandyDensity1);
            this.sandyDensities.Add(this.sandyDensity2);
            this.sandyDensities.Add(this.sandyDensity3);
            this.sandyDensities.Add(this.sandyDensity4);
            this.cohesiveDensities.Add(this.cohesiveDensity0);
            this.cohesiveDensities.Add(this.cohesiveDensity1);
            this.cohesiveDensities.Add(this.cohesiveDensity2);
            this.cohesiveDensities.Add(this.cohesiveDensity3);
            this.cohesiveDensities.Add(this.cohesiveDensity4);
            this.cohesiveDensities.Add(this.cohesiveDensity5);
        }

        #endregion

        #region Methods

        public void ClearCohesiveDensities()
        {
            this.cohesiveDensities.ForEach(x => x.IsChecked = false);
        }

        public void ClearLayer()
        {
            this.sands.ForEach(x => x.IsChecked = false);
        }

        public void ClearSandyDensities()
        {
            this.sandyDensities.ForEach(x => x.IsChecked = false);
        }

        public void ClearWets()
        {
            this.wets.ForEach(x => x.IsChecked = false);
        }

        public List<Tuple<string, string>> GetCohesiveDensities()
        {
            return this.getCheckedCheckBoxes(this.cohesiveDensities);
        }

        public string GetSand()
        {
            return this.sands.Find(x => x.IsChecked.Equals(true))?.Content.ToString();
        }

        public List<Tuple<string, string>> GetSandyDensities()
        {
            return this.getCheckedCheckBoxes(this.sandyDensities);
        }

        public List<Tuple<string, string>> GetWets()
        {
            return this.getCheckedCheckBoxes(this.wets);
        }

        public void SetAction(Action<string> setB1, Action<List<Tuple<string, string>>> setB2, Action<List<Tuple<string, string>>> setB3, Action<List<Tuple<string, string>>> setB4)
        {
            this.SetB1 = setB1;
            this.SetB2 = setB2;
            this.SetB3 = setB3;
            this.SetB4 = setB4;
        }

        public bool SetCohesiveDensitiesByComment(List<string> targets)
        {
            return this.setCheckBoxesByComment(this.cohesiveDensities, targets);
        }

        public bool SetCohesiveDensitiesByTitle(List<string> targets)
        {
            return this.setCheckBoxesByTitle(this.cohesiveDensities, targets);
        }

        public bool SetSand(string target)
        {
            RadioButton radioButton = this.sands.Find(x => x.Content.Equals(target));
            if (radioButton != null)
            {
                this.sands.ForEach(x => x.IsChecked = false);
                radioButton.IsChecked = true;
                return true;
            }
            return false;
        }

        public bool SetSandyDensitiesByComment(List<string> targets)
        {
            return this.setCheckBoxesByComment(this.sandyDensities, targets);
        }

        public bool SetSandyDensitiesByTitle(List<string> targets)
        {
            return this.setCheckBoxesByTitle(this.sandyDensities, targets);
        }

        public bool SetWetsByComment(List<string> targets)
        {
            return this.setCheckBoxesByComment(this.wets, targets);
        }

        public bool SetWetsByTitle(List<string> targets)
        {
            return this.setCheckBoxesByTitle(this.wets, targets);
        }

        private void CohesiveDensity_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.SetB4(this.GetCohesiveDensities());
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
            if (returnData.Count > 2)
            {
                List<Tuple<string, string>> newReturnData = new List<Tuple<string, string>>();
                newReturnData.Add(returnData.First());
                newReturnData.Add(returnData.Last());
                return newReturnData;
            }
            return returnData;
        }

        private void Sand_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            this.SetB1(this.GetSand());
        }

        private void SandyDensity_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.SetB3(this.GetSandyDensities());
        }

        private bool setCheckBoxesByComment(List<CheckBox> targetCheckBoxes, List<string> targets)
        {
            bool returnData = true;
            targetCheckBoxes.ForEach(x => x.IsChecked = false);
            foreach (string item in targets)
            {
                CheckBox checkBox = targetCheckBoxes.Find(x => ((TextBlock)((StackPanel)x.Content).Children[1]).Text.Equals(item));
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
                CheckBox checkBox = targetCheckBoxes.Find(x => ((TextBlock)((StackPanel)x.Content).Children[0]).Text.Replace(" ", string.Empty).Equals(item));
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

        private void Wet_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.SetB2(this.GetWets());
        }

        #endregion
    }
}
