namespace Attendance.ViewModels
{
    using WpfBase.Bases;
    using WpfBase.Managers;

    public class AttendanceMainVM : ViewModelBase
    {
        public AttendanceMainVM()
        {

        }
        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewModelManager.AddValue(typeof(AttendanceMainVM), this);
        }
    }
}
