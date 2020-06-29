namespace GAIA2020.Design
{
    using GaiaDB;
    using GaiaDB.Enums;
    using HMFrameWork.Ancestor;
    using System;
    using System.Windows.Media;

    public partial class InputUIView : AUserControl, ILogStyleChanged
    {
        #region Constructors

        public InputUIView()
        {
            InitializeComponent();
        }

        #endregion

        #region 로그스타일 인터페이스
        public void iLogStyleChange()
        {
            // 로그스타일 변경으로 인한 작업
            LoadStyle();
        }
        #endregion

        #region 로그스타일에 맞춰 스타일 및 파일생성
        public eDepartment CurrLogStyle
        {
            get
            {
                return DBDoc.Get_CurrDoc().Get_ActiveStyle();
            }
        }

        public bool LoadStyle()
        {
            Type type = null;
            switch (CurrLogStyle)
            {
                case eDepartment.Fill:
                case eDepartment.Bridge:
                case eDepartment.Cut:
                case eDepartment.Tunnel:
                    type = typeof(APartDescViewModel);
                    break;
                case eDepartment.TrialPit:
                case eDepartment.HandAuger:
                    type = typeof(B3PartDescViewModel);
                    break;
            }
            if (type == null)
            {
                return false;
            }
            else
            {
                ((InputUIViewModel)this.DataContext).CurrentVM = App.GetViewModelManager().GetValue(type);
                return true;
            }
        }
        #endregion

        #region Methods

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            App.GetViewManager().AddValue(typeof(InputUIView), this);
        }

        #endregion
    }
}
