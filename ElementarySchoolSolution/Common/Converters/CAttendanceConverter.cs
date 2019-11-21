namespace Common.Converters
{
    using Common.Enums;
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;

    public class CAttendanceConverter : IValueConverter
    {
        #region Methods
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EAttendance tmp = (EAttendance)value;
            BitmapImage returnData;

            switch (value)
            {
                case EAttendance.결석:
                    returnData = new BitmapImage(new Uri("/Common;component/Images/redCircle.png", UriKind.Relative));
                    break;
                case EAttendance.지각:
                    returnData = new BitmapImage(new Uri("/Common;component/Images/yellowCircle.png", UriKind.Relative));
                    break;
                case EAttendance.조퇴:
                    returnData = new BitmapImage(new Uri("/Common;component/Images/greenCircle.png", UriKind.Relative));
                    break;
                case EAttendance.현장학습:
                    returnData = new BitmapImage(new Uri("/Common;component/Images/blueCircle.png", UriKind.Relative));
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
