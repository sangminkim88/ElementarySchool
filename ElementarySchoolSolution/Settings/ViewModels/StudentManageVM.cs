namespace Settings.ViewModels
{
    using Common;
    using Common.Models;
    using FileManager;
    using Microsoft.Win32;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Input;
    using WpfBase.Bases;
    using WpfBase.Common;

    public class StudentManageVM : ViewModelBase
    {
        #region Fields

        private string currentStudentFilePath = string.Empty;

        private ObservableCollection<Student> students = new ObservableCollection<Student>();

        #endregion

        #region Constructors

        public StudentManageVM()
        {
            Import = new RelayCommand(ExcuteImport);
            Export = new RelayCommand(ExcuteExport);
            Save = new RelayCommand(ExcuteSave);
            Initial = new RelayCommand(ExcuteInitial);

            this.CurrentStudentFilePath = ConfigManager.ReadProfileString(EConfigSection.Students.ToString(), 
                EConfigKey.FilePath.ToString(), this.CurrentStudentFilePath);
            if (this.CurrentStudentFilePath != null || !this.CurrentStudentFilePath.Equals(string.Empty))
            {
                this.setStudents(XmlManager.Deserialize(this.CurrentStudentFilePath, this.Students.GetType()) as ObservableCollection<Student>);
            }
        }

        #endregion

        #region Properties

        public string CurrentStudentFilePath
        {
            get { return currentStudentFilePath; }
            set { SetValue(ref currentStudentFilePath, value); }
        }

        public ICommand Export { get; private set; }

        public ICommand Import { get; private set; }

        public ICommand Initial { get; private set; }

        public ICommand Save { get; private set; }

        public ObservableCollection<Student> Students
        {
            get { return students; }
            set { students = value; }
        }

        #endregion

        #region Methods

        private void ExcuteExport(object obj)
        {
            string destination = null;

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

            if (XmlManager.Serialize(this.Students, destination))
            {
                MessageBox.Show("Export 되었습니다.", "성공", MessageBoxButton.OK, MessageBoxImage.None);
            }
            else
            {
                MessageBox.Show("Export 실패하였습니다.", "실패", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExcuteImport(object obj)
        {
            if (!this.CurrentStudentFilePath.Length.Equals(0) && MessageBox.Show("변경한 내용은 저장되지 않습니다. 그래도 진행하시겠습니까?", "경고",
               MessageBoxButton.YesNo, MessageBoxImage.Warning).Equals(MessageBoxResult.No))
            {
                return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "dat Files (*.dat)|*.dat|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog().Equals(true))
            {
                this.setStudents(XmlManager.Deserialize(openFileDialog.FileName, this.Students.GetType()) as ObservableCollection<Student>);

                this.CurrentStudentFilePath = openFileDialog.FileName;

                ConfigManager.WriteProfileString(EConfigSection.Students.ToString(), 
                    EConfigKey.FilePath.ToString(), this.CurrentStudentFilePath);
            }
        }

        private void ExcuteInitial(object obj)
        {
            if (this.CurrentStudentFilePath.Length.Equals(0))
            {
                MessageBox.Show("Import된 파일이 없습니다.", "실패", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (MessageBox.Show("변경한 내용은 저장되지 않습니다. 그래도 진행하시겠습니까?", "경고",
                MessageBoxButton.YesNo, MessageBoxImage.Warning).Equals(MessageBoxResult.Yes))
            {
                this.setStudents(XmlManager.Deserialize(this.CurrentStudentFilePath, this.Students.GetType()) as ObservableCollection<Student>);                
            }
        }

        private void ExcuteSave(object obj)
        {
            if (this.CurrentStudentFilePath.Length.Equals(0))
            {
                MessageBox.Show("Import된 파일이 없습니다. Export로 먼저 파일을 만드세요.", "실패",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (MessageBox.Show("변경한 내용을 저장하시겠습니까?", "확인",
                MessageBoxButton.YesNo, MessageBoxImage.Information).Equals(MessageBoxResult.Yes))
            {
                XmlManager.Serialize(this.Students, this.CurrentStudentFilePath);
                ConfigManager.WriteProfileString(EConfigSection.Students.ToString(), 
                    EConfigKey.FilePath.ToString(), this.CurrentStudentFilePath);
            }
        }

        private void setStudents(ObservableCollection<Student> data)
        {
            this.Students.Clear();
            foreach (var item in data)
            {
                this.Students.Add(item);
            }
        }

        #endregion
    }
}
