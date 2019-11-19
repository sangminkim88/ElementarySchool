namespace Common.Models
{
    using System;

    public class AttendanceRecord
    {
        #region Fields

        private DateTime date;

        private string documentTitle;

        private Student studentRecord;

        private bool submitDocument;

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
