namespace Attendance.ViewModels
{
    using Attendance.Models;
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

        private List<ICalendarData> attendanceRecords = new List<ICalendarData>();

        private string condition;

        private string currentFilePath = string.Empty;

        private DateTime? endDate;

        private bool isEnableView;

        private ObservableCollection<AttendanceRecord> resultAttendance = new ObservableCollection<AttendanceRecord>();

        private int selectedAttendanceIndex;

        private int selectedStudentIndex;

        private DateTime? startDate;

        private List<Student> students = new List<Student>();

        #endregion

        #region Constructors

        public AttendanceMainVM(MyCalendar myCalendar)
        {
            CalendarDayClickedCommand = new RelayCommand(OnCalendarDayClicked);
            CalendarDayModifyCommand = new RelayCommand(OnCalendarDayModify);
            CalendarDayDeleteCommand = new RelayCommand(OnCalendarDayDelete);
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
                this.Students.Add(new Student(string.Empty));
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

            this.CurrentFilePath = ConfigManager.ReadProfileString(EConfigSection.Attendance.ToString(),
                EConfigKey.FilePath.ToString(), this.CurrentFilePath);
            if (!this.CurrentFilePath.Equals(string.Empty))
            {
                this.attendanceRecords = ((List<AttendanceRecord>)XmlManager.Deserialize(this.CurrentFilePath, typeof(List<AttendanceRecord>))).
                    ConvertAll(x => (ICalendarData)x);
                myCalendar.BuildCalendarOutCaller(this.attendanceRecords);
            }
        }

        #endregion

        #region Properties

        public ICommand AddConditionCommand { get; private set; }

        public List<ICalendarData> AttendanceRecords
        {
            get { return attendanceRecords; }
            set { attendanceRecords = value; }
        }

        public ICommand CalendarDayClickedCommand { get; private set; }

        public ICommand CalendarDayDeleteCommand { get; private set; }

        public ICommand CalendarDayModifyCommand { get; private set; }

        public ICommand ClearConditionCommand { get; private set; }

        public ICommand ClearResultConditionCommand { get; private set; }

        public string Condition
        {
            get { return condition; }
            set { SetValue(ref condition, value); }
        }

