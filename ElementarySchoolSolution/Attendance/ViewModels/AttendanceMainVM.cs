namespace Attendance.ViewModels
{
    using System.Windows.Input;
    using WpfBase.Bases;
    using WpfBase.Common;
    using WpfBase.Managers;

    public class AttendanceMainVM : ViewModelBase
    {
        public ICommand CalendarDayClickedCommand { get; private set; }

        public AttendanceMainVM()
        {
            CalendarDayClickedCommand = new RelayCommand(OmCalendarDayClicked);

        }
        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewModelManager.AddValue(typeof(AttendanceMainVM), this);
        }

        private void OmCalendarDayClicked(object obj)
        {
            var mouseArgs = obj as MouseButtonEventArgs;
            if (mouseArgs == null)
                return;
            //test
        }



    }
}
