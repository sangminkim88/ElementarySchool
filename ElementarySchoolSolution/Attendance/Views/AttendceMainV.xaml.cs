
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
    }
}
