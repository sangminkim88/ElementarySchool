namespace Attendance.ViewModels
{
    using Attendance.Popups;
    using Attendance.Views;
    using Common;
    using Common.Models;
    using FileManager;
    using Microsoft.Win32;
    using MyCalendar;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using WpfBase.Bases;
    using WpfBase.Common;
    using WpfBase.Managers;

    public class AttendanceMainVM : ViewModelBase
    {
        #region Fields

        private List<AttendanceRecord> attendanceRecords = new List<AttendanceRecord>();

        private ObservableCollection<AttendanceRecord> resultAttendance = new ObservableCollection<AttendanceRecord>();

        public ObservableCollection<AttendanceRecord> ResultAttendance
        {
            get { return resultAttendance; }
            set { resultAttendance = value; }
        }
               
        private string condition;

        private string currentFileName;

        private DateTime? endDate;

        private bool isEnableView;

        private int selectedAttendanceIndex;

        private int selectedStudentIndex;

        private DateTime? startDate;

        private List<Student> students = new List<Student>();

        #endregion

        #region Constructors

        public AttendanceMainVM(MyCalendar myCalendar)
        {
            CalendarDayClickedCommand = new RelayCommand(OmCalendarDayClicked);
            SaveCommand = new RelayCommand(ExecuteSave);
            ImportCommand = new RelayCommand(ExecuteImport);
            ExportCommand = new RelayCommand(ExecuteExport);
            ClearConditionCommand = new RelayCommand(ExecuteClearCondition);
            AddConditionCommand = new RelayCommand(ExecuteAddCondition);
            ClearResultConditionCommand = new RelayCommand(ExecuteClearResultCondition);

            string tmpStudentsPath = string.Empty;
            tmpStudentsPath = ConfigManager.ReadProfileString(EConfigSection.Students.ToString(), EConfigKey.FilePath.ToString(), tmpStudentsPath);
            if (tmpStudentsPath != null && !tmpStudentsPath.Equals(string.Empty))
            {
                this.Students.Add(new Student());
                foreach (var item in XmlManager.Deserialize(tmpStudentsPath, this.Students.GetType()) as List<Student>)
                {
                    this.Students.Add(item);
                }
                this.IsEnableView = true;
            }
            else
            {
                MessageBox.Show("학생 정보가 없습니다. 설정을 먼저 해주세요.");
                this.Dispose();
                var a = ViewManager.GetValue(typeof(AttendanceMainV), false);
                this.IsEnableView = false;
                return;
            }

            var tmpAttendanceRecordsPath = string.Empty;
            tmpAttendanceRecordsPath = ConfigManager.ReadProfileString(EConfigSection.Attendance.ToString(),
                EConfigKey.FilePath.ToString(), tmpAttendanceRecordsPath);
            if (tmpAttendanceRecordsPath != null && !tmpAttendanceRecordsPath.Equals(string.Empty))
            {
                this.attendanceRecords = XmlManager.Deserialize(tmpAttendanceRecordsPath, this.Students.GetType()) as List<AttendanceRecord>;
                myCalendar.BuildCalendarOutCaller(this.attendanceRecords);
            }
        }

        #endregion

        #region Properties

        public ICommand AddConditionCommand { get; private set; }
        
        public List<AttendanceRecord> AttendanceRecords
        {
            get { return attendanceRecords; }
            set { attendanceRecords = value; }
        }

        public ICommand CalendarDayClickedCommand { get; private set; }

        public ICommand ClearConditionCommand { get; private set; }

        public ICommand ClearResultConditionCommand { get; private set; }

        public string Condition
        {
            get { return condition; }
            set { SetValue(ref condition, value); }
        }

        public DateTime? EndDate
        {
            get { return endDate; }
            set { SetValue(ref endDate, value); }
        }

        public ICommand ExportCommand { get; private set; }

        public ICommand ImportCommand { get; private set; }

        public bool IsEnableView
        {
            get { return isEnableView; }
            set { SetValue(ref isEnableView, value); }
        }

        public ICommand SaveCommand { get; private set; }

        public int SelectedAttendanceIndex
        {
            get { return selectedAttendanceIndex; }
            set { SetValue(ref selectedAttendanceIndex, value); }
        }

        public int SelectedStudentIndex
        {
            get { return selectedStudentIndex; }
            set { SetValue(ref selectedStudentIndex, value); }
        }

        public DateTime? StartDate
        {
            get { return startDate; }
            set { SetValue(ref startDate, value); }
        }

        public List<Student> Students
        {
            get { return students; }
            set { SetValue(ref students, value); }
        }

        #endregion

        #region Methods

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewModelManager.AddValue(typeof(AttendanceMainVM), this);
        }
        
        private void ExecuteAddCondition(object obj)
        {
            List<string> values = new List<string>();
            if (!this.SelectedStudentIndex.Equals(0))
            {
                values.Add(this.Students[this.SelectedStudentIndex].Name);
            }
            if (!this.SelectedAttendanceIndex.Equals(0))
            {
                values.Add(Enum.GetName(typeof(EAttendance), this.SelectedAttendanceIndex - 1));
            }
            if (this.StartDate.HasValue)
            {
                values.Add(this.StartDate.Value.ToString("yy. MM. dd"));
            }
            if (this.EndDate.HasValue)
            {
                values.Add(this.EndDate.Value.ToString("yy. MM. dd"));
            }

            if (values.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < values.Count; i++)
                {
                    if (!i.Equals(0))
                    {
                        sb.Append(" & ");
                    }
                    sb.Append(values[i]);
                }

                if (this.Condition == null || this.Condition.Length.Equals(0))
                {
                    this.Condition = sb.ToString();
                }
                else
                {
                    StringBuilder sb2 = new StringBuilder();
                    sb2.Append(this.Condition);
                    sb2.Append(" & ");
                    sb2.Append(sb);
                    this.Condition = sb2.ToString();
                }
            }
        }
        
        private void ExecuteClearCondition(object obj)
        {
            this.SelectedStudentIndex = 0;
            this.SelectedAttendanceIndex = 0;
            this.StartDate = null;
            this.EndDate = null;
        }

        private void ExecuteClearResultCondition(object obj)
        {
            this.Condition = string.Empty;
            this.ResultAttendance.Clear();
        }

        private void ExecuteExport(object o)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "dat Files (*.dat)|*.dat|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                XmlManager.Serialize(this.attendanceRecords, saveFileDialog.FileName);
            }
        }

        private void ExecuteImport(object o)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("저장을 먼저 하시겠습니까?", "경고",
                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            if (messageBoxResult.Equals(MessageBoxResult.Cancel))
            {
                return;
            }
            else if (messageBoxResult.Equals(MessageBoxResult.Yes))
            {
                this.ExecuteSave(null);
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "dat Files (*.dat)|*.dat|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog().Equals(true))
            {
                this.currentFileName = openFileDialog.FileName;

                this.attendanceRecords = (List<AttendanceRecord>)XmlManager.Deserialize(this.currentFileName, typeof(List<AttendanceRecord>));

                MyCalendar calendar = o as MyCalendar;
                calendar.BuildCalendarOutCaller(this.attendanceRecords);

                ConfigManager.WriteProfileString(EConfigSection.Attendance.ToString(), EConfigKey.FilePath.ToString(),
                    this.currentFileName);
            }
        }

        private void ExecuteSave(object o)
        {
            XmlManager.Serialize(this.attendanceRecords, this.currentFileName);
            ConfigManager.WriteProfileString(EConfigSection.Attendance.ToString(), EConfigKey.FilePath.ToString(),
                this.currentFileName);
        }
        
        private void OmCalendarDayClicked(object obj)
        {
            DayClickedEventArgs mouseArgs = obj as DayClickedEventArgs;
            if (mouseArgs == null) return;

            AttendancePopup popup = new AttendancePopup(mouseArgs.Date);
            if (popup.ShowDialog().Value)
            {
                this.AttendanceRecords.Add(new AttendanceRecord(mouseArgs.Date, new Student(popup.Name),
                    popup.Attendance, popup.DocumentTitle, popup.SubmitDocument));
                (mouseArgs.Calendar as MyCalendar).BuildCalendarOutCaller(this.AttendanceRecords);
            }
        }

        #endregion
    }
}
