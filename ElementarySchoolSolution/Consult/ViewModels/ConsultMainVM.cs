namespace Consult.ViewModels
{
    using WpfBase.Bases;
    using WpfBase.Managers;

    public class ConsultMainVM : ViewModelBase
    {
        public ConsultMainVM()
        {

        }
        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewModelManager.AddValue(typeof(ConsultMainVM), this);
        }
    }
}
