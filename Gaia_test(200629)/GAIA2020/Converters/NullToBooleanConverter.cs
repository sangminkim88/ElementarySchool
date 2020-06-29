namespace GAIA2020.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class NullToBooleanConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            else
            {
                if (value is bool)
                {
                    return (bool)value;
                }
                else
                {
                    return true;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }

        #endregion
    }
}
