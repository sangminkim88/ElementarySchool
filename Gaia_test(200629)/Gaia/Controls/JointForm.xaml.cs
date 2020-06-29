namespace Gaia.Controls
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    public partial class JointForm : Grid
    {
        private TextBlock selectedTextBlock;
        #region Constructors

        public JointForm()
        {
            InitializeComponent();
        }

        #endregion

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
    }
}
