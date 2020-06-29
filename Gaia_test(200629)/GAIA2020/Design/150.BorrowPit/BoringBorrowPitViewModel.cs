namespace GAIA2020.Design
{
    using HMFrameWork.Ancestor;

    public class BoringBorrowPitViewModel : AViewModel
    {
        #region Constructors

        public BoringBorrowPitViewModel()
        {
        }

        #endregion

        #region Methods

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            App.GetViewModelManager().AddValue(typeof(BoringBorrowPitViewModel), this);
        }

        #endregion
    }
}
