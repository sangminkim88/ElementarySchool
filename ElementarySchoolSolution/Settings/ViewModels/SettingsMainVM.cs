namespace Settings.ViewModels
{
    using WpfBase.Bases;
    using WpfBase.Managers;

    public class SettingsMainVM : ViewModelBase
    {
        public SettingsMainVM()
        {

        }
        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewModelManager.AddValue(typeof(SettingsMainVM), this);
        }
    }
}
