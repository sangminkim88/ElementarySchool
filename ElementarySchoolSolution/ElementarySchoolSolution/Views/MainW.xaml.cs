namespace TestSolution.Views
{
    using SampleProject.Views;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media.Animation;
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
        
        private void MenuShow_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.downMenu.Visibility = this.downMenu.Visibility.Equals(System.Windows.Visibility.Visible) ?
                System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;

            string animationName = this.downMenu.Visibility.Equals(System.Windows.Visibility.Visible) ?
                "sbHideMenu" : "sbShowMenu";

            Storyboard sb = Resources[animationName] as Storyboard;
            sb.Begin(this.menuPanel);

        }


        private void menuButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image image = sender as Image;
            image.Opacity = 0.4;
        }

        private void menuButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image image = sender as Image;
            image.Opacity = 0.9;
        }

        private void Button_Click_2(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(this.Cursor != Cursors.Wait)
            {
                Mouse.OverrideCursor = Cursors.Hand;
            }
        }

        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Wait)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }
    }
}
