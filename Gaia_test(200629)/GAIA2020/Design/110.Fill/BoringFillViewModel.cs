namespace GAIA2020.Design
{
    using HMFrameWork.Ancestor;

    public class BoringFillViewModel : AViewModel
    {
        #region Constructors

        public BoringFillViewModel()
        {
        }

        #endregion

        #region Methods

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            App.GetViewModelManager().AddValue(typeof(BoringFillViewModel), this);
        }

        #endregion
    }
}
