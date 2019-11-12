
using WpfBase.Bases;
using WpfBase.Managers;

namespace SampleProject.VIews
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class FirstV : ViewBase
    {
        public FirstV()
        {
            InitializeComponent();
        }

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewManager.AddValue(typeof(FirstV), this);
        }
    }
}
