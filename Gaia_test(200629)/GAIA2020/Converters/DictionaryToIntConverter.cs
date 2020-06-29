namespace GAIA2020.Converters
{
    using GAIA2020.Utilities;
    using GaiaDB.Enums;
    using System;
    using System.Globalization;
    using System.Windows.Data;

    internal class DictionaryToIntConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableDictionary<eDepartment, int> List = value as ObservableDictionary<eDepartment, int>;
            var tmp = Enum.Parse(typeof(eDepartment), parameter.ToString());

            if (tmp == null)
            {
                return 0;
            }
            else
            {
                return List[(eDepartment)tmp];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        #endregion
    }
}
