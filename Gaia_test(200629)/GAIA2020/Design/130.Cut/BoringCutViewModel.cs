namespace GAIA2020.Design
{
    using HMFrameWork.Ancestor;

    public class BoringCutViewModel : AViewModel
    {
        #region Constructors

        public BoringCutViewModel()
        {
        }

        #endregion

        #region Methods

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            App.GetViewModelManager().AddValue(typeof(BoringCutViewModel), this);
        }

        #endregion
    }
}
