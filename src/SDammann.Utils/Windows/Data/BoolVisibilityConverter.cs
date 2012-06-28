namespace SDammann.Utils.Windows.Data {
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Converter support for conversion from boolean to visibility. <c>true</c> is converted 
    /// to <see cref="Visibility.Visible"/> and <c>false</c> is converted to <see cref="Visibility.Collapsed"/>
    /// and vice versa.
    /// </summary>
    public sealed class BoolVisibilityConverter : IValueConverter {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            bool boolValue = System.Convert.ToBoolean(value);

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            Visibility v = (Visibility) value;

            return v == Visibility.Visible;
        }

        #endregion
    }
}