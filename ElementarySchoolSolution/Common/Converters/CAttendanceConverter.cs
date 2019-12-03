namespace Common.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public class CAttendanceConverter : IValueConverter
    {
        #region Methods
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EAttendance tmp = (EAttendance)value;
            SolidColorBrush returnData;

            switch (value)
            {             
                case EAttendance.결석:
                    returnData = Brushes.Red;
                    break;
                case EAttendance.지각:
                    returnData = Brushes.Yellow;
                    break;
                case EAttendance.조퇴:
                    returnData = Brushes.Green;
                    break;
                case EAttendance.현장학습:
                    returnData = Brushes.Blue;
                    break;
                default:
                    returnData = null;
                    break;
            }
            return returnData;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        #endregion
    }
}
