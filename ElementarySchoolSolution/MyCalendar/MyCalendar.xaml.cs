namespace MyCalendar
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class DayClickedEventArgs : EventArgs
    {
        #region Constructors

        public DayClickedEventArgs(DateTime date)
        {
            this.date = date;
        }

        #endregion

        #region Properties

        public DateTime date { get; private set; }

        #endregion
    }

    public partial class MyCalendar : UserControl
    {
        #region Fields

        private List<DayCalendar> dayCalendars = new List<DayCalendar>();

        #endregion

        #region Constructors

        public MyCalendar()
        {
            InitializeComponent();

            int todayYear = DateTime.Now.Year;
            int todayMonth = DateTime.Now.Month;

            this.Year = todayYear;
            this.monthCombo.SelectedIndex = todayMonth - 1;

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

        #region Events

        public event EventHandler<DayClickedEventArgs> DayClicked;

        #endregion

        #region Properties

        public int Month
        {
            get { return this.monthCombo.SelectedIndex + 1; }
            set { this.monthCombo.SelectedIndex = value - 1; }
        }

        public int Year
        {
            get { return int.Parse(yearLabel.Content.ToString()); }
            set { yearLabel.Content = value; }
        }

        #endregion

        #region Methods

        private static int DayOfWeekNumber(DayOfWeek dow)
        {
            return Convert.ToInt32(dow.ToString("D"));
        }

        private void buildCalendar(DateTime targetDate)
        {
            DateTime d = new DateTime(targetDate.Year, targetDate.Month, 1);
            int offset = DayOfWeekNumber(d.DayOfWeek);
            if (offset != 1) d = d.AddDays(-offset);

            for (int box = 0; box < 42; box++)
            {
                this.dayCalendars[box].Date = d;
                this.dayCalendars[box].IsToday = (d == DateTime.Today);
                this.dayCalendars[box].IsTargetMonth = (targetDate.Month == d.Month);
                this.dayCalendars[box].Clicked += DayClickedEvent;
                d = d.AddDays(1);
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.dayCalendars.Count.Equals(0)) return;

            int month = monthCombo.SelectedIndex + 1;

            DateTime targetDate = new DateTime(Year, month, 1);

            this.buildCalendar(targetDate);
        }

        private void DayClickedEvent(object sender, DayClickedEventArgs e)
        {
            if (DayClicked == null) return;

            DayClicked(this, new DayClickedEventArgs((sender as DayCalendar).Date));
        }

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
    }
}
