using System;
using System.Windows.Controls;

namespace Calendar
{
    /// <summary>
    /// Interaction logic for MyCalendar.xaml
    /// </summary>
    public partial class MyCalendar : UserControl
    {
        public int Year
        {
            get { return int.Parse(yearLabel.Content.ToString()); }
            set { yearLabel.Content = value; }
        }

        public MyCalendar()
        {
            InitializeComponent();
            this.monthCombo.SelectedIndex = DateTime.Today.Month - 1;
        }

        private void Image_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string name = (sender as Image).Name;

            int i = name.Equals("prevButton") ? -1 : 1;
            if (monthCombo.SelectedIndex + i > 11)
            {
                this.yearLabel.Content = int.Parse(this.yearLabel.Content.ToString()) + 1;
                monthCombo.SelectedIndex = 0;
            }
            else if (monthCombo.SelectedIndex + i < 0)
            {
                this.yearLabel.Content = int.Parse(this.yearLabel.Content.ToString()) - 1;
                monthCombo.SelectedIndex = 11;
            }
            else
            {
                monthCombo.SelectedIndex = monthCombo.SelectedIndex + i;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.yearLabel == null) return;

            int month = monthCombo.SelectedIndex + 1;

            DateTime targetDate = new DateTime(Year, month, 1);

            this.calendar?.BuildCalendar(targetDate);
        }
    }
}
