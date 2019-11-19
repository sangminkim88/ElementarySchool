namespace TestSolution.ViewModels
{
    using Attendance.Views;
    using Consult.Views;
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using TestSolution.Views;
    using WpfBase.Bases;
    using WpfBase.Common;
    using WpfBase.Managers;

    public class MainWVM : ViewModelBase
    {
        #region Fields

        private string DEFAULT_TITLE = "초등학교에선 무슨일이!?";

        private static Brush ACTIVE_COLOR = new SolidColorBrush(Colors.Purple);

        private static Brush UNACTIVE_COLOR = new SolidColorBrush(Colors.Pink);

        private Brush attendanceColor;

        private Brush businessLogColor;

        private Brush consultColor;

        private WpfBase.Bases.ViewBase prevView;

        private string title;

        #endregion

        #region Constructors

        public MainWVM()
        {
            CommandShowMainViews = new RelayCommand(ExecuteShowMainViews);
            this.AttendanceColor = UNACTIVE_COLOR;
            this.ConsultColor = UNACTIVE_COLOR;
            this.BusinessLogColor = UNACTIVE_COLOR;
            this.title = DEFAULT_TITLE;
        }

        #endregion

        #region Properties

        public Brush AttendanceColor
        {
            get { return attendanceColor; }
            set { SetValue(ref attendanceColor, value); }
        }

        public Brush BusinessLogColor
        {
            get { return businessLogColor; }
            set { businessLogColor = value; }
        }

        public ICommand CommandShowMainViews { get; private set; }

        public Brush ConsultColor
        {
            get { return consultColor; }
            set { SetValue(ref consultColor, value); }
        }

        public string Title
        {
            get { return title; }
            set { SetValue(ref title, value); }
        }

        #endregion

        #region Methods

        private static void getView(object o, MainW mainW, out Type type, out PropertyInfo backColorProperty)
        {
            RoutedEventArgs e = o as RoutedEventArgs;
            int index = mainW.menuImagePanel.Children.IndexOf(e.Source as Button);
            type = null;
            backColorProperty = null;
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
        }

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewModelManager.AddValue(typeof(MainWVM), this);
        }

        private void ExecuteShowMainViews(object o)
        {
            MainW mainW = ViewManager.GetValue(typeof(MainW)) as MainW;
            Type type;
            PropertyInfo backColorProperty;
            getView(o, mainW, out type, out backColorProperty);

            WpfBase.Bases.ViewBase mainV = ViewManager.GetValue(type, false) as WpfBase.Bases.ViewBase;

            //처음 생성 시
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
                this.Title = mainV.Title;
            }
            //이미 생성된 이후
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
                    this.Title = mainV.Title;
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
                            this.Title = DEFAULT_TITLE;
                        }
                        else
                        {
                            this.prevView.Visibility = Visibility.Visible;
                            this.Title = prevView.Title;
                        }
                        backColorProperty.SetValue(this, UNACTIVE_COLOR);
                    }
                }
            }
        }

        #endregion
    }
}
