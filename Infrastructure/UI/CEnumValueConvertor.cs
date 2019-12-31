using System;
using System.Globalization;
using System.Windows.Data;

namespace Infrastructure.UI
{
    public class CEnumValueConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;
            try
            {
                var enumParameter = (Enum)Enum.Parse(value.GetType(), parameter.ToString());
                return ((Enum)value).HasFlag(enumParameter);
            }
            catch
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;
            try
            {
                var returnEnum = Enum.Parse(targetType, parameter.ToString());
                return returnEnum;
            }
            catch
            {
                throw new ArgumentException($"EnumValueConvertor parameter '{parameter}' undefined");
            }
        }
    }
}
