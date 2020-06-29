namespace GAIA2020.Design
{
    using HMFrameWork.Ancestor;
    using HMFrameWork.Command;
    using System.Windows.Input;

    public class BoringBridgeViewModel : AViewModel
    {
        #region Fields

        private AViewModel currentVM;

        #endregion

        #region Constructors

        public BoringBridgeViewModel()
        {
            CommandViewChangeTest = new RelayCommand(ViewChangeTest);
            this.CurrentVM = App.GetViewModelManager().GetValue(typeof(APartDescViewModel));
        }

        #endregion

        #region Properties

        public ICommand CommandViewChangeTest { get; private set; }

        public AViewModel CurrentVM
        {
            get { return currentVM; }
            set
            {
                SetValue(ref currentVM, value);
            }
        }

        #endregion

        #region Methods

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            App.GetViewModelManager().AddValue(typeof(BoringBridgeViewModel), this);
        }

        private void ViewChangeTest(object obj)
        {
            var mngr = App.GetViewModelManager();
            if (obj.Equals("0"))
            {
                this.CurrentVM = mngr.GetValue(typeof(APartDescViewModel));
            }
            else
            {
                B1PartDescViewModel b1PartDescViewModel = mngr.GetValue(typeof(B1PartDescViewModel)) as B1PartDescViewModel;
                this.CurrentVM = b1PartDescViewModel;                
            }
        }

        #endregion
    }
}
