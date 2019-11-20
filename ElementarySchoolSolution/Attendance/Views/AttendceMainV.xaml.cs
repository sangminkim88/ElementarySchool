namespace Attendance.Views
{
    using Attendance.ViewModels;
    using Microsoft.Win32;
    using System.Windows;
    using System.Windows.Input;
    using WpfBase.Bases;
    using WpfBase.Managers;

    public partial class AttendceMainV : ViewBase
    {
        #region Fields

        private string currentFilePath;

        private bool isModified;

        #endregion

        #region Constructors

        public AttendceMainV()
        {
            InitializeComponent();
            this.Title = "출석관리";
        }

        #endregion

        #region Properties

        public bool IsModified
        {
            get { return isModified; }
            set { isModified = value; }
        }

        #endregion

        #region Methods

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewManager.AddValue(typeof(AttendceMainV), this);
        }

        private void Export_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "dat Files (*.dat)|*.dat|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == true)
            {
                //string a = saveFileDialog.FileName;
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

        private void Import_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            (DataContext as AttendanceMainVM).SetCalendarData(this.calendar);            

            //if (this.isModified)
            //{
            //    MessageBoxResult messageBoxResult = MessageBox.Show("수정사항이 있습니다. 저장하시겠습니까?", "경고",
            //        MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            //    if (messageBoxResult.Equals(MessageBoxResult.Cancel))
            //    {
            //        return;
            //    }
            //    else if (messageBoxResult.Equals(MessageBoxResult.Yes))
            //    {
            //        (DataContext as AttendanceMainVM).SaveCommand.Execute(null);
            //    }
            //}
            //OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.Filter = "dat Files (*.dat)|*.dat|All Files (*.*)|*.*";

            //if (openFileDialog.ShowDialog().Equals(true))
            //{
            //    this.currentFilePath = openFileDialog.FileName;

            //}
        }

        #endregion
    }
}

