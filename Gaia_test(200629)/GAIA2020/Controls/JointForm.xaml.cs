namespace GAIA2020.Controls
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    public partial class JointForm : UserControl
    {
        #region Fields

        private TextBlock selectedTextBlock;

        #endregion

        #region Constructors

        public JointForm()
        {
            InitializeComponent();
        }

        #endregion

        #region Methods

        private void TextBlock_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            if (this.selectedTextBlock == null)
            {
                textBlock.Background = Brushes.DarkSalmon;
                this.selectedTextBlock = textBlock;
            }
            else
            {
                this.selectedTextBlock.Background = Brushes.Transparent;
                if (textBlock.Equals(this.selectedTextBlock))
                {
                    this.selectedTextBlock = null;
                }
                else
                {
                    this.selectedTextBlock = textBlock;
                    this.selectedTextBlock.Background = Brushes.DarkSalmon;
                }
            }
        }

        #endregion
    }
}
