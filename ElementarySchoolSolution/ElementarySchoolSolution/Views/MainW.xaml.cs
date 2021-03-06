﻿namespace TestSolution.Views
{
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

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewManager.AddValue(typeof(MainW), this);
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.MenuShow_PreviewMouseLeftButtonUp(null, null);
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Wait)
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

        private void menuButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image image = sender as Image;
            image.Opacity = 0.7;
            if (image.Name.Equals("businessLog"))
            {
                image.Opacity = 1.0;
            }
        }

        private void menuButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Image image = sender as Image;
            image.Opacity = 0.4;
            if (image.Name.Equals("businessLog"))
            {
                image.Opacity = 0.6;
            }
        }

        private void MenuShow_PreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.downMenu.Visibility = this.downMenu.Visibility.Equals(System.Windows.Visibility.Visible) ?
                System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;

            string animationName = this.downMenu.Visibility.Equals(System.Windows.Visibility.Visible) ?
                "sbHideMenu" : "sbShowMenu";

            Storyboard sb = Resources[animationName] as Storyboard;
            sb.Begin(this.menuPanel);
        }

        #endregion
    }
}
