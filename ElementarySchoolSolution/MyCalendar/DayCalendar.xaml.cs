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

        private List<AttendanceRecord> attendanceRecords = new List<AttendanceRecord>();

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

        public List<AttendanceRecord> AttendanceRecords
        {
            get { return attendanceRecords; }
            set { attendanceRecords = value; RaisePropertyChanged("AttendanceRecords"); }
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
    }
}
