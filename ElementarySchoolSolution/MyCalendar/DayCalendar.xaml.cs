namespace MyCalendar
{
    using System;
    using System.ComponentModel;
    using System.Windows.Controls;

    public partial class DayCalendar : Border, INotifyPropertyChanged
    {
        #region Fields

        private DateTime date;

        private bool isTargetMonth;

        private bool isToday;

        private string notes;

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

        public string Notes
        {
            get { return notes; }
            set { notes = value; RaisePropertyChanged("Notes"); }
        }

        #endregion

        #region Methods

        private void InnerBorder_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Clicked(this, new DayClickedEventArgs(this.date));
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
