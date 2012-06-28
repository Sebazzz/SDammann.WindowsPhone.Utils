namespace SDammann.Utils.Windows.Data {
    using System;
    using System.Globalization;
    using System.Windows.Data;


    /// <summary>
    /// Converts a value to a other value by multiplying or dividing it by the value passed to the parameter
    /// </summary>
    public sealed class DoubleIntegerConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            CultureInfo formatProvider = CultureInfo.CurrentCulture;

            int multiplier = 1;
            IConvertible param = parameter as IConvertible;
            if (param != null) {
                multiplier = param.ToInt32(CultureInfo.CurrentCulture);
            }

            IConvertible val = value as IConvertible;
            if (val != null) {
                if (targetType == typeof(Double)) {
                    return val.ToDouble(formatProvider) * multiplier;
                }

                if (targetType == typeof(Int32)) {
                    return val.ToInt32(formatProvider) * multiplier;
                }

                if (targetType == typeof(Single)) {
                    return val.ToSingle(formatProvider) * multiplier;
                }

                return val.ToType(targetType, formatProvider);
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  CultureInfo culture) {
            CultureInfo formatProvider = CultureInfo.CurrentCulture;

            int multiplier = 1;
            IConvertible param = parameter as IConvertible;
            if (param != null) {
                multiplier = param.ToInt32(CultureInfo.CurrentCulture);
            }

            IConvertible val = value as IConvertible;
            if (val != null) {
                if (targetType == typeof(Double)) {
                    return val.ToDouble(formatProvider) / multiplier;
                }

                if (targetType == typeof(Int32)) {
                    return val.ToInt32(formatProvider) / multiplier;
                }

                if (targetType == typeof(Single)) {
                    return val.ToSingle(formatProvider) / multiplier;
                }
                
                return val.ToType(targetType, formatProvider);
            }

            return value;
        }

        #endregion
    }
}