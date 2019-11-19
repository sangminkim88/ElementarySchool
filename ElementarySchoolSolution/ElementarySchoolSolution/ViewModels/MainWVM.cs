namespace TestSolution.ViewModels
{
    using Attendance.Views;
    using Consult.Views;
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using TestSolution.Views;
    using WpfBase.Bases;
    using WpfBase.Common;
    using WpfBase.Managers;
    using System.Linq;
    using System.Windows.Media;
    using System.Reflection;

    public class MainWVM : ViewModelBase
    {
        private static Brush ACTIVE_COLOR = new SolidColorBrush(Colors.Purple);
        private static Brush UNACTIVE_COLOR = new SolidColorBrush(Colors.Pink);

        public ICommand CommandShowMainViews { get; private set; }

        private WpfBase.Bases.ViewBase prevView;

        private Brush attendanceColor;

        public Brush AttendanceColor
        {
            get { return attendanceColor; }
            set { SetValue(ref attendanceColor, value); }
        }

        private Brush consultColor;

        public Brush ConsultColor
        {
            get { return consultColor; }
            set { SetValue(ref consultColor, value); }
        }

        private Brush businessLogColor;

        public Brush BusinessLogColor
        {
            get { return businessLogColor; }
            set { businessLogColor = value; }
        }



        /// <summary>
        /// Executes ShowAttendance
        /// </summary>
        private void ExecuteShowMainViews(object o)
        {
            RoutedEventArgs e = o as RoutedEventArgs;
            MainW mainW = ViewManager.GetValue(typeof(MainW)) as MainW;

            int index = mainW.menuImagePanel.Children.IndexOf(e.Source as Button);
            Type type = null;
            PropertyInfo backColorProperty = null;
            switch (index)
            {
                case 0:
                    type = typeof(AttendceMainV).Assembly.GetType("Attendance.Views.AttendceMainV");
                    backColorProperty = typeof(MainWVM).GetProperty("AttendanceColor");
                    break;
                case 1:
                    type = typeof(ConsultMainV).Assembly.GetType("Consult.Views.ConsultMainV");
                    backColorProperty = typeof(MainWVM).GetProperty("ConsultColor");
                    break;
                case 2:
                    type = typeof(ConsultMainV).Assembly.GetType("Consult.Views.BusinessLogMainV");
                    backColorProperty = typeof(MainWVM).GetProperty("BusinessLogColor");
                    break;
            }

            WpfBase.Bases.ViewBase mainV = ViewManager.GetValue(type, false) as WpfBase.Bases.ViewBase;

            if (mainV == null)
            {
                mainV = ViewManager.GetValue(type) as WpfBase.Bases.ViewBase;

                foreach (var item in mainW.mainStage.Children)
                {
                    if (item is WpfBase.Bases.ViewBase)
                    {
                        WpfBase.Bases.ViewBase tmp = item as WpfBase.Bases.ViewBase;
                        if (tmp.Visibility.Equals(Visibility.Visible))
                        {
                            this.prevView = tmp;
                        }
                        tmp.Visibility = Visibility.Collapsed;
                    }
                }

                mainW.mainStage.Children.Add(mainV);
                backColorProperty.SetValue(this, ACTIVE_COLOR);
            }
            else
            {
                if (mainV.Visibility.Equals(Visibility.Collapsed))
                {
                    foreach (var item in mainW.mainStage.Children)
                    {
                        if (item is WpfBase.Bases.ViewBase)
                        {
                            WpfBase.Bases.ViewBase tmp = item as WpfBase.Bases.ViewBase;
                            if (tmp.Visibility.Equals(Visibility.Visible))
                            {
                                this.prevView = tmp;
                            }
                            tmp.Visibility = Visibility.Collapsed;
                        }
                    }

                    mainV.Visibility = Visibility.Visible;

                }
                else
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("현재 창을 닫으시겠습니까?\n저장하지 않은 정보는 사라집니다.", "경고",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                    if (messageBoxResult.Equals(MessageBoxResult.Yes))
                    {
                        ViewManager.RemoveValue(type);
                        mainW.mainStage.Children.Remove(mainV);

                        if (mainW.mainStage.Children.Count.Equals(0))
                        {
                            this.prevView = null;
                        }
                        else
                        {
                            this.prevView.Visibility = Visibility.Visible;
                        }
                        backColorProperty.SetValue(this, UNACTIVE_COLOR);
                    }
                }
            }
        }


        public MainWVM()
        {
            CommandShowMainViews = new RelayCommand(ExecuteShowMainViews);
            this.AttendanceColor = UNACTIVE_COLOR;
            this.ConsultColor = UNACTIVE_COLOR;
            this.BusinessLogColor = UNACTIVE_COLOR;
        }

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewModelManager.AddValue(typeof(MainWVM), this);
        }



    }
}
