namespace Attendance.ViewModels
{
    using Common.Models;
    using FileManager;
    using MyCalendar;
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;
    using WpfBase.Bases;
    using WpfBase.Common;
    using WpfBase.Managers;

    public class AttendanceMainVM : ViewModelBase
    {

        private List<AttendanceRecord> attendanceRecords = new List<AttendanceRecord>();

        public List<AttendanceRecord> AttendanceRecord
        {
            get { return attendanceRecords; }
            set { attendanceRecords = value; }
        }

        #region Fields
        private string currentFileName;
        #endregion

        #region Constructors

        public AttendanceMainVM()
        {
            CalendarDayClickedCommand = new RelayCommand(OmCalendarDayClicked);
            SaveCommand = new RelayCommand(ExecuteSaveCommand);

            //ConfigManager.WriteProfileString("Attendance", "onLoadFilePath", "Test~");
            var findfolder = "";
            findfolder = ConfigManager.ReadProfileString("Attendance", "onLoadFilePath", findfolder);
        }

        #endregion

        #region Properties

        public ICommand CalendarDayClickedCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }


        #endregion

        #region Methods

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewModelManager.AddValue(typeof(AttendanceMainVM), this);
        }

        private void ExecuteSaveCommand(object obj)
        {
            //XmlManager.Serialize(this.AttendanceRecord,  @"D:\test\test.xml");
            //var tmp = XmlManager.Deserialize(@"D:\test\test.xml", typeof(List<AttendanceRecord>));

        }

        private void OmCalendarDayClicked(object obj)
        {
            var mouseArgs = obj as MouseButtonEventArgs;
            if (mouseArgs == null)
                return;
        }

        public void SetCalendarData(MyCalendar calendar)
        {
            this.attendanceRecords.Add(new AttendanceRecord());
            this.attendanceRecords.Add(new AttendanceRecord());
            calendar.BuildCalendarOutCaller(this.attendanceRecords);
        }

        #endregion
    }
}
