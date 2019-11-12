namespace SampleProject.ViewModels
{
    using WpfBase.Bases;
    using WpfBase.Managers;

    public class FirstVM : ViewModelBase
    {
        public FirstVM()
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
