namespace BusinessLog.ViewModels
{
    using WpfBase.Bases;
    using WpfBase.Managers;

    public class BusinessLogMainVM : ViewModelBase
    {
        public BusinessLogMainVM()
        {

        }
        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewModelManager.AddValue(typeof(BusinessLogMainVM), this);
        }
    }
}