        public string CurrentFilePath
        {
            get { return currentFilePath; }
            set { SetValue(ref currentFilePath , value); }
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

        public ObservableCollection<AttendanceRecord> ResultAttendance
        {
            get { return resultAttendance; }
            set { resultAttendance = value; }
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
            Dictionary<string, string> values = new Dictionary<string, string>();

            if (!this.SelectedStudentIndex.Equals(0))
            {
                values.Add("Name", this.Students[this.SelectedStudentIndex].Name);
            }
            if (!this.SelectedAttendanceIndex.Equals(0))
            {
                values.Add("Category", Enum.GetName(typeof(EAttendance), this.SelectedAttendanceIndex - 1));
            }
            if (this.StartDate.HasValue)
            {
                values.Add("StartDate", this.StartDate.Value.ToString("yy.MM.dd"));
            }
            if (this.EndDate.HasValue)
            {
                values.Add("EndDate", this.EndDate.Value.ToString("yy.MM.dd"));
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
                    sb.Append(values.ElementAt(i).Value);
                }

                if (this.Condition == null || this.Condition.Length.Equals(0))
                {
                    this.Condition = sb.ToString();
                }
                else
                {
                    StringBuilder sb2 = new StringBuilder();
                    sb2.AppendLine(this.Condition);
                    sb2.Append(sb.ToString());
                    //sb2.Append(sb);
                    this.Condition = sb2.ToString();
                }

                List<AttendanceRecord> filtered = new List<AttendanceRecord>();
                for (int i = 0; i < values.Count; i++)
                {
                    List<AttendanceRecord> tmp;
                    if (i.Equals(0))
                    {
                        tmp = this.AttendanceRecords.ConvertAll(x => (AttendanceRecord)x);
                    }
                    else
                    {
                        tmp = filtered;
                    }
                    switch (values.ElementAt(i).Key)
                    {
                        case "Name":
                            filtered = tmp.FindAll(x => x.StudentRecord.Name.Equals(values.ElementAt(i).Value));
                            break;
                        case "Category":
                            filtered = tmp.FindAll(x => x.EAttendance.ToString().Equals(values.ElementAt(i).Value));
                            break;
                        case "StartDate":
                            DateTime dateTime1 = DateTime.Parse(values.ElementAt(i).Value);
                            filtered = tmp.FindAll(x => DateTime.Compare(x.Date, dateTime1) >= 0);
                            break;
                        case "EndDate":
                            DateTime dateTime2 = DateTime.Parse(values.ElementAt(i).Value);
                            filtered = tmp.FindAll(x => DateTime.Compare(x.Date, dateTime2) <= 0);
                            break;
                        default:
                            break;
                    }

                }

                foreach (var item in filtered)
                {
                    this.ResultAttendance.Add(item);
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
                XmlManager.Serialize(this.attendanceRecords.ConvertAll(x => (AttendanceRecord)x), saveFileDialog.FileName);
            }
        }

        private void ExecuteImport(object o)
        {
            if (!this.CurrentFilePath.Length.Equals(0) && MessageBox.Show("변경한 내용은 저장되지 않습니다. 그래도 진행하시겠습니까?", "경고",
                  MessageBoxButton.YesNo, MessageBoxImage.Warning).Equals(MessageBoxResult.No))
            {
                return;
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "dat Files (*.dat)|*.dat|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog().Equals(true))
            {
                this.CurrentFilePath = openFileDialog.FileName;

                this.attendanceRecords = ((List<AttendanceRecord>)XmlManager.Deserialize(this.CurrentFilePath, typeof(List<AttendanceRecord>)))
                    .ConvertAll(x => (ICalendarData)x);

                MyCalendar calendar = o as MyCalendar;
                calendar.BuildCalendarOutCaller(this.attendanceRecords);

                ConfigManager.WriteProfileString(EConfigSection.Attendance.ToString(), EConfigKey.FilePath.ToString(),
                    this.CurrentFilePath);
            }
        }

        private void ExecuteSave(object o)
        {
            if (this.CurrentFilePath.Length.Equals(0))
            {
                MessageBox.Show("Import된 파일이 없습니다. Export로 먼저 파일을 만드세요.", "실패",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (MessageBox.Show("변경한 내용을 저장하시겠습니까?", "확인",
                MessageBoxButton.YesNo, MessageBoxImage.Information).Equals(MessageBoxResult.Yes))
            {
                XmlManager.Serialize(this.attendanceRecords.ConvertAll(x => (AttendanceRecord)x), this.CurrentFilePath);
                ConfigManager.WriteProfileString(EConfigSection.Attendance.ToString(), EConfigKey.FilePath.ToString(),
                    this.CurrentFilePath);
            }
        }

        private void OnCalendarDayClicked(object obj)
        {
            DayClickedEventArgs mouseArgs = obj as DayClickedEventArgs;
            if (mouseArgs == null) return;

            AttendancePopup popup = new AttendancePopup(mouseArgs.Date, this.Students);
            if (popup.ShowDialog().Value)
            {
                this.AttendanceRecords.Add(new AttendanceRecord(mouseArgs.Date, popup.SelectedStudent,
                    popup.EAttendanceMember, popup.DocumentTitle, popup.SubmitDocument));
                (mouseArgs.Calendar as MyCalendar).BuildCalendarOutCaller(this.AttendanceRecords);
            }
        }

        private void OnCalendarDayDelete(object obj)
        {
            DayModifyEventArgs mouseArgs = obj as DayModifyEventArgs;
            if (mouseArgs == null) return;

            AttendanceRecord attendanceRecord = mouseArgs.DataContext as AttendanceRecord;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(attendanceRecord.Date.ToShortDateString());
            sb.AppendLine(attendanceRecord.StudentRecord.Name);
            sb.AppendLine(attendanceRecord.EAttendance.ToString());
            sb.AppendLine();
            sb.AppendLine("삭제하시겠습니까?");

            if (MessageBox.Show(sb.ToString(), "확인", MessageBoxButton.YesNo).Equals(MessageBoxResult.Yes))
            {
                this.AttendanceRecords.Remove(attendanceRecord);
                (mouseArgs.Calendar as MyCalendar).BuildCalendarOutCaller(this.AttendanceRecords);
            }
        }

        private void OnCalendarDayModify(object obj)
        {
            DayModifyEventArgs mouseArgs = obj as DayModifyEventArgs;
            if (mouseArgs == null) return;

            AttendancePopup popup = new AttendancePopup(mouseArgs.DataContext as AttendanceRecord, this.Students);
            if (popup.ShowDialog().Value)
            {
                AttendanceRecord attendanceRecord = mouseArgs.DataContext as AttendanceRecord;
                attendanceRecord.Date = popup.datePicker.SelectedDate.Value;
                attendanceRecord.StudentRecord = popup.SelectedStudent;
                attendanceRecord.EAttendance = popup.EAttendanceMember;
                attendanceRecord.DocumentTitle = popup.DocumentTitle;
                attendanceRecord.SubmitDocument = popup.SubmitDocument;

                (mouseArgs.Calendar as MyCalendar).BuildCalendarOutCaller(this.AttendanceRecords);
            }
        }

        #endregion
    }
}
