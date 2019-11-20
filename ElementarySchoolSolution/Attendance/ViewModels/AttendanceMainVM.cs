namespace Attendance.ViewModels
{
    using Common.Models;
    using FileManager;
    using Microsoft.Win32;
    using MyCalendar;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;
    using WpfBase.Bases;
    using WpfBase.Common;
    using WpfBase.Managers;

    public class AttendanceMainVM : ViewModelBase
    {
        #region Fields

        private List<AttendanceRecord> attendanceRecords = new List<AttendanceRecord>();

        private string currentFileName;

        private bool isModified;

        #endregion

        #region Constructors

        public AttendanceMainVM()
        {
            CalendarDayClickedCommand = new RelayCommand(OmCalendarDayClicked);
            SaveCommand = new RelayCommand(ExecuteSave);
            ImportCommand = new RelayCommand(ExecuteImport);
            ExportCommand = new RelayCommand(ExecuteExport);

            //ConfigManager.WriteProfileString("Attendance", "onLoadFilePath", "Test~");
            var findfolder = "";
            findfolder = ConfigManager.ReadProfileString("Attendance", "onLoadFilePath", findfolder);
        }

        #endregion

        #region Properties

        public List<AttendanceRecord> AttendanceRecords
        {
            get { return attendanceRecords; }
            set { attendanceRecords = value; }
        }

        public ICommand CalendarDayClickedCommand { get; private set; }

        public ICommand ExportCommand { get; private set; }

        public ICommand ImportCommand { get; private set; }

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

        private void ExecuteExport(object o)
        {
            string destination = null;
            if (o != null && o.GetType().Equals(typeof(bool)) && ((bool)o).Equals(true))
            {
                destination = this.currentFileName;
            }
            else
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "dat Files (*.dat)|*.dat|All Files (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == true)
                {
                    destination = saveFileDialog.FileName;
                }
                else
                {
                    return;
                }
            }
            XmlManager.Serialize(this.attendanceRecords, destination);
        }

        private void ExecuteImport(object o)
        {
            if (this.isModified)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("수정사항이 있습니다. 저장하시겠습니까?", "경고",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                if (messageBoxResult.Equals(MessageBoxResult.Cancel))
                {
                    return;
                }
                else if (messageBoxResult.Equals(MessageBoxResult.Yes))
                {
                    this.ExecuteSave(null);
                }
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "dat Files (*.dat)|*.dat|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog().Equals(true))
            {
                this.currentFileName = openFileDialog.FileName;

                this.attendanceRecords = (List<AttendanceRecord>)XmlManager.Deserialize(this.currentFileName, typeof(List<AttendanceRecord>));

                MyCalendar calendar = o as MyCalendar;
                calendar.BuildCalendarOutCaller(this.attendanceRecords);
                this.isModified = false;
            }
        }

        private void ExecuteSave(object o)
        {
            this.ExecuteExport(true);
        }

        private void OmCalendarDayClicked(object obj)
        {
            var mouseArgs = obj as MouseButtonEventArgs;
            if (mouseArgs == null) return;

        }

        #endregion
    }
}
