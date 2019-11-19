namespace MyCalendar
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using System.Windows.Input;

    public partial class MyCalendar : UserControl
    {
        public event EventHandler<DayClickedEventArgs> DayClicked;
        private List<DayCalendar> dayCalendars = new List<DayCalendar>();

        public int Year
        {
            get { return int.Parse(yearLabel.Content.ToString()); }
            set { yearLabel.Content = value; }
        }

        public int Month
        {
            get { return this.monthCombo.SelectedIndex +1; }
        }

        #region Constructors

        public MyCalendar()
        {
            InitializeComponent();

            int todayYear = DateTime.Now.Year;
            int todayMonth = DateTime.Now.Month;

            this.Year = todayYear;
            this.monthCombo.SelectedIndex = todayMonth -1;

            for (int i = 0; i < 42; i++)
            {
                DayCalendar tmp = new DayCalendar();
                dayCalendars.Add(tmp);

                tmp.SetValue(Grid.ColumnProperty, i % 7);
                tmp.SetValue(Grid.RowProperty, 2 + (i / 7));
                mainGrid.Children.Add(tmp);
            }
            this.buildCalendar(DateTime.Today);
        }

        #endregion

        #region Methods

        private void Image_PreviewMouseUp(object sender, MouseButtonEventArgs e)
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

        #endregion

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.dayCalendars.Count.Equals(0)) return;

            int month = monthCombo.SelectedIndex + 1;

            DateTime targetDate = new DateTime(Year, month, 1);

            this.buildCalendar(targetDate);
        }


        private void buildCalendar(DateTime targetDate)
        {
            DateTime d = new DateTime(targetDate.Year, targetDate.Month, 1);
            int offset = DayOfWeekNumber(d.DayOfWeek);
            if (offset != 1) d = d.AddDays(-offset);

            for (int box = 0; box < 42; box++)
            {
                this.dayCalendars[box].Date = d.Day;
                this.dayCalendars[box].IsToday = (d == DateTime.Today);
                this.dayCalendars[box].IsTargetMonth = (targetDate.Month == d.Month);
                this.dayCalendars[box].Clicked += DayClickedEvent;
                d = d.AddDays(1);
            }
        }

        private void DayClickedEvent(object sender, DayClickedEventArgs e)
        {
            if (DayClicked == null) return;

            DayClicked(this, new DayClickedEventArgs((sender as DayCalendar).Date));
        }
        
        private static int DayOfWeekNumber(DayOfWeek dow)
        {
            return Convert.ToInt32(dow.ToString("D"));
        }

    }
    public class DayClickedEventArgs : EventArgs
    {
        public int date { get; private set; }

        public DayClickedEventArgs(int date)
        {
            this.date = date;
        }
    }
}