namespace GAIA2020.Manager
{
    using GAIA2020.Design;
    using GAIA2020.Frame;
    using GAIA2020.Menu;
    using HMFrameWork.Ancestor;
    using HMFrameWork.Manager;

    /// <summary>
    /// Defines the <see cref="ViewModelManager" />.
    /// </summary>
    public class ViewModelManager : BaseManager<AViewModel>
    {
        #region Properties

        /// <summary>
        /// Gets the APartDescViewModel.
        /// </summary>
        public APartDescViewModel APartDescViewModel { get => GetValue(typeof(APartDescViewModel)) as APartDescViewModel; }

        /// <summary>
        /// Gets the B1PartDescViewModel.
        /// </summary>
        public B1PartDescViewModel B1PartDescViewModel { get => GetValue(typeof(B1PartDescViewModel)) as B1PartDescViewModel; }

        /// <summary>
        /// Gets the B2PartDescViewModel.
        /// </summary>
        public B2PartDescViewModel B2PartDescViewModel { get => GetValue(typeof(B2PartDescViewModel)) as B2PartDescViewModel; }

        /// <summary>
        /// Gets the B3PartDescViewModel.
        /// </summary>
        public B3PartDescViewModel B3PartDescViewModel { get => GetValue(typeof(B3PartDescViewModel)) as B3PartDescViewModel; }

        /// <summary>
        /// Gets the BoringViewModel.
        /// </summary>
        public BoringViewModel BoringViewModel { get => GetValue(typeof(BoringViewModel)) as BoringViewModel; }

        /// <summary>
        /// Gets the InputUIViewModel.
        /// </summary>
        public InputUIViewModel InputUIViewModel { get => GetValue(typeof(InputUIViewModel)) as InputUIViewModel; }

        /// <summary>
        /// Gets the LocationViewModel.
        /// </summary>
        public LocationViewModel LocationViewModel { get => GetValue(typeof(LocationViewModel)) as LocationViewModel; }

        /// <summary>
        /// Gets the MainFrameViewModel.
        /// </summary>
        public MainFrameViewModel MainFrameViewModel { get => GetValue(typeof(MainFrameViewModel)) as MainFrameViewModel; }

        /// <summary>
        /// Gets the MenuViewModel.
        /// </summary>
        public MenuViewModel MenuViewModel { get => GetValue(typeof(MenuViewModel)) as MenuViewModel; }

        /// <summary>
        /// Gets the UcJointShapeViewModel.
        /// </summary>
        public UcJointShapeViewModel UcJointShapeViewModel { get => GetValue(typeof(UcJointShapeViewModel)) as UcJointShapeViewModel; }

        #endregion
    }
}
