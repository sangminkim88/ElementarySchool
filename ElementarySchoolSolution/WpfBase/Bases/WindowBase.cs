namespace WpfBase.Bases
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Input;
    using WpfBase.Common;
    using WpfBase.Interface;

    public abstract class WindowBase : Window, IView, ISupportInitialize
    {
        #region Constructors

        public WindowBase()
        {
            CommandMouseLeftButtonDown = new RelayCommand(OnMouseLeftButtonDown);

            CommandWindowMin = new RelayCommand(OnWindowMin);
            CommandWindowMax = new RelayCommand(OnWindowMax);
            CommandWindowClose = new RelayCommand(OnWindowClose);

            LocationChanged += Window_LocationChanged;
        }

        #endregion

        #region Properties

        public ICommand CommandMouseLeftButtonDown { get; private set; }

        public ICommand CommandWindowClose { get; private set; }

        public ICommand CommandWindowMax { get; private set; }

        public ICommand CommandWindowMin { get; private set; }

        #endregion

        #region Methods

        public override abstract void BeginInit();

        public override abstract void EndInit();

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected virtual void Title_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        protected virtual void Window_Minimize(object sender, RoutedEventArgs e)
        {
        }

        protected virtual void Window_Restore(object sender, RoutedEventArgs e)
        {
            WindowState = (WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
        }

        private void OnMouseLeftButtonDown(object obj)
        {
            var mouseArgs = obj as MouseButtonEventArgs;
            if (mouseArgs == null)
                return;

            this.Title_MouseLeftButtonDown(this, mouseArgs);
        }

        private void OnWindowClose(object obj)
        {
            this.Close();
        }

        private void OnWindowMax(object obj)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void OnWindowMin(object obj)
        {
            Window_Minimize(this, null);
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        #endregion
    }
}
