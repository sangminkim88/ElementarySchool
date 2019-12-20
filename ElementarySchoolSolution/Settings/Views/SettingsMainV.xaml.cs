namespace Settings.Views
{
    using Settings.ViewModels;
    using WpfBase.Bases;
    using WpfBase.Managers;

    public partial class SettingsMainV : ViewBase
    {
        #region Constructors

        public SettingsMainV()
        {
            InitializeComponent();
            this.Title = "설정";
            this.MenuIndex = 3;
            this.DataContext = new SettingsMainVM(this.settingMainStage);
        }

        #endregion

        #region Methods

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewManager.AddValue(typeof(SettingsMainV), this);
        }

        #endregion
    }
}
