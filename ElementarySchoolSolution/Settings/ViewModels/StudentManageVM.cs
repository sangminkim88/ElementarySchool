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

        private string currentFilePath = string.Empty;

        private ObservableCollection<Student> students = new ObservableCollection<Student>();

        #endregion

        #region Constructors

        public StudentManageVM()
        {
            Import = new RelayCommand(ExcuteImport);
            AllClear = new RelayCommand(ExcuteAllClear);
            Export = new RelayCommand(ExcuteExport);
            Save = new RelayCommand(ExcuteSave);
            Initial = new RelayCommand(ExcuteInitial);

            this.CurrentFilePath = ConfigManager.ReadProfileString(EConfigSection.Students.ToString(),
                EConfigKey.FilePath.ToString(), this.CurrentFilePath);
            if (this.CurrentFilePath != null && !this.CurrentFilePath.Equals(string.Empty))
            {
                var a = XmlManager.Deserialize(this.CurrentFilePath, this.Students.GetType()) as ObservableCollection<Student>;

                this.setStudents(a);
            }
        }

        #endregion

        #region Properties

        public ICommand AllClear { get; private set; }

        public string CurrentFilePath
        {
            get { return currentFilePath; }
            set { SetValue(ref currentFilePath, value); }
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

        private void ExcuteAllClear(object obj)
        {
            if (MessageBox.Show("모든 내용을 지우시겠습니까?", "경고",
     MessageBoxButton.YesNo, MessageBoxImage.Warning).Equals(MessageBoxResult.Yes))
            {
                this.Students.Clear();
                this.CurrentFilePath = string.Empty;
                ConfigManager.WriteProfileString(EConfigSection.Students.ToString(), EConfigKey.FilePath.ToString(), this.CurrentFilePath);
            }
        }

        private void ExcuteExport(object obj)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "dat Files (*.dat)|*.dat|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                string filePath = saveFileDialog.FileName;
                if (XmlManager.Serialize(this.Students, filePath))
                {
                    MessageBox.Show("내보내기 되었습니다.", "성공", MessageBoxButton.OK, MessageBoxImage.None);
                    this.CurrentFilePath = filePath;

                    ConfigManager.WriteProfileString(EConfigSection.Students.ToString(),
                        EConfigKey.FilePath.ToString(), this.CurrentFilePath);
                }
                else
                {
                    MessageBox.Show("내보내기 실패하였습니다.", "실패", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ExcuteImport(object obj)
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
                var data = XmlManager.Deserialize(openFileDialog.FileName, this.Students.GetType());

                if (data == null)
                {
                    MessageBox.Show("학생정보 파일이 아닙니다.", "에러", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                this.setStudents(data as ObservableCollection<Student>);

                this.CurrentFilePath = openFileDialog.FileName;

                ConfigManager.WriteProfileString(EConfigSection.Students.ToString(),
                    EConfigKey.FilePath.ToString(), this.CurrentFilePath);
            }
        }

        private void ExcuteInitial(object obj)
        {
            if (this.CurrentFilePath.Length.Equals(0))
            {
                MessageBox.Show("작업중인 파일이 없습니다.", "실패", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (MessageBox.Show("변경한 내용은 저장되지 않습니다. 그래도 진행하시겠습니까?", "경고",
                MessageBoxButton.YesNo, MessageBoxImage.Warning).Equals(MessageBoxResult.Yes))
            {
                this.setStudents(XmlManager.Deserialize(this.CurrentFilePath, this.Students.GetType()) as ObservableCollection<Student>);
            }
        }

        private void ExcuteSave(object obj)
        {
            if (this.CurrentFilePath.Length.Equals(0))
            {
                this.ExcuteExport(null);
            }
            else if (MessageBox.Show("변경한 내용을 저장하시겠습니까?", "확인",
                MessageBoxButton.YesNo, MessageBoxImage.Information).Equals(MessageBoxResult.Yes))
            {
                XmlManager.Serialize(this.Students, this.CurrentFilePath);
                ConfigManager.WriteProfileString(EConfigSection.Students.ToString(),
                    EConfigKey.FilePath.ToString(), this.CurrentFilePath);
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
