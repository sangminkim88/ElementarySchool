namespace GAIA2020.Manager
{
    using GAIA2020.Design;
    using HMFrameWork.Ancestor;
    using HMFrameWork.Manager;
    using System;

    /// <summary>
    /// Defines the <see cref="ViewManager" />.
    /// </summary>
    public class ViewManager : BaseManager<AUserControl>
    {
        #region Fields

        /// <summary>
        /// Defines the _boringfillView.
        /// </summary>
        private BoringFillView _boringfillView;

        /// <summary>
        /// Defines the _generalNoteView.
        /// </summary>
        private GeneralNoteView _generalNoteView;

        /// <summary>
        /// Defines the _locationView.
        /// </summary>
        private LocationView _locationView;

        /// <summary>
        /// Defines the _logView.
        /// </summary>
        private LogView _logView;

        /// <summary>
        /// Defines the _printViewView.
        /// </summary>
        private PrintView _printView;

        /// <summary>
        /// Defines the aPartDescView.
        /// </summary>
        private APartDescView aPartDescView;

        /// <summary>
        /// Defines the b1PartDescView.
        /// </summary>
        private B1PartDescView b1PartDescView;

        /// <summary>
        /// Defines the b2PartDescView.
        /// </summary>
        private B2PartDescView b2PartDescView;

        /// <summary>
        /// Defines the b3PartDescView.
        /// </summary>
        private B3PartDescView b3PartDescView;

        /// <summary>
        /// Defines the boringBorrowPitView.
        /// </summary>
        private BoringBorrowPitView boringBorrowPitView;

        /// <summary>
        /// Defines the boringBridgeViewModel.
        /// </summary>
        private BoringBridgeView boringBridgeViewModel;

        /// <summary>
        /// Defines the boringCutView.
        /// </summary>
        private BoringCutView boringCutView;

        /// <summary>
        /// Defines the boringTunnelView.
        /// </summary>
        private BoringTunnelView boringTunnelView;

        /// <summary>
        /// Defines the boringView.
        /// </summary>
        private BoringView boringView;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the APartDescView.
        /// </summary>
        public APartDescView APartDescView
        {
            get
            {
                return GetValue(typeof(APartDescView)) as APartDescView;
            }
            set
            {
                aPartDescView = value;
            }
        }

        /// <summary>
        /// Gets or sets the B1PartDescView.
        /// </summary>
        public B1PartDescView B1PartDescView
        {
            get
            {
                return GetValue(typeof(B1PartDescView)) as B1PartDescView;
            }
            set
            {
                b1PartDescView = value;
            }
        }

        /// <summary>
        /// Gets or sets the B2PartDescView.
        /// </summary>
        public B2PartDescView B2PartDescView
        {
            get
            {
                return GetValue(typeof(B2PartDescView)) as B2PartDescView;
            }
            set
            {
                b2PartDescView = value;
            }
        }

        /// <summary>
        /// Gets or sets the B3PartDescView.
        /// </summary>
        public B3PartDescView B3PartDescView
        {
            get
            {
                return GetValue(typeof(B3PartDescView)) as B3PartDescView;
            }
            set
            {
                b3PartDescView = value;
            }
        }

        /// <summary>
        /// Gets or sets the BoringBorrowPitView.
        /// </summary>
        public BoringBorrowPitView BoringBorrowPitView
        {
            get
            {
                return GetValue(typeof(BoringBorrowPitView)) as BoringBorrowPitView;
            }
            set
            {
                boringBorrowPitView = value;
            }
        }

        /// <summary>
        /// Gets or sets the BoringBridgeView.
        /// </summary>
        public BoringBridgeView BoringBridgeView
        {
            get
            {
                return GetValue(typeof(BoringBridgeView)) as BoringBridgeView;
            }
            set
            {
                boringBridgeViewModel = value;
            }
        }

        /// <summary>
        /// Gets or sets the BoringCutView.
        /// </summary>
        public BoringCutView BoringCutView
        {
            get
            {
                return GetValue(typeof(BoringCutView)) as BoringCutView;
            }
            set
            {
                boringCutView = value;
            }
        }

        /// <summary>
        /// Gets or sets the boringfillView.
        /// </summary>
        public BoringFillView boringfillView
        {
            get
            {
                Type type = typeof(BoringFillView);
                _boringfillView = GetValue(type) as BoringFillView;
                return _boringfillView;
            }
            set
            {
                _boringfillView = value;
            }
        }

        /// <summary>
        /// Gets or sets the BoringTunnelView.
        /// </summary>
        public BoringTunnelView BoringTunnelView
        {
            get
            {
                return GetValue(typeof(BoringTunnelView)) as BoringTunnelView;
            }
            set
            {
                boringTunnelView = value;
            }
        }

        /// <summary>
        /// Gets or sets the BoringView.
        /// </summary>
        public BoringView BoringView
        {
            get
            {
                return GetValue(typeof(BoringView)) as BoringView;
            }
            set
            {
                boringView = value;
            }
        }

        /// <summary>
        /// Gets or sets the generalNoteView.
        /// </summary>
        public GeneralNoteView generalNoteView
        {
            get
            {
                return GetValue(typeof(GeneralNoteView)) as GeneralNoteView;
            }
            set
            {
                _generalNoteView = value;
            }
        }

        /// <summary>
        /// Gets or sets the locationView.
        /// </summary>
        public LocationView locationView
        {
            get
            {
                return GetValue(typeof(LocationView)) as LocationView;
            }
            set
            {
                _locationView = value;
            }
        }

        /// <summary>
        /// Gets or sets the logView.
        /// </summary>
        public LogView logView
        {
            get
            {
                return GetValue(typeof(LogView)) as LogView;
            }
            set
            {
                _logView = value;
            }
        }

        /// <summary>
        /// Gets or sets the printView.
        /// </summary>
        public PrintView printView
        {
            get
            {
                return GetValue(typeof(PrintView)) as PrintView;
            }
            set
            {
                _printView = value;
            }
        }

        #endregion
    }
}
