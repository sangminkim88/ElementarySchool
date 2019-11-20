namespace Common.Models
{
    using Common.Enums;
    using System;

    public class AttendanceRecord
    {
        #region Fields

        private eAttendance _eAttendance;

        private DateTime date;

        private string documentTitle;

        private Student studentRecord;

        private bool submitDocument;

        #endregion

        #region Constructors

        public AttendanceRecord()
        {
            this.date = DateTime.Today.AddDays(1);
            this.studentRecord = new Student();
            this.studentRecord.Name = "test";
            this.studentRecord.Number = "010";
            this.studentRecord.Sex = Enums.eSex.남성;
            this.submitDocument = false;
            this.EAttendance = eAttendance.Lateness;
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

        public eAttendance EAttendance
        {
            get { return _eAttendance; }
            set { _eAttendance = value; }
        }

        public Student StudentRecord
        {
            get { return studentRecord; }
            set { studentRecord = value; }
        }

        public bool SunmitDocument
        {
            get { return submitDocument; }
            set { submitDocument = value; }
        }

        #endregion
    }
}
