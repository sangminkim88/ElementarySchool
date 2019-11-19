namespace Common.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    public class WaterMarkTextBox : TextBox
    {
        #region Fields

        public static readonly DependencyProperty WaterMarkProperty;

        #endregion

        #region Constructors

        static WaterMarkTextBox()
        {
            Type ownerType = typeof(WaterMarkTextBox);
            WaterMarkProperty = DependencyProperty.Register(nameof(WaterMark), typeof(string), ownerType, new PropertyMetadata(null));
        }

        #endregion

        #region Properties

        public string WaterMark
        {
            get { return (string)GetValue(WaterMarkProperty); }
            set { SetValue(WaterMarkProperty, value); }
        }

        #endregion
    }
}
