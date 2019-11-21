
using WpfBase.Bases;
using WpfBase.Managers;

namespace BusinessLog.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class BusinessLogMainV : ViewBase
    {
        public BusinessLogMainV()
        {
            InitializeComponent();
            this.Title = "업무관리";
        }

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewManager.AddValue(typeof(BusinessLogMainV), this);
        }

        private void Test_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.test.Content = int.Parse(this.test.Content.ToString()) + 1;
        }
    }
}
