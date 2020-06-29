namespace GAIA2020.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for UserControl1.xaml.
    /// </summary>
    public partial class WatermarkTextbox : TextBox
    {
        #region Fields

        /// <summary>
        /// Defines the WaterMarkProperty.
        /// </summary>
        public static readonly DependencyProperty WaterMarkProperty;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="WatermarkTextbox"/> class.
        /// </summary>
        static WatermarkTextbox()
        {
            Type ownerType = typeof(WatermarkTextbox);
            WaterMarkProperty = DependencyProperty.Register(nameof(WaterMark), typeof(string), ownerType, new PropertyMetadata(null));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WatermarkTextbox"/> class.
        /// </summary>
        public WatermarkTextbox()
        {
            InitializeComponent();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the WaterMark.
        /// </summary>
        public string WaterMark
        {
            get { return (string)GetValue(WaterMarkProperty); }
            set { SetValue(WaterMarkProperty, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The PART_Text_IsVisibleChanged.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="DependencyPropertyChangedEventArgs"/>.</param>
        private void PART_Text_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Focusable)
            {
                textBox.Focus();
            }
        }

        #endregion
    }
}
