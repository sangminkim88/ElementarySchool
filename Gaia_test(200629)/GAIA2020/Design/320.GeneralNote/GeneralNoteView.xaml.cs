namespace GAIA2020.Design
{
    using GAIA2020.Utilities;
    using HMFrameWork.Ancestor;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Defines the <see cref="GeneralNoteView" />.
    /// </summary>
    public partial class GeneralNoteView : AUserControl
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralNoteView"/> class.
        /// </summary>
        public GeneralNoteView()
        {
            InitializeComponent();
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
            App.GetViewManager().AddValue(typeof(GeneralNoteView), this);
        }

        /// <summary>
        /// The Panel_Loaded.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void Panel_Loaded(object sender, RoutedEventArgs e)
        {
            this.listFormat.IsChecked = true;
        }

        /// <summary>
        /// The RadioButton_Checked.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton checkedRadioButton = sender as RadioButton;
            double width;
            if (checkedRadioButton.Name.Equals("listFormat"))
            {
                panel.Columns = 1;
                width = this.scrollView.ActualWidth;
            }
            else
            {
                panel.Columns = 2;
                width = this.scrollView.ActualWidth / 2;
            }
            foreach (var item in this.panel.Children)
            {
                if (item is ZoomContainer zoomContainer)
                {
                    zoomContainer.Width = width;
                    zoomContainer.slider.Value = 1;
                }
            }
        }

        /// <summary>
        /// The ZoomContainer_PreviewMouseWheel.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="MouseWheelEventArgs"/>.</param>
        private void ZoomContainer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers.Equals(ModifierKeys.Control))
                return;
            int count = e.Delta / 120;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    this.scrollView.LineUp();
                }
            }
            else
            {

                for (int i = 0; i < -count; i++)
                {
                    this.scrollView.LineDown();
                }
            }
            e.Handled = true;
        }

        #endregion
    }
}
