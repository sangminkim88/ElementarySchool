namespace TestSolution.ViewModels
{
    using WpfBase.Bases;
    using WpfBase.Managers;

    public class MainWVM : ViewModelBase
    {
        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewModelManager.AddValue(typeof(MainWVM), this);
        }
    }
}
