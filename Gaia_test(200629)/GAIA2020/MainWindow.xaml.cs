namespace GAIA2020
{
    using GAIA2020.Utilities;
    using HMFrameWork.Ancestor;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// Defines the <see cref="MainWindow" />.
    /// </summary>
    public partial class MainWindow : AWindow
    {
        #region Fields

        /// <summary>
        /// Defines the maximizeByClick.
        /// </summary>
        private bool maximizeByClick;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            //상민(200625)
            //TextPolygon에서 Loaded 함수 내 VisualTreeHelper.GetDescendantBounds 오류로 추가함
            //디버그 모드에서는 발생하지 않지만 릴리즈모드에서는 반환값이 의도와는 다른 값으로 나와 다운됨
            //비주얼트리를 구성하기 위해 생성을 미리 함
            var tmp = App.GetViewManager().APartDescView;

            NotifyHelper.Instance.SetLogFileDirectory(ConfigHelper.GetProgramFilePath());
        }

        #endregion

        #region Methods

        /// <summary>
        /// The Window_Close.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        protected override void Window_Close(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }

        /// <summary>
        /// The _MouseEnter.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.Windows.Input.MouseEventArgs"/>.</param>
        private void _MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Wait)
            {
                Mouse.OverrideCursor = Cursors.Hand;
            }
        }

        /// <summary>
        /// The _MouseLeave.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.Windows.Input.MouseEventArgs"/>.</param>
        private void _MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Wait)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// The AWindow_SizeChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="SizeChangedEventArgs"/>.</param>
        private void AWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowStyle = WindowStyle.SingleBorderWindow;
                WindowStyle = WindowStyle.None;
            }
        }

        /// <summary>
        /// The Grid_MouseLeftButtonDown.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.Windows.Input.MouseButtonEventArgs"/>.</param>
        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            if (e.ClickCount == 2)
            {
                if (WindowState == WindowState.Maximized)
                {
                    WindowState = WindowState.Normal;
                }
                else if (WindowState != WindowState.Maximized)
                {
                    WindowState = WindowState.Maximized;
                    maximizeByClick = true;
                }
            }
            else
            {
                if (WindowState is WindowState.Normal) DragMove();
            }
        }

        /// <summary>
        /// The Grid_MouseMove.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.Windows.Input.MouseEventArgs"/>.</param>
        private void Grid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (maximizeByClick)
            {
                maximizeByClick = false;
                return;
            }
            if (e.LeftButton.Equals(MouseButtonState.Pressed) &&
                    WindowState.Equals(WindowState.Maximized))
            {
                Point point = Mouse.GetPosition(this);
                double percent = point.X / (this.Width / 100);
                WindowState = WindowState.Normal;

                this.Left = point.X - ((this.Width / 100) * percent);
                this.Top = -point.Y;

                DragMove();
            }
        }

        #endregion
    }
}
