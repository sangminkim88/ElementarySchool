namespace Attendance.Views
{
    using FileManager;
    using Microsoft.Win32;
    using System.Windows.Input;
    using WpfBase.Bases;
    using WpfBase.Managers;

    public partial class AttendceMainV : ViewBase
    {
        #region Constructors

        public AttendceMainV()
        {
            InitializeComponent();
            this.Title = "출석관리";
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

        #endregion
    }
}
