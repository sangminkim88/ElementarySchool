namespace Gaia.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class InitView : Grid
    {
        #region Fields

        internal int i = 0;

        private Test addData = new Test { Test1 = "+추가" };

        private List<Test> data = new List<Test>();

        private List<Test> filteredData;

        private Action<string, string, string, string, string> loadMainViewAction;

        #endregion

        #region Constructors

        public InitView(Action<string, string, string, string, string> loadMainViewAction)
        {
            InitializeComponent();
            this.loadMainViewAction = loadMainViewAction;
            this.WaterMarkTextBox_TextChanged(null, null);
        }

        #endregion

        #region Methods

        private void addItem(object sender, MouseButtonEventArgs e)
        {
            DataGridCell dataGridCell = sender as DataGridCell;
            int rowIndex = this.dataGrid.Items.Count - 1;
            if (dataGridCell.Column.Header != null && dataGridCell.Column.Header.Equals("사업명") &&
                DataGridRow.GetRowContainingElement(dataGridCell).GetIndex().Equals(rowIndex))
            {
                //임의 데이터
                Test data = new Test();// { Test1 = "Test3", Test2 = "Test4" };
                data.Test1 = i.ToString();
                data.Test2 = (1 + i++).ToString();
                data.Test3 = new SubTest();
                data.Test3.Test4 = "test4";
                data.Test3.Test5 = "test5";
                data.Test3.Test6 = "test6";
                data.Test3.Test7 = "test7";
                data.Test3.Test8 = "test8";
                this.data.Add(data);
                this.WaterMarkTextBox_TextChanged(null, null);
            }
        }

        private void columnHeader_Click2(object sender, RoutedEventArgs e)
        {
            List<Test> list = new List<Test>();
            foreach (var item in this.dataGrid.Items.OfType<Test>())
            {
                list.Add(item);
            }
            list.Remove(addData);
            list = list.OrderBy(x => x.Test1).ToList();
            list.Add(addData);
            this.dataGrid.Items.Clear();
            foreach (var item in list)
            {
                this.dataGrid.Items.Add(item);
            }
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            int rowIndex = this.dataGrid.Items.IndexOf(e.Row.DataContext);
            if (rowIndex.Equals(this.dataGrid.Items.Count - 1))
            {
                Style style = new Style();
                style.TargetType = typeof(DataGridRow);
                //style.Setters.Add(new Setter() { Property = ForegroundProperty, Value = Brushes.Blue });
                //style.Setters.Add(new Setter() { Property = FontWeightProperty, Value = FontWeights.Bold });
                style.Setters.Add(new Setter() { Property = TextBlock.TextAlignmentProperty, Value = TextAlignment.Center });
                style.Setters.Add(new Setter() { Property = TextBlock.TextDecorationsProperty, Value = TextDecorations.Baseline });

                e.Row.Style = style;
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Test data = this.dataGrid.SelectedItem as Test;
            if (data != null)
            {
                this.upper.Text = data.Test1;
                this.bottom.Text = data.Test2;
            }
        }

        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow dataGridRow = sender as DataGridRow;
            Test selectedData = dataGridRow.Item as Test;
            this.loadMainViewAction(selectedData.Test1, selectedData.Test1, selectedData.Test1, selectedData.Test1, selectedData.Test1);
        }

        private void setSelectedRow(int selectedIndex)
        {
            object tmp = dataGrid.Items[selectedIndex];
            dataGrid.SelectedItem = tmp;
            dataGrid.ScrollIntoView(tmp);
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(selectedIndex);
            row.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void WaterMarkTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.filteredData = this.data.FindAll(x => x.Test1.Contains(this.searchTextBox.Text));
            this.filteredData.Add(addData);
            if (this.dataGrid != null)
            {
                int selectedIndex = this.dataGrid.SelectedIndex;
                this.dataGrid.Items.Clear();
                foreach (var item in this.filteredData)
                {
                    this.dataGrid.Items.Add(item);
                }
                if (!selectedIndex.Equals(-1))
                {
                    this.setSelectedRow(selectedIndex);
                }
            }
        }

        #endregion
    }

    public class SubTest
    {
        #region Properties

        public string Test4 { get; set; }

        public string Test5 { get; set; }

        public string Test6 { get; set; }

        public string Test7 { get; set; }

        public string Test8 { get; set; }

        #endregion
    }

    public class Test
    {
        #region Properties

        public string Test1 { get; set; }

        public string Test2 { get; set; }

        public SubTest Test3 { get; set; }

        #endregion
    }
}
