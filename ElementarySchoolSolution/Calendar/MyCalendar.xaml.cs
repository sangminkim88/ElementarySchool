using System;
using System.Windows.Controls;

namespace Calendar
{
    /// <summary>
    /// Interaction logic for MyCalendar.xaml
    /// </summary>
    public partial class MyCalendar : UserControl
    {
        private int year = 2019;

        public int Year
        {
            get { return year; }
            set { year = value; }
        }

        public MyCalendar()
        {
            InitializeComponent();
        }

        private void Image_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string name = (sender as Image).Name;

            int i = name.Equals("prevButton") ? -1 : 1;
            monthCombo.SelectedIndex = monthCombo.SelectedIndex + i;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (monthCombo.SelectedItem == null) return;

            int month = monthCombo.SelectedIndex + 1;

            DateTime targetDate = new DateTime(year, month, 1);

            this.calendar?.BuildCalendar(targetDate);
        }
    }
}
