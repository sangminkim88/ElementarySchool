namespace GAIA2020.Design
{
    using System;
    using HMFrameWork.Ancestor;
    using HMFrameWork.Interface;

    public class InputUIViewModel : AViewModel
    {
        #region Fields

        private IViewModel currentVM;

        #endregion

        #region Constructors

        public InputUIViewModel()
        {
        }

        #endregion

        #region Properties

        public IViewModel CurrentVM
        {
            get { return currentVM; }
            set
            {
                SetValue(ref currentVM, value);
            }
        }
        
        #endregion

        #region Methods

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            var viewModelManager = App.GetViewModelManager();
            viewModelManager.AddValue(typeof(InputUIViewModel), this);
            this.CurrentVM = viewModelManager.GetValue(typeof(APartDescViewModel)) as APartDescViewModel;
        }

        #endregion
    }
}
