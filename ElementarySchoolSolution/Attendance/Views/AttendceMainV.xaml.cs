
using System.Windows.Input;
using WpfBase.Bases;
using WpfBase.Managers;

namespace Attendance.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class AttendceMainV : ViewBase
    {
        public AttendceMainV()
        {
            InitializeComponent();
        }

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewManager.AddValue(typeof(AttendceMainV), this);
        }

        private void Test_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.test.Content = int.Parse(this.test.Content.ToString()) + 1;
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
    }
}
