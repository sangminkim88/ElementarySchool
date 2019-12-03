namespace Attendance.Popups
{
    using Common;
    using System;
    using System.Linq;
    using System.Windows;

    public partial class AttendancePopup : Window
    {
        #region Constructors

        public AttendancePopup(DateTime date)
        {
            InitializeComponent();

            this.yearLabel.Content = date.Year;
            this.monthLabel.Content = date.Month;
            this.dayLabel.Content = date.Day;

            attendanceCombo.ItemsSource = Enum.GetValues(typeof(EAttendance)).Cast<EAttendance>();

            this.attendanceCombo.SelectedIndex = 0;
            this.nameCombo.SelectedIndex = 0;
        }

        #endregion

        #region Properties

        public EAttendance Attendance
        {
            get { return (EAttendance)this.attendanceCombo.SelectedIndex; }
        }

        public string DocumentTitle
        {
            get { return this.documentTitle.Text; }
        }

        public string Name
        {
            get { return this.nameCombo.Text; }
        }

        public bool SubmitDocument
        {
            get { return this.submitDocument.IsChecked is true ? true : false; }
        }

        #endregion

        #region Methods

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #endregion

        private void AttendanceCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(this.attendanceCombo.SelectedItem.Equals(EAttendance.결석) || 
                this.attendanceCombo.SelectedItem.Equals(EAttendance.현장학습))
            {
                this.documentPanel.Visibility = Visibility.Visible;
            }
            else
            {
                this.documentPanel.Visibility = Visibility.Collapsed;
            }
        }
    }
}
