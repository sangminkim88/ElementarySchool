namespace GAIA2020.Design
{
    using GAIA2020.Utilities;
    using HMFrameWork.Ancestor;
    using HMFrameWork.Command;
    using System.Windows.Controls;
    using System.Windows.Input;

    public class GeneralNoteViewModel : AViewModel
    {
        #region Constructors

        public ICommand CommandPrint { get; private set; }

        public GeneralNoteViewModel()
        {
            CommandPrint = new RelayCommand(Print);
        }

        private void Print(object obj)
        {
            var printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                foreach (var item in (obj as WrapPanel).Children)
                {
                    if (item is ZoomContainer)
                    {
                        Image tmpImage = (item as ZoomContainer).MyImage;
                        tmpImage.Height = GaiaConstants.PIXEL_TO_CM_FACTOR * 29.7;
                        tmpImage.Width = GaiaConstants.PIXEL_TO_CM_FACTOR * 21;
                        printDialog.PrintVisual(tmpImage, "GeneralNote Print");
                    }
                }                    
            }
        }

        #endregion

        #region Methods

        public override void BeginInit()
        {
        }

        public override void EndInit()
        {
            App.GetViewModelManager().AddValue(typeof(GeneralNoteViewModel), this);
        }

        #endregion
    }
}
