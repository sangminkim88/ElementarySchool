namespace MyCalendar
{
    using Common.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Controls;
    using System.Windows.Media;

    public partial class DayCalendar : Border, INotifyPropertyChanged
    {
        #region Fields

        private DateTime date;

        private bool isTargetMonth;

        private bool isToday;

        private List<ICalendarData> calendarData = new List<ICalendarData>();

        #endregion

        #region Constructors

        public DayCalendar()
        {
            this.DataContext = this;
            InitializeComponent();
        }

        #endregion

        #region Events

        public event EventHandler<DayClickedEventArgs> Clicked;
        public event EventHandler<DayModifyEventArgs> Delete;
        public event EventHandler<DayModifyEventArgs> Modify;

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public DateTime Date
        {
            get { return date; }
            set { date = value; RaisePropertyChanged("Date"); }
        }

        public bool IsTargetMonth
        {
            get { return isTargetMonth; }
            set { isTargetMonth = value; RaisePropertyChanged("IsTargetMonth"); }
        }

        public bool IsToday
        {
            get { return isToday; }
            set { isToday = value; RaisePropertyChanged("IsToday"); }
        }

        public List<ICalendarData> CalendarData
        {
            get { return calendarData; }
            set { calendarData = value; RaisePropertyChanged("CalendarData"); }
        }

        #endregion

        #region Methods

        private void InnerBorder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var parent = VisualTreeHelper.GetParent(this);
            while (!(parent is MyCalendar))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            Clicked(this, new DayClickedEventArgs(this.date, parent));
        }

        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private void Grid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var parent = VisualTreeHelper.GetParent(this);
            while (!(parent is MyCalendar))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            Modify(this, new DayModifyEventArgs((sender as Grid).DataContext, parent));
        }

        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var parent = VisualTreeHelper.GetParent(this);
            while (!(parent is MyCalendar))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            Delete(this, new DayModifyEventArgs((sender as MenuItem).DataContext, parent));
        }

    }
}
