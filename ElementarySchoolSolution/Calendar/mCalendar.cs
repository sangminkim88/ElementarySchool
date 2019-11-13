namespace Calendar
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    public class mCalendar : Control
    {
        #region Fields

        public static readonly DependencyProperty CurrentDateProperty = DependencyProperty.Register("CurrentDate", typeof(DateTime), typeof(mCalendar));

        #endregion

        #region Constructors

        static mCalendar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(mCalendar), new FrameworkPropertyMetadata(typeof(mCalendar)));
        }

        public mCalendar()
        {
            DataContext = this;
            CurrentDate = DateTime.Today;

            DayNames = new ObservableCollection<string> { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

            Days = new ObservableCollection<Day>();
            BuildCalendar(DateTime.Today);
        }

        #endregion

        #region Events

        public event EventHandler<DayChangedEventArgs> DayChanged;

        #endregion

        #region Properties

        public DateTime CurrentDate
        {
            get { return (DateTime)GetValue(CurrentDateProperty); }
            set { SetValue(CurrentDateProperty, value); }
        }

        public ObservableCollection<string> DayNames { get; set; }

        public ObservableCollection<Day> Days { get; set; }

        #endregion

        #region Methods

        private static int DayOfWeekNumber(DayOfWeek dow)
        {
            return Convert.ToInt32(dow.ToString("D"));
        }

        public void BuildCalendar(DateTime targetDate)
        {
            Days.Clear();

            //Calculate when the first day of the month is and work out an 
            //offset so we can fill in any boxes before that.
            DateTime d = new DateTime(targetDate.Year, targetDate.Month, 1);
            int offset = DayOfWeekNumber(d.DayOfWeek);
            if (offset != 1) d = d.AddDays(-offset);

            //Show 6 weeks each with 7 days = 42
            for (int box = 1; box <= 42; box++)
            {
                Day day = new Day { Date = d, Enabled = true, IsTargetMonth = targetDate.Month == d.Month };
                day.PropertyChanged += Day_Changed;
                day.IsToday = d == DateTime.Today;
                Days.Add(day);
                d = d.AddDays(1);
            }
        }

        private void Day_Changed(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "Notes") return;
            if (DayChanged == null) return;

            DayChanged(this, new DayChangedEventArgs(sender as Day));
        }

        #endregion
    }

    public class DayChangedEventArgs : EventArgs
    {
        #region Constructors

        public DayChangedEventArgs(Day day)
        {
            this.Day = day;
        }

        #endregion

        #region Properties

        public Day Day { get; private set; }

        #endregion
    }
}
