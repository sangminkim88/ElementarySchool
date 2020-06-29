namespace GAIA2020.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

    /// <summary>
    /// Defines the <see cref="ControlTitle" />.
    /// </summary>
    public partial class SimpleInputTextBlock : UserControl
    {
        #region Fields

        /// <summary>
        /// Defines the ButtonContentProperty.
        /// </summary>
        public static readonly DependencyProperty ButtonContentProperty =
           DependencyProperty.Register("ButtonContent", typeof(object), typeof(SimpleInputTextBlock), new UIPropertyMetadata(default(object), buttonContentChanged));

        /// <summary>
        /// Defines the CustomCommandParameterProperty.
        /// </summary>
        public static readonly DependencyProperty CustomCommandParameterProperty =
           DependencyProperty.Register("CustomCommandParameter", typeof(string), typeof(SimpleInputTextBlock), new UIPropertyMetadata(default(string)));

        /// <summary>
        /// Defines the CustomCommandProperty.
        /// </summary>
        public static readonly DependencyProperty CustomCommandProperty =
           DependencyProperty.Register("CustomCommand", typeof(ICommand), typeof(SimpleInputTextBlock), new UIPropertyMetadata(default(ICommand)));

        /// <summary>
        /// Defines the DataProperty.
        /// </summary>
        public static readonly DependencyProperty DataProperty =
           DependencyProperty.Register("Data", typeof(string), typeof(SimpleInputTextBlock), new UIPropertyMetadata(default(string), dataChanged));

        /// <summary>
        /// Defines the FixedTextProperty.
        /// </summary>
        public static readonly DependencyProperty FixedTextProperty =
           DependencyProperty.Register("FixedText", typeof(string), typeof(SimpleInputTextBlock), new UIPropertyMetadata(default(string), fixedTextChanged));

        /// <summary>
        /// Defines the WatermarkProperty.
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty =
           DependencyProperty.Register("Watermark", typeof(string), typeof(SimpleInputTextBlock), new UIPropertyMetadata(default(string), watermarkChanged));

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleInputTextBlock"/> class.
        /// </summary>
        public SimpleInputTextBlock()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        /// <summary>
        /// Defines the ButtonClicked.
        /// </summary>
        public event EventHandler ButtonClicked;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the ButtonContent.
        /// </summary>
        public object ButtonContent
        {
            get { return (object)GetValue(ButtonContentProperty); }
            set { SetValue(ButtonContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether CustomCommand.
        /// </summary>
        public bool CustomCommand
        {
            get { return (bool)GetValue(CustomCommandProperty); }
            set { SetValue(CustomCommandProperty, value); }
        }

        /// <summary>
        /// Gets or sets the CustomCommandParameter.
        /// </summary>
        public string CustomCommandParameter
        {
            get { return (string)GetValue(CustomCommandParameterProperty); }
            set { SetValue(CustomCommandParameterProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Data.
        /// </summary>
        public string Data
        {
            get { return (string)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        /// <summary>
        /// Gets or sets the FixedText.
        /// </summary>
        public string FixedText
        {
            get { return (string)GetValue(FixedTextProperty); }
            set { SetValue(FixedTextProperty, value); }
        }

        /// <summary>
        /// Gets or sets the Watermark.
        /// </summary>
        public string Watermark
        {
            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The buttonContentChanged.
        /// </summary>
        /// <param name="d">The d<see cref="DependencyObject"/>.</param>
        /// <param name="e">The e<see cref="DependencyPropertyChangedEventArgs"/>.</param>
        private static void buttonContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SimpleInputTextBlock simpleInputTextBlock = d as SimpleInputTextBlock;
            simpleInputTextBlock.button.Content = e.NewValue;
        }

        /// <summary>
        /// The dataChanged.
        /// </summary>
        /// <param name="d">The d<see cref="DependencyObject"/>.</param>
        /// <param name="e">The e<see cref="DependencyPropertyChangedEventArgs"/>.</param>
        private static void dataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SimpleInputTextBlock simpleInputTextBlock = d as SimpleInputTextBlock;
            simpleInputTextBlock.watermarkText.Text = e.NewValue.ToString();
            simpleInputTextBlock.CustomCommandParameter = simpleInputTextBlock.FixedText + e.NewValue;
        }

        /// <summary>
        /// The fixedTextChanged.
        /// </summary>
        /// <param name="d">The d<see cref="DependencyObject"/>.</param>
        /// <param name="e">The e<see cref="DependencyPropertyChangedEventArgs"/>.</param>
        private static void fixedTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SimpleInputTextBlock simpleInputTextBlock = d as SimpleInputTextBlock;
            simpleInputTextBlock.fixedText.Text = e.NewValue.ToString();
        }

        /// <summary>
        /// The watermarkChanged.
        /// </summary>
        /// <param name="d">The d<see cref="DependencyObject"/>.</param>
        /// <param name="e">The e<see cref="DependencyPropertyChangedEventArgs"/>.</param>
        private static void watermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SimpleInputTextBlock simpleInputTextBlock = d as SimpleInputTextBlock;
            simpleInputTextBlock.watermarkText.WaterMark = e.NewValue.ToString();
        }

        /// <summary>
        /// The buttonClick.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="RoutedEventArgs"/>.</param>
        private void buttonClick(object sender, RoutedEventArgs e)
        {
            if (ButtonClicked != null)
            {
                ButtonClicked(this, e);
            }
        }

        /// <summary>
        /// The WatermarkText_KeyUp.
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/>.</param>
        /// <param name="e">The e<see cref="KeyEventArgs"/>.</param>
        private void WatermarkText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                this.button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                this.button.Command.Execute(this.CustomCommandParameter);
            }
        }

        #endregion
    }
}
