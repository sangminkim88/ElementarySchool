
using WpfBase.Bases;
using WpfBase.Managers;

namespace Settings.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class SettingsMainV : ViewBase
    {
        public SettingsMainV()
        {
            InitializeComponent();
            this.Title = "설정";
        }

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewManager.AddValue(typeof(SettingsMainV), this);
        }
    }
}
