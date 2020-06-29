namespace GAIA2020.Design
{
    using HMFrameWork.Ancestor;

    public class BoringTunnelViewModel : AViewModel
    {
        #region Constructors

        public BoringTunnelViewModel()
        {
        }

        #endregion

        #region Methods

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            App.GetViewModelManager().AddValue(typeof(BoringTunnelViewModel), this);
        }

        #endregion
    }
}
