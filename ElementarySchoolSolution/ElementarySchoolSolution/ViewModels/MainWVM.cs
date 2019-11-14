namespace TestSolution.ViewModels
{
    using System.Windows;
    using WpfBase.Bases;
    using WpfBase.Common;
    using WpfBase.Managers;

    public class MainWVM : ViewModelBase
    {
        private Visibility isMenuShow;

        public Visibility IsMenuShow
        {
            get { return isMenuShow; }
            set { SetValue(ref isMenuShow , value); }
        }

        


        public MainWVM()
        {

        }

        /// <summary>
        /// The showMenuCommand field
        /// </summary>
        private RelayCommand showMenu;

        /// <summary>
        /// Gets the ShowMenuCommand
        /// </summary>
        public RelayCommand ShowMenu
        {
            get
            {
                if (this.showMenu == null)
                {
                    this.showMenu = new RelayCommand(command => this.ExecuteShowMenu());
                }

                return this.showMenu;
            }
        }

        /// <summary>
        /// Executes ShowMenu
        /// </summary>
        private void ExecuteShowMenu()
        {
            this.IsMenuShow = this.isMenuShow.Equals(Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            ViewModelManager.AddValue(typeof(MainWVM), this);
        }



    }
}
