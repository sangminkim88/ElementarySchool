namespace Attendance.Models
{
    using Common;
    using Common.Models;
    using MyCalendar;
    using System;

    public class AttendanceRecord : ICalendarData
    {
        #region Fields

        private EAttendance _eAttendance;

        private DateTime date;

        private string documentTitle;

        private Student studentRecord;

        private bool submitDocument;

        #endregion

        #region Constructors

        public AttendanceRecord()
        {
        }

        public AttendanceRecord(DateTime date, Student student, EAttendance eAttendance, string documentTitle, bool submitDocument)
        {
            this.date = date;
            this.studentRecord = student;
            this._eAttendance = eAttendance;
            this.documentTitle = documentTitle;
            this.submitDocument = submitDocument;
        }

        #endregion

        #region Properties

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public string DocumentTitle
        {
            get { return documentTitle; }
            set { documentTitle = value; }
        }

        public EAttendance EAttendance
        {
            get { return _eAttendance; }
            set { _eAttendance = value; }
        }

        public Student StudentRecord
        {
            get { return studentRecord; }
            set { studentRecord = value; }
        }

        public bool SubmitDocument
        {
            get { return submitDocument; }
            set { submitDocument = value; }
        }

        #endregion
    }
}
