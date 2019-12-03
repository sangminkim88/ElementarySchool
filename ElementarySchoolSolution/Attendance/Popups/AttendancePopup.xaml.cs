namespace Attendance.Popups
{
    using Common;
    using Common.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    public partial class AttendancePopup : Window
    {
        private List<Student> students;
        #region Constructors

        public AttendancePopup(DateTime date, List<Student> students)
        {
            InitializeComponent();

            this.yearLabel.Content = date.Year;
            this.monthLabel.Content = date.Month;
            this.dayLabel.Content = date.Day;

            this.students = students;

            attendanceCombo.ItemsSource = Enum.GetValues(typeof(EAttendance)).Cast<EAttendance>();

            this.attendanceCombo.SelectedIndex = 0;
            this.nameCombo.ItemsSource = students.Select(x=>x.Name);
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

        public Student SelectedStudent
        {
            get { return this.students.Find(x=>x.Name.Equals(this.nameCombo.Text)); }
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
