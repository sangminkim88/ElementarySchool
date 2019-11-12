namespace CommonControl.Models
{
    using System;
    using System.ComponentModel;

    public class Day : INotifyPropertyChanged
    {
        #region Fields

        private DateTime date;

        private bool enabled;

        private bool isTargetMonth;

        private bool isToday;

        private string notes;

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        public DateTime Date
        {
            get { return date; }
            set
            {
                date = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Date"));
            }
        }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                enabled = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Enabled"));
            }
        }

        public bool IsTargetMonth
        {
            get { return isTargetMonth; }
            set
            {
                isTargetMonth = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsTargetMonth"));
            }
        }

        public bool IsToday
        {
            get { return isToday; }
            set
            {
                isToday = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsToday"));
            }
        }

        public string Notes
        {
            get { return notes; }
            set
            {
                notes = value;
                if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Notes"));
            }
        }

        #endregion
    }
}
