using GaiaDB;
using GaiaDB.Enums;
using HMFrameWork.Ancestor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAIA2020.Design
{
    public class LogViewModel : AViewModel
    {
        public LogViewModel()
        {

        }

        #region 로그스타일
        public eDepartment CurrLogStyle
        {
            get
            {
                return DBDoc.Get_CurrDoc().Get_ActiveStyle();
            }
        }

        public void SetCurrLogStyle(eDepartment e)
        {
            var mngr = App.GetViewManager();
            BoringView vv = mngr.GetValue(typeof(BoringView), false) as BoringView;
            vv?.LoadStyle(e);
        }
        #endregion

        #region ISupportInitialize 인터페이스
        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            App.GetViewModelManager().AddValue(typeof(LogViewModel), this);            
        }
        #endregion

    }
}
