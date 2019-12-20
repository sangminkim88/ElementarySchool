
using WpfBase.Bases;
using WpfBase.Managers;

namespace Consult.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ConsultMainV : ViewBase
    {
        public ConsultMainV()
        {
            InitializeComponent();
            this.Title = "상담관리";
            this.MenuIndex = 1;
        }

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewManager.AddValue(typeof(ConsultMainV), this);
        }

        private void Test_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //this.test.Content = int.Parse(this.test.Content.ToString()) + 1;
        }
    }
}
