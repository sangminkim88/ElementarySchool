using HmDataDocument;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GAIA2020.Design
{
    using GAIA2020.Models;
    using GaiaDB;

    public partial class PrjListViewModel : IDBUpdate
    {
        #region DBUpdate

        public void DBUpdate(TransactionData trData, bool bUndo)
        {
            if (trData.state == TRANSACTION_STATE.NEW || trData.state == TRANSACTION_STATE.OPEN)
            { DBUpdate_All(); }
            else if (trData.state == TRANSACTION_STATE.UPDATE)
            { }
            else if (trData.state == TRANSACTION_STATE.SAVE)
            {
                if (SelectedData.Name.Equals("+추가"))
                    return;

                DBDoc doc = DBDoc.Get_CurrDoc();
                DBDataHINFO hinfoD = null;
                if (doc.hinfo.Get_Data(ref hinfoD))
                {
                    ProjectModel pm = createProjectModel(hinfoD);
                    SelectedData.Fill = pm.Fill;
                    SelectedData.Bridge = pm.Bridge;
                    SelectedData.Cut = pm.Cut;
                    SelectedData.Tunnel = pm.Tunnel;
                    SelectedData.BorrowPit = pm.BorrowPit;
                    SelectedData.HandAuger = pm.HandAuger;
                    SelectedData.Name = pm.Name;
                    SelectedData.Order = pm.Order;
                    SelectedData.Thumbnail = pm.Thumbnail;
                    SelectedData.TrialPit = pm.TrialPit;
                }
            }
            else if (trData.state == TRANSACTION_STATE.USER)
            { }
        }

        public bool Is_Exist()
        {
            return true;
        }

        public void DBUpdate_All()
        {}

        #endregion DBUpdate
    }
}
