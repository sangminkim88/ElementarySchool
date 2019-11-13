namespace TestSolution.Views
{
    using SampleProject.Views;
    using WpfBase.Bases;
    using WpfBase.Managers;

    public partial class MainW : WindowBase
    {
        #region Constructors

        public MainW()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        public override void EndInit()
        {
            ViewManager.AddValue(typeof(MainW), this);
        }

        public override void BeginInit()
        {
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FirstW fv = new FirstW();
            fv.Show();
        }

        #endregion

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            Attendance.Views.MainV view = new Attendance.Views.MainV();
            this.panel.Children.Add(view);
        }
    }
}
