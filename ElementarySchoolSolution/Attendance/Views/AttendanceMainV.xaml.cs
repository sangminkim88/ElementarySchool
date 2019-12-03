namespace Attendance.Views
{
    using Attendance.ViewModels;
    using Common;
    using System;
    using System.Linq;
    using System.Windows.Input;
    using WpfBase.Bases;
    using WpfBase.Managers;

    public partial class AttendanceMainV : ViewBase
    {
        #region Constructors

        public AttendanceMainV()
        {
            InitializeComponent();
            this.DataContext = new AttendanceMainVM(this.calendar);
            this.Title = "출석관리";
            var tmp = Enum.GetNames(typeof(EAttendance)).ToList();
            tmp.Insert(0, string.Empty);
            this.attendanceCombo.ItemsSource = tmp;
            this.attendanceCombo.SelectedIndex = 0;
        }

        #endregion

        #region Methods

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewManager.AddValue(typeof(AttendanceMainV), this);
        }

        private void EndDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.endDatePicker?.SelectedDate != null)
            {
                DateTime dateTime = (DateTime)this.endDatePicker?.SelectedDate;
                if (dateTime < this.startDatePicker?.SelectedDate)
                {
                    this.startDatePicker.SelectedDate = dateTime;
                }
            }
        }

        private void GridSplitter_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Wait)
            {
                Mouse.OverrideCursor = Cursors.SizeWE;
            }
        }

        private void GridSplitter_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Wait)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        private void StartDatePicker_SelectedDateChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.startDatePicker?.SelectedDate != null)
            {
                DateTime dateTime = (DateTime)this.startDatePicker?.SelectedDate;
                if (dateTime > this.endDatePicker?.SelectedDate)
                {
                    this.endDatePicker.SelectedDate = dateTime;
                }
            }
        }

        #endregion
    }
}
