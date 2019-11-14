using System.Windows;
using WpfBase.Bases;
using WpfBase.Managers;

namespace SampleProject.Views
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class FirstW : WindowBase
    {
        public FirstW()
        {
            InitializeComponent();
        }

        #region  ISupportInitialize
        public override void BeginInit() { }
        public override void EndInit()
        {
            ViewManager.AddValue(typeof(FirstW), this);
        }

        #endregion

    }
}
