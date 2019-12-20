namespace TestSolution.ViewModels
{
    using Attendance.Views;
    using BusinessLog.Views;
    using Consult.Views;
    using Settings.Views;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using TestSolution.Views;
    using WpfBase.Bases;
    using WpfBase.Common;
    using WpfBase.Managers;
    using ViewBase = WpfBase.Bases.ViewBase;

    public class MainWVM : ViewModelBase
    {
        #region Fields

        private static Brush ACTIVE_COLOR = new SolidColorBrush(Colors.Gold);

        private static Brush CURRENT_ACTIVE_COLOR = new SolidColorBrush(Colors.MediumBlue);

        private static Brush UNACTIVE_COLOR = new SolidColorBrush(Colors.Pink);

        private string DEFAULT_TITLE = "초등학교에선 무슨일이!?  v__1.1";

        private string title;

        private LinkedList<ViewBase> viewStack = new LinkedList<ViewBase>();

        #endregion

        #region Constructors

        public MainWVM()
        {
            CommandShowMainViews = new RelayCommand(ExecuteShowMainViews);

            MenuColor.Add(UNACTIVE_COLOR);
            MenuColor.Add(UNACTIVE_COLOR);
            MenuColor.Add(UNACTIVE_COLOR);
            MenuColor.Add(UNACTIVE_COLOR);

            this.title = DEFAULT_TITLE;
        }

        #endregion

        #region Properties

        public ICommand CommandShowMainViews { get; private set; }

        public ObservableCollection<Brush> MenuColor { get; set; } = new ObservableCollection<Brush>();

        public string Title
        {
            get { return title; }
            set { SetValue(ref title, value); }
        }

        #endregion

        #region Methods

        private static void getView(object o, MainW mainW, out Type type)
        {
            RoutedEventArgs e = o as RoutedEventArgs;
            int index = mainW.menuImagePanel.Children.IndexOf(e.Source as Button);
            type = null;
            switch (index)
            {
                case 0:
                    type = typeof(AttendanceMainV).Assembly.GetType("Attendance.Views.AttendanceMainV");
                    break;
                case 1:
                    type = typeof(ConsultMainV).Assembly.GetType("Consult.Views.ConsultMainV");
                    break;
                case 2:
                    type = typeof(BusinessLogMainV).Assembly.GetType("BusinessLog.Views.BusinessLogMainV");
                    break;
                default:
                    type = typeof(SettingsMainV).Assembly.GetType("Settings.Views.SettingsMainV");
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
            getView(o, mainW, out type);
            ViewBase mainV = ViewManager.GetValue(type) as ViewBase;
            if (((ViewModelBase)mainV.DataContext).IsGoodInit)
            {
                //현재 화면일 경우
                if (this.viewStack.Last != null && this.viewStack.Last.Value.Equals(mainV))
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("현재 창을 닫으시겠습니까?\n저장하지 않은 정보는 사라집니다.", "경고",
                                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                    if (messageBoxResult.Equals(MessageBoxResult.Yes))
                    {
                        this.MenuColor[mainV.MenuIndex] = UNACTIVE_COLOR;

                        mainW.mainStage.Children.Remove(mainV);
                        this.viewStack.RemoveLast();
                        ViewManager.RemoveValue(type);
                        ViewModelManager.RemoveValue(mainV.DataContext.GetType());

                        if (this.viewStack.Last != null)
                        {
                            ViewBase oldView = this.viewStack.Last.Value;

                            this.MenuColor[oldView.MenuIndex] = CURRENT_ACTIVE_COLOR;
                            oldView.Visibility = Visibility.Visible;
                        }
                    }
                }
                //과거 화면일 경우
                else if (this.viewStack.Contains(mainV))
                {
                    ViewBase oldView = this.viewStack.Last.Value;

                    this.MenuColor[oldView.MenuIndex] = ACTIVE_COLOR;
                    oldView.Visibility = Visibility.Collapsed;

                    this.viewStack.Remove(mainV);
                    this.viewStack.AddLast(mainV);

                    this.MenuColor[mainV.MenuIndex] = CURRENT_ACTIVE_COLOR;
                    mainV.Visibility = Visibility.Collapsed;
                }
                //처음 켜는 경우
                else
                {
                    if (this.viewStack.Last != null)
                    {
                        ViewBase oldView = this.viewStack.Last.Value;

                        this.MenuColor[oldView.MenuIndex] = ACTIVE_COLOR;
                        oldView.Visibility = Visibility.Collapsed;
                    }
                    this.viewStack.AddLast(mainV);

                    mainW.mainStage.Children.Add(mainV);
                    this.MenuColor[mainV.MenuIndex] = CURRENT_ACTIVE_COLOR;
                }

                if (this.viewStack.Last != null)
                {
                    ViewBase newView = this.viewStack.Last.Value;
                    this.Title = newView.Title;
                    DoubleAnimation show = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1));
                    Storyboard.SetTarget(show, newView);
                    Storyboard.SetTargetProperty(show, new PropertyPath(UserControl.OpacityProperty));
                    Storyboard sb = new Storyboard();
                    sb.Children.Add(show);
                    sb.Begin();
                }
                else
                {
                    this.Title = DEFAULT_TITLE;
                }
            }
        }

        #endregion
    }
}
