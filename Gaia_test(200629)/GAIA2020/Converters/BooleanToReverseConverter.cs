﻿namespace GAIA2020.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    /// <summary>
    /// Defines the <see cref="BooleanToReverseConverter" />.
    /// </summary>
    public class BooleanToReverseConverter : IValueConverter
    {
        #region Methods

        /// <summary>
        /// The Convert.
        /// </summary>
        /// <param name="value">The value<see cref="object"/>.</param>
        /// <param name="targetType">The targetType<see cref="Type"/>.</param>
        /// <param name="parameter">The parameter<see cref="object"/>.</param>
        /// <param name="culture">The culture<see cref="CultureInfo"/>.</param>
        /// <returns>The <see cref="object"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return !(bool)value;
            }
            return false;
        }

        /// <summary>
        /// The ConvertBack.
        /// </summary>
        /// <param name="value">The value<see cref="object"/>.</param>
        /// <param name="targetType">The targetType<see cref="Type"/>.</param>
        /// <param name="parameter">The parameter<see cref="object"/>.</param>
        /// <param name="culture">The culture<see cref="CultureInfo"/>.</param>
        /// <returns>The <see cref="object"/>.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }

        #endregion
    }
}