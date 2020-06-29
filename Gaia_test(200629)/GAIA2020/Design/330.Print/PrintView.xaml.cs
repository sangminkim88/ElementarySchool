namespace GAIA2020.Design
{
    using HMFrameWork.Ancestor;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Defines the <see cref="PrintView" />.
    /// </summary>
    public partial class PrintView : AUserControl
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintView"/> class.
        /// </summary>
        public PrintView()
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
            App.GetViewManager().AddValue(typeof(PrintView), this);
        }

        /// <summary>
        /// The _MouseEnter.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="MouseEventArgs"/>.</param>
        private void _MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Wait)
            {
                if (sender is Grid)
                {
                    Mouse.OverrideCursor = Cursors.Hand;
                }
                else
                {
                    Mouse.OverrideCursor = Cursors.SizeWE;
                }
            }
        }

        /// <summary>
        /// The _MouseLeave.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="MouseEventArgs"/>.</param>
        private void _MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Wait)
            {
                Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// The DxfButton_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.Windows.RoutedEventArgs"/>.</param>
        private void DxfButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        /// <summary>
        /// The PdfButton_Click.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="System.Windows.RoutedEventArgs"/>.</param>
        private void PdfButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        #endregion
    }
}
