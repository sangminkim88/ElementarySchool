namespace Gaia
{
    using Gaia.Views;
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;

    public partial class MainWindow : Window, INotifyPropertyChanged, GaiaDB.IMainFrame
    {
        private Action<string, string, string, string, string> loadMainViewAction;

        //
        public System.Drawing.Bitmap Get_PreViewImage()
        {
            // 썸내일 이미지 파일 만들것
            return null;
        }


        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            double width = SystemParameters.PrimaryScreenWidth;
            if (width < 1920) {
                this.MaxWidth = width;
                this.MaxHeight = this.MaxWidth / 16 * 9;
            }
            this.loadMainViewAction += loadMainView;
            this.mainStage.Children.Add(new InitView(loadMainViewAction));
        }
        
        private void loadMainView(string a, string b, string c, string d, string e)
        {
            this.mainStage.Children.RemoveAt(2);
            this.mainStage.Children.Add(new MainView(a, b, c, d, e));

            GaiaDB.MainFrame_Imp.Set_MainFrame(this); // 하위 프로젝트(GIDB등)에서 접근을 위해서 MainFrame을 등록시켜줍니다.
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Methods

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed_Close(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void CommandBinding_Executed_Restore(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.WindowState.Equals(WindowState.Maximized))
            {
                SystemCommands.RestoreWindow(this);
            }
            else
            {
                SystemCommands.MaximizeWindow(this);
            }
        }

        private void CommandBinding_Executed_Minimize(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        #endregion

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
                }
            }
            else
            {
                if (WindowState is WindowState.Normal) DragMove();
            }
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Wait)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Wait)
            {
                Mouse.OverrideCursor = Cursors.Hand;
            }
        }
    }
}
