namespace GAIA2020.Design
{
    using HMFrameWork.Ancestor;
    using HMFrameWork.Command;
    using System.Windows.Input;

    public partial class LocationViewModel : AViewModel
    {

        #region Command
        ICommand commandSelect;
        #endregion

        #region Property
        public ICommand CommandSelect
        {
            get
            {
                return commandSelect ?? (commandSelect = new RelayCommand(CommandSelectExecute));
            }
        }
        #endregion

        #region Command Execute
        private void CommandSelectExecute(object parameter)
        {
            string strCmd = parameter.ToString();
            int nIndex = strCmd.IndexOf("cmd=");
            if (nIndex < 0) return;
            string strKey = strCmd.Substring(nIndex+4);
            if (strKey == "none")
            { m_ucModelView.Set_WebImage(false); }
            else if(strKey == "ngii_Map")
            { m_ucModelView.Set_WebImage(true, HmTileMap.TypeMapView.NGII_GENERAL, true); }
            else if (strKey == "daum_Map")
            { m_ucModelView.Set_WebImage(true, HmTileMap.TypeMapView.DAUM_GENERAL, true); }
            else if (strKey == "daum_Sky")
            { m_ucModelView.Set_WebImage(true, HmTileMap.TypeMapView.DAUM_SKYVIEW, true); }
        }
        #endregion


        #region Constructors

        public LocationViewModel()
        {
        }

        #endregion

        #region Methods

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            App.GetViewModelManager().AddValue(typeof(LocationViewModel), this);
        }

        #endregion

        public System.Drawing.Bitmap RenderToBitmap(int nWidth = 0, int nHeight = 0)
        {
            return m_ucModelView.RenderToBitmap(nWidth, nHeight);
        }
    }
}
