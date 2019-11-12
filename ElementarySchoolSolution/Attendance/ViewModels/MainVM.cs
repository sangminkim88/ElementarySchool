namespace Attendance.ViewModels
{
    using WpfBase.Bases;
    using WpfBase.Managers;

    public class MainVM : ViewModelBase
    {
        public MainVM()
        {

        }
        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewModelManager.AddValue(typeof(MainVM), this);
        }
    }
}
