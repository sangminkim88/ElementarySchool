namespace GAIA2020.Menu
{
    using GAIA2020.Design;
    using GAIA2020.Frame;
    using GaiaDB;
    using GaiaDB.Enums;
    using HMFrameWork.Helper;
    using System;
    using System.Windows.Input;

    using HmGeometry;

    /// <summary>
    /// Defines the <see cref="MenuViewModel" />.
    /// </summary>
    partial class MenuViewModel
    {
        #region Fields

        /// <summary>
        /// Defines the currentMenu.
        /// </summary>
        private string currentMenu;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the CommandCheck.
        /// </summary>
        public ICommand CommandCheck { get; private set; }

        /// <summary>
        /// Gets the CommandOthers.
        /// </summary>
        public ICommand CommandOthers { get; private set; }

        #endregion

        #region Methods
        /// <summary>
        /// The Check.
        /// </summary>
        /// <param name="obj">The obj<see cref="object"/>.</param>
        private void check(object obj)
        {
            string menuName = obj.ToString();
            if (menuName.Equals(currentMenu)) return;
            currentMenu = menuName;

            (App.GetViewModelManager().GetValue(typeof(MainFrameViewModel)) as MainFrameViewModel).CurrentVM =
                        App.GetViewModelManager().GetValue(typeof(LogViewModel));            

            eDepartment newMenu = (eDepartment)Enum.Parse(typeof(eDepartment), menuName);
            if (DBDoc.Get_CurrDoc().Get_ActiveStyle() != newMenu)
            {
#if false
                // 스타일 변경
                (WindowHelper.GetTypedWindow(typeof(MainFrameView)) as MainFrameView).iLogStyleChange();
#endif
                DBDoc.Get_CurrDoc().Set_ActiveStyle(newMenu);

                // 1. 입력화면 전면에 표시
                //var mngr = App.GetViewModelManager();
                //InputUIViewModel vm = mngr.GetValue(typeof(InputUIViewModel), false) as InputUIViewModel;
                //vm.CurrentVM = mngr.GetValue(typeof(APartDescViewModel)) as APartDescViewModel;
#if false
                DBDoc doc = DBDoc.Get_CurrDoc();
                HmDBKey actDrlg = doc.Get_ActiveDrillLog();
                if (newMenu == doc.drlg.Get_Department(actDrlg.nKey)) return;

                HmKeyList keyList = new HmKeyList();
                if (doc.drlg.Get_KeyList(keyList, newMenu) == 0) actDrlg.Set("DRLG", 0);
                else actDrlg.Set("DRLG", keyList[0], 1);

                doc.Set_ActiveDrillLog(actDrlg);
#endif
            }
        }

        /// <summary>
        /// The GeneralNote.
        /// </summary>
        /// <param name="obj">The obj<see cref="object"/>.</param>
        private void others(object obj)
        {
            string menuName = obj.ToString();
            // 전역값과 비교
            if (menuName.Equals(currentMenu)) return;
            currentMenu = menuName;

            Type type = null;
            switch (menuName)
            {
                case "Location":
                    type = typeof(LocationViewModel);
                    break;
                case "GeneralNote":
                    type = typeof(GeneralNoteViewModel);
                    break;
                case "Print":
                    type = typeof(PrintViewModel);
                    break;
            }
                (App.GetViewModelManager().GetValue(typeof(MainFrameViewModel)) as MainFrameViewModel).CurrentVM =
                    App.GetViewModelManager().GetValue(type);

        }
#endregion
    }
}
