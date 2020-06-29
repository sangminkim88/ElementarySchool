namespace GAIA2020.Frame
{
    using GAIA2020.Design;
    using GAIA2020.Menu;
    using HMFrameWork.Ancestor;
    using HMFrameWork.Command;
    using HMFrameWork.Interface;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    /// <summary>
    /// Defines the <see cref="MainFrameViewModel" />.
    /// </summary>
    public class MainFrameViewModel : AViewModel
    {
        #region Fields

        /// <summary>
        /// Defines the _commandChangeWorkProcess.
        /// </summary>
        private ICommand _commandChangeWorkProcess;

        /// <summary>
        /// Defines the _CurrViewModel.
        /// </summary>
        private IViewModel _CurrViewModel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainFrameViewModel"/> class.
        /// </summary>
        public MainFrameViewModel()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the CommandChangeWorkProcess.
        /// </summary>
        public ICommand CommandChangeWorkProcess
        {
            get
            {
                return _commandChangeWorkProcess ?? (_commandChangeWorkProcess = new RelayCommand(ChangeWorkProcessExecute, CanChangeWorkProcessExecute));
            }
        }

        /// <summary>
        /// Gets the CommandPrint.
        /// </summary>
        public ICommand CommandPrint { get; private set; }

        /// <summary>
        /// Gets or sets the CurrentVM.
        /// </summary>
        public IViewModel CurrentVM
        {
            get { return _CurrViewModel; }
            set
            {
                SetValue(ref _CurrViewModel, value);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The BeginInit.
        /// </summary>
        public override void BeginInit()
        {
        }

        /// <summary>
        /// The EndInit.
        /// </summary>
        public override void EndInit()
        {
            //App.GetViewModelManager().AddValue(typeof(MainFrameViewModel), this);

            // 초기화면
            var mngr = App.GetViewModelManager();
            // 자신을 추가한다
            mngr.AddValue(typeof(MainFrameViewModel), this);
            //
            CurrentVM = mngr.GetValue(typeof(LogViewModel));
        }

        /// <summary>
        /// The CanChangeWorkProcessExecute.
        /// </summary>
        /// <param name="parameter">The parameter<see cref="object"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool CanChangeWorkProcessExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// The ChangeWorkProcessExecute.
        /// </summary>
        /// <param name="parameter">The parameter<see cref="object"/>.</param>
        private void ChangeWorkProcessExecute(object parameter)
        {
            if (parameter != null)
            {

            }
        }

        #endregion
    }
}
