
using WpfBase.Bases;
using WpfBase.Managers;

namespace Attendance.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MainV : ViewBase
    {
        public MainV()
        {
            InitializeComponent();
        }

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewManager.AddValue(typeof(MainV), this);
        }
    }
}
