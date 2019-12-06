namespace Attendance.Popups
{
    using Attendance.Models;
    using Common;
    using Common.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    public partial class AttendancePopup : Window
    {
        #region Fields

        private List<Student> students;

        #endregion

        #region Constructors

        public AttendancePopup(AttendanceRecord attendanceRecord, List<Student> students)
        {
            InitializeComponent();

            this.datePicker.SelectedDate = attendanceRecord.Date;

            this.students = students;
            this.nameCombo.ItemsSource = students.Select(x => x.Name);
            this.nameCombo.SelectedIndex = this.nameCombo.Items.IndexOf(attendanceRecord.StudentRecord.Name);

            this.attendanceCombo.ItemsSource = Enum.GetValues(typeof(EAttendance)).Cast<EAttendance>();
            this.attendanceCombo.SelectedIndex = this.attendanceCombo.Items.IndexOf(attendanceRecord.EAttendance);

            this.documentTitle.Text = attendanceRecord.DocumentTitle;

            this.submitDocument.IsChecked = attendanceRecord.SubmitDocument;

            this.addButton.Content = "수정";
        }

        public AttendancePopup(DateTime date, List<Student> students)
        {
            InitializeComponent();

            this.datePicker.SelectedDate = date;

            this.students = students;

            attendanceCombo.ItemsSource = Enum.GetValues(typeof(EAttendance)).Cast<EAttendance>();

            this.attendanceCombo.SelectedIndex = 0;
            this.nameCombo.ItemsSource = students.Select(x=>x.Name);
            this.nameCombo.SelectedIndex = 0;
        }

        #endregion

        #region Properties

        public string DocumentTitle
        {
            get { return this.documentTitle.Text; }
        }

        public EAttendance EAttendanceMember
        {
            get { return (EAttendance)this.attendanceCombo.SelectedIndex; }
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
            if(this.EAttendanceMember.Equals(EAttendance.지각) || this.EAttendanceMember.Equals(EAttendance.조퇴))
            {
                this.submitDocument.IsChecked = true;
            }

            this.DialogResult = true;
        }

        private void AttendanceCombo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(this.attendanceCombo.SelectedItem.Equals(EAttendance.결석) || 
                this.attendanceCombo.SelectedItem.Equals(EAttendance.현장학습))
            {
                this.documentPanel.Visibility = Visibility.Visible;
                this.submitDocument.IsChecked = false;
            }
            else
            {
                this.documentPanel.Visibility = Visibility.Collapsed;
                this.submitDocument.IsChecked = true;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        #endregion
    }
}
