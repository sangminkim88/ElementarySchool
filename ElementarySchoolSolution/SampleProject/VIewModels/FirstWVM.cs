namespace SampleProject.ViewModels
{
    using WpfBase.Bases;
    using WpfBase.Managers;

    public class FirstWVM : ViewModelBase
    {
        public FirstWVM()
        {

        }
        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewModelManager.AddValue(typeof(FirstWVM), this);
        }
    }
}
